/*
 * Exevalator Ver.2.0.1 - by RINEARN 2021-2024
 * This software is released under the "Unlicense" license.
 * You can choose the "CC0" license instead, if you want.
 */

#ifndef EXEVALATOR_CPP
#define EXEVALATOR_CPP

#include <string>
#include <vector>
#include <map>
#include <set>
#include <memory>
#include <limits>
#include <cctype>
#include <cstdint>
#include <cstdlib>
#include <stdexcept>

#include "exevalator.hpp"


namespace exevalator {

/**
 * Creates a new interpreter engine of Exevalator.
 */
Exevalator::Exevalator() {
    this->last_evaluated_expression = std::string { "" };
}

/**
 * Releases resources of the interpreter engine of Exevalator.
 */
Exevalator::~Exevalator() {
    this->memory.clear();
    this->memory.shrink_to_fit();
    this->variable_table.clear();
    std::map<std::string, size_t>{}.swap(this->variable_table);
    this->function_table.clear();
    std::map<std::string, std::shared_ptr<ExevalatorFunctionInterface>>{}.swap(this->function_table);
    this->last_evaluated_expression.clear();
    this->last_evaluated_expression.shrink_to_fit();
}

/**
 * Evaluates (computes) the value of an expression.
 *
 * @param expression The expression to be evaluated
 * @return The evaluated value
 */
double Exevalator::eval(const std::string &expression) {
    if (this->settings.max_expression_char_count <= expression.length()) {
        throw new ExevalatorException(
            std::string { "The length of the expression exceeds the limit: Settings.max_expression_char_count: " }
            + std::to_string(settings.max_expression_char_count)
        );
    }

    try {
        // If the expression changed from the last-evaluated expression, re-parsing is necessary.
        if (!(this->last_evaluated_expression == expression) || !(this->evaluator.is_evaluatable())) {

            // Perform lexical analysis, and get tokens from the inputted expression.
            std::vector<Token> tokens = LexicalAnalyzer::analyze(std::string(expression), settings);

            /*
            // Temporary, for debugging tokens
            for (size_t itoken=0; itoken<tokens.size(); ++itoken) {
                std::cout << "tokens[" << itoken << "]: " << tokens[itoken].word << std::endl;
            }
            */

            // Perform parsing, and get AST(Abstract Syntax Tree) from tokens.
            std::unique_ptr<AstNode> ast = Parser::parse(tokens);

            /*
            // Temporary, for debugging AST
            std::cout << ast->to_markupped_text(0) << std::endl;
            */

            // Update the evaluator, to evaluate the parsed AST.
            this->evaluator.update(
                this->settings, ast, this->variable_table, this->function_table
            );

            this->last_evaluated_expression = expression;
        }

        // Evaluate the value of the expression, and return it.
        double evaluated_value = this->evaluator.evaluate(this->memory);
        return evaluated_value;

    } catch (ExevalatorException ee) {
        throw ee;

    // Wrap an unexpected exception by Exevalator.Exception and rethrow it.
    } catch (...) {
        throw ExevalatorException { "Unexpected exception/error occurred" };
    }
}

/**
 * Re-evaluates (re-computes) the value of the expression evaluated by "eval" method last time.
 * This method works faster than calling "eval" method repeatedly for the same expression.
 * Note that, the result value may different with the last evaluated value, 
 * if values of variables or behaviour of functions had changed.
 * 
 * @return The evaluated value
 */
double Exevalator::reeval() {
    if (this->evaluator.is_evaluatable()) {
        double evaluated_value = this->evaluator.evaluate(this->memory);
        return evaluated_value;
    } else {
        throw new ExevalatorException("\"reeval\" is not available before using \"eval\"");
    }
}

/**
 * Declares a new variable, for using the value of it in expressions.
 * 
 * @param name The name of the variable to be declared
 * @returns The virtual address of the declared variable,
 *          which useful for accessing to the variable faster.
 *          See "write_variable_at" and "read_variable_at" method.
 */
size_t Exevalator::declare_variable(const std::string &name) {
    if (this->settings.max_name_char_count <= name.length()) {
        throw new ExevalatorException(
            std::string { "The length of the variable name exceeds the limit: Settings.max_name_char_count: " }
            + std::to_string(settings.max_name_char_count)
        );
    }
    size_t address = this->memory.size();
    this->memory.push_back(0.0);
    this->variable_table[name] = address;
    return address;
}

/**
 * Writes the value to the variable having the specified name.
 *
 * @param name The name of the variable to be written
 * @param value The new value of the variable
 */
void Exevalator::write_variable(const std::string &name, double value) {
    if (this->settings.max_name_char_count <= name.length()
            || this->variable_table.find(name) == this->variable_table.end()) {
        throw ExevalatorException((std::string { "Variable not found: " } + name).c_str());
    }
    size_t address = this->variable_table[name];
    this->memory[address] = value;
}

/**
 * Writes the value to the variable at the specified virtual address.
 * This function works faster than "write_variable" function.
 * 
 * @param address The virtual address of the variable to be written
 * @param value The new value of the variable
 */
void Exevalator::write_variable_at(size_t address, double value) {
    if (this->memory.size() <= address) {
        throw ExevalatorException((std::string { "Invalid variable address: " } + std::to_string(address)).c_str());
    }
    this->memory[address] = value;
}

/**
 * Reads the value of the variable having the specified name.
 * 
 * @param name The name of the variable to be read
 * @return The current value of the variable
 */
double Exevalator::read_variable(const std::string &name) {
    if (this->settings.max_name_char_count <= name.length()
            || this->variable_table.find(name) == this->variable_table.end()) {
        throw ExevalatorException((std::string { "Variable not found: " } + name).c_str());
    }
    size_t address = this->variable_table[name];
    return this->memory[address];
}

/**
 * Reads the value of the variable at the specified virtual address.
 * This function works faster than "read_variable" function.
 * 
 * @param address The virtual address of the variable to be read
 * @return The current value of the variable
 */
double Exevalator::read_variable_at(size_t address) {
    if (this->memory.size() <= address) {
        throw ExevalatorException((std::string { "Invalid variable address: " } + std::to_string(address)).c_str());
    }
    return this->memory[address];
}

/**
 * Connects a function, for using it in expressions.
 * 
 * @param name The name of the function used in the expression
 * @param function The function to be connected
 */
void Exevalator::connect_function(const std::string &name, const std::shared_ptr<ExevalatorFunctionInterface> &function_ptr) {
    if (this->settings.max_name_char_count <= name.length()) {
        throw new ExevalatorException(
            std::string { "The length of the variable name exceeds the limit: Settings.max_name_char_count: " }
            + std::to_string(settings.max_name_char_count)
        );
    }
    if (!function_ptr) {
        throw new ExevalatorException(
            std::string { "A null/empty function pointer has been connected." }
        );
    }
    this->function_table[name] = function_ptr;
}


// The implementation of the virtual destructor of ExevalatorFunctionInterface.
ExevalatorFunctionInterface::~ExevalatorFunctionInterface() {
}


/**
 * Creates an new Exception having the specified error message.
 * 
 * @param error_message The error message
 */
ExevalatorException::ExevalatorException(std::string error_message): std::exception {} {
    this->error_message = error_message;
};
ExevalatorException::~ExevalatorException() {
    this->error_message.clear();
    this->error_message.shrink_to_fit();
};
const char* ExevalatorException::what() const noexcept {
    return this->error_message.c_str();
}


/**
 * Splits (tokenizes) the expression into tokens, and analyze them.
 *
 * @param expression The expression to be tokenized/analyzed
 * @param settings The Setting instance storing setting values
 * @return Analyzed tokens
 */
std::vector<Token> LexicalAnalyzer::analyze(const std::string &expression, const Settings &settings) {

    // Extracts and escapes all number literals in the expression.
    std::vector<std::string> number_literals;
    std::string literal_escaped_expression = LexicalAnalyzer::escape_number_literals(
        expression, number_literals, settings
    );

    // Split the expression into token words.
    std::vector<std::string> token_words = LexicalAnalyzer::split_expression_into_token_words(
        literal_escaped_expression, settings
    );

    // Checks the total number of tokens.
    if (token_words.size() == 0) {
        throw ExevalatorException("The inputted expression is empty.");
    }
    if (settings.max_token_count < token_words.size()) {
        throw new ExevalatorException(
            std::string { "The number of tokens exceeds the limit: Settings.max_token_count: " }
            + std::to_string(settings.max_token_count)
        );
    }

    // Create Token structs.
    // Also, escaped number literals will be recovered.
    std::vector<Token> tokens = LexicalAnalyzer::create_tokens_from_token_words(
        token_words, number_literals, settings
    );

    // Check syntactic correctness of tokens.
    LexicalAnalyzer::check_parenthesis_opening_closings(tokens);
    LexicalAnalyzer::check_empty_parentheses(tokens);
    LexicalAnalyzer::check_locations_of_operators_and_leafs(tokens);

    return tokens;
}

/**
 * Detects the end of the specified number literal in the expression.
 * 
 * @param expression The expression containing the number literal
 * @param literal_begin The index of the character at the beginning of the number literal
 * @return The index of the character at the end of the number literal
 */
size_t LexicalAnalyzer::detect_end_of_num_literal(const std::string &expression, size_t literal_begin) {
    size_t char_count = expression.length();
    bool is_integer_part = true;
    bool is_decimal_part = false;
    bool is_exponent_part = false;

    for (size_t ichar=literal_begin; ichar<char_count; ++ichar) {
        char ch = expression.at(ichar);

        // Numbers (0,1,2, ... 9)
        if (std::isdigit(ch)) {
            continue;

        // Decimal point
        } else if (ch == '.') {

            // If reading integer part, go to decimal part
            if (is_integer_part) {
                is_integer_part = false;
                is_decimal_part = true;

            // '.' can be only between integer part and decimal part,
            // so elsewhere the '.' belongs to the next token.
            } else {
                return ichar - 1;
            }

        // Beginning of the exponent part
        } else if (ch == 'e' || ch == 'E') {

            // If reading integer or decimal part, go to exponent part
            if (is_integer_part || is_decimal_part) {
                is_integer_part = false;
                is_decimal_part = false;
                is_exponent_part = true;

            // Elsewhere the 'e'/'E' belongs to the next token.
            } else {
                return ichar - 1;
            } 

        // Sign of the exponent part
        } else if (ch == '+' || ch == '-') {

            // In the number literal, the sign can only be at the beginning of the exponent part,
            // so elsewhere it is the next token (operator).
            // Note that, '-' of "-1.23" is a unary minus operator, not contained in the number literal.
            if (!is_exponent_part || (expression.at(ichar - 1) != 'e' && expression.at(ichar - 1) != 'E')) {
                if (ichar == 0)  {
                    throw ExevalatorException("Incorrect beggining of number literal.");
                }
                return ichar - 1;
            }

        } else {
            if (ichar == 0)  {
                throw ExevalatorException("Incorrect beggining of number literal.");
            }
            return ichar - 1;
        }
    }

    // Reaches when the number literal ends at the end of the expression.
    return char_count - 1;
}

/**
 *  Extracts all number literals in the expression, and replaces them to the escaped representation.
 * 
 * @param expression The expression to be processed
 * @param literal_store The vector to which extracted number literals will be stored
 * @param settings The Setting instance storing setting values
 * @return The expression in which all number literals are escaped
 */
std::string LexicalAnalyzer::escape_number_literals(
        const std::string &expression, std::vector<std::string> &literal_store, const Settings &settings) {

    size_t char_count = expression.length();
    std::string escaped_expression;

    for (size_t ichar=0; ichar<char_count; ++ichar) {
        char ch = expression.at(ichar);

        bool token_begin = ichar == 0
            || expression.at(ichar-1) == ' '
            || settings.token_splitter_character_set.find(expression.at(ichar-1)) != settings.token_splitter_character_set.end();

        // Numbers (0,1,2, ... 9)
        if (token_begin && std::isdigit(ch)) {
            size_t literal_end = LexicalAnalyzer::detect_end_of_num_literal(expression, ichar);
            std::string literal = expression.substr(ichar, literal_end + 1);
            literal_store.push_back(literal);
            ichar = literal_end;
            escaped_expression.append(settings.escaped_number_literal);
        } else {
            escaped_expression.push_back(ch);
        }
    }

    return escaped_expression;
}

/**
 * Sprits (tokenizes) the expression into token-words. 
 * The type of each word is "string", not "Token" struct yet, at this stage.
 * 
 * @param expression The expression to be splitted (tokenized) into token-words
 * @param settings The Setting instance storing setting values
 * @return Token-words
 */
std::vector<std::string> LexicalAnalyzer::split_expression_into_token_words(
        const std::string &expression, const Settings &settings) {

    std::string spaced_expression;
    size_t char_count = expression.length();

    for (size_t ichar=0; ichar<char_count; ++ichar) {
        char ch = expression.at(ichar);
        if (settings.token_splitter_character_set.find(ch) != settings.token_splitter_character_set.end()) {
            spaced_expression.push_back(' ');
            spaced_expression.push_back(ch);
            spaced_expression.push_back(' ');
        } else {
            spaced_expression.push_back(ch);
        }
    }

    std::vector<std::string> token_words;
    size_t first_non_space = spaced_expression.find_first_not_of(' ');
    if (first_non_space == std::string::npos) {
        return token_words;
    }
    size_t non_space_length = spaced_expression.find_last_not_of(' ') - first_non_space + 1;
    std::string trimmed_expression = spaced_expression.substr(first_non_space, non_space_length);
    std::string token_word;
    size_t trimmed_char_count = trimmed_expression.length();
    char last_char = ' ';
    for (size_t ichar=0; ichar<trimmed_char_count; ++ichar) {
        char ch = trimmed_expression.at(ichar);
        if (ch == ' ') {
            if (last_char != ' ') {
                token_words.push_back(token_word);
                token_word = std::string { };
            }
        } else {
            token_word.push_back(ch);
        }
        last_char = ch;
    }
    if (token_word.length() != 0) {
        token_words.push_back(token_word);
    }
    return token_words;
}

/**
 * Creates "Token" type instances from "string" type token-words.
 * Also, escaped number literals will be recovered.
 * 
 * @param token_words Token-words to be converted to `Token` type instances
 * @param number_literals Values of number literals to be recovered
 * @param settings settings The Setting instance storing setting values
 * @return "Token" type instances
 */
std::vector<Token> LexicalAnalyzer::create_tokens_from_token_words(
        const std::vector<std::string> &token_words, const std::vector<std::string> &number_literals, const Settings &settings) {

    size_t token_count = token_words.size();

    // Stores the parenthesis-depth, which will increase at "(" and decrease at ")".
    int64_t parenthesis_depth = 0;

    // Stores the parenthesis-depth when a function call operator begins,
    // for detecting the end of the function operator.
    std::set<int64_t> call_parenthesis_depths;

    Token last_token;
    bool has_last_token = false;

    size_t literal_count = 0;
    std::vector<Token> tokens;
    for (int itoken=0; itoken<token_count; ++itoken) {
        std::string word = token_words[itoken];

        // Cases of open parentheses, or beginning of function calls.
        if (word == "(") {
            parenthesis_depth++;
            if (1 <= itoken && tokens[itoken - 1].type == TokenType::FUNCTION_IDENTIFIER) {
                call_parenthesis_depths.insert(parenthesis_depth);
                OperatorInfo opinfo = settings.call_symbol_operator_map.at(word[0]);
                tokens.push_back(Token{ TokenType::OPERATOR, word, opinfo });
            } else {
                tokens.push_back(Token{ TokenType::PARENTHESIS, word });
            }

        // Cases of closes parentheses, or end of function calls.
        } else if (word == ")") {
            if (call_parenthesis_depths.find(parenthesis_depth) != call_parenthesis_depths.end()) {
                call_parenthesis_depths.erase(parenthesis_depth);
                OperatorInfo opinfo = settings.call_symbol_operator_map.at(word[0]);
                tokens.push_back(Token{ TokenType::OPERATOR, word, opinfo });
            } else {
                tokens.push_back(Token{ TokenType::PARENTHESIS, word });
            }
            parenthesis_depth--;

        // Case of operators.
        } else if (word.length() == 1 && settings.operator_symbol_set.find(word[0]) != settings.operator_symbol_set.end()) {

            OperatorInfo opinfo;
            if (!has_last_token || last_token.word == "("
                        || (last_token.type == TokenType::OPERATOR && last_token.opinfo.type != OperatorType::CALL)) {

                if (settings.unary_prefix_symbol_operator_map.find(word[0]) != settings.unary_prefix_symbol_operator_map.end()) {
                    opinfo = settings.unary_prefix_symbol_operator_map.at(word[0]);
                } else {
                    throw ExevalatorException(std::string { "No such unary-prefix operator: " } + word );
                }

            } else if (last_token.word == ")"
                        || last_token.type == TokenType::NUMBER_LITERAL
                        || last_token.type == TokenType::VARIABLE_IDENTIFIER) {

                if (settings.binary_symbol_operator_map.find(word[0]) != settings.binary_symbol_operator_map.end()) {
                    opinfo = settings.binary_symbol_operator_map.at(word[0]);
                } else {
                    throw ExevalatorException(std::string { "No such binary operator: " } + word );
                }

            } else {
                std::string error_message = "Unexpected operator syntax: " + word;
                throw ExevalatorException { error_message.c_str() };
            }
            tokens.push_back(Token{ TokenType::OPERATOR, word, opinfo });

        // Cases if literals and separator.
        } else if (word == settings.escaped_number_literal) {
            tokens.push_back(Token{ TokenType::NUMBER_LITERAL, number_literals[literal_count] });
            literal_count++;
        } else if (word == ",") {
            tokens.push_back(Token{ TokenType::EXPRESSION_SEPARATOR, word });

        // Cases of variable identifier of function identifier.
        } else {
            if (itoken < token_count - 1 && token_words[itoken + 1] == "(") {
                tokens.push_back(Token{ TokenType::FUNCTION_IDENTIFIER, word });
            } else {
                tokens.push_back(Token{ TokenType::VARIABLE_IDENTIFIER, word });
            }
        }
        last_token = tokens.back();
        has_last_token = true;
    }

    return tokens;
}

/**
 * Checks the number and correspondence of open "(" / closed ")" parentheses.
 * An ExevalatorException will be thrown when any errors detected.
 * If no error detected, nothing will occur.
 *
 * @param tokens Tokens of the inputted expression.
 */
void LexicalAnalyzer::check_parenthesis_opening_closings(const std::vector<Token> &tokens) {
    size_t token_count = tokens.size();
    int64_t hierarchy = 0; // Increases at "(" and decreases at ")".

    for (int64_t itoken=0; itoken<token_count; ++itoken) {
        Token token = tokens[itoken];
        if (token.word == "(") {
            hierarchy++;
        } else if (token.word == ")") {
            hierarchy--;
        }

        // If the value of hierarchy is negative, the open parenthesis is deficient.
        if (hierarchy < 0) {
            throw ExevalatorException { "The number of open parenthesis \"(\" is deficient." };
        }
    }

    // If the value of hierarchy is not zero at the end of the expression,
    // the closed parentheses ")" is deficient.
    if (hierarchy > 0) {
        throw ExevalatorException { "The number of open parenthesis \")\" is deficient." };
    }
}

/**
 * Checks that empty parentheses "()" are not contained in the expression.
 * An ExevalatorException will be thrown when any errors detected.
 * If no error detected, nothing will occur.
 *
 * @param tokens Tokens of the inputted expression.
 */
void LexicalAnalyzer::check_empty_parentheses(const std::vector<Token> &tokens) {
    size_t token_count = tokens.size();
    size_t content_counter = 0;

    for (int itoken=0; itoken<token_count; ++itoken) {
        Token token = tokens[itoken];
        if (token.type == TokenType::PARENTHESIS) { // Excepting CALL operators
            if (token.word == "(") {
                content_counter = 0;
            } else if (token.word == ")") {
                if (content_counter == 0) {
                    throw ExevalatorException {
                        "The content parentheses \"()\" should not be empty (excluding function calls)."
                    };
                }
            }
        } else {
            content_counter++;
        }
    }
}

/**
 * Checks correctness of locations of operators and leaf elements (literals and identifiers).
 * An ExevalatorException will be thrown when any errors detected.
 * If no error detected, nothing will occur.
 *
 * @param tokens Tokens of the inputted expression.
 */
void LexicalAnalyzer::check_locations_of_operators_and_leafs(const std::vector<Token> &tokens) {
    size_t token_count = tokens.size();
    std::set<TokenType> leaf_type_set;
    leaf_type_set.insert(TokenType::NUMBER_LITERAL);
    leaf_type_set.insert(TokenType::VARIABLE_IDENTIFIER);

    // Reads and check tokens from left to right.
    for (int itoken=0; itoken<token_count; ++itoken) {
        Token token = tokens[itoken];

        // Prepare information of next/previous token.
        bool next_is_leaf = itoken != token_count-1 && leaf_type_set.find(tokens[itoken+1].type) != leaf_type_set.end();
        bool prev_is_leaf = itoken != 0 && leaf_type_set.find(tokens[itoken-1].type) != leaf_type_set.end();
        bool next_is_open_parenthesis = itoken < token_count-1 && tokens[itoken+1].word == "(";
        bool prev_is_close_parenthesis = itoken != 0 && tokens[itoken-1].word == ")";
        bool next_is_prefix_operator = itoken < token_count-1
                    && tokens[itoken+1].type == TokenType::OPERATOR
                    && tokens[itoken+1].opinfo.type == OperatorType::UNARY_PREFIX;
        bool next_is_function_call_begin = next_is_open_parenthesis
                    && tokens[itoken+1].type == TokenType::OPERATOR
                    && tokens[itoken+1].opinfo.type == OperatorType::CALL;
        bool next_is_function_identifier = itoken < token_count-1
                    && tokens[itoken+1].type == TokenType::FUNCTION_IDENTIFIER;

        // Case of operators
        if (token.type == TokenType::OPERATOR) {
            OperatorType optype = token.opinfo.type;

            // Cases of unary-prefix operators
            if (optype == OperatorType::UNARY_PREFIX) {

                // Only leafs, open parentheses, and unary-prefix operators can be operands.
                if ( !(next_is_leaf || next_is_open_parenthesis || next_is_prefix_operator) ) {
                    std::string error_message = std::string { "An operand is required at the right of: \"{}\"" } + token.word;
                    throw ExevalatorException { error_message.c_str() };
                }
            } // Cases of unary-prefix operators

            // Cases of binary operators or a separator of partial expressions
            if (optype == OperatorType::BINARY || token.word == ",") {

                // Only leaf elements, open parenthesis, and unary-prefix operator can be a right-operand.
                if ( !(next_is_leaf || next_is_open_parenthesis || next_is_prefix_operator || next_is_function_identifier) ) {
                    std::string error_message = std::string { "An operand is required at the right of: \"{}\"" } + token.word;
                    throw ExevalatorException { error_message.c_str() };
                }
                // Only leaf elements and closed parenthesis can be a right-operand.
                if ( !(prev_is_leaf || prev_is_close_parenthesis) ) {
                    std::string error_message = std::string { "An operand is required at the left of: \"{}\"" } + token.word;
                    throw ExevalatorException { error_message.c_str() };
                }
            } // Cases of binary operators or a separator of partial expressions

        } // Case of operators

        // Case of leaf elements
        if (leaf_type_set.find(token.type) != leaf_type_set.end()) {

            // An other leaf element or an open parenthesis can not be at the right of an leaf element.
            if (!next_is_function_call_begin && (next_is_open_parenthesis || next_is_leaf)) {
                std::string error_message = std::string { "An operand is required at the right of: \"{}\"" } + token.word;
                throw ExevalatorException { error_message.c_str() };
            }

            // An other leaf element or a closed parenthesis can not be at the left of an leaf element.
            if (prev_is_close_parenthesis || prev_is_leaf) {
                std::string error_message = std::string { "An operand is required at the left of: \"{}\"" } + token.word;
                throw ExevalatorException { error_message.c_str() };
            }
        } // Case of leaf elements
    } // Loops for each token
}


/**
 * Parses tokens and construct Abstract Syntax Tree (AST).
 *
 * @param tokens Tokens to be parsed
 * @return The root node of the constructed AST
 */
std::unique_ptr<AstNode> Parser::parse(const std::vector<Token> &tokens) {

    // Number of tokens
    size_t token_count = tokens.size();

    // Working stack to form multiple AstNode instances into a tree-shape.
    std::vector<std::unique_ptr<AstNode>> stack;

    // Tokens of temporary nodes used in the above working stack, for isolating ASTs of partial expressions.
    Token parenthesis_lid_token = Token { TokenType::STACK_LID, "(PARENTHESIS_STACK_LID)" };
    Token separator_lid_token   = Token { TokenType::STACK_LID, "(SEPARATOR_STACK_LID)"   };
    Token call_begin_lid_token  = Token { TokenType::STACK_LID, "(CALL_BEGIN_STACK_LID)"  };

    // The vector storing next operator's precedence for each token.
    // At [i], it is stored that the precedence of the first operator of which token-index is greater than i.
    std::vector<uint64_t> next_operator_precedences = Parser::get_next_operator_precedences(tokens);

    // Read tokens from left to right.
    size_t itoken = 0;
    while (itoken < token_count) {

        Token token = tokens[itoken];
        std::unique_ptr<AstNode> operator_node;

        // Case of literals and identifiers: "1.23", "x", "f", etc.
        if (token.type == TokenType::NUMBER_LITERAL
                || token.type == TokenType::VARIABLE_IDENTIFIER
                || token.type == TokenType::FUNCTION_IDENTIFIER) {
            stack.push_back(std::make_unique<AstNode>(token));
            itoken++;
            continue;

        // Case of parenthesis: "(" or ")"
        } else if (token.type == TokenType::PARENTHESIS) {
            if (token.word == "(") {
                //stack.push_back(parenthesis_lid.clone());
                stack.push_back(std::make_unique<AstNode>(parenthesis_lid_token));
                itoken++;
                continue;
            } else { // Case of ")"
                std::vector<std::unique_ptr<AstNode>> partial_expr_nodes;
                Parser::pop_partial_expr_nodes(partial_expr_nodes, stack, parenthesis_lid_token);
                operator_node = std::move(partial_expr_nodes.front());
            }

        // Case of separator: ","
        } else if (token.type == TokenType::EXPRESSION_SEPARATOR) {
            stack.push_back(std::make_unique<AstNode>(separator_lid_token));
            itoken++;
            continue;

        // Case of operators: "+", "-", etc.
        } else if (token.type == TokenType::OPERATOR) {
            operator_node = std::make_unique<AstNode>(token);
            uint64_t next_op_precedence = next_operator_precedences[itoken];

            // Case of unary-prefix operators:
            // * Connect the node of right-token as an operand, if necessary (depending the next operator's precedence).
            if (token.opinfo.type == OperatorType::UNARY_PREFIX) {
                if (Parser::should_add_right_operand(token.opinfo.precedence, next_op_precedence)) {
                    operator_node->child_nodes.push_back( std::make_unique<AstNode>(tokens[itoken + 1]) );
                    itoken++;
                } // else: Operand will be connected later. See the bottom of this loop.

            // Case of binary operators:
            // * Always connect the node of left-token as an operand.
            // * Connect the node of right-token as an operand, if necessary (depending the next operator's precedence).
            } else if (token.opinfo.type == OperatorType::BINARY) {
                operator_node->child_nodes.push_back(std::move(stack.back()));
                stack.pop_back();
                if (Parser::should_add_right_operand(token.opinfo.precedence, next_op_precedence)) {
                    operator_node->child_nodes.push_back(std::make_unique<AstNode>(tokens[itoken + 1]));
                    itoken++;
                } // else: Right-operand will be connected later. See the bottom of this loop.

            // Case of function-call operators.
            } else if (token.opinfo.type == OperatorType::CALL) {
                if (token.word == "(") {
                    operator_node->child_nodes.push_back(std::move(stack.back())); // Add function-identifier node at the top of the stack.
                    stack.pop_back();
                    stack.push_back(std::move(operator_node));
                    stack.push_back(std::make_unique<AstNode>(call_begin_lid_token)); // The marker to correct partial expressions of args from the stack.
                    itoken++;
                    continue;
                } else { // Case of ")"
                    std::vector<std::unique_ptr<AstNode>> arg_nodes;
                    Parser::pop_partial_expr_nodes(arg_nodes, stack, call_begin_lid_token);
                    size_t arg_count = arg_nodes.size();
                    operator_node = std::move(stack.back());
                    stack.pop_back();
                    for (size_t iarg=0; iarg<arg_count; ++iarg) {
                        operator_node->child_nodes.push_back(std::move(arg_nodes[arg_count - iarg - 1])); // Adding and reversing the order.
                    }
                }
            }
        }
        
        // If the precedence of the operator at the top of the stack is stronger than the next operator,
        // connect all "unconnected yet" operands and operators in the stack.
        while (Parser::should_add_right_operand_to_stacked_operator(stack, next_operator_precedences[itoken])) {
            std::unique_ptr<AstNode> old_operator_node = std::move(operator_node);
            operator_node = std::move(stack.back());
            stack.pop_back();
            operator_node->child_nodes.push_back(std::move(old_operator_node));
        }
        stack.push_back(std::move(operator_node));
        itoken++;
    }

    // The AST has been constructed on the stack, and only its root node is stored in the stack, so return it.
    return std::move(stack.back());
}

/**
 * Judges whether the right-side token should be connected directly as an operand, to the target operator.
 *
 * @param target_operator_precedence The precedence of the target operator (smaller value gives higher precedence)
 * @param next_operator_precedence The precedence of the next operator (smaller value gives higher precedence)
 * @return Returns true if the right-side token (operand) should be connected to the target operator
 */
constexpr bool Parser::should_add_right_operand(uint64_t target_operator_precedence, uint64_t next_operator_precedence) {
    return target_operator_precedence <= next_operator_precedence; // left is stronger
}

/**
 * Judges whether the right-side token should be connected directly as an operand,
 * to the operator at the top of the working stack.
 *
 * @param stack The working stack used for the parsing
 * @param next_operator_precedence The precedence of the next operator (smaller value gives higher precedence)
 * @return Returns true if the right-side token (operand) should be connected to the operator at the top of the stack
 */
bool Parser::should_add_right_operand_to_stacked_operator(
        const std::vector<std::unique_ptr<AstNode>> &stack, uint64_t next_operator_precedence) {

    if (stack.size() == 0) {
        return false;
    }
    if (stack.back()->token.type != TokenType::OPERATOR) {
        return false;
    }
    return Parser::should_add_right_operand(
        stack.back()->token.opinfo.precedence, next_operator_precedence
    );
}

/**
 * Pops root nodes of ASTs of partial expressions constructed on the stack.
 *
 * @param ret Root nodes of ASTs of partial expressions
 * @param stack The working stack used for the parsing
 * @param end_stack_lid_node_token The token of the temporary node pushed in the stack,
 *                                 at the end of partial expressions to be popped
 */
void Parser::pop_partial_expr_nodes(
        std::vector<std::unique_ptr<AstNode>> &ret, std::vector<std::unique_ptr<AstNode>> &stack,
        const Token &end_stack_lid_node_token) {

    if (stack.size() == 0) {
        throw ExevalatorException("Unexpected end of a partial expression");
    }

    while (stack.size() != 0) {
        if (stack.back()->token.type == TokenType::STACK_LID) {
            std::unique_ptr<AstNode> stack_lid_node = std::move(stack.back());
            stack.pop_back();
            if (stack_lid_node->token.word == end_stack_lid_node_token.word) {
                break;
            }
        } else {
            ret.push_back(std::move(stack.back()));
            stack.pop_back();
        }
    }
}

/**
 * Returns a vectir storing next operator's precedence for each token.
 * In the returned array, it will stored at [i] that
 * precedence of the first operator of which token-index is greater than i.
 *
 * @param tokens All tokens to be parsed
 * @return The vector storing next operator's precedence for each token
 */
std::vector<uint64_t> Parser::get_next_operator_precedences(const std::vector<Token> &tokens) {
    size_t token_count = tokens.size();
    uint64_t last_op_precedence = std::numeric_limits<uint64_t>::max();
    std::vector<uint64_t> next_op_precedences;
    next_op_precedences.resize(token_count);

    for (size_t iloop=0; iloop<token_count; ++iloop) {
        size_t itoken = token_count - iloop - 1;
        Token token = tokens[itoken];
        next_op_precedences[itoken] = last_op_precedence;

        if (token.type == TokenType::OPERATOR) {
            last_op_precedence = token.opinfo.precedence;
        }

        if (token.type == TokenType::PARENTHESIS) {
            if (token.word == "(") {
                last_op_precedence = 0; // most prior
            } else {
                last_op_precedence = std::numeric_limits<uint64_t>::max(); // least prior
            }
        }
    }
    return next_op_precedences;
}


/**
 * Expresses the AST under this node in XML-like text format.
 *
 * @param indent_stage The stage of indent of this node
 * @return XML-like text representation of the AST under this node
 */
std::string AstNode::to_markupped_text(uint64_t indent_stage) {
    std::string result;
    const char *end_of_line = "\n";
    const char *indent_unit = "  ";

    std::string indent;
    for (uint64_t iindent=0; iindent<indent_stage; ++iindent) {
        indent.append(indent_unit);
    }

    result.append(indent);

    result.append("<");
    result.append(token_type_name(this->token.type));
    result.append(" word=\"");
    result.append(this->token.word);
    result.append("\"");

    if (this->token.type == TokenType::OPERATOR) {
        OperatorInfo opinfo = this->token.opinfo;
        result.append(" optype=\"");
        result.append(operator_type_name(opinfo.type));
        result.append("\" precedence=\"");
        result.append(std::to_string(opinfo.precedence));
        result.append("\"");
    }

    if (0 < this->child_nodes.size()) {
        result.append(">");
        for (std::unique_ptr<AstNode> &child_node: this->child_nodes) {
            result.append(end_of_line);
            result.append(child_node->to_markupped_text(indent_stage + 1));
       }
        result.append(end_of_line);
        result.append(indent);
        result.append("</");
        result.append(token_type_name(this->token.type));
        result.append(">");

    } else {
        result.append(" />");
    }
    return result;
}

/**
 * Checks that depths in the AST of all nodes under this node (child nodes, grandchild nodes, and so on)
 * does not exceeds the specified maximum value.
 * An ExevalatorException will be thrown when the depth exceeds the maximum value.
 * If the depth does not exceeds the maximum value, nothing will occur.
 *
 * @param depth_of_this_node The depth of this node in the AST.
 * @param max_ast_depth The maximum value of the depth of the AST.
 */
void AstNode::check_depth(uint64_t depth_of_this_node, uint64_t max_ast_depth) const {
    if (max_ast_depth < depth_of_this_node) {
        throw new ExevalatorException(
            std::string { "The depth of the AST exceeds the limit: Settings.max_ast_depth: " }
            + std::to_string(max_ast_depth)
        );
    }
    for (const std::unique_ptr<AstNode> &child_node: this->child_nodes) {
        child_node->check_depth(depth_of_this_node + 1, max_ast_depth);
    }
}


/*
 * Updates the state to evaluate the value of the AST.
 *
 * @param settings The Setting instance storing setting values
 * @param ast The root node of the AST
 * @param variable_table The Map mapping each variable name to an address of the variable
 * @param function_table The Map mapping each function name to an IExevalatorFunctionInterface instance
 */
void Evaluator::update(
            const Settings &settings,
            const std::unique_ptr<AstNode> &ast,
            const std::map<std::string, size_t> &variable_table,
            const std::map<std::string, std::shared_ptr<ExevalatorFunctionInterface>> &function_table) {

    this->evaluator_node_tree = Evaluator::create_evaluator_node_tree(
        settings, ast, variable_table, function_table
    );
}

/**
 * Returns whether "evaluate" method is available on the current state.
 *
 * @return true if "evaluate" method is available.
 */
bool Evaluator::is_evaluatable() {
    return (bool)this->evaluator_node_tree; // if evaluator_node_tree is a null pointer, returns false
}

/**
 * Evaluates the value of the AST set by "update" method.
 *
 * @param memory The Vec used as as a virtual memory storing values of variables.
 * @return The evaluated value.
 */
double Evaluator::evaluate(const std::vector<double> &memory) {
    return this->evaluator_node_tree->evaluate(memory);
}

/**
 * Creates a tree of evaluator nodes corresponding with the specified AST.
 *
 * @param settings The Setting instance storing setting values
 * @param ast The root node of the AST
 * @param variable_table The Map mapping each variable name to an address of the variable
 * @param function_table The Map mapping each function name to an IExevalatorFunctionInterface instance
 * @return The root node of the created tree of evaluator nodes.
 */
std::unique_ptr<Evaluator::EvaluatorNode> Evaluator::create_evaluator_node_tree(
            const Settings &settings,
            const std::unique_ptr<AstNode> &ast,
            const std::map<std::string, size_t> &variable_table,
            const std::map<std::string, std::shared_ptr<ExevalatorFunctionInterface>> &function_table) {

    // Note: This method creates a tree of evaluator nodes by traversing each node in the AST recursively.

    // Create an node for evaluating number literal.
    if (ast->token.type == TokenType::NUMBER_LITERAL) {
        double literal_value;
        try {
            literal_value = stod(ast->token.word);
        } catch (...) {
            std::string error_message = std::string { "Invalid number literal: " } + ast->token.word;
            throw ExevalatorException { error_message.c_str() };
        }
        return std::make_unique<Evaluator::NumberLiteralEvaluatorNode>(literal_value);

    // Create nodes for evaluating operators.
    } else if (ast->token.type == TokenType::OPERATOR) {

        // Unary operator "-"
        if (ast->token.opinfo.type == OperatorType::UNARY_PREFIX && ast->token.word == "-") {
            return std::make_unique<Evaluator::MinusEvaluatorNode>(
                create_evaluator_node_tree(settings, ast->child_nodes[0], variable_table, function_table)
            );

        // Binary operator "+"
        } else if (ast->token.opinfo.type == OperatorType::BINARY && ast->token.word == "+") {
            return std::make_unique<Evaluator::AdditionEvaluatorNode>(
                create_evaluator_node_tree(settings, ast->child_nodes[0], variable_table, function_table),
                create_evaluator_node_tree(settings, ast->child_nodes[1], variable_table, function_table)
            );

        // Binary operator "-"
        } else if (ast->token.opinfo.type == OperatorType::BINARY && ast->token.word == "-") {
            return std::make_unique<Evaluator::SubtractionEvaluatorNode>(
                create_evaluator_node_tree(settings, ast->child_nodes[0], variable_table, function_table),
                create_evaluator_node_tree(settings, ast->child_nodes[1], variable_table, function_table)
            );

        // Binary operator "*"
        } else if (ast->token.opinfo.type == OperatorType::BINARY && ast->token.word == "*") {
            return std::make_unique<Evaluator::MultiplicationEvaluatorNode>(
                create_evaluator_node_tree(settings, ast->child_nodes[0], variable_table, function_table),
                create_evaluator_node_tree(settings, ast->child_nodes[1], variable_table, function_table)
            );

        // Binary operator "/"
        } else if (ast->token.opinfo.type == OperatorType::BINARY && ast->token.word == "/") {
            return std::make_unique<Evaluator::DivisionEvaluatorNode>(
                create_evaluator_node_tree(settings, ast->child_nodes[0], variable_table, function_table),
                create_evaluator_node_tree(settings, ast->child_nodes[1], variable_table, function_table)
            );

        // Function call operator "("
        } else if (ast->token.opinfo.type == OperatorType::CALL && ast->token.word == "(") {
            size_t child_count = ast->child_nodes.size();
            std::string identifier = ast->child_nodes[0]->token.word;
            if (function_table.find(identifier) == function_table.end()) {
                throw ExevalatorException(("Function not found: " + identifier).c_str());
            }
            std::shared_ptr<ExevalatorFunctionInterface> function_ptr = function_table.at(identifier);
            std::vector<std::unique_ptr<Evaluator::EvaluatorNode>> arguments;
            for (size_t ichild=1; ichild<child_count; ++ichild) {
                arguments.push_back(
                    create_evaluator_node_tree(settings, ast->child_nodes[ichild], variable_table, function_table)
                );
            }
            return std::make_unique<Evaluator::FunctionEvaluatorNode>(
                function_ptr, identifier, arguments
            );
        } else {
            std::string error_message = std::string { "Unexpected operator: " } + ast->token.opinfo.symbol;
            throw ExevalatorException { error_message.c_str() };
        }

    // Create an node for evaluating the value of a variable.
    } else if (ast->token.type == TokenType::VARIABLE_IDENTIFIER) {
        std::string identifier = ast->token.word;
        if (variable_table.find(identifier) == variable_table.end()) {
            throw ExevalatorException(("Variable not found: " + identifier).c_str());
        }
        size_t address = variable_table.at(identifier);
        return std::make_unique<Evaluator::VariableEvaluatorNode>(address);

    } else {
        throw ExevalatorException(std::string("Unexpected token type: ").append(token_type_name(ast->token.type)).c_str());
    }
}

/**
 * The implementation of the virtual destructor of the super class of evaluator nodes.
 */
Evaluator::EvaluatorNode::~EvaluatorNode() {
}

/**
 * Creates the evaluator node for evaluating the value of a number literal.
 */
Evaluator::NumberLiteralEvaluatorNode::NumberLiteralEvaluatorNode(double literal_value) {
    this->literal_value = literal_value;
}

/**
 * Initializes the value of the number literal.
 *
 * @param literal The number literal
 */
double Evaluator::NumberLiteralEvaluatorNode::evaluate(const std::vector<double> &memory) {
    return this->literal_value;
}

/**
 * Creates the evaluator node for evaluating the value of a unary-minus operator.
 *
 * @param operand The node for evaluating the operand
 */
Evaluator::MinusEvaluatorNode::MinusEvaluatorNode(std::unique_ptr<EvaluatorNode> operand) {
    this->operand = std::move(operand);
}

/**
 * Performs the unary-minus operation.
 *
 * @param memory The array storing values of variables
 * @return The result value of the unary-minus operation
 */
double Evaluator::MinusEvaluatorNode::evaluate(const std::vector<double> &memory) {
    return -this->operand->evaluate(memory);
}

/**
 * Creates the evaluator node for evaluating the value of a addition operator.
 *
 * @param left_operand The node for evaluating the left-side operand
 * @param right_operand The node for evaluating the right-side operand
 */
Evaluator::AdditionEvaluatorNode::AdditionEvaluatorNode(std::unique_ptr<EvaluatorNode> left_operand, std::unique_ptr<EvaluatorNode> right_operand) {
    this->left_operand = std::move(left_operand);
    this->right_operand = std::move(right_operand);
}

/**
 * Performs the addition.
 *
 * @param memory The array storing values of variables
 * @return The result value of the addition
 */
double Evaluator::AdditionEvaluatorNode::evaluate(const std::vector<double> &memory) {
    return this->left_operand->evaluate(memory) + this->right_operand->evaluate(memory);
}

/**
 * Creates the evaluator node for evaluating the value of a subtraction operator.
 *
 * @param left_operand The node for evaluating the left-side operand
 * @param right_operand The node for evaluating the right-side operand
 */
Evaluator::SubtractionEvaluatorNode::SubtractionEvaluatorNode(std::unique_ptr<EvaluatorNode> left_operand, std::unique_ptr<EvaluatorNode> right_operand) {
    this->left_operand = std::move(left_operand);
    this->right_operand = std::move(right_operand);
}

/**
 * Performs the subtraction.
 *
 * @param memory The array storing values of variables
 * @return The result value of the subtraction
 */
double Evaluator::SubtractionEvaluatorNode::evaluate(const std::vector<double> &memory) {
    return this->left_operand->evaluate(memory) - this->right_operand->evaluate(memory);
}

/**
 * Creates the evaluator node for evaluating the value of a multiplication operator.
 *
 * @param left_operand The node for evaluating the left-side operand
 * @param right_operand The node for evaluating the right-side operand
 */
Evaluator::MultiplicationEvaluatorNode::MultiplicationEvaluatorNode(std::unique_ptr<EvaluatorNode> left_operand, std::unique_ptr<EvaluatorNode> right_operand) {
    this->left_operand = std::move(left_operand);
    this->right_operand = std::move(right_operand);
}

/**
 * Performs the multiplication.
 *
 * @param memory The array storing values of variables
 * @return The result value of the multiplication
 */
double Evaluator::MultiplicationEvaluatorNode::evaluate(const std::vector<double> &memory) {
    return this->left_operand->evaluate(memory) * this->right_operand->evaluate(memory);
}

/**
 * Creates the evaluator node for evaluating the value of a division operator.
 *
 * @param left_operand The node for evaluating the left-side operand
 * @param right_operand The node for evaluating the right-side operand
 */
Evaluator::DivisionEvaluatorNode::DivisionEvaluatorNode(std::unique_ptr<EvaluatorNode> left_operand, std::unique_ptr<EvaluatorNode> right_operand) {
    this->left_operand = std::move(left_operand);
    this->right_operand = std::move(right_operand);
}

/**
 * Performs the division.
 *
 * @param memory The array storing values of variables
 * @return The result value of the division
 */
double Evaluator::DivisionEvaluatorNode::evaluate(const std::vector<double> &memory) {
    return this->left_operand->evaluate(memory) / this->right_operand->evaluate(memory);
}

/**
 * Creates the evaluator node for evaluating the value of a variable.
 * 
 * @param address The virtual address of the variable
 */
Evaluator::VariableEvaluatorNode::VariableEvaluatorNode(size_t address) {
    this->address = address;
}

/**
 * Returns the value of the variable.
 *
 * @param memory The array storing values of variables
 * @return The value of the variable
 */
double Evaluator::VariableEvaluatorNode::evaluate(const std::vector<double> &memory) {
    return memory[this->address];
}

/**
 * Creates the evaluator node for evaluating a function-call operator.
 *
 * @param function_ptr The function to be called
 * @param identifier The name of the function
 * @param arguments Evaluator nodes for evaluating values of arguments
 */
Evaluator::FunctionEvaluatorNode::FunctionEvaluatorNode(
        std::shared_ptr<ExevalatorFunctionInterface> &function_ptr,
        std::string identifier,
        std::vector<std::unique_ptr<EvaluatorNode>> &arguments) {

    this->function_ptr = function_ptr;
    this->identifier = identifier;
    for (std::unique_ptr<EvaluatorNode> &argument: arguments) {
        this->arguments.push_back(std::move(argument));
    }
}

/**
 * Releases resources of the evaluator node for evaluating a function-call operator.
 */
Evaluator::FunctionEvaluatorNode::~FunctionEvaluatorNode() {
    this->identifier.clear();
    this->identifier.shrink_to_fit();
    this->arguments.clear();
    this->arguments.shrink_to_fit();
}

/**
 * Calls the function and returns the returned value of the function.
 *
 * @param memory The array storing values of variables
 * @return The returned value of the function
 */
double Evaluator::FunctionEvaluatorNode::evaluate(const std::vector<double> &memory) {
    size_t arg_count = this->arguments.size();
    std::vector<double> arg_evaluated_values;
    arg_evaluated_values.resize(this->arguments.size());
    for (size_t iarg=0; iarg<arg_count; ++iarg) {
        arg_evaluated_values[iarg] = this->arguments[iarg]->evaluate(memory);
    }
    try {
        double return_value = (*this->function_ptr)(arg_evaluated_values);
        return return_value;
    } catch (...) {
        std::string error_message = std::string { "Function error: "} + this->identifier;
        throw ExevalatorException { error_message.c_str() };
    }
}


} // namespace exevalator

#endif
