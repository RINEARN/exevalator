# Exevalator Ver.2.2.3 - by RINEARN 2021-2025
# This software is released under the "Unlicense" license.
# You can choose the "CC0" license instead, if you want.

from __future__ import annotations
from dataclasses import dataclass
from typing import List,Dict,Set
from typing import Optional
from enum import Enum, auto
from abc import ABC, abstractmethod
import os
import re

# !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
# To change the language of error messages,
# copy the contents of ERROR_MESSAGES_*.py and 
# overwrite the ErrorMessage class below with them.
# !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

class ErrorMessages:
    """
    Error messages for ExevalatorException.

    You can customize messages by editing or overriding these class attributes.
    Placeholders `$0`, `$1`, ... are replaced by positional arguments during formatting.
    """

    EMPTY_EXPRESSION = "The inputted expression is empty."
    TOO_MANY_TOKENS = "The number of tokens exceeds the limit (StaticSettings.MAX_TOKEN_COUNT: '$0')"
    DEFICIENT_OPEN_PARENTHESIS = "The number of open parentheses '(' is deficient."
    DEFICIENT_CLOSED_PARENTHESIS = "The number of closed parentheses ')' is deficient."
    EMPTY_PARENTHESIS = "The content of parentheses '()' should not be empty."
    RIGHT_OPERAND_REQUIRED = "An operand is required at the right of: '$0'"
    LEFT_OPERAND_REQUIRED = "An operand is required at the left of: '$0'"
    RIGHT_OPERATOR_REQUIRED = "An operator is required at the right of: '$0'"
    LEFT_OPERATOR_REQUIRED = "An operator is required at the left of: '$0'"
    UNKNOWN_UNARY_PREFIX_OPERATOR = "Unknown unary-prefix operator: '$0'"
    UNKNOWN_BINARY_OPERATOR = "Unknown binary operator: '$0'"
    UNKNOWN_OPERATOR_SYNTAX = "Unknown operator syntax: '$0'"
    EXCEEDS_MAX_AST_DEPTH = "The depth of the AST exceeds the limit (StaticSettings.MAX_AST_DEPTH: '$0')"
    UNEXPECTED_PARTIAL_EXPRESSION = "Unexpected end of a partial expression"
    INVALID_NUMBER_LITERAL = "Invalid number literal: '$0'"
    INVALID_MEMORY_ADDRESS = "Invalid memory address: '$0'"
    FUNCTION_ERROR = "Function Error ('$0'): $1"
    VARIABLE_NOT_FOUND = "Variable not found: '$0'"
    FUNCTION_NOT_FOUND = "Function not found: '$0'"
    UNEXPECTED_OPERATOR = "Unexpected operator: '$0'"
    UNEXPECTED_TOKEN = "Unexpected token: '$0'"
    TOO_LONG_EXPRESSION = "The length of the expression exceeds the limit (StaticSettings.MAX_EXPRESSION_CHAR_COUNT: '$0')"
    UNEXPECTED_ERROR = "Unexpected error occurred: $0"
    REEVAL_NOT_AVAILABLE = '"reeval" is not available before using "eval"'
    TOO_LONG_VARIABLE_NAME = "The length of the variable name exceeds the limit (StaticSettings.MAX_NAME_CHAR_COUNT: '$0')"
    TOO_LONG_FUNCTION_NAME = "The length of the function name exceeds the limit (StaticSettings.MAX_NAME_CHAR_COUNT: '$0')"
    VARIABLE_ALREADY_DECLARED = "The variable '$0' is already declared"
    FUNCTION_ALREADY_CONNECTED = "The function '$0' is already connected"
    INVALID_VARIABLE_ADDRESS = "Invalid memory address: '$0'"


