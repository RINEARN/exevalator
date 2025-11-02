// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
// To set the error-message language to English (the default),
// copy the contents below and
// overwrite the ErrorMessages struct in exevalator.hpp with them.
// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

/**
 * Error messages of ExevalatorException,
 * which is thrown by Exevalator if the input expression is syntactically incorrect, or uses undeclared variables/functions.
 * You can customize the error message of Exevalator by modifying the values of the properties of this class.
 */
struct ErrorMessages {

    // Note: std::string_view requires C++17 or later.

    constexpr static std::string_view EMPTY_EXPRESSION { "The inputted expression is empty." };
    constexpr static std::string_view TOO_MANY_TOKENS { "The number of tokens exceeds the limit (StaticSettings.MAX_TOKEN_COUNT: '$0')" };
    constexpr static std::string_view DEFICIENT_OPEN_PARENTHESIS { "The number of open parentheses '(' is deficient." };
    constexpr static std::string_view DEFICIENT_CLOSED_PARENTHESIS { "The number of closed parentheses ')' is deficient." };
    constexpr static std::string_view EMPTY_PARENTHESIS { "The content of parentheses '()' should not be empty." };
    constexpr static std::string_view RIGHT_OPERAND_REQUIRED { "An operand is required at the right of: '$0'" };
    constexpr static std::string_view LEFT_OPERAND_REQUIRED { "An operand is required at the left of: '$0'" };
    constexpr static std::string_view RIGHT_OPERATOR_REQUIRED { "An operator is required at the right of: '$0'" };
    constexpr static std::string_view LEFT_OPERATOR_REQUIRED { "An operator is required at the left of: '$0'" };
    constexpr static std::string_view UNKNOWN_UNARY_PREFIX_OPERATOR { "Unknown unary-prefix operator: '$0'" };
    constexpr static std::string_view UNKNOWN_BINARY_OPERATOR { "Unknown binary operator: '$0'" };
    constexpr static std::string_view UNKNOWN_OPERATOR_SYNTAX { "Unknown operator syntax: '$0'" };
    constexpr static std::string_view EXCEEDS_MAX_AST_DEPTH { "The depth of the AST exceeds the limit (StaticSettings.MAX_AST_DEPTH: '$0')" };
    constexpr static std::string_view UNEXPECTED_PARTIAL_EXPRESSION { "Unexpected end of a partial expression" };
    constexpr static std::string_view INVALID_NUMBER_LITERAL { "Invalid number literal: '$0'" };
    constexpr static std::string_view INVALID_MEMORY_ADDRESS { "Invalid memory address: '$0'" };
    constexpr static std::string_view FUNCTION_ERROR { "Function Error ('$0'): $1" };
    constexpr static std::string_view VARIABLE_NOT_FOUND { "Variable not found: '$0'" };
    constexpr static std::string_view FUNCTION_NOT_FOUND { "Function not found: '$0'" };
    constexpr static std::string_view UNEXPECTED_OPERATOR { "Unexpected operator: '$0'" };
    constexpr static std::string_view UNEXPECTED_TOKEN { "Unexpected token: '$0'" };
    constexpr static std::string_view TOO_LONG_EXPRESSION { "The length of the expression exceeds the limit (StaticSettings.MAX_EXPRESSION_CHAR_COUNT: '$0')" };
    constexpr static std::string_view UNEXPECTED_ERROR { "Unexpected error occurred: $0" };
    constexpr static std::string_view REEVAL_NOT_AVAILABLE { "\"reeval\" is not available before using \"eval\"" };
    constexpr static std::string_view TOO_LONG_VARIABLE_NAME { "The length of the variable name exceeds the limit (StaticSettings.MAX_NAME_CHAR_COUNT: '$0')" };
    constexpr static std::string_view TOO_LONG_FUNCTION_NAME { "The length of the function name exceeds the limit (StaticSettings.MAX_NAME_CHAR_COUNT: '$0')" };
    constexpr static std::string_view VARIABLE_ALREADY_DECLARED { "The variable '$0' is already declared" };
    constexpr static std::string_view FUNCTION_ALREADY_CONNECTED { "The function '$0' is already connected" };
    constexpr static std::string_view INVALID_VARIABLE_ADDRESS { "Invalid memory address: '$0'" };
    constexpr static std::string_view INVALID_FUNCTION_POINTER { "A null/empty function pointer has been connected." };
};
