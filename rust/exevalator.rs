/*
 * Exevalator Ver.2.2.0 - by RINEARN 2021-2024
 * This software is released under the "Unlicense" license.
 * You can choose the "CC0" license instead, if you want.
 */

use std::collections::HashSet;
use std::collections::HashMap;
use std::collections::VecDeque;
use std::fmt;


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


/// Interpreter Engine of Exevalator.
pub struct Exevalator<'exvlife> {

    /// The struct storing values of setting items.
    settings: Settings,

    /// The Vec used as as a virtual memory storing values of variables.
    memory: Vec<f64>,

    /// The object evaluating the value of the expression.
    evaluator: Evaluator,

    /// The HashMap mapping each name of a variable to an addresses on the virtual memory.
    variable_table: HashMap<String, usize>,

    /// The HashMap mapping each name of a function to a function pointer.
    function_table: HashMap<String, fn(Vec<f64>)->Result<f64,ExevalatorError> >,

    /// Caches the content of the expression evaluated last time, to skip re-parsing.
    last_evaluated_expression: &'exvlife str,
}

impl<'exvlife> Exevalator<'exvlife> {

    /// Creates a new interpreter of the Exevalator.
    ///
    /// * Return value - The created instance.
    ///
    #[allow(dead_code)]
    pub fn new() -> Self {
        Self {
            settings: Settings::new(),
            memory: Vec::new(),
            variable_table: HashMap::new(),
            function_table: HashMap::new(),
            evaluator: Evaluator::new(),
            last_evaluated_expression: "",
        }
    }
    
    /// Evaluates (computes) the value of an expression.
    /// 
    /// * `expression` - The expression to be evaluated.
    /// * Return value - The evaluated value, or ExevalatorError if any error detected.
    ///
    #[allow(dead_code)]
    pub fn eval(&mut self, expression: &'exvlife str) -> Result<f64,ExevalatorError> {
        if self.settings.max_expression_char_count < expression.len() {
            return Err(ExevalatorError::new(
                &ErrorMessages::TOO_LONG_EXPRESSION.replace("$0", &self.settings.max_expression_char_count.to_string())
            ));
        }

        // If the expression changed from the last-evaluated expression, re-parsing is necessary.
        if !expression.eq(self.last_evaluated_expression) || !self.evaluator.is_evaluatable() {

            // Perform lexical analysis, and get tokens from the inputted expression.
            let tokens: Vec<Token> = match LexicalAnalyzer::analyze( &(expression.to_string()), &self.settings) {
                Ok(lexer_output) => lexer_output,
                Err(lexer_error) => return Err(lexer_error),
            };

            // Perform parsing, and get AST(Abstract Syntax Tree) from tokens.
            let ast: AstNode = match Parser::parse(&tokens, &self.settings) {
                Ok(parser_output) => parser_output,
                Err(parser_error) => return Err(parser_error),
            };

            // Update the evaluator, to evaluate the parsed AST.
            match self.evaluator.update(&ast, &self.variable_table, &self.function_table, &self.settings) {
                Ok(created_node) => Some(created_node),
                Err(node_creation_error) => return Err(node_creation_error),
            };

            self.last_evaluated_expression = expression;
        }

        // Evaluate the value of the expression, and return it.
        match self.evaluator.evaluate(&self.memory) {
            Ok(evaluated_value) => return Ok(evaluated_value),
            Err(evaluation_error) => return Err(evaluation_error),
        };
    }

    /// Re-evaluates (re-computes) the value of the expression evaluated by "eval" method last time.
    /// This method is more efficient than calling "eval" method repeatedly for the same expression.
    /// Note that, the result value may differ from the last evaluated value, 
    /// if values of variables or behaviour of functions had changed.
    /// 
    /// * Return value - The evaluated value, or ExevalatorError if any error detected.
    ///
    #[allow(dead_code)]
    pub fn reeval(&mut self) -> Result<f64,ExevalatorError> {
        if self.evaluator.is_evaluatable() {
            match self.evaluator.evaluate(&self.memory) {
                Ok(evaluated_value) => return Ok(evaluated_value),
                Err(evaluation_error) => return Err(evaluation_error),
            };
        } else {
            return Err(ExevalatorError::new(ErrorMessages::REEVAL_NOT_AVAILABLE));
        }
    }

    /// Declares a new variable, for using the value of it in the expression.
    /// 
    /// * `name` - The name of the variable to be declared.
    /// * Return value - The virtual address of the declared variable,
    ///                  which useful for accessing to the variable faster.
    ///                  See 'write_variable_at' and 'read_variable_at' function.
    ///
    #[allow(dead_code)]
    pub fn declare_variable(&mut self, name: &str) -> Result<usize, ExevalatorError> {
        if self.settings.max_name_char_count < name.len() {
            return Err(ExevalatorError::new(
                &ErrorMessages::TOO_LONG_VARIABLE_NAME.replace("$0", &self.settings.max_name_char_count.to_string())
            ));
        }
        if self.variable_table.get(name) != None {
            return Err(ExevalatorError::new(&ErrorMessages::VARIABLE_ALREADY_DECLARED.replace("$0", name)));
        }
        let address: usize = self.memory.len();
        self.memory.push(0.0);
        self.variable_table.insert(name.to_string(), address);
        return Ok(address);
    }

    /// Writes the value to the variable having the specified name.
    ///
    /// * 'name' - The name of the variable to be written.
    /// * `value` - The new value of the variable.
    /// * Return value - Nothing, or ExevalatorError if any error detected.
    /// 
    #[allow(dead_code)]
    pub fn write_variable(&mut self, name: &str, value: f64) -> Result<(), ExevalatorError> {
        if self.settings.max_name_char_count < name.len() || !self.variable_table.contains_key(name) {
            return Err(ExevalatorError::new(&ErrorMessages::VARIABLE_NOT_FOUND.replace("$0", &name)));
        }
        let address: usize = *self.variable_table.get(name).unwrap();
        return self.write_variable_at(address, value);
    }

    /// Writes the value to the variable at the specified virtual address.
    /// This function is more efficient than `write_variable` function.
    ///
    /// * 'address' - The virtual address of the variable to be written.
    /// * `value` - The new value of the variable.
    /// * Return value - Nothing, or ExevalatorError if any error detected.
    /// 
    #[allow(dead_code)]
    pub fn write_variable_at(&mut self, address: usize, value: f64) -> Result<(), ExevalatorError> {
        if self.memory.len() <= address  {
            return Err(ExevalatorError::new(&ErrorMessages::INVALID_VARIABLE_ADDRESS.replace("$0", &address.to_string())));
        }
        self.memory[address] = value;
        return Ok(());
    }

    /// Reads the value of the variable having the specified name.
    ///
    /// * 'name' - The name of the variable to be read.
    /// * Return value - The current value of the variable.
    ///
    #[allow(dead_code)]
    pub fn read_variable(&self, name: &str) -> Result<f64, ExevalatorError> {
        if self.settings.max_name_char_count < name.len() || !self.variable_table.contains_key(name) {
            return Err(ExevalatorError::new(&ErrorMessages::VARIABLE_NOT_FOUND.replace("$0", name)));
        }
        let address: usize = *self.variable_table.get(name).unwrap();
        return self.read_variable_at(address);
    }

    /// Reads the value of the variable at the specified virtual address.
    /// This function is more efficient than `read_variable` function.
    ///
    /// * 'address' - The virtual address of the variable to be read.
    /// * Return value - The current value of the variable.
    ///
    #[allow(dead_code)]
    pub fn read_variable_at(&self, address: usize) -> Result<f64, ExevalatorError> {
        if self.memory.len() <= address  {
            return Err(ExevalatorError::new(&ErrorMessages::INVALID_VARIABLE_ADDRESS.replace("$0", &address.to_string())));
        }
        return Ok(self.memory[address]);
    }

