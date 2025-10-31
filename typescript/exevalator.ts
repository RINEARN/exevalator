/*
 * Exevalator Ver.2.2.2 - by RINEARN 2021-2025
 * This software is released under the "Unlicense" license.
 * You can choose the "CC0" license instead, if you want.
 */

// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
// To change the language of error messages,
// copy the contents of ERROR_MESSAGE_*.ts and 
// overwrite the ErrorMessage class below with them.
// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

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

/**
 * Interpreter class of Exevalator.
 */
export default class Exevalator {

    /** The array used as as a virtual memory storing values of variables. */
    private memory: number[];

    /** The current usage (max used index + 1) of the memory. */
    private memoryUsage: number;

    /** The object evaluating the value of the expression. */
    private evaluator: Evaluator;

    /** The Map mapping each variable name to an address of the variable. */
    private variableTable: Map<string, number>;

    /** The Map mapping each function name to an IExevalatorFunction instance. */
    private functionTable: Map<string, ExevalatorFunctionInterface>;

    /** Caches the content of the expression evaluated last time, to skip re-parsing. */
    private lastEvaluatedExpression : string | undefined;

    /**
     * Creates a new interpreter of Exevalator.
     */
    constructor() {        
        this.memory = new Array(StaticSettings.MAX_VARIABLE_COUNT);
        this.memoryUsage = 0;
        this.evaluator = new Evaluator();
        this.variableTable = new Map<string, number>();
        this.functionTable = new Map<string, ExevalatorFunctionInterface>();
        this.lastEvaluatedExpression = undefined;
    }

    /**
     * Evaluates (computes) the value of an expression.
     * 
     * @param expression - The expression to be evaluated.
     * @returns The evaluated value.
     * @throws ExevalatorError - Thrown if the input expression is syntactically incorrect, or uses undeclared variables/functions.
     */
    public eval(expression: string): number {
        if (expression == null || expression === undefined) {
            throw new ExevalatorError(ErrorMessages.ARGS_MUST_NOT_BE_NULL_OR_UNDEFINED.replace("$0", "expression"));
        }
        if (StaticSettings.MAX_EXPRESSION_CHAR_COUNT < expression.length) {
            throw new ExevalatorError(ErrorMessages.TOO_LONG_EXPRESSION.replace("$0", StaticSettings.MAX_EXPRESSION_CHAR_COUNT.toString()));
        }

        try {
            const expressionChanged: boolean = expression !== this.lastEvaluatedExpression;

            // If the expression changed from the last-evaluated expression, re-parsing is necessary.
            if (expressionChanged || !this.evaluator.isEvaluatable()) {

                // Split the expression into tokens, and analyze them.
                const tokens: Token[] = LexicalAnalyzer.analyze(expression);

                /*
                // Temporary, for debugging tokens
                for (const token of tokens) {
                    console.log(token.toString());
                }
                */

                // Construct AST (Abstract Syntax Tree) by parsing tokens.
                const ast: AstNode = Parser.parse(tokens);

                /*
                // Temporary, for debugging AST
                console.log(ast.toMarkuppedText());
                */

                // Update the evaluator, to evaluate the parsed AST.
                this.evaluator.update(ast, this.variableTable, this.functionTable);

                this.lastEvaluatedExpression = expression;
            }

            // Evaluate the value of the expression, and return it.
            const evaluatedValue: number = this.evaluator.evaluate(this.memory);
            return evaluatedValue;

        } catch (error) {
            if (error instanceof ExevalatorError) {
                throw error;
            }
            const errorMessage = error instanceof Error ? error.message : String(error);
            throw new ExevalatorError(ErrorMessages.UNEXPECTED_ERROR.replace("$0", errorMessage));
        }
    }

    /**
     * Re-evaluates (re-computes) the value of the expression evaluated by "eval" method last time.
     * This method may (slightly) work faster than calling "eval" method repeatedly for the same expression.
     * Note that, the result value may differ from the last evaluated value, 
     * if values of variables or behaviour of functions had changed.
     * 
     * @returns The evaluated value
     */
    public reeval(): number {
        if (this.evaluator.isEvaluatable()) {
            const evaluatedValue: number = this.evaluator.evaluate(this.memory);
            return evaluatedValue;
        } else {
            throw new ExevalatorError(ErrorMessages.REEVAL_NOT_AVAILABLE);
        }
    }

    /**
     * Declares a new variable, for using the value of it in expressions.
     * 
     * @param name - The name of the variable to be declared.
     * @returns The virtual address of the declared variable,
     *             which useful for accessing to the variable faster.
     *             See "writeVariableAt" and "readVariableAt" method.
     * @throws ExevalatorError - Thrown if the specified variable name is invalid or already used.
     */
    public declareVariable(name: string): number {
        if (name == null || name === undefined) {
            throw new ExevalatorError(ErrorMessages.ARGS_MUST_NOT_BE_NULL_OR_UNDEFINED.replace("$0", "name"));
        }
        if (StaticSettings.MAX_NAME_CHAR_COUNT < name.length) {
            throw new ExevalatorError(ErrorMessages.TOO_LONG_VARIABLE_NAME.replace("$0", StaticSettings.MAX_NAME_CHAR_COUNT.toString()));
        }
        if (this.variableTable.has(name)) {
            throw new ExevalatorError(ErrorMessages.VARIABLE_ALREADY_DECLARED.replace("$0", name));
        }
        if (StaticSettings.MAX_VARIABLE_COUNT <= this.memoryUsage) {
            throw new ExevalatorError(ErrorMessages.VARIABLE_COUNT_EXCEEDED_LIMIT.replace("$0", ErrorMessages.VARIABLE_COUNT_EXCEEDED_LIMIT.toString()));
        }
        if (2147483647 + 1 <= this.memory.length) { // The theoretical limit for correctly filtering the address.
            throw new ExevalatorError(ErrorMessages.VARIABLE_COUNT_EXCEEDED_LIMIT.replace("$0", (2147483647 + 1).toString()));
        }
        const address = this.memoryUsage;
        this.memory[address] = 0.0;
        this.variableTable.set(name, address);
        this.memoryUsage++;
        return address;
    }

    /**
     * Writes the value to the variable having the specified name.
     *
     * @param name - The name of the variable to be written.
     * @param value - The new value of the variable.
     * @throws ExevalatorError - Thrown if the specified variable has not been declared.
     */
    public writeVariable(name: string, value: number): void {
        if (name == null || name === undefined) {
            throw new ExevalatorError(ErrorMessages.ARGS_MUST_NOT_BE_NULL_OR_UNDEFINED.replace("$0", "name"));
        }
        if (StaticSettings.MAX_NAME_CHAR_COUNT < name.length || !this.variableTable.has(name)) {
            throw new ExevalatorError(ErrorMessages.VARIABLE_NOT_FOUND.replace("$0", name));
        }
        const address: number = this.variableTable.get(name)!;
        this.writeVariableAt(address, value);
    }

    /**
     * Writes the value to the variable at the specified virtual address.
     * This method is more efficient than "WriteVariable" method.
     *
     * @param address - The virtual address of the variable to be written.
     * @param value - The new value of the variable.
     * @throws ExevalatorError - Thrown if the specified address has not been asigned for any variable.
     */
    public writeVariableAt(address: number, value: number): void {
        if (address < 0 || this.memory.length <= address) {
            throw new ExevalatorError(ErrorMessages.INVALID_VARIABLE_ADDRESS.replace("$0", address.toString()));
        }
        address = ((address | 0) & ~(address >> 31)) & ((StaticSettings.MAX_VARIABLE_COUNT - 1) | 0);
        this.memory[address] = value;
    }

