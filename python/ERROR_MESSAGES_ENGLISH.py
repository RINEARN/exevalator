# !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
# To set the error-message language to English (the default),
# copy the contents below and
# overwrite the ErrorMessages class in exevalator.py with them.
# !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

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