class Exevalator:
    """
    Exevalator interpreter engine.
    
    Core evaluator class. See README for details.
    """


    def __init__(self) -> None:
        # The array used as a virtual memory storing values of variables.
        self.memory: List[float] = [0.0] * 64
        # self.memory: array = array('d')

        # The current usage (max used index + 1) of the memory.
        self.memoryUsage: int = 0

        # The object evaluating the value of the expression.
        self.evaluator: Evaluator = Evaluator()

        # The Map mapping each variable name to an address of the variable.
        self.variableTable: Dict[str, int] = {}

        # The Map mapping each function name to a FunctionInterface instance.
        self.functionTable: Dict[str, FunctionInterface] = {}

        # Caches the content of the expression evaluated last time, to skip re-parsing.
        self.lastEvaluatedExpression: Optional[str] = None


    def eval(self, expression: str) -> float:
        """
        Evaluates (computes) the value of an expression.

        Args:
            expression (str): The expression to evaluate (e.g., "1.2 + 3.4").

        Returns:
            float: The computed value (double-precision).

        Examples:
            >>> ex = Exevalator()
            >>> ex.eval("1.2 + 3.4")
            4.6
        """

        if expression is None:  # type: ignore[unreachable]
            raise ExevalatorException(
                ErrorMessages.UNEXPECTED_ERROR.replace("$0", "Null expression")
            )

        if StaticSettings.MAX_EXPRESSION_CHAR_COUNT < len(expression):
            raise ExevalatorException(
                ErrorMessages.TOO_LONG_EXPRESSION.replace(
                    "$0", str(StaticSettings.MAX_EXPRESSION_CHAR_COUNT)
                )
            )

        try:
            expressionChanged = (
                self.lastEvaluatedExpression is None
                or expression is not self.lastEvaluatedExpression # comparing references
                and expression != self.lastEvaluatedExpression    # comparing values
            )

            # If the expression changed from the last-evaluated expression, re-parsing is necessary.
            if expressionChanged or not self.evaluator.isEvaluatable():

                # Tokenize & analyze
                tokens = LexicalAnalyzer.analyze(expression)

                # Dump tokens
                # for token in tokens:
                #     print(token)

                # Parse -> AST
                ast = Parser.parse(tokens)

                # Dump AST
                # print(ast.toMarkuppedText())

                # Update evaluator with current symbol tables
                self.evaluator.update(ast, self.variableTable, self.functionTable)

                # Cache
                self.lastEvaluatedExpression = expression

            # Evaluate
            return self.evaluator.evaluate(self.memory)

        except ExevalatorException:
            # Re-throw library exception as-is
            raise
        except Exception as e:
            # Wrap unexpected exceptions
            raise ExevalatorException(
                ErrorMessages.UNEXPECTED_ERROR.replace("$0", str(e))
            ) from e


    def reeval(self) -> float:
        """
        Re-evaluates the value of the expression evaluated by `eval` last time.
        May be slightly faster than calling `eval` repeatedly for the same expression.

        Note:
            The result may differ from the last value if variables/functions changed.
        """
        if self.evaluator.isEvaluatable():
            return self.evaluator.evaluate(self.memory)
        raise ExevalatorException(ErrorMessages.REEVAL_NOT_AVAILABLE)


    def declareVariable(self, name: str) -> int:
        """
        Declares a new variable, for using the value of it in expressions.

        Args:
            name (str): The name of the variable to be declared.

        Returns:
            int: The virtual address of the declared variable.
        """
        if name is None:  # type: ignore[unreachable]
            raise ExevalatorException("Null variable name")

        if StaticSettings.MAX_NAME_CHAR_COUNT < len(name):
            raise ExevalatorException(
                ErrorMessages.TOO_LONG_VARIABLE_NAME.replace(
                    "$0", str(StaticSettings.MAX_NAME_CHAR_COUNT)
                )
            )
        if name in self.variableTable:
            raise ExevalatorException(
                ErrorMessages.VARIABLE_ALREADY_DECLARED.replace("$0", name)
            )

        # Expand memory if full (double size, Java と同じノリ)
        if len(self.memory) == self.memoryUsage:
            stock = self.memory[:]
            self.memory = [0.0] * (len(stock) * 2)
            self.memory[: len(stock)] = stock

        # Assign address & register
        address = self.memoryUsage
        self.variableTable[name] = address
        self.memoryUsage += 1
        return address


    def writeVariable(self, name: str, value: float) -> None:
        """
        Writes the value to the variable having the specified name.

        Args:
            name (str): The name of the variable to be written.
            value (float): The new value of the variable.
        """
        if name is None:  # type: ignore[unreachable]
            raise ExevalatorException("Null variable name")

        if (
            StaticSettings.MAX_NAME_CHAR_COUNT < len(name)
            or name not in self.variableTable
        ):
            raise ExevalatorException(
                ErrorMessages.VARIABLE_NOT_FOUND.replace("$0", name)
            )
        address = self.variableTable[name]
        self.writeVariableAt(address, value)


    def writeVariableAt(self, address: int, value: float) -> None:
        """
        Writes the value to the variable at the specified virtual address.
        This method is more efficient than `writeVariable`.

        Args:
            address (int): The virtual address of the variable to be written.
            value (float): The new value of the variable.
        """
        if address < 0 or self.memoryUsage <= address:
            raise ExevalatorException(
                ErrorMessages.INVALID_VARIABLE_ADDRESS.replace("$0", str(address))
            )
        self.memory[address] = value


    def readVariable(self, name: str) -> float:
        """
        Reads the value of the variable having the specified name.

        Args:
            name (str): The name of the variable to be read.

        Returns:
            float: The current value of the variable.
        """
        if name is None:  # type: ignore[unreachable]
            raise ExevalatorException("Null variable name")

        if (
            StaticSettings.MAX_NAME_CHAR_COUNT < len(name)
            or name not in self.variableTable
        ):
            raise ExevalatorException(
                ErrorMessages.VARIABLE_NOT_FOUND.replace("$0", name)
            )
        address = self.variableTable[name]
        return self.readVariableAt(address)


    def readVariableAt(self, address: int) -> float:
        """
        Reads the value of the variable at the specified virtual address.
        This method is more efficient than `readVariable`.

        Args:
            address (int): The virtual address of the variable to be read.

        Returns:
            float: The current value of the variable.
        """
        if address < 0 or self.memoryUsage <= address:
            raise ExevalatorException(
                ErrorMessages.INVALID_VARIABLE_ADDRESS.replace("$0", str(address))
            )
        return self.memory[address]


    def connectFunction(self, name: str, function: FunctionInterface) -> None:
        """
        Connects a function, for using it in expressions.

        Args:
            name (str): The name of the function used in the expression.
            function (FunctionInterface): The function to be connected.
        """
        if name is None or function is None:  # type: ignore[unreachable]
            raise ExevalatorException("Null function name or instance")

        if StaticSettings.MAX_NAME_CHAR_COUNT < len(name):
            raise ExevalatorException(
                ErrorMessages.TOO_LONG_FUNCTION_NAME.replace(
                    "$0", str(StaticSettings.MAX_NAME_CHAR_COUNT)
                )
            )
        if name in self.functionTable:
            raise ExevalatorException(
                ErrorMessages.FUNCTION_ALREADY_CONNECTED.replace("$0", name)
            )
        self.functionTable[name] = function


class FunctionInterface(ABC):
    """The interface to implement functions available in expressions."""

    @abstractmethod
    def invoke(self, arguments: List[float]) -> float:
        """
        Invokes the function.

        Args:
            arguments: An array storing values of arguments.

        Returns:
            The return value of the function.
        """
        raise NotImplementedError


class ExevalatorException(Exception):
    """
    Base exception thrown by the Exevalator engine.

    Create with a message, and optionally chain a cause exception.
    """

    def __init__(self, message: str) -> None:
        super().__init__(message)


