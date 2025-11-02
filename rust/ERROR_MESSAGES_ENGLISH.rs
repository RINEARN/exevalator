
// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
// To set the error-message language to English (the default),
// copy the contents below and
// overwrite the ErrorMessages struct in exevalator.rs with them.
// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

/// Error messages of ExevalatorException,
/// which is thrown by Exevalator if the input expression is syntactically incorrect, or uses undeclared variables/functions.
/// You can customize the error message of Exevalator by modifying the values of the properties of this class.
struct ErrorMessages;
impl ErrorMessages {
    pub const EMPTY_EXPRESSION: &'static str = "The inputted expression is empty.";
    pub const TOO_MANY_TOKENS: &'static str = "The number of tokens exceeds the limit (StaticSettings.MAX_TOKEN_COUNT: '$0')";
    pub const DEFICIENT_OPEN_PARENTHESIS: &'static str = "The number of open parentheses '(' is deficient.";
    pub const DEFICIENT_CLOSED_PARENTHESIS: &'static str = "The number of closed parentheses ')' is deficient.";
    pub const EMPTY_PARENTHESIS: &'static str = "The content of parentheses '()' should not be empty.";
    pub const RIGHT_OPERAND_REQUIRED: &'static str = "An operand is required at the right of: '$0'";
    pub const LEFT_OPERAND_REQUIRED: &'static str = "An operand is required at the left of: '$0'";
    pub const RIGHT_OPERATOR_REQUIRED: &'static str = "An operator is required at the right of: '$0'";
    pub const LEFT_OPERATOR_REQUIRED: &'static str = "An operator is required at the left of: '$0'";
    pub const UNKNOWN_UNARY_PREFIX_OPERATOR: &'static str = "Unknown unary-prefix operator: '$0'";
    pub const UNKNOWN_BINARY_OPERATOR: &'static str = "Unknown binary operator: '$0'";
    // pub const UNKNOWN_OPERATOR_SYNTAX: &'static str = "Unknown operator syntax: '$0'";
    pub const EXCEEDS_MAX_AST_DEPTH: &'static str = "The depth of the AST exceeds the limit (StaticSettings.MAX_AST_DEPTH: '$0')";
    pub const UNEXPECTED_PARTIAL_EXPRESSION: &'static str = "Unexpected end of a partial expression";
    pub const INVALID_NUMBER_LITERAL: &'static str = "Invalid number literal: '$0'";
    // pub const INVALID_MEMORY_ADDRESS: &'static str = "Invalid memory address: '$0'";
    pub const FUNCTION_ERROR: &'static str = "Function Error ('$0'): $1";
    pub const VARIABLE_NOT_FOUND: &'static str = "Variable not found: '$0'";
    pub const FUNCTION_NOT_FOUND: &'static str = "Function not found: '$0'";
    // pub const UNEXPECTED_OPERATOR: &'static str = "Unexpected operator: '$0'";
    // pub const UNEXPECTED_TOKEN: &'static str = "Unexpected token: '$0'";
    pub const TOO_LONG_EXPRESSION: &'static str = "The length of the expression exceeds the limit (StaticSettings.MAX_EXPRESSION_CHAR_COUNT: '$0')";
    // pub const UNEXPECTED_ERROR: &'static str = "Unexpected error occurred: $0";
    pub const REEVAL_NOT_AVAILABLE: &'static str = "\"reeval\" is not available before using \"eval\"";
    pub const TOO_LONG_VARIABLE_NAME: &'static str = "The length of the variable name exceeds the limit (StaticSettings.MAX_NAME_CHAR_COUNT: '$0')";
    pub const TOO_LONG_FUNCTION_NAME: &'static str = "The length of the function name exceeds the limit (StaticSettings.MAX_NAME_CHAR_COUNT: '$0')";
    pub const VARIABLE_ALREADY_DECLARED: &'static str = "The variable '$0' is already declared";
    pub const FUNCTION_ALREADY_CONNECTED: &'static str = "The function '$0' is already connected";
    pub const INVALID_VARIABLE_ADDRESS: &'static str = "Invalid memory address: '$0'";
}