    /**
     * Reads the value of the variable having the specified name.
     *
     * @param name - The name of the variable to be read.
     * @returns The current value of the variable.
     * @throws ExevalatorError - Thrown if the specified variable has not been declared.
     */
    public readVariable(name: string): number {
        if (name == null || name === undefined) {
            throw new ExevalatorError(ErrorMessages.ARGS_MUST_NOT_BE_NULL_OR_UNDEFINED.replace("$0", "name"));
        }
        if (StaticSettings.MAX_NAME_CHAR_COUNT < name.length || !this.variableTable.has(name)) {
            throw new ExevalatorError(ErrorMessages.VARIABLE_NOT_FOUND.replace("$0", name));
        }
        const address: number = this.variableTable.get(name)!;
        return this.readVariableAt(address);
    }

    /**
     * Reads the value of the variable at the specified virtual address.
     * This method is more efficient than "ReadVariable" method.
     *
     * @param address - The virtual address of the variable to be read.
     * @returns The current value of the variable.
     * @throws ExevalatorError - Thrown if the specified address has not been asigned for any variable.
     */
    public readVariableAt(address: number): number {
        if (address < 0 || this.memory.length <= address) {
            throw new ExevalatorError(ErrorMessages.INVALID_VARIABLE_ADDRESS.replace("$0", address.toString()));
        }
        address = ((address | 0) & ~(address >> 31)) & ((StaticSettings.MAX_VARIABLE_COUNT - 1) | 0);
        return this.memory[address];
    }

    /**
     * Connects a function, for using it in expressions.
     *
     * @param name - The name of the function used in the expression.
     * @param functionImpl - The function to be connected.
     * @throws ExevalatorError - Thrown if the specified function name is invalid or already used.
     */
    public connectFunction(name: string, functionImpl: ExevalatorFunctionInterface): void {
        if (name == null || name === undefined) {
            throw new ExevalatorError(ErrorMessages.ARGS_MUST_NOT_BE_NULL_OR_UNDEFINED.replace("$0", "name"));
        }
        if (StaticSettings.MAX_NAME_CHAR_COUNT < name.length) {
            throw new ExevalatorError(
                ErrorMessages.TOO_LONG_FUNCTION_NAME.replace("$0", StaticSettings.MAX_NAME_CHAR_COUNT.toString())
            );
        }
        if (this.functionTable.has(name)) {
            throw new ExevalatorError(ErrorMessages.FUNCTION_ALREADY_CONNECTED.replace("$0", name));
        }
        this.functionTable.set(name, functionImpl);
    }
}


/**
 * The Error thrown if the input expression is syntactically incorrect,
 * or uses undeclared variables/functions.
 */
export class ExevalatorError extends Error {
    constructor(message: string) {
        super(message);
        this.name = "ExevalatorError";
    }
}

/**
 * The Error thrown if the implementation of Exevalator is incorrect.
 */
export class ExevalatorImplementationError extends Error {
    constructor(message: string) {
        super(message);
        this.name = "ExevalatorImplementationError";
    }
}


/**
 * The interface to implement functions available in expressions.
 */
export interface ExevalatorFunctionInterface {

    /**
     * Invokes the function.
     * 
     * @param args - An array storing values of arguments.
     * @returns The return value of the function.
     */
    invoke(args: number[]): number;
}


/**
 * The class performing functions of a lexical analyzer.
 */
class LexicalAnalyzer {

    /**
     * Splits (tokenizes) the expression into tokens, and analyze them.
     *
     * @param expression - The expression to be tokenized/analyzed.
     * @returns Analyzed tokens.
     * @throws ExevalatorError Thrown if the input exception is syntactically incorrect.
     */
    public static analyze(expression: string): Token[] {

        // Firstly, to simplify the tokenization,
        // replace number literals in the expression to the escaped representation: "@NUMBER_LITERAL",
        // because number literals may contains "+" or "-" in their exponent part.
        let numberLiteralList: string[] = [];
        expression = this.escapeNumberLiterals(expression, numberLiteralList);

        // Tokenize (split) the expression into token words.
        for (const splitter of StaticSettings.TOKEN_SPLITTER_SYMBOL_LIST) {
            const escapedSplitter = splitter.replace(/[.*+?^${}()|[\]\\]/g, '\\$&'); // Escape special characters in splitters
            const splitterRegex = new RegExp(escapedSplitter, 'g');
            expression = expression.replace(splitterRegex, ` ${splitter} `);
        }
        let tokenWords: string[] = expression.trim().split(/\s+/);

        // For an empty expression (containing no tokens), the above returns { "" }, not { }.
        // So we should detect/handle it as follows.
        if (tokenWords.length == 1 && tokenWords[0].length == 0) {
            throw new ExevalatorError(ErrorMessages.EMPTY_EXPRESSION);
        }

        // Checks the total number of tokens.
        if (StaticSettings.MAX_TOKEN_COUNT < tokenWords.length) {
            throw new ExevalatorError(ErrorMessages.TOO_MANY_TOKENS.replace("$0", StaticSettings.MAX_TOKEN_COUNT.toString()));
        }

        // Create Token instances.
        // Also, escaped number literals will be recovered.
        let tokens: Token[] = this.createTokensFromTokenWords(tokenWords, numberLiteralList);

        // Checks syntactic correctness of tokens of inputted expressions.
        this.checkParenthesisBalance(tokens);
        this.checkEmptyParentheses(tokens);
        this.checkLocationsOfOperatorsAndLeafs(tokens);
    
        return tokens;
    }