class LexicalAnalyzer:
    """
    The class performing functions of a lexical analyzer.
    """

    @staticmethod
    def analyze(expression: str) -> List[Token]:
        """
        Splits (tokenizes) the expression into tokens, and analyze them.

        Args:
            expression (str): The expression to be tokenized/analyzed.

        Returns:
            List[Token]: Analyzed tokens.

        Raises:
            ExevalatorException: If the expression is empty or exceeds limits, etc.
        """

        # Firstly, to simplify the tokenization,
        # replace number literals in the expression to the escaped representation: "@NUMBER_LITERAL",
        # because number literals may contain "+" or "-" in their exponent part.
        numberLiteralList: List[str] = []
        expression = LexicalAnalyzer.escapeNumberLiterals(expression, numberLiteralList)

        # Tokenize (split) the expression into token words.
        for splitter in StaticSettings.TOKEN_SPLITTER_SYMBOL_LIST:
            expression = expression.replace(str(splitter), f" {splitter} ")
        tokenWords: List[str] = expression.strip().split()

        # Empty expression detection (the case: tokenWords == [])
        if not tokenWords:
            raise ExevalatorException(ErrorMessages.EMPTY_EXPRESSION)

        # Checks the total number of tokens.
        if len(tokenWords) > StaticSettings.MAX_TOKEN_COUNT:
            limit = str(StaticSettings.MAX_TOKEN_COUNT)
            error_message = ErrorMessages.TOO_MANY_TOKENS.replace("$0", limit)
            raise ExevalatorException(error_message)

        # Create Token instances.
        # Also, escaped number literals will be recovered.
        tokens: List[Token] = LexicalAnalyzer.createTokensFromTokenWords(
            tokenWords, numberLiteralList
        )

        # Checks syntactic correctness of tokens of inputted expressions.
        LexicalAnalyzer.checkParenthesisBalance(tokens)
        LexicalAnalyzer.checkEmptyParentheses(tokens)
        LexicalAnalyzer.checkLocationsOfOperatorsAndLeafs(tokens)

        return tokens


    # ------------------------
    # Below: not implemented yet (移植しながら埋める)
    # ------------------------

    @staticmethod
    def escapeNumberLiterals(expression: str, numberLiteralList: List[str]) -> str:
        """
        Replace number literals with an escaped marker and collect originals.

        Args:
            expression (str): The raw expression string.
            numberLiteralList (List[str]): A list to append detected number literals in order.

        Returns:
            The expression with all number literals replaced by ESCAPED_NUMBER_LITERAL.
        """
        pattern = StaticSettings.NUMBER_LITERAL_REGEX_COMPILED

        # Detect all number literals and append them to numberLiteralList (in order).
        numberLiteralList.extend(m.group(0) for m in pattern.finditer(expression))

        # Replace all number literals in the expression with the marker "@NUMBER_LITERAL@".
        replaced = pattern.sub(StaticSettings.ESCAPED_NUMBER_LITERAL, expression)

        return replaced

    @staticmethod
    def createTokensFromTokenWords(tokenWords: List[str],
                                   numberLiteralList: List[str]) -> List["Token"]:
        """
        Convert token words into Token instances.
        Also recover escaped number literals.
        """
        tokenCount = len(tokenWords)

        # Stores the parenthesis-depth, which will increase at "(" and decrease at ")".
        parenthesisDepth: int = 0

        # Stores the parenthesis-depth where a function call begins,
        # for detecting the end of the function operator.
        callParenthesisDepths: Set[int] = set()

        tokens: List[Token] = [None] * tokenCount  # type: ignore[list-item]
        lastToken: Token | None = None
        iliteral: int = 0
        
        for itoken in range(tokenCount):
            word = tokenWords[itoken]

            # Cases of open parentheses, or beginning of function calls.
            if word == "(":
                parenthesisDepth += 1
                if itoken >= 1 and tokens[itoken - 1] is not None and tokens[itoken - 1].type == TokenType.FUNCTION_IDENTIFIER:
                    # this '(' is a call-begin operator
                    callParenthesisDepths.add(parenthesisDepth)
                    op = StaticSettings.CALL_OPERATOR_SYMBOL_DICT[word]  # '('
                    tokens[itoken] = Token(TokenType.OPERATOR, word, op)
                else:
                    # this '(' is an open parenthesis
                    tokens[itoken] = Token(TokenType.PARENTHESIS, word)

            # Cases of close parentheses, or end of function calls.
            elif word == ")":
                if parenthesisDepth in callParenthesisDepths:
                    # this ')' is a call-end operator
                    callParenthesisDepths.remove(parenthesisDepth)
                    op = StaticSettings.CALL_OPERATOR_SYMBOL_DICT[word]  # ')'
                    tokens[itoken] = Token(TokenType.OPERATOR, word, op)
                else:
                    # this ')' is an close parenthesis
                    tokens[itoken] = Token(TokenType.PARENTHESIS, word)
                parenthesisDepth -= 1

            # Case of separators of function arguments (treated as special operator).
            elif word == ",":
                op = StaticSettings.CALL_OPERATOR_SYMBOL_DICT[word]
                tokens[itoken] = Token(TokenType.OPERATOR, word, op)

            # Cases of other operators.
            elif len(word) == 1 and word in StaticSettings.OPERATOR_SYMBOL_SET:
                op = None

                # Unary-prefix operators:
                if (lastToken is None
                    or lastToken.word == "("
                    or lastToken.word == ","
                    or (lastToken.type == TokenType.OPERATOR and lastToken.operator is not None and lastToken.operator.type != OperatorType.CALL)):
                    if word not in StaticSettings.UNARY_PREFIX_OPERATOR_SYMBOL_DICT:
                        mesasge = ErrorMessages.UNKNOWN_UNARY_PREFIX_OPERATOR.replace("$0", word)
                        raise ExevalatorException(mesasge)
                    op = StaticSettings.UNARY_PREFIX_OPERATOR_SYMBOL_DICT[word]

                # Binary operators:
                elif (lastToken.word == ")"
                    or lastToken.type == TokenType.NUMBER_LITERAL
                    or lastToken.type == TokenType.VARIABLE_IDENTIFIER):
                    if word not in StaticSettings.BINARY_OPERATOR_SYMBOL_DICT:
                        mesasge = ErrorMessages.UNKNOWN_BINARY_OPERATOR.replace("$0", word)
                        raise ExevalatorException(mesasge)
                    op = StaticSettings.BINARY_OPERATOR_SYMBOL_DICT[word]

                else:
                    mesasge = ErrorMessages.UNKNOWN_OPERATOR_SYNTAX.replace("$0", word)
                    raise ExevalatorException(mesasge)

                tokens[itoken] = Token(type=TokenType.OPERATOR, word=word, operator=op)

            # Case of number literals (escaped marker)
            elif word == StaticSettings.ESCAPED_NUMBER_LITERAL:
                try:
                    literal = numberLiteralList[iliteral]
                except IndexError as ie:
                    message = ErrorMessages.UNEXPECTED_ERROR.replace("$0", "literal index out of range")
                    raise ExevalatorException(message) from ie
                tokens[itoken] = Token(type=TokenType.NUMBER_LITERAL, word=literal)
                iliteral += 1

            # Cases of variable identifier or function identifier
            else:
                if itoken < tokenCount - 1 and tokenWords[itoken + 1] == "(":
                    tokens[itoken] = Token(TokenType.FUNCTION_IDENTIFIER, word)
                else:
                    tokens[itoken] = Token(TokenType.VARIABLE_IDENTIFIER, word)

            lastToken = tokens[itoken]

        # If unrecovered number literals exist: error
        if iliteral != len(numberLiteralList):
            message = ErrorMessages.UNEXPECTED_ERROR.replace("$0", "unrecovered number literals detected")
            raise ExevalatorException(message)

        return tokens  # type: ignore[return-value]


    @staticmethod
    def checkParenthesisBalance(tokens: List[Token]) -> None:
        """
        Check if the numbers of '(' and ')' are balanced.

        An ExevalatorException will be raised when any errors detected.
        If no error detected, nothing will occur.

        Args:
            tokens (List[Token]): Tokens of the inputted expression.
        """
        hierarchy = 0  # Increases at "(" and decreases at ")".

        for token in tokens:
            if token.word == "(":
                hierarchy += 1
            elif token.word == ")":
                hierarchy -= 1

            # If the value of hierarchy is negative, the open parenthesis is deficient.
            if hierarchy < 0:
                raise ExevalatorException(ErrorMessages.DEFICIENT_OPEN_PARENTHESIS)

        # If the value of hierarchy is not zero at the end of the expression,
        # the closed parentheses ")" is deficient.
        if hierarchy > 0:
            raise ExevalatorException(ErrorMessages.DEFICIENT_CLOSED_PARENTHESIS)


    @staticmethod
    def checkEmptyParentheses(tokens: List[Token]) -> None:
        """
        Check if there exists an empty '()'.

        Function-call parentheses (CALL operators) are excluded from this check.

        An ExevalatorException will be raised when any errors detected.
        If no error detected, nothing will occur.

        Args:
            tokens (List[Token]): Tokens of the inputted expression.
        """
        tokenCount = len(tokens)
        contentCounter = 0
        for tokenIndex in range(tokenCount):
            token = tokens[tokenIndex]
            if token.type == TokenType.PARENTHESIS:  # Excepting CALL operators
                if token.word == "(":
                    contentCounter = 0
                elif token.word == ")":
                    if contentCounter == 0:
                        raise ExevalatorException(ErrorMessages.EMPTY_PARENTHESIS)
            else:
                contentCounter += 1

    @staticmethod
    def checkLocationsOfOperatorsAndLeafs(tokens: List[Token]) -> None:
        """
        Checks correctness of locations of operators and leaf elements (literals and identifiers).
        An ExevalatorException will be thrown when any errors detected.
        If no error detected, nothing will occur.

        Args:
            tokens (List[Token]): Tokens of the inputted expression.
        """
        tokenCount = len(tokens)
        leafTypeSet = {TokenType.NUMBER_LITERAL, TokenType.VARIABLE_IDENTIFIER}

        # Reads and check tokens from left to right.
        for tokenIndex in range(tokenCount):
            token = tokens[tokenIndex]

            # Prepare information of next/previous token.
            nextIsLeaf = (
                tokenIndex != tokenCount - 1
                and tokens[tokenIndex + 1].type in leafTypeSet
            )
            prevIsLeaf = (
                tokenIndex != 0
                and tokens[tokenIndex - 1].type in leafTypeSet
            )
            nextIsOpenParenthesis = (
                tokenIndex < tokenCount - 1
                and tokens[tokenIndex + 1].word == "("
            )
            prevIsCloseParenthesis = (
                tokenIndex != 0
                and tokens[tokenIndex - 1].word == ")"
            )
            nextIsPrefixOperator = (
                tokenIndex < tokenCount - 1
                and tokens[tokenIndex + 1].type == TokenType.OPERATOR
                and tokens[tokenIndex + 1].operator.type == OperatorType.UNARY_PREFIX
            )
            nextIsFunctionCallBegin = (
                nextIsOpenParenthesis
                and tokenIndex < tokenCount - 1
                and tokens[tokenIndex + 1].type == TokenType.OPERATOR
                and tokens[tokenIndex + 1].operator.type == OperatorType.CALL
            )
            nextIsFunctionIdentifier = (
                tokenIndex < tokenCount - 1
                and tokens[tokenIndex + 1].type == TokenType.FUNCTION_IDENTIFIER
            )

            # Case of operators
            if token.type == TokenType.OPERATOR:
                # Cases of unary-prefix operators
                if token.operator is not None and token.operator.type == OperatorType.UNARY_PREFIX:
                    # Only leafs, open parentheses, unary-prefix and function-call operators can be an operand.
                    if not (nextIsLeaf or nextIsOpenParenthesis or nextIsPrefixOperator or nextIsFunctionIdentifier):
                        message = ErrorMessages.RIGHT_OPERAND_REQUIRED.replace("$0", token.word)
                        raise ExevalatorException(message)

                # Cases of binary operators or a separator of partial expressions
                if (token.operator is not None and token.operator.type == OperatorType.BINARY) or token.word == ",":
                    # Only leafs, open parentheses, unary-prefix and function-call operators can be right-operands.
                    if not (nextIsLeaf or nextIsOpenParenthesis or nextIsPrefixOperator or nextIsFunctionIdentifier):
                        message = ErrorMessages.RIGHT_OPERAND_REQUIRED.replace("$0", token.word)
                        raise ExevalatorException(message)
                    # Only leaf elements and closed parenthesis can be a left-operand.
                    if not (prevIsLeaf or prevIsCloseParenthesis):
                        message = ErrorMessages.LEFT_OPERAND_REQUIRED.replace("$0", token.word)
                        raise ExevalatorException(message)

            # Case of leaf elements
            if token.type in leafTypeSet:
                # Another leaf or an open parenthesis cannot be at the right of a leaf element.
                if (not nextIsFunctionCallBegin) and (nextIsOpenParenthesis or nextIsLeaf):
                    message = ErrorMessages.RIGHT_OPERATOR_REQUIRED.replace("$0", token.word)
                    raise ExevalatorException(message)

                # Another leaf element or a closed parenthesis cannot be at the left of a leaf element.
                if prevIsCloseParenthesis or prevIsLeaf:
                    message = ErrorMessages.LEFT_OPERATOR_REQUIRED.replace("$0", token.word)
                    raise ExevalatorException(message)


