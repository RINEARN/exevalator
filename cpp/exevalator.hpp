/*
 * Exevalator Ver.1.0.1 - by RINEARN 2021-2022
 * This software is released under the CC0.
 */

#ifndef EXEVALATOR_HPP
#define EXEVALATOR_HPP

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

namespace exevalator {

/**
 * The Exception class thrown in/by this interpreter engine.
 */
class ExevalatorException: public std::exception {

    /** Stores the error message of this exception. */
    std::string error_message;

public:

    /**
     * Creates an new Exception having the specified error message.
     * 
     * @param error_message The error message
     */
    ExevalatorException(std::string error_message);
    ~ExevalatorException();
    const char* what() const noexcept;
};


/**
 * The interface class of functions available in expressions.
 */
class ExevalatorFunctionInterface {
public:

    /**
     * Invokes the function.
     * 
     * @param arguments The vector storing values of arguments
     * @return The return value of the function
     */
    virtual double operator()(const std::vector<double> &arguments) = 0;
    virtual ~ExevalatorFunctionInterface() = 0;
};


/**
 * The enum representing types of operators.
 */
enum OperatorType {

    /** Represents unary operator, for example: - of -1.23. */
    UNARY_PREFIX,

    /** Represents binary operator, for example: + of 1+2. */
    BINARY,

    /** Represents function-call operator. */
    CALL,
};

/**
 * The struct storing information of an oprator.
 */
struct OperatorInfo {

    /** The type of operator. */
    OperatorType type;

    /** The symbol of this operator (for example: '+'). */
    char symbol;

    /** The precedence of this operator (smaller value gives higher precedence). */
    uint64_t precedence;
};

/**
 * Converts a OperatorType enum to its name.
 * 
 * @param type A OperatorType enum element to be converted.
 * @return The name of the spacified enum element.
 */
static std::string operator_type_name(OperatorType type) {
    switch (type) {
        case OperatorType::UNARY_PREFIX: return std::string { "UNARY_PREFIX" };
        case OperatorType::BINARY:       return std::string { "BINARY" };
        case OperatorType::CALL:         return std::string { "CALL" };
        default: return std::string { "UNKNOWN" };
    }
}


/**
 * The enum representing types of tokens.
 */
enum TokenType {

    /** Represents number literal tokens, for example: 1.23. */
    NUMBER_LITERAL,

    /** Represents operator tokens, for example: +. */
    OPERATOR,

    /** Represents separator tokens of partial expressions: ,. */
    EXPRESSION_SEPARATOR,

    /** Represents parenthesis, for example: ( and ) of (1*(2+3)). */
    PARENTHESIS,

    /** Represents variable-identifier tokens, for example: x. */
    VARIABLE_IDENTIFIER,

    /** Represents function-identifier tokens, for example: f. */
    FUNCTION_IDENTIFIER,

    /** Represents temporary token for isolating partial expressions in the stack, in parser. */
    STACK_LID,
};


/**
 * The struct storing information of a token.
 */
struct Token {
    
    /** The type of this token. */
    TokenType type;

    /** The text representation of this token. */
    std::string word;

    /** The detailed information of the operator, if the type of this token is OPERATOR. */
    OperatorInfo opinfo;
};

/**
 * Converts a TokenType enum to its name.
 * 
 * @param type A TokenType enum element to be converted.
 * @return The name of the spacified enum element.
 */
static std::string token_type_name(TokenType type) {
    switch (type) {
        case TokenType::NUMBER_LITERAL:       return std::string { "NUMBER_LITERAL" };
        case TokenType::OPERATOR:             return std::string { "OPERATOR" };
        case TokenType::EXPRESSION_SEPARATOR: return std::string { "EXPRESSION_SEPARATOR" };
        case TokenType::PARENTHESIS:          return std::string { "PARENTHESIS" };
        case TokenType::VARIABLE_IDENTIFIER:  return std::string { "VARIABLE_IDENTIFIER" };
        case TokenType::FUNCTION_IDENTIFIER:  return std::string { "FUNCTION_IDENTIFIER" };
        case TokenType::STACK_LID:            return std::string { "STACK_LID" };
        default: return std::string { "UNKNOWN" };
    }
}


/**
 * The struct for storing values of setting items.
 */
struct Settings {

    /** The maximum number of characters in an expression. */
    size_t max_expression_char_count;

