' Exevalator Ver.2.1.0 - by RINEARN 2021-2024
' This software is released under the "Unlicense" license.
' You can choose the "CC0" license instead, if you want.

' Put this code in your source code folder, and import it as
' "using Rinearn.ExevalatorCS;" from a source file in which you want to use Exevalator.

Imports System
Imports System.Collections.Generic
Imports System.Text.RegularExpressions
Imports System.Text
Imports System.Globalization

Namespace Rinearn.ExevalatorVB

    ''' <summary>
    ''' Error messages of ExevalatorException,
    ''' which is thrown by Exevalator if the input expression is syntactically incorrect, or uses undeclared variables/functions.
    ''' You can customize the error message of Exevalator by modifying the values of the properties of this class.
    ''' </summary>
    Public Structure ErrorMessages
        Public Const EMPTY_EXPRESSION As String = "The inputted expression is empty."
        Public Const TOO_MANY_TOKENS As String = "The number of tokens exceeds the limit (StaticSettings.MaxTokenCount: '$0')"
        Public Const DEFICIENT_OPEN_PARENTHESIS As String = "The number of open parentheses '(' is deficient."
        Public Const DEFICIENT_CLOSED_PARENTHESIS As String = "The number of closed parentheses ')' is deficient."
        Public Const EMPTY_PARENTHESIS As String = "The content of parentheses '()' should not be empty."
        Public Const RIGHT_OPERAND_REQUIRED As String = "An operand is required at the right of: '$0'"
        Public Const LEFT_OPERAND_REQUIRED As String = "An operand is required at the left of: '$0'"
        Public Const RIGHT_OPERATOR_REQUIRED As String = "An operator is required at the right of: '$0'"
        Public Const LEFT_OPERATOR_REQUIRED As String = "An operator is required at the left of: '$0'"
        Public Const UNKNOWN_UNARY_PREFIX_OPERATOR As String = "Unknown unary-prefix operator: '$0'"
        Public Const UNKNOWN_BINARY_OPERATOR As String = "Unknown binary operator: '$0'"
        Public Const UNKNOWN_OPERATOR_SYNTAX As String = "Unknown operator syntax: '$0'"
        Public Const EXCEEDS_MAX_AST_DEPTH As String = "The depth of the AST exceeds the limit (StaticSettings.MaxAstDepth: '$0')"
        Public Const UNEXPECTED_PARTIAL_EXPRESSION As String = "Unexpected end of a partial expression"
        Public Const INVALID_NUMBER_LITERAL As String = "Invalid number literal: '$0'"
        Public Const INVALID_MEMORY_ADDRESS As String = "Invalid memory address: '$0'"
        Public Const FUNCTION_ERROR As String = "Function Error ('$0'): $1"
        Public Const VARIABLE_NOT_FOUND As String = "Variable not found: '$0'"
        Public Const FUNCTION_NOT_FOUND As String = "Function not found: '$0'"
        Public Const UNEXPECTED_OPERATOR As String = "Unexpected operator: '$0'"
        Public Const UNEXPECTED_TOKEN As String = "Unexpected token: '$0'"
        Public Const TOO_LONG_EXPRESSION As String = "The length of the expression exceeds the limit (StaticSettings.MaxExpressionCharCount: '$0')"
        Public Const UNEXPECTED_ERROR As String = "Unexpected error occurred: $0"
        Public Const REEVAL_NOT_AVAILABLE As String = """reeval"" is not available before using ""eval"""
        Public Const TOO_LONG_VARIABLE_NAME As String = "The length of the variable name exceeds the limit (StaticSettings.MaxNameCharCount: '$0')"
        Public Const TOO_LONG_FUNCTION_NAME As String = "The length of the function name exceeds the limit (StaticSettings.MaxNameCharCount: '$0')"
        Public Const VARIABLE_ALREADY_DECLARED As String = "The variable '$0' is already declared"
        Public Const FUNCTION_ALREADY_CONNECTED As String = "The function '$0' is already connected"
        Public Const INVALID_VARIABLE_ADDRESS As String = "Invalid memory address: '$0'"
    End Structure


    ''' <summary>
    ''' Interpreter Engine of Exevalator.
    ''' </summary>
    Public Class Exevalator

        ''' <summary>The List used as as a virtual memory storing values of variables.</summary>
        Dim Memory As List(Of Double)

        ''' <summary>The object evaluating the value of the expression.</summary>
        Dim Evaluator As Evaluator

        ''' <summary>The Dictionary mapping each variable name to an address of the variable.</summary>
        Dim VariableTable As Dictionary(Of String, Integer)

        ''' <summary>The Dictionary mapping each function name to an IExevalatorFunction instance.</summary>
        Dim FunctionTable As Dictionary(Of string, IExevalatorFunction)

        ''' <summary>Caches the content of the expression evaluated last time, to skip re-parsing.</summary>
        Dim LastEvaluatedExpression As String

        ''' <summary>
        ''' Creates a new interpreter of the Exevalator.
        ''' </summary>
        Public Sub New()
            Me.Memory = New List(Of Double)()
            Me.Evaluator = New Evaluator()
            Me.VariableTable = New Dictionary(Of String, Integer)()
            Me.FunctionTable = New Dictionary(Of string, IExevalatorFunction)()
            Me.LastEvaluatedExpression = ""
        End Sub

        ''' <summary>
        ''' Evaluates (computes) the value of an expression.
        ''' </summary>
        ''' <param name="expression">The expression to be evaluated</param>
        ''' <returns>The evaluated value</returns>
        Public Function Eval(expression As String) As Double
            If expression Is Nothing Then
                Throw New NullReferenceException()
            End If
            if StaticSettings.MaxExpressionCharCount < expression.Length Then
                Throw New ExevalatorException( _
                    ErrorMessages.TOO_LONG_EXPRESSION.Replace("$0", StaticSettings.MaxExpressionCharCount.ToString()) _
                )
            End If

            Try
                ' If the expression changed from the last-evaluated expression, re-parsing is necessary.
                Dim expressionChanged As Boolean = Not String.Equals(expression, Me.LastEvaluatedExpression)
                If expressionChanged OrElse Not Me.Evaluator.IsEvaluatable() Then

                    ' Perform lexical analysis, and get tokens from the inputted expression.
                    Dim tokens() As Token = LexicalAnalyzer.Analyze(expression)

                    ' Temporary, for debugging tokens
                    ' For Each token As Token in tokens
                    '     Console.WriteLine(token)
                    ' Next

                    ' Perform parsing, and get AST(Abstract Syntax Tree) from tokens.
                    Dim ast As AstNode = Parser.Parse(tokens)

                    ' Temporary, for debugging ast
                    ' Console.WriteLine(ast.ToMarkuppedText())

                    ' Update the evaluator, to evaluate the parsed AST.
                    Me.Evaluator.Update(ast, Me.VariableTable, Me.FunctionTable)

                    Me.LastEvaluatedExpression = expression
                End If

                ' Evaluate the value of the expression, and return it.
                Dim evaluatedValue As Double = Me.Evaluator.Evaluate(Me.Memory)
                Return evaluatedValue

            Catch e As Exception
                If TypeOf e Is ExevalatorException Then
                    Throw
                Else
                    Throw New ExevalatorException(ErrorMessages.UNEXPECTED_ERROR.replace("$0", e.Message), e)
                End If
            End Try
        End Function

        ''' <summary>
        ''' Re-evaluates (re-computes) the value of the expression evaluated by "eval" method last time.
        ''' This method may (slightly) work faster than calling "eval" method repeatedly for the same expression.
        ''' Note that, the result value may differ from the last evaluated value, 
        ''' if values of variables or behaviour of functions had changed.
        ''' </summary>
        ''' <returns>The evaluated value</returns>
        Public Function Reeval() As Double
            If Me.Evaluator.IsEvaluatable() Then
                Dim evaluatedValue As Double = Me.Evaluator.Evaluate(Me.Memory)
                Return evaluatedValue
            Else
                Throw New ExevalatorException(ErrorMessages.REEVAL_NOT_AVAILABLE)
            End If
        End Function

        ''' <summary>
        ''' Declares a new variable, for using the value of it in expressions.
        ''' </summary>
        ''' <param name="name">The name of the variable to be declared</param>
        ''' <returns>
        '''     The virtual address of the declared variable,
        '''     which useful for accessing to the variable faster.
        '''     See "WriteVariableAt" and "ReadVariableAt" method.
        ''' </returns>
        Public Function DeclareVariable(name As String) As Integer
            If name Is Nothing Then
                Throw New NullReferenceException()
            End If
            If StaticSettings.MaxNameCharCount < name.Length Then
                Throw New ExevalatorException( _
                    ErrorMessages.TOO_LONG_VARIABLE_NAME.Replace("$0", StaticSettings.MaxNameCharCount.ToString()) _
                )
            End If
            If Me.VariableTable.ContainsKey(name) Then
                Throw New ExevalatorException(ErrorMessages.VARIABLE_ALREADY_DECLARED.Replace("$0", name))
            End If
            Dim address As Integer = Me.Memory.Count
            Me.Memory.Add(0.0)
            Me.VariableTable(name) = address
            Return address
        End Function

        ''' <summary>
        ''' Writes the value to the variable having the specified name.
        ''' </summary>
        ''' <param name="name">The name of the variable to be written</param>
        ''' <param name="value">The new value of the variable</param>
        Public Sub WriteVariable(name As String, value As Double)
            If name Is Nothing Then
                Throw New NullReferenceException()
            End If
            If StaticSettings.MaxNameCharCount < name.Length OrElse Not Me.VariableTable.ContainsKey(name) Then
                Throw New ExevalatorException(ErrorMessages.VARIABLE_NOT_FOUND.Replace("$0", name))
            End If
            Dim address As Integer = Me.VariableTable(name)
            Me.WriteVariableAt(address, value)
        End Sub

        ''' <summary>
        ''' Writes the value to the variable at the specified virtual address.
        ''' This method is more efficient than "WriteVariable" method.
        ''' </summary>
        ''' <param name="address">The virtual address of the variable to be written</param>
        ''' <param name="value">The new value of the variable</param>
        Public Sub WriteVariableAt(address As Integer, value As Double)
            If address < 0 OrElse Me.Memory.Count <= address Then
                Throw New ExevalatorException(ErrorMessages.INVALID_VARIABLE_ADDRESS.Replace("$0", address.ToString()))
            End If
            Me.Memory(address) = value
        End Sub

        ''' <summary>
        ''' Reads the value of the variable having the specified name.
        ''' </summary>
        ''' <param name="name">The name of the variable to be read</param>
        ''' <returns>The current value of the variable</returns>
        Public Function ReadVariable(name As String) As Double
            If name Is Nothing
                Throw New NullReferenceException()
            End If
            If StaticSettings.MaxNameCharCount < name.Length OrElse Not Me.VariableTable.ContainsKey(name) Then
                Throw New ExevalatorException(ErrorMessages.VARIABLE_NOT_FOUND.Replace("$0", name))
            End If
            Dim address As Integer = Me.VariableTable(name)
            Return Me.ReadVariableAt(address)
        End Function

        ''' <summary>
        ''' Reads the value of the variable at the specified virtual address.
        ''' This method is more efficient than "ReadVariable" method.
        ''' </summary>
        ''' <param name="address">The virtual address of the variable to be read</param>
        ''' <returns>The current value of the variable</returns>
        Public Function ReadVariableAt(address As Integer) As Double
            If address < 0 OrElse Me.Memory.Count <= address Then
                Throw New ExevalatorException(ErrorMessages.INVALID_VARIABLE_ADDRESS.Replace("$0", address.ToString()))
            End If
            Return Me.Memory(address)
        End Function

        ''' <summary>
        ''' Connects a function, for using it in the expression.
        ''' </summary>
        ''' <param name="name">The name of the function used in the expression</param>
        ''' <param name="function">The function to be connected</param>
        Public Sub ConnectFunction(name As String, functionImpl As IExevalatorFunction)
            If name Is Nothing then
                Throw New NullReferenceException()
            End If
            If StaticSettings.MaxNameCharCount < name.Length Then
                Throw New ExevalatorException( _
                    ErrorMessages.TOO_LONG_FUNCTION_NAME.Replace("$0", StaticSettings.MaxNameCharCount.ToString()) _
                )
            End If
            If Me.FunctionTable.ContainsKey(name) Then
                Throw New ExevalatorException(ErrorMessages.FUNCTION_ALREADY_CONNECTED.Replace("$0", name))
            End If
            Me.FunctionTable(name) = functionImpl
        End Sub

    End Class

    Public Class ExevalatorException
        Inherits Exception
        Public Sub New(errorMessage As String)
            MyBase.New(errorMessage)
        End Sub
        Public Sub New(errorMessage As String, causeException As Exception)
            MyBase.New(errorMessage, causeException)
        End Sub
    End Class

    Public Interface IExevalatorFunction
        Function Invoke(arguments As Double()) As Double
    End Interface


    ''' <summary>
    ''' The class performing functions of a lexical analyzer.
    ''' </summary>
    Public Class LexicalAnalyzer

        ''' <summary>
        ''' Splits (tokenizes) the expression into tokens, and analyze them.
        ''' </summary>
        ''' <param name="expression">The expression to be tokenized/analyzed</param>
        ''' <returns>Analyzed tokens</returns>
        Public Shared Function Analyze(expression As String) As Token()

            ' Firstly, to simplify the tokenization,
            ' replace number literals in the expression to the escaped representation: "@NUMBER_LITERAL",
            ' because number literals may contains "+" or "-" in their exponent part.
            Dim numberLiterals As List(Of string) = New List(Of string)()
            Dim escapedExpression As String = LexicalAnalyzer.EscapeNumberLiterals(expression, numberLiterals)

            ' Tokenize (split) the expression into token word(string)s.
            Dim spacedExpression As String = escapedExpression
            For Each tokenSplitter As Char in StaticSettings.TokenSplitterSymbolList
                spacedExpression = spacedExpression.Replace(tokenSplitter.ToString(), " " + tokenSplitter.ToString() + " ")
            Next

            spacedExpression = spacedExpression.Trim()
            Dim tokenWords() As String = Regex.Split(spacedExpression, "\s+")

            ' For an empty expression (containing no tokens), the above returns { "" }, not { }.
            ' So we should detect/handle it as follows.
            If tokenWords.Length = 1 AndAlso tokenWords(0).Length = 0 Then
                Throw New ExevalatorException(ErrorMessages.EMPTY_EXPRESSION)
            End If

            ' Checks the total number of tokens.
            If StaticSettings.MaxTokenCount < tokenWords.Length Then
                Throw new ExevalatorException( _
                    ErrorMessages.TOO_MANY_TOKENS.Replace("$0", StaticSettings.MaxTokenCount.ToString()) _
                )
            End If

            ' Create Token-type instances from token word(string)s.
            ' Also, escaped number literals will be recovered.
            Dim tokens As Token() = LexicalAnalyzer.CreateTokensFromTokenWords(tokenWords, numberLiterals.ToArray())

            ' Check syntactic correctness of tokens.
            LexicalAnalyzer.CheckParenthesisBalance(tokens)
            LexicalAnalyzer.CheckEmptyParentheses(tokens)
            LexicalAnalyzer.CheckLocationsOfOperatorsAndLeafs(tokens)

            Return tokens
        End Function

        ''' <summary>
        ''' Extracts all number literals in the expression, and replaces them to the escaped representation.
        ''' </summary>
        ''' <param name="expression">The expression to be processed</param>
        ''' <param name="numberLiterals">The List to which extracted number literals will be stored</param>
        ''' <returns>The expression in which all number literals are escaped</returns>
        Private Shared Function EscapeNumberLiterals(expression As String, numberLiterals As List(Of String)) As String
            Dim escapedExpressionBuilder As StringBuilder = New StringBuilder()

            Dim matchedResults As MatchCollection = Regex.Matches(expression, StaticSettings.NumberLiteralRegex)
            Dim lastLiteralEnd As Integer = -1

            For Each matchedResult As Match in matchedResults
                Dim literalWord As String = matchedResult.Value
                Dim literalBegin As Integer = matchedResult.Index
                numberLiterals.Add(literalWord)

                Dim intervalLength As Integer = literalBegin - lastLiteralEnd - 1
                escapedExpressionBuilder.Append(expression.Substring(lastLiteralEnd + 1, intervalLength))
                escapedExpressionBuilder.Append(StaticSettings.EscapedNumberLiteral)
                lastLiteralEnd = literalBegin + literalWord.Length - 1
            Next

            If lastLiteralEnd < expression.Length - 1 Then
                Dim tailLength As Integer = expression.Length - lastLiteralEnd - 1
                escapedExpressionBuilder.Append(expression.Substring(lastLiteralEnd + 1, tailLength))
            End If

            Dim escapedExpression As String = escapedExpressionBuilder.ToString()
            Return escapedExpression
        End Function

        ''' <summary>
        ''' Create Token-type instances from token word(string)s.
        ''' Also, escaped number literals will be recovered.
        ''' </summary>
        ''' <param name="tokenWords">Tokenized words</param>
        ''' <returns>Created Token-type instances</returns>
        Private Shared Function CreateTokensFromTokenWords(tokenWords() As String, numberLiterals() As String) As Token()
            Dim tokenCount As Integer = tokenWords.Length

            ' Stores the parenthesis-depth, which will increase at "(" and decrease at ")".
            Dim parenthesisDepth As Integer = 0

            ' Stores the parenthesis-depth when a function call operator begins,
            ' for detecting the end of the function operator.
            Dim callParenthesisDepths As HashSet(Of Integer) = New HashSet(Of Integer)()

            Dim iliteral As Integer = 0
            Dim lastToken As Token? = Nothing
            Dim tokens(tokenCount - 1) As Token

            For itoken As Integer = 0 To tokenCount - 1
                Dim word As String = tokenWords(itoken)

                ' Cases of open parentheses, or beginning of function calls.
                If String.Equals(word, "(") Then
                    parenthesisDepth += 1
                    If 1 <= itoken AndAlso tokens(itoken - 1).Type = TokenType.FunctionIdentifier Then
                        callParenthesisDepths.Add(parenthesisDepth)
                        Dim op As OperatorInfo = StaticSettings.CallSymbolOperatorDict(word(0))
                        tokens(itoken) = New Token(TokenType.OperatorToken, word, op)
                    Else
                        tokens(itoken) = New Token(TokenType.Parenthesis, word)
                    End If

                ' Cases of closes parentheses, or end of function calls.
                Else If String.Equals(word, ")") Then
                    If callParenthesisDepths.Contains(parenthesisDepth) Then
                        callParenthesisDepths.Remove(parenthesisDepth)
                        Dim op As OperatorInfo = StaticSettings.CallSymbolOperatorDict(word(0))
                        tokens(itoken) = New Token(TokenType.OperatorToken, word, op)
                    Else
                        tokens(itoken) = New Token(TokenType.Parenthesis, word)
                    End If
                    parenthesisDepth -= 1

                ' Cases of operators.
                Else If word.Length = 1 AndAlso StaticSettings.OperatorSymbolSet.Contains(word(0)) Then
                    tokens(itoken) = New Token(TokenType.OperatorToken, word)

                    Dim op As OperatorInfo?
                    If lastToken Is Nothing _
                            OrElse String.equals(lastToken?.Word, "(") _
                            OrElse String.equals(lastToken?.Word, ",") _
                            OrElse (lastToken?.Type = TokenType.OperatorToken _
                            AndAlso lastToken?.OperatorInfo?.Type <> OperatorType.FunctionCall) Then

                        If StaticSettings.UnaryPrefixSymbolOperatorDict.ContainsKey(word(0)) Then
                            op = StaticSettings.UnaryPrefixSymbolOperatorDict(word(0))
                        Else
                            Throw New ExevalatorException(ErrorMessages.UNKNOWN_UNARY_PREFIX_OPERATOR.Replace("$0", word))
                        End If

                    Else If String.Equals(lastToken?.Word, ")") _
                          OrElse lastToken?.Type = TokenType.NumberLiteral _
                          OrElse lastToken?.Type = TokenType.VariableIdentifier Then

                        If StaticSettings.BinarySymbolOperatorDict.ContainsKey(word(0)) Then
                            op = StaticSettings.BinarySymbolOperatorDict(word(0))
                        Else
                            Throw New ExevalatorException(ErrorMessages.UNKNOWN_BINARY_OPERATOR.Replace("$0", word))
                        End If

                    Else
                        Throw New ExevalatorException(ErrorMessages.UNKNOWN_OPERATOR_SYNTAX.Replace("$0", word))
                    End If

                    tokens(itoken) = New Token(TokenType.OperatorToken, word, op.Value)

                ' Cases of literals, and separator.
                Else If String.Equals(word, StaticSettings.EscapedNumberLiteral) Then
                    tokens(itoken) = New Token(TokenType.NumberLiteral, numberLiterals(iliteral))
                    iliteral += 1

                Else If String.Equals(word, ",") Then
                    tokens(itoken) = New Token(TokenType.ExpressionSeparator, word)

                ' Cases of variable identifier of function identifier.
                Else
                    If itoken < tokenCount - 1 AndAlso String.Equals(tokenWords(itoken + 1), "(") Then
                        tokens(itoken) = New Token(TokenType.FunctionIdentifier, word)
                    Else
                        tokens(itoken) = New Token(TokenType.VariableIdentifier, word)
                    End If
                End If

                lastToken = tokens(itoken)
            Next

            Return tokens
        End Function

        ''' <summary>
        ''' Checks the number and correspondence of open "(" / closed ")" parentheses.
        ''' An ExevalatorException will be thrown when any errors detected.
        ''' If no error detected, nothing will occur.
        ''' </summary>
        ''' <param name="tokens">Tokens of the inputted expression.</param>
        Private Shared Sub CheckParenthesisBalance(tokens() As Token)
            Dim tokenCount As Integer = tokens.Length
            Dim hierarchy As Integer = 0 ' Increases at "(" and decreases at ")".

            For itoken As Integer = 0 To tokenCount - 1
                Dim token As Token = tokens(itoken)
                If String.Equals(token.Word, "(") Then
                    hierarchy += 1
                Else If String.Equals(token.Word, ")") Then
                    hierarchy -= 1
                End If

                ' If the value of hierarchy is negative, the open parenthesis is deficient.
                If hierarchy < 0 Then
                    Throw New ExevalatorException(ErrorMessages.DEFICIENT_OPEN_PARENTHESIS)
                End If
            Next

            ' If the value of hierarchy is not zero at the end of the expression,
            ' the closed parentheses ")" is deficient.
            If hierarchy > 0 Then
                Throw New ExevalatorException(ErrorMessages.DEFICIENT_CLOSED_PARENTHESIS)
            End If
        End Sub

        ''' <summary>
        ''' Checks that empty parentheses "()" are not contained in the expression.
        ''' An ExevalatorException will be thrown when any errors detected.
        ''' If no error detected, nothing will occur.
        ''' </summary>
        ''' <param name="tokens">Tokens of the inputted expression.</param>
        Private Shared Sub CheckEmptyParentheses(tokens() As Token)
            Dim tokenCount As Integer = tokens.Length
            Dim contentCounter As Integer = 0

            For itoken As Integer = 0 To tokenCount - 1
                Dim token As Token = tokens(itoken)

                If token.Type = TokenType.Parenthesis Then ' Excepting function-call operators
                    If String.Equals(token.Word, "(") Then
                        contentCounter = 0
                    Else If String.Equals(token.Word, ")") Then
                        If contentCounter = 0 Then
                            Throw New ExevalatorException(ErrorMessages.EMPTY_PARENTHESIS)
                        End If
                    End If
                Else
                    contentCounter += 1
                End If
            Next
        End Sub

        ''' <summary>
        ''' Checks correctness of locations of operators and leaf elements (literals and identifiers).
        ''' An ExevalatorException will be thrown when any errors detected.
        ''' If no error detected, nothing will occur.
        ''' </summary>
        ''' <param name="tokens">Tokens of the inputted expression.</param>
        Private Shared Sub CheckLocationsOfOperatorsAndLeafs(tokens() As Token)
            Dim tokenCount As Integer = tokens.Length
            Dim leafTypeSet As HashSet(Of TokenType) = New HashSet(Of TokenType)()
            leafTypeSet.Add(TokenType.NumberLiteral)
            leafTypeSet.Add(TokenType.VariableIdentifier)

            ' Reads and check tokens from left to right.
            For itoken As Integer = 0 To tokenCount - 1
                Dim token As Token = tokens(itoken)

                ' Prepare information of next/previous token.
                Dim nextIsLeaf As Boolean = itoken <> tokenCount - 1 AndAlso leafTypeSet.Contains(tokens(itoken + 1).Type)
                Dim prevIsLeaf As Boolean = itoken <> 0 AndAlso leafTypeSet.Contains(tokens(itoken - 1).Type)
                Dim nextIsOpenParenthesis As Boolean = itoken < tokenCount - 1 AndAlso String.Equals(tokens(itoken + 1).Word, "(")
                Dim prevIsCloseParenthesis As Boolean = itoken <> 0 AndAlso String.Equals(tokens(itoken - 1).Word, ")")
                Dim nextIsPrefixOperator As Boolean = itoken < tokenCount - 1 _
                        AndAlso tokens(itoken + 1).Type = TokenType.OperatorToken _
                        AndAlso tokens(itoken + 1).OperatorInfo?.Type = OperatorType.UnaryPrefix
                Dim nextIsFunctionCallBegin As Boolean = nextIsOpenParenthesis _
                        AndAlso tokens(itoken + 1).Type = TokenType.OperatorToken _
                        AndAlso tokens(itoken + 1).OperatorInfo?.Type = OperatorType.FunctionCall
                Dim nextIsFunctionIdentifier As Boolean = itoken < tokenCount - 1 _
                        AndAlso tokens(itoken + 1).Type = TokenType.FunctionIdentifier

                ' Case of operators
                If token.Type = TokenType.OperatorToken Then

                    ' Cases of unary-prefix operators
                    If token.OperatorInfo?.Type = OperatorType.UnaryPrefix Then

                        'Only leafs, open parentheses, unary-prefix and function-call operators can be an operand.
                        If Not (nextIsLeaf OrElse nextIsOpenParenthesis OrElse nextIsPrefixOperator orElse nextIsFunctionIdentifier) Then
                            Throw New ExevalatorException(ErrorMessages.RIGHT_OPERAND_REQUIRED.Replace("$0", token.Word))
                        End If

                    End If ' Cases of unary-prefix operators

                    ' Cases of binary operators or a separator of partial expressions
                    If token.OperatorInfo?.Type = OperatorType.Binary OrElse String.Equals(token.Word, ",") Then

                        ' Only leafs, open parentheses, unary-prefix and function-call operators can be a right-operands.
                        If Not (nextIsLeaf OrElse nextIsOpenParenthesis OrElse nextIsPrefixOperator OrElse nextIsFunctionIdentifier) Then
                            Throw New ExevalatorException(ErrorMessages.RIGHT_OPERAND_REQUIRED.Replace("$0", token.Word))
                        End If

                        ' Only leaf elements and closed parenthesis can be a right-operand.
                        If Not (prevIsLeaf OrElse prevIsCloseParenthesis) Then
                            Throw New ExevalatorException(ErrorMessages.LEFT_OPERAND_REQUIRED.Replace("$0", token.Word))
                        End If

                    End If ' Cases of binary operators or a separator of partial expressions

                End If ' Case of operators

                ' Case of leaf elements
                If leafTypeSet.Contains(token.Type) Then

                    ' An other leaf element or an open parenthesis can not be at the right of an leaf element.
                    If Not nextIsFunctionCallBegin AndAlso (nextIsOpenParenthesis OrElse nextIsLeaf) Then
                        Throw New ExevalatorException(ErrorMessages.RIGHT_OPERATOR_REQUIRED.Replace("$0", token.Word))
                    End If

                    ' An other leaf element or a closed parenthesis can not be at the left of an leaf element.
                    If prevIsCloseParenthesis OrElse prevIsLeaf then
                        Throw New ExevalatorException(ErrorMessages.LEFT_OPERATOR_REQUIRED.Replace("$0", token.Word))
                    End If

                End If ' Case of leaf elements
            Next ' Loops for each token
        End Sub ' End of this method
    End Class


    ''' <summary>
    ''' The class performing functions of a parser.
    ''' </summary>
    Public Class Parser

        ''' <summary>
        ''' Parses tokens and construct Abstract Syntax Tree (AST).
        ''' </summary>
        ''' <param name="tokens">Tokens to be parsed</param>
        ''' <returns>The root node of the constructed AST</returns>
        Public Shared Function Parse(tokens() As Token) As AstNode

            '# In this method, we use a non-recursive algorithm for the parsing.
            '# Processing cost is maybe O(N), where N is the number of tokens. 

            ' Number of tokens
            Dim tokenCount As Integer = tokens.Length

            ' Working stack to form multiple AstNode instances into a tree-shape.
            Dim stack As Stack(Of AstNode) = New Stack(Of AstNode)()

            ' Tokens of temporary nodes used in the above working stack, for isolating ASTs of partial expressions.
            Dim parenthesisStackLidToken As Token = New Token(TokenType.StackLid, "(PARENTHESIS_STACK_LID)")
            Dim separatorStackLidToken As Token = New Token(TokenType.StackLid, "(SEPARATOR_STACK_LID)")
            Dim callBeginStackLidToken As Token = New Token(TokenType.StackLid, "(CALL_BEGIN_STACK_LID)")

            ' The array storing next operator's precedence for each token.
            ' At (i), it is stored that the precedence of the first operator of which token-index is greater than i.
            Dim nextOperatorPrecedences() As UInteger = Parser.GetNextOperatorPrecedences(tokens)

            ' Read tokens from left to right.
            Dim itoken As Integer = 0
            Do
                Dim token As Token = tokens(itoken)
                Dim operatorNode As AstNode = Nothing

                ' Case of literals and identifiers: "1.23", "x", "f", etc.
                If token.Type = TokenType.NumberLiteral _
                        OrElse token.Type = TokenType.VariableIdentifier _
                        OrElse token.Type = TokenType.FunctionIdentifier Then
                    stack.Push(New AstNode(token))
                    itoken += 1
                    Continue Do

                ' Case of parenthesis: "(" or ")"
                Else If token.Type = TokenType.Parenthesis Then
                    If token.Word = "(" Then
                        stack.Push(New AstNode(parenthesisStackLidToken))
                        itoken += 1
                        Continue Do
                    Else ' Case of ")"
                        operatorNode = Parser.PopPartialExprNodes(stack, parenthesisStackLidToken)(0)
                    End If

                ' Case of separator: ","
                Else If token.Type = TokenType.ExpressionSeparator Then
                    stack.Push(New AstNode(separatorStackLidToken))
                    itoken += 1
                    Continue Do

                ' Case of operators: "+", "-", etc.
                Else If token.Type = TokenType.OperatorToken Then
                    operatorNode = New AstNode(token)
                    Dim nextOpPrecedence As UInteger = nextOperatorPrecedences(itoken)

                    ' Case of unary-prefix operators:
                    ' * Connect the node of right-token as an operand, if necessary (depending the next operator's precedence).
                    If token.OperatorInfo.Value.Type = OperatorType.UnaryPrefix Then
                        If Parser.ShouldAddRightOperand(token.OperatorInfo.Value.Precedence, nextOpPrecedence) Then
                            operatorNode.ChildNodeList.Add(New AstNode(tokens(itoken + 1)))
                            itoken += 1
                        End If ' else: Operand will be connected later. See the bottom of this loop.

                    ' Case of binary operators:
                    ' Always connect the node of left-token as an operand.
                    ' Connect the node of right-token as an operand, if necessary (depending the next operator's precedence).
                    Else If token.OperatorInfo.Value.Type = OperatorType.Binary Then
                        operatorNode.ChildNodeList.Add(stack.Pop())
                        If Parser.ShouldAddRightOperand(token.OperatorInfo.Value.Precedence, nextOpPrecedence) Then
                            operatorNode.ChildNodeList.Add(New AstNode(tokens(itoken + 1)))
                            itoken += 1
                        End if ' else: Right-operand will be connected later. See the bottom of this loop.

                    ' Case of function-call operators.
                    Else If token.OperatorInfo.Value.Type = OperatorType.FunctionCall Then
                        If String.Equals(token.Word, "(") Then
                            operatorNode.ChildNodeList.Add(stack.Pop()) ' Add function-identifier node at the top of the stack.
                            stack.Push(operatorNode)
                            stack.Push(new AstNode(callBeginStackLidToken)) ' The marker to correct partial expressions of args from the stack.
                            itoken += 1
                            Continue Do

                        ' Case of ")"
                        Else
                            Dim argNodes() As AstNode = Parser.PopPartialExprNodes(stack, callBeginStackLidToken)
                            operatorNode = stack.Pop()

                            For Each argNode As AstNode in argNodes
                                operatorNode.ChildNodeList.Add(argNode)
                            Next
                        End If
                    End If
                End if

                ' If the precedence of the operator at the top of the stack is stronger than the next operator,
                ' connect all "unconnected yet" operands and operators in the stack.
                While Parser.ShouldAddRightOperandToStackedOperator(stack, nextOperatorPrecedences(itoken))
                    Dim oldOperatorNode As AstNode = operatorNode
                    operatorNode = stack.Pop()
                    operatorNode.ChildNodeList.Add(oldOperatorNode)
                End While

                stack.Push(operatorNode)
                itoken += 1

            Loop While itoken < tokenCount

            ' The AST has been constructed on the stack, and only its root node is stored in the stack.
            Dim rootNodeOfExpressionAst As AstNode = stack.Pop()

            ' Check that the depth of the constructed AST does not exceeds the limit.
            rootNodeOfExpressionAst.CheckDepth(1, StaticSettings.MaxAstDepth)

            Return rootNodeOfExpressionAst
        End Function

        ''' <summary>
        ''' Judges whether the right-side token should be connected directly as an operand, to the target operator.
        ''' </summary>
        ''' <param name="targetOperatorPrecedence">The precedence of the target operator (smaller value gives higher precedence)</param>
        ''' <param name="nextOperatorPrecedence">The precedence of the next operator (smaller value gives higher precedence)</param>
        ''' <returns>Returns true if the right-side token (operand) should be connected to the target operator</returns>
        Private Shared Function ShouldAddRightOperand(targetOperatorPrecedence As UInteger, nextOperatorPrecedence As UInteger) As Boolean
            Return targetOperatorPrecedence <= nextOperatorPrecedence ' left is stronger
        End Function

        ''' <summary>
        ''' Judges whether the right-side token should be connected directly as an operand,
        ''' to the operator at the top of the working stack.
        ''' </summary>
        ''' <param name="stack" The working stack used for the parsing</param>
        ''' <param name="nextOperatorPrecedence">The precedence of the next operator (smaller value gives higher precedence)</param>
        ''' <returns>Returns true if the right-side token (operand) should be connected to the operator at the top of the stack</returns>
        Private Shared Function ShouldAddRightOperandToStackedOperator(stack As Stack(Of AstNode), nextOperatorPrecedence As UInteger) As Boolean
            If stack.Count = 0 OrElse stack.Peek().Token.Type <> TokenType.OperatorToken Then
                Return False
            End If
            Return ShouldAddRightOperand(stack.Peek().Token.OperatorInfo.Value.Precedence, nextOperatorPrecedence)
        End Function

        ''' <summary>
        ''' Pops root nodes of ASTs of partial expressions constructed on the stack.
        ''' In the returned array, the popped nodes are stored in FIFO order.
        ''' </summary>
        ''' <param name="stack">The working stack used for the parsing</param>
        ''' <param name="targetStackLidToken">
        '''     The token of the temporary node pushed in the stack, at the end of partial expressions to be popped
        ''' </param>
        ''' <returns>Root nodes of ASTs of partial expressions</returns>
        Private Shared Function PopPartialExprNodes(stack As Stack(Of AstNode), endStackLidToken As Token) As AstNode()
            If stack.Count = 0 Then
                Throw new ExevalatorException(ErrorMessages.UNEXPECTED_PARTIAL_EXPRESSION)
            End If

            Dim partialExprNodeList As List(Of AstNode) = New List(Of AstNode)()
            While stack.Count <> 0

                If stack.Peek().Token.Type = TokenType.StackLid Then

                    Dim stackLidNode As AstNode = stack.Pop()
                    If Equals(stackLidNode.Token, endStackLidToken) Then
                        Exit While
                    End If
                Else
                    partialExprNodeList.Add(stack.Pop())
                End If
            End While

            Dim nodeCount As Integer = partialExprNodeList.Count
            Dim partialExprNodes(nodeCount - 1) As AstNode
            For inode As Integer = 0 To nodeCount - 1
                partialExprNodes(inode) = partialExprNodeList(nodeCount - inode - 1) ' Storing elements in reverse order.
            Next
            Return partialExprNodes
        End Function

        ''' <summary>
        ''' Returns an array storing next operator's precedence for each token.
        ''' In the returned array, it will stored at [i] that
        ''' precedence of the first operator of which token-index is greater than i.
        ''' </summary>
        ''' <param name="tokens">All tokens to be parsed</param>
        ''' <returns>The array storing next operator's precedence for each token</returns>
        Private Shared Function GetNextOperatorPrecedences(tokens() As Token) As UInteger()
            Dim tokenCount As Integer = tokens.Length
            Dim lastOperatorPrecedence As UInteger = UInteger.MaxValue ' least prior
            Dim nextOperatorPrecedences(tokenCount - 1) As UInteger

            For itoken As Integer = tokenCount - 1 To 0 Step -1
                Dim token As Token = tokens(itoken)
                nextOperatorPrecedences(itoken) = lastOperatorPrecedence

                If token.Type = TokenType.OperatorToken Then
                    lastOperatorPrecedence = token.OperatorInfo.Value.Precedence
                End If

                If token.Type = TokenType.Parenthesis Then
                    If String.Equals(token.Word, "(") Then
                        lastOperatorPrecedence = 0 ' most prior
                    ' case of ")"
                    Else
                        lastOperatorPrecedence = UInteger.MaxValue ' least prior
                    End If
                End If
            Next

            Return nextOperatorPrecedences
        End Function
    End Class


    ''' <summary>
    ''' The enum representing types of operators.
    ''' </summary>
    Public Enum OperatorType

        ''' <summary>Represents unary operator, for example: - of -1.23</summary>
        UnaryPrefix

        ''' <summary>Represents binary operator, for example: + of 1+2</summary>
        Binary

        ''' <summary>Represents function-call operator</summary>
        FunctionCall

    End Enum

    ''' <summary>
    ''' The struct storing information of an operator.
    ''' </summary>
    Public Structure OperatorInfo

        ''' <summary>The type of operator tokens.</summary>
        Public Readonly Type As OperatorType

        ''' <summary>The symbol of this operator (for example: '+').</summary>
        Public Readonly Symbol As Char

        ''' <summary>The precedence of this operator (smaller value gives higher precedence).</summary>
        Public Readonly Precedence As UInteger

        ''' <summary>
        ''' Create an Operator instance storing specified information.
        ''' </summary>
        ''' <param name="type">The type of this operator</param>
        ''' <param name="symbol">The symbol of this operator</param>
        ''' <param name="precedence">The precedence of this operator</param>
        Public Sub New(type As OperatorType, symbol As Char, precedence As UInteger)
            Me.Type = type
            Me.Symbol = symbol
            Me.Precedence = precedence
        End Sub

        ''' <summary>
        ''' Returns whether this instance equals to the specified "compared" instance.
        ''' </summary>
        ''' <param name="compared">The instance to be compared with this instance</param>
        ''' <returns>true if this instance equals to the specified "compared" instance</returns>
        Public Overrides Function Equals(compared As Object) As Boolean
            If TypeOf compared IsNot OperatorInfo Then
                Return False
            End If
            Dim comparedOperator As OperatorInfo = CType(compared, OperatorInfo)
            Dim result As Boolean = (Me.Type = comparedOperator.Type) _
                AndAlso (Me.Symbol = comparedOperator.Symbol) _
                AndAlso (Me.Precedence = comparedOperator.Precedence)
            Return result
        End Function

        'equal operator
        Public Shared Operator =(leftOperator As OperatorInfo, rightOperator As OperatorInfo) As Boolean
            Return leftOperator.Equals(rightOperator)
        End Operator

        'non-equal operator
        Public Shared Operator <>(leftOperator As OperatorInfo, rightOperator As OperatorInfo) As Boolean
            Return Not leftOperator.Equals(rightOperator)
        End Operator

        ''' <summary>
        ''' Generates the hash-code of this instance.
        ''' </summary>
        ''' <returns>The hash-code of this instance</returns>
        Public Overrides Function GetHashCode() As Integer
            Return Me.Type.GetHashCode() Xor Me.Symbol.GetHashCode() Xor Me.Precedence.GetHashCode()
        End Function

        ''' <summary>
        ''' Returns the String representation of this Operator instance.
        ''' </summary>
        ''' <returns>The String representation of this Operator instance</returns>
        Public Overrides Function ToString() As String
            Return "Operator [Symbol=" + Me.Symbol.ToString() + _
                   ", Precedence=" + Me.Precedence.ToString() + _
                   ", Type=" + Me.Type.ToString() + _
                   "]"
        End Function

    End Structure


    ''' <summary>
    ''' The enum representing types of tokens.
    ''' </summary>
    Public Enum TokenType

        ''' <summary>Represents number literal tokens, for example: 1.23</summary>
        NumberLiteral

        ''' <summary>Represents operator tokens, for example: +</summary>
        OperatorToken ' "Operator" is a reserved word.

        ''' <summary>Represents separator tokens of partial expressions: ,</summary>
        ExpressionSeparator

        ''' <summary>Represents parenthesis, for example: ( and ) of (1*(2+3))</summary>
        Parenthesis

        ''' <summary>Represents variable-identifier tokens, for example: x</summary>
        VariableIdentifier

        ''' <summary>Represents function-identifier tokens, for example: f</summary>
        FunctionIdentifier

        ''' <summary>Represents temporary token for isolating partial expressions in the stack, in parser</summary>
        StackLid

    End Enum

    ''' <summary>
    ''' The struct storing information of a token.
    ''' </summary>
    Public Structure Token

        ''' <summary>The type of this token.</summary>
        Public Readonly Type As TokenType

        ''' <summary>The text representation of this token.</summary>
        Public Readonly Word As String

        ''' <summary>The detailed information of the operator, if the type of this token is OPERATOR.</summary>
        Public Readonly OperatorInfo As OperatorInfo?

        ''' <summary>
        ''' Create an Token instance storing specified information.
        ''' </summary>
        ''' <param name="type">The type of this token</param>
        ''' <param name="word">The text representation of this token</param>
        Public Sub New(type As TokenType, word As String)
            Me.Type = type
            Me.Word = word
            Me.OperatorInfo = Nothing
        End Sub

        ''' <summary>
        ''' Create an Token instance storing specified information.
        ''' </summary>
        ''' <param name="type">The type of this token</param>
        ''' <param name="word">The text representation of this token</param>
        ''' <param name="op">The detailed information of the operator, for OPERATOR type tokens</param>
        Public Sub New(type As TokenType, word As String, op As OperatorInfo)
            Me.Type = type
            Me.Word = word
            Me.OperatorInfo = op
        End SUb

        ''' <summary>
        ''' Returns whether this instance equals to the specified "compared" instance.
        ''' </summary>
        ''' <param name="compared">The instance to be compared with this instance</param>
        ''' <returns>true if this instance equals to the specified "compared" instance</returns>
        Public Overrides Function Equals(compared As Object) As Boolean
            If TypeOf compared IsNot Token Then
                Return False
            End If

            Dim comparedToken As Token = CType(compared, Token)
            If Me.OperatorInfo.HasValue AndAlso comparedToken.OperatorInfo.HasValue Then
                Return Me.Type = comparedToken.Type _
                    AndAlso String.Equals(Me.Word, comparedToken.Word) _
                    AndAlso Equals(Me.OperatorInfo.Value, comparedToken.OperatorInfo.Value)

            Else If Not Me.OperatorInfo.HasValue AndAlso Not comparedToken.OperatorInfo.HasValue Then
                Return Me.Type = comparedToken.Type _
                    AndAlso Me.Word = comparedToken.Word

            Else
                Return False
            End IF
        End Function

        Public Shared Operator =(leftToken As Token, rightToken As Token) As Boolean
            Return leftToken.Equals(rightToken)
        End Operator

        Public Shared operator <>(leftToken As Token, rightToken As Token) As Boolean
            Return Not leftToken.Equals(rightToken)
        End Operator

        ''' <summary>
        ''' Generates the hash-code of this instance.
        ''' </summary>
        ''' <returns>The hash-code of this instance</returns>
        Public Overrides Function GetHashCode() As Integer
            Dim hashCode As Integer = Me.Type.GetHashCode() Xor Me.Word.GetHashCode()
            If Me.OperatorInfo.HasValue Then
                hashCode = hashCode Xor Me.OperatorInfo.GetHashCode()
            End If
            Return hashCode
        End Function

        ''' <summary>
        ''' Returns the String representation of this Operator instance.
        ''' </summary>
        ''' <returns>The String representation of this Operator instance</returns>
        Public Overrides Function ToString() As String
            If Me.OperatorInfo Is Nothing Then
                Return "Token [Type=" + Me.Type.ToString() + _
                       ", Word=" + Me.Word.ToString() + _
                       "]"
            Else
                Return "Token [Type=" + Me.Type.ToString() + _
                       ", Word=" + Me.Word.ToString() + _
                       ", Operator.Type=" + Me.OperatorInfo?.Type.ToString() + _
                       ", Operator.Precedence=" + Me.OperatorInfo?.Precedence.ToString() + _
                       "]"
            End If
        End Function
    End Structure


    ''' <summary>
    ''' The class storing information of an node of an AST.
    ''' </summary>
    Public Class AstNode

        ''' <summary>The token corresponding with this AST node.</summary>
        Public Readonly token As Token

        ''' <summary>The list of child nodes of this AST node.</summary>
        Public ChildNodeList As List(Of AstNode)

        ''' <summary>
        ''' Create an AST node instance storing specified information.
        ''' </summary> 
        ''' <param name="token">The token corresponding with this AST node</param>
        Sub New(token As Token)
            Me.Token = token
            Me.ChildNodeList = New List(Of AstNode)()
        End Sub

        ''' <summary>
        ''' Checks that depths in the AST of all nodes under this node (child nodes, grandchild nodes, and so on)
        ''' does not exceeds the specified maximum value.
        ''' An ExevalatorException will be thrown when the depth exceeds the maximum value.
        ''' If the depth does not exceeds the maximum value, nothing will occur.
        ''' </summary>
        ''' <param name="depthOfThisNode">The depth of this node in the AST</param>
        ''' <param name="maxAstDepth">The maximum value of the depth of the AST</param>
        Public Sub CheckDepth(depthOfThisNode As Integer, maxAstDepth As Integer)
            If maxAstDepth < depthOfThisNode Then
                Throw New ExevalatorException( _
                    ErrorMessages.EXCEEDS_MAX_AST_DEPTH.replace("$0", StaticSettings.MaxAstDepth.ToString()) _
                )
            End If

            For Each childNode as AstNode in Me.ChildNodeList
                childNode.CheckDepth(depthOfThisNode + 1, maxAstDepth)
            Next
        End Sub

        ''' <summary>
        ''' Expresses the AST under this node in XML-like text format.
        ''' </summary>
        ''' <param name="indentStage">The stage of indent of this node</param>
        ''' <returns>XML-like text representation of the AST under this node</returns>
        Public Function ToMarkuppedText(Optional indentStage As Integer = 0) As String
            Dim indentBuilder As StringBuilder = New StringBuilder()

            For istage As Integer = 0 To indentStage - 1
                indentBuilder.Append(StaticSettings.AstIndent)
            Next

            Dim indent As String = indentBuilder.ToString()
            Dim eol As String = Environment.NewLine
            Dim resultBuilder As StringBuilder = New StringBuilder()

            resultBuilder.Append(indent)
            resultBuilder.Append("<")
            resultBuilder.Append(Me.Token.Type.ToString())
            resultBuilder.Append(" word=""")
            resultBuilder.Append(Me.Token.Word.ToString())
            resultBuilder.Append("""")

            If Me.Token.Type = TokenType.OperatorToken Then
                resultBuilder.Append(" optype=""")
                resultBuilder.Append(Me.Token.OperatorInfo?.Type.ToString())
                resultBuilder.Append(""" precedence=""")
                resultBuilder.Append(Me.Token.OperatorInfo?.Precedence.ToString())
                resultBuilder.Append("""")
            End If

            if 0 < Me.ChildNodeList.Count Then
                resultBuilder.Append(">")
                For Each childNode As AstNode in ChildNodeList
                    resultBuilder.Append(eol)
                    resultBuilder.Append(childNode.ToMarkuppedText(indentStage + 1))
                Next
                resultBuilder.Append(eol)
                resultBuilder.Append(indent)
                resultBuilder.Append("</")
                resultBuilder.Append(Me.Token.Type.ToString())
                resultBuilder.Append(">")

            Else
                resultBuilder.Append(" />")
            End If

            Return resultBuilder.ToString()
        End Function
    End Class


    ''' <summary>
    ''' The class for evaluating the value of an AST.
    ''' </summary>
    Public Class Evaluator

        ''' <summary>The tree of evaluator nodes, which evaluates an expression.</summary>
        Private evaluatorNodeTree As EvaluatorNode = Nothing

        ''' <summary>
        ''' Updates the state to evaluate the value of the AST.
        ''' </summary>
        ''' <param name="ast">The root node of the AST.</param>
        ''' <param name="variableTable"> The Map mapping each variable name to an address of the variable.</param>
        ''' <param name="functionTable"> The Map mapping each function name to an IExevalatorFunction instance.</param>
        Public Sub Update( _
                ast As AstNode, _
                variableTable As Dictionary(Of String, Integer), _
                functionTable As Dictionary(Of String, IExevalatorFunction))

            Me.evaluatorNodeTree = Evaluator.CreateEvaluatorNodeTree(ast, variableTable, functionTable)
        End Sub

        ''' <summary>
        ''' Returns whether "evaluate" method is available on the current state.
        ''' </summary>
        ''' <returns>Return value - True if "evaluate" method is available.</returns>
        Public Function IsEvaluatable() As Boolean
            Return Not (Me.evaluatorNodeTree Is Nothing)
        End Function

        ''' <summary>
        ''' Evaluates the value of the AST set by "update" method.
        ''' </summary>
        ''' <param name="memory">The Vec used as as a virtual memory storing values of variables.</param>
        ''' <returns>The evaluated value.</returns>
        Public Function Evaluate(memory As List(Of Double)) As Double
            If Me.evaluatorNodeTree Is Nothing Then
                Throw New ExevalatorException("The Evaluator is not initialized but ""evaluate"" method is called.")
            Else
                Return Me.evaluatorNodeTree.Evaluate(memory)
            End If
        End Function

        ''' <summary>
        ''' Creates a tree of evaluator nodes corresponding with the specified AST.
        ''' </summary>
        ''' <param name="ast">The root node of the AST.</param>
        ''' <param name="variableTable">The Dictionary mapping each variable name to an address of the variable.</param>
        ''' <param name="functionTable">The Dictionary mapping each function name to an IExevalatorFunction instance.</param>
        ''' <returns>The root node of the created tree of evaluator nodes.</returns>
        Public Shared Function CreateEvaluatorNodeTree( _
                ast As AstNode, _
                variableTable As Dictionary(Of String, Integer), _
                functionTable As Dictionary(Of String, IExevalatorFunction)) As Evaluator.EvaluatorNode

            ' Note: This method creates a tree of evaluator nodes by traversing each node in the AST recursively.

            Dim childNodeList As List(Of AstNode) = ast.ChildNodeList
            Dim childCount As Integer = childNodeList.Count

            ' Creates evaluation nodes of child nodes, and store then into an array.
            Dim childNodeNodes(childCount - 1) As Evaluator.EvaluatorNode
            For ichild As Integer = 0 To childCount - 1
                childNodeNodes(ichild) = CreateEvaluatorNodeTree(childNodeList(ichild), variableTable, functionTable)
            Next

            ' Initialize evaluation nodes of this node.
            Dim token As Token = ast.Token
            If token.Type = TokenType.NumberLiteral Then
                Dim literalValue As Double = Double.NaN
                If Not Double.TryParse(token.Word, NumberStyles.Float, CultureInfo.InvariantCulture, literalValue) Then
                    Throw new ExevalatorException(ErrorMessages.INVALID_NUMBER_LITERAL.Replace("$0", token.Word))
                End If
                Return New Evaluator.NumberLiteralEvaluatorNode(literalValue)

            Else If token.Type = TokenType.VariableIdentifier Then
                If Not variableTable.ContainsKey(token.Word) Then
                    Throw new ExevalatorException(ErrorMessages.VARIABLE_NOT_FOUND.Replace("$0", token.Word))
                End If
                Dim address As Integer = variableTable(token.Word)
                Return New Evaluator.VariableEvaluatorNode(address)

            Else If token.Type = TokenType.FunctionIdentifier Then
                Return New Evaluator.NopEvaluatorNode()

            Else If token.Type = TokenType.OperatorToken Then
                Dim op As OperatorInfo = token.OperatorInfo.Value

                If op.Type = OperatorType.UnaryPrefix AndAlso Char.Equals(op.Symbol, "-"c) Then
                    Return New Evaluator.MinusEvaluatorNode(childNodeNodes(0))

                Else If op.Type = OperatorType.Binary AndAlso Char.Equals(op.Symbol, "+"c) Then
                    Return New Evaluator.AdditionEvaluatorNode(childNodeNodes(0), childNodeNodes(1))

                Else If op.Type = OperatorType.Binary AndAlso Char.Equals(op.Symbol, "-"c) Then
                    Return New Evaluator.SubtractionEvaluatorNode(childNodeNodes(0), childNodeNodes(1))

                Else If op.Type = OperatorType.Binary AndAlso Char.Equals(op.Symbol, "*"c) Then
                    Return New Evaluator.MultiplicationEvaluatorNode(childNodeNodes(0), childNodeNodes(1))

                Else If op.Type = OperatorType.Binary AndAlso Char.Equals(op.Symbol, "/"c) Then
                    Return New Evaluator.DivisionEvaluatorNode(childNodeNodes(0), childNodeNodes(1))

                Else If op.Type = OperatorType.FunctionCall AndAlso Char.Equals(op.Symbol, "("c) Then
                    Dim identifier As String = childNodeList(0).Token.Word
                    If Not functionTable.ContainsKey(identifier) Then
                        Throw new ExevalatorException(ErrorMessages.FUNCTION_NOT_FOUND.Replace("$0", identifier))
                    End If

                    Dim functionImpl As IExevalatorFunction = functionTable(identifier)
                    Dim argNodes(childCount - 2) As Evaluator.EvaluatorNode ' originally [childCount-1], but in VB, we must specify max-index, not length, so specifying -2 here
                    For iarg As Integer = 0 To childCount - 2
                        argNodes(iarg) = childNodeNodes(iarg + 1)
                    Next

                    Return New Evaluator.FunctionEvaluatorNode(functionImpl, argNodes)

                Else
                    Throw New ExevalatorException(ErrorMessages.UNEXPECTED_OPERATOR.replace("$0", op.Symbol.ToString()))
                End If

            Else
                Throw new ExevalatorException(ErrorMessages.UNEXPECTED_TOKEN.replace("$0", token.Type.ToString()))
            End If
        End Function

        ''' <summary>
        ''' The super class of evaluator nodes.
        ''' </summary>
        Public MustInherit Class EvaluatorNode

            ''' <summary>
            ''' Evaluate the value of this node.
            ''' </summary>
            ''' <param name="memory">The List storing values of variables.</param>
            ''' <returns>The result value of the evaluation</returns>
            Public MustOverride Function Evaluate(memory As List(Of double)) As Double
        End Class

        ''' <summary>
        ''' The super class of evaluator nodes of binary operations.
        ''' </summary>
        Public MustInherit Class BinaryOperationEvaluatorNode
            Inherits EvaluatorNode

            ''' <summary>The nofr for evaluating the right-side operand.</summary>
            Protected Readonly LeftOperandNode As EvaluatorNode

            ''' <summary>The node for evaluating the left-side operand.</summary>
            Protected Readonly RightOperandNode As EvaluatorNode

            ''' <summary>
            ''' Initializes operands.
            ''' </summary>
            ''' <param name="leftOperandNode">The node for evaluating the left-side operand</param>
            ''' <param name="rightOperandNode">The node for evaluating the right-side operand</param>
            Protected Sub New (leftOperandNode As EvaluatorNode, rightOperandNode As EvaluatorNode)
                Me.LeftOperandNode = leftOperandNode
                Me.RightOperandNode = rightOperandNode
            End Sub
        End Class

        ''' <summary>
        ''' The evaluator node for evaluating the value of a addition operator.
        ''' </summary>
        Public Class AdditionEvaluatorNode
            Inherits BinaryOperationEvaluatorNode

            ''' <summary>
            ''' Initializes operands.
            ''' </summary>
            ''' <param name="leftOperandNode">The node for evaluating the left-side operand</param>
            ''' <param name="rightOperandNode">The node for evaluating the right-side operand</param>
            Public Sub New(leftOperandNode As EvaluatorNode, rightOperandNode As EvaluatorNode)
                MyBase.New(leftOperandNode, rightOperandNode)
            End Sub

            ''' <summary>
            ''' Performs the addition.
            ''' </summary>
            ''' <param name="memory">The List storing values of variables.</param>
            ''' <returns>The result value of the addition</returns>
            Public Overrides Function Evaluate(memory As List(Of Double)) As Double
                return Me.LeftOperandNode.Evaluate(memory) + Me.RightOperandNode.Evaluate(memory)
            End Function
        End Class

        ''' <summary>
        ''' The evaluator node for evaluating the value of a subtraction operator.
        ''' </summary>
        Public Class SubtractionEvaluatorNode
            Inherits BinaryOperationEvaluatorNode

            ''' <summary>
            ''' Initializes operands.
            ''' </summary>
            ''' <param name="leftOperandNode">The node for evaluating the left-side operand</param>
            ''' <param name="rightOperandNode">The node for evaluating the right-side operand</param>
            Public Sub New(leftOperandNode As EvaluatorNode, rightOperandNode As EvaluatorNode)
                MyBase.New(leftOperandNode, rightOperandNode)
            End Sub

            ''' <summary>
            ''' Performs the subtraction.
            ''' </summary>
            ''' <param name="memory">The List storing values of variables.</param>
            ''' <returns>The result value of the subtraction</returns>
            Public Overrides Function Evaluate(memory As List(Of Double)) As Double
                Return Me.LeftOperandNode.Evaluate(memory) - Me.RightOperandNode.Evaluate(memory)
            End Function
        End Class

        ''' <summary>
        ''' The evaluator node for evaluating the value of a multiplication operator.
        ''' </summary>
        Public Class MultiplicationEvaluatorNode
            Inherits BinaryOperationEvaluatorNode

            ''' <summary>
            ''' Initializes operands.
            ''' </summary>
            ''' <param name="leftOperandNode">The node for evaluating the left-side operand</param>
            ''' <param name="rightOperandNode">The node for evaluating the right-side operand</param>
            Public Sub New(leftOperandNode As EvaluatorNode, rightOperandNode As EvaluatorNode)
                MyBase.New(leftOperandNode, rightOperandNode)
            End Sub

            ''' <summary>
            ''' Performs the multiplication.
            ''' </summary>
            ''' <param name="memory">The List storing values of variables.</param>
            ''' <returns>The result value of the multiplication</returns>
            Public Overrides Function Evaluate(memory As List(Of Double)) As Double
                Return Me.LeftOperandNode.Evaluate(memory) * Me.RightOperandNode.Evaluate(memory)
            End Function
        End Class

        ''' <summary>
        ''' The evaluator node for evaluating the value of a division operator.
        ''' </summary>
        Public Class DivisionEvaluatorNode
            Inherits BinaryOperationEvaluatorNode

            ''' <summary>
            ''' Initializes operands.
            ''' </summary>
            ''' <param name="leftOperandNode">The node for evaluating the left-side operand</param>
            ''' <param name="rightOperandNode">The node for evaluating the right-side operand</param>
            Public Sub New(leftOperandNode As EvaluatorNode, rightOperandNode As EvaluatorNode)
                MyBase.New(leftOperandNode, rightOperandNode)
            End Sub

            ''' <summary>
            ''' Performs the division.
            ''' </summary>
            ''' <param name="memory">The List storing values of variables.</param>
            ''' <returns>The result value of the division</returns>
            Public Overrides Function Evaluate(memory As List(Of Double)) As Double
                Return Me.LeftOperandNode.Evaluate(memory) / Me.RightOperandNode.Evaluate(memory)
            End Function
        End Class

        ''' <summary>
        ''' The evaluator node for evaluating the value of a unary-minus operator.
        ''' </summary>
        Public Class MinusEvaluatorNode
            Inherits EvaluatorNode

            ''' <summary>The node for evaluating the operand.</summary>
            Private Readonly OperandNode As EvaluatorNode

            ''' <summary>
            ''' Initializes the operand.
            ''' </summary>
            ''' <param name="operandNode">The node for evaluating the operand</param>
            Public Sub New(operandNode As EvaluatorNode)
                Me.OperandNode = operandNode
            End Sub

            ''' <summary>
            ''' Performs the unary-minus operation.
            ''' </summary>
            ''' <param name="memory">The List storing values of variables.</param>
            ''' <returns>The result value of the unary-minus operation</returns>
            Public Overrides Function Evaluate(memory As List(Of Double)) As Double
                Return -Me.OperandNode.Evaluate(memory)
            End Function
        End Class

        ''' <summary>
        ''' The evaluator node for evaluating the value of a number literal.
        ''' </summary>
        Public Class NumberLiteralEvaluatorNode
            Inherits EvaluatorNode

            ''' <summary>The value of the number literal.</summary>
            Private Readonly LiteralValue As Double

            ''' <summary>
            ''' Initializes the value of the number literal.
            ''' </summary>
            ''' <param name="literalValue">The value of the number literal</param>
            Public Sub New(literalValue As Double)
                Me.LiteralValue = literalValue
            End Sub

            ''' <summary>
            ''' Returns the value of the number literal.
            ''' </summary>
            ''' <param name="memory">The List storing values of variables.</param>
            ''' <returns>The value of the number literal</returns>
            Public Overrides Function Evaluate(memory As List(Of Double)) As Double
                Return Me.LiteralValue
            End Function
        End Class

        ''' <summary>
        ''' The evaluator node for evaluating the value of a variable.
        ''' </summary>
        Public Class VariableEvaluatorNode
            Inherits EvaluatorNode

            ''' <summary>The address of the variable.</summary>
            Private Readonly Address As Integer

            ''' <summary>
            ''' Initializes the address of the variable.
            ''' </summary>
            ''' <param name="address">The address of the variable</param>
            Public Sub New(address As Integer)
                Me.Address = address
            End Sub

            ''' <summary>
            ''' Returns the value of the variable.
            ''' </summary>
            ''' <param name="memory">The List storing values of variables.</param>
            ''' <returns>The value of the variable</returns>
            Public Overrides Function Evaluate(memory As List(Of Double)) As Double
                If Me.Address < 0 OrElse memory.Count <= Me.Address Then
                    Throw New ExevalatorException(ErrorMessages.INVALID_VARIABLE_ADDRESS.Replace("$0", Me.Address.ToString()))
                End If
                Return memory(Me.Address)
            End Function
        End Class

        ''' <summary>
        ''' The evaluator node for evaluating the value of a function-call operator.
        ''' </summary>
        Public Class FunctionEvaluatorNode
            Inherits EvaluatorNode

            ''' <summary>The address of the variable.</summary>
            Private Readonly FunctionImpl As IExevalatorFunction

            ''' <summary>Evaluator nodes for evaluating values of arguments.</summary>
            Private Readonly ArgumentEvalNodes() As EvaluatorNode

            ''' <summary>An array storing evaluated values of arguments.</summary>
            Private ArgumentArrayBuffer() As Double

            ''' <summary>
            ''' Initializes the function to be called.
            ''' </summary>
            ''' <param name="function">The function to be called</param>
            ''' <param name="argumentEvalNodes">Evaluator nodes for evaluating values of arguments</param>
            Public Sub New(functionImpl As IExevalatorFunction, argumentEvalNodes() As EvaluatorNode)
                Me.FunctionImpl = functionImpl
                Me.ArgumentEvalNodes = argumentEvalNodes
                ReDim Me.ArgumentArrayBuffer(Me.ArgumentEvalNodes.Length - 1)
            End Sub

            ''' <summary>
            ''' Calls the function and returns the returned value of the function.
            ''' </summary>
            ''' <param name="memory">The List storing values of variables.</param>
            ''' <returns>The returned value of the function</returns>
            Public Overrides Function Evaluate(memory As List(Of Double)) As Double
                For iarg As Integer = 0 To Me.ArgumentEvalNodes.Length - 1
                    Me.ArgumentArrayBuffer(iarg) = Me.ArgumentEvalNodes(iarg).Evaluate(memory)
                Next
                
                Try
                    Return Me.FunctionImpl.Invoke(Me.ArgumentArrayBuffer)
                Catch e as Exception
                    Throw New ExevalatorException(ErrorMessages.FUNCTION_ERROR, e)
                End Try

            End Function
        End Class

        ''' <summary>
        ''' The evaluator node for evaluating nothing.
        ''' </summary>
        Public Class NopEvaluatorNode
            Inherits EvaluatorNode

            ''' <summary>
            ''' Creates an instance.
            ''' </summary>
            Public Sub New()
            End Sub

            ''' <summary>
            ''' Performs nothing.
            ''' </summary>
            ''' <returns>The value of NaN.</returns>
            Public Overrides Function Evaluate(memory As List(Of Double)) As Double
                Return Double.NaN
            End Function
        End Class
    End Class


    ''' <summary>
    ''' The struct defining static setting values.
    ''' </summary>
    Public Structure StaticSettings

        ''' <summary>The maximum number of characters in an expression.</summary>
        Public Const MaxExpressionCharCount As Integer = 256

        ''' <summary>The maximum number of characters of variable/function names.</summary>
        public Const MaxNameCharCount As Integer = 64

        ''' <summary>The maximum number of tokens in an expression.</summary>
        public Const MaxTokenCount As Integer = 64

        ''' <summary>The maximum depth of an Abstract Syntax Tree (AST).</summary>
        public Const MaxAstDepth As Integer = 32

        ''' <summary>The indent used in text representations of ASTs.</summary>
        public Const AstIndent As String = "  "

        ''' <summary>The regular expression of number literals.</summary>
        Public Const NumberLiteralRegex As String =
            "(?<=(\s|\+|-|\*|/|\(|\)|,|^))" + ' Token splitters or start of expression
            "([0-9]+(\.[0-9]+)?)" +           ' Significand part
            "((e|E)(\+|-)?[0-9]+)?"           ' Exponent part

        ''' <summary>The escaped representation of number literals in expressions.</summary>
        Public Const EscapedNumberLiteral As String = "@NUMBER_LITERAL@"

        ''' <summary>The HashSet of symbols of available operators.</summary>
        Public Shared Readonly OperatorSymbolSet As HashSet(Of Char)

        ''' <summary>The Dictionary mapping each symbol of a binary operator to a Operator struct.</summary>
        public Shared Readonly BinarySymbolOperatorDict As Dictionary(Of Char, OperatorInfo)

        ''' <summary>The Dictionary mapping each symbol of a unary-prefix operator to a Operator struct.</summary>
        Public Shared Readonly UnaryPrefixSymbolOperatorDict As Dictionary(Of Char, OperatorInfo)

        ''' <summary>The Dictionary mapping each symbol of a function-call operator to a Operator struct.</summary>
        Public Shared Readonly CallSymbolOperatorDict As Dictionary(Of Char, OperatorInfo)

        ''' <summary>The List of symbols to split an expression into tokens.</summary>
        Public Shared Readonly TokenSplitterSymbolList As List(Of Char)

        ''' <summary>Initializes values of static-readonly members.</summary>
        Shared Sub New()
            Dim additionOperator As OperatorInfo = New OperatorInfo(OperatorType.Binary, "+"c, 400)
            Dim subtractionOperator As OperatorInfo = new OperatorInfo(OperatorType.Binary, "-"c, 400)
            Dim multiplicationOperator As OperatorInfo = new OperatorInfo(OperatorType.Binary, "*"c, 300)
            Dim divisionOperator As OperatorInfo = new OperatorInfo(OperatorType.Binary, "/"c, 300)
            Dim minusOperator As OperatorInfo = new OperatorInfo(OperatorType.UnaryPrefix, "-"c, 200)
            Dim callBeginOperator As OperatorInfo = new OperatorInfo(OperatorType.FunctionCall, "("c, 100)
            Dim callEndOperator As OperatorInfo = new OperatorInfo(OperatorType.FunctionCall, ")"c, UInteger.MaxValue) ' least prior

            OperatorSymbolSet = New HashSet(Of Char)()
            OperatorSymbolSet.Add("+"c)
            OperatorSymbolSet.Add("-"c)
            OperatorSymbolSet.Add("*"c)
            OperatorSymbolSet.Add("/"c)
            OperatorSymbolSet.Add("("c)
            OperatorSymbolSet.Add(")"c)

            BinarySymbolOperatorDict = New Dictionary(Of Char, OperatorInfo)()
            BinarySymbolOperatorDict.Add("+"c, additionOperator)
            BinarySymbolOperatorDict.Add("-"c, subtractionOperator)
            BinarySymbolOperatorDict.Add("*"c, multiplicationOperator)
            BinarySymbolOperatorDict.Add("/"c, divisionOperator)

            UnaryPrefixSymbolOperatorDict = New Dictionary(Of Char, OperatorInfo)()
            UnaryPrefixSymbolOperatorDict.Add("-"c, minusOperator)

            CallSymbolOperatorDict = New Dictionary(Of Char, OperatorInfo)()
            CallSymbolOperatorDict.Add("("c, callBeginOperator)
            CallSymbolOperatorDict.Add(")"c, callEndOperator)

            TokenSplitterSymbolList = New List(Of Char)()
            TokenSplitterSymbolList.Add("+"c)
            TokenSplitterSymbolList.Add("-"c)
            TokenSplitterSymbolList.Add("*"c)
            TokenSplitterSymbolList.Add("/"c)
            TokenSplitterSymbolList.Add("("c)
            TokenSplitterSymbolList.Add(")"c)
            TokenSplitterSymbolList.Add(","c)
        End Sub

    End Structure

End Namespace