    /**
     * Creates Token-type instances from token words (String).
     *
     * @param tokenWords - Token words (String) to be converted to Token instances.
     * @param numberLiterals - The List storing number literals.
     * @returns Created Token instances.
     * @throws ExevalatorError Thrown if the input exception is syntactically incorrect.
     */
    private static createTokensFromTokenWords(tokenWords: string[], numberLiterals: string[]): Token[] {
        const tokenCount: number = tokenWords.length;

        // Stores the parenthesis-depth, which will increase at "(" and decrease at ")".
        let parenthesisDepth: number = 0;

        // Stores the parenthesis-depth when a function call operator begins,
        // for detecting the end of the function operator.
        let callParenthesisDepths: Set<number> = new Set<number>();

        let tokens: Token[] = new Array(tokenCount);
        let lastToken: Token | undefined = undefined;
        let iliteral: number = 0;
        for (let itoken: number = 0; itoken < tokenCount; itoken++) {
            const word: string = tokenWords[itoken];

            // Cases of open parentheses, or beginning of function calls.
            if (word === "(") {
                parenthesisDepth++;
                if (1 <= itoken && tokens[itoken - 1].type == TokenType.FUNCTION_IDENTIFIER) {
                    callParenthesisDepths.add(parenthesisDepth);
                    const op: Operator | undefined = StaticSettings.CALL_OPERATOR_SYMBOL_MAP.get(word.charAt(0));
                    tokens[itoken] = new Token(TokenType.OPERATOR, word, op);
                } else {
                    tokens[itoken] = new Token(TokenType.PARENTHESIS, word);
                }

            // Cases of closes parentheses, or end of function calls.
            } else if (word === ")") {
                if (callParenthesisDepths.has(parenthesisDepth)) {
                    callParenthesisDepths.delete(parenthesisDepth);
                    const op: Operator | undefined = StaticSettings.CALL_OPERATOR_SYMBOL_MAP.get(word.charAt(0));
                    tokens[itoken] = new Token(TokenType.OPERATOR, word, op);
                } else {
                    tokens[itoken] = new Token(TokenType.PARENTHESIS, word);
                }
                parenthesisDepth--;

            // Case of separators of function arguments:
            // they are handled as a special operator, for the algorithm of the parser of Exevalator.
            } else if (word === ",") {
                const op: Operator | undefined = StaticSettings.CALL_OPERATOR_SYMBOL_MAP.get(word.charAt(0));
                tokens[itoken] = new Token(TokenType.OPERATOR, word, op);

            // Cases of operators.
            } else if (word.length == 1 && StaticSettings.OPERATOR_SYMBOL_SET.has(word.charAt(0))) {
                let op: Operator | undefined = undefined;

                // Cases of unary-prefix operators.
                if (!lastToken
                        || lastToken.word === "("
                        || lastToken.word === ","
                        || (lastToken.type === TokenType.OPERATOR && lastToken.operator?.type != OperatorType.CALL) ) {

                    if (!StaticSettings.UNARY_PREFIX_OPERATOR_SYMBOL_MAP.has(word.charAt(0))) {
                        throw new ExevalatorError(ErrorMessages.UNKNOWN_UNARY_PREFIX_OPERATOR.replace("$0", word));
                    }
                    op = StaticSettings.UNARY_PREFIX_OPERATOR_SYMBOL_MAP.get(word.charAt(0));

                // Cases of binary operators.
                } else if (lastToken.word === ")"
                        || lastToken.type == TokenType.NUMBER_LITERAL
                        || lastToken.type == TokenType.VARIABLE_IDENTIFIER) {

                    if (!StaticSettings.BINARY_OPERATOR_SYMBOL_MAP.has(word.charAt(0))) {
                        throw new ExevalatorError(ErrorMessages.UNKNOWN_BINARY_OPERATOR.replace("$0", word));
                    }
                    op = StaticSettings.BINARY_OPERATOR_SYMBOL_MAP.get(word.charAt(0));

                } else {
                    throw new ExevalatorError(ErrorMessages.UNKNOWN_OPERATOR_SYNTAX.replace("$0", word));
                }
                tokens[itoken] = new Token(TokenType.OPERATOR, word, op);

            // Case of literals.
            } else if (word === StaticSettings.ESCAPED_NUMBER_LITERAL) {
                tokens[itoken] = new Token(TokenType.NUMBER_LITERAL, numberLiterals[iliteral]);
                iliteral++;

            // Cases of variable identifier of function identifier.
            } else {
                if (itoken < tokenCount - 1 && tokenWords[itoken + 1] === "(") {
                    tokens[itoken] = new Token(TokenType.FUNCTION_IDENTIFIER, word);
                } else {
                    tokens[itoken] = new Token(TokenType.VARIABLE_IDENTIFIER, word);
                }
            }
            lastToken = tokens[itoken];
        }
        return tokens;
    }

    /**
     * Replaces number literals in the expression to the escaped representation: "@NUMBER_LITERAL@".
     *
     * @param expression - The expression of which number literals are not escaped yet.
     * @param literalStoreList - The list to which number literals will be added.
     * @returns The expression in which number literals are escaped.
     */
    private static escapeNumberLiterals(expression: string, literalStoreList: string[]): string {

        // Create the regular expression pattern representing number literals.
        const numberLiteralPattern: RegExp = new RegExp(StaticSettings.NUMBER_LITERAL_REGEX, 'g');
        let match: RegExpExecArray | null = null;

        // Search the next number literal, and loops while any literals undetected yet exist.
        while ((match = numberLiteralPattern.exec(expression)) !== null) {
            const matchedLiteral: string = match[0]; // [0] is matched text, and [1], [2], ... are matched groups.
            literalStoreList.push(matchedLiteral);
        }

        // Replace all number literals in the expression to the escaped representation.
        const replacedExpression = expression.replace(numberLiteralPattern, StaticSettings.ESCAPED_NUMBER_LITERAL);
        return replacedExpression;
    }

    /**
     * Checks the number and correspondence of open "(" and closed ")" parentheses.
     * An ExevalatorError will be thrown when any errors detected.
     * If no error detected, nothing will occur.
     *
     * @param tokens - Tokens of the inputted expression.
     * @throws ExevalatorError Thrown if correspondence of open "(" and closed ")" parentheses is broken.
     */
    private static checkParenthesisBalance(tokens: Token[]): void {
        const tokenCount: number = tokens.length;
        let hierarchy: number = 0; // Increases at "(" and decreases at ")".

        for (let itoken: number = 0; itoken < tokenCount; itoken ++) {
            const token: Token = tokens[itoken];
            if (token.word === "(") {
                hierarchy++;
            } else if (token.word === ")") {
                hierarchy--;
            }

            // If the value of hierarchy is negative, the open parenthesis is deficient.
            if (hierarchy < 0) {
                throw new ExevalatorError(ErrorMessages.DEFICIENT_OPEN_PARENTHESIS);
            }
        }

        // If the value of hierarchy is not zero at the end of the expression,
        // the closed parentheses ")" is deficient.
        if (hierarchy > 0) {
            throw new ExevalatorError(ErrorMessages.DEFICIENT_CLOSED_PARENTHESIS);
        }
    }

    /**
     * Checks that empty parentheses "()" are not contained in the expression.
     * An ExevalatorError will be thrown when any errors detected.
     * If no error detected, nothing will occur.
     *
     * @param tokens - Tokens of the inputted expression.
     * @throws ExevalatorError Thrown if an empty parenthesis exists.
     */
    private static checkEmptyParentheses(tokens: Token[]): void {
        const tokenCount: number = tokens.length;
        let contentCounter: number = 0;
        for (let itoken: number = 0; itoken < tokenCount; itoken++) {
            const token: Token = tokens[itoken];
            if (token.type === TokenType.PARENTHESIS) { // Excepting CALL operators
                if (token.word === "(") {
                    contentCounter = 0;
                } else if (token.word === ")") {
                    if (contentCounter === 0) {
                        throw new ExevalatorError(ErrorMessages.EMPTY_PARENTHESIS);
                    }
                }
            } else {
                contentCounter++;
            }
        }
    }

