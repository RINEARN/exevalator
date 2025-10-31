// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
// To set the error-message language to English (the default),
// copy the contents below and
// overwrite the ErrorMessage class in exevalator.java with them.
// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

/**
 * Error messages of ExevalatorException,
 * which is thrown by Exevalator if the input expression is syntactically incorrect, or uses undeclared variables/functions.
 * You can customize the error message of Exevalator by modifying the values of the properties of this class.
 */
final class ErrorMessages {
    public static final String EMPTY_EXPRESSION = "The inputted expression is empty.";
    public static final String TOO_MANY_TOKENS = "The number of tokens exceeds the limit (StaticSettings.MAX_TOKEN_COUNT: '$0')";
    public static final String DEFICIENT_OPEN_PARENTHESIS = "The number of open parentheses '(' is deficient.";
    public static final String DEFICIENT_CLOSED_PARENTHESIS = "The number of closed parentheses ')' is deficient.";
    public static final String EMPTY_PARENTHESIS = "The content of parentheses '()' should not be empty.";
    public static final String RIGHT_OPERAND_REQUIRED = "An operand is required at the right of: '$0'";
    public static final String LEFT_OPERAND_REQUIRED = "An operand is required at the left of: '$0'";
    public static final String RIGHT_OPERATOR_REQUIRED = "An operator is required at the right of: '$0'";
    public static final String LEFT_OPERATOR_REQUIRED = "An operator is required at the left of: '$0'";
    public static final String UNKNOWN_UNARY_PREFIX_OPERATOR = "Unknown unary-prefix operator: '$0'";
    public static final String UNKNOWN_BINARY_OPERATOR = "Unknown binary operator: '$0'";
    public static final String UNKNOWN_OPERATOR_SYNTAX = "Unknown operator syntax: '$0'";
    public static final String EXCEEDS_MAX_AST_DEPTH = "The depth of the AST exceeds the limit (StaticSettings.MAX_AST_DEPTH: '$0')";
    public static final String UNEXPECTED_PARTIAL_EXPRESSION = "Unexpected end of a partial expression";
    public static final String INVALID_NUMBER_LITERAL = "Invalid number literal: '$0'";
    public static final String INVALID_MEMORY_ADDRESS = "Invalid memory address: '$0'";
    public static final String FUNCTION_ERROR = "Function Error ('$0'): $1";
    public static final String VARIABLE_NOT_FOUND = "Variable not found: '$0'";
    public static final String FUNCTION_NOT_FOUND = "Function not found: '$0'";
    public static final String UNEXPECTED_OPERATOR = "Unexpected operator: '$0'";
    public static final String UNEXPECTED_TOKEN = "Unexpected token: '$0'";
    public static final String TOO_LONG_EXPRESSION = "The length of the expression exceeds the limit (StaticSettings.MAX_EXPRESSION_CHAR_COUNT: '$0')";
    public static final String UNEXPECTED_ERROR = "Unexpected error occurred: $0";
    public static final String REEVAL_NOT_AVAILABLE = "\"reeval\" is not available before using \"eval\"";
    public static final String TOO_LONG_VARIABLE_NAME = "The length of the variable name exceeds the limit (StaticSettings.MAX_NAME_CHAR_COUNT: '$0')";
    public static final String TOO_LONG_FUNCTION_NAME = "The length of the function name exceeds the limit (StaticSettings.MAX_NAME_CHAR_COUNT: '$0')";
    public static final String VARIABLE_ALREADY_DECLARED = "The variable '$0' is already declared";
    public static final String FUNCTION_ALREADY_CONNECTED = "The function '$0' is already connected";
    public static final String INVALID_VARIABLE_ADDRESS = "Invalid memory address: '$0'";
}