    /** The maximum number of characters of variable/function names. */
    size_t max_name_char_count;

    /** The maximum number of tokens in an expression. */
    size_t max_token_count;

    /** The maximum depth of an Abstract Syntax Tree (AST). */
    uint64_t max_ast_depth;

    /** Set storing all symbols of operators. */
    std::set<char> operator_symbol_set;

    /** Map mapping each symbol of a binary operator to OperatorInfo instance. */
    std::map<char, OperatorInfo> binary_symbol_operator_map;

    /** Map mapping each symbol of a unary-prefix operator to OperatorInfo instance. */
    std::map<char, OperatorInfo> unary_prefix_symbol_operator_map;
    
    /** Map mapping each symbol of a call operator to OperatorInfo instance. */
    std::map<char, OperatorInfo> call_symbol_operator_map;
    
    /** Vector storing characters at which an expression will be splitted into tokens. */
    std::vector<char> token_splitter_character_list;

    /** Set storing characters at which an expression will be splitted into tokens. */
    std::set<char> token_splitter_character_set;

    /** Escaped representation of number literals, used in lexical analysis. */
    std::string escaped_number_literal;

    /**
     * Creates a new instance storing default setting values.
     */
    Settings() {
        max_expression_char_count = 256;
        max_name_char_count = 64;
        max_token_count = 64;
        max_ast_depth = 32;
        OperatorInfo addition_operator_info       = OperatorInfo { OperatorType::BINARY,       '+', 400 };
        OperatorInfo subtraction_operator_info    = OperatorInfo { OperatorType::BINARY,       '-', 400 };
        OperatorInfo multiplication_operator_info = OperatorInfo { OperatorType::BINARY,       '*', 300 };
        OperatorInfo division_operator_info       = OperatorInfo { OperatorType::BINARY,       '/', 300 };
        OperatorInfo minus_operator_info          = OperatorInfo { OperatorType::UNARY_PREFIX, '-', 200 };
        OperatorInfo call_begin_operator_info     = OperatorInfo { OperatorType::CALL,         '(', 100 };
        OperatorInfo call_end_operator_info       = OperatorInfo { OperatorType::CALL,         ')', std::numeric_limits<uint64_t>::max() };
        this->binary_symbol_operator_map['+']       = addition_operator_info;
        this->binary_symbol_operator_map['-']       = subtraction_operator_info;
        this->binary_symbol_operator_map['*']       = multiplication_operator_info;
        this->binary_symbol_operator_map['/']       = division_operator_info;
        this->unary_prefix_symbol_operator_map['-'] = minus_operator_info;
        this->call_symbol_operator_map['(']         = call_begin_operator_info;
        this->call_symbol_operator_map[')']         = call_end_operator_info;
        this->operator_symbol_set.insert('+');
        this->operator_symbol_set.insert('-');
        this->operator_symbol_set.insert('*');
        this->operator_symbol_set.insert('/');
        this->operator_symbol_set.insert('-');
        this->operator_symbol_set.insert('(');
        this->operator_symbol_set.insert(')');
        this->token_splitter_character_list.push_back('+');
        this->token_splitter_character_list.push_back('-');
        this->token_splitter_character_list.push_back('*');
        this->token_splitter_character_list.push_back('/');
        this->token_splitter_character_list.push_back('(');
        this->token_splitter_character_list.push_back(')');
        this->token_splitter_character_list.push_back(',');
        this->token_splitter_character_set.insert('+');
        this->token_splitter_character_set.insert('-');
        this->token_splitter_character_set.insert('*');
        this->token_splitter_character_set.insert('/');
        this->token_splitter_character_set.insert('(');
        this->token_splitter_character_set.insert(')');
        this->token_splitter_character_set.insert(',');
        this->escaped_number_literal = std::string { "@NUMBER_LITERAL@" };
    }