    /// Connects a function, for using it in the expression.
    /// 
    /// * `name` - The name of the function used in the expression.
    /// * `function_pointer` - The pointer of the function to be connected.
    /// * 'address' - The virtual address of the function (useless on the current version, but might be used in future).
    ///
    #[allow(dead_code)]
    pub fn connect_function(&mut self, name: &str, function_pointer: fn(Vec<f64>)->Result<f64,ExevalatorError>)
            -> Result<usize, ExevalatorError> {
        if self.settings.max_name_char_count < name.len() {
            return Err(ExevalatorError::new(
                &ErrorMessages::TOO_LONG_FUNCTION_NAME.replace("$0", &self.settings.max_name_char_count.to_string())
            ));
        }
        if self.function_table.get(name) != None {
            return Err(ExevalatorError::new(&ErrorMessages::FUNCTION_ALREADY_CONNECTED.replace("$0", name)));
        }
        let address = self.function_table.len();
        self.function_table.insert(name.to_string(), function_pointer);
        return Ok(address);
    }
}


/// The error struct, for notifying errors which occurred in the exevalator.
#[derive(Debug)]
pub struct ExevalatorError {
    error_message: String,
}
impl ExevalatorError {

    /// Creates a new error struct having the specified error message.
    ///
    /// * `error_message` - The error message.
    ///
    pub fn new(error_message: &str) -> Self {
        Self {
            error_message: error_message.to_string(),
        }
    }
}
impl fmt::Display for ExevalatorError {

    /// Returns the formatted error message to be displayed.
    fn fmt(&self, f: &mut fmt::Formatter) -> fmt::Result {
        return write!(f, "{}", &self.error_message);
    }
}


/// The object performing functions of a lexical analyzer.
struct LexicalAnalyzer {
}

impl LexicalAnalyzer {

    /// Splits (tokenizes) the expression into tokens, and analyze them.
    ///
    /// * `expression` - The expression to be tokenized/analyzed.
    /// * `settings` - The struct storing values of setting items.
    /// * Return value - Analyzed tokens.
    ///
    fn analyze(expression: &str, settings: &Settings) -> Result<Vec<Token>,ExevalatorError> {

        // Replaces number literals in the espression to the escaped representation,
        // because they may contain same chars with operators, e.g.: '+', '-'.
        // Detected number literals will be stored into the Vec `num_literals`.
        let mut num_literals: Vec<String> = Vec::new();
        let escaped_expression: String = LexicalAnalyzer::escape_number_literals(
            &expression, &mut num_literals, settings
        );
        
        // Split the expression into token words.
        let token_words: Vec<String> = LexicalAnalyzer::split_expression_into_token_words(
            &escaped_expression, settings
        );

        // Checks the total number of tokens.
        if token_words.len() == 0 {
            return Err(ExevalatorError::new(ErrorMessages::EMPTY_EXPRESSION));
        }
        if settings.max_token_count < token_words.len() {
            return Err(ExevalatorError::new(
                &ErrorMessages::TOO_MANY_TOKENS.replace("$0", &settings.max_token_count.to_string())
            ));
        }
 
        // Create Token structs.
        // Also, escaped number literals will be recovered.
        let tokens: Vec<Token> = match LexicalAnalyzer::create_tokens_from_token_words(
            &token_words, &num_literals, settings
        ) {
            Ok(created_tokens) => created_tokens,
            Err(tokenization_error) => return Err(tokenization_error),
        };

        // Check syntactic correctness of tokens.
        if let Err(syntax_error) = LexicalAnalyzer::check_parenthesis_balance(&tokens) {
            return Err(syntax_error);
        }
        if let Err(syntax_error) = LexicalAnalyzer::check_empty_parentheses(&tokens) {
            return Err(syntax_error);
        }
        if let Err(syntax_error) = LexicalAnalyzer::check_locations_of_operators_and_leafs(&tokens) {
            return Err(syntax_error);
        }
        return Ok(tokens);
    }

    /// Detects the end of the specified number literal in the expression.
    ///
    /// * `expression` - The expression containing the number literal.
    /// * `literal_begin` - The index of the character at the beginning of the number literal.
    /// * Return value - The index of the character at the end of the number literal.
    ///
    fn detect_end_of_num_literal(expression: &str, literal_begin: usize) -> usize {
        let chars: Vec<char> = expression.chars().collect();
        let char_count: usize = chars.len();

        let mut is_integer_part = true;
        let mut is_decimal_part = false;
        let mut is_exponent_part = false;

        for ichar in literal_begin..char_count {
            let c: char = chars[ichar];

            // Numbers (0,1,2, ... 9)
            if c.is_digit(10) {
                continue;

            // Decimal point
            } else if c == '.' {

                // If reading integer part, go to decimal part
                if is_integer_part {
                    is_integer_part = false;
                    is_decimal_part = true;

                // '.' can be only between integer part and decimal part,
                // so elsewhere the '.' belongs to the next token.
                } else {
                    return ichar - 1;
                }

            // Beginning of the exponent part
            } else if c == 'e' || c == 'E' {

                // If reading integer or decimal part, go to exponent part
                if is_integer_part || is_decimal_part {
                    is_integer_part = false;
                    is_decimal_part = false;
                    is_exponent_part = true;

                // Elsewhere the 'e'/'E' belongs to the next token.
                } else {
                    return ichar - 1;
                } 

            // Sign of the exponent part
            } else if c == '+' || c == '-' {

                // In the number literal, the sign can only be at the beginning of the exponent part,
                // so elsewhere it is the next token (operator).
                // Note that, '-' of "-1.23" is a unary minus operator, not contained in the number literal.
                if ichar == 0 || !is_exponent_part || (chars[ichar - 1] != 'e' && chars[ichar - 1] != 'E') {
                    return ichar - 1;
                }

            } else {
                return ichar - 1;
            }
        }

        // Reaches when the number literal ends at the end of the expression.
        return char_count - 1;
    }

    /// Detects all number literals in the expression, and replace them to the escaped representation.
    ///
    /// * `expression` - The expression to be processed.
    /// * `number_literals` To this argument, detected number literals will be stored.
    /// * `settings` - The struct storing values of setting items.
    /// * Return value - The expression in which all number literals are escaped.
    ///
    fn escape_number_literals(expression: &str, number_literals: &mut Vec<String>, settings: &Settings)
            -> String {

        let mut escaped_expression: String = String::new();
        let chars: Vec<char> = expression.chars().collect();
        let char_count: usize = chars.len();

        let mut ichar: usize = 0;
        while ichar < char_count {
            let c: char = chars[ichar];

            let token_begin: bool = ichar == 0
                || chars[ichar-1] == ' '
                || settings.space_equivalents.contains(&chars[ichar-1])
                || settings.token_splitters.contains(&chars[ichar-1]);

            // Numbers (0,1,2, ... 9)
            if token_begin && c.is_digit(10) {
                let literal_end: usize = LexicalAnalyzer::detect_end_of_num_literal(expression, ichar);
                let literal: String = expression[ichar .. literal_end + 1].to_string();
                number_literals.push(literal);
                ichar = literal_end + 1;
                escaped_expression.push_str(&settings.escaped_number_literal);
            } else {
                escaped_expression.push(c);
                ichar += 1;
            }
        }
        return escaped_expression;
    }

    /// Sprits (tokenizes) the expression into token-words. 
    /// The type of each word is `String`, not `Token` struct yet, at this stage.
    ///
    /// * `expression` - The expression to be splitted (tokenized) into token-words.
    /// * `settings` - The struct storing values of setting items.
    /// * Return value - Token-words.
    ///
    fn split_expression_into_token_words(expression: &str, settings: &Settings) -> Vec<String> {
        let mut token_words: Vec<String> = Vec::new();

        const SPACE: &str = " ";
        let mut spaced_expression: String = expression.to_string();
        for splitter in &settings.token_splitters {
            let spaced_splitter: String = format!("{}{}{}", SPACE, splitter, SPACE);
            spaced_expression = spaced_expression.replace(&splitter.to_string(), &spaced_splitter);
        }
        for space_equiv_char in &settings.space_equivalents {
            spaced_expression = spaced_expression.replace(&space_equiv_char.to_string(), SPACE);
        }

        let splitteds: Vec<&str> = (spaced_expression.split_whitespace()).collect();
        for splitted in splitteds.iter() {
            token_words.push(splitted.to_string());
        }
        return token_words;
    }

