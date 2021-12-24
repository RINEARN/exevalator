/*
 * Copyright(C) 2021 RINEARN (Fumihiro Matsui)
 * This software is released under the MIT License.
 */

/*
 * Put this code in your source code folder, and write the package-statement as follows.
 */
// package your.projects.package.anywhere;

import java.util.List;
import java.util.ArrayList;
import java.util.Deque;
import java.util.ArrayDeque;
import java.util.Set;
import java.util.HashSet;
import java.util.EnumSet;
import java.util.Map;
import java.util.concurrent.ConcurrentHashMap;
import java.util.regex.Matcher;
import java.util.regex.Pattern;
import java.lang.reflect.Field;
import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;


/**
 * Interpreter Engine of Exevalator.
 */
public final class Exevalator {

	/** The interconnect providing objects shared among multiple components of this interpreter. */
	private volatile Interconnect interconnect;

    /** Caches the content of the expression evaluated last time, to skip re-parsing. */
	private volatile String lastEvaluatedExpression;

    /** The unit for evaluating an expression. */
	private volatile Evaluator.EvaluatorUnit evaluatorUnit;

	/**
	 * Creates a new interpreter of the Exevalator.
	 */
	public Exevalator() {
		this.interconnect = new Interconnect();
		this.lastEvaluatedExpression = null;
		this.evaluatorUnit = null;
	}

	/**
	 * Evaluates (computes) the value of an expression.
	 *
	 * @param expression The expression to be evaluated
	 * @return The evaluated value
	 */
	public double eval(String expression) {

		boolean expressionChanged = expression != this.lastEvaluatedExpression
			&& !expression.equals(this.lastEvaluatedExpression);

        // If the expression changed from the last-evaluated expression, re-parsing is necessary.
		if (this.evaluatorUnit == null || expressionChanged) {

			// Split the expression into tokens, and analyze them.
			Token[] tokens = LexicalAnalyzer.analyze(expression);

			/*
			// Temporary, for debugging tokens
			for (Token token: tokens) {
				System.out.println(token.toString());
			}
			*/

			// Construct AST (Abstract Syntax Tree) by parsing tokens.
			AstNode ast = Parser.parse(tokens);

			/*
			// Temporary, for debugging AST
			System.out.println(ast.toString());
			*/

			// Evaluate (compute) the value of the root node of the AST.
			this.evaluatorUnit = ast.createEvaluatorUnit(this.interconnect);

			this.lastEvaluatedExpression = expression;
		}

        // Evaluate the value of the expression, and return it.
		double evaluatedValue = this.evaluatorUnit.evaluate();
		return evaluatedValue;
	}

	/**
	 * Connects the variable for accessing to it in the expression.
	 *
	 * @param variable The variable to be connected.
	 */
	public synchronized void connectVariable(AbstractVariable variable) {
		this.interconnect.connectVariable(variable);
	}

	/**
	 * Connects the static field for accessing to it in the expression as a variable.
	 *
	 * @param field The static field to be connected as a variable.
	 */
	public synchronized void connectFieldAsVariable(Field field) {
		this.interconnect.connectVariable(new FieldToVariableAdapter(field, null));
	}

	/**
	 * Connects the non-static field for accessing to it in the expression as a variable.
	 *
	 * @param field The non-static field to be connected as a variable.
	 * @param objectInstance The instance of the object to which the field belongs.
	 */
	public synchronized void connectFieldAsVariable(Field field, Object objectInstance) {
		this.interconnect.connectVariable(new FieldToVariableAdapter(field, objectInstance));
	}

	/**
	 * Disconnects the variable having the specified name.
	 *
	 * @param variableName The name of the variable to be disconnected.
	 */
	public synchronized void disconnectVariable(String variableName) {
		this.interconnect.disconnectVariable(variableName);
	}

	/**
	 * Disconnects all variables.
	 */
	public synchronized void disconnectAllVariables() {
		this.interconnect.disconnectAllVariables();
	}

	/**
	 * Connects the function for calling it in the expression.
	 *
	 * @param function The function to be connected.
	 */
	public synchronized void connectFunction(AbstractFunction function) {
		this.interconnect.connectFunction(function);
	}

	/**
	 * Connects the static method for calling it in the expression as a function.
	 *
	 * @param method The method to be connected as a function.
	 */
	public synchronized void connectMethodAsFunction(Method method) {
		this.interconnect.connectFunction(new MethodToFunctionAdapter(method, null));
	}

	/**
	 * Connects the non-static method for calling it in the expression as a function.
	 *
	 * @param method The method to be connected as a function.
	 * @param objectInstance The instance of the object to which the method belongs.
	 */
	public synchronized void connectMethodAsFunction(Method method, Object objectInstance) {
		this.interconnect.connectFunction(new MethodToFunctionAdapter(method, objectInstance));
	}

	/**
	 * Disconnects the function having the specified information.
	 *
	 * @param functionName The name of the function to be disconnected.
	 * @param parameterCount The number of parameters of the function to be disconnected.
	 */
	public synchronized void disconnectFunction(String functionName, int parameterCount) {
		this.interconnect.disconnectFunction(functionName, parameterCount);
	}

	/**
	 * Disconnects all functions.
	 */
	public synchronized void disconnectAllFunctions() {
		this.interconnect.disconnectAllFunctions();
	}

	/**
	 * The Exception class thrown in/by this engine.
	 */
	@SuppressWarnings("serial")
	public static final class ExevalatorException extends RuntimeException {

		/**
		 * Create an instance having the specified error message.
		 *
		 * @param errorMessage The error message explaining the cause of this exception
		 */
		public ExevalatorException(String errorMessage) {
			super(errorMessage);
		}

		/**
		 * Create an instance having the specified error message and the cause exception.
		 *
		 * @param errorMessage The error message explaining the cause of this exception
		 * @param causeException The cause exception of this exception
		 */
		public ExevalatorException(String errorMessage, Exception causeException) {
			super(errorMessage, causeException);
		}
	}
}


/**
 * The class performing functions of a lexical analyzer.
 */
final class LexicalAnalyzer {

	/**
	 * Splits (tokenizes) the expression into tokens, and analyze them.
	 *
	 * @param expression The expression to be tokenized/analyzed
	 * @return Analyzed tokens
	 */
	public static Token[] analyze(String expression) {

		// Firstly, to simplify the tokenization,
		// replace number literals in the expression to the escaped representation: "@NUMBER_LITERAL",
		// because number literals may contains "+" or "-" in their exponent part.
		List<String> numberLiteralList = new ArrayList<String>();
		expression = escapeNumberLiterals(expression, numberLiteralList);

		// Tokenize (split) the expression into token words.
		for (char splitter: StaticSettings.TOKEN_SPLITTER_SYMBOL_LIST) {
			expression = expression.replace(Character.toString(splitter), " " + splitter + " ");
		}
		String[] tokenWords = expression.trim().split("\\s+");

		// Create Token instances.
		// Also, escaped number literals will be recovered.
		Token[] tokens = createTokensFromTokenWords(tokenWords, numberLiteralList);

		// Checks syntactic correctness of tokens of inputted expressions.
		checkParenthesisOpeningClosings(tokens);
		checkEmptyParentheses(tokens);
		checkLocationsOfOperatorsAndLeafs(tokens);

		return tokens;
	}