    /**
     * Release members storing setting values.
     */
    ~Settings() {
        this->binary_symbol_operator_map.clear();
        std::map<char, OperatorInfo>{}.swap(this->binary_symbol_operator_map);

        this->unary_prefix_symbol_operator_map.clear();
        std::map<char, OperatorInfo>{}.swap(this->unary_prefix_symbol_operator_map);

        this->call_symbol_operator_map.clear();
        std::map<char, OperatorInfo>{}.swap(this->call_symbol_operator_map);

        this->operator_symbol_set.clear();
        std::set<char>{}.swap(this->operator_symbol_set);

        this->token_splitter_character_list.clear();
        this->token_splitter_character_list.shrink_to_fit();

        this->token_splitter_character_set.clear();
        std::set<char>{}.swap(this->token_splitter_character_set);

        this->escaped_number_literal.clear();
        this->escaped_number_literal.shrink_to_fit();
    }
};


/**
 * The class providing various types of evaluator units
 * which evaluate values of operators, literals, etc.
 */
class Evaluator {
public:

    /**
     * The super class of evaluator units.
     */
    class EvaluatorUnit {
    public:

        /**
         * Performs the evaluation.
         *
         * @param memory The array storing values of variables
         * @return The evaluated value
         */
        virtual double evaluate(const std::vector<double> &memory) = 0;

        // The destructor of this class should be virtual.
        virtual ~EvaluatorUnit() = 0;
    };

    /**
     * The evaluator unit for evaluating the value of a number literal.
     */
    class NumberLiteralEvaluatorUnit: public EvaluatorUnit {
        
        /** The value of the number literal. */
        double literal_value;

    public:

        /**
         * Creates the evaluator unit for evaluating the value of a number literal.
         */
        NumberLiteralEvaluatorUnit(double literal_value);

        /**
         * Initializes the value of the number literal.
         *
         * @param literal The number literal
         */
        double evaluate(const std::vector<double> &memory);
    };


    /**
     * The evaluator unit for evaluating the value of a unary-minus operator.
     */
    class MinusEvaluatorUnit: public EvaluatorUnit {

        /** The unit for evaluating the operand. */
        std::unique_ptr<EvaluatorUnit> operand;

    public:
    
        /**
         * Creates the evaluator unit for evaluating the value of a unary-minus operator.
         *
         * @param operand The unit for evaluating the operand
         */
        MinusEvaluatorUnit(std::unique_ptr<EvaluatorUnit> operand);

        /**
         * Performs the unary-minus operation.
         *
         * @param memory The array storing values of variables
         * @return The result value of the unary-minus operation
         */
        double evaluate(const std::vector<double> &memory);
    };


    class AdditionEvaluatorUnit: public EvaluatorUnit {

        /** The unit for evaluating the left-side operand. */
        std::unique_ptr<EvaluatorUnit> left_operand;

        /** The unit for evaluating the right-side operand. */
        std::unique_ptr<EvaluatorUnit> right_operand;

    public:
    
        /**
         * Creates the evaluator unit for evaluating the value of a addition operator.
         *
         * @param left_operand The unit for evaluating the left-side operand
         * @param right_operand The unit for evaluating the right-side operand
         */
        AdditionEvaluatorUnit(std::unique_ptr<EvaluatorUnit> left_operand, std::unique_ptr<EvaluatorUnit> right_operand);

        /**
         * Performs the addition.
         *
         * @param memory The array storing values of variables
         * @return The result value of the addition
         */
        double evaluate(const std::vector<double> &memory);
    };

    class SubtractionEvaluatorUnit: public EvaluatorUnit {

        /** The unit for evaluating the left-side operand. */
        std::unique_ptr<EvaluatorUnit> left_operand;

        /** The unit for evaluating the right-side operand. */
        std::unique_ptr<EvaluatorUnit> right_operand;

    public:
    
        /**
         * Creates the evaluator unit for evaluating the value of a subtraction operator.
         *
         * @param left_operand The unit for evaluating the left-side operand
         * @param right_operand The unit for evaluating the right-side operand
         */
        SubtractionEvaluatorUnit(std::unique_ptr<EvaluatorUnit> left_operand, std::unique_ptr<EvaluatorUnit> right_operand);
        
        /**
         * Performs the subtraction.
         *
         * @param memory The array storing values of variables
         * @return The result value of the subtraction
         */
        double evaluate(const std::vector<double> &memory);
    };

    class MultiplicationEvaluatorUnit: public EvaluatorUnit {

        /** The unit for evaluating the left-side operand. */
        std::unique_ptr<EvaluatorUnit> left_operand;

        /** The unit for evaluating the right-side operand. */
        std::unique_ptr<EvaluatorUnit> right_operand;

    public:
    