    /// Creates `Token` type instances from `String` type token-words.
    /// Also, escaped number literals will be recovered.
    ///
    /// * `token_words` - Token-words to be converted to `Token` type instances.
    /// * `number_literals` - Values of number literals to be recovered.
    /// * `settings` - The struct storing values of setting items.
    /// * Return value - `Token` type instances.
    ///
    fn create_tokens_from_token_words(
            token_words: &Vec<String>, number_literals: &Vec<String>, settings: &Settings)
            -> Result<Vec<Token>, ExevalatorError> {

        let token_count: usize = token_words.len();

        // Stores the parenthesis-depth, which will increase at "(" and decrease at ")".
        let mut parenthesis_depth: i64 = 0;

        // Stores the parenthesis-depth when a function call operator begins,
        // for detecting the end of the function operator.
        let mut call_parenthesis_depths: HashSet<i64> = HashSet::new();

        let mut tokens: Vec<Token> = Vec::new();
        let mut last_token: Option<&Token> = None;
        let mut last_operator_type: Option<&OperatorType> = None;
        let mut iliteral: usize = 0;

        for itoken in 0..token_count {
            let word: &String = &token_words[itoken];
            
            // Cases of open parentheses, or beginning of function calls.
            if word.eq("(") {
                parenthesis_depth += 1;
                if 1 <= itoken && (&tokens[itoken - 1]).token_type == TokenType::FunctionIdentifier {
                    call_parenthesis_depths.insert(parenthesis_depth);
                    let mut token: Token = Token::new(TokenType::Operator, word.clone());
                    token.operator = Some(settings.call_symbol_operator_map[&'('].clone());
                    last_operator_type = Some(&OperatorType::Call);
                    tokens.push(token);
                } else {
                    tokens.push(Token::new(TokenType::Parenthesis, word.clone()));
                }

            // Cases of closes parentheses, or end of function calls.
            } else if word.eq(")") {
                if call_parenthesis_depths.contains(&parenthesis_depth) {
                    call_parenthesis_depths.remove(&parenthesis_depth);
                    let mut token: Token = Token::new(TokenType::Operator, word.clone());
                    token.operator = Some(settings.call_symbol_operator_map[&')'].clone());
                    last_operator_type = Some(&OperatorType::Call);
                    tokens.push(token);
                } else {
                    tokens.push(Token::new(TokenType::Parenthesis, word.clone()));
                }
                parenthesis_depth -= 1;

            // Cases of operators.
            } else if word.len() == 1 && settings.operator_symbol_set.contains(&word.chars().nth(0).unwrap()) {
                let mut token: Token = Token::new(TokenType::Operator, word.clone());
                let operator_symbol_char: char = word.chars().nth(0).unwrap();

                // Cases of unary-prefix operators.
                if last_token.is_none()
                        || last_token.unwrap().word.eq("(")
                        || last_token.unwrap().word.eq(",")
                        || (last_token.unwrap().token_type == TokenType::Operator
                        && *last_operator_type.unwrap() != OperatorType::Call) {

                    token.operator = match settings.unary_prefix_symbol_operator_map.get(&operator_symbol_char) {
                        Some(unary_prefix_operator) => Some(unary_prefix_operator.clone()),
                        None => return Err(ExevalatorError::new(&ErrorMessages::UNKNOWN_UNARY_PREFIX_OPERATOR.replace("$0", word))),
                    };
                    last_operator_type = Some(&OperatorType::UnaryPrefix);

                // Cases of binary operators.
                } else if last_token.unwrap().word.eq(")")
                        || last_token.unwrap().token_type == TokenType::NumberLiteral
                        || last_token.unwrap().token_type == TokenType::VariableIdentifier {

                    token.operator = match settings.binary_symbol_operator_map.get(&operator_symbol_char) {
                        Some(binary_operator) => Some(binary_operator.clone()),
                        None => return Err(ExevalatorError::new(&ErrorMessages::UNKNOWN_BINARY_OPERATOR.replace("$0", word))),
                    };
                    last_operator_type = Some(&OperatorType::Binary);
                }
                tokens.push(token);

            // Cases of literals and separator.
            } else if word.eq(&settings.escaped_number_literal) {
                tokens.push(Token::new(TokenType::NumberLiteral, number_literals[iliteral].clone()));
                iliteral += 1;
            } else if word.eq(",") {
                tokens.push(Token::new(TokenType::ExpressionSeparator, word.clone()));

            // Cases of variable identifier of function identifier.
            } else {
                if itoken < token_count - 1 && token_words.get(itoken + 1).unwrap().eq("(") {
                    tokens.push(Token::new(TokenType::FunctionIdentifier, word.clone()));
                } else {
                    tokens.push(Token::new(TokenType::VariableIdentifier, word.clone()));
                }
            }
            last_token = Some(&tokens.last().unwrap());
        }
        return Ok(tokens);
    }

    /// Checks syntactic correctness of tokens of inputted expressions.
    ///
    /// * `tokens` - Tokens to be checked.
    /// * Return value - Nothing, or ExevalatorError if any error detected.
    ///
    fn check_parenthesis_balance(tokens: &Vec<Token>) -> Result<(), ExevalatorError> {
        let token_count: usize = tokens.len();
        let mut hierarchy: i64 = 0; // Increases at "(" and decreases at ")".

        for itoken in 0..token_count {
            let token: &Token = &tokens[itoken];
            if token.word.eq("(") {
                hierarchy += 1;
            } else if token.word.eq(")") {
                hierarchy -= 1;
            }

            // If the value of hierarchy is negative, the open parenthesis is deficient.
            if hierarchy < 0 {
                return Err(ExevalatorError::new(ErrorMessages::DEFICIENT_OPEN_PARENTHESIS));
            }
        }

        // If the value of hierarchy is not zero at the end of the expression,
        // the closed parentheses ")" is deficient.
        if hierarchy > 0 {
            return Err(ExevalatorError::new(ErrorMessages::DEFICIENT_CLOSED_PARENTHESIS));
        }
        return Ok(());
    }

    /// Checks that empty parentheses "()" are not contained in the expression.
    ///
    /// * `tokens` - Tokens to be checked.
    /// * Return value - Nothing, or ExevalatorError if any error detected.
    ///
    fn check_empty_parentheses(tokens: &Vec<Token>) -> Result<(), ExevalatorError> {
        let token_count: usize = tokens.len();
        let mut content_counter: usize = 0;

        for itoken in 0..token_count {
            let token: &Token = &tokens[itoken];
            if token.token_type == TokenType::Parenthesis { // Excepting CALL operators
                if token.word.eq("(") {
                    content_counter = 0;
                } else if token.word.eq(")") {
                    if content_counter == 0 {
                        return Err(ExevalatorError::new(ErrorMessages::EMPTY_PARENTHESIS));
                    }
                }
            } else {
                content_counter += 1;
            }
        }
        return Ok(());
    }