class Parser:
    """
    The class performing functions of a parser.
    """

    @staticmethod
    def parse(tokens: List[Token]) -> AstNode:
        """
        Parses tokens and construct Abstract Syntax Tree (AST).

        Args:
            tokens: Tokens to be parsed.

        Returns:
            The root node of the constructed AST.
        """

        # Number of tokens
        tokenCount = len(tokens)

        # Working stack to form multiple AstNode instances into a tree-shape.
        stack: list[AstNode] = []

        # Temporary nodes used in the above working stack, for isolating ASTs of partial expressions.
        parenthesisStackLid = AstNode(Token(type=TokenType.STACK_LID, word="(PARENTHESIS_STACK_LID)"))
        separatorStackLid   = AstNode(Token(type=TokenType.STACK_LID, word="(SEPARATOR_STACK_LID)"))
        callBeginStackLid   = AstNode(Token(type=TokenType.STACK_LID, word="(CALL_BEGIN_STACK_LID)"))

        # The array storing next operator's precedence for each token.
        # At [i], it stores the precedence of the first operator whose token-index is greater than i.
        nextOperatorPrecedences = Parser.getNextOperatorPrecedences(tokens)

        # Read tokens from left to right.
        itoken = 0
        while True:  # do { ... } while (itoken < tokenCount);

            token = tokens[itoken]
            operatorNode: AstNode | None = None

            # Case of literals and identifiers: "1.23", "x", "f", etc.
            if token.type in (TokenType.NUMBER_LITERAL, TokenType.VARIABLE_IDENTIFIER, TokenType.FUNCTION_IDENTIFIER):
                stack.append(AstNode(token))
                itoken += 1
                continue

            # Case of parenthesis: "(" or ")"
            elif token.type == TokenType.PARENTHESIS:
                if token.word == "(":
                    stack.append(parenthesisStackLid)
                    itoken += 1
                    continue
                else:  # ")"
                    operatorNode = Parser.popPartialExprNodes(stack, parenthesisStackLid)[0]

            # Case of operators: "+", "-", etc.
            elif token.type == TokenType.OPERATOR:
                operatorNode = AstNode(token)
                nextOpPrecedence = nextOperatorPrecedences[itoken]

                # Case of unary-prefix operators
                if token.operator.type == OperatorType.UNARY_PREFIX:
                    if Parser.shouldAddRightOperand(token.operator.associativity, token.operator.precedence, nextOpPrecedence):
                        operatorNode.childNodeList.append(AstNode(tokens[itoken + 1]))
                        itoken += 1  # looked-ahead

                # Case of binary operators
                elif token.operator.type == OperatorType.BINARY:
                    operatorNode.childNodeList.append(stack.pop())
                    if Parser.shouldAddRightOperand(token.operator.associativity, token.operator.precedence, nextOpPrecedence):
                        operatorNode.childNodeList.append(AstNode(tokens[itoken + 1]))
                        itoken += 1  # looked-ahead

                # Case of function-call operators
                elif token.operator.type == OperatorType.CALL:
                    if token.word == "(":
                        # Add function-identifier node at the top of the stack.
                        operatorNode.childNodeList.append(stack.pop())
                        stack.append(operatorNode)
                        # Marker to collect partial expressions of args from the stack.
                        stack.append(callBeginStackLid)
                        itoken += 1
                        continue
                    elif token.word == ")":
                        argNodes = Parser.popPartialExprNodes(stack, callBeginStackLid)
                        operatorNode = stack.pop()  # the '(' CALL operator node
                        for argNode in argNodes:
                            operatorNode.childNodeList.append(argNode)
                    elif token.word == ",":
                        stack.append(separatorStackLid)
                        itoken += 1
                        continue

            # If the precedence of the operator at the top of the stack is stronger than the next operator,
            # connect all "unconnected yet" operands and operators in the stack.
            while Parser.shouldAddRightOperandToStackedOperator(stack, nextOperatorPrecedences[itoken]):
                oldOperatorNode = operatorNode
                operatorNode = stack.pop()
                operatorNode.childNodeList.append(oldOperatorNode)  # type: ignore[arg-type]

            stack.append(operatorNode)  # type: ignore[arg-type]
            itoken += 1

            # Tail of "do { ... } while (itoken < tokenCount);"
            if not (itoken < tokenCount):
                break
        
        # The AST has been constructed on the stack, and only its root node is stored in the stack.
        rootNodeOfExpressionAst = stack.pop()

        # Check that the depth of the constructed AST does not exceed the limit.
        rootNodeOfExpressionAst.checkDepth(1, StaticSettings.MAX_AST_DEPTH)

        return rootNodeOfExpressionAst


    @staticmethod
    def shouldAddRightOperand(
        targetOperatorAssociativity: OperatorAssociativity,
        targetOperatorPrecedence: int,
        nextOperatorPrecedence: int,
    ) -> bool:
        """
        Judges whether the right-side token should be connected directly as an operand, to the target operator.

        Smaller precedence value means higher priority.
        """
        # If the precedence of the target operator is stronger than the next operator, return true.
        # If the precedence of the next operator is stronger than the target operator, return false.
        # If the precedence of both operators is the same:
        #         Return true if the target operator is left-associative.
        #         Return false if the target operator is right-associative.
        targetOpPrecedenceIsStrong = targetOperatorPrecedence < nextOperatorPrecedence
        targetOpPrecedenceIsEqual = targetOperatorPrecedence == nextOperatorPrecedence
        targetOpAssociativityIsLeft = targetOperatorAssociativity == OperatorAssociativity.LEFT
        return targetOpPrecedenceIsStrong or (targetOpPrecedenceIsEqual and targetOpAssociativityIsLeft)

    @staticmethod
    def shouldAddRightOperandToStackedOperator(
        stack: List[AstNode], nextOperatorPrecedence: int
    ) -> bool:
        """
        Judges whether the right-side token should be connected directly as an operand,
        to the operator at the top of the working stack.
        """
        if len(stack) == 0 or stack[-1].token.type != TokenType.OPERATOR:
            return False
        
        operatorOnStackTop: Operator = stack[-1].token.operator  # type: ignore[assignment]

        return Parser.shouldAddRightOperand(
            operatorOnStackTop.associativity,
            operatorOnStackTop.precedence,
            nextOperatorPrecedence,
        )

    @staticmethod
    def popPartialExprNodes(stack: List[AstNode], endStackLidNode: AstNode) -> List[AstNode]:
        """
        Pops root nodes of ASTs of partial expressions constructed on the stack.
        In the returned list, the popped nodes are stored in FIFO order.
        """
        if len(stack) == 0:
            raise ExevalatorException(ErrorMessages.UNEXPECTED_PARTIAL_EXPRESSION)

        partialExprNodeList: List[AstNode] = []
        while len(stack) != 0:
            if stack[-1].token.type == TokenType.STACK_LID:
                stackLidNode = stack.pop()
                if stackLidNode is endStackLidNode:
                    break
            else:
                partialExprNodeList.append(stack.pop())

        # Store elements in FIFO order and return
        nodeCount = len(partialExprNodeList)
        partialExprNodes: List[AstNode] = [None] * nodeCount  # type: ignore[list-item]
        for inode in range(nodeCount):
            partialExprNodes[inode] = partialExprNodeList[nodeCount - inode - 1]
        return partialExprNodes  # type: ignore[return-value]

    @staticmethod
    def getNextOperatorPrecedences(tokens: List[Token]) -> List[int]:
        """
        Returns an array storing next operator's precedence for each token.
        At index i, it stores the precedence of the first operator whose token-index is greater than i.
        """
        tokenCount = len(tokens)
        lastOperatorPrecedence: int = StaticSettings.LEAST_PRIOR_OPERATOR_PRECEDENCE
        nextOperatorPrecedences: List[int] = [StaticSettings.LEAST_PRIOR_OPERATOR_PRECEDENCE] * tokenCount

        for itoken in range(tokenCount - 1, -1, -1):
        # for (int itoken=tokenCount-1; 0<=itoken; itoken--) {

            token = tokens[itoken]
            nextOperatorPrecedences[itoken] = lastOperatorPrecedence

            if token.type == TokenType.OPERATOR:
                lastOperatorPrecedence = token.operator.precedence  # type: ignore[assignment]

            if token.type == TokenType.PARENTHESIS:
                if token.word == "(":
                    lastOperatorPrecedence = 0  # most prior
                else:  # case of ")"
                    lastOperatorPrecedence = StaticSettings.LEAST_PRIOR_OPERATOR_PRECEDENCE

        return nextOperatorPrecedences