        /**
         * Creates the evaluator unit for evaluating the value of a multiplication operator.
         *
         * @param left_operand The unit for evaluating the left-side operand
         * @param right_operand The unit for evaluating the right-side operand
         */
        MultiplicationEvaluatorUnit(std::unique_ptr<EvaluatorUnit> left_operand, std::unique_ptr<EvaluatorUnit> right_operand);

        /**
         * Performs the multiplication.
         *
         * @param memory The array storing values of variables
         * @return The result value of the multiplication
         */
        double evaluate(const std::vector<double> &memory);
    };

    class DivisionEvaluatorUnit: public EvaluatorUnit {

        /** The unit for evaluating the left-side operand. */
        std::unique_ptr<EvaluatorUnit> left_operand;

        /** The unit for evaluating the right-side operand. */
        std::unique_ptr<EvaluatorUnit> right_operand;

    public:
    
        /**
         * Creates the evaluator unit for evaluating the value of a division operator.
         *
         * @param left_operand The unit for evaluating the left-side operand
         * @param right_operand The unit for evaluating the right-side operand
         */
        DivisionEvaluatorUnit(std::unique_ptr<EvaluatorUnit> left_operand, std::unique_ptr<EvaluatorUnit> right_operand);

        /**
         * Performs the division.
         *
         * @param memory The array storing values of variables
         * @return The result value of the division
         */
        double evaluate(const std::vector<double> &memory);
    };

    /**
     * The evaluator unit for evaluating the value of a variable.
     */
    class VariableEvaluatorUnit: public EvaluatorUnit {

        /** The address of the variable. */
        size_t address;

    public:

        /**
         * Creates the evaluator unit for evaluating the value of a variable.
         * 
         * @param address The virtual address of the variable
         */
        VariableEvaluatorUnit(size_t address);

        /**
         * Returns the value of the variable.
         *
         * @param memory The array storing values of variables
         * @return The value of the variable
         */
        double evaluate(const std::vector<double> &memory);
    };

    /**
     * The evaluator unit for evaluating a function-call operator.
     */
    class FunctionEvaluatorUnit: public EvaluatorUnit {

        /** The function to be called. */
        std::shared_ptr<ExevalatorFunctionInterface> function_ptr;

        /** The name of the function. */
        std::string identifier;

        /** Evaluator units for evaluating values of arguments. */
        std::vector<std::unique_ptr<EvaluatorUnit>> arguments;

    public:

        /**
         * Creates the evaluator unit for evaluating a function-call operator.
         *
         * @param function_ptr The function to be called
         * @param identifier The name of the function
         * @param arguments Evaluator units for evaluating values of arguments
         */
        FunctionEvaluatorUnit(
            std::shared_ptr<ExevalatorFunctionInterface> &function_ptr,
            std::string identifier,
            std::vector<std::unique_ptr<EvaluatorUnit>> &arguments
        );

        /**
         * Releases resources of the evaluator unit for evaluating a function-call operator.
         */
        ~FunctionEvaluatorUnit();

        /**
         * Calls the function and returns the returned value of the function.
         *
         * @param memory The array storing values of variables
         * @return The returned value of the function
         */
        double evaluate(const std::vector<double> &memory);
    };
};


/**
 * The class storing information of an node of an AST.
 */
class AstNode {
public:

    /** The token corresponding with this AST node. */
    Token token;

    /**  The list of child nodes of this AST node. */
    std::vector<std::unique_ptr<AstNode>> child_nodes;
    
    /**
     * Creates an AST node instance corresponding with the specified token.
     * 
     * @param token The token corresponding with the AST to be created.
     */
    AstNode(Token token) {
        this->token = token;
    }

    /**
     * Expresses the AST under this node in XML-like text format.
     *
     * @param indent_stage The stage of indent of this node
     * @return XML-like text representation of the AST under this node
     */
    std::string to_markupped_text(uint64_t indent_stage);

    /**
     * Creates the evaluator unit for evaluating the value of this AST node.
     *
     * @param settings The Setting instance storing setting values
     * @param variable_table The Map mapping each variable name to an address of the variable
     * @param function_table The Map mapping each function name to an IExevalatorFunctionInterface instance
     * @return The created evaluator unit
     */
    std::unique_ptr<Evaluator::EvaluatorUnit> create_evaluator_unit(const Settings &settings, 
            const std::map<std::string, size_t> &variable_table,
            const std::map<std::string, std::shared_ptr<ExevalatorFunctionInterface>> &function_table);