    /// Checks correctness of locations of operators and leaf elements (literals and identifiers).
    ///
    /// * `tokens` - Tokens to be checked.
    /// * Return value - Nothing, or ExevalatorError if any error detected.
    ///
    fn check_locations_of_operators_and_leafs(tokens: &Vec<Token>) -> Result<(), ExevalatorError> {
        let token_count: usize = tokens.len();
        let mut leaf_type_set: HashSet<TokenType> = HashSet::new();
        leaf_type_set.insert(TokenType::NumberLiteral);
        leaf_type_set.insert(TokenType::VariableIdentifier);

        // Reads and check tokens from left to right.
        for itoken in 0..token_count {
            let token: &Token = &tokens[itoken];

            // Prepare information of next/previous token.
            let next_is_leaf: bool = itoken != token_count-1 && leaf_type_set.contains(&tokens[itoken+1].token_type);
            let prev_is_leaf: bool = itoken != 0 && leaf_type_set.contains(&tokens[itoken-1].token_type);
            let next_is_open_parenthesis: bool = itoken < token_count-1 && tokens[itoken+1].word.eq("(");
            let prev_is_close_parenthesis: bool = itoken != 0 && tokens[itoken-1].word.eq(")");
            let next_is_prefix_operator: bool = itoken < token_count-1
                    && tokens[itoken+1].token_type == TokenType::Operator
                    && tokens[itoken+1].operator.as_ref().unwrap().operator_type == OperatorType::UnaryPrefix;
            let next_is_function_call_begin: bool = next_is_open_parenthesis
                    && tokens[itoken+1].token_type == TokenType::Operator
                    && tokens[itoken+1].operator.as_ref().unwrap().operator_type == OperatorType::Call;
            let next_is_function_identifier: bool = itoken < token_count-1
                    && tokens[itoken+1].token_type == TokenType::FunctionIdentifier;

            // Case of operators
            if token.token_type == TokenType::Operator {
                let operator_type: &OperatorType = &token.operator.as_ref().unwrap().operator_type;

                // Cases of unary-prefix operators
                if *operator_type == OperatorType::UnaryPrefix {

                    // Only leafs, open parentheses, unary-prefix and function-call operators can be an operand.
                    if !(next_is_leaf || next_is_open_parenthesis || next_is_prefix_operator || next_is_function_identifier) {
                        return Err(ExevalatorError::new(&ErrorMessages::RIGHT_OPERAND_REQUIRED.replace("$0", &token.word)));
                    }
                } // Cases of unary-prefix operators

                // Cases of binary operators or a separator of partial expressions
                if *operator_type == OperatorType::Binary || token.word.eq(",") {

                    // Only leaf elements, open parenthesis, and unary-prefix operator can be a right-operand.
                    if !(next_is_leaf || next_is_open_parenthesis || next_is_prefix_operator || next_is_function_identifier) {
                        return Err(ExevalatorError::new(&ErrorMessages::RIGHT_OPERAND_REQUIRED.replace("$0", &token.word)));
                    }
                    // Only leaf elements and closed parenthesis can be a right-operand.
                    if !(prev_is_leaf || prev_is_close_parenthesis) {
                        return Err(ExevalatorError::new(&ErrorMessages::LEFT_OPERAND_REQUIRED.replace("$0", &token.word)));
                    }
                } // Cases of binary operators or a separator of partial expressions

            } // Case of operators

            // Case of leaf elements
            if leaf_type_set.contains(&token.token_type) {

                // An other leaf element or an open parenthesis can not be at the right of an leaf element.
                if !next_is_function_call_begin && (next_is_open_parenthesis || next_is_leaf) {
                    return Err(ExevalatorError::new(&ErrorMessages::RIGHT_OPERATOR_REQUIRED.replace("$0", &token.word)));
                }

                // An other leaf element or a closed parenthesis can not be at the left of an leaf element.
                if prev_is_close_parenthesis || prev_is_leaf {
                    return Err(ExevalatorError::new(&ErrorMessages::LEFT_OPERATOR_REQUIRED.replace("$0", &token.word)));
                }
            } // Case of leaf elements
        } // Loops for each token

        return Ok(());
    }
}


/// The object performing functions of a parser.
struct Parser {
}

impl Parser {

    /// Parses tokens and construct Abstract Syntax Tree (AST).
    ///
    /// * `tokens` - Tokens Tokens to be parsed.
    /// * `settings` - The struct storing values of setting items.
    /// * Return value - The root node of the constructed AST.
    ///
    fn parse(tokens: &Vec<Token>, settings: &Settings) -> Result<AstNode,ExevalatorError> {

        // Number of tokens
        let token_count: usize = tokens.len();

        // Working stack to form multiple AstNode instances into a tree-shape.
        let mut stack: VecDeque<AstNode> = VecDeque::new();

        // Tokens of temporary nodes used in the above working stack, for isolating ASTs of partial expressions.
        let parenthesis_lid_token: Token = Token::new(TokenType::StackLid, "(PARENTHESIS_STACK_LID)".to_string());
        let separator_lid_token: Token   = Token::new(TokenType::StackLid, "(SEPARATOR_STACK_LID)".to_string());
        let call_begin_lid_token: Token   = Token::new(TokenType::StackLid, "(CALL_BEGIN_STACK_LID)".to_string());

        // The Vec storing next operator's precedence for each token.
        // At [i], it is stored that the precedence of the first operator of which token-index is greater than i.
        let next_operator_precedences: Vec<u64> = Parser::get_next_operator_precedences(&tokens);
        
        // Read tokens from left to right.
        let mut itoken: usize = 0;
        while itoken < token_count {

            let token: &Token = &tokens[itoken];
            let mut operator_node: Option<AstNode> = None;

            // Case of literals and identifiers: "1.23", "x", "f", etc.
            if token.token_type == TokenType::NumberLiteral
                    || token.token_type == TokenType::VariableIdentifier
                    || token.token_type == TokenType::FunctionIdentifier {
                stack.push_back(AstNode::new(token.clone()));
                itoken += 1;
                continue;

            // Case of parenthesis: "(" or ")"
            } else if token.token_type == TokenType::Parenthesis {
                if token.word.eq("(") {
                    //stack.push_back(parenthesis_lid.clone());
                    stack.push_back( AstNode::new(parenthesis_lid_token.clone()) );
                    itoken += 1;
                    continue;
                } else { // Case of ")"
                    let partial_expr_node: AstNode = match Parser::pop_partial_expr_nodes(&mut stack, &parenthesis_lid_token) {
                        Ok(mut popped_nodes) => popped_nodes.remove(0),
                        Err(popping_error) => return Err(popping_error),
                    };
                    operator_node = Some(partial_expr_node); // Note: the peak of AST of expression is an operator node.
                }

            // Case of separator: ","
            } else if token.token_type == TokenType::ExpressionSeparator {
                stack.push_back( AstNode::new(separator_lid_token.clone()) );
                itoken += 1;
                continue;

            // Case of operators: "+", "-", etc.
            } else if token.token_type == TokenType::Operator {
                operator_node = Some( AstNode::new(token.clone()) );
                let mut opnode_mut_ref: &mut AstNode = operator_node.as_mut().unwrap();
                let operator: &Operator = token.operator.as_ref().unwrap();
                let next_op_precedence: u64 = next_operator_precedences[itoken];

                // Case of unary-prefix operators:
                // * Connect the node of right-token as an operand, if necessary (depending the next operator's precedence).
                if operator.operator_type == OperatorType::UnaryPrefix {
                    if Parser::should_add_right_operand(operator.precedence, next_op_precedence) {
                        opnode_mut_ref.child_nodes.push( AstNode::new(tokens[itoken + 1].clone()) );
                        itoken += 1;
                    } // else: Operand will be connected later. See the bottom of this loop.

                // Case of binary operators:
                // * Always connect the node of left-token as an operand.
                // * Connect the node of right-token as an operand, if necessary (depending the next operator's precedence).
                } else if operator.operator_type == OperatorType::Binary {
                    opnode_mut_ref.child_nodes.push( stack.pop_back().unwrap() );
                    if Parser::should_add_right_operand(operator.precedence, next_op_precedence) {
                        opnode_mut_ref.child_nodes.push( AstNode::new(tokens[itoken + 1].clone()) );
                        itoken += 1;
                    } // else: Right-operand will be connected later. See the bottom of this loop.

                // Case of function-call operators.
                } else if operator.operator_type == OperatorType::Call {
                    if token.word.eq("(") {
                        opnode_mut_ref.child_nodes.push( stack.pop_back().unwrap() ); // Add function-identifier node at the top of the stack.
                        stack.push_back( operator_node.unwrap() );
                        stack.push_back( AstNode::new(call_begin_lid_token.clone()) ); // The marker to correct partial expressions of args from the stack.
                        itoken += 1;
                        continue;
                    } else { // Case of ")"
                        let arg_nodes: Vec<AstNode> = match Parser::pop_partial_expr_nodes(&mut stack, &call_begin_lid_token) {
                            Ok(popped_nodes) => popped_nodes,
                            Err(popping_error) => return Err(popping_error),
                        };
                        operator_node = Some(stack.pop_back().unwrap());
                        opnode_mut_ref = operator_node.as_mut().unwrap();
                        for arg_node in arg_nodes {
                            opnode_mut_ref.child_nodes.push(arg_node);
                        }
                    }
                }
            }

            // If the precedence of the operator at the top of the stack is stronger than the next operator,
            // connect all "unconnected yet" operands and operators in the stack.
            while Parser::should_add_right_operand_to_stacked_operator(&mut stack, next_operator_precedences[itoken]) {
                let old_operator_node: AstNode = operator_node.unwrap();
                operator_node = Some(stack.pop_back().unwrap());
                operator_node.as_mut().unwrap().child_nodes.push(old_operator_node);
            }
            stack.push_back(operator_node.unwrap());
            itoken += 1;
        }

        // The AST has been constructed on the stack, and only its root node is stored in the stack.
        let root_node_of_expression_ast: AstNode = stack.pop_back().unwrap();

        // Check that the depth of the constructed AST does not exceeds the limit,
        // and return the AST if its depth does not exceed the limti. 
        match root_node_of_expression_ast.check_depth(1, settings.max_ast_depth) {
            Err(ast_depth_error) => return Err(ast_depth_error),
            Ok(()) => return Ok(root_node_of_expression_ast),
        }
    }