    /**
     * Checks correctness of locations of operators and leaf elements (literals and identifiers).
     * An ExevalatorError will be thrown when any errors detected.
     * If no error detected, nothing will occur.
     *
     * @param tokens - Tokens of the inputted expression.
     * @throws ExevalatorError Thrown if an empty parenthesis exists.
     */
    private static checkLocationsOfOperatorsAndLeafs(tokens: Token[]): void {
        const tokenCount = tokens.length;
        const leafTypeSet: Set<TokenType> = new Set<TokenType>([
            TokenType.NUMBER_LITERAL, TokenType.VARIABLE_IDENTIFIER
        ]);

        // Reads and check tokens from left to right.
        for (let itoken: number = 0; itoken<tokenCount; itoken++) {
            const token: Token = tokens[itoken];

            // Prepare information of next/previous token.
            const nextIsLeaf: boolean = itoken != tokenCount - 1 && leafTypeSet.has(tokens[itoken + 1].type);
            const prevIsLeaf: boolean = itoken != 0 && leafTypeSet.has(tokens[itoken - 1].type);
            const nextIsOpenParenthesis: boolean = itoken < tokenCount - 1 && tokens[itoken + 1].word === "(";
            const prevIsCloseParenthesis: boolean = itoken != 0 && tokens[itoken - 1].word === ")";
            const nextIsPrefixOperator: boolean = itoken < tokenCount - 1
                    && tokens[itoken + 1].type === TokenType.OPERATOR
                    && tokens[itoken + 1].operator?.type === OperatorType.UNARY_PREFIX;
            const nextIsFunctionCallBegin: boolean = nextIsOpenParenthesis
                    && tokens[itoken + 1].type === TokenType.OPERATOR
                    && tokens[itoken + 1].operator?.type === OperatorType.CALL;
            const nextIsFunctionIdentifier: boolean = itoken < tokenCount-1
                    && tokens[itoken + 1].type == TokenType.FUNCTION_IDENTIFIER;

            // Case of operators
            if (token.type === TokenType.OPERATOR) {

                // Cases of unary-prefix operators
                if (token.operator?.type === OperatorType.UNARY_PREFIX) {

                    // Only leafs, open parentheses, unary-prefix and function-call operators can be an operand.
                    if ( !(nextIsLeaf || nextIsOpenParenthesis || nextIsPrefixOperator || nextIsFunctionIdentifier) ) {
                        throw new ExevalatorError(ErrorMessages.RIGHT_OPERAND_REQUIRED.replace("$0", token.word));
                    }
                } // Cases of unary-prefix operators

                // Cases of binary operators or a separator of partial expressions
                if (token.operator?.type === OperatorType.BINARY || token.word === ",") {

                    // Only leaf elements, open parenthesis, and unary-prefix operator can be a right-operand.
                    if( !(nextIsLeaf || nextIsOpenParenthesis || nextIsPrefixOperator || nextIsFunctionIdentifier) ) {
                        throw new ExevalatorError(ErrorMessages.RIGHT_OPERAND_REQUIRED.replace("$0", token.word));
                    }
                    // Only leaf elements and closed parenthesis can be a right-operand.
                    if( !(prevIsLeaf || prevIsCloseParenthesis) ) {
                        throw new ExevalatorError(ErrorMessages.LEFT_OPERAND_REQUIRED.replace("$0", token.word));
                    }
                } // Cases of binary operators or a separator of partial expressions

            } // Case of operators

            // Case of leaf elements
            if (leafTypeSet.has(token.type)) {

                // An other leaf element or an open parenthesis can not be at the right of an leaf element.
                if (!nextIsFunctionCallBegin && (nextIsOpenParenthesis || nextIsLeaf)) {
                    throw new ExevalatorError(ErrorMessages.RIGHT_OPERATOR_REQUIRED.replace("$0", token.word));
                }

                // An other leaf element or a closed parenthesis can not be at the left of an leaf element.
                if (prevIsCloseParenthesis || prevIsLeaf) {
                    throw new ExevalatorError(ErrorMessages.LEFT_OPERATOR_REQUIRED.replace("$0", token.word));
                }
            } // Case of leaf elements
        } // Loops for each token
    } // End of this method
}


/**
 * The class performing functions of a parser.
 */
class Parser {

    /**
     * Parses tokens and construct Abstract Syntax Tree (AST).
     *
     * @param tokens - Tokens to be parsed.
     * @return The root node of the constructed AST.
     */
    public static parse(tokens: Token[]): AstNode {

        /* In this method, we use a non-recursive algorithm for the parsing.
        * Processing cost is maybe O(N), where N is the number of tokens. */

        // Number of tokens
        const tokenCount: number = tokens.length;

        // Working stack to form multiple AstNode instances into a tree-shape.
        let stack: AstNode[] = [];

        // Temporary nodes used in the above working stack, for isolating ASTs of partial expressions.
        const parenthesisStackLid: AstNode = new AstNode(new Token(TokenType.STACK_LID, "(PARENTHESIS_STACK_LID)"));
        const separatorStackLid: AstNode = new AstNode(new Token(TokenType.STACK_LID, "(SEPARATOR_STACK_LID)"));
        const callBeginStackLid: AstNode = new AstNode(new Token(TokenType.STACK_LID, "(CALL_BEGIN_STACK_LID)"));

        // The array storing next operator's precedence for each token.
        // At [i], it is stored that the precedence of the first operator of which token-index is greater than i.
        let nextOperatorPrecedences: number[] = Parser.getNextOperatorPrecedences(tokens);

        // Read tokens from left to right.
        let itoken: number = 0;
        do {
            const token: Token = tokens[itoken];

            // Case of literals and identifiers: "1.23", "x", "f", etc.
            if (token.type === TokenType.NUMBER_LITERAL
                    || token.type === TokenType.VARIABLE_IDENTIFIER
                    || token.type === TokenType.FUNCTION_IDENTIFIER) {
                stack.push(new AstNode(token));
                itoken++; // To "continue" for the next token.
                continue;

            // Case of parenthesis: "(" or ")"
            } else if (token.type === TokenType.PARENTHESIS) {
                if (token.word === "(") {
                    stack.push(parenthesisStackLid);
                } else { // Case of ")"
                    const operatorNode: AstNode = Parser.popPartialExprNodes(stack, parenthesisStackLid)[0];
                    stack.push(operatorNode);
                    Parser.connectOperatorsInStack(stack, nextOperatorPrecedences[itoken]);
                }
                itoken++; // To "continue" for the next token.
                continue;

            // Case of operators: "+", "-", etc.
            } else if (token.type === TokenType.OPERATOR) {
                let operatorNode = new AstNode(token);
                const nextOpPrecedence: number = nextOperatorPrecedences[itoken];

                // Case of unary-prefix operators:
                // * Connect the node of right-token as an operand, if necessary (depending the next operator's precedence).
                if (token.operator?.type === OperatorType.UNARY_PREFIX) {
                    if (Parser.shouldAddRightTokenAsOperand(token.operator.associativity, token.operator.precedence, nextOpPrecedence)) {
                        operatorNode.childNodeList.push(new AstNode(tokens[itoken + 1]));
                        itoken++; // Because one token is looked ahead. Don't "continue" here.
                    } // else: Operand will be connected later. See the bottom of this loop.

                // Case of binary operators:
                // * Always connect the node of left-token as an operand.
                // * Connect the node of right-token as an operand, if necessary (depending the next operator's precedence).
                } else if (token.operator?.type === OperatorType.BINARY) {
                    operatorNode.childNodeList.push(stack.pop()!);
                    if (Parser.shouldAddRightTokenAsOperand(token.operator.associativity, token.operator.precedence, nextOpPrecedence)) {
                        operatorNode.childNodeList.push(new AstNode(tokens[itoken + 1]));
                        itoken++; // Because one token is looked ahead. Don't "continue" here.
                    } // else: Right-operand will be connected later. See the bottom of this loop.

                // Case of function-call operators.
                } else if (token.operator?.type === OperatorType.CALL) {
                    if (token.word === "(") {
                        operatorNode.childNodeList.push(stack.pop()!); // Add function-identifier node at the top of the stack.
                        stack.push(operatorNode);
                        stack.push(callBeginStackLid); // The marker to correct partial expressions of args from the stack.
                        itoken++; // To "continue" for the next token.
                        continue;
                    } else if (token.word === ")") {
                        const argNodes: AstNode[] = Parser.popPartialExprNodes(stack, callBeginStackLid);
                        operatorNode = stack.pop()!;
                        for (const argNode of argNodes) {
                            operatorNode.childNodeList.push(argNode);
                        }
                    } else if (token.word === ",") {
                        stack.push(separatorStackLid);
                        itoken++;
                        continue;
                    }
                }

                // Connects all operators in the stack of which precedence is higher than the next operator's precedence.
                stack.push(operatorNode);
                Parser.connectOperatorsInStack(stack, nextOperatorPrecedences[itoken]);
                itoken++;
                continue;

            } else {
                throw new ExevalatorImplementationError(`Unexpected token type: ${TokenType[token.type]}`);
            }

        } while (itoken < tokenCount);

        // The AST has been constructed on the stack, and only its root node is stored in the stack.
        const rootNodeOfExpressionAst: AstNode = stack.pop()!;

        // Check that the depth of the constructed AST does not exceeds the limit.
        rootNodeOfExpressionAst.checkDepth(1, StaticSettings.MAX_AST_DEPTH);

        return rootNodeOfExpressionAst;
    }