	/**
	 * Creates Token-type instances from token words (String).
	 *
	 * @param tokenWords Token words (String) to be converted to Token instances
	 * @param numberLiteralList The List storing number literals.
	 * @return Created Token instances
	 */
	private static Token[] createTokensFromTokenWords(String[] tokenWords, List<String> numberLiterals) {
		int tokenCount = tokenWords.length;

		// Stores the parenthesis-depth, which will increase at "(" and decrease at ")".
		int parenthesisDepth = 0;

		// Stores the parenthesis-depth when a function call operator begins,
		// for detecting the end of the function operator.
		Set<Integer> callParenthesisDepths = new HashSet<Integer>();

		Token[] tokens = new Token[tokenCount];
		Token lastToken = null;
		int iliteral = 0;
		for (int itoken=0; itoken<tokenCount; itoken++) {
			String word = tokenWords[itoken];

			// Cases of open parentheses, or beginning of function calls.
			if (word.equals("(")) {
				parenthesisDepth++;
				if (1 <= itoken && tokens[itoken - 1].type == TokenType.FUNCTION_IDENTIFIER) {
					callParenthesisDepths.add(parenthesisDepth);
					Operator op = StaticSettings.CALL_OPERATOR_SYMBOL_MAP.get(word.charAt(0));
					tokens[itoken] = new Token(TokenType.OPERATOR, word, op);
				} else {
					tokens[itoken] = new Token(TokenType.PARENTHESIS, word);
				}

			// Cases of closes parentheses, or end of function calls.
			} else if (word.equals(")")) {
				if (callParenthesisDepths.contains(parenthesisDepth)) {
					callParenthesisDepths.remove(parenthesisDepth);
					Operator op = StaticSettings.CALL_OPERATOR_SYMBOL_MAP.get(word.charAt(0));
					tokens[itoken] = new Token(TokenType.OPERATOR, word, op);
				} else {
					tokens[itoken] = new Token(TokenType.PARENTHESIS, word);
				}
				parenthesisDepth--;

			// Cases of operators.
			} else if (word.length() == 1 && StaticSettings.OPERATOR_SYMBOL_SET.contains(word.charAt(0))) {
				Operator op = null;

				// Cases of unary-prefix operators.
				if (lastToken == null
						|| lastToken.word.equals("(")
						|| (lastToken.type == TokenType.OPERATOR && lastToken.operator.type != OperatorType.CALL) ) {

					if (!StaticSettings.UNARY_PREFIX_OPERATOR_SYMBOL_MAP.containsKey(word.charAt(0))) {
						throw new Exevalator.ExevalatorException("Unknown unary-prefix operator: " + word);
					}
					op = StaticSettings.UNARY_PREFIX_OPERATOR_SYMBOL_MAP.get(word.charAt(0));

				// Cases of binary operators.
				} else if (lastToken.word.equals(")")
						|| lastToken.type == TokenType.NUMBER_LITERAL
						|| lastToken.type == TokenType.VARIABLE_IDENTIFIER) {

					if (!StaticSettings.BINARY_OPERATOR_SYMBOL_MAP.containsKey(word.charAt(0))) {
						throw new Exevalator.ExevalatorException("Unknown binary operator: " + word);
					}
					op = StaticSettings.BINARY_OPERATOR_SYMBOL_MAP.get(word.charAt(0));

				} else {
					throw new Exevalator.ExevalatorException("Unexpected operator syntax: " + word);
				}
				tokens[itoken] = new Token(TokenType.OPERATOR, word, op);

				// Cases of literals, and separator.
			} else if (word.equals(StaticSettings.ESCAPED_NUMBER_LITERAL)) {
				tokens[itoken] = new Token(TokenType.NUMBER_LITERAL, numberLiterals.get(iliteral));
				iliteral++;
			} else if (word.equals(",")) {
				tokens[itoken] = new Token(TokenType.EXPRESSION_SEPARATOR, word);

			// Cases of variable identifier of function identifier.
			} else {
				if (itoken < tokenCount - 1 && tokenWords[itoken + 1].equals("(")) {
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
	 * @param expression The expression of which number literals are not escaped yet
	 * @param literalStoreList The list to which number literals will be added
	 * @return The expression in which number literals are escaped.
	 */
	private static String escapeNumberLiterals(String expression, List<String> literalStoreList) {
		Pattern numberLiteralPattern = Pattern.compile(StaticSettings.NUMBER_LITERAL_REGEX);
		Matcher numberLiteralMatcher = numberLiteralPattern.matcher(expression);

		// Search the next number literal, and loops while any literals undetected yet exist.
		while(numberLiteralMatcher.find()) {

			// Extract a number literal detected by the above searching.
			String matchedLiteral = numberLiteralMatcher.group();

			// Add the extracted literal to the List specified to the argument.
			literalStoreList.add(matchedLiteral);
		}

		// Replace all number literals in the expression to the escaped representation.
		numberLiteralMatcher.reset();
		String replacedExpression = numberLiteralMatcher.replaceAll(StaticSettings.ESCAPED_NUMBER_LITERAL);
		return replacedExpression;
	}

	/**
	 * Checks the number and correspondence of open "(" / closed ")" parentheses.
	 * An ExevalatorException will be thrown when any errors detected.
	 * If no error detected, nothing will occur.
	 *
	 * @param tokens Tokens of the inputted expression.
	 */
	private static void checkParenthesisOpeningClosings(Token[] tokens) {
		int tokenCount = tokens.length;
		int hierarchy = 0; // Increases at "(" and decreases at ")".

		for (int tokenIndex=0; tokenIndex<tokenCount; tokenIndex++) {
			Token token = tokens[tokenIndex];
			if (token.word.equals("(")) {
				hierarchy++;
			} else if (token.word.equals(")")) {
				hierarchy--;
			}

			// If the value of hierarchy is negative, the open parenthesis is deficient.
			if (hierarchy < 0) {
				throw new Exevalator.ExevalatorException(
					"The number of open parenthesis \"(\" is deficient."
				);
			}
		}

		// If the value of hierarchy is not zero at the end of the expression,
		// the closed parentheses ")" is deficient.
		if (hierarchy > 0) {
			throw new Exevalator.ExevalatorException(
				"The number of closed parenthesis \")\" is deficient."
			);
		}
	}

	/**
	 * Checks that empty parentheses "()" are not contained in the expression.
	 * An ExevalatorException will be thrown when any errors detected.
	 * If no error detected, nothing will occur.
	 *
	 * @param tokens Tokens of the inputted expression.
	 */
	private static void checkEmptyParentheses(Token[] tokens) {
		int tokenCount = tokens.length;
		int contentCounter = 0;
		for (int tokenIndex=0; tokenIndex<tokenCount; tokenIndex++) {
			Token token = tokens[tokenIndex];
			if (token.type == TokenType.PARENTHESIS) { // Excepting CALL operators
				if (token.word.equals("(")) {
					contentCounter = 0;
				} else if (token.word.equals(")")) {
					if (contentCounter == 0) {
						throw new Exevalator.ExevalatorException(
							"The content parentheses \"()\" should not be empty (excluding function calls)."
						);
					}
				}
			} else {
				contentCounter++;
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
	private static void checkLocationsOfOperatorsAndLeafs(Token[] tokens) {
		int tokenCount = tokens.length;
		Set<TokenType> leafTypeSet = EnumSet.noneOf(TokenType.class);
		leafTypeSet.add(TokenType.NUMBER_LITERAL);
		leafTypeSet.add(TokenType.VARIABLE_IDENTIFIER);

		// Reads and check tokens from left to right.
		for (int tokenIndex=0; tokenIndex<tokenCount; tokenIndex++) {
			Token token = tokens[tokenIndex];

			// Prepare information of next/previous token.
			boolean nextIsLeaf = tokenIndex!=tokenCount-1 && leafTypeSet.contains(tokens[tokenIndex+1].type);
			boolean prevIsLeaf = tokenIndex!=0 && leafTypeSet.contains(tokens[tokenIndex-1].type);
			boolean nextIsOpenParenthesis = tokenIndex < tokenCount-1 && tokens[tokenIndex+1].word.equals("(");
			boolean prevIsCloseParenthesis = tokenIndex != 0 && tokens[tokenIndex-1].word.equals(")");
			boolean nextIsPrefixOperator = tokenIndex < tokenCount-1
					&& tokens[tokenIndex+1].type == TokenType.OPERATOR
					&& tokens[tokenIndex+1].operator.type == OperatorType.UNARY_PREFIX;
			boolean nextIsFunctionCallBegin = nextIsOpenParenthesis
					&& tokens[tokenIndex+1].type == TokenType.OPERATOR
					&& tokens[tokenIndex+1].operator.type == OperatorType.CALL;
			boolean nextIsFunctionIdentifier = tokenIndex < tokenCount-1
					&& tokens[tokenIndex+1].type == TokenType.FUNCTION_IDENTIFIER;

			// Case of operators
			if (token.type == TokenType.OPERATOR) {

				// Cases of unary-prefix operators
				if (token.operator.type == OperatorType.UNARY_PREFIX) {

					// Only leafs, open parentheses, and unary-prefix operators can be operands.
					if ( !(  nextIsLeaf || nextIsOpenParenthesis || nextIsPrefixOperator  ) ) {
						throw new Exevalator.ExevalatorException("An operand is required at the right of: \"" + token.word + "\"");
					}
				} // Cases of unary-prefix operators

				// Cases of binary operators or a separator of partial expressions
				if (token.operator.type == OperatorType.BINARY || token.word.equals(",")) {

					// Only leaf elements, open parenthesis, and unary-prefix operator can be a right-operand.
					if( !(  nextIsLeaf || nextIsOpenParenthesis || nextIsPrefixOperator || nextIsFunctionIdentifier ) ) {
						throw new Exevalator.ExevalatorException("An operand is required at the right of: \"" + token.word + "\"");
					}
					// Only leaf elements and closed parenthesis can be a right-operand.
					if( !(  prevIsLeaf || prevIsCloseParenthesis  ) ) {
						throw new Exevalator.ExevalatorException("An operand is required at the left of: \"" + token.word + "\"");
					}
				} // Cases of binary operators or a separator of partial expressions

			} // Case of operators

			// Case of leaf elements
			if (leafTypeSet.contains(token.type)) {

				// An other leaf element or an open parenthesis can not be at the right of an leaf element.
				if (!nextIsFunctionCallBegin && (nextIsOpenParenthesis || nextIsLeaf)) {
					throw new Exevalator.ExevalatorException("An operator is required at the right of: \"" + token.word + "\"");
				}

				// An other leaf element or a closed parenthesis can not be at the left of an leaf element.
				if (prevIsCloseParenthesis || prevIsLeaf) {
					throw new Exevalator.ExevalatorException("An operator is required at the left of: \"" + token.word + "\"");
				}
			} // Case of leaf elements
		} // Loops for each token
	} // End of this method
}


/**
 * The class performing functions of a parser.
 */
final class Parser {

	/**
	 * Parses tokens and construct Abstract Syntax Tree (AST).
	 *
	 * @param tokens Tokens to be parsed
	 * @return The root node of the constructed AST
	 */
	public static AstNode parse(Token[] tokens) {

		/* In this method, we use a non-recursive algorithm for the parsing.
		* Processing cost is maybe O(N), where N is the number of tokens. */

		// Number of tokens
		int tokenCount = tokens.length;

		// Working stack to form multiple AstNode instances into a tree-shape.
		Deque<AstNode> stack = new ArrayDeque<AstNode>();

		// Temporary nodes used in the above working stack, for isolating ASTs of partial expressions.
		AstNode parenthesisStackLid = new AstNode(new Token(TokenType.STACK_LID, "(PARENTHESIS_STACK_LID)"));
		AstNode separatorStackLid = new AstNode(new Token(TokenType.STACK_LID, "(SEPARATOR_STACK_LID)"));
		AstNode callBeginStackLid = new AstNode(new Token(TokenType.STACK_LID, "(CALL_BEGIN_STACK_LID)"));

		// The array storing next operator's precedence for each token.
		// At [i], it is stored that the precedence of the first operator of which token-index is greater than i.
		int[] nextOperatorPrecedences = getNextOperatorPrecedences(tokens);

		// Read tokens from left to right.
		int itoken = 0;
		do {
			Token token = tokens[itoken];
			AstNode operatorNode = null;

			// Case of literals and identifiers: "1.23", "x", "f", etc.
			if (token.type == TokenType.NUMBER_LITERAL
					|| token.type == TokenType.VARIABLE_IDENTIFIER
					|| token.type == TokenType.FUNCTION_IDENTIFIER) {
				stack.push(new AstNode(token));
				itoken++;
				continue;

			// Case of parenthesis: "(" or ")"
			} else if (token.type == TokenType.PARENTHESIS) {
				if (token.word.equals("(")) {
					stack.push(parenthesisStackLid);
					itoken++;
					continue;
				} else { // Case of ")"
					operatorNode = popPartialExprNodes(stack, parenthesisStackLid)[0];
				}

			// Case of separator: ","
			} else if (token.type == TokenType.EXPRESSION_SEPARATOR) {
				stack.push(separatorStackLid);
				itoken++;
				continue;

			// Case of operators: "+", "-", etc.
			} else if (token.type == TokenType.OPERATOR) {
				operatorNode = new AstNode(token);
				int nextOpPrecedence = nextOperatorPrecedences[itoken];

				// Case of unary-prefix operators:
				// * Connect the node of right-token as an operand, if necessary (depending the next operator's precedence).
				if (token.operator.type == OperatorType.UNARY_PREFIX) {
					if (shouldAddRightOperand(token.operator.precedence, nextOpPrecedence)) {
						operatorNode.childNodeList.add(new AstNode(tokens[itoken + 1]));
						itoken++;
					} // else: Operand will be connected later. See the bottom of this loop.

				// Case of binary operators:
				// * Always connect the node of left-token as an operand.
				// * Connect the node of right-token as an operand, if necessary (depending the next operator's precedence).
				} else if (token.operator.type == OperatorType.BINARY) {
					operatorNode.childNodeList.add(stack.pop());
					if (shouldAddRightOperand(token.operator.precedence, nextOpPrecedence)) {
						operatorNode.childNodeList.add(new AstNode(tokens[itoken + 1]));
						itoken++;
					} // else: Right-operand will be connected later. See the bottom of this loop.

				// Case of function-call operators.
				} else if (token.operator.type == OperatorType.CALL) {
					if (token.word.equals("(")) {
						operatorNode.childNodeList.add(stack.pop()); // Add function-identifier node at the top of the stack.
						stack.push(operatorNode);
						stack.push(callBeginStackLid); // The marker to correct partial expressions of args from the stack.
						itoken++;
						continue;
					} else { // Case of ")"
						AstNode[] argNodes = popPartialExprNodes(stack, callBeginStackLid);
						operatorNode = stack.pop();
						for (AstNode argNode: argNodes) {
							operatorNode.childNodeList.add(argNode);
						}
					}
				}
			}

			// If the precedence of the operator at the top of the stack is stronger than the next operator,
			// connect all "unconnected yet" operands and operators in the stack.
			while (shouldAddRightOperandToStackedOperator(stack, nextOperatorPrecedences[itoken])) {
				AstNode oldOperatorNode = operatorNode;
				operatorNode = stack.pop();
				operatorNode.childNodeList.add(oldOperatorNode);
			}
			stack.push(operatorNode);
			itoken++;

		} while (itoken < tokenCount);

		// The AST has been constructed on the stack, and only its root node is stored in the stack, so return it.
		return stack.pop();
	}

	/**
	 * Judges whether the right-side token should be connected directly as an operand, to the target operator.
	 *
	 * @param targetOperatorPrecedence The precedence of the target operator (smaller value gives higher precedence).
	 * @param nextOperatorPrecedence The precedence of the next operator (smaller value gives higher precedence).
	 * @return Returns true if the right-side token (operand) should be connected to the target operator
	 */
	private static boolean shouldAddRightOperand(int targetOperatorPrecedence, int nextOperatorPrecedence) {
		return targetOperatorPrecedence <= nextOperatorPrecedence; // left is stronger
	}

	/**
	 * Judges whether the right-side token should be connected directly as an operand,
	 * to the operator at the top of the working stack.
	 *
	 * @param stack The working stack used for the parsing
	 * @param nextOperatorPrecedence The precedence of the next operator (smaller value gives higher precedence).
	 * @return Returns true if the right-side token (operand) should be connected to the operator at the top of the stack
	 */
	private static boolean shouldAddRightOperandToStackedOperator(Deque<AstNode> stack, int nextOperatorPrecedence) {
		if (stack.size() == 0 || stack.peek().token.type != TokenType.OPERATOR) {
			return false;
		}
		return shouldAddRightOperand(stack.peek().token.operator.precedence, nextOperatorPrecedence);
	}

	/**
	 * Pops root nodes of ASTs of partial expressions constructed on the stack.
	 *
	 * @param stack The working stack used for the parsing
	 * @param targetStackLidNode The temporary node pushed in the stack, at the end of partial expressions to be popped
	 * @return Root nodes of ASTs of partial expressions
	 */
	private static AstNode[] popPartialExprNodes(Deque<AstNode> stack, AstNode endStackLidNode) {
		if (stack.size() == 0) {
			throw new Exevalator.ExevalatorException("Unexpected end of a partial expression");
		}
		List<AstNode> partialExprNodeList = new ArrayList<AstNode>();
		while(stack.size() != 0) {
			if (stack.peek().token.type == TokenType.STACK_LID) {
				AstNode stackLidNode = stack.pop();
				if (stackLidNode == endStackLidNode) {
					break;
				}
			} else {
				partialExprNodeList.add(stack.pop());
			}
		}
		return partialExprNodeList.toArray(new AstNode[partialExprNodeList.size()]);
	}

	/**
	 * Returns an array storing next operator's precedence for each token.
	 * In the returned array, it will stored at [i] that
	 * precedence of the first operator of which token-index is greater than i.
	 *
	 * @param tokens All tokens to be parsed
	 * @return The array storing next operator's precedence for each token
	 */
	private static int[] getNextOperatorPrecedences(Token[] tokens) {
		int tokenCount = tokens.length;
		int lastOperatorPrecedence = Integer.MAX_VALUE; // least prior
		int[] nextOperatorPrecedences = new int[tokenCount];

		for (int itoken=tokenCount-1; 0<=itoken; itoken--) {
			Token token = tokens[itoken];
			nextOperatorPrecedences[itoken] = lastOperatorPrecedence;

			if (token.type == TokenType.OPERATOR) {
				lastOperatorPrecedence = token.operator.precedence;
			}

			if (token.type == TokenType.PARENTHESIS) {
				if (token.word.equals("(")) {
					lastOperatorPrecedence = 0; // most prior
				} else { // case of ")"
					lastOperatorPrecedence = Integer.MAX_VALUE; // least prior
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
 * The class storing information of an operator.
 */
final class Operator {

	/** The symbol of this operator (for example: '+'). */
	public final char symbol;

	/** The precedence of this operator (smaller value gives higher precedence). */
	public final int precedence;

	/** The type of operator tokens. */
	public final OperatorType type;

	/**
	 * Create an Operator instance storing specified information.
	 *
	 * @param type The type of this operator
	 * @param symbol The symbol of this operator
	 * @param precedence The precedence of this operator
	 */
	public Operator(OperatorType type, char symbol, int precedence) {
		this.type = type;
		this.symbol = symbol;
		this.precedence = precedence;
	}

	/**
	 * Returns the String representation of this Operator instance.
	 */
	@Override
	public String toString() {
		return "Operator [symbol=" + symbol + ", precedence=" + precedence + ", type=" + type + "]";
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

	/** Represents separator tokens of partial expressions: , */
	EXPRESSION_SEPARATOR,

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
final class Token {

	/** The type of this token. */
	public final TokenType type;

	/** The text representation of this token. */
	public final String word;

	/** The detailed information of the operator, if the type of this token is OPERATOR. */
	public final Operator operator;

	/**
	 * Create an Token instance storing specified information.
	 *
	 * @param type The type of this token
	 * @param word The text representation of this token
	 */
	public Token(TokenType type, String word) {
		this.type = type;
		this.word = word;
		this.operator = null;
	}

	/**
	 * Create an Token instance storing specified information.
	 *
	 * @param type The type of this token
	 * @param word The text representation of this token
	 * @param operator The detailed information of the operator, for OPERATOR type tokens
	 */
	public Token(TokenType type, String word, Operator operator) {
		this.type = type;
		this.word = word;
		this.operator = operator;
	}

	/**
	 * Returns the String representation of this Token instance.
	 */
	@Override
	public String toString() {
		if (this.operator == null) {
			return "Token [type=" + type + ", word=" + word + "]";
		} else {
			return "Token [type=" + type + ", word=" + word +
				", operator.type=" + operator.type + ", operator.precedence=" + operator.precedence + "]";
		}
	}
}


/**
 * The class storing information of an node of an AST.
 */
final class AstNode {

	/** The token corresponding with this AST node. */
	public final Token token;

	/** The list of child nodes of this AST node. */
	public final List<AstNode> childNodeList;

	/** The evaluator unit for evaluating the value of this AST node. */
	private Evaluator.EvaluatorUnit evaluatorUnit;

	/**
	 * Create an AST node instance storing specified information.
	 *
	 * @param token The token corresponding with this AST node
	 */
	public AstNode(Token token) {
		this.token = token;
		this.childNodeList = new ArrayList<AstNode>();
	}

	/**
	 * Creates the evaluator unit for evaluating the value of this AST node.
	 *
	 * @param interconnect The interconnect providing references to variables and functions.
	 * @return The created evaluator unit.
	 */
	public Evaluator.EvaluatorUnit createEvaluatorUnit(Interconnect interconnect) {

		// Initialize evaluation units of child nodes, and store then into an array.
		int childCount = this.childNodeList.size();
		Evaluator.EvaluatorUnit childNodeUnits[] = new Evaluator.EvaluatorUnit[childCount];
		for (int ichild=0; ichild<childCount; ichild++) {
			childNodeUnits[ichild] = this.childNodeList.get(ichild).createEvaluatorUnit(interconnect);
		}

		// Initialize evaluation units of this node.
		if (this.token.type == TokenType.NUMBER_LITERAL) {
			return new Evaluator.NumberLiteralEvaluatorUnit(this.token.word);
		} else if (this.token.type == TokenType.VARIABLE_IDENTIFIER) {
			return new Evaluator.VariableValueEvaluatorUnit(this.token.word, interconnect);
		} else if (this.token.type == TokenType.FUNCTION_IDENTIFIER) {
			return null;
		} else if (this.token.type == TokenType.OPERATOR) {
			Operator op = this.token.operator;

			if (op.type == OperatorType.UNARY_PREFIX && op.symbol == '-') {
				return new Evaluator.MinusEvaluatorUnit(childNodeUnits[0]);
			} else if (op.type == OperatorType.BINARY && op.symbol == '+') {
				return new Evaluator.AdditionEvaluatorUnit(childNodeUnits[0], childNodeUnits[1]);
			} else if (op.type == OperatorType.BINARY && op.symbol == '-') {
				return new Evaluator.SubtractionEvaluatorUnit(childNodeUnits[0], childNodeUnits[1]);
			} else if (op.type == OperatorType.BINARY && op.symbol == '*') {
				return new Evaluator.MultiplicationEvaluatorUnit(childNodeUnits[0], childNodeUnits[1]);
			} else if (op.type == OperatorType.BINARY && op.symbol == '/') {
				return new Evaluator.DivisionEvaluatorUnit(childNodeUnits[0], childNodeUnits[1]);
			} else if (op.type == OperatorType.CALL && op.symbol == '(') {
				String identifier = this.childNodeList.get(0).token.word;
				return new Evaluator.FunctionCallEvaluatorUnit(identifier, interconnect, childNodeUnits);
			} else {
				throw new Exevalator.ExevalatorException("Unexpected operator: " + op);
			}
		} else {
			throw new Exevalator.ExevalatorException("Unexpected token type: " + this.token.type);
		}
	}

	/**
	 * Evaluates the value of this AST node.
	 *
	 * @return The evaluated value of this AST node.
	 */
	public double evaluate() {
		return this.evaluatorUnit.evaluate();
	}

	/**
	 * Expresses the AST under this node in XML-like text format.
	 *
	 * @returns XML-like text representation of the AST under this node
	 */
	@Override
	public String toString() {
		return this.toMarkupText(0);
	}

	/**
	 * Expresses the AST under this node in XML-like text format.
	 *
	 * @param indentStage The stage of indent of this node
	 * @return XML-like text representation of the AST under this node
	 */
	private String toMarkupText(int indentStage) {
		StringBuilder indentBuilder = new StringBuilder();
		for (int istage=0; istage<indentStage; istage++) {
			indentBuilder.append(StaticSettings.AST_INDENT);
		}
		final String indent = indentBuilder.toString();
		final String eol = System.getProperty("line.separator");
		StringBuilder resultBuilder = new StringBuilder();

		resultBuilder.append(indent);
		resultBuilder.append("<");
		resultBuilder.append(this.token.type);
		resultBuilder.append(" word=\"");
		resultBuilder.append(this.token.word);
		resultBuilder.append("\"");
		if (this.token.type == TokenType.OPERATOR) {
			resultBuilder.append(" optype=\"");
			resultBuilder.append(this.token.operator.type);
			resultBuilder.append("\" precedence=\"");
			resultBuilder.append(this.token.operator.precedence);
			resultBuilder.append("\"");
		}

		if (0 < this.childNodeList.size()) {
			resultBuilder.append(">");
			for (AstNode childNode: this.childNodeList) {
				resultBuilder.append(eol);
				resultBuilder.append(childNode.toMarkupText(indentStage + 1));
			}
			resultBuilder.append(eol);
			resultBuilder.append(indent);
			resultBuilder.append("</");
			resultBuilder.append(this.token.type);
			resultBuilder.append(">");

		} else {
			resultBuilder.append(" />");
		}

		return resultBuilder.toString();
	}
}


/**
 * The class providing various types of evaluator units
 * which evaluate values of operators, literals, etc.
 */
final class Evaluator {

	/**
	 * The super class of evaluator units.
	 */
	public static abstract class EvaluatorUnit {

		/**
		 * Performs the evaluation.
		 *
		 * @return The evaluated value.
		 */
		public abstract double evaluate();
	}

	/**
	 * The super class of evaluator units of binary operations.
	 */
	public static abstract class BinaryOperationEvaluatorUnit extends EvaluatorUnit {

		/** The unit for evaluating the right-side operand. */
		protected final EvaluatorUnit leftOperandUnit;

		/** The unit for evaluating the left-side operand. */
		protected final EvaluatorUnit rightOperandUnit;

		/**
		 * Initializes operands.
		 *
		 * @param leftOperandUnit The unit for evaluating the left-side operand
		 * @param rightOperandUnit The unit for evaluating the right-side operand
		 */
		protected BinaryOperationEvaluatorUnit(EvaluatorUnit leftOperandUnit, EvaluatorUnit rightOperandUnit) {
			this.leftOperandUnit = leftOperandUnit;
			this.rightOperandUnit = rightOperandUnit;
		}
	}

	/**
	 * The evaluator unit for evaluating the value of a addition operator.
	 */
	public static final class AdditionEvaluatorUnit extends BinaryOperationEvaluatorUnit {

		/**
		 * Initializes operands.
		 *
		 * @param leftOperandUnit The unit for evaluating the left-side operand
		 * @param rightOperandUnit The unit for evaluating the right-side operand
		 */
		public AdditionEvaluatorUnit(EvaluatorUnit leftOperandUnit, EvaluatorUnit rightOperandUnit) {
			super(leftOperandUnit, rightOperandUnit);
		}

		/**
		 * Performs the addition.
		 *
		 * @return The result value of the addition
		 */
		@Override
		public double evaluate() {
			return this.leftOperandUnit.evaluate() + this.rightOperandUnit.evaluate();
		}
	}

	/**
	 * The evaluator unit for evaluating the value of a subtraction operator.
	 */
	public static final class SubtractionEvaluatorUnit extends BinaryOperationEvaluatorUnit {

		/**
		 * Initializes operands.
		 *
		 * @param leftOperandUnit The unit for evaluating the left-side operand
		 * @param rightOperandUnit The unit for evaluating the right-side operand
		 */
		public SubtractionEvaluatorUnit(EvaluatorUnit leftOperandUnit, EvaluatorUnit rightOperandUnit) {
			super(leftOperandUnit, rightOperandUnit);
		}

		/**
		 * Performs the subtraction.
		 *
		 * @return The result value of the subtraction
		 */
		@Override
		public double evaluate() {
			return this.leftOperandUnit.evaluate() - this.rightOperandUnit.evaluate();
		}
	}

	/**
	 * The evaluator unit for evaluating the value of a multiplication operator.
	 */
	public static final class MultiplicationEvaluatorUnit extends BinaryOperationEvaluatorUnit {

		/**
		 * Initializes operands.
		 *
		 * @param leftOperandUnit The unit for evaluating the left-side operand
		 * @param rightOperandUnit The unit for evaluating the right-side operand
		 */
		public MultiplicationEvaluatorUnit(EvaluatorUnit leftOperandUnit, EvaluatorUnit rightOperandUnit) {
			super(leftOperandUnit, rightOperandUnit);
		}

		/**
		 * Performs the multiplication.
		 *
		 * @return The result value of the multiplication
		 */
		@Override
		public double evaluate() {
			return this.leftOperandUnit.evaluate() * this.rightOperandUnit.evaluate();
		}
	}

	/**
	 * The evaluator unit for evaluating the value of a division operator.
	 */
	public static final class DivisionEvaluatorUnit extends BinaryOperationEvaluatorUnit {

		/**
		 * Initializes operands.
		 *
		 * @param leftOperandUnit The unit for evaluating the left-side operand
		 * @param rightOperandUnit The unit for evaluating the right-side operand
		 */
		public DivisionEvaluatorUnit(EvaluatorUnit leftOperandUnit, EvaluatorUnit rightOperandUnit) {
			super(leftOperandUnit, rightOperandUnit);
		}

		/**
		 * Performs the division.
		 *
		 * @return The result value of the division
		 */
		@Override
		public double evaluate() {
			return this.leftOperandUnit.evaluate() / this.rightOperandUnit.evaluate();
		}
	}

	/**
	 * The evaluator unit for evaluating the value of a unary-minus operator.
	 */
	public static final class MinusEvaluatorUnit extends EvaluatorUnit {

		/** The unit for evaluating the operand. */
		private final EvaluatorUnit operandUnit;

		/**
		 * Initializes the operand.
		 *
		 * @param operandUnit The unit for evaluating the operand
		 */
		public MinusEvaluatorUnit(EvaluatorUnit operandUnit) {
			this.operandUnit = operandUnit;
		}

		/**
		 * Performs the division.
		 *
		 * @return The result value of the division.
		 */
		@Override
		public double evaluate() {
			return -this.operandUnit.evaluate();
		}
	}

	/**
	 * The evaluator unit for evaluating the value of a number literal.
	 */
	public static final class NumberLiteralEvaluatorUnit extends EvaluatorUnit {

		/** The value of the number literal. */
		private final double value;

		/**
		 * Initializes the value of the number literal.
		 *
		 * @param literal The number literal
		 */
		public NumberLiteralEvaluatorUnit(String literal) {
			try {
				this.value = Double.parseDouble(literal);
			} catch (NumberFormatException nfe) {
				throw new Exevalator.ExevalatorException("Invalid number literal: " + literal);
			}
		}

		/**
		 * Returns the value of the number literal.
		 *
		 * @return The value of the number literal.
		 */
		@Override
		public double evaluate() {
			return this.value;
		}
	}

	/**
	 * The evaluator unit for evaluating the value of a variable.
	 */
	public static final class VariableValueEvaluatorUnit extends EvaluatorUnit {

		/** The variable to be evaluated. */
		private volatile AbstractVariable variable;

		/**
		 * Initializes the reference to the variable.
		 *
		 * @param variableName The name of the variable to be evaluated
		 * @param interconnect The interconnect providing the reference to the variable
		 */
		public VariableValueEvaluatorUnit(String variableName, Interconnect interconnect) {
			if (!interconnect.isVariableConnected(variableName)) {
				throw new Exevalator.ExevalatorException("No variable found: " + variableName);
			}
			this.variable = interconnect.getVariable(variableName);
		}

		/**
		 * Returns the value of the variable.
		 *
		 * @return The value of the variable.
		 */
		@Override
		public double evaluate() {
			return this.variable.getVariableValue();
		}
	}

	/**
	 * The evaluator unit for evaluating a function-call operator.
	 *
	 */
	public static final class FunctionCallEvaluatorUnit extends EvaluatorUnit {

		/** The variable to be called. */
		private volatile AbstractFunction function;

		/** Evaluator units for evaluating values of arguments. */
		private volatile EvaluatorUnit[] argumentEvalUnits;

		/** An array storing evaluated values of arguments. */
		private volatile double[] argumentArrayBuffer;

		/** The number of arguments. */
		private volatile int argumentCount;

		public FunctionCallEvaluatorUnit(
				String functionName, Interconnect interconnect, EvaluatorUnit[] argumentEvalUnits) {

			this.argumentCount = argumentEvalUnits.length - 1; // The first element is identifier, so -1.
			if (!interconnect.isFunctionConnected(functionName, this.argumentCount)) {
				throw new Exevalator.ExevalatorException(
					"No function found: " + functionName + "(" + this.argumentCount + "-arguments"
				);
			}
			this.function = interconnect.getFunction(functionName, this.argumentCount);
			this.argumentEvalUnits = argumentEvalUnits;
			this.argumentArrayBuffer = new double[this.argumentCount];
		}

		/**
		 * Calls the function and returns the returned value of the function.
		 *
		 * @return The returned value of the function
		 */
		@Override
		public double evaluate() {
			for (int iarg=0; iarg<this.argumentCount; iarg++) {
				this.argumentArrayBuffer[iarg] = this.argumentEvalUnits[iarg + 1].evaluate();
			}
			return this.function.invoke(this.argumentArrayBuffer);
		}
	}
}


/**
 * The super class of variables available on this interpreter.
 */
abstract class AbstractVariable {

	/**
	 * Gets the name of this variable.
	 *
	 * @return The name of this variable.
	 */
	public abstract String getVariableName();

	/**
	 * Sets the value of this variable.
	 *
	 * @param value The value to be set to this variable
	 */
	public abstract void setVariableValue(double value);

	/**
	 *  Gets the value of this variable.
	 *
	 * @return The value of this variable.
	 */
	public abstract double getVariableValue();
}


/**
 * The super class of functions available on this interpreter.
 */
abstract class AbstractFunction {

	/**
	 * Gets the name of this function.
	 *
	 * @return The name of this function.
	 */
	public abstract String getFunctionName();

	/**
	 * Gets names of parameters.
	 *
	 * @return Names of parameters.
	 */
	public abstract String[] getParameterNames();

	/**
	 * Invokes this function.
	 *
	 * @param arguments The arguments to be passed to this function.
	 * @return The returned value of this function.
	 */
	public abstract double invoke(double[] arguments);
}


/**
 * The class for connecting a Field to this interpreter as a variable.
 */
final class FieldToVariableAdapter extends AbstractVariable {

	/** The field to be connected as a variable. */
	private volatile Field field;

	/** The instance of the object to which the field belongs. */
	private Object objectInstance;

	/**
	 * Creates the adapter to connect the specified field as a variable.
	 *
	 * @param field The field to be connected as a variable
	 * @param objectInstance The instance of the object to which the field belongs.
	 */
	public FieldToVariableAdapter (Field field, Object objectInstance) {
		this.field = field;
		this.objectInstance = objectInstance;
		if (this.field.getType() != double.class && this.field.getType() != Double.class) {
			throw new Exevalator.ExevalatorException("Incorrect data type of the variable: " + this.field.getName());
		}
	}

	@Override
	public String getVariableName() {
		return this.field.getName();
	}

	@Override
	public void setVariableValue(double value) {
		try {
			this.field.set(this.objectInstance, value);
		} catch (IllegalArgumentException | IllegalAccessException e) {
			throw new Exevalator.ExevalatorException("Can not set a value to the variable: " + this.field.getName(), e);
		}
	}

	@Override
	public double getVariableValue() {
		try {
			return (double)this.field.get(this.objectInstance);
		} catch (IllegalArgumentException | IllegalAccessException e) {
			throw new Exevalator.ExevalatorException("Can not get a value of the variable: " + this.field.getName(), e);
		}
	}
}


/**
 * The class for connecting a Method to this interpreter as a function.
 */
final class MethodToFunctionAdapter extends AbstractFunction {

	/** The method to be connected as a function. */
	private volatile Method method;

	/** The instance of the object to which the method belongs. */
	private Object objectInstance;

	/**
	 * Creates the adapter to connect the specified method as a function.
	 *
	 * @param method The method to be connected as a function
	 * @param objectInstance The instance of the object to which the method belongs.
	 */
	public MethodToFunctionAdapter(Method method, Object objectInstance) {
		this.method = method;
		this.objectInstance = objectInstance;
	}

	@Override
	public String getFunctionName() {
		return this.method.getName();
	}

	@Override
	public String[] getParameterNames() {
		int parameterCount = this.method.getParameterCount();
		String[] parameterNames = new String[parameterCount];
		for (int iparam=0; iparam<parameterCount; iparam++) {
			parameterNames[iparam] = "arg" + iparam;
		}
		return parameterNames;
	}

	@Override
	public double invoke(double[] arguments) {
		int argCount = arguments.length;
		Object[] argObjects = new Object[argCount];
		for (int iarg=0; iarg<argCount; iarg++) {
			argObjects[iarg] = arguments[iarg];
		}

		double returnedValue = Double.NaN;
		try {
			returnedValue = (double)this.method.invoke(this.objectInstance, argObjects);
		} catch (IllegalAccessException | IllegalArgumentException | InvocationTargetException e) {
			throw new Exevalator.ExevalatorException("Can not invoke the function: " + this.method.getName(), e);
		}
		return returnedValue;
	}
}


/**
 * The class to provide objects shared among multiple components of this interpreter.
 */
class Interconnect {

	/** The table mapping from names of variables to instances of variables. */
	private volatile ConcurrentHashMap<String, AbstractVariable> variableTable;

	/** The table mapping from names of functions to instances of functions. */
	private volatile ConcurrentHashMap<String, AbstractFunction> functionTable;

	/**
	 * Creates an Interconnect instance to which nothing is connected.
	 */
	public Interconnect() {
		this.variableTable = new ConcurrentHashMap<String, AbstractVariable>();
		this.functionTable = new ConcurrentHashMap<String, AbstractFunction>();
	}

	/**
	 * Connects the new variable to share it between components of this interpreter.
	 *
	 * @param variable The variable to be connected.
	 */
	public synchronized void connectVariable(AbstractVariable variable) {
		this.variableTable.put(variable.getVariableName(), variable);
	}

	/**
	 * Returns the variable having the specified name, if it is connected.
	 *
	 * @param variableName The name of the variable to be gotten.
	 * @return The variable having the specified name.
	 */
	public synchronized AbstractVariable getVariable(String variableName) {
		return this.variableTable.get(variableName);
	}

	/**
	 * Checks whether the variable having the specified name is connected or not.
	 *
	 * @param variableName The name of the variable to be checked.
	 * @return True if the variable having the specified name is connected.
	 */
	public synchronized boolean isVariableConnected(String variableName) {
		return this.variableTable.containsKey(variableName);
	}

	/**
	 * Disconnects the variable having the specified name.
	 *
	 * @param variableName The name of the variable to be disconnected.
	 */
	public synchronized void disconnectVariable(String variableName) {
		this.variableTable.remove(variableName);
	}

	/**
	 * Disconnects all variables..
	 */
	public synchronized void disconnectAllVariables() {
		this.variableTable.clear();
	}

	/**
	 * Connects the new function to share it between components of this interpreter.
	 *
	 * @param variable The function to be connected.
	 */
	public synchronized void connectFunction(AbstractFunction function) {
		// Append the number of parameters to the tail of the function name, to support function-overloading.
		String key = function.getFunctionName() + "$" + function.getParameterNames().length;
		this.functionTable.put(key, function);
	}

	/**
	 * Returns the function having the specified information, if it is connected.
	 *
	 * @param functionName The name of the function to be gotten.
	 * @param parameterCount The number of parameters of the function to be gotten.
	 * @return The function having the specified information.
	 */
	public synchronized AbstractFunction getFunction(String functionName, int parameterCount) {
		String key = functionName + "$" + parameterCount; // See the comment in "connectFunction" method.
		return this.functionTable.get(key);
	}

	/**
	 * Checks whether the function having the specified information is connected or not.
	 *
	 * @param functionName The name of the function to be checked.
	 * @param parameterCount The number of parameters of the function to be checked.
	 * @return True if the function having the specified information is connected.
	 */
	public synchronized boolean isFunctionConnected(String functionName, int parameterCount) {
		String key = functionName + "$" + parameterCount; // See the comment in "connectFunction" method.
		return this.functionTable.containsKey(key);
	}

	/**
	 * Disconnects the function having the specified information.
	 *
	 * @param functionName The name of the function to be disconnected.
	 * @param parameterCount The number of parameters of the function to be disconnected.
	 */
	public synchronized void disconnectFunction(String functionName, int parameterCount) {
		String key = functionName + "$" + parameterCount; // See the comment in "connectFunction" method.
		this.functionTable.remove(key);
	}

	/**
	 * Disconnects all functions.
	 */
	public synchronized void disconnectAllFunctions() {
		this.functionTable.clear();
	}
}


/**
 * The class defining static setting values.
 */
final class StaticSettings {

	/** The indent used in text representations of ASTs. */
	public static final String AST_INDENT = "  ";

	/** The regular expression of number literals. */
	public static final String NUMBER_LITERAL_REGEX =
		"([0-9]+(\\.[0-9]+)?)" +    // Significand part
		"((e|E)(\\+|-)?[0-9]+)?";   // Exponent part

	/** The escaped representation of number literals in expressions */
	public static final String ESCAPED_NUMBER_LITERAL = "@NUMBER_LITERAL@";

	/** The set of symbols of available operators. */
	public static final Set<Character> OPERATOR_SYMBOL_SET;

	/** The Map mapping each symbol of an unary-prefix operator to an instance of Operator class. */
	public static final Map<Character, Operator> UNARY_PREFIX_OPERATOR_SYMBOL_MAP;

	/** The Map mapping each symbol of an binary operator to an instance of Operator class. */
	public static final Map<Character, Operator> BINARY_OPERATOR_SYMBOL_MAP;

	/** The Map mapping each symbol of an call operator to an instance of Operator class. */
	public static final Map<Character, Operator> CALL_OPERATOR_SYMBOL_MAP;

	/** The list of symbols to split an expression into tokens. */
	public static final List<Character> TOKEN_SPLITTER_SYMBOL_LIST;

	static {
		Operator additionOperator       = new Operator(OperatorType.BINARY, '+', 400);
		Operator subtractionOperator    = new Operator(OperatorType.BINARY, '-', 400);
		Operator multiplicationOperator = new Operator(OperatorType.BINARY, '*', 300);
		Operator divisionOperator       = new Operator(OperatorType.BINARY, '/', 300);
		Operator minusOperator          = new Operator(OperatorType.UNARY_PREFIX, '-', 200);
		Operator callBeginOperator      = new Operator(OperatorType.CALL, '(', 100);
		Operator callEndOperator        = new Operator(OperatorType.CALL, ')', Integer.MAX_VALUE); // least prior

		OPERATOR_SYMBOL_SET = new HashSet<Character>();
		OPERATOR_SYMBOL_SET.add('+');
		OPERATOR_SYMBOL_SET.add('-');
		OPERATOR_SYMBOL_SET.add('*');
		OPERATOR_SYMBOL_SET.add('/');
		OPERATOR_SYMBOL_SET.add('(');
		OPERATOR_SYMBOL_SET.add(')');

		UNARY_PREFIX_OPERATOR_SYMBOL_MAP = new ConcurrentHashMap<Character, Operator>();
		UNARY_PREFIX_OPERATOR_SYMBOL_MAP.put('-', minusOperator);

		BINARY_OPERATOR_SYMBOL_MAP = new ConcurrentHashMap<Character, Operator>();
		BINARY_OPERATOR_SYMBOL_MAP.put('+', additionOperator);
		BINARY_OPERATOR_SYMBOL_MAP.put('-', subtractionOperator);
		BINARY_OPERATOR_SYMBOL_MAP.put('*', multiplicationOperator);
		BINARY_OPERATOR_SYMBOL_MAP.put('/', divisionOperator);

		CALL_OPERATOR_SYMBOL_MAP = new ConcurrentHashMap<Character, Operator>();
		CALL_OPERATOR_SYMBOL_MAP.put('(', callBeginOperator);
		CALL_OPERATOR_SYMBOL_MAP.put(')', callEndOperator);

		TOKEN_SPLITTER_SYMBOL_LIST = new ArrayList<Character>();
		TOKEN_SPLITTER_SYMBOL_LIST.add('+');
		TOKEN_SPLITTER_SYMBOL_LIST.add('-');
		TOKEN_SPLITTER_SYMBOL_LIST.add('*');
		TOKEN_SPLITTER_SYMBOL_LIST.add('/');
		TOKEN_SPLITTER_SYMBOL_LIST.add('(');
		TOKEN_SPLITTER_SYMBOL_LIST.add(')');
		TOKEN_SPLITTER_SYMBOL_LIST.add(',');
	};
}
