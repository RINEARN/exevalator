# Exevalator Ver.2.3.1 - by RINEARN 2021-2025
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
    REEVAL_NOT_AVAILABLE = "'reeval' is not available before using 'eval'"
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
        self.memory_usage: int = 0

        # The object evaluating the value of the expression.
        self.evaluator: Evaluator = Evaluator()

        # The Map mapping each variable name to an address of the variable.
        self.variable_table: Dict[str, int] = {}

        # The Map mapping each function name to a FunctionInterface instance.
        self.function_table: Dict[str, FunctionInterface] = {}

        # Caches the content of the expression evaluated last time, to skip re-parsing.
        self.last_evaluated_expression: Optional[str] = None


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
                self.last_evaluated_expression is None
                or expression is not self.last_evaluated_expression # comparing references
                and expression != self.last_evaluated_expression    # comparing values
            )

            # If the expression changed from the last-evaluated expression, re-parsing is necessary.
            if expressionChanged or not self.evaluator.is_evaluatable():

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
                self.evaluator.update(ast, self.variable_table, self.function_table)

                # Cache
                self.last_evaluated_expression = expression

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
        if self.evaluator.is_evaluatable():
            return self.evaluator.evaluate(self.memory)
        raise ExevalatorException(ErrorMessages.REEVAL_NOT_AVAILABLE)


    def declare_variable(self, name: str) -> int:
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
        if name in self.variable_table:
            raise ExevalatorException(
                ErrorMessages.VARIABLE_ALREADY_DECLARED.replace("$0", name)
            )

        # Expand memory if full (double size, Java と同じノリ)
        if len(self.memory) == self.memory_usage:
            stock = self.memory[:]
            self.memory = [0.0] * (len(stock) * 2)
            self.memory[: len(stock)] = stock

        # Assign address & register
        address = self.memory_usage
        self.variable_table[name] = address
        self.memory_usage += 1
        return address


    def write_variable(self, name: str, value: float) -> None:
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
            or name not in self.variable_table
        ):
            raise ExevalatorException(
                ErrorMessages.VARIABLE_NOT_FOUND.replace("$0", name)
            )
        address = self.variable_table[name]
        self.write_variable_at(address, value)


    def write_variable_at(self, address: int, value: float) -> None:
        """
        Writes the value to the variable at the specified virtual address.
        This method is more efficient than `write_variable`.

        Args:
            address (int): The virtual address of the variable to be written.
            value (float): The new value of the variable.
        """
        if address < 0 or self.memory_usage <= address:
            raise ExevalatorException(
                ErrorMessages.INVALID_VARIABLE_ADDRESS.replace("$0", str(address))
            )
        self.memory[address] = value


    def read_variable(self, name: str) -> float:
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
            or name not in self.variable_table
        ):
            raise ExevalatorException(
                ErrorMessages.VARIABLE_NOT_FOUND.replace("$0", name)
            )
        address = self.variable_table[name]
        return self.read_variable_at(address)


    def read_variable_at(self, address: int) -> float:
        """
        Reads the value of the variable at the specified virtual address.
        This method is more efficient than `read_variable`.

        Args:
            address (int): The virtual address of the variable to be read.

        Returns:
            float: The current value of the variable.
        """
        if address < 0 or self.memory_usage <= address:
            raise ExevalatorException(
                ErrorMessages.INVALID_VARIABLE_ADDRESS.replace("$0", str(address))
            )
        return self.memory[address]


    def connect_function(self, name: str, function: FunctionInterface) -> None:
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
        if name in self.function_table:
            raise ExevalatorException(
                ErrorMessages.FUNCTION_ALREADY_CONNECTED.replace("$0", name)
            )
        self.function_table[name] = function


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
        number_literal_list: List[str] = []
        expression = LexicalAnalyzer._escape_number_literals(expression, number_literal_list)

        # Tokenize (split) the expression into token words.
        for splitter in StaticSettings.TOKEN_SPLITTER_SYMBOL_LIST:
            expression = expression.replace(str(splitter), f" {splitter} ")
        token_words: List[str] = expression.strip().split()

        # Empty expression detection (the case: token_words == [])
        if not token_words:
            raise ExevalatorException(ErrorMessages.EMPTY_EXPRESSION)

        # Checks the total number of tokens.
        if len(token_words) > StaticSettings.MAX_TOKEN_COUNT:
            limit = str(StaticSettings.MAX_TOKEN_COUNT)
            error_message = ErrorMessages.TOO_MANY_TOKENS.replace("$0", limit)
            raise ExevalatorException(error_message)

        # Create Token instances.
        # Also, escaped number literals will be recovered.
        tokens: List[Token] = LexicalAnalyzer._create_tokens_from_token_words(
            token_words, number_literal_list
        )

        # Checks syntactic correctness of tokens of inputted expressions.
        LexicalAnalyzer._check_parenthesis_balance(tokens)
        LexicalAnalyzer._check_empty_parentheses(tokens)
        LexicalAnalyzer._check_locations_of_operators_and_leafs(tokens)

        return tokens


    @staticmethod
    def _escape_number_literals(expression: str, number_literal_list: List[str]) -> str:
        """
        Replace number literals with an escaped marker and collect originals.

        Args:
            expression (str): The raw expression string.
            number_literal_list (List[str]): A list to append detected number literals in order.

        Returns:
            The expression with all number literals replaced by ESCAPED_NUMBER_LITERAL.
        """
        pattern = StaticSettings.NUMBER_LITERAL_REGEX_COMPILED

        # Detect all number literals and append them to number_literal_list (in order).
        number_literal_list.extend(m.group(0) for m in pattern.finditer(expression))

        # Replace all number literals in the expression with the marker "@NUMBER_LITERAL@".
        replaced = pattern.sub(StaticSettings.ESCAPED_NUMBER_LITERAL, expression)

        return replaced


    @staticmethod
    def _create_tokens_from_token_words(token_words: List[str],
                                   number_literal_list: List[str]) -> List[Token]:
        """
        Convert token words into Token instances.
        Also recover escaped number literals.
        """
        token_count = len(token_words)

        # Stores the parenthesis-depth, which will increase at "(" and decrease at ")".
        parenthesis_depth: int = 0

        # Stores the parenthesis-depth where a function call begins,
        # for detecting the end of the function operator.
        callparenthesis_depths: Set[int] = set()

        tokens: List[Token] = [None] * token_count  # type: ignore[list-item]
        last_token: Token | None = None
        iliteral: int = 0
        
        for itoken in range(token_count):
            word = token_words[itoken]

            # Cases of open parentheses, or beginning of function calls.
            if word == "(":
                parenthesis_depth += 1
                if itoken >= 1 and tokens[itoken - 1] is not None and tokens[itoken - 1].type == TokenType.FUNCTION_IDENTIFIER:
                    # this '(' is a call-begin operator
                    callparenthesis_depths.add(parenthesis_depth)
                    op = StaticSettings.CALL_OPERATOR_SYMBOL_DICT[word]  # '('
                    tokens[itoken] = Token(type=TokenType.OPERATOR, word=word, operator=op)
                else:
                    # this '(' is an open parenthesis
                    tokens[itoken] = Token(type=TokenType.PARENTHESIS, word=word, operator=None)

            # Cases of close parentheses, or end of function calls.
            elif word == ")":
                if parenthesis_depth in callparenthesis_depths:
                    # this ')' is a call-end operator
                    callparenthesis_depths.remove(parenthesis_depth)
                    op = StaticSettings.CALL_OPERATOR_SYMBOL_DICT[word]  # ')'
                    tokens[itoken] = Token(type=TokenType.OPERATOR, word=word, operator=op)
                else:
                    # this ')' is an close parenthesis
                    tokens[itoken] = Token(type=TokenType.PARENTHESIS, word=word, operator=None)
                parenthesis_depth -= 1

            # Case of separators of function arguments (treated as special operator).
            elif word == ",":
                op = StaticSettings.CALL_OPERATOR_SYMBOL_DICT[word]
                tokens[itoken] = Token(type=TokenType.OPERATOR, word=word, operator=op)

            # Cases of other operators.
            elif len(word) == 1 and word in StaticSettings.OPERATOR_SYMBOL_SET:
                op = None

                # Unary-prefix operators:
                if (last_token is None
                    or last_token.word == "("
                    or last_token.word == ","
                    or (last_token.type == TokenType.OPERATOR and last_token.operator is not None and last_token.operator.type != OperatorType.CALL)):
                    if word not in StaticSettings.UNARY_PREFIX_OPERATOR_SYMBOL_DICT:
                        mesasge = ErrorMessages.UNKNOWN_UNARY_PREFIX_OPERATOR.replace("$0", word)
                        raise ExevalatorException(mesasge)
                    op = StaticSettings.UNARY_PREFIX_OPERATOR_SYMBOL_DICT[word]

                # Binary operators:
                elif (last_token.word == ")"
                    or last_token.type == TokenType.NUMBER_LITERAL
                    or last_token.type == TokenType.VARIABLE_IDENTIFIER):
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
                    literal = number_literal_list[iliteral]
                except IndexError as ie:
                    message = ErrorMessages.UNEXPECTED_ERROR.replace("$0", "literal index out of range")
                    raise ExevalatorException(message) from ie
                tokens[itoken] = Token(type=TokenType.NUMBER_LITERAL, word=literal)
                iliteral += 1

            # Cases of variable identifier or function identifier
            else:
                if itoken < token_count - 1 and token_words[itoken + 1] == "(":
                    tokens[itoken] = Token(type=TokenType.FUNCTION_IDENTIFIER, word=word, operator=None)
                else:
                    tokens[itoken] = Token(type=TokenType.VARIABLE_IDENTIFIER, word=word, operator=None)

            last_token = tokens[itoken]

        # If unrecovered number literals exist: error
        if iliteral != len(number_literal_list):
            message = ErrorMessages.UNEXPECTED_ERROR.replace("$0", "unrecovered number literals detected")
            raise ExevalatorException(message)

        return tokens  # type: ignore[return-value]


    @staticmethod
    def _check_parenthesis_balance(tokens: List[Token]) -> None:
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
    def _check_empty_parentheses(tokens: List[Token]) -> None:
        """
        Check if there exists an empty '()'.

        Function-call parentheses (CALL operators) are excluded from this check.

        An ExevalatorException will be raised when any errors detected.
        If no error detected, nothing will occur.

        Args:
            tokens (List[Token]): Tokens of the inputted expression.
        """
        token_count = len(tokens)
        content_counter = 0
        for token_index in range(token_count):
            token = tokens[token_index]
            if token.type == TokenType.PARENTHESIS:  # Excepting CALL operators
                if token.word == "(":
                    content_counter = 0
                elif token.word == ")":
                    if content_counter == 0:
                        raise ExevalatorException(ErrorMessages.EMPTY_PARENTHESIS)
            else:
                content_counter += 1

    @staticmethod
    def _check_locations_of_operators_and_leafs(tokens: List[Token]) -> None:
        """
        Checks correctness of locations of operators and leaf elements (literals and identifiers).
        An ExevalatorException will be thrown when any errors detected.
        If no error detected, nothing will occur.

        Args:
            tokens (List[Token]): Tokens of the inputted expression.
        """
        token_count = len(tokens)
        leaf_type_set = {TokenType.NUMBER_LITERAL, TokenType.VARIABLE_IDENTIFIER}

        # Reads and check tokens from left to right.
        for token_index in range(token_count):
            token = tokens[token_index]

            # Prepare information of next/previous token.
            next_is_leaf = (
                token_index != token_count - 1
                and tokens[token_index + 1].type in leaf_type_set
            )
            prev_is_leaf = (
                token_index != 0
                and tokens[token_index - 1].type in leaf_type_set
            )
            next_is_open_parenthesis = (
                token_index < token_count - 1
                and tokens[token_index + 1].word == "("
            )
            prev_is_close_parenthesis = (
                token_index != 0
                and tokens[token_index - 1].word == ")"
            )
            next_is_prefix_operator = (
                token_index < token_count - 1
                and tokens[token_index + 1].type == TokenType.OPERATOR
                and tokens[token_index + 1].operator.type == OperatorType.UNARY_PREFIX
            )
            next_is_function_call_begin = (
                next_is_open_parenthesis
                and token_index < token_count - 1
                and tokens[token_index + 1].type == TokenType.OPERATOR
                and tokens[token_index + 1].operator.type == OperatorType.CALL
            )
            next_is_function_identifier = (
                token_index < token_count - 1
                and tokens[token_index + 1].type == TokenType.FUNCTION_IDENTIFIER
            )

            # Case of operators
            if token.type == TokenType.OPERATOR:
                # Cases of unary-prefix operators
                if token.operator is not None and token.operator.type == OperatorType.UNARY_PREFIX:
                    # Only leafs, open parentheses, unary-prefix and function-call operators can be an operand.
                    if not (next_is_leaf or next_is_open_parenthesis or next_is_prefix_operator or next_is_function_identifier):
                        message = ErrorMessages.RIGHT_OPERAND_REQUIRED.replace("$0", token.word)
                        raise ExevalatorException(message)

                # Cases of binary operators or a separator of partial expressions
                if (token.operator is not None and token.operator.type == OperatorType.BINARY) or token.word == ",":
                    # Only leafs, open parentheses, unary-prefix and function-call operators can be right-operands.
                    if not (next_is_leaf or next_is_open_parenthesis or next_is_prefix_operator or next_is_function_identifier):
                        message = ErrorMessages.RIGHT_OPERAND_REQUIRED.replace("$0", token.word)
                        raise ExevalatorException(message)
                    # Only leaf elements and closed parenthesis can be a left-operand.
                    if not (prev_is_leaf or prev_is_close_parenthesis):
                        message = ErrorMessages.LEFT_OPERAND_REQUIRED.replace("$0", token.word)
                        raise ExevalatorException(message)

            # Case of leaf elements
            if token.type in leaf_type_set:
                # Another leaf or an open parenthesis cannot be at the right of a leaf element.
                if (not next_is_function_call_begin) and (next_is_open_parenthesis or next_is_leaf):
                    message = ErrorMessages.RIGHT_OPERATOR_REQUIRED.replace("$0", token.word)
                    raise ExevalatorException(message)

                # Another leaf element or a closed parenthesis cannot be at the left of a leaf element.
                if prev_is_close_parenthesis or prev_is_leaf:
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
        token_count = len(tokens)

        # Working stack to form multiple AstNode instances into a tree-shape.
        stack: list[AstNode] = []

        # Temporary nodes used in the above working stack, for isolating ASTs of partial expressions.
        parenthesis_stack_lid = AstNode(Token(type=TokenType.STACK_LID, word="(PARENTHESIS_STACK_LID)"))
        separator_stack_lid   = AstNode(Token(type=TokenType.STACK_LID, word="(SEPARATOR_STACK_LID)"))
        call_begin_stack_lid   = AstNode(Token(type=TokenType.STACK_LID, word="(CALL_BEGIN_STACK_LID)"))

        # The array storing next operator's precedence for each token.
        # At [i], it stores the precedence of the first operator whose token-index is greater than i.
        next_operator_precedences = Parser._get_next_operator_precedences(tokens)

        # Read tokens from left to right.
        itoken = 0
        while True:  # do { ... } while (itoken < token_count);

            # When jumped to here by "continue",
            # we must check the condition and break this loop if necessary.
            if not (itoken < token_count):
                break

            token = tokens[itoken]
            operator_node: AstNode | None = None

            # Case of literals and identifiers: "1.23", "x", "f", etc.
            if token.type in (TokenType.NUMBER_LITERAL, TokenType.VARIABLE_IDENTIFIER, TokenType.FUNCTION_IDENTIFIER):
                stack.append(AstNode(token))
                itoken += 1
                continue

            # Case of parenthesis: "(" or ")"
            elif token.type == TokenType.PARENTHESIS:
                if token.word == "(":
                    stack.append(parenthesis_stack_lid)
                    itoken += 1
                    continue
                else:  # ")"
                    operator_node = Parser._pop_partial_expr_nodes(stack, parenthesis_stack_lid)[0]

            # Case of operators: "+", "-", etc.
            elif token.type == TokenType.OPERATOR:
                operator_node = AstNode(token)
                next_op_precedence = next_operator_precedences[itoken]

                # Case of unary-prefix operators
                if token.operator.type == OperatorType.UNARY_PREFIX:
                    if Parser._should_add_right_operand(token.operator.associativity, token.operator.precedence, next_op_precedence):
                        operator_node.child_node_list.append(AstNode(tokens[itoken + 1]))
                        itoken += 1  # looked-ahead

                # Case of binary operators
                elif token.operator.type == OperatorType.BINARY:
                    operator_node.child_node_list.append(stack.pop())
                    if Parser._should_add_right_operand(token.operator.associativity, token.operator.precedence, next_op_precedence):
                        operator_node.child_node_list.append(AstNode(tokens[itoken + 1]))
                        itoken += 1  # looked-ahead

                # Case of function-call operators
                elif token.operator.type == OperatorType.CALL:
                    if token.word == "(":
                        # Add function-identifier node at the top of the stack.
                        operator_node.child_node_list.append(stack.pop())
                        stack.append(operator_node)
                        # Marker to collect partial expressions of args from the stack.
                        stack.append(call_begin_stack_lid)
                        itoken += 1
                        continue
                    elif token.word == ")":
                        argNodes = Parser._pop_partial_expr_nodes(stack, call_begin_stack_lid)
                        operator_node = stack.pop()  # the '(' CALL operator node
                        for argNode in argNodes:
                            operator_node.child_node_list.append(argNode)
                    elif token.word == ",":
                        stack.append(separator_stack_lid)
                        itoken += 1
                        continue

            # If the precedence of the operator at the top of the stack is stronger than the next operator,
            # connect all "unconnected yet" operands and operators in the stack.
            while Parser._should_add_right_operandToStackedOperator(stack, next_operator_precedences[itoken]):
                oldoperator_node = operator_node
                operator_node = stack.pop()
                operator_node.child_node_list.append(oldoperator_node)  # type: ignore[arg-type]

            stack.append(operator_node)  # type: ignore[arg-type]
            itoken += 1

            # Tail of "do { ... } while (itoken < token_count);":
            # We must check the condition and break this loop if necessary.
            if not (itoken < token_count):
                break
        
        # The AST has been constructed on the stack, and only its root node is stored in the stack.
        root_node_of_expression_ast = stack.pop()

        # Check that the depth of the constructed AST does not exceed the limit.
        root_node_of_expression_ast.checkDepth(1, StaticSettings.MAX_AST_DEPTH)

        return root_node_of_expression_ast


    @staticmethod
    def _should_add_right_operand(
        targetOperatorAssociativity: OperatorAssociativity,
        targetOperatorPrecedence: int,
        next_operator_precedence: int,
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
        target_op_precedence_is_strong = targetOperatorPrecedence < next_operator_precedence
        target_op_precedence_is_equal = targetOperatorPrecedence == next_operator_precedence
        target_op_associativity_is_left = targetOperatorAssociativity == OperatorAssociativity.LEFT
        return target_op_precedence_is_strong or (target_op_precedence_is_equal and target_op_associativity_is_left)

    @staticmethod
    def _should_add_right_operandToStackedOperator(
        stack: List[AstNode], next_operator_precedence: int
    ) -> bool:
        """
        Judges whether the right-side token should be connected directly as an operand,
        to the operator at the top of the working stack.
        """
        if len(stack) == 0 or stack[-1].token.type != TokenType.OPERATOR:
            return False
        
        operator_on_stack_top: Operator = stack[-1].token.operator  # type: ignore[assignment]

        return Parser._should_add_right_operand(
            operator_on_stack_top.associativity,
            operator_on_stack_top.precedence,
            next_operator_precedence,
        )

    @staticmethod
    def _pop_partial_expr_nodes(stack: List[AstNode], endStackLidNode: AstNode) -> List[AstNode]:
        """
        Pops root nodes of ASTs of partial expressions constructed on the stack.
        In the returned list, the popped nodes are stored in FIFO order.
        """
        if len(stack) == 0:
            raise ExevalatorException(ErrorMessages.UNEXPECTED_PARTIAL_EXPRESSION)

        partial_expr_node_list: List[AstNode] = []
        while len(stack) != 0:
            if stack[-1].token.type == TokenType.STACK_LID:
                stack_lid_node = stack.pop()
                if stack_lid_node is endStackLidNode:
                    break
            else:
                partial_expr_node_list.append(stack.pop())

        # Store elements in FIFO order and return
        node_count = len(partial_expr_node_list)
        partial_expr_nodes: List[AstNode] = [None] * node_count  # type: ignore[list-item]
        for inode in range(node_count):
            partial_expr_nodes[inode] = partial_expr_node_list[node_count - inode - 1]
        return partial_expr_nodes  # type: ignore[return-value]

    @staticmethod
    def _get_next_operator_precedences(tokens: List[Token]) -> List[int]:
        """
        Returns an array storing next operator's precedence for each token.
        At index i, it stores the precedence of the first operator whose token-index is greater than i.
        """
        token_count = len(tokens)
        last_operator_precedence: int = StaticSettings.LEAST_PRIOR_OPERATOR_PRECEDENCE
        next_operator_precedences: List[int] = [StaticSettings.LEAST_PRIOR_OPERATOR_PRECEDENCE] * token_count

        for itoken in range(token_count - 1, -1, -1):
        # for (int itoken=token_count-1; 0<=itoken; itoken--) {

            token = tokens[itoken]
            next_operator_precedences[itoken] = last_operator_precedence

            if token.type == TokenType.OPERATOR:
                last_operator_precedence = token.operator.precedence  # type: ignore[assignment]

            if token.type == TokenType.PARENTHESIS:
                if token.word == "(":
                    last_operator_precedence = 0  # most prior
                else:  # case of ")"
                    last_operator_precedence = StaticSettings.LEAST_PRIOR_OPERATOR_PRECEDENCE

        return next_operator_precedences


class Evaluator:
    """
    The class for evaluating the value of an AST.
    """

    def __init__(self) -> None:
        # The tree of evaluator nodes, which evaluates an expression.
        self.evaluator_node_tree: Optional[EvaluatorNode] = None

    def update(
        self,
        ast: AstNode,
        variable_table: Dict[str, int],
        function_table: Dict[str, FunctionInterface],
    ) -> None:
        """
        Updates the state to evaluate the value of the AST.
        """
        node = Evaluator._create_evaluator_node_tree(ast, variable_table, function_table)
        if node is None:
            raise ExevalatorException(
                ErrorMessages.UNEXPECTED_ERROR.replace("$0", "root node has no evaluator")
            )
        self.evaluator_node_tree = node

    def is_evaluatable(self) -> bool:
        """
        Returns whether `evaluate` method is available on the current state.
        """
        return self.evaluator_node_tree is not None

    def evaluate(self, memory: List[float]) -> float:
        """
        Evaluates the value of the AST set by `update` method.
        """
        if self.evaluator_node_tree is None:
            message = ErrorMessages.UNEXPECTED_ERROR.replace("$0", "evaluator_node_tree has not been initialized yet.")
            raise ExevalatorException(message)
        return self.evaluator_node_tree.evaluate(memory)

    @staticmethod
    def _create_evaluator_node_tree(
        ast: "AstNode",
        variable_table: Dict[str, int],
        function_table: Dict[str, FunctionInterface],
    ) -> Optional[EvaluatorNode]:
        """
        Creates a tree of evaluator nodes corresponding with the specified AST.

        Note: This method creates a tree of evaluator nodes by traversing each node in the AST recursively.
        """
        child_node_list: List[AstNode] = ast.child_node_list
        child_count = len(child_node_list)

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
        # operator read the name from `child_node_list[0].token.word`.

        # Creates evaluator nodes of child nodes, and store them into an array.
        child_node_nodes: List[Optional[EvaluatorNode]] = [None] * child_count
        for ichild in range(child_count):
            child_ast_node = child_node_list[ichild]
            child_node_nodes[ichild] = Evaluator._create_evaluator_node_tree(child_ast_node, variable_table, function_table)

        # Initialize evaluator node of THIS node
        token = ast.token
        if token.type == TokenType.NUMBER_LITERAL:
            return NumberLiteralEvaluatorNode(token.word)

        elif token.type == TokenType.VARIABLE_IDENTIFIER:
            if token.word not in variable_table:
                raise ExevalatorException(
                    ErrorMessages.VARIABLE_NOT_FOUND.replace("$0", token.word)
                )
            address = variable_table[token.word]
            return VariableEvaluatorNode(address)

        elif token.type == TokenType.FUNCTION_IDENTIFIER:
            return None  # type: ignore[return-value]

        elif token.type == TokenType.OPERATOR:
            op: "Operator" = token.operator  # type: ignore[assignment]

            if op.type == OperatorType.UNARY_PREFIX and op.symbol == "-":
                return MinusEvaluatorNode(
                    Evaluator._requireNotNone(child_node_nodes[0])
                )

            elif op.type == OperatorType.BINARY and op.symbol == "+":
                return AdditionEvaluatorNode(
                    Evaluator._requireNotNone(child_node_nodes[0]),
                    Evaluator._requireNotNone(child_node_nodes[1])
                )

            elif op.type == OperatorType.BINARY and op.symbol == "-":
                return SubtractionEvaluatorNode(
                    Evaluator._requireNotNone(child_node_nodes[0]),
                    Evaluator._requireNotNone(child_node_nodes[1])
                )

            elif op.type == OperatorType.BINARY and op.symbol == "*":
                return MultiplicationEvaluatorNode(
                    Evaluator._requireNotNone(child_node_nodes[0]),
                    Evaluator._requireNotNone(child_node_nodes[1])
                )

            elif op.type == OperatorType.BINARY and op.symbol == "/":
                return DivisionEvaluatorNode(
                    Evaluator._requireNotNone(child_node_nodes[0]),
                    Evaluator._requireNotNone(child_node_nodes[1])
                )

            elif op.type == OperatorType.CALL and op.symbol == "(":
                identifier = child_node_list[0].token.word  # FUNCTION_IDENTIFIER node
                if identifier not in function_table:
                    raise ExevalatorException(
                        ErrorMessages.FUNCTION_NOT_FOUND.replace("$0", identifier)
                    )
                function = function_table[identifier]
                argCount = child_count - 1
                argNodes: List[EvaluatorNode] = [None] * argCount  # type: ignore[list-item]
                for iarg in range(argCount):
                    # child_node_nodes[1..] are arguments
                    argNodes[iarg] = Evaluator._requireNotNone(child_node_nodes[iarg + 1])
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


#@dataclass(frozen=True, kw_only=True) # kw_only is not supported on Python 3.9, we want to support it.
@dataclass(frozen=True)
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


#@dataclass(frozen=True, kw_only=True) # kw_only is not supported on Python 3.9, we want to support it.
@dataclass(frozen=True)
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
    child_node_list: List[AstNode]  # The list of child nodes of this AST node.

    def __init__(self, token: Token) -> None:
        """
        Create an AST node instance storing specified information.

        Args:
            token: The token corresponding with this AST node.
        """
        self.token = token
        self.child_node_list = []

    def checkDepth(self, depth_of_this_node: int, max_ast_depth: int) -> None:
        """
        Checks that depths under this node do not exceed the specified maximum.

        Args:
            depth_of_this_node: The depth of this node in the AST.
            max_ast_depth: The maximum depth allowed.

        Raises:
            ExevalatorException: If the depth exceeds the maximum value.
        """
        if max_ast_depth < depth_of_this_node:
            message = ErrorMessages.EXCEEDS_MAX_AST_DEPTH.replace(
                "$0", str(StaticSettings.MAX_AST_DEPTH)
            )
            raise ExevalatorException(message)
        for child_node in self.child_node_list:
            child_node.checkDepth(depth_of_this_node + 1, max_ast_depth)

    def toMarkuppedText(self, indent_stage: int = 0) -> str:
        """
        Expresses the AST under this node in XML-like text format.

        Args:
            indent_stage: The stage of indent of this node.

        Returns:
            XML-like text representation of the AST under this node.
        """
        indent = StaticSettings.AST_INDENT * indent_stage
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

        if self.child_node_list:
            parts.append(">")
            for child_node in self.child_node_list:
                parts.append(eol)
                parts.append(child_node.toMarkuppedText(indent_stage + 1))
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

    __slots__ = ("left_operand_node", "right_operand_node")

    def __init__(self, left_operand_node: EvaluatorNode, right_operand_node: EvaluatorNode) -> None:
        """
        Initializes operands.

        Args:
            left_operand_node: The node for evaluating the left-side operand.
            right_operand_node: The node for evaluating the right-side operand.
        """
        self.left_operand_node = left_operand_node
        self.right_operand_node = right_operand_node


class AdditionEvaluatorNode(BinaryOperationEvaluatorNode):
    """The evaluator node for evaluating the value of an addition operator."""

    def evaluate(self, memory: List[float]) -> float:
        """Performs the addition."""
        return self.left_operand_node.evaluate(memory) + self.right_operand_node.evaluate(memory)


class SubtractionEvaluatorNode(BinaryOperationEvaluatorNode):
    """The evaluator node for evaluating the value of a subtraction operator."""

    def evaluate(self, memory: List[float]) -> float:
        """Performs the subtraction."""
        return self.left_operand_node.evaluate(memory) - self.right_operand_node.evaluate(memory)


class MultiplicationEvaluatorNode(BinaryOperationEvaluatorNode):
    """The evaluator node for evaluating the value of a multiplication operator."""

    def evaluate(self, memory: List[float]) -> float:
        """Performs the multiplication."""
        return self.left_operand_node.evaluate(memory) * self.right_operand_node.evaluate(memory)


class DivisionEvaluatorNode(BinaryOperationEvaluatorNode):
    """The evaluator node for evaluating the value of a division operator."""

    def evaluate(self, memory: List[float]) -> float:
        """Performs the division."""
        return self.left_operand_node.evaluate(memory) / self.right_operand_node.evaluate(memory)


class MinusEvaluatorNode(EvaluatorNode):
    """The evaluator node for evaluating the value of a unary-minus operator."""

    __slots__ = ("operand_node",)

    def __init__(self, operand_node: EvaluatorNode) -> None:
        """
        Initializes the operand.

        Args:
            operand_node: The node for evaluating the operand.
        """
        self.operand_node = operand_node

    def evaluate(self, memory: List[float]) -> float:
        """Performs the negation (unary minus)."""
        return -self.operand_node.evaluate(memory)


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

    __slots__ = ("function", "function_name", "argument_eval_nodes", "argument_array_buffer")

    def __init__(
        self,
        function: FunctionInterface,
        function_name: str,
        argument_eval_nodes: List[EvaluatorNode],
    ) -> None:
        """
        Initializes information of functions to be called.

        Args:
            function: The function to be called.
            function_name: The name of the function.
            argument_eval_nodes: Evaluator nodes for evaluating values of arguments.
        """
        self.function = function
        self.function_name = function_name
        self.argument_eval_nodes = argument_eval_nodes
        # Preallocate buffer
        self.argument_array_buffer = [0.0] * len(self.argument_eval_nodes)

    def evaluate(self, memory: List[float]) -> float:
        """
        Calls the function and returns the returned value of the function.
        """
        argCount = len(self.argument_eval_nodes)
        for iarg in range(argCount):
            self.argument_array_buffer[iarg] = self.argument_eval_nodes[iarg].evaluate(memory)
        try:
            return self.function.invoke(self.argument_array_buffer)
        except Exception as e:
            message = (
                ErrorMessages.FUNCTION_ERROR
                .replace("$0", self.function_name)
                .replace("$1", str(e))
            )
            raise ExevalatorException(message) from e


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
        r"([eE][+\-]?[0-9]+)?"         # exponent
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
    _addition_operator = Operator(
        type=OperatorType.BINARY,
        symbol='+', precedence=400, associativity=OperatorAssociativity.LEFT
    )
    _subtraction_operator = Operator(
        type=OperatorType.BINARY,
        symbol='-', precedence=400, associativity=OperatorAssociativity.LEFT
    )
    _multiplication_operator = Operator(
        type=OperatorType.BINARY,
        symbol='*', precedence=300, associativity=OperatorAssociativity.LEFT
    )
    _division_operator = Operator(
        type=OperatorType.BINARY,
        symbol='/', precedence=300, associativity=OperatorAssociativity.LEFT
    )
    _minus_operator = Operator(
        type=OperatorType.UNARY_PREFIX,
        symbol='-', precedence=200, associativity=OperatorAssociativity.RIGHT
    )
    _call_begin_operator = Operator(
        type=OperatorType.CALL,
        symbol='(', precedence=100, associativity=OperatorAssociativity.LEFT
    )
    _call_end_operator = Operator(
        type=OperatorType.CALL,
        symbol=')', precedence=LEAST_PRIOR_OPERATOR_PRECEDENCE, associativity=OperatorAssociativity.LEFT
    )
    _call_separator_operator = Operator(
        type=OperatorType.CALL,
        symbol=',', precedence=LEAST_PRIOR_OPERATOR_PRECEDENCE, associativity=OperatorAssociativity.LEFT
    )

    # The set of symbols of available operators.
    OPERATOR_SYMBOL_SET: Set[str] = {
        '+', '-', '*', '/', '(', ')', ','
    }

    # The Map mapping each symbol of an unary-prefix operator to an instance of Operator class.
    UNARY_PREFIX_OPERATOR_SYMBOL_DICT: Dict[str, Operator] = {
        '-': _minus_operator,
    }

    # The Map mapping each symbol of a binary operator to an instance of Operator class.
    BINARY_OPERATOR_SYMBOL_DICT: Dict[str, Operator] = {
        '+': _addition_operator,
        '-': _subtraction_operator,
        '*': _multiplication_operator,
        '/': _division_operator,
    }

    # The Map mapping each symbol of a call operator to an instance of Operator class.
    CALL_OPERATOR_SYMBOL_DICT: Dict[str, Operator] = {
        '(': _call_begin_operator,
        ')': _call_end_operator,
        ',': _call_separator_operator,
    }

    # The list of symbols to split an expression into tokens.
    TOKEN_SPLITTER_SYMBOL_LIST: List[str] = ['+', '-', '*', '/', '(', ')', ',']