    /**
     * Judges whether the right-side token should be connected directly as an operand, to the target operator.
     *
     * @param targetOperatorAssociativity - The associativity of the target operator.
     * @param targetOperatorPrecedence - The precedence of the target operator (smaller value gives higher precedence).
     * @param nextOperatorPrecedence - The precedence of the next operator (smaller value gives higher precedence).
     * @return Returns true if the right-side token (operand) should be connected to the target operator.
     */
    private static shouldAddRightTokenAsOperand(
            targetOperatorAssociativity: OperatorAssociativity,
            targetOperatorPrecedence: number, nextOperatorPrecedence: number): boolean {

		// If the precedence of the target operator is stronger than the next operator, return true.
		// If the precedence of the next operator is stronger than the target operator, return false.
		// If the precedence of both operators is the same:
		//         Return true if the target operator is left-associative.
		//         Return false if the target operator is right-associative.

		const targetOpPrecedenceIsStrong: boolean = targetOperatorPrecedence < nextOperatorPrecedence; // Smaller value gives higher precedence.
		const targetOpPrecedenceIsEqual: boolean = targetOperatorPrecedence == nextOperatorPrecedence; // Smaller value gives higher precedence.
		const targetOpAssociativityIsLeft: boolean = targetOperatorAssociativity == OperatorAssociativity.LEFT;
		return targetOpPrecedenceIsStrong || (targetOpPrecedenceIsEqual && targetOpAssociativityIsLeft);
    }

    /**
     * Judges whether the right-side token should be connected directly as an operand,
     * to the operator at the top of the working stack.
     *
     * @param stack - The working stack used for the parsing.
     * @param nextOperatorPrecedence - The precedence of the next operator (smaller value gives higher precedence).
     * @return Returns true if the right-side token (operand) should be connected to the operator at the top of the stack.
     */
    private static shouldAddRightOperandToStackedOperator(stack: AstNode[], nextOperatorPrecedence: number): boolean {
        if (stack.length == 0) {
            return false;
        }
        let stackTopToken: Token = stack[stack.length - 1].token;
        if (stackTopToken.type != TokenType.OPERATOR || !stackTopToken.operator) {
            return false;
        }

        const operatorOnStackTop: Operator = stackTopToken.operator;
        return Parser.shouldAddRightTokenAsOperand(
            operatorOnStackTop.associativity, operatorOnStackTop.precedence, nextOperatorPrecedence
        );
    }

    /**
     * Connects all operators in the stack of which precedence is higher than the next operator's precedence.
     * 
     * @param stack - The working stack used for the parsing.
     * @param nextOperatorPrecedence - The precedence of the next operator (smaller value gives higher precedence).
     */
    private static connectOperatorsInStack(stack: AstNode[], nextOperatorPrecedence: number) {

        // Pop the current stack-top node.
        // Note that, it is not necessarily an operator-type node.
        // It can be a literal, for example, 
        // when the caller-side loop isparsing a partial expression in which only a literal (e.g.: "(1.23)").
        let stackTopNode: AstNode = stack.pop()!;
        
        // If the updated stack-top node is an operator-type node, and it is prior to the next operator, 
        // connect the previously popped stack-top node to the current stack-top node as an operand.
        // Repeat the above while the updated stack-top node is an operator-type and prior to the next operator. 
        while (Parser.shouldAddRightOperandToStackedOperator(stack, nextOperatorPrecedence)) {
            const operandNode: AstNode = stackTopNode;
            stackTopNode = stack.pop()!;
            if (stackTopNode.token.type !== TokenType.OPERATOR) {
                throw new ExevalatorImplementationError(
                    "The popped node must be an operator-type node because isRightOperandForStackTopOperator() returned true."
                );
            }
            stackTopNode.childNodeList.push(operandNode);
        }
        stack.push(stackTopNode!);
    }

    /**
     * Pops root nodes of ASTs of partial expressions constructed on the stack.
     * In the returned array, the popped nodes are stored in FIFO order.
     *
     * @param stack - The working stack used for the parsing.
     * @param endStackLidNode - The temporary node pushed in the stack, at the end of partial expressions to be popped.
     * @return Root nodes of ASTs of partial expressions.
     */
    private static popPartialExprNodes(stack: AstNode[], endStackLidNode: AstNode): AstNode[] {
        if (stack.length == 0) {
            throw new ExevalatorError(ErrorMessages.UNEXPECTED_PARTIAL_EXPRESSION);
        }
        let partialExprNodeList: AstNode[] = [];
        while(stack.length != 0) {
            if (stack[stack.length - 1].token.type === TokenType.STACK_LID) {
                let stackLidNode: AstNode | undefined = stack.pop();
                if (stackLidNode === endStackLidNode) {
                    break;
                }
            } else {
                let partialExprNode: AstNode | undefined = stack.pop();
                if (partialExprNode) {
                    partialExprNodeList.push(partialExprNode);
                }
            }
        }
        const nodeCount = partialExprNodeList.length;
        let partialExprNodes: AstNode[] = new Array(nodeCount);
        for (let inode: number = 0; inode<nodeCount; inode++) {
            partialExprNodes[inode] = partialExprNodeList[nodeCount - inode - 1]; // Storing elements in reverse order.
        }
        return partialExprNodes;
    }

    /**
     * Returns an array storing next operator's precedence for each token.
     * In the returned array, it will stored at [i] that
     * precedence of the first operator of which token-index is greater than i.
     *
     * @param tokens - All tokens to be parsed.
     * @return The array storing next operator's precedence for each token.
     */
    private static getNextOperatorPrecedences(tokens: Token[]): number[] {
        const tokenCount: number = tokens.length;
        let lastOperatorPrecedence: number = Number.MAX_SAFE_INTEGER; // least prior
        let nextOperatorPrecedences: number[] = new Array(tokenCount);

        for (let itoken: number = tokenCount - 1; 0 <= itoken; itoken--) {
            const token: Token = tokens[itoken];
            nextOperatorPrecedences[itoken] = lastOperatorPrecedence;

            if (token.type === TokenType.OPERATOR && token.operator) {
                lastOperatorPrecedence = token.operator.precedence;
            }

            if (token.type === TokenType.PARENTHESIS) {
                if (token.word === "(") {
                    lastOperatorPrecedence = 0; // most prior
                } else { // case of ")"
                    lastOperatorPrecedence = Number.MAX_SAFE_INTEGER; // least prior
                }
            }
        }
        return nextOperatorPrecedences;
    }
}


/**
 * The enum representing types of operators.
 */
enum OperatorType {

    /** Represents unary operator, for example: - of -1.23 */
    UNARY_PREFIX,

    /** Represents binary operator, for example: + of 1+2 */
    BINARY,

    /** Represents function-call operator */
    CALL
}


/**
 * The enum representing associativities of operators.
 */
enum OperatorAssociativity {

    /** Represents left-associative. */
    LEFT,

