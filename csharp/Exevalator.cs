/*
 * Exevalator Ver.2.0.1 - by RINEARN 2021-2024
 * This software is released under the "Unlicense" license.
 * You can choose the "CC0" license instead, if you want.
 */

/*
 * Put this code in your source code folder, and import it as
 * "using Rinearn.ExevalatorCS;" from a source file in which you want to use Exevalator.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Rinearn.ExevalatorCS
{
#nullable enable

    /// <summary>
    /// Interpreter Engine of Exevalator.
    /// </summary>
    class Exevalator
    {

        /// <summary>The List used as as a virtual memory storing values of variables.</summary>
        List<double> Memory;

        /// <summary>The object evaluating the value of the expression.</summary>
        Evaluator Evaluator;

        /// <summary>The Dictionary mapping each variable name to an address of the variable.</summary>
        Dictionary<string, int> VariableTable;

        /// <summary>The Dictionary mapping each function name to an IExevalatorFunction instance.</summary>
        Dictionary<string, IExevalatorFunction> FunctionTable;

        /// <summary>Caches the content of the expression evaluated last time, to skip re-parsing.</summary>
        string LastEvaluatedExpression;

        /// <summary>
        /// Creates a new interpreter of the Exevalator.
        /// </summary>
        public Exevalator()
        {
            this.Memory = new List<double>();
            this.Evaluator = new Evaluator();
            this.VariableTable = new Dictionary<string, int>();
            this.FunctionTable = new Dictionary<string, IExevalatorFunction>();
            this.LastEvaluatedExpression = "";
        }

        /// <summary>
        /// Evaluates (computes) the value of an expression.
        /// </summary>
        /// <param name="expression">The expression to be evaluated</param>
        /// <returns>The evaluated value</returns>
        public double Eval(string expression)
        {
            if (expression == null)
            {
                throw new NullReferenceException();
            }
            if (StaticSettings.MaxExpressionCharCount < expression.Length)
            {
                throw new ExevalatorException(
                    "The length of the expression exceeds the limit "
                    + "(StaticSettings.MaxExpressionCharCount: "
                    + StaticSettings.MaxExpressionCharCount + ")"
                );
            }

            try
            {
                // If the expression changed from the last-evaluated expression, re-parsing is necessary.
                if (expression != this.LastEvaluatedExpression || !this.Evaluator.IsEvaluatable())
                {

                    // Perform lexical analysis, and get tokens from the inputted expression.
                    Token[] tokens = LexicalAnalyzer.Analyze(expression);

                    /*
                    // Temporary, for debugging tokens
                    foreach (Token token in tokens) {
                        Console.WriteLine(token);
                    }
                    */

                    // Perform parsing, and get AST(Abstract Syntax Tree) from tokens.
                    AstNode ast = Parser.Parse(tokens);

                    /*
                    // Temporary, for debugging ast
                    Console.WriteLine(ast.ToMarkuppedText());
                    */

                    // Update the evaluator, to evaluate the parsed AST.
                    this.Evaluator.Update(ast, this.VariableTable, this.FunctionTable);

                    this.LastEvaluatedExpression = expression;
                }

                // Evaluate the value of the expression, and return it.
                double evaluatedValue = this.Evaluator.Evaluate(this.Memory);
                return evaluatedValue;

            }
            catch (ExevalatorException ee)
            {
                    throw ee;
            }
            // Wrap an unexpected exception by Exevalator.Exception and rethrow it.
            catch (Exception e)
            {
                throw new ExevalatorException("Unexpected exception/error occurred", e);
            }
        }

        /// <summary>
        /// Re-evaluates (re-computes) the value of the expression evaluated by "eval" method last time.
        /// This method may (slightly) work faster than calling "eval" method repeatedly for the same expression.
        /// Note that, the result value may different with the last evaluated value, 
        /// if values of variables or behaviour of functions had changed.
        /// </summary>
        /// <returns>The evaluated value</returns>
        public double Reeval()
        {
            if (this.Evaluator.IsEvaluatable()) {
                throw new ExevalatorException("\"Reeval\" is not available before using \"Eval\"");
            } else {
                double evaluatedValue = this.Evaluator.Evaluate(this.Memory);
                return evaluatedValue;
            }
        }

        /// <summary>
        /// Declares a new variable, for using the value of it in expressions.
        /// </summary>
        /// <param name="name">The name of the variable to be declared</param>
        /// <returns>
        ///     The virtual address of the declared variable,
        ///     which useful for accessing to the variable faster.
        ///     See "WriteVariableAt" and "ReadVariableAt" method.
        /// </returns>
        public int DeclareVariable(string name)
        {
            if (name == null)
            {
                throw new NullReferenceException();
            }
            if (StaticSettings.MaxNameCharCount < name.Length)
            {
                throw new ExevalatorException(
                    "The length of the variable name exceeds the limit (StaticSettings.MaxNameCharCount: "
                    + StaticSettings.MaxNameCharCount + ")"
                );
            }
            int address = this.Memory.Count;
            this.Memory.Add(0.0);
            this.VariableTable[name] = address;
            return address;
        }

        /// <summary>
        /// Writes the value to the variable having the specified name.
        /// </summary>
        /// <param name="name">The name of the variable to be written</param>
        /// <param name="value">The new value of the variable</param>
        public void WriteVariable(string name, double value)
        {
            if (name == null)
            {
                throw new NullReferenceException();
            }
            if (StaticSettings.MaxNameCharCount < name.Length || !this.VariableTable.ContainsKey(name))
            {
                throw new ExevalatorException("Variable not found: " + name);
            }
            int address = this.VariableTable[name];
            this.WriteVariableAt(address, value);
        }

        /// <summary>
        /// Writes the value to the variable at the specified virtual address.
        /// This method works faster than "WriteVariable" method.
        /// </summary>
        /// <param name="address">The virtual address of the variable to be written</param>
        /// <param name="value">The new value of the variable</param>
        public void WriteVariableAt(int address, double value)
        {
            if (address < 0 || this.Memory.Count <= address)
            {
                throw new ExevalatorException("Invalid variable address: " + address);
            }
            this.Memory[address] = value;
        }

        /// <summary>
        /// Reads the value of the variable having the specified name.
        /// </summary>
        /// <param name="name">The name of the variable to be read</param>
        /// <returns>The current value of the variable</returns>
        public double ReadVariable(string name)
        {
            if (name == null)
            {
                throw new NullReferenceException();
            }
            if (StaticSettings.MaxNameCharCount < name.Length || !this.VariableTable.ContainsKey(name))
            {
                throw new ExevalatorException("Variable not found: " + name);
            }
            int address = this.VariableTable[name];
            return this.ReadVariableAt(address);
        }

        /// <summary>
        /// Reads the value of the variable at the specified virtual address.
        /// This method works faster than "EeadVariable" method.
        /// </summary>
        /// <param name="address">The virtual address of the variable to be read</param>
        /// <returns>The current value of the variable</returns>
        public double ReadVariableAt(int address)
        {
            if (address < 0 || this.Memory.Count <= address)
            {
                throw new ExevalatorException("Invalid variable address: " + address);
            }
            return this.Memory[address];
        }

        /// <summary>
        /// Connects a function, for using it in the expression.
        /// </summary>
        /// <param name="name">The name of the function used in the expression</param>
        /// <param name="function">The function to be connected</param>
        public void ConnectFunction(string name, IExevalatorFunction function)
        {
            if (name == null)
            {
                throw new NullReferenceException();
            }
            if (StaticSettings.MaxNameCharCount < name.Length)
            {
                throw new ExevalatorException(
                    "The length of the function name exceeds the limit (StaticSettings.MaxNameCharCount: "
                    + StaticSettings.MaxNameCharCount + ")"
                );
            }
            this.FunctionTable[name] = function;
        }
    }


    public class ExevalatorException : Exception
    {
        public ExevalatorException(string errorMessage) : base(errorMessage)
        {
        }
        public ExevalatorException(string errorMessage, Exception causeException) : base(errorMessage, causeException)
        {
        }
    }

    public interface IExevalatorFunction
    {
        double Invoke(double[] arguments);
    }


    /// <summary>
    /// The class performing functions of a lexical analyzer.
    /// </summary>
    public class LexicalAnalyzer
    {
        /// <summary>
        /// Splits (tokenizes) the expression into tokens, and analyze them.
        /// </summary>
        /// <param name="expression">The expression to be tokenized/analyzed</param>
        /// <returns>Analyzed tokens</returns>
        public static Token[] Analyze(string expression)
        {

            // Firstly, to simplify the tokenization,
            // replace number literals in the expression to the escaped representation: "@NUMBER_LITERAL",
            // because number literals may contains "+" or "-" in their exponent part.
            List<string> numberLiterals = new List<string>();
            string escapedExpression = LexicalAnalyzer.EscapeNumberLiterals(expression, numberLiterals);

            // Tokenize (split) the expression into token word(string)s.
            string spacedExpression = escapedExpression;
            foreach (char tokenSplitter in StaticSettings.TokenSplitterSymbolList)
            {
                spacedExpression = spacedExpression.Replace(Char.ToString(tokenSplitter), " " + tokenSplitter + " ");
            }
            spacedExpression = spacedExpression.Trim();
            string[] tokenWords = Regex.Split(spacedExpression, "\\s+");

            // For an empty expression (containing no tokens), the above returns { "" }, not { }.
            // So we should detect/handle it as follows.
            if (tokenWords.Length == 1 && tokenWords[0].Length == 0)
            {
                throw new ExevalatorException("The inputted expression is empty");
            }

            // Checks the total number of tokens.
            if (StaticSettings.MaxTokenCount < tokenWords.Length)
            {
                throw new ExevalatorException(
                    "The number of tokens exceeds the limit (StaticSettings.MaxTokenCount: "
                    + StaticSettings.MaxTokenCount + ")"
                );
            }

            // Create Token-type instances from token word(string)s.
            // Also, escaped number literals will be recovered.
            Token[] tokens = LexicalAnalyzer.CreateTokensFromTokenWords(tokenWords, numberLiterals.ToArray());

            // Check syntactic correctness of tokens.
            LexicalAnalyzer.CheckParenthesisOpeningClosings(tokens);
            LexicalAnalyzer.CheckEmptyParentheses(tokens);
            LexicalAnalyzer.CheckLocationsOfOperatorsAndLeafs(tokens);

            return tokens;
        }

        /// <summary>
        /// Extracts all number literals in the expression, and replaces them to the escaped representation.
        /// </summary>
        /// <param name="expression">The expression to be processed</param>
        /// <param name="numberLiterals">The List to which extracted number literals will be stored</param>
        /// <returns>The expression in which all number literals are escaped</returns>
        private static string EscapeNumberLiterals(string expression, List<string> numberLiterals)
        {
            StringBuilder escapedExpressionBuilder = new StringBuilder();

            MatchCollection matchedResults = Regex.Matches(expression, StaticSettings.NumberLiteralRegex);
            int lastLiteralEnd = -1;
            foreach (Match matchedResult in matchedResults)
            {
                string literalWord = matchedResult.Value;
                int literalBegin = matchedResult.Index;
                numberLiterals.Add(literalWord);

                int intervalLength = literalBegin - lastLiteralEnd - 1;
                escapedExpressionBuilder.Append(expression.Substring(lastLiteralEnd + 1, intervalLength));
                escapedExpressionBuilder.Append(StaticSettings.EscapedNumberLiteral);
                lastLiteralEnd = literalBegin + literalWord.Length - 1;
            }
            if (lastLiteralEnd < expression.Length - 1)
            {
                int tailLength = expression.Length - lastLiteralEnd - 1;
                escapedExpressionBuilder.Append(expression.Substring(lastLiteralEnd + 1, tailLength));
            }
            string escapedExpression = escapedExpressionBuilder.ToString();
            return escapedExpression;
        }

        /// <summary>
        /// Create Token-type instances from token word(string)s.
        /// Also, escaped number literals will be recovered.
        /// </summary>
        /// <param name="tokenWords">Tokenized words</param>
        /// <returns>Created Token-type instances</returns>
        private static Token[] CreateTokensFromTokenWords(string[] tokenWords, string[] numberLiterals)
        {
            int tokenCount = tokenWords.Length;

            // Stores the parenthesis-depth, which will increase at "(" and decrease at ")".
            int parenthesisDepth = 0;

            // Stores the parenthesis-depth when a function call operator begins,
            // for detecting the end of the function operator.
            HashSet<int> callParenthesisDepths = new HashSet<int>();

            int iliteral = 0;
            Token? lastToken = null;
            Token[] tokens = new Token[tokenCount];
            for (int itoken = 0; itoken < tokenCount; itoken++)
            {
                String word = tokenWords[itoken];

                // Cases of open parentheses, or beginning of function calls.
                if (word == "(")
                {
                    parenthesisDepth++;
                    if (1 <= itoken && tokens[itoken - 1].Type == TokenType.FunctionIdentifier)
                    {
                        callParenthesisDepths.Add(parenthesisDepth);
                        Operator op = StaticSettings.CallSymbolOperatorDict[word[0]];
                        tokens[itoken] = new Token(TokenType.Operator, word, op);
                    }
                    else
                    {
                        tokens[itoken] = new Token(TokenType.Parenthesis, word);
                    }

                }
                // Cases of closes parentheses, or end of function calls.
                else if (word == ")")
                {
                    if (callParenthesisDepths.Contains(parenthesisDepth))
                    {
                        callParenthesisDepths.Remove(parenthesisDepth);
                        Operator op = StaticSettings.CallSymbolOperatorDict[word[0]];
                        tokens[itoken] = new Token(TokenType.Operator, word, op);
                    }
                    else
                    {
                        tokens[itoken] = new Token(TokenType.Parenthesis, word);
                    }
                    parenthesisDepth--;

                }
                // Cases of operators.
                else if (word.Length == 1 && StaticSettings.OperatorSymbolSet.Contains(word[0]))
                {
                    tokens[itoken] = new Token(TokenType.Operator, word);

                    Operator? op;
                    if (lastToken == null || lastToken?.Word == "("
                            || (lastToken?.Type == TokenType.Operator
                            && lastToken?.Operator?.Type != OperatorType.Call))
                    {

                        if (StaticSettings.UnaryPrefixSymbolOperatorDict.ContainsKey(word[0]))
                        {
                            op = StaticSettings.UnaryPrefixSymbolOperatorDict[word[0]];
                        }
                        else
                        {
                            throw new ExevalatorException("Unknown unary-prefix operator: " + word);
                        }

                    }
                    else if (lastToken?.Word == ")"
                          || lastToken?.Type == TokenType.NumberLiteral
                          || lastToken?.Type == TokenType.VariableIdentifier)
                    {

                        if (StaticSettings.BinarySymbolOperatorDict.ContainsKey(word[0]))
                        {
                            op = StaticSettings.BinarySymbolOperatorDict[word[0]];
                        }
                        else
                        {
                            throw new ExevalatorException("Unknown binary operator: " + word);
                        }

                    }
                    else
                    {
                        throw new ExevalatorException("Unexpected operator syntax: " + word);
                    }
                    tokens[itoken] = new Token(TokenType.Operator, word, op.Value);

                }
                // Cases of literals, and separator.
                else if (word == StaticSettings.EscapedNumberLiteral)
                {
                    tokens[itoken] = new Token(TokenType.NumberLiteral, numberLiterals[iliteral]);
                    iliteral++;
                }
                else if (word == ",")
                {
                    tokens[itoken] = new Token(TokenType.ExpressionSeparator, word);

                }
                // Cases of variable identifier of function identifier.
                else
                {
                    if (itoken < tokenCount - 1 && tokenWords[itoken + 1] == "(")
                    {
                        tokens[itoken] = new Token(TokenType.FunctionIdentifier, word);
                    }
                    else
                    {
                        tokens[itoken] = new Token(TokenType.VariableIdentifier, word);
                    }
                }
                lastToken = tokens[itoken];
            }
            return tokens;
        }

        /// <summary>
        /// Checks the number and correspondence of open "(" / closed ")" parentheses.
        /// An ExevalatorException will be thrown when any errors detected.
        /// If no error detected, nothing will occur.
        /// </summary>
        /// <param name="tokens">Tokens of the inputted expression.</param>
        private static void CheckParenthesisOpeningClosings(Token[] tokens)
        {
            int tokenCount = tokens.Length;
            int hierarchy = 0; // Increases at "(" and decreases at ")".

            for (int itoken = 0; itoken < tokenCount; itoken++)
            {
                Token token = tokens[itoken];
                if (token.Word == "(")
                {
                    hierarchy++;
                }
                else if (token.Word == ")")
                {
                    hierarchy--;
                }

                // If the value of hierarchy is negative, the open parenthesis is deficient.
                if (hierarchy < 0)
                {
                    throw new ExevalatorException(
                        "The number of open parenthesis \"(\" is deficient."
                    );
                }
            }

            // If the value of hierarchy is not zero at the end of the expression,
            // the closed parentheses ")" is deficient.
            if (hierarchy > 0)
            {
                throw new ExevalatorException(
                    "The number of closed parenthesis \")\" is deficient."
                );
            }
        }

        /// <summary>
        /// Checks that empty parentheses "()" are not contained in the expression.
        /// An ExevalatorException will be thrown when any errors detected.
        /// If no error detected, nothing will occur.
        /// </summary>
        /// <param name="tokens">Tokens of the inputted expression.</param>
        private static void CheckEmptyParentheses(Token[] tokens)
        {
            int tokenCount = tokens.Length;
            int contentCounter = 0;
            for (int itoken = 0; itoken < tokenCount; itoken++)
            {
                Token token = tokens[itoken];
                if (token.Type == TokenType.Parenthesis)
                { // Excepting CALL operators
                    if (token.Word == "(")
                    {
                        contentCounter = 0;
                    }
                    else if (token.Word == ")")
                    {
                        if (contentCounter == 0)
                        {
                            throw new ExevalatorException(
                                "The content parentheses \"()\" should not be empty (excluding function calls)."
                            );
                        }
                    }
                }
                else
                {
                    contentCounter++;
                }
            }
        }

        /// <summary>
        /// Checks correctness of locations of operators and leaf elements (literals and identifiers).
        /// An ExevalatorException will be thrown when any errors detected.
        /// If no error detected, nothing will occur.
        /// </summary>
        /// <param name="tokens">Tokens of the inputted expression.</param>
        private static void CheckLocationsOfOperatorsAndLeafs(Token[] tokens)
        {
            int tokenCount = tokens.Length;
            HashSet<TokenType> leafTypeSet = new HashSet<TokenType>();
            leafTypeSet.Add(TokenType.NumberLiteral);
            leafTypeSet.Add(TokenType.VariableIdentifier);

            // Reads and check tokens from left to right.
            for (int itoken = 0; itoken < tokenCount; itoken++)
            {
                Token token = tokens[itoken];

                // Prepare information of next/previous token.
                bool nextIsLeaf = itoken != tokenCount - 1 && leafTypeSet.Contains(tokens[itoken + 1].Type);
                bool prevIsLeaf = itoken != 0 && leafTypeSet.Contains(tokens[itoken - 1].Type);
                bool nextIsOpenParenthesis = itoken < tokenCount - 1 && tokens[itoken + 1].Word == "(";
                bool prevIsCloseParenthesis = itoken != 0 && tokens[itoken - 1].Word == ")";
                bool nextIsPrefixOperator = itoken < tokenCount - 1
                        && tokens[itoken + 1].Type == TokenType.Operator
                        && tokens[itoken + 1].Operator?.Type == OperatorType.UnaryPrefix;
                bool nextIsFunctionCallBegin = nextIsOpenParenthesis
                        && tokens[itoken + 1].Type == TokenType.Operator
                        && tokens[itoken + 1].Operator?.Type == OperatorType.Call;
                bool nextIsFunctionIdentifier = itoken < tokenCount - 1
                        && tokens[itoken + 1].Type == TokenType.FunctionIdentifier;

                // Case of operators
                if (token.Type == TokenType.Operator)
                {

                    // Cases of unary-prefix operators
                    if (token.Operator?.Type == OperatorType.UnaryPrefix)
                    {

                        // Only leafs, open parentheses, and unary-prefix operators can be operands.
                        if (!(nextIsLeaf || nextIsOpenParenthesis || nextIsPrefixOperator))
                        {
                            throw new ExevalatorException("An operand is required at the right of: \"" + token.Word + "\"");
                        }
                    } // Cases of unary-prefix operators

                    // Cases of binary operators or a separator of partial expressions
                    if (token.Operator?.Type == OperatorType.Binary || token.Word == ",")
                    {

                        // Only leaf elements, open parenthesis, and unary-prefix operator can be a right-operand.
                        if (!(nextIsLeaf || nextIsOpenParenthesis || nextIsPrefixOperator || nextIsFunctionIdentifier))
                        {
                            throw new ExevalatorException("An operand is required at the right of: \"" + token.Word + "\"");
                        }
                        // Only leaf elements and closed parenthesis can be a right-operand.
                        if (!(prevIsLeaf || prevIsCloseParenthesis))
                        {
                            throw new ExevalatorException("An operand is required at the left of: \"" + token.Word + "\"");
                        }
                    } // Cases of binary operators or a separator of partial expressions

                } // Case of operators

                // Case of leaf elements
                if (leafTypeSet.Contains(token.Type))
                {

                    // An other leaf element or an open parenthesis can not be at the right of an leaf element.
                    if (!nextIsFunctionCallBegin && (nextIsOpenParenthesis || nextIsLeaf))
                    {
                        throw new ExevalatorException("An operator is required at the right of: \"" + token.Word + "\"");
                    }

                    // An other leaf element or a closed parenthesis can not be at the left of an leaf element.
                    if (prevIsCloseParenthesis || prevIsLeaf)
                    {
                        throw new ExevalatorException("An operator is required at the left of: \"" + token.Word + "\"");
                    }
                } // Case of leaf elements
            } // Loops for each token
        } // End of this method
    }


    /// <summary>
    /// The class performing functions of a parser.
    /// </summary>
    public class Parser
    {
        /// <summary>
        /// Parses tokens and construct Abstract Syntax Tree (AST).
        /// </summary>
        /// <param name="tokens">Tokens to be parsed</param>
        /// <returns>The root node of the constructed AST</returns>
        public static AstNode Parse(Token[] tokens)
        {
            /* In this method, we use a non-recursive algorithm for the parsing.
             * Processing cost is maybe O(N), where N is the number of tokens. */

            // Number of tokens
            int tokenCount = tokens.Length;

            // Working stack to form multiple AstNode instances into a tree-shape.
            Stack<AstNode> stack = new Stack<AstNode>();

            // Tokens of temporary nodes used in the above working stack, for isolating ASTs of partial expressions.
            Token parenthesisStackLidToken = new Token(TokenType.StackLid, "(PARENTHESIS_STACK_LID)");
            Token separatorStackLidToken = new Token(TokenType.StackLid, "(SEPARATOR_STACK_LID)");
            Token callBeginStackLidToken = new Token(TokenType.StackLid, "(CALL_BEGIN_STACK_LID)");

            // The array storing next operator's precedence for each token.
            // At [i], it is stored that the precedence of the first operator of which token-index is greater than i.
            uint[] nextOperatorPrecedences = Parser.GetNextOperatorPrecedences(tokens);

            // Read tokens from left to right.
            int itoken = 0;
            do
            {
                Token token = tokens[itoken];
                AstNode? operatorNode = null;

                // Case of literals and identifiers: "1.23", "x", "f", etc.
                if (token.Type == TokenType.NumberLiteral
                        || token.Type == TokenType.VariableIdentifier
                        || token.Type == TokenType.FunctionIdentifier)
                {
                    stack.Push(new AstNode(token));
                    itoken++;
                    continue;

                }
                // Case of parenthesis: "(" or ")"
                else if (token.Type == TokenType.Parenthesis)
                {
                    if (token.Word == "(")
                    {
                        stack.Push(new AstNode(parenthesisStackLidToken));
                        itoken++;
                        continue;
                    }
                    // Case of ")"
                    else
                    {
                        operatorNode = Parser.PopPartialExprNodes(stack, parenthesisStackLidToken)[0];
                    }
                }
                // Case of separator: ","
                else if (token.Type == TokenType.ExpressionSeparator)
                {
                    stack.Push(new AstNode(separatorStackLidToken));
                    itoken++;
                    continue;
                }
                // Case of operators: "+", "-", etc.
                else if (token.Type == TokenType.Operator)
                {
                    operatorNode = new AstNode(token);
                    uint nextOpPrecedence = nextOperatorPrecedences[itoken];

                    // Case of unary-prefix operators:
                    // * Connect the node of right-token as an operand, if necessary (depending the next operator's precedence).
                    if (token.Operator!.Value.Type == OperatorType.UnaryPrefix)
                    {
                        if (Parser.ShouldAddRightOperand(token.Operator.Value.Precedence, nextOpPrecedence))
                        {
                            operatorNode.ChildNodeList.Add(new AstNode(tokens[itoken + 1]));
                            itoken++;
                        } // else: Operand will be connected later. See the bottom of this loop.

                    }
                    // Case of binary operators:
                    // * Always connect the node of left-token as an operand.
                    // * Connect the node of right-token as an operand, if necessary (depending the next operator's precedence).
                    else if (token.Operator!.Value.Type == OperatorType.Binary)
                    {
                        operatorNode.ChildNodeList.Add(stack.Pop());
                        if (Parser.ShouldAddRightOperand(token.Operator.Value.Precedence, nextOpPrecedence))
                        {
                            operatorNode.ChildNodeList.Add(new AstNode(tokens[itoken + 1]));
                            itoken++;
                        } // else: Right-operand will be connected later. See the bottom of this loop.
                    }
                    // Case of function-call operators.
                    else if (token.Operator!.Value.Type == OperatorType.Call)
                    {
                        if (token.Word == "(")
                        {
                            operatorNode.ChildNodeList.Add(stack.Pop()); // Add function-identifier node at the top of the stack.
                            stack.Push(operatorNode);
                            stack.Push(new AstNode(callBeginStackLidToken)); // The marker to correct partial expressions of args from the stack.
                            itoken++;
                            continue;
                        }
                        // Case of ")"
                        else
                        {
                            AstNode[] argNodes = Parser.PopPartialExprNodes(stack, callBeginStackLidToken);
                            operatorNode = stack.Pop();
                            foreach (AstNode argNode in argNodes)
                            {
                                operatorNode.ChildNodeList.Add(argNode);
                            }
                        }
                    }
                }

                // If the precedence of the operator at the top of the stack is stronger than the next operator,
                // connect all "unconnected yet" operands and operators in the stack.
                while (Parser.ShouldAddRightOperandToStackedOperator(stack, nextOperatorPrecedences[itoken]))
                {
                    AstNode oldOperatorNode = operatorNode!;
                    operatorNode = stack.Pop();
                    operatorNode.ChildNodeList.Add(oldOperatorNode);
                }
                stack.Push(operatorNode!);
                itoken++;

            } while (itoken < tokenCount);

            // The AST has been constructed on the stack, and only its root node is stored in the stack.
            AstNode rootNodeOfExpressionAst = stack.Pop();

            // Check that the depth of the constructed AST does not exceeds the limit.
            rootNodeOfExpressionAst.CheckDepth(1, StaticSettings.MaxAstDepth);

            return rootNodeOfExpressionAst;
        }

        /// <summary>
        /// Judges whether the right-side token should be connected directly as an operand, to the target operator.
        /// </summary>
        /// <param name="targetOperatorPrecedence">The precedence of the target operator (smaller value gives higher precedence)</param>
        /// <param name="nextOperatorPrecedence">The precedence of the next operator (smaller value gives higher precedence)</param>
        /// <returns>Returns true if the right-side token (operand) should be connected to the target operator</returns>
        private static bool ShouldAddRightOperand(uint targetOperatorPrecedence, uint nextOperatorPrecedence)
        {
            return targetOperatorPrecedence <= nextOperatorPrecedence; // left is stronger
        }

        /// <summary>
        /// Judges whether the right-side token should be connected directly as an operand,
        /// to the operator at the top of the working stack.
        /// </summary>
        /// <param name="stack" The working stack used for the parsing</param>
        /// <param name="nextOperatorPrecedence">The precedence of the next operator (smaller value gives higher precedence)</param>
        /// <returns>Returns true if the right-side token (operand) should be connected to the operator at the top of the stack</returns>
        private static bool ShouldAddRightOperandToStackedOperator(Stack<AstNode> stack, uint nextOperatorPrecedence)
        {
            if (stack.Count == 0 || stack.Peek().Token.Type != TokenType.Operator)
            {
                return false;
            }
            return ShouldAddRightOperand(stack.Peek().Token.Operator!.Value.Precedence, nextOperatorPrecedence);
        }

        /// <summary>
        /// Pops root nodes of ASTs of partial expressions constructed on the stack.
        /// In the returned array, the popped nodes are stored in FIFO order.
        /// </summary>
        /// <param name="stack">The working stack used for the parsing</param>
        /// <param name="targetStackLidToken">
        ///     The token of the temporary node pushed in the stack, at the end of partial expressions to be popped
        /// </param>
        /// <returns>Root nodes of ASTs of partial expressions</returns>
        private static AstNode[] PopPartialExprNodes(Stack<AstNode> stack, Token endStackLidToken)
        {
            if (stack.Count == 0)
            {
                throw new ExevalatorException("Unexpected end of a partial expression");
            }
            List<AstNode> partialExprNodeList = new List<AstNode>();
            while (stack.Count != 0)
            {
                if (stack.Peek().Token.Type == TokenType.StackLid)
                {
                    AstNode stackLidNode = stack.Pop();
                    if (Equals(stackLidNode.Token, endStackLidToken))
                    {
                        break;
                    }
                }
                else
                {
                    partialExprNodeList.Add(stack.Pop());
                }
            }
            int nodeCount = partialExprNodeList.Count;
            AstNode[] partialExprNodes = new AstNode[nodeCount];
            for (int inode=0; inode<nodeCount; inode++) {
                partialExprNodes[inode] = partialExprNodeList[nodeCount - inode - 1]; // Storing elements in reverse order.
            }
            return partialExprNodes;
        }

        /// <summary>
        /// Returns an array storing next operator's precedence for each token.
        /// In the returned array, it will stored at [i] that
        /// precedence of the first operator of which token-index is greater than i.
        /// </summary>
        /// <param name="tokens">All tokens to be parsed</param>
        /// <returns>The array storing next operator's precedence for each token</returns>
        private static uint[] GetNextOperatorPrecedences(Token[] tokens)
        {
            int tokenCount = tokens.Length;
            uint lastOperatorPrecedence = System.UInt32.MaxValue; // least prior
            uint[] nextOperatorPrecedences = new uint[tokenCount];

            for (int itoken = tokenCount - 1; 0 <= itoken; itoken--)
            {
                Token token = tokens[itoken];
                nextOperatorPrecedences[itoken] = lastOperatorPrecedence;

                if (token.Type == TokenType.Operator)
                {
                    lastOperatorPrecedence = token.Operator!.Value.Precedence;
                }

                if (token.Type == TokenType.Parenthesis)
                {
                    if (token.Word == "(")
                    {
                        lastOperatorPrecedence = 0; // most prior
                    }
                    else
                    { // case of ")"
                        lastOperatorPrecedence = System.UInt32.MaxValue; // least prior
                    }
                }
            }
            return nextOperatorPrecedences;
        }
    }


    /// <summary>
    /// The enum representing types of operators.
    /// </summary>
    public enum OperatorType
    {

        /// <summary>Represents unary operator, for example: - of -1.23</summary>
        UnaryPrefix,

        /// <summary>Represents binary operator, for example: + of 1+2</summary>
        Binary,

        /// <summary>Represents function-call operator</summary>
        Call,
    }

    /// <summary>
    /// The struct storing information of an operator.
    /// </summary>
    public struct Operator
    {

        /// <summary>The type of operator tokens.</summary>
        public readonly OperatorType Type;

        /// <summary>The symbol of this operator (for example: '+').</summary>
        public readonly char Symbol;

        /// <summary>The precedence of this operator (smaller value gives higher precedence).</summary>
        public readonly uint Precedence;

        /// <summary>
        /// Create an Operator instance storing specified information.
        /// </summary>
        /// <param name="type">The type of this operator</param>
        /// <param name="symbol">The symbol of this operator</param>
        /// <param name="precedence">The precedence of this operator</param>
        public Operator(OperatorType type, char symbol, uint precedence)
        {
            this.Type = type;
            this.Symbol = symbol;
            this.Precedence = precedence;
        }

        /// <summary>
        /// Returns whether this instance equals to the specified "compared" instance.
        /// </summary>
        /// <param name="compared">The instance to be compared with this instance</param>
        /// <returns>true if this instance equals to the specified "compared" instance</returns>
        public override bool Equals(object? compared)
        {
            if (!(compared is Operator))
            {
                return false;
            }
            Operator comparedOperator = (Operator)compared;
            return this.Type == comparedOperator.Type
                && this.Symbol == comparedOperator.Symbol
                && this.Precedence == comparedOperator.Precedence;
        }

        public static bool operator ==(Operator leftOperator, Operator rightOperator)
        {
            return leftOperator.Equals(rightOperator);
        }

        public static bool operator !=(Operator leftOperator, Operator rightOperator)
        {
            return !leftOperator.Equals(rightOperator);
        }

        /// <summary>
        /// Generates the hash-code of this instance.
        /// </summary>
        /// <returns>The hash-code of this instance</returns>
        public override int GetHashCode()
        {
            return this.Type.GetHashCode() ^ this.Symbol.GetHashCode() ^ this.Precedence.GetHashCode();
        }

        /// <summary>
        /// Returns the String representation of this Operator instance.
        /// </summary>
        /// <returns>The String representation of this Operator instance</returns>
        public override string ToString()
        {
            return "Operator [Symbol=" + this.Symbol +
                   ", Precedence=" + this.Precedence +
                   ", Type=" + this.Type +
                   "]";
        }
    }


    /// <summary>
    /// The enum representing types of tokens.
    /// </summary>
    public enum TokenType
    {

        /// <summary>Represents number literal tokens, for example: 1.23</summary>
        NumberLiteral,

        /// <summary>Represents operator tokens, for example: +</summary>
        Operator,

        /// <summary>Represents separator tokens of partial expressions: ,</summary>
        ExpressionSeparator,

        /// <summary>Represents parenthesis, for example: ( and ) of (1*(2+3))</summary>
        Parenthesis,

        /// <summary>Represents variable-identifier tokens, for example: x</summary>
        VariableIdentifier,

        /// <summary>Represents function-identifier tokens, for example: f</summary>
        FunctionIdentifier,

        /// <summary>Represents temporary token for isolating partial expressions in the stack, in parser</summary>
        StackLid,
    }

    /// <summary>
    /// The struct storing information of a token.
    /// </summary>
    public struct Token
    {

        /// <summary>The type of this token.</summary>
        public readonly TokenType Type;

        /// <summary>The text representation of this token.</summary>
        public readonly String Word;

        /// <summary>The detailed information of the operator, if the type of this token is OPERATOR.</summary>
        public readonly Operator? Operator;

        /// <summary>
        /// Create an Token instance storing specified information.
        /// </summary>
        /// <param name="type">The type of this token</param>
        /// <param name="word">The text representation of this token</param>
        public Token(TokenType type, String word)
        {
            this.Type = type;
            this.Word = word;
            this.Operator = null;
        }

        /// <summary>
        /// Create an Token instance storing specified information.
        /// </summary>
        /// <param name="type">The type of this token</param>
        /// <param name="word">The text representation of this token</param>
        /// <param name="op">The detailed information of the operator, for OPERATOR type tokens</param>
        public Token(TokenType type, String word, Operator op)
        {
            this.Type = type;
            this.Word = word;
            this.Operator = op;
        }

        /// <summary>
        /// Returns whether this instance equals to the specified "compared" instance.
        /// </summary>
        /// <param name="compared">The instance to be compared with this instance</param>
        /// <returns>true if this instance equals to the specified "compared" instance</returns>
        public override bool Equals(object? compared)
        {
            if (!(compared is Token))
            {
                return false;
            }
            Token comparedToken = (Token)compared;
            if (this.Operator.HasValue && comparedToken.Operator.HasValue)
            {
                return this.Type == comparedToken.Type
                    && this.Word == comparedToken.Word
                    && Equals(this.Operator.Value, comparedToken.Operator.Value);
            }
            else if (!this.Operator.HasValue && !comparedToken.Operator.HasValue)
            {
                return this.Type == comparedToken.Type
                    && this.Word == comparedToken.Word;
            }
            else
            {
                return false;
            }
        }

        public static bool operator ==(Token leftToken, Token rightToken)
        {
            return leftToken.Equals(rightToken);
        }

        public static bool operator !=(Token leftToken, Token rightToken)
        {
            return !leftToken.Equals(rightToken);
        }

        /// <summary>
        /// Generates the hash-code of this instance.
        /// </summary>
        /// <returns>The hash-code of this instance</returns>
        public override int GetHashCode()
        {
            int hashCode = this.Type.GetHashCode() ^ this.Word.GetHashCode();
            if (this.Operator.HasValue)
            {
                hashCode ^= this.Operator.GetHashCode();
            }
            return hashCode;
        }

        /// <summary>
        /// Returns the String representation of this Operator instance.
        /// </summary>
        /// <returns>The String representation of this Operator instance</returns>
        public override string ToString()
        {
            if (this.Operator == null)
            {
                return "Token [Type=" + this.Type +
                       ", Word=" + this.Word +
                       "]";
            }
            else
            {
                return "Token [Type=" + this.Type +
                       ", Word=" + this.Word +
                       ", Operator.Type=" + this.Operator?.Type +
                       ", Operator.Precedence=" + this.Operator?.Precedence +
                       "]";
            }
        }
    }


    /// <summary>
    /// The class storing information of an node of an AST.
    /// </summary>
    public class AstNode
    {

        /// <summary>The token corresponding with this AST node.</summary>
        public readonly Token Token;

        /// <summary>The list of child nodes of this AST node.</summary>
        public List<AstNode> ChildNodeList;

        /// <summary>
        /// Create an AST node instance storing specified information.
        /// </summary> 
        /// <param name="token">The token corresponding with this AST node</param>
        public AstNode(Token token)
        {
            this.Token = token;
            this.ChildNodeList = new List<AstNode>();
        }

        /// <summary>
        /// Checks that depths in the AST of all nodes under this node (child nodes, grandchild nodes, and so on)
        /// does not exceeds the specified maximum value.
        /// An ExevalatorException will be thrown when the depth exceeds the maximum value.
        /// If the depth does not exceeds the maximum value, nothing will occur.
        /// </summary>
        /// <param name="depthOfThisNode">The depth of this node in the AST</param>
        /// <param name="maxAstDepth">The maximum value of the depth of the AST</param>
        public void CheckDepth(int depthOfThisNode, int maxAstDepth)
        {
            if (maxAstDepth < depthOfThisNode)
            {
                throw new ExevalatorException(
                    "The depth of the AST exceeds the limit (StaticSettings.MaxAstDepth: "
                    + StaticSettings.MaxAstDepth + ")"
                );
            }
            foreach (AstNode childNode in this.ChildNodeList)
            {
                childNode.CheckDepth(depthOfThisNode + 1, maxAstDepth);
            }
        }

        /// <summary>
        /// Expresses the AST under this node in XML-like text format.
        /// </summary>
        /// <param name="indentStage">The stage of indent of this node</param>
        /// <returns>XML-like text representation of the AST under this node</returns>
        public String ToMarkuppedText(int indentStage = 0)
        {
            StringBuilder indentBuilder = new StringBuilder();
            for (int istage = 0; istage < indentStage; istage++)
            {
                indentBuilder.Append(StaticSettings.AstIndent);
            }
            string indent = indentBuilder.ToString();
            string eol = Environment.NewLine;
            StringBuilder resultBuilder = new StringBuilder();

            resultBuilder.Append(indent);
            resultBuilder.Append("<");
            resultBuilder.Append(this.Token.Type);
            resultBuilder.Append(" word=\"");
            resultBuilder.Append(this.Token.Word);
            resultBuilder.Append("\"");
            if (this.Token.Type == TokenType.Operator)
            {
                resultBuilder.Append(" optype=\"");
                resultBuilder.Append(this.Token.Operator?.Type);
                resultBuilder.Append("\" precedence=\"");
                resultBuilder.Append(this.Token.Operator?.Precedence);
                resultBuilder.Append("\"");
            }

            if (0 < this.ChildNodeList.Count)
            {
                resultBuilder.Append(">");
                foreach (AstNode childNode in this.ChildNodeList)
                {
                    resultBuilder.Append(eol);
                    resultBuilder.Append(childNode.ToMarkuppedText(indentStage + 1));
                }
                resultBuilder.Append(eol);
                resultBuilder.Append(indent);
                resultBuilder.Append("</");
                resultBuilder.Append(this.Token.Type);
                resultBuilder.Append(">");

            }
            else
            {
                resultBuilder.Append(" />");
            }

            return resultBuilder.ToString();
        }
    }

    /// <summary>
    /// The class for evaluating the value of an AST.
    /// </summary>
    public class Evaluator
    {
        /// <summary>The tree of evaluator nodes, which evaluates an expression.</summary>
        private EvaluatorNode? evaluatorNodeTree = null;

        /// <summary>
        /// Updates the state to evaluate the value of the AST.
        /// </summary>
        /// <param name="ast">The root node of the AST.</param>
        /// <param name="variableTable"> The Map mapping each variable name to an address of the variable.</param>
        /// <param name="functionTable"> The Map mapping each function name to an IExevalatorFunction instance.</param>
        public void Update(
                AstNode ast,
                Dictionary<String, int> variableTable,
                Dictionary<String, IExevalatorFunction> functionTable)
        {
            this.evaluatorNodeTree = Evaluator.CreateEvaluatorNodeTree(ast, variableTable, functionTable);
        }

        /// <summary>
        /// Returns whether "evaluate" method is available on the current state.
        /// </summary>
        /// <returns>Return value - True if "evaluate" method is available.</returns>
        public bool IsEvaluatable()
        {
            return this.evaluatorNodeTree != null;
        }

        /// <summary>
        /// Evaluates the value of the AST set by "update" method.
        /// </summary>
        /// <param name="memory">The Vec used as as a virtual memory storing values of variables.</param>
        /// <returns>The evaluated value.</returns>
        public double Evaluate(List<double> memory)
        {
            if (this.evaluatorNodeTree == null)
            {
                throw new ExevalatorException("The Evaluator is not initialized but \"evaluate\" method is called.");
            }
            else
            {
                return this.evaluatorNodeTree.Evaluate(memory);
            }
        }

        /// <summary>
        /// Creates a tree of evaluator nodes corresponding with the specified AST.
        /// </summary>
        /// <param name="ast">The root node of the AST.</param>
        /// <param name="variableTable">The Dictionary mapping each variable name to an address of the variable.</param>
        /// <param name="functionTable">The Dictionary mapping each function name to an IExevalatorFunction instance.</param>
        /// <returns>The root node of the created tree of evaluator nodes.</returns>
        public static Evaluator.EvaluatorNode CreateEvaluatorNodeTree(
                AstNode ast,
                Dictionary<string, int> variableTable,
                Dictionary<string, IExevalatorFunction> functionTable)
        {
            // Note: This method creates a tree of evaluator nodes by traversing each node in the AST recursively.

            List<AstNode> childNodeList = ast.ChildNodeList;
            int childCount = childNodeList.Count;

            // Creates evaluation nodes of child nodes, and store then into an array.
            Evaluator.EvaluatorNode[] childNodeNodes = new Evaluator.EvaluatorNode[childCount];
            for (int ichild = 0; ichild < childCount; ichild++)
            {
                childNodeNodes[ichild] = CreateEvaluatorNodeTree(childNodeList[ichild], variableTable, functionTable);
            }

            // Initialize evaluation nodes of this node.
            Token token = ast.Token;
            if (token.Type == TokenType.NumberLiteral)
            {
                double literalValue = System.Double.NaN;
                if (!System.Double.TryParse(token.Word, out literalValue))
                {
                    throw new ExevalatorException("Invalid number literal: " + token.Word);
                }
                return new Evaluator.NumberLiteralEvaluatorNode(literalValue);

            }
            else if (token.Type == TokenType.VariableIdentifier)
            {
                if (!variableTable.ContainsKey(token.Word))
                {
                    throw new ExevalatorException("Variable not found: " + token.Word);
                }
                int address = variableTable[token.Word];
                return new Evaluator.VariableEvaluatorNode(address);

            }
            else if (token.Type == TokenType.FunctionIdentifier)
            {
                return new Evaluator.NopEvaluatorNode();
            }
            else if (token.Type == TokenType.Operator)
            {
                Operator op = token.Operator!.Value;
                if (op.Type == OperatorType.UnaryPrefix && op.Symbol == '-')
                {
                    return new Evaluator.MinusEvaluatorNode(childNodeNodes[0]);
                }
                else if (op.Type == OperatorType.Binary && op.Symbol == '+')
                {
                    return new Evaluator.AdditionEvaluatorNode(childNodeNodes[0], childNodeNodes[1]);
                }
                else if (op.Type == OperatorType.Binary && op.Symbol == '-')
                {
                    return new Evaluator.SubtractionEvaluatorNode(childNodeNodes[0], childNodeNodes[1]);
                }
                else if (op.Type == OperatorType.Binary && op.Symbol == '*')
                {
                    return new Evaluator.MultiplicationEvaluatorNode(childNodeNodes[0], childNodeNodes[1]);
                }
                else if (op.Type == OperatorType.Binary && op.Symbol == '/')
                {
                    return new Evaluator.DivisionEvaluatorNode(childNodeNodes[0], childNodeNodes[1]);
                }
                else if (op.Type == OperatorType.Call && op.Symbol == '(')
                {
                    String identifier = childNodeList[0].Token.Word;
                    if (!functionTable.ContainsKey(identifier))
                    {
                        throw new ExevalatorException("Function not found: " + identifier);
                    }
                    IExevalatorFunction function = functionTable[identifier];
                    Evaluator.EvaluatorNode[] argNodes = new Evaluator.EvaluatorNode[childCount - 1];
                    for (int iarg = 0; iarg < childCount - 1; iarg++)
                    {
                        argNodes[iarg] = childNodeNodes[iarg + 1];
                    }
                    return new Evaluator.FunctionEvaluatorNode(function, argNodes);
                }
                else
                {
                    throw new ExevalatorException("Unexpected operator: " + op);
                }
            }
            else
            {
                throw new ExevalatorException("Unexpected token type: " + token.Type);
            }
        }

        /// <summary>
        /// The super class of evaluator nodes.
        /// </summary>
        public abstract class EvaluatorNode
        {

            /// <summary>
            /// Evaluate the value of this node.
            /// </summary>
            /// <param name="memory">The List storing values of variables.</param>
            /// <returns>The result value of the evaluation</returns>
            public abstract double Evaluate(List<double> memory);
        }

        /// <summary>
        /// The super class of evaluator nodes of binary operations.
        /// </summary>
        public abstract class BinaryOperationEvaluatorNode : EvaluatorNode
        {

            /// <summary>The nofr for evaluating the right-side operand.</summary>
            protected readonly EvaluatorNode LeftOperandNode;

            /// <summary>The node for evaluating the left-side operand.</summary>
            protected readonly EvaluatorNode RightOperandNode;

            /// <summary>
            /// Initializes operands.
            /// </summary>
            /// <param name="leftOperandNode">The node for evaluating the left-side operand</param>
            /// <param name="rightOperandNode">The node for evaluating the right-side operand</param>
            protected BinaryOperationEvaluatorNode(EvaluatorNode leftOperandNode, EvaluatorNode rightOperandNode)
            {
                this.LeftOperandNode = leftOperandNode;
                this.RightOperandNode = rightOperandNode;
            }
        }

        /// <summary>
        /// The evaluator node for evaluating the value of a addition operator.
        /// </summary>
        public class AdditionEvaluatorNode : BinaryOperationEvaluatorNode
        {
            /// <summary>
            /// Initializes operands.
            /// </summary>
            /// <param name="leftOperandNode">The node for evaluating the left-side operand</param>
            /// <param name="rightOperandNode">The node for evaluating the right-side operand</param>
            public AdditionEvaluatorNode(EvaluatorNode leftOperandNode, EvaluatorNode rightOperandNode)
            : base(leftOperandNode, rightOperandNode)
            {
            }

            /// <summary>
            /// Performs the addition.
            /// </summary>
            /// <param name="memory">The List storing values of variables.</param>
            /// <returns>The result value of the addition</returns>
            public override double Evaluate(List<double> memory)
            {
                return this.LeftOperandNode.Evaluate(memory) + this.RightOperandNode.Evaluate(memory);
            }
        }

        /// <summary>
        /// The evaluator node for evaluating the value of a subtraction operator.
        /// </summary>
        public class SubtractionEvaluatorNode : BinaryOperationEvaluatorNode
        {
            /// <summary>
            /// Initializes operands.
            /// </summary>
            /// <param name="leftOperandNode">The node for evaluating the left-side operand</param>
            /// <param name="rightOperandNode">The node for evaluating the right-side operand</param>
            public SubtractionEvaluatorNode(EvaluatorNode leftOperandNode, EvaluatorNode rightOperandNode)
            : base(leftOperandNode, rightOperandNode)
            {
            }

            /// <summary>
            /// Performs the subtraction.
            /// </summary>
            /// <param name="memory">The List storing values of variables.</param>
            /// <returns>The result value of the subtraction</returns>
            public override double Evaluate(List<double> memory)
            {
                return this.LeftOperandNode.Evaluate(memory) - this.RightOperandNode.Evaluate(memory);
            }
        }

        /// <summary>
        /// The evaluator node for evaluating the value of a multiplication operator.
        /// </summary>
        public class MultiplicationEvaluatorNode : BinaryOperationEvaluatorNode
        {
            /// <summary>
            /// Initializes operands.
            /// </summary>
            /// <param name="leftOperandNode">The node for evaluating the left-side operand</param>
            /// <param name="rightOperandNode">The node for evaluating the right-side operand</param>
            public MultiplicationEvaluatorNode(EvaluatorNode leftOperandNode, EvaluatorNode rightOperandNode)
            : base(leftOperandNode, rightOperandNode)
            {
            }

            /// <summary>
            /// Performs the multiplication.
            /// </summary>
            /// <param name="memory">The List storing values of variables.</param>
            /// <returns>The result value of the multiplication</returns>
            public override double Evaluate(List<double> memory)
            {
                return this.LeftOperandNode.Evaluate(memory) * this.RightOperandNode.Evaluate(memory);
            }
        }

        /// <summary>
        /// The evaluator node for evaluating the value of a division operator.
        /// </summary>
        public class DivisionEvaluatorNode : BinaryOperationEvaluatorNode
        {
            /// <summary>
            /// Initializes operands.
            /// </summary>
            /// <param name="leftOperandNode">The node for evaluating the left-side operand</param>
            /// <param name="rightOperandNode">The node for evaluating the right-side operand</param>
            public DivisionEvaluatorNode(EvaluatorNode leftOperandNode, EvaluatorNode rightOperandNode)
            : base(leftOperandNode, rightOperandNode)
            {
            }

            /// <summary>
            /// Performs the division.
            /// </summary>
            /// <param name="memory">The List storing values of variables.</param>
            /// <returns>The result value of the division</returns>
            public override double Evaluate(List<double> memory)
            {
                return this.LeftOperandNode.Evaluate(memory) / this.RightOperandNode.Evaluate(memory);
            }
        }

        /// <summary>
        /// The evaluator node for evaluating the value of a unary-minus operator.
        /// </summary>
        public class MinusEvaluatorNode : EvaluatorNode
        {
            /// <summary>The node for evaluating the operand.</summary>
            private readonly EvaluatorNode OperandNode;

            /// <summary>
            /// Initializes the operand.
            /// </summary>
            /// <param name="operandNode">The node for evaluating the operand</param>
            public MinusEvaluatorNode(EvaluatorNode operandNode)
            {
                this.OperandNode = operandNode;
            }

            /// <summary>
            /// Performs the unary-minus operation.
            /// </summary>
            /// <param name="memory">The List storing values of variables.</param>
            /// <returns>The result value of the unary-minus operation</returns>
            public override double Evaluate(List<double> memory)
            {
                return -this.OperandNode.Evaluate(memory);
            }
        }

        /// <summary>
        /// The evaluator node for evaluating the value of a number literal.
        /// </summary>
        public class NumberLiteralEvaluatorNode : EvaluatorNode
        {
            /// <summary>The value of the number literal.</summary>
            private readonly double LiteralValue;

            /// <summary>
            /// Initializes the value of the number literal.
            /// </summary>
            /// <param name="literalValue">The value of the number literal</param>
            public NumberLiteralEvaluatorNode(double literalValue)
            {
                this.LiteralValue = literalValue;
            }

            /// <summary>
            /// Returns the value of the number literal.
            /// </summary>
            /// <param name="memory">The List storing values of variables.</param>
            /// <returns>The value of the number literal</returns>
            public override double Evaluate(List<double> memory)
            {
                return this.LiteralValue;
            }
        }

        /// <summary>
        /// The evaluator node for evaluating the value of a variable.
        /// </summary>
        public class VariableEvaluatorNode : EvaluatorNode
        {
            /// <summary>The address of the variable.</summary>
            private readonly int Address;

            /// <summary>
            /// Initializes the address of the variable.
            /// </summary>
            /// <param name="address">The address of the variable</param>
            public VariableEvaluatorNode(int address)
            {
                this.Address = address;
            }

            /// <summary>
            /// Returns the value of the variable.
            /// </summary>
            /// <param name="memory">The List storing values of variables.</param>
            /// <returns>The value of the variable</returns>
            public override double Evaluate(List<double> memory)
            {
                if (this.Address < 0 || memory.Count <= this.Address)
                {
                    throw new ExevalatorException("Invalid variable address: " + this.Address);
                }
                return memory[this.Address];
            }
        }

        /// <summary>
        /// The evaluator node for evaluating the value of a function-call operator.
        /// </summary>
        public class FunctionEvaluatorNode : EvaluatorNode
        {
            /// <summary>The address of the variable.</summary>
            private readonly IExevalatorFunction Function;

            /// <summary>Evaluator nodes for evaluating values of arguments.</summary>
            private readonly EvaluatorNode[] ArgumentEvalNodes;

            /// <summary>An array storing evaluated values of arguments.</summary>
            private double[] ArgumentArrayBuffer;

            /// <summary>
            /// Initializes the function to be called.
            /// </summary>
            /// <param name="function">The function to be called</param>
            /// <param name="argumentEvalNodes">Evaluator nodes for evaluating values of arguments</param>
            public FunctionEvaluatorNode(IExevalatorFunction function, EvaluatorNode[] argumentEvalNodes)
            {
                this.Function = function;
                this.ArgumentEvalNodes = argumentEvalNodes;
                this.ArgumentArrayBuffer = new double[this.ArgumentEvalNodes.Length];
            }

            /// <summary>
            /// Calls the function and returns the returned value of the function.
            /// </summary>
            /// <param name="memory">The List storing values of variables.</param>
            /// <returns>The returned value of the function</returns>
            public override double Evaluate(List<double> memory)
            {
                for (int iarg = 0; iarg < this.ArgumentEvalNodes.Length; iarg++)
                {
                    this.ArgumentArrayBuffer[iarg] = this.ArgumentEvalNodes[iarg].Evaluate(memory);
                }
                return this.Function.Invoke(this.ArgumentArrayBuffer);
            }
        }

        /// <summary>
        /// The evaluator node for evaluating nothing.
        /// </summary>
        public class NopEvaluatorNode : EvaluatorNode
        {
            /// <summary>
            /// Creates an instance.
            /// </summary>
            public NopEvaluatorNode()
            {
            }

            /// <summary>
            /// Performs nothing.
            /// </summary>
            /// <returns>The value of NaN.</returns>
            public override double Evaluate(List<double> memory)
            {
                return System.Double.NaN;
            }
        }
    }



    /// <summary>
    /// The struct defining static setting values.
    /// </summary>
    public struct StaticSettings
    {

        /// <summary>The maximum number of characters in an expression.</summary>
        public const int MaxExpressionCharCount = 256;

        /// <summary>The maximum number of characters of variable/function names.</summary>
        public const int MaxNameCharCount = 64;

        /// <summary>The maximum number of tokens in an expression.</summary>
        public const int MaxTokenCount = 64;

        /// <summary>The maximum depth of an Abstract Syntax Tree (AST).</summary>
        public const int MaxAstDepth = 32;

        /// <summary>The indent used in text representations of ASTs.</summary>
        public const string AstIndent = "  ";

        /// <summary>The regular expression of number literals.</summary>
        public const string NumberLiteralRegex =
            "(?<=(\\s|\\+|-|\\*|/|\\(|\\)|,|^))" + // Token splitters, or beginning of the expression
            "([0-9]+(\\.[0-9]+)?)" +               // Significand part
            "((e|E)(\\+|-)?[0-9]+)?";              // Exponent part

        /// <summary>The escaped representation of number literals in expressions.</summary>
        public const string EscapedNumberLiteral = "@NUMBER_LITERAL@";

        /// <summary>The HashSet of symbols of available operators.</summary>
        public static readonly HashSet<char> OperatorSymbolSet;

        /// <summary>The Dictionary mapping each symbol of a binary operator to a Operator struct.</summary>
        public static readonly Dictionary<char, Operator> BinarySymbolOperatorDict;

        /// <summary>The Dictionary mapping each symbol of a unary-prefix operator to a Operator struct.</summary>
        public static readonly Dictionary<char, Operator> UnaryPrefixSymbolOperatorDict;

        /// <summary>The Dictionary mapping each symbol of a function-call operator to a Operator struct.</summary>
        public static readonly Dictionary<char, Operator> CallSymbolOperatorDict;

        /// <summary>The List of symbols to split an expression into tokens.</summary>
        public static readonly List<char> TokenSplitterSymbolList;

        /// <summary>Initializes values of static-readonly members.</summary>
        static StaticSettings()
        {
            Operator additionOperator = new Operator(OperatorType.Binary, '+', 400);
            Operator subtractionOperator = new Operator(OperatorType.Binary, '-', 400);
            Operator multiplicationOperator = new Operator(OperatorType.Binary, '*', 300);
            Operator divisionOperator = new Operator(OperatorType.Binary, '/', 300);
            Operator minusOperator = new Operator(OperatorType.UnaryPrefix, '-', 200);
            Operator callBeginOperator = new Operator(OperatorType.Call, '(', 100);
            Operator callEndOperator = new Operator(OperatorType.Call, ')', System.UInt32.MaxValue); // least prior

            OperatorSymbolSet = new HashSet<char>();
            OperatorSymbolSet.Add('+');
            OperatorSymbolSet.Add('-');
            OperatorSymbolSet.Add('*');
            OperatorSymbolSet.Add('/');
            OperatorSymbolSet.Add('(');
            OperatorSymbolSet.Add(')');

            BinarySymbolOperatorDict = new Dictionary<char, Operator>();
            BinarySymbolOperatorDict.Add('+', additionOperator);
            BinarySymbolOperatorDict.Add('-', subtractionOperator);
            BinarySymbolOperatorDict.Add('*', multiplicationOperator);
            BinarySymbolOperatorDict.Add('/', divisionOperator);

            UnaryPrefixSymbolOperatorDict = new Dictionary<char, Operator>();
            UnaryPrefixSymbolOperatorDict.Add('-', minusOperator);

            CallSymbolOperatorDict = new Dictionary<char, Operator>();
            CallSymbolOperatorDict.Add('(', callBeginOperator);
            CallSymbolOperatorDict.Add(')', callEndOperator);

            TokenSplitterSymbolList = new List<char>();
            TokenSplitterSymbolList.Add('+');
            TokenSplitterSymbolList.Add('-');
            TokenSplitterSymbolList.Add('*');
            TokenSplitterSymbolList.Add('/');
            TokenSplitterSymbolList.Add('(');
            TokenSplitterSymbolList.Add(')');
            TokenSplitterSymbolList.Add(',');
        }
    }
}