    /// Judges whether the right-side token should be connected directly as an operand, to the target operator.
    ///
    /// * `target_operator_precedence` - The precedence of the target operator (smaller value gives higher precedence).
    /// * `next_operator_precedence` - The precedence of the next operator (smaller value gives higher precedence).
    /// * Return value - Returns true if the right-side token (operand) should be connected to the target operator.
    ///
    fn should_add_right_operand(target_operator_precedence: u64, next_operator_precedence: u64) -> bool {
        return target_operator_precedence <= next_operator_precedence; // left is stronger
    }

    /// Judges whether the right-side token should be connected directly as an operand,
    /// to the operator at the top of the working stack.
    ///
    /// * `stack` - The working stack used for the parsing.
    /// * `next_operator_precedence` - The precedence of the next operator (smaller value gives higher precedence).
    /// * Return value - Returns true if the right-side token (operand) should be connected to the operator at the top of the stack.
    ///
    fn should_add_right_operand_to_stacked_operator(stack: &mut VecDeque<AstNode>, next_operator_precedence: u64) -> bool {
        if stack.len() == 0 {
            return false;
        }

        let back_node: &AstNode = stack.back().unwrap();
        if back_node.token.token_type != TokenType::Operator {
            return false;
        }
        return Parser::should_add_right_operand(
            back_node.token.operator.as_ref().unwrap().precedence, next_operator_precedence
        );
    }

    /// Pops root nodes of ASTs of partial expressions constructed on the stack.
    /// In the returned array, the popped nodes are stored in FIFO order.
    ///
    /// * `stack` - The working stack used for the parsing.
    /// * `end_stack_lid_node_token` - The token of the temporary node pushed in the stack,
    ///                                at the end of partial expressions to be popped.
    /// * Return value - Root nodes of ASTs of partial expressions.
    ///
    fn pop_partial_expr_nodes(stack: &mut VecDeque<AstNode>, end_stack_lid_node_token: &Token)
            -> Result<Vec<AstNode>, ExevalatorError> {

        if stack.len() == 0 {
            return Err(ExevalatorError::new(ErrorMessages::UNEXPECTED_PARTIAL_EXPRESSION));
        }
        let mut partial_expr_nodes: Vec<AstNode> = Vec::new();

        while stack.len() != 0 {
            if stack.back().unwrap().token.token_type == TokenType::StackLid {
                let stack_lid_node: AstNode = stack.pop_back().unwrap();
                if stack_lid_node.token.word.eq(&end_stack_lid_node_token.word) {
                    break;
                }
            } else {
                partial_expr_nodes.push(stack.pop_back().unwrap());
            }
        }
        partial_expr_nodes.reverse();
        return Ok(partial_expr_nodes);
    }

    /// Returns an Vec storing next operator's precedence for each token.
    /// In the returned Vec, it will stored at \[i\] that
    /// precedence of the first operator of which token-index is greater than i.
    ///
    /// * `tokens` - All tokens to be parsed.
    /// * Return value - The array storing next operator's precedence for each token.
    ///
    fn get_next_operator_precedences(tokens: &Vec<Token>) -> Vec<u64> {
        let token_count: usize = tokens.len();
        let mut last_op_precedence: u64 = std::u64::MAX;
        let mut next_op_precedences: Vec<u64> = Vec::with_capacity(token_count);

        for _itoken in 0..token_count {
            next_op_precedences.push(0);
        }

        for itoken in (0..token_count).rev() {
            let token: &Token = &tokens[itoken];
            next_op_precedences[itoken] = last_op_precedence;

            if token.token_type == TokenType::Operator {
                last_op_precedence = token.operator.as_ref().unwrap().precedence;
            }

            if token.token_type == TokenType::Parenthesis {
                if token.word.eq("(") {
                    last_op_precedence = 0; // most prior
                } else {
                    last_op_precedence = std::u64::MAX; // least prior
                }
            }
        }
        return next_op_precedences;
    }
}


/// The enum representing types of operators.
#[derive(PartialEq,Eq,Clone,Copy,Debug)]
enum OperatorType {

    /// Represents unary operator, for example: - of -1.23.
    UnaryPrefix,

    /// Represents binary operator, for example: + of 1+2.
    Binary,

    /// Represents function-call operator.
    Call,
}

/// The struct storing information of an operator.
#[derive(PartialEq,Eq,Clone,Debug)]
struct Operator {

    /// The type of operator.
    operator_type: OperatorType,

    /// The symbol of this operator (for example: '+').
    symbol: char,

    /// The precedence of this operator (smaller value gives higher precedence).
    precedence: u64,
}
impl Operator {

    /// Creates a new instance storing specified information.
    ///
    /// * `operator_type` The type of this operator.
    /// * `symbol` - The symbol of this operator.
    /// * `precedence` - The precedence of this operator.
    /// * Return value - The created instance.
    ///
    fn new(operator_type: OperatorType, symbol: char, precedence: u64) -> Self {
        Self {
            operator_type: operator_type,
            symbol: symbol,
            precedence: precedence,
        }
    }

    /// Clones this instance.
    ///
    /// * Return value - The cloned instance.
    ///
    fn clone(&self) -> Self {
        Self {
            operator_type: self.operator_type.clone(),
            symbol: self.symbol.clone(),
            precedence: self.precedence.clone(),
        }
    }
}


/// The enum representing types of tokens.
#[derive(PartialEq,Eq,Clone,Copy,Debug, Hash)]
enum TokenType {

    /// Represents number literal tokens, for example: 1.23.
    NumberLiteral,

    /// Represents operator tokens, for example: +.
    Operator,

    /// Represents separator tokens of partial expressions: ,.
    ExpressionSeparator,

    /// Represents parenthesis, for example: ( and ) of (1*(2+3)).
    Parenthesis,