    /** Represents right-associative. */
    RIGHT
}


/**
 * The class storing information of an operator.
 */
class Operator {

    /** The symbol of this operator (for example: '+'). */
    public readonly symbol: string;

    /** The precedence of this operator (smaller value gives higher precedence). */
    public readonly precedence: number;

    /** The type of operator tokens. */
    public readonly type: OperatorType;

    /** The associativity of operator tokens. */
    public readonly associativity: OperatorAssociativity;

    /**
     * Create an Operator instance storing specified information.
     *
     * @param type - The type of this operator.
     * @param symbol - The symbol of this operator.
     * @param precedence - The precedence of this operator.
     * @param associativity - The associativity of this operator.
     */
    public constructor(type: OperatorType, symbol: string, precedence: number, associativity: OperatorAssociativity) {
        this.type = type;
        this.symbol = symbol;
        this.precedence = precedence;
        this.associativity = associativity;
    }

    /**
     * Returns the String representation of this Operator instance.
     */
    public toString(): string {
        return `Operator [symbol=${this.symbol}, type=${this.type}, precedence=${this.precedence}, associativity=${this.associativity}]`;
    }
}


/**
 * The enum representing types of tokens.
 */
enum TokenType {

    /** Represents number literal tokens, for example: 1.23 */
    NUMBER_LITERAL,

    /** Represents operator tokens, for example: + */
    OPERATOR,

    /** Represents parenthesis, for example: ( and ) of (1*(2+3)) */
    PARENTHESIS,

    /** Represents variable-identifier tokens, for example: x */
    VARIABLE_IDENTIFIER,

    /** Represents function-identifier tokens, for example: f */
    FUNCTION_IDENTIFIER,

    /** Represents temporary token for isolating partial expressions in the stack, in parser */
    STACK_LID
}


/**
 * The class storing information of an token.
 */
class Token {

    /** The type of this token. */
    public type: TokenType;

    /** The text representation of this token. */
    public word: string;

    /** The detailed information of the operator, if the type of this token is OPERATOR. */
    public operator?: Operator;

    /**
     * Create an Token instance storing specified information.
     *
     * @param type - The type of this token.
     * @param word - The text representation of this token.
     * @param operator - The detailed information of the operator, for OPERATOR type tokens.
     */
    public constructor(type: TokenType, word: string, operator?: Operator) {
        this.type = type;
        this.word = word;
        if (operator) {
            this.operator = operator;
        }
    }

    /**
     * Returns the String representation of this Token instance.
     */
    public toString(): string {
        if (!this.operator) {
            return `Token [type=${TokenType[this.type]}, word=${this.word}]`;
        } else {
            return `Token [type=${TokenType[this.type]}, word=${this.word}, ` +
                `operator.type=${OperatorType[this.operator.type]}, operator.precedence=${this.operator.precedence}]`;
        }
    }
}


/**
 * The class storing information of an node of an AST.
 */
class AstNode {

    /** The token corresponding with this AST node. */
    public token: Token;

    /** The list of child nodes of this AST node. */
    public childNodeList: AstNode[];

    /**
     * Create an AST node instance storing specified information.
     *
     * @param token - The token corresponding with this AST node
     */
    public constructor(token: Token) {
        this.token = token;
        this.childNodeList = [];
    }

    /**
     * Checks that depths in the AST of all nodes under this node (child nodes, grandchild nodes, and so on)
     * does not exceeds the specified maximum value.
     * An ExevalatorException will be thrown when the depth exceeds the maximum value.
     * If the depth does not exceeds the maximum value, nothing will occur.
     *
     * @param depthOfThisNode - The depth of this node in the AST.
     * @param maxAstDepth - The maximum value of the depth of the AST.
     */
    public checkDepth(depthOfThisNode: number, maxAstDepth: number): void {
        if (maxAstDepth < depthOfThisNode) {
            throw new ExevalatorError(ErrorMessages.EXCEEDS_MAX_AST_DEPTH.replace("$0", StaticSettings.MAX_AST_DEPTH.toString()));
        }
        for (const childNode of this.childNodeList) {
            childNode.checkDepth(depthOfThisNode + 1, maxAstDepth);
        }
    }

    /**
     * Expresses the AST under this node in XML-like text format.
     *
     * @param indentStage - The stage of indent of this node.
     * @return XML-like text representation of the AST under this node.
     */
    public toMarkuppedText(indentStage: number = 0): string {
        let indent: string = "";
        for (let istage: number = 0; istage<indentStage; istage++) {
            indent += StaticSettings.AST_INDENT;
        }
        const eol = "\n";

        let result: string = "";
        result += indent;
        result += "<";
        result += TokenType[this.token.type];
        result += " word=\"";
        result += this.token.word;
        result += "\"";
        if (this.token.type === TokenType.OPERATOR) {
            if (this.token.operator) {
                result += " optype=\"";
                result += OperatorType[this.token.operator.type];
                result += "\" precedence=\"";
                result += this.token.operator?.precedence;
                result += "\"";
            }
        }

        if (0 < this.childNodeList.length) {
            result += ">";
            for (const childNode of this.childNodeList) {
                result += eol;
                result += childNode.toMarkuppedText(indentStage + 1);
            }
            result += eol;
            result += indent;
            result += "</";
            result += TokenType[this.token.type];
            result += ">";

        } else {
            result += " />";
        }
        return result;
    }
}


/**
 * The class for evaluating the value of an AST.
 */
class Evaluator {

    /** The tree of evaluator nodes, which evaluates an expression. */
    private evaluatorNodeTree: EvaluatorNode | undefined = undefined;

    /**
     * Updates the state to evaluate the value of the AST.
     *
     * @param ast The root node of the AST.
     * @param variableTable - The Map mapping each variable name to an address of the variable.
     * @param functionTable - The Map mapping each function name to an IExevalatorFunction instance.
     */
    update(ast: AstNode, variableTable: Map<string, number>, functionTable: Map<string, ExevalatorFunctionInterface>): void {
        this.evaluatorNodeTree = Evaluator.createEvaluatorNodeTree(ast, variableTable, functionTable);
    }

    /**
     * Returns whether "evaluate" method is available on the current state.
     *
     * @returns Returns true if "evaluate" method is available.
     */
    public isEvaluatable(): boolean {
        if (this.evaluatorNodeTree) {
            return true;
        }
        return false;
    }

    /**
     * Evaluates the value of the AST set by "update" method.
     *
     * @param memory - The Vec used as as a virtual memory storing values of variables.
     * @returns The evaluated value.
     */
    public evaluate(memory: number[]): number {
        return this.evaluatorNodeTree!.evaluate(memory);
    }