    /**
     * Checks that depths in the AST of all nodes under this node (child nodes, grandchild nodes, and so on)
     * does not exceeds the specified maximum value.
     * An ExevalatorException will be thrown when the depth exceeds the maximum value.
     * If the depth does not exceeds the maximum value, nothing will occur.
     *
     * @param depth_of_this_node The depth of this node in the AST
     * @param max_ast_depth The maximum value of the depth of the AST
     */
    void check_depth(uint64_t depth_of_this_node, uint64_t max_ast_depth) const;
};


/**
 * The class performing functions of a lexical analyzer.
 */
class LexicalAnalyzer {

    /**
     * Detects the end of the specified number literal in the expression.
     * 
     * @param expression The expression containing the number literal
     * @param literal_begin The index of the character at the beginning of the number literal
     * @return The index of the character at the end of the number literal
     */
    static size_t detect_end_of_num_literal(const std::string &expression, size_t literal_begin);

    /**
     *  Extracts all number literals in the expression, and replaces them to the escaped representation.
     * 
     * @param expression The expression to be processed
     * @param literal_store The vector to which extracted number literals will be stored
     * @param settings The Setting instance storing setting values
     * @return The expression in which all number literals are escaped
     */
    static std::string escape_number_literals(
        const std::string &expression, std::vector<std::string> &literal_store, const Settings &settings
    );

    /**
     * Sprits (tokenizes) the expression into token-words. 
     * The type of each word is "string", not "Token" struct yet, at this stage.
     * 
     * @param expression The expression to be splitted (tokenized) into token-words
     * @param settings The Setting instance storing setting values
     * @return Token-words
     */
    static std::vector<std::string> split_expression_into_token_words(const std::string &expression, const Settings &settings);

    /**
     * Creates "Token" type instances from "string" type token-words.
     * Also, escaped number literals will be recovered.
     * 
     * @param token_words Token-words to be converted to `Token` type instances
     * @param number_literals Values of number literals to be recovered
     * @param settings settings The Setting instance storing setting values
     * @return "Token" type instances
     */
    static std::vector<Token> create_tokens_from_token_words(
        const std::vector<std::string> &token_words, const std::vector<std::string> &number_literals, const Settings &settings
    );

    /**
     * Checks the number and correspondence of open "(" / closed ")" parentheses.
     * An ExevalatorException will be thrown when any errors detected.
     * If no error detected, nothing will occur.
     *
     * @param tokens Tokens of the inputted expression.
     */
    static void check_parenthesis_opening_closings(const std::vector<Token> &tokens);

    /**
     * Checks that empty parentheses "()" are not contained in the expression.
     * An ExevalatorException will be thrown when any errors detected.
     * If no error detected, nothing will occur.
     *
     * @param tokens Tokens of the inputted expression.
     */
    static void check_empty_parentheses(const std::vector<Token> &tokens);

    /**
     * Checks correctness of locations of operators and leaf elements (literals and identifiers).
     * An ExevalatorException will be thrown when any errors detected.
     * If no error detected, nothing will occur.
     *
     * @param tokens Tokens of the inputted expression.
     */
    static void check_locations_of_operators_and_leafs(const std::vector<Token> &tokens);

public:

    /**
     * Splits (tokenizes) the expression into tokens, and analyze them.
     *
     * @param expression The expression to be tokenized/analyzed
     * @return Analyzed tokens
     */
    static std::vector<Token> analyze(const std::string &expression, const Settings &settings);
};


/**
 * The class performing functions of a parser.
 */
class Parser {
    
    /**
     * Judges whether the right-side token should be connected directly as an operand, to the target operator.
     *
     * @param target_operator_precedence The precedence of the target operator (smaller value gives higher precedence)
     * @param next_operator_precedence The precedence of the next operator (smaller value gives higher precedence)
     * @return Returns true if the right-side token (operand) should be connected to the target operator
     */
    static constexpr bool should_add_right_operand(uint64_t target_operator_precedence, uint64_t next_operator_precedence);

    /**
     * Judges whether the right-side token should be connected directly as an operand,
     * to the operator at the top of the working stack.
     *
     * @param stack The working stack used for the parsing
     * @param next_operator_precedence The precedence of the next operator (smaller value gives higher precedence)
     * @return Returns true if the right-side token (operand) should be connected to the operator at the top of the stack
     */
    static bool should_add_right_operand_to_stacked_operator(
        const std::vector<std::unique_ptr<AstNode>> &stack, uint64_t next_operator_precedence
    );