    /// Represents variable-identifier tokens, for example: x.
    VariableIdentifier,

    /// Represents function-identifier tokens, for example: f.
    FunctionIdentifier,

    /// Represents temporary token for isolating partial expressions in the stack, in parser.
    StackLid,
}

/// The object storing information of an token.
#[derive(Clone,Debug)]
struct Token {
    token_type: TokenType,
    word: String,
    operator: Option<Operator>,
}

impl Token {

    /// Create a new instance storing specified information.
    ///
    /// * `token_type` - The type of this token.
    /// * `word` - The text representation of this token.
    ///
    fn new(token_type: TokenType, word: String) -> Self {
        Self {
            token_type: token_type,
            word: word,
            operator: None,
        }
    }

    /// Clones this instance.
    ///
    /// * Return value - The cloned instance.
    ///
    fn clone(&self) -> Self {
        Self {
            token_type: self.token_type.clone(),
            word: self.word.clone(),
            operator: self.operator.clone(),
        }
    }
}


/// The struct storing information of an node of an AST.
struct AstNode {

    /// The token corresponding with this AST node.
    token: Token,

    /// The list of child nodes of this AST node.
    child_nodes: Vec<AstNode>,
}

impl AstNode {
    
    /// Creates an AST node instance storing specified information.
    ///
    /// * `token` - The token corresponding with this AST node.
    ///
    fn new(token: Token) -> Self {
        Self {
            token: token,
            child_nodes: Vec::new(),
        }
    }

    /// Checks that depths in the AST of all nodes under this node (child nodes, grandchild nodes, and so on)
    /// does not exceeds the specified maximum value.
    /// An ExevalatorError will be returned when the depth exceeds the maximum value.
    /// If the depth does not exceeds the maximum value, nothing will be returned.
    ///
    /// * `depthOfThisNode` The depth of this node in the AST.
    /// * `maxAstDepth` The maximum value of the depth of the AST.
    /// * Return value - Nothing, or ExevalatorError if any error detected.
    ///
    fn check_depth(&self, depth_of_this_node: u64, max_ast_depth: u64) -> Result<(), ExevalatorError> {
        if max_ast_depth < depth_of_this_node {
            return Err(ExevalatorError::new(
                &ErrorMessages::EXCEEDS_MAX_AST_DEPTH.replace("$0", &max_ast_depth.to_string())
            ));
        }
        for child_node in &self.child_nodes {
            if let Err(depth_error) = child_node.check_depth(depth_of_this_node + 1, max_ast_depth) {
                return Err(depth_error)
            }
        }
        return Ok(());
    }

    /// Expresses the AST under this node in XML-like text format.
    ///
    /// * `indent_stage` The stage of indent of this node.
    /// * Return value - XML-like text representation of the AST under this node.
    ///
    #[allow(dead_code)]
    fn to_markupped_text(&self, indent_stage: u64) -> String {
        let mut result: String = String::new();
        const END_OF_LINE: &str = "\n";
        const INDENT_UNIT: &str = "  ";

        let indent: String = INDENT_UNIT.repeat(indent_stage as usize);
        result.push_str(&indent);

        result.push_str("<");
        result.push_str(&format!("{:?}", self.token.token_type));
        result.push_str(" word=\"");
        result.push_str(&self.token.word);
        result.push_str("\"");
        if self.token.token_type == TokenType::Operator {
            let operator: &Operator = self.token.operator.as_ref().unwrap();
            result.push_str(" optype=\"");
            result.push_str(&format!("{:?}", operator.operator_type));
            result.push_str("\" precedence=\"");
            result.push_str(&operator.precedence.to_string());
            result.push_str("\"");
        }

        if 0 < self.child_nodes.len() {
            result.push_str(">");
            for child_node in &self.child_nodes {
                result.push_str(END_OF_LINE);
                result.push_str(&child_node.to_markupped_text(indent_stage + 1));
            }
            result.push_str(END_OF_LINE);
            result.push_str(&indent);
            result.push_str("</");
            result.push_str(&format!("{:?}", self.token.token_type));
            result.push_str(">");

        } else {
            result.push_str(" />");
        }

        return result;
    }
}


/// The object for evaluating the value of an AST.
struct Evaluator {
    
    /// The tree of evaluator nodes, which evaluates the value of AST.
    evaluator_node_tree: Option<Box<dyn EvaluatorNode>>,

}

impl Evaluator {

    /// Creates a new evaluator.
    ///
    /// * Return value - The created instance.
    ///
    #[allow(dead_code)]
    pub fn new() -> Self {
        Self {
            evaluator_node_tree: None,
        }
    }

    /// Updates the state to evaluate the value of the AST.
    ///
    /// * `ast` - The root node of the AST to be evaluated.
    /// * `variable_table` - The HashMap mapping variable names to virtual addresses.
    /// * `function_table` - The HashMap mapping function names to function pointers.
    /// * `settings` - The struct storing values of setting items.
    /// * Return value - The ExevalatorError if any error detected.
    /// 
    fn update(
        &mut self,
        ast: &AstNode,
        variable_table: &HashMap<String, usize>,
        function_table: &HashMap<String, fn(Vec<f64>)->Result<f64,ExevalatorError> >,
        settings: &Settings) -> Result<(),ExevalatorError> {

        self.evaluator_node_tree = 
        match Evaluator::create_evaluator_node_tree(&ast, &variable_table, &function_table, &settings) {
            Ok(created_node) => Some(created_node),
            Err(node_creation_error) => return Err(node_creation_error),
        };
        return Ok(());
    }

    /// Returns whether "evaluate" method is available on the current state.
    ///
    /// * Return value - True if "evaluate" method is available.
    ///
    fn is_evaluatable(&self) -> bool {
        return !self.evaluator_node_tree.is_none();
    }

    /// Evaluates the value of the AST set by "update" method.
    ///
    /// * `memory` - The Vec used as as a virtual memory storing values of variables.
    /// * Return value - The evaluated value.
    ///
    fn evaluate(&self, memory: &Vec<f64>) -> Result<f64, ExevalatorError> {
        let node: &Box<dyn EvaluatorNode> = &(*self.evaluator_node_tree.as_ref().unwrap());
        return node.evaluate(&memory);
    }