    /**
     * Creates a tree of evaluator nodes corresponding with the specified AST.
     *
     * @param ast - The root node of the AST.
     * @param variableTable - The Map mapping each variable name to an address of the variable.
     * @param functionTable - The Map mapping each function name to an IExevalatorFunction instance.
     * @returns The root node of the created tree of evaluator nodes.
     */
    public static createEvaluatorNodeTree (
            ast: AstNode, variableTable: Map<string, number>, functionTable: Map<string, ExevalatorFunctionInterface>): EvaluatorNode {

        // Note: This method creates a tree of evaluator nodes by traversing each node in the AST recursively.

        const childNodeList: AstNode[] = ast.childNodeList;
        const childCount: number = childNodeList.length;

        // Creates evaluator nodes of child nodes, and store then into an array.
        let childNodeNodes: EvaluatorNode[] = new Array(childCount);
        for (let ichild: number = 0; ichild < childCount; ichild++) {
            const childAstNode: AstNode = childNodeList[ichild];
            const childEvaluatorNode: EvaluatorNode | undefined = Evaluator.createEvaluatorNodeTree(childAstNode, variableTable, functionTable);
            if (childEvaluatorNode) {
                childNodeNodes[ichild] = childEvaluatorNode;
            }
        }

        // Initialize evaluator nodes of this node.
        const token: Token = ast.token;
        if (token.type === TokenType.NUMBER_LITERAL) {
            return new NumberLiteralEvaluatorNode(token.word);
        } else if (token.type === TokenType.VARIABLE_IDENTIFIER) {
            if (!variableTable.has(token.word)) {
                throw new ExevalatorError(ErrorMessages.VARIABLE_NOT_FOUND.replace("$0", token.word));
            }
            const address = variableTable.get(token.word)!;
            return new VariableEvaluatorNode(address);
        } else if (token.type === TokenType.FUNCTION_IDENTIFIER) {
            return new NopEvaluatorNode();
        } else if (token.type === TokenType.OPERATOR) {
            const op: Operator = token.operator!;

            if (op.type === OperatorType.UNARY_PREFIX && op.symbol === "-") {
                return new MinusEvaluatorNode(childNodeNodes[0]);
            } else if (op.type === OperatorType.BINARY && op.symbol === "+") {
                return new AdditionEvaluatorNode(childNodeNodes[0], childNodeNodes[1]);
            } else if (op.type === OperatorType.BINARY && op.symbol === "-") {
                return new SubtractionEvaluatorNode(childNodeNodes[0], childNodeNodes[1]);
            } else if (op.type === OperatorType.BINARY && op.symbol === "*") {
                return new MultiplicationEvaluatorNode(childNodeNodes[0], childNodeNodes[1]);
            } else if (op.type === OperatorType.BINARY && op.symbol === "/") {
                return new DivisionEvaluatorNode(childNodeNodes[0], childNodeNodes[1]);
            } else if (op.type === OperatorType.CALL && op.symbol === "(") {
                const identifier: string = childNodeList[0].token.word;
                if (!functionTable.has(identifier)) {
                    throw new ExevalatorError(ErrorMessages.FUNCTION_NOT_FOUND.replace("$0", identifier));
                }
                const functionImpl: ExevalatorFunctionInterface = functionTable.get(identifier)!;
                const argCount: number = childCount - 1;
                let argNodes: EvaluatorNode[] = new Array(argCount);
                for (let iarg: number = 0; iarg<argCount; iarg++) {
                    argNodes[iarg] = childNodeNodes[iarg + 1];
                }
                return new FunctionEvaluatorNode(functionImpl, identifier, argNodes);
            } else {
                throw new ExevalatorError(ErrorMessages.UNEXPECTED_OPERATOR.replace("$0", op.symbol));
            }
        } else {
            throw new ExevalatorError(ErrorMessages.UNEXPECTED_TOKEN.replace("$0", TokenType[token.type]));
        }
    }
}

/**
 * The base class of evaluator nodes.
 */
abstract class EvaluatorNode {

    /**
     * Performs the evaluation.
     *
     * @param memory - The array storing values of variables.
     * @returns The evaluated value.
     */
    public abstract evaluate(memory: number[]): number;
}

/**
 * The placeholder node which performs nothing.
 */
class NopEvaluatorNode extends EvaluatorNode {

    /**
     * Performs nothing.
     *
     * @param memory - The array storing values of variables.
     * @returns Always returns NaN.
     */
    public evaluate(memory: number[]): number {
        return NaN;
    }
}

/**
 * The base class of evaluator nodes of binary operations.
 */
abstract class BinaryOperationEvaluatorNode extends EvaluatorNode {

    /**
     * Initializes operands.
     *
     * @param leftOperandNode - The node for evaluating the left-side operand
     * @param rightOperandNode - The node for evaluating the right-side operand
     */
    protected constructor(
            protected leftOperandNode: EvaluatorNode, 
            protected rightOperandNode: EvaluatorNode
    ) {
        super();
    }
}

/**
 * The evaluator node for evaluating the value of a addition operator.
 */
class AdditionEvaluatorNode extends BinaryOperationEvaluatorNode {

    /**
     * Initializes operands.
     *
     * @param leftOperandNode - The node for evaluating the left-side operand.
     * @param rightOperandNode - The node for evaluating the right-side operand.
     */
    public constructor(leftOperandNode: EvaluatorNode, rightOperandNode: EvaluatorNode) {
        super(leftOperandNode, rightOperandNode);
    }

    /**
     * Performs the addition.
     *
     * @param memory - The array storing values of variables.
     * @return The result value of the addition.
     */
    public evaluate(memory: number[]): number {
        return this.leftOperandNode.evaluate(memory) + this.rightOperandNode.evaluate(memory);
    }
}

/**
 * The evaluator node for evaluating the value of a subtraction operator.
 */
class SubtractionEvaluatorNode extends BinaryOperationEvaluatorNode {

    /**
     * Initializes operands.
     *
     * @param leftOperandNode - The node for evaluating the left-side operand.
     * @param rightOperandNode - The node for evaluating the right-side operand.
     */
    public constructor(leftOperandNode: EvaluatorNode, rightOperandNode: EvaluatorNode) {
        super(leftOperandNode, rightOperandNode);
    }

    /**
     * Performs the subtraction.
     *
     * @param memory - The array storing values of variables.
     * @return The result value of the subtraction.
     */
    public evaluate(memory: number[]): number {
        return this.leftOperandNode.evaluate(memory) - this.rightOperandNode.evaluate(memory);
    }
}

/**
 * The evaluator node for evaluating the value of a multiplication operator.
 */
class MultiplicationEvaluatorNode extends BinaryOperationEvaluatorNode {

    /**
     * Initializes operands.
     *
     * @param leftOperandNode - The node for evaluating the left-side operand.
     * @param rightOperandNode - The node for evaluating the right-side operand.
     */
    public constructor(leftOperandNode: EvaluatorNode, rightOperandNode: EvaluatorNode) {
        super(leftOperandNode, rightOperandNode);
    }

    /**
     * Performs the multiplication.
     *
     * @param memory - The array storing values of variables.
     * @return The result value of the multiplication.
     */
    public evaluate(memory: number[]) {
        return this.leftOperandNode.evaluate(memory) * this.rightOperandNode.evaluate(memory);
    }
}

/**
 * The evaluator node for evaluating the value of a division operator.
 */
class DivisionEvaluatorNode extends BinaryOperationEvaluatorNode {

    /**
     * Initializes operands.
     *
     * @param leftOperandNode - The node for evaluating the left-side operand.
     * @param rightOperandNode - The node for evaluating the right-side operand.
     */
    public constructor(leftOperandNode: EvaluatorNode, rightOperandNode: EvaluatorNode) {
        super(leftOperandNode, rightOperandNode);
    }

    /**
     * Performs the division.
     *
     * @param memory - The array storing values of variables.
     * @return The result value of the division.
     */
    public evaluate(memory: number[]): number {
        return this.leftOperandNode.evaluate(memory) / this.rightOperandNode.evaluate(memory);
    }
}

/**
 * The evaluator node for evaluating the value of a unary-minus operator.
 */
class MinusEvaluatorNode extends EvaluatorNode {

    /**
     * Initializes the operand.
     *
     * @param operandNode The node for evaluating the operand
     */
    public constructor(private operandNode: EvaluatorNode) {
        super();
    }