class Evaluator:
    """
    The class for evaluating the value of an AST.
    """

    def __init__(self) -> None:
        # The tree of evaluator nodes, which evaluates an expression.
        self.evaluatorNodeTree: Optional[EvaluatorNode] = None

    def update(
        self,
        ast: "AstNode",
        variableTable: Dict[str, int],
        functionTable: Dict[str, "FunctionInterface"],
    ) -> None:
        """
        Updates the state to evaluate the value of the AST.
        """
        node = Evaluator._createEvaluatorNodeTree(ast, variableTable, functionTable)
        if node is None:
            raise ExevalatorException(
                ErrorMessages.UNEXPECTED_ERROR.replace("$0", "root node has no evaluator")
            )
        self.evaluatorNodeTree = node

    def isEvaluatable(self) -> bool:
        """
        Returns whether `evaluate` method is available on the current state.
        """
        return self.evaluatorNodeTree is not None

    def evaluate(self, memory: List[float]) -> float:
        """
        Evaluates the value of the AST set by `update` method.
        """
        if self.evaluatorNodeTree is None:
            message = ErrorMessages.UNEXPECTED_ERROR.replace("$0", "evaluatorNodeTree has not been initialized yet.")
            raise ExevalatorException(message)
        return self.evaluatorNodeTree.evaluate(memory)

    @staticmethod
    def _createEvaluatorNodeTree(
        ast: "AstNode",
        variableTable: Dict[str, int],
        functionTable: Dict[str, FunctionInterface],
    ) -> Optional[EvaluatorNode]:
        """
        Creates a tree of evaluator nodes corresponding with the specified AST.

        Note: This method creates a tree of evaluator nodes by traversing each node in the AST recursively.
        """
        childNodeList: List["AstNode"] = ast.childNodeList
        childCount = len(childNodeList)

        # NOTE ABOUT FUNCTION_IDENTIFIER -> Optional[EvaluatorNode] (None):
        # We deliberately do NOT create an EvaluatorNode for FUNCTION_IDENTIFIER.
        # In the AST, a function call is modeled as:
        #     CALL("(")  <- root
        #       ├── FUNCTION_IDENTIFIER("f")   <- function name
        #       ├── <arg0>
        #       └── <arg1> ...
        # The evaluator tree keeps the "shape" aligned to the AST, but the function
        # name token has no corresponding evaluator behavior. To avoid inventing a
        # dummy no-op node, we store `None` for FUNCTION_IDENTIFIER and let the CALL
        # operator read the name from `childNodeList[0].token.word`.

        # Creates evaluator nodes of child nodes, and store them into an array.
        childNodeNodes: List[Optional[EvaluatorNode]] = [None] * childCount
        for ichild in range(childCount):
            childAstNode = childNodeList[ichild]
            childNodeNodes[ichild] = Evaluator._createEvaluatorNodeTree(childAstNode, variableTable, functionTable)

        # Initialize evaluator node of THIS node
        token = ast.token
        if token.type == TokenType.NUMBER_LITERAL:
            return NumberLiteralEvaluatorNode(token.word)

        elif token.type == TokenType.VARIABLE_IDENTIFIER:
            if token.word not in variableTable:
                raise ExevalatorException(
                    ErrorMessages.VARIABLE_NOT_FOUND.replace("$0", token.word)
                )
            address = variableTable[token.word]
            return VariableEvaluatorNode(address)

        elif token.type == TokenType.FUNCTION_IDENTIFIER:
            return None  # type: ignore[return-value]

        elif token.type == TokenType.OPERATOR:
            op: "Operator" = token.operator  # type: ignore[assignment]

            if op.type == OperatorType.UNARY_PREFIX and op.symbol == "-":
                return MinusEvaluatorNode(
                    Evaluator._requireNotNone(childNodeNodes[0])
                )

            elif op.type == OperatorType.BINARY and op.symbol == "+":
                return AdditionEvaluatorNode(
                    Evaluator._requireNotNone(childNodeNodes[0]),
                    Evaluator._requireNotNone(childNodeNodes[1])
                )

            elif op.type == OperatorType.BINARY and op.symbol == "-":
                return SubtractionEvaluatorNode(
                    Evaluator._requireNotNone(childNodeNodes[0]),
                    Evaluator._requireNotNone(childNodeNodes[1])
                )

            elif op.type == OperatorType.BINARY and op.symbol == "*":
                return MultiplicationEvaluatorNode(
                    Evaluator._requireNotNone(childNodeNodes[0]),
                    Evaluator._requireNotNone(childNodeNodes[1])
                )

            elif op.type == OperatorType.BINARY and op.symbol == "/":
                return DivisionEvaluatorNode(
                    Evaluator._requireNotNone(childNodeNodes[0]),
                    Evaluator._requireNotNone(childNodeNodes[1])
                )

            elif op.type == OperatorType.CALL and op.symbol == "(":
                identifier = childNodeList[0].token.word  # FUNCTION_IDENTIFIER node
                if identifier not in functionTable:
                    raise ExevalatorException(
                        ErrorMessages.FUNCTION_NOT_FOUND.replace("$0", identifier)
                    )
                function = functionTable[identifier]
                argCount = childCount - 1
                argNodes: List["EvaluatorNode"] = [None] * argCount  # type: ignore[list-item]
                for iarg in range(argCount):
                    # childNodeNodes[1..] are arguments
                    argNodes[iarg] = Evaluator._requireNotNone(childNodeNodes[iarg + 1])
                return FunctionEvaluatorNode(function, identifier, argNodes)

            else:
                raise ExevalatorException(
                    ErrorMessages.UNEXPECTED_OPERATOR.replace("$0", str(op.symbol))
                )

        else:
            raise ExevalatorException(
                ErrorMessages.UNEXPECTED_TOKEN.replace("$0", str(token.type))
            )

    @staticmethod
    def _requireNotNone(node: Optional[EvaluatorNode]) -> EvaluatorNode:
        if node is None:
            raise ExevalatorException(ErrorMessages.UNEXPECTED_ERROR.replace("$0", "internal null node"))
        return node



