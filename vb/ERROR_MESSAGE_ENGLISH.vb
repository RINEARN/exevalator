    ''' !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    ''' To set the error-message language to English (the default),
    ''' copy the contents below and
    ''' overwrite the ErrorMessage structure in exevalator.vb with them.
    ''' !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

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
