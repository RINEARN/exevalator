// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
// To set the error-message language to English (the default),
// copy the contents below and
// overwrite the ErrorMessages class in exevalator.ts with them.
// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

/**
 * Error messages of ExevalatorError,
 * which is thrown by Exevalator if the input expression is syntactically incorrect, or uses undeclared variables/functions.
 * You can customize the error message of Exevalator by modifying the values of the properties of this class.
 */
class ErrorMessages {
    public static readonly EMPTY_EXPRESSION: string = "The inputted expression is empty.";
    public static readonly TOO_MANY_TOKENS: string = "The number of tokens exceeds the limit (StaticSettings.MAX_TOKEN_COUNT: '$0')";
    public static readonly DEFICIENT_OPEN_PARENTHESIS: string = "The number of open parentheses '(' is deficient.";
    public static readonly DEFICIENT_CLOSED_PARENTHESIS: string = "The number of closed parentheses ')' is deficient.";
    public static readonly EMPTY_PARENTHESIS: string = "The content of parentheses '()' should not be empty.";
    public static readonly RIGHT_OPERAND_REQUIRED: string = "An operand is required at the right of: '$0'";
    public static readonly LEFT_OPERAND_REQUIRED: string = "An operand is required at the left of: '$0'";
    public static readonly RIGHT_OPERATOR_REQUIRED: string = "An operator is required at the right of: '$0'";
    public static readonly LEFT_OPERATOR_REQUIRED: string = "An operator is required at the left of: '$0'";
    public static readonly UNKNOWN_UNARY_PREFIX_OPERATOR: string = "Unknown unary-prefix operator: '$0'";
    public static readonly UNKNOWN_BINARY_OPERATOR: string = "Unknown binary operator: '$0'";
    public static readonly UNKNOWN_OPERATOR_SYNTAX: string = "Unknown operator syntax: '$0'";
    public static readonly EXCEEDS_MAX_AST_DEPTH: string = "The depth of the AST exceeds the limit (StaticSettings.MAX_AST_DEPTH: '$0')";
    public static readonly UNEXPECTED_PARTIAL_EXPRESSION: string = "Unexpected end of a partial expression";
    public static readonly INVALID_NUMBER_LITERAL: string = "Invalid number literal: '$0'";
    public static readonly INVALID_MEMORY_ADDRESS: string = "Invalid memory address: '$0'";
    public static readonly ADDRESS_MUST_BE_ZERO_OR_POSITIVE_INT32: string = "The address must be zero or a positive 32-bit integer: '$0'";
    public static readonly FUNCTION_ERROR: string = "Function Error ('$0'): $1";
    public static readonly VARIABLE_NOT_FOUND: string = "Variable not found: '$0'";
    public static readonly FUNCTION_NOT_FOUND: string = "Function not found: '$0'";
    public static readonly UNEXPECTED_OPERATOR: string = "Unexpected operator: '$0'";
    public static readonly UNEXPECTED_TOKEN: string = "Unexpected token: '$0'";
    public static readonly ARGS_MUST_NOT_BE_NULL_OR_UNDEFINED: string = "The argument(s) must not be null or undefined: '$0'";
    public static readonly TOO_LONG_EXPRESSION: string = "The length of the expression exceeds the limit (StaticSettings.MAX_EXPRESSION_CHAR_COUNT: '$0')";
    public static readonly UNEXPECTED_ERROR: string = "Unexpected error occurred: $0";
    public static readonly REEVAL_NOT_AVAILABLE: string = "\"reeval\" is not available before using \"eval\"";
    public static readonly TOO_LONG_VARIABLE_NAME: string = "The length of the variable name exceeds the limit (StaticSettings.MAX_NAME_CHAR_COUNT: '$0')";
    public static readonly TOO_LONG_FUNCTION_NAME: string = "The length of the function name exceeds the limit (StaticSettings.MAX_NAME_CHAR_COUNT: '$0')";
    public static readonly VARIABLE_ALREADY_DECLARED: string = "The variable '$0' is already declared";
    public static readonly FUNCTION_ALREADY_CONNECTED: string = "The function '$0' is already connected";
    public static readonly INVALID_VARIABLE_ADDRESS: string = "Invalid memory address: '$0'";
    public static readonly VARIABLE_COUNT_EXCEEDED_LIMIT: string = "The number of variables has exceeded the limit of: '$0'";
}