class OperatorType(Enum):
    """The enum representing types of operators."""

    UNARY_PREFIX = auto() # Represents unary operator, for example: - of -1.23
    BINARY = auto()       # Represents binary operator, for example: + of 1+2
    CALL = auto()         # Represents function-call operator.


class OperatorAssociativity(Enum):
    """The enum representing associativities of operators."""

    LEFT = auto()  # Represents left-associative.
    RIGHT = auto() # Represents right-associative.


@dataclass(frozen=True, kw_only=True)
class Operator:
    """The class storing information of an operator."""

    type: OperatorType  # The type of operator tokens.
    symbol: str         # The symbol of this operator (for example: '+').
    precedence: int     # The precedence of this operator (smaller value gives higher precedence).
    associativity: OperatorAssociativity # The associativity of operator tokens.

    def __post_init__(self):
        if not isinstance(self.type, OperatorType):
            raise ValueError("The type of `type` must be `OperatorType`")
        if not isinstance(self.symbol, str):
            raise ValueError("The type of `symbol` must be `str`")
        if not isinstance(self.precedence, int):
            raise ValueError("The type of `precedence` must be `int`")
        if not isinstance(self.associativity, OperatorAssociativity):
            raise ValueError("The type of `associativity` must be `OperatorAssociativity`")


