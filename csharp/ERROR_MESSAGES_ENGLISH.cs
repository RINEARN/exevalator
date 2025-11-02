    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    // To set the error-message language to English (the default),
    // copy the contents below and
    // overwrite the ErrorMessages struct in Exevalator.vb with them.
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

    /// <summary>
    /// Error messages of ExevalatorException,
    /// which is thrown by Exevalator if the input expression is syntactically incorrect, or uses undeclared variables/functions.
    /// You can customize the error message of Exevalator by modifying the values of the properties of this class.
    /// </summary>
    public struct ErrorMessages
    {
        public static readonly string EMPTY_EXPRESSION = "The inputted expression is empty.";
        public static readonly string TOO_MANY_TOKENS = "The number of tokens exceeds the limit (StaticSettings.MaxTokenCount: '$0')";
        public static readonly string DEFICIENT_OPEN_PARENTHESIS = "The number of open parentheses '(' is deficient.";
        public static readonly string DEFICIENT_CLOSED_PARENTHESIS = "The number of closed parentheses ')' is deficient.";
        public static readonly string EMPTY_PARENTHESIS = "The content of parentheses '()' should not be empty.";
        public static readonly string RIGHT_OPERAND_REQUIRED = "An operand is required at the right of: '$0'";
        public static readonly string LEFT_OPERAND_REQUIRED = "An operand is required at the left of: '$0'";
        public static readonly string RIGHT_OPERATOR_REQUIRED = "An operator is required at the right of: '$0'";
        public static readonly string LEFT_OPERATOR_REQUIRED = "An operator is required at the left of: '$0'";
        public static readonly string UNKNOWN_UNARY_PREFIX_OPERATOR = "Unknown unary-prefix operator: '$0'";
        public static readonly string UNKNOWN_BINARY_OPERATOR = "Unknown binary operator: '$0'";
        public static readonly string UNKNOWN_OPERATOR_SYNTAX = "Unknown operator syntax: '$0'";
        public static readonly string EXCEEDS_MAX_AST_DEPTH = "The depth of the AST exceeds the limit (StaticSettings.MaxAstDepth: '$0')";
        public static readonly string UNEXPECTED_PARTIAL_EXPRESSION = "Unexpected end of a partial expression";
        public static readonly string INVALID_NUMBER_LITERAL = "Invalid number literal: '$0'";
        public static readonly string INVALID_MEMORY_ADDRESS = "Invalid memory address: '$0'";
        public static readonly string FUNCTION_ERROR = "Function Error ('$0'): $1";
        public static readonly string VARIABLE_NOT_FOUND = "Variable not found: '$0'";
        public static readonly string FUNCTION_NOT_FOUND = "Function not found: '$0'";
        public static readonly string UNEXPECTED_OPERATOR = "Unexpected operator: '$0'";
        public static readonly string UNEXPECTED_TOKEN = "Unexpected token: '$0'";
        public static readonly string TOO_LONG_EXPRESSION = "The length of the expression exceeds the limit (StaticSettings.MaxExpressionCharCount: '$0')";
        public static readonly string UNEXPECTED_ERROR = "Unexpected error occurred: $0";
        public static readonly string REEVAL_NOT_AVAILABLE = "\"reeval\" is not available before using \"eval\"";
        public static readonly string TOO_LONG_VARIABLE_NAME = "The length of the variable name exceeds the limit (StaticSettings.MaxNameCharCount: '$0')";
        public static readonly string TOO_LONG_FUNCTION_NAME = "The length of the function name exceeds the limit (StaticSettings.MaxNameCharCount: '$0')";
        public static readonly string VARIABLE_ALREADY_DECLARED = "The variable '$0' is already declared";
        public static readonly string FUNCTION_ALREADY_CONNECTED = "The function '$0' is already connected";
        public static readonly string INVALID_VARIABLE_ADDRESS = "Invalid memory address: '$0'";
    }