    /// Creates a tree of evaluator nodes corresponding with the specified AST.
    ///
    /// * `ast` - The root node of the AST.
    /// * `variable_table` - The HashMap mapping variable names to virtual addresses.
    /// * `function_table` - The HashMap mapping function names to function pointers.
    /// * `settings` - The struct storing values of setting items.
    /// * Return value - The Box pointing the created node, or ExevalatorError if any error detected.
    ///
    fn create_evaluator_node_tree(
        ast: &AstNode,
        variable_table: &HashMap<String, usize>,
        function_table: &HashMap<String, fn(Vec<f64>)->Result<f64,ExevalatorError> >,
        settings: &Settings)
        -> Result<Box<dyn EvaluatorNode>, ExevalatorError> {

        // Note: This method creates a tree of evaluator nodes by traversing each node in the AST recursively.

       let token_type: &TokenType = &ast.token.token_type;

       // Create an node for evaluating number literal.
        if *token_type == TokenType::NumberLiteral {
            let literal_value: f64 = match ast.token.word.parse() {
                Ok(parse_result) => parse_result,
                Err(_parse_error) => return Err(ExevalatorError::new(
                    &ErrorMessages::INVALID_NUMBER_LITERAL.replace("$0", &ast.token.word)
                )),
            };
            return Ok(Box::new(NumberLiteralEvaluatorNode {
                literal_value: literal_value,
            }));

        // Create nodes for evaluating operators.
        } else if *token_type == TokenType::Operator {
            let operator: &Operator = &ast.token.operator.as_ref().unwrap();
            let optype: OperatorType = operator.operator_type;
            let opsymbol: char = operator.symbol;

            // Unary operator "-"
            if optype == OperatorType::UnaryPrefix && opsymbol == '-' {
                return Ok(Box::new(MinusEvaluatorNode {
                    operand: match Evaluator::create_evaluator_node_tree(&ast.child_nodes[0], variable_table, function_table, settings) {
                        Ok(created_node) => created_node, Err(creation_error) => return Err(creation_error),
                    },
                }));

            // Binary operator "+"
            } else if optype == OperatorType::Binary && opsymbol == '+' {
                return Ok(Box::new(AdditionEvaluatorNode {
                    left_operand:  match Evaluator::create_evaluator_node_tree(&ast.child_nodes[0], variable_table, function_table, settings) {
                        Ok(created_node) => created_node, Err(creation_error) => return Err(creation_error),
                    },
                    right_operand: match Evaluator::create_evaluator_node_tree(&ast.child_nodes[1], variable_table, function_table, settings) {
                        Ok(created_node) => created_node, Err(creation_error) => return Err(creation_error),
                    },
                }));

            // Binary operator "-"
            } else if optype == OperatorType::Binary && opsymbol == '-' {
                return Ok(Box::new(SubtractionEvaluatorNode {
                    left_operand:  match Evaluator::create_evaluator_node_tree(&ast.child_nodes[0], variable_table, function_table, settings) {
                        Ok(created_node) => created_node, Err(creation_error) => return Err(creation_error),
                    },
                    right_operand: match Evaluator::create_evaluator_node_tree(&ast.child_nodes[1], variable_table, function_table, settings) {
                        Ok(created_node) => created_node, Err(creation_error) => return Err(creation_error),
                    },
                }));

            // Binary operator "*"
            } else if optype == OperatorType::Binary && opsymbol == '*' {
                return Ok(Box::new(MultiplicationEvaluatorNode {
                    left_operand:  match Evaluator::create_evaluator_node_tree(&ast.child_nodes[0], variable_table, function_table, settings) {
                        Ok(created_node) => created_node, Err(creation_error) => return Err(creation_error),
                    },
                    right_operand: match Evaluator::create_evaluator_node_tree(&ast.child_nodes[1], variable_table, function_table, settings) {
                        Ok(created_node) => created_node, Err(creation_error) => return Err(creation_error),
                    },
                }));

            // Binary operator "/"
            } else if optype == OperatorType::Binary && opsymbol == '/' {
                return Ok(Box::new(DivisionEvaluatorNode {
                    left_operand:  match Evaluator::create_evaluator_node_tree(&ast.child_nodes[0], variable_table, function_table, settings) {
                        Ok(created_node) => created_node, Err(creation_error) => return Err(creation_error),
                    },
                    right_operand: match Evaluator::create_evaluator_node_tree(&ast.child_nodes[1], variable_table, function_table, settings) {
                        Ok(created_node) => created_node, Err(creation_error) => return Err(creation_error),
                    },
                }));

            // Function call operator "("
            } else if optype == OperatorType::Call && opsymbol == '(' {
                let child_count = ast.child_nodes.len();
                let identifier: String = ast.child_nodes[0].token.word.clone();
                if !function_table.contains_key(&identifier) {
                    return Err(ExevalatorError::new(
                        &ErrorMessages::FUNCTION_NOT_FOUND.replace("$0", &identifier)
                    ));
                }
                let function_pointer: fn(Vec<f64>) -> Result<f64,ExevalatorError> = *function_table.get(&identifier).unwrap();
                let mut arguments: Vec<Box<dyn EvaluatorNode>> = Vec::new();
                for ichild in 1..child_count {
                    arguments.push(
                        match Evaluator::create_evaluator_node_tree(&ast.child_nodes[ichild], variable_table, function_table, settings) {
                            Ok(created_node) => created_node, Err(creation_error) => return Err(creation_error),
                        }
                    );
                }
                return Ok(Box::new(FunctionEvaluatorNode {
                    function_pointer: function_pointer,
                    identifier: identifier,
                    arguments: arguments,
                }));

            } else {
                panic!("Unexpected operator: {:?}", operator);
            }

        // Create an node for evaluating the value of a variable.
        } else if *token_type == TokenType::VariableIdentifier {
            if !variable_table.contains_key(&ast.token.word) {
                return Err(ExevalatorError::new(
                    &ErrorMessages::VARIABLE_NOT_FOUND.replace("$0", &ast.token.word)
                ));
            }
            let address: usize = *variable_table.get(&ast.token.word).unwrap();
            return Ok(Box::new(VariableEvaluatorNode {
                address: address,
            }));

        } else {
            panic!("Unexpected token type: {:?}", token_type);
        }
    }
}

/// The trait defining the function which is implemented by all kinds of evaluation nodes. 
trait EvaluatorNode {

    /// Evaluates the value.
    ///
    /// * 'memory' - The virtual memory storing values of variables.
    /// * Return value - The evaluated value, or ExevalatorError if any error detected.
    ///
    fn evaluate(&self, memory: &Vec<f64>) -> Result<f64,ExevalatorError>;
}

/// The evaluator node evaluating the value of a number literal.
struct NumberLiteralEvaluatorNode {
    /// The value of the number literal.
    literal_value: f64,
}
impl EvaluatorNode for NumberLiteralEvaluatorNode {
    /// Defined in `EvaluatorNode` trait.
    fn evaluate(&self, _memory: &Vec<f64>) -> Result<f64,ExevalatorError> {
        return Ok(self.literal_value);
    }
}

/// The evaluator node evaluating the value of an unary-minus operator.
struct MinusEvaluatorNode {
    /// The evaluator node for evaluating the value of the operand.
    operand: Box<dyn EvaluatorNode>,
}
impl EvaluatorNode for MinusEvaluatorNode {
    /// Defined in `EvaluatorNode` trait.
    fn evaluate(&self, memory: &Vec<f64>) -> Result<f64,ExevalatorError> {
        let operand_value: f64 = match (*self.operand).evaluate(memory) {
            Ok(evaluate_value) => evaluate_value, Err(evaluation_error) => return Err(evaluation_error),
        };
        return Ok(-operand_value);
    }
}

/// The evaluator node evaluating the value of an addition operator.
struct AdditionEvaluatorNode {
    /// The evaluator node for evaluating the value of the left-operand.
    left_operand: Box<dyn EvaluatorNode>,
    /// The evaluator node for evaluating the value of the right-operand.
    right_operand: Box<dyn EvaluatorNode>,
}
impl EvaluatorNode for AdditionEvaluatorNode {
    /// Defined in `EvaluatorNode` trait.
    fn evaluate(&self, memory: &Vec<f64>) -> Result<f64,ExevalatorError> {
        let left_operand_value: f64 = match (*self.left_operand).evaluate(memory) {
            Ok(evaluate_value) => evaluate_value, Err(evaluation_error) => return Err(evaluation_error),
        };
        let right_operand_value: f64 = match (*self.right_operand).evaluate(memory) {
            Ok(evaluate_value) => evaluate_value, Err(evaluation_error) => return Err(evaluation_error),
        };
        return Ok(left_operand_value + right_operand_value);
    }
}

/// The evaluator node evaluating the value of a subtraction operator.
struct SubtractionEvaluatorNode {
    /// The evaluator node for evaluating the value of the left-operand.
    left_operand: Box<dyn EvaluatorNode>,
    /// The evaluator node for evaluating the value of the right-operand.
    right_operand: Box<dyn EvaluatorNode>,
}
impl EvaluatorNode for SubtractionEvaluatorNode {
    /// Defined in `EvaluatorNode` trait.
    fn evaluate(&self, memory: &Vec<f64>) -> Result<f64,ExevalatorError> {
        let left_operand_value: f64 = match (*self.left_operand).evaluate(memory) {
            Ok(evaluate_value) => evaluate_value, Err(evaluation_error) => return Err(evaluation_error),
        };
        let right_operand_value: f64 = match (*self.right_operand).evaluate(memory) {
            Ok(evaluate_value) => evaluate_value, Err(evaluation_error) => return Err(evaluation_error),
        };
        return Ok(left_operand_value - right_operand_value);
    }
}