class TokenType(Enum):
    """The enum representing types of tokens."""

    NUMBER_LITERAL = auto()       # Represents number literal tokens, for example: 1.23
    OPERATOR = auto()             # Represents operator tokens, for example: +
    PARENTHESIS = auto()          # Represents parenthesis, for example: ( and ) of (1*(2+3))
    VARIABLE_IDENTIFIER = auto()  # Represents variable-identifier tokens, for example: x
    FUNCTION_IDENTIFIER = auto()  # Represents function-identifier tokens, for example: f
    STACK_LID = auto()            # Represents temporary token for isolating partial expressions in the stack, in parser


@dataclass(frozen=True, kw_only=True)
class Token:
    """The class storing information of an operator."""

    type: TokenType  # The symbol of this operator (for example: '+').
    word: str        # The precedence of this operator (smaller value gives higher precedence).
    operator: Optional[Operator] = None  # The type of operator tokens.

    def __post_init__(self):
        if not isinstance(self.type, TokenType):
            raise ValueError("The type of `type` must be `TokenType`")
        if not isinstance(self.word, str):
            raise ValueError("The type of `word` must be `str`")
        if self.operator is not None and not isinstance(self.operator, Operator):
            raise ValueError("The type of `word` must be `Operator` or None")

class AstNode:
    """The class storing information of a node of an AST."""

    token: Token                  # The token corresponding with this AST node.
    childNodeList: List[AstNode]  # The list of child nodes of this AST node.

    def __init__(self, token: Token) -> None:
        """
        Create an AST node instance storing specified information.

        Args:
            token: The token corresponding with this AST node.
        """
        self.token = token
        self.childNodeList = []

    def checkDepth(self, depthOfThisNode: int, maxAstDepth: int) -> None:
        """
        Checks that depths under this node do not exceed the specified maximum.

        Args:
            depthOfThisNode: The depth of this node in the AST.
            maxAstDepth: The maximum depth allowed.

        Raises:
            ExevalatorException: If the depth exceeds the maximum value.
        """
        if maxAstDepth < depthOfThisNode:
            message = ErrorMessages.EXCEEDS_MAX_AST_DEPTH.replace(
                "$0", str(StaticSettings.MAX_AST_DEPTH)
            )
            raise ExevalatorException(message)
        for childNode in self.childNodeList:
            childNode.checkDepth(depthOfThisNode + 1, maxAstDepth)

    def toMarkuppedText(self, indentStage: int = 0) -> str:
        """
        Expresses the AST under this node in XML-like text format.

        Args:
            indentStage: The stage of indent of this node.

        Returns:
            XML-like text representation of the AST under this node.
        """
        indent = StaticSettings.AST_INDENT * indentStage
        eol = os.linesep
        parts: List[str] = []

        parts.append(indent)
        parts.append("<")
        parts.append(self.token.type.name)  # Enum名をそのまま出力（Javaと同様）
        parts.append(' word="')
        parts.append(self.token.word)
        parts.append('"')

        if self.token.type == TokenType.OPERATOR and self.token.operator is not None:
            parts.append(' optype="')
            parts.append(self.token.operator.type.name)
            parts.append('" precedence="')
            parts.append(str(self.token.operator.precedence))
            parts.append('"')

        if self.childNodeList:
            parts.append(">")
            for childNode in self.childNodeList:
                parts.append(eol)
                parts.append(childNode.toMarkuppedText(indentStage + 1))
            parts.append(eol)
            parts.append(indent)
            parts.append("</")
            parts.append(self.token.type.name)
            parts.append(">")
        else:
            parts.append(" />")

        return "".join(parts)


class EvaluatorNode(ABC):
    """The super class of evaluator nodes."""

    @abstractmethod
    def evaluate(self, memory: List[float]) -> float:
        """
        Performs the evaluation.

        Args:
            memory: The array storing values of variables.

        Returns:
            The evaluated value.
        """
        raise NotImplementedError


class BinaryOperationEvaluatorNode(EvaluatorNode):
    """The super class of evaluator nodes of binary operations."""

    __slots__ = ("leftOperandNode", "rightOperandNode")

    def __init__(self, leftOperandNode: EvaluatorNode, rightOperandNode: EvaluatorNode) -> None:
        """
        Initializes operands.

        Args:
            leftOperandNode: The node for evaluating the left-side operand.
            rightOperandNode: The node for evaluating the right-side operand.
        """
        self.leftOperandNode = leftOperandNode
        self.rightOperandNode = rightOperandNode


class AdditionEvaluatorNode(BinaryOperationEvaluatorNode):
    """The evaluator node for evaluating the value of an addition operator."""

    def evaluate(self, memory: List[float]) -> float:
        """Performs the addition."""
        return self.leftOperandNode.evaluate(memory) + self.rightOperandNode.evaluate(memory)


class SubtractionEvaluatorNode(BinaryOperationEvaluatorNode):
    """The evaluator node for evaluating the value of a subtraction operator."""

    def evaluate(self, memory: List[float]) -> float:
        """Performs the subtraction."""
        return self.leftOperandNode.evaluate(memory) - self.rightOperandNode.evaluate(memory)


class MultiplicationEvaluatorNode(BinaryOperationEvaluatorNode):
    """The evaluator node for evaluating the value of a multiplication operator."""

    def evaluate(self, memory: List[float]) -> float:
        """Performs the multiplication."""
        return self.leftOperandNode.evaluate(memory) * self.rightOperandNode.evaluate(memory)


class DivisionEvaluatorNode(BinaryOperationEvaluatorNode):
    """The evaluator node for evaluating the value of a division operator."""

    def evaluate(self, memory: List[float]) -> float:
        """Performs the division."""
        return self.leftOperandNode.evaluate(memory) / self.rightOperandNode.evaluate(memory)


class MinusEvaluatorNode(EvaluatorNode):
    """The evaluator node for evaluating the value of a unary-minus operator."""

    __slots__ = ("operandNode",)

    def __init__(self, operandNode: EvaluatorNode) -> None:
        """
        Initializes the operand.

        Args:
            operandNode: The node for evaluating the operand.
        """
        self.operandNode = operandNode

    def evaluate(self, memory: List[float]) -> float:
        """Performs the negation (unary minus)."""
        return -self.operandNode.evaluate(memory)


class NumberLiteralEvaluatorNode(EvaluatorNode):
    """The evaluator node for evaluating the value of a number literal."""

    __slots__ = ("value",)

    def __init__(self, literal: str) -> None:
        """
        Initializes the value of the number literal.

        Args:
            literal: The number literal.
        """
        try:
            self.value = float(literal)
        except ValueError:
            raise ExevalatorException(
                ErrorMessages.INVALID_NUMBER_LITERAL.replace("$0", literal)
            )

    def evaluate(self, memory: List[float]) -> float:
        """Returns the value of the number literal."""
        return self.value