    /**
     * Performs the division.
     *
     * @param memory - The array storing values of variables.
     * @return The result value of the division.
     */
    public evaluate(memory: number[]): number {
        return -this.operandNode.evaluate(memory);
    }
}

/**
 * The evaluator node for evaluating the value of a number literal.
 */
class NumberLiteralEvaluatorNode extends EvaluatorNode {

    /** The value of the number literal. */
    private value: number;

    /**
     * Initializes the value of the number literal.
     *
     * @param literal The number literal.
     * @throws ExevalatorError - Thrown if it failed to convert the literal to a numeric value.
     */
    public constructor(literal: string) {
        super();

        //this.value = parseFloat(literal); // This conversion is ambiguous a little.
        this.value = Number(literal); // This is more strict than the above, but it may convert to an integer.
        if (isNaN(this.value)) {
            throw new ExevalatorError(ErrorMessages.INVALID_NUMBER_LITERAL.replace("$0", literal));
        }
        this.value += 0.0; // if the value is an integer, convert it to a floating point number.
    }

    /**
     * Returns the value of the number literal.
     *
     * @param memory - The array storing values of variables.
     * @return The value of the number literal.
     */
    public evaluate(memory: number[]): number {
        return this.value;
    }
}

/**
 * The evaluator node for evaluating the value of a variable.
 */
class VariableEvaluatorNode extends EvaluatorNode {

    /**
     * Initializes the address of the variable.
     *
     * @param address - The address of the variable.
     */
    public constructor(private address: number) {
        super();
        if (address < 0 || address > 0xFFFFFFFF || !Number.isInteger(address)) {
            throw new ExevalatorError(ErrorMessages.ADDRESS_MUST_BE_ZERO_OR_POSITIVE_INT32.replace("$0", address.toString()));
        }
    }

    /**
     * Returns the value of the variable.
     *
     * @param memory - The array storing values of variables.
     * @return The value of the variable.
     */
    public evaluate(memory: number[]): number {
        if (this.address < 0 || memory.length <= this.address) {
            throw new ExevalatorError(ErrorMessages.INVALID_MEMORY_ADDRESS.replace("$0", this.address.toString()));
        }
        this.address = ((this.address | 0) & ~(this.address >> 31)) & ((StaticSettings.MAX_VARIABLE_COUNT - 1) | 0);
        return memory[this.address];
    }
}

/**
 * The evaluator node for evaluating a function-call operator.
 *
 */
class FunctionEvaluatorNode extends EvaluatorNode {

    /** An array storing evaluated values of arguments. */
    private argumentArrayBuffer: number[];

    /**
     * Initializes information of functions to be called.
     *
     * @param functionImpl - The function to be called.
     * @param functionName - The name of the function.
     * @param argumentEvalNodes - Evaluator nodes for evaluating values of arguments.
     */
    public constructor(
            private functionImpl: ExevalatorFunctionInterface,
            private functionName: string,
            private argumentEvalNodes: EvaluatorNode[]
    ) {
        super();
        this.argumentArrayBuffer = Array(this.argumentEvalNodes.length);
    }

    /**
     * Calls the function and returns the returned value of the function.
     *
     * @param memory - The array storing values of variables.
     * @return The returned value of the function.
     * @throws ExevalatorError - Thrown if any error occurred while the function is being executed.
     */
    public evaluate(memory: number[]): number {
        const argCount: number = this.argumentEvalNodes.length;
        for (let iarg: number = 0; iarg < argCount; iarg++) {
            this.argumentArrayBuffer[iarg] = this.argumentEvalNodes[iarg].evaluate(memory);
        }
        try {
            return this.functionImpl.invoke(this.argumentArrayBuffer);
        } catch (error) {
            const errorMessage = error instanceof Error ? error.message : String(error);
            throw new ExevalatorError(ErrorMessages.FUNCTION_ERROR.replace("$0", this.functionName).replace("$1", errorMessage));
        }
    }
}

/**
 * The class defining static setting values.
 */
export class StaticSettings {

    /** The maximum number of characters in an expression. */
    public static readonly MAX_EXPRESSION_CHAR_COUNT: number = 256;

    /** The maximum number of characters of variable/function names. */
    public static readonly MAX_NAME_CHAR_COUNT: number = 64;

    /** The maximum number of tokens in an expression. */
    public static readonly MAX_TOKEN_COUNT: number = 64;

    /** The maximum depth of an Abstract Syntax Tree (AST). */
    public static readonly MAX_AST_DEPTH: number = 32;

    /** The maximum number of variables. */
    public static readonly MAX_VARIABLE_COUNT = 2**10; // Must be in the form of 2**n and smaller than 2147483647 + 1
    // !!!!! IMPORTANT !!!!!
    // When you modified the above value, you should run "test.rs".

    /** The indent used in text representations of ASTs. */
    public static readonly AST_INDENT: string = "  ";

    /** The regular expression of number literals. */
    public static readonly NUMBER_LITERAL_REGEX: string =
        "(?<=(\\s|\\+|-|\\*|/|\\(|\\)|,|^))" + // Token splitters or start of expression
        "([0-9]+(\\.[0-9]+)?)" +               // Significand part
        "((e|E)(\\+|-)?[0-9]+)?";              // Exponent part

    /** The escaped representation of number literals in expressions */
    public static readonly ESCAPED_NUMBER_LITERAL: string = "@NUMBER_LITERAL@";

    /** The set of symbols of available operators. */
    public static readonly OPERATOR_SYMBOL_SET: Set<string> = new Set<string>([
        "+", "-", "*", "/", "(", ")", ","
    ]);

    /** The Map mapping each symbol of an unary-prefix operator to an instance of Operator class. */
    public static readonly UNARY_PREFIX_OPERATOR_SYMBOL_MAP: Map<string, Operator> = new Map<string, Operator>([
        ["-", new Operator(OperatorType.UNARY_PREFIX, '-', 200, OperatorAssociativity.RIGHT)] // unary-minus operator
    ]);

    /** The Map mapping each symbol of an binary operator to an instance of Operator class. */
    public static readonly BINARY_OPERATOR_SYMBOL_MAP: Map<string, Operator> = new Map<string, Operator>([
        ["+", new Operator(OperatorType.BINARY, '+', 400, OperatorAssociativity.LEFT)], // addition operator
        ["-", new Operator(OperatorType.BINARY, '-', 400, OperatorAssociativity.LEFT)], // subtraction operator
        ["*", new Operator(OperatorType.BINARY, '*', 300, OperatorAssociativity.LEFT)], // multiplication operator
        ["/", new Operator(OperatorType.BINARY, '/', 300, OperatorAssociativity.LEFT)]  // division operator
    ]);

    /** The Map mapping each symbol of an call operator to an instance of Operator class. */
    public static readonly CALL_OPERATOR_SYMBOL_MAP: Map<string, Operator> = new Map<string, Operator>([
        ["(", new Operator(OperatorType.CALL, "(", 100, OperatorAssociativity.LEFT)],                     // call-begin operator
        [")", new Operator(OperatorType.CALL, ")", Number.MAX_SAFE_INTEGER, OperatorAssociativity.LEFT)], // call-end operator, least prior
        [",", new Operator(OperatorType.CALL, ",", Number.MAX_SAFE_INTEGER, OperatorAssociativity.LEFT)]  // call-separator operator, least prior
    ]);

    /** The list of symbols to split an expression into tokens. */
    public static readonly TOKEN_SPLITTER_SYMBOL_LIST: string[] = [
        "+", "-", "*", "/", "(", ")", ","
    ];
}