/// The evaluator node evaluating the value of a multiplication operator.
struct MultiplicationEvaluatorNode {
    /// The evaluator node for evaluating the value of the left-operand.
    left_operand: Box<dyn EvaluatorNode>,
    /// The evaluator node for evaluating the value of the right-operand.
    right_operand: Box<dyn EvaluatorNode>,
}
impl EvaluatorNode for MultiplicationEvaluatorNode {
    /// Defined in `EvaluatorNode` trait.
    fn evaluate(&self, memory: &Vec<f64>) -> Result<f64,ExevalatorError> {
        let left_operand_value: f64 = match (*self.left_operand).evaluate(memory) {
            Ok(evaluate_value) => evaluate_value, Err(evaluation_error) => return Err(evaluation_error),
        };
        let right_operand_value: f64 = match (*self.right_operand).evaluate(memory) {
            Ok(evaluate_value) => evaluate_value, Err(evaluation_error) => return Err(evaluation_error),
        };
        return Ok(left_operand_value * right_operand_value);
    }
}

/// The evaluator node evaluating the value of a division operator.
struct DivisionEvaluatorNode {
    /// The node for evaluating the value of the left-operand.
    left_operand: Box<dyn EvaluatorNode>,
    /// The evaluator node for evaluating the value of the right-operand.
    right_operand: Box<dyn EvaluatorNode>,
}
impl EvaluatorNode for DivisionEvaluatorNode {
    /// Defined in `EvaluatorNode` trait.
    fn evaluate(&self, memory: &Vec<f64>) -> Result<f64,ExevalatorError> {
        let left_operand_value: f64 = match (*self.left_operand).evaluate(memory) {
            Ok(evaluate_value) => evaluate_value, Err(evaluation_error) => return Err(evaluation_error),
        };
        let right_operand_value: f64 = match (*self.right_operand).evaluate(memory) {
            Ok(evaluate_value) => evaluate_value, Err(evaluation_error) => return Err(evaluation_error),
        };
        return Ok(left_operand_value / right_operand_value);
    }
}

/// The evaluator node evaluating (reading) the value of a variable.
struct VariableEvaluatorNode {
    /// The address (on the virutal memory) of the variable to be read.
    address: usize,
}
impl EvaluatorNode for VariableEvaluatorNode {
    /// Defined in `EvaluatorNode` trait.
    fn evaluate(&self, memory: &Vec<f64>) -> Result<f64,ExevalatorError> {
        return Ok(memory[self.address]);
    }
}

/// The evaluator node evaluating (invoking) a function.
struct FunctionEvaluatorNode {
    /// The function pointer of the function to be invoked.
    function_pointer: fn(Vec<f64>) -> Result<f64,ExevalatorError>,
    /// The identifier (name) of the function.
    identifier: String,
    /// Evaluator nodes for evaluating values of arguments.
    arguments: Vec<Box<dyn EvaluatorNode>>,
}
impl EvaluatorNode for FunctionEvaluatorNode {
    /// Defined in `EvaluatorNode` trait.
    fn evaluate(&self, memory: &Vec<f64>) -> Result<f64,ExevalatorError> {
        let mut arg_evaluated_values: Vec<f64> = Vec::new();
        for arg in &self.arguments {
            let arg_evaluated_value: f64 = match (*arg).evaluate(memory) {
                Ok(evaluated_value) => evaluated_value, Err(evaluation_error) => return Err(evaluation_error),
            };
            arg_evaluated_values.push(arg_evaluated_value);
        }
        match (self.function_pointer)(arg_evaluated_values) {
            Ok(evaluated_value) => return Ok(evaluated_value),
            Err(evaluation_error) => {
                let error_message: String = ErrorMessages::FUNCTION_ERROR
                    .replace("$0", &self.identifier).replace("$1", &evaluation_error.error_message);
                return Err(ExevalatorError::new(&error_message))
            }
        }
    }
}


/// The struct storing values of setting items.
struct Settings {

    /// The maximum number of characters in an expression.
    max_expression_char_count: usize,

    /// The maximum number of characters of variable/function names.
    max_name_char_count: usize,

    /// The maximum number of tokens in an expression.
    max_token_count: usize,

    /// The maximum depth of an Abstract Syntax Tree (AST).
    max_ast_depth: u64,

    /// Set instance storing all symbols of operators.
    operator_symbol_set: HashSet<char>,

    // Map mapping from each symbol of binary operator to the operator.
    binary_symbol_operator_map: HashMap<char, Operator>,

    // Map mapping from each symbol of unary-prefix operator to the operator.
    unary_prefix_symbol_operator_map: HashMap<char, Operator>,

    // Map mapping from each symbol of call operator to the operator.
    call_symbol_operator_map: HashMap<char, Operator>,

    /// Vec instance storing words at which an expression will be splitted into tokens.
    token_splitters: Vec<char>,

    /// Vec instance storing words which are equivalent to white spaces.
    space_equivalents: Vec<char>,

    /// Escaped representation of number literals, used in lexical analysis.
    escaped_number_literal: String,
}

impl Settings {

    /// Creates a new settings storing default values.
    ///
    /// * Return value - The created instance.
    ///
    fn new() -> Self {
        let mut settings: Self = Self {
            max_expression_char_count: 256,
            max_name_char_count: 64,
            max_token_count: 64,
            max_ast_depth: 32,
            operator_symbol_set: HashSet::new(),
            binary_symbol_operator_map: HashMap::new(),
            unary_prefix_symbol_operator_map: HashMap::new(),
            call_symbol_operator_map: HashMap::new(),
            token_splitters: Vec::new(),
            space_equivalents: Vec::new(),
            escaped_number_literal: "@NUMBER_LITERAL@".to_string(),
        };
        let addition_operator: Operator       = Operator::new(OperatorType::Binary,      '+', 400);
        let subtraction_operator: Operator    = Operator::new(OperatorType::Binary,      '-', 400);
        let multiplication_operator: Operator = Operator::new(OperatorType::Binary,      '*', 300);
        let division_operator: Operator       = Operator::new(OperatorType::Binary,      '/', 300);
        let minus_operator: Operator          = Operator::new(OperatorType::UnaryPrefix, '-', 200);
        let call_begin_operator: Operator     = Operator::new(OperatorType::Call,        '(', 100);
        let call_end_operator: Operator       = Operator::new(OperatorType::Call,        ')', std::u64::MAX);
        settings.operator_symbol_set.insert('+');
        settings.operator_symbol_set.insert('-');
        settings.operator_symbol_set.insert('*');
        settings.operator_symbol_set.insert('/');
        settings.operator_symbol_set.insert('(');
        settings.operator_symbol_set.insert(')');
        settings.binary_symbol_operator_map.insert('+', addition_operator.clone());
        settings.binary_symbol_operator_map.insert('-', subtraction_operator.clone());
        settings.binary_symbol_operator_map.insert('*', multiplication_operator.clone());
        settings.binary_symbol_operator_map.insert('/', division_operator.clone());
        settings.unary_prefix_symbol_operator_map.insert('-', minus_operator.clone());
        settings.call_symbol_operator_map.insert('(', call_begin_operator.clone());
        settings.call_symbol_operator_map.insert(')', call_end_operator.clone());
        settings.token_splitters.push('+');
        settings.token_splitters.push('-');
        settings.token_splitters.push('*');
        settings.token_splitters.push('/');
        settings.token_splitters.push('(');
        settings.token_splitters.push(')');
        settings.token_splitters.push(',');
        settings.space_equivalents.push('\n');
        settings.space_equivalents.push('\r');
        settings.space_equivalents.push('\t');
        return settings;
    }
}