class VariableEvaluatorNode(EvaluatorNode):
    """The evaluator node for evaluating the value of a variable."""

    __slots__ = ("address",)

    def __init__(self, address: int) -> None:
        """
        Initializes the address of the variable.

        Args:
            address: The address of the variable.
        """
        self.address = address

    def evaluate(self, memory: List[float]) -> float:
        """
        Returns the value of the variable.

        Raises:
            ExevalatorException: If the address is out of range.
        """
        if self.address < 0 or len(memory) <= self.address:
            raise ExevalatorException(
                ErrorMessages.INVALID_MEMORY_ADDRESS.replace("$0", str(self.address))
            )
        return memory[self.address]


class FunctionEvaluatorNode(EvaluatorNode):
    """
    The evaluator node for evaluating a function-call operator.
    """

    __slots__ = ("function", "functionName", "argumentEvalNodes", "argumentArrayBuffer")

    def __init__(
        self,
        function: FunctionInterface,
        functionName: str,
        argumentEvalNodes: List[EvaluatorNode],
    ) -> None:
        """
        Initializes information of functions to be called.

        Args:
            function: The function to be called.
            functionName: The name of the function.
            argumentEvalNodes: Evaluator nodes for evaluating values of arguments.
        """
        self.function = function
        self.functionName = functionName
        self.argumentEvalNodes = argumentEvalNodes
        # Preallocate buffer
        self.argumentArrayBuffer = [0.0] * len(self.argumentEvalNodes)

    def evaluate(self, memory: List[float]) -> float:
        """
        Calls the function and returns the returned value of the function.
        """
        argCount = len(self.argumentEvalNodes)
        for iarg in range(argCount):
            self.argumentArrayBuffer[iarg] = self.argumentEvalNodes[iarg].evaluate(memory)
        try:
            return self.function.invoke(self.argumentArrayBuffer)
        except Exception as e:
            msg = (
                ErrorMessages.FUNCTION_ERROR
                .replace("$0", self.functionName)
                .replace("$1", str(e))
            )
            raise ExevalatorException(msg) from e


class StaticSettings:
    """Static setting values."""

    # The maximum number of characters in an expression.
    MAX_EXPRESSION_CHAR_COUNT: int = 256

    # The maximum number of characters of variable/function names.
    MAX_NAME_CHAR_COUNT: int = 64

    # The maximum number of tokens in an expression.
    MAX_TOKEN_COUNT: int = 64

    # The maximum depth of an Abstract Syntax Tree (AST).
    MAX_AST_DEPTH: int = 32

    # The precedence value of least prior operators.
    LEAST_PRIOR_OPERATOR_PRECEDENCE: int = 9223372036854775807

    # The indent used in text representations of ASTs.
    AST_INDENT: str = "  "

    # The regular expression of number literals.
    #   (?<=...) : fixed-width lookbehind (OK in Python's re)
    NUMBER_LITERAL_REGEX: str = (
        r"(?:^|(?<=[\s\+\-\*/\(\),]))"    # token splitters or start
        r"([0-9]+(\.[0-9]+)?)"            # significand
        r"((e|E)(\\+|-)?[0-9]+)?"         # exponent
    )
    NUMBER_LITERAL_REGEX_COMPILED = re.compile(NUMBER_LITERAL_REGEX)

    # NOTE:
    # Python's `re` does not support variable-length lookbehind.
    # In the Java version we used `(?<=...|^)` to mean "start of string OR just after a splitter".
    # That becomes non-fixed-width in Python because `^` is zero-width while the other branch is 1 char.
    # To keep lookbehind fixed-width, we move the start-of-string check outside:
    #     (?:^|(?<=[\s\+\-\*/\(\),])) ...
    # The lookbehind now always matches exactly one character, so the pattern compiles on Python.
    # (Also note: inside a character class [...], keep - escaped or place it at either end
    #  to avoid range syntax, e.g., [-+] or [+\-].)

    # The escaped representation of number literals in expressions.
    ESCAPED_NUMBER_LITERAL: str = "@NUMBER_LITERAL@"

    # --- Operators (same precedence/assoc as Java) ---
    _additionOperator = Operator(
        type=OperatorType.BINARY,
        symbol='+', precedence=400, associativity=OperatorAssociativity.LEFT
    )
    _subtractionOperator = Operator(
        type=OperatorType.BINARY,
        symbol='-', precedence=400, associativity=OperatorAssociativity.LEFT
    )
    _multiplicationOperator = Operator(
        type=OperatorType.BINARY,
        symbol='*', precedence=300, associativity=OperatorAssociativity.LEFT
    )
    _divisionOperator = Operator(
        type=OperatorType.BINARY,
        symbol='/', precedence=300, associativity=OperatorAssociativity.LEFT
    )
    _minusOperator = Operator(
        type=OperatorType.UNARY_PREFIX,
        symbol='-', precedence=200, associativity=OperatorAssociativity.RIGHT
    )
    _callBeginOperator = Operator(
        type=OperatorType.CALL,
        symbol='(', precedence=100, associativity=OperatorAssociativity.LEFT
    )
    _callEndOperator = Operator(
        type=OperatorType.CALL,
        symbol=')', precedence=LEAST_PRIOR_OPERATOR_PRECEDENCE, associativity=OperatorAssociativity.LEFT
    )
    _callSeparatorOperator = Operator(
        type=OperatorType.CALL,
        symbol=',', precedence=LEAST_PRIOR_OPERATOR_PRECEDENCE, associativity=OperatorAssociativity.LEFT
    )

    # The set of symbols of available operators.
    OPERATOR_SYMBOL_SET: Set[str] = {
        '+', '-', '*', '/', '(', ')', ','
    }

    # The Map mapping each symbol of an unary-prefix operator to an instance of Operator class.
    UNARY_PREFIX_OPERATOR_SYMBOL_DICT: Dict[str, Operator] = {
        '-': _minusOperator,
    }

    # The Map mapping each symbol of a binary operator to an instance of Operator class.
    BINARY_OPERATOR_SYMBOL_DICT: Dict[str, Operator] = {
        '+': _additionOperator,
        '-': _subtractionOperator,
        '*': _multiplicationOperator,
        '/': _divisionOperator,
    }

    # The Map mapping each symbol of a call operator to an instance of Operator class.
    CALL_OPERATOR_SYMBOL_DICT: Dict[str, Operator] = {
        '(': _callBeginOperator,
        ')': _callEndOperator,
        ',': _callSeparatorOperator,
    }

    # The list of symbols to split an expression into tokens.
    TOKEN_SPLITTER_SYMBOL_LIST: List[str] = ['+', '-', '*', '/', '(', ')', ',']