    /**
     * Pops root nodes of ASTs of partial expressions constructed on the stack.
     *
     * @param ret Root nodes of ASTs of partial expressions
     * @param stack The working stack used for the parsing
     * @param end_stack_lid_node_token The token of the temporary node pushed in the stack,
     *                                 at the end of partial expressions to be popped
     */
    static void pop_partial_expr_nodes(
        std::vector<std::unique_ptr<AstNode>> &ret, std::vector<std::unique_ptr<AstNode>> &stack,
        const Token &end_stack_lid_node_token
    );

    /**
     * Returns a vectir storing next operator's precedence for each token.
     * In the returned array, it will stored at [i] that
     * precedence of the first operator of which token-index is greater than i.
     *
     * @param tokens All tokens to be parsed
     * @return The vector storing next operator's precedence for each token
     */
    static std::vector<uint64_t> get_next_operator_precedences(const std::vector<Token> &tokens);

public:

    /**
     * Parses tokens and construct Abstract Syntax Tree (AST).
     *
     * @param tokens Tokens to be parsed
     * @return The root node of the constructed AST
     */
    static std::unique_ptr<AstNode> parse(const std::vector<Token> &tokens);
};


/**
 * Interpreter Engine of Exevalator.
 */
class Exevalator {

    /** The Setting instance storing setting values. */
    Settings settings;
    
    /** The vector used as as a virtual memory storing values of variables. */
    std::vector<double> memory;
    
    /** The Map mapping each variable name to an address of the variable. */
    std::map<std::string, size_t> variable_table;
    
    /** The Map mapping each function name to an IExevalatorFunctionInterface instance. */
    std::map<std::string, std::shared_ptr<ExevalatorFunctionInterface>> function_table;
    
    /** The unit for evaluating an expression. */
    std::unique_ptr<Evaluator::EvaluatorUnit> evaluator_unit;
    
    /** Caches the content of the expression evaluated last time, to skip re-parsing. */
    std::string last_evaluated_expression;

public:

    /**
     * Creates a new interpreter engine of Exevalator.
     */
    Exevalator();

    /**
     * Releases resources of the interpreter engine of Exevalator.
     */
    ~Exevalator();

    /**
     * Evaluates (computes) the value of an expression.
     *
     * @param expression The expression to be evaluated
     * @return The evaluated value
     */
    double eval(const std::string &expression);

    /**
     * Re-evaluates (re-computes) the value of the expression evaluated by "eval" method last time.
     * This method works faster than calling "eval" method repeatedly for the same expression.
     * Note that, the result value may different with the last evaluated value, 
     * if values of variables or behaviour of functions had changed.
     * 
     * @return The evaluated value
     */
    double reeval();

    /**
     * Declares a new variable, for using the value of it in expressions.
     * 
     * @param name The name of the variable to be declared
     * @returns The virtual address of the declared variable,
     *          which useful for accessing to the variable faster.
     *          See "write_variable_at" and "read_variable_at" method.
     */
    size_t declare_variable(const std::string &name);

    /**
     * Writes the value to the variable having the specified name.
     *
     * @param name The name of the variable to be written
     * @param value The new value of the variable
     */
    void write_variable(const std::string &name, double value);

    /**
     * Writes the value to the variable at the specified virtual address.
     * This function works faster than "write_variable" function.
     * 
     * @param address The virtual address of the variable to be written
     * @param value The new value of the variable
     */
    void write_variable_at(size_t address, double variable);

    /**
     * Reads the value of the variable having the specified name.
     * 
     * @param name The name of the variable to be read
     * @return The current value of the variable
     */
    double read_variable(const std::string &name);

    /**
     * Reads the value of the variable at the specified virtual address.
     * This function works faster than "read_variable" function.
     * 
     * @param address The virtual address of the variable to be read
     * @return The current value of the variable
     */
    double read_variable_at(size_t address);

    /**
     * Connects a function, for using it in expressions.
     * 
     * @param name The name of the function used in the expression
     * @param function The function to be connected
     */
    void connect_function(const std::string &name, const std::shared_ptr<ExevalatorFunctionInterface> &function_ptr);
};

} // namespace exevalator

#endif
