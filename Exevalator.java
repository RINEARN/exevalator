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
import java.util.concurrent.ConcurrentHashMap;
import java.util.regex.Matcher;
import java.util.regex.Pattern;


/**
 * Interpreter Engine of Exevalator.
 */
public final class Exevalator {

	/** The interconnect providing objects shared among multiple components of this interpreter. */
	private volatile Interconnect interconnect;

	/**
	 * Creates a new interpreter of the Exevalator.
	 */
	public Exevalator() {
		this.interconnect = new Interconnect();
	}

	/**
	 * Evaluates (computes) the value of an expression.
	 *
	 * @param expression The expression to be evaluated
	 * @return The evaluated value
	 */
	public double eval(String expression) {

		// Split the expression into tokens, and analyze them.
		Token[] tokens = new LexicalAnalyzer().analyze(expression);

		// Check syntactic correctness of tokens.
		new LexicalChecker().check(tokens);

		// Construct AST (Abstract Syntax Tree) by parsing tokens.
		AstNode ast = new Parser().parse(tokens);

		// Evaluate (compute) the value of the root node of the AST.
		ast.initializeEvaluatorUnit(this.interconnect);
		double evaluatedValue = ast.evaluate();

		/*
		// Temporary, for debugging tokens
		for (Token token: tokens) {
			System.out.println(token.toString());
		}
		*/

		/*
		// Temporary, for debugging AST
		System.out.println(ast.toString());
		*/

		return evaluatedValue;
	}

	/**
	 * Connects the new variable to share it between components of this interpreter.
	 *
	 * @param variable The variable to be connected.
	 */
	public synchronized void connectVariable(AbstractVariable variable) {
		this.interconnect.connectVariable(variable);
	}


	/**
	 * The class performing functions of a lexical analyzer.
	 */
	private static final class LexicalAnalyzer {

		/**
		 * Splits (tokenizes) the expression into tokens, and analyze them.
		 *
		 * @param expression The expression to be tokenized/analyzed
		 * @return Analyzed tokens
		 */
		public Token[] analyze(String expression) {

			// Split the expression into token words.
			String[] tokenWords = this.splitExpressionIntoTokenWords(expression);

			// Create Token instances.
			Token[] tokens = this.createTokensFromTokenWords(tokenWords);

			// Analyze detailed information for operator tokens.
			tokens = this.analyzeInformationOfOperands(tokens);
			return tokens;
		}

		/**
		 * Splits the inputted expression into token words.
		 *
		 * @param expression The inputted expression
		 * @return Splitted token words
		 */
		private String[] splitExpressionIntoTokenWords(String expression) {

			// Firstly, to simplify the tokenization,
			// replace number literals in the expression to the escaped representation: "@NUMBER_LITERAL",
			// because number literals may contains "+" or "-" in their exponent part.
			List<String> numberLiteralList = new ArrayList<String>();
			expression = this.escapeNumberLiterals(expression, numberLiteralList);

			// Tokenize (split) the expression into token words.
			for (String splitter: StaticSettings.TOKEN_SPLITTER_SYMBOL_LIST) {
				expression = expression.replace(splitter, " " + splitter + " ");
			}
			String[] tokenWords = expression.trim().split("\\s+");
			int tokenCount = tokenWords.length;

			// Recover escaped number literals.
			int numberLiteralIndex = 0;
			for (int itoken=0; itoken<tokenCount; itoken++) {
				if (tokenWords[itoken].equals(StaticSettings.ESCAPED_NUMBER_LITERAL)) {
					tokenWords[itoken] = numberLiteralList.get(numberLiteralIndex);
					numberLiteralIndex++;
				}
			}
			return tokenWords;
		}

		/**
		 * Creates Token-type instances from token words (String).
		 *
		 * @param tokenWords Token words (String) to be converted to Token instances
		 * @return Created Token instances
		 */
		private Token[] createTokensFromTokenWords(String[] tokenWords) {
			int tokenCount = tokenWords.length;

			// Stores the parenthesis-depth, which will increase at "(" and decrease at ")".
			int parenthesisDepth = 0;

			// Stores the parenthesis-depth when a function call operator begins,
			// for detecting the end of the function operator.
			Set<Integer> callParenthesisDepths = new HashSet<Integer>();

			Token[] tokens = new Token[tokenCount];
			String numberLiteralRegexForMatching = "^" + StaticSettings.NUMBER_LITERAL_REGEX + "$";
			for (int itoken=0; itoken<tokenCount; itoken++) {
				String word = tokenWords[itoken];

				// Cases of open parentheses, or beginning of function calls.
				if (word.equals("(")) {
					parenthesisDepth++;
					if (1 <= itoken && tokens[itoken - 1].type == Token.Type.FUNCTION_IDENTIFIER) {
						callParenthesisDepths.add(parenthesisDepth);
						tokens[itoken] = new Token(Token.Type.OPERATOR, word);
					} else {
						tokens[itoken] = new Token(Token.Type.PARENTHESIS, word);
					}

				// Cases of closes parentheses, or end of function calls.
				} else if (word.equals(")")) {
					if (callParenthesisDepths.contains(parenthesisDepth)) {
						callParenthesisDepths.remove(parenthesisDepth);
						tokens[itoken] = new Token(Token.Type.OPERATOR, word);
					} else {
						tokens[itoken] = new Token(Token.Type.PARENTHESIS, word);
					}
					parenthesisDepth--;

				// Cases of operator, literals, and separator.
				} else if (StaticSettings.OPERATOR_SYMBOL_SET.contains(word)) {
					tokens[itoken] = new Token(Token.Type.OPERATOR, word);
				} else if (word.matches(numberLiteralRegexForMatching)) {
					tokens[itoken] = new Token(Token.Type.NUMBER_LITERAL, word);
				} else if (word.equals(",")) {
					tokens[itoken] = new Token(Token.Type.EXPRESSION_SEPARATOR, word);

				// Cases of variable identifier of function identifier.
				} else {
					if (itoken < tokenCount - 1 && tokenWords[itoken + 1].equals("(")) {
						tokens[itoken] = new Token(Token.Type.FUNCTION_IDENTIFIER, word);
					} else {
						tokens[itoken] = new Token(Token.Type.VARIABLE_IDENTIFIER, word);
					}
				}
			}
			return tokens;
		}

		/**
		 * Analyzes information of OPERAND-type tokens in specified tokens.
		 *
		 * @param tokens Tokens to be analyzed.
		 * @return Tokens to which information of operands are added.
		 */
		private Token[] analyzeInformationOfOperands(Token[] tokens) {
			int tokenCount = tokens.length;
			Token[] resultTokens = new Token[tokenCount];

			// Analyze detailed information for operator tokens.
			Token lastToken = null;
			for (int itoken=0; itoken<tokenCount; itoken++) {
				Token token = tokens[itoken];
				if (token.type != Token.Type.OPERATOR) {
					resultTokens[itoken] = token;
					lastToken = token;
					continue;
				}
				Operator operator = null;

				// Cases of function call operators.
				if (token.word.equals("(") || token.word.equals(")") ) {
					operator = StaticSettings.searchOperator(Operator.Type.CALL, token.word);

				// Cases of unary-prefix operators.
				} else if (lastToken == null
						|| lastToken.word.equals("(")
						|| (lastToken.type == Token.Type.OPERATOR && lastToken.operator.type != Operator.Type.CALL) ) {
					operator = StaticSettings.searchOperator(Operator.Type.UNARY_PREFIX, token.word);

				// Cases of binary operators.
				} else if (lastToken.word.equals(")")
						|| lastToken.type == Token.Type.NUMBER_LITERAL
						|| lastToken.type == Token.Type.VARIABLE_IDENTIFIER) {
					operator = StaticSettings.searchOperator(Operator.Type.BINARY, token.word);

				} else {
					throw new ExevalatorException("Unexpected operator syntax: " + token.word);
				}
				resultTokens[itoken] = new Token(token.type, token.word, operator);
				lastToken = resultTokens[itoken];
			}
			return resultTokens;
		}

		/**
		 * Replaces number literals in the expression to the escaped representation: "@NUMBER_LITERAL@".
		 *
		 * @param expression The expression of which number literals are not escaped yet
		 * @param literalStoreList The list to which number literals will be added
		 * @return The expression in which number literals are escaped.
		 */
		private String escapeNumberLiterals(String expression, List<String> literalStoreList) {
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
	}


	/**
	 * The class for checking syntactic correctness of tokens of inputted expressions.
	 */
	private static final class LexicalChecker {

		/*
		 * Checks syntactic correctness of tokens of inputted expressions.
		 * An ExevalatorException will be thrown when any errors detected.
		 * If no error detected, nothing will occur.
		 *
		 * @param tokens Tokens of the inputted expression.
		 */
		public void check(Token[] tokens) {
			this.checkParenthesisOpeningClosings(tokens);
			this.checkEmptyParentheses(tokens);
			this.checkLocationsOfOperatorsAndLeafs(tokens);
		}

		/**
		 * Checks the number and correspondence of open "(" / closed ")" parentheses.
		 * An ExevalatorException will be thrown when any errors detected.
		 * If no error detected, nothing will occur.
		 *
		 * @param tokens Tokens of the inputted expression.
		 */
		private void checkParenthesisOpeningClosings(Token[] tokens) {
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
					throw new ExevalatorException(
						"The number of open parenthesis \"(\" is deficient."
					);
				}
			}

			// If the value of hierarchy is not zero at the end of the expression,
			// the closed parentheses ")" is deficient.
			if (hierarchy > 0) {
				throw new ExevalatorException(
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
		private void checkEmptyParentheses(Token[] tokens) {
			int tokenCount = tokens.length;
			int contentCounter = 0;
			for (int tokenIndex=0; tokenIndex<tokenCount; tokenIndex++) {
				Token token = tokens[tokenIndex];
				if (token.type == Token.Type.PARENTHESIS) { // Excepting CALL operators
					if (token.word.equals("(")) {
						contentCounter = 0;
					} else if (token.word.equals(")")) {
						if (contentCounter == 0) {
							throw new ExevalatorException(
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
		private void checkLocationsOfOperatorsAndLeafs(Token[] tokens) {
			int tokenCount = tokens.length;
			Set<Token.Type> leafTypeSet = EnumSet.noneOf(Token.Type.class);
			leafTypeSet.add(Token.Type.NUMBER_LITERAL);
			leafTypeSet.add(Token.Type.VARIABLE_IDENTIFIER);

			// Reads and check tokens from left to right.
			for (int tokenIndex=0; tokenIndex<tokenCount; tokenIndex++) {
				Token token = tokens[tokenIndex];

				// Prepare information of next/previous token.
				boolean nextIsLeaf = tokenIndex!=tokenCount-1 && leafTypeSet.contains(tokens[tokenIndex+1].type);
				boolean prevIsLeaf = tokenIndex!=0 && leafTypeSet.contains(tokens[tokenIndex-1].type);
				boolean nextIsOpenParenthesis = tokenIndex < tokenCount-1 && tokens[tokenIndex+1].word.equals("(");
				boolean prevIsCloseParenthesis = tokenIndex != 0 && tokens[tokenIndex-1].word.equals(")");
				boolean nextIsPrefixOperator = tokenIndex < tokenCount-1
						&& tokens[tokenIndex+1].type == Token.Type.OPERATOR
						&& tokens[tokenIndex+1].operator.type == Operator.Type.UNARY_PREFIX;
				boolean nextIsFunctionCallBegin = nextIsOpenParenthesis
						&& tokens[tokenIndex+1].type == Token.Type.OPERATOR
						&& tokens[tokenIndex+1].operator.type == Operator.Type.CALL;
				boolean nextIsFunctionIdentifier = tokenIndex < tokenCount-1
						&& tokens[tokenIndex+1].type == Token.Type.FUNCTION_IDENTIFIER;

				// Case of operators
				if (token.type == Token.Type.OPERATOR) {

					// Cases of unary-prefix operators
					if (token.operator.type == Operator.Type.UNARY_PREFIX) {

						// Only leafs, open parentheses, and unary-prefix operators can be operands.
						if ( !(  nextIsLeaf || nextIsOpenParenthesis || nextIsPrefixOperator  ) ) {
							throw new ExevalatorException("An operand is required at the right of: \"" + token.word + "\"");
						}
					} // Cases of unary-prefix operators

					// Cases of binary operators or a separator of partial expressions
					if (token.operator.type == Operator.Type.BINARY || token.word.equals(",")) {

						// Only leaf elements, open parenthesis, and unary-prefix operator can be a right-operand.
						if( !(  nextIsLeaf || nextIsOpenParenthesis || nextIsPrefixOperator || nextIsFunctionIdentifier ) ) {
							throw new ExevalatorException("An operand is required at the right of: \"" + token.word + "\"");
						}
						// Only leaf elements and closed parenthesis can be a right-operand.
						if( !(  prevIsLeaf || prevIsCloseParenthesis  ) ) {
							throw new ExevalatorException("An operand is required at the left of: \"" + token.word + "\"");
						}
					} // Cases of binary operators or a separator of partial expressions

				} // Case of operators

				// Case of leaf elements
				if (leafTypeSet.contains(token.type)) {

					// An other leaf element or an open parenthesis can not be at the right of an leaf element.
					if (!nextIsFunctionCallBegin && (nextIsOpenParenthesis || nextIsLeaf)) {
						throw new ExevalatorException("An operator is required at the right of: \"" + token.word + "\"");
					}

					// An other leaf element or a closed parenthesis can not be at the left of an leaf element.
					if (prevIsCloseParenthesis || prevIsLeaf) {
						throw new ExevalatorException("An operator is required at the left of: \"" + token.word + "\"");
					}
				} // Case of leaf elements
			} // Loops for each token
		} // End of this method
	}


	/**
	 * The class performing functions of a parser.
	 */
	private static final class Parser {

		/**
		 * Parses tokens and construct Abstract Syntax Tree (AST).
		 *
		 * @param tokens Tokens to be parsed
		 * @return The root node of the constructed AST
		 */
		public AstNode parse(Token[] tokens) {

			/* In this method, we use a non-recursive algorithm for the parsing.
			 * Processing cost is maybe O(N), where N is the number of tokens. */

			// Number of tokens
			int tokenCount = tokens.length;

			// Working stack to form multiple AstNode instances into a tree-shape.
			Deque<AstNode> stack = new ArrayDeque<AstNode>();

			// Temporarys node used in the above working stack, for isolating ASTs of partial expressions.
			AstNode parenthesisStackLid = new AstNode(new Token(Token.Type.STACK_LID, "(PARENTHESIS_STACK_LID)"));
			AstNode separatorStackLid = new AstNode(new Token(Token.Type.STACK_LID, "(SEPARATOR_STACK_LID)"));
			AstNode callBeginStackLid = new AstNode(new Token(Token.Type.STACK_LID, "(CALL_BEGIN_STACK_LID)"));

			// The array storing next operator's precedence for each token.
			// At [i], it is stored that the precedence of the first operator of which token-index is greater than i.
			int[] nextOperatorPrecedences = this.getNextOperatorPrecedences(tokens);

			// Read tokens from left to right.
			int itoken = 0;
			do {
				Token token = tokens[itoken];
				AstNode operatorNode = null;

				// Case of literals and identifiers: "1.23", "x", "f", etc.
				if (token.type == Token.Type.NUMBER_LITERAL
						|| token.type == Token.Type.VARIABLE_IDENTIFIER
						|| token.type == Token.Type.FUNCTION_IDENTIFIER) {
					stack.push(new AstNode(token));
					itoken++;
					continue;

				// Case of parenthesis: "(" or ")"
				} else if (token.type == Token.Type.PARENTHESIS) {
					if (token.word.equals("(")) {
						stack.push(parenthesisStackLid);
						itoken++;
						continue;
					} else { // Case of ")"
						operatorNode = this.popPartialExprNodes(stack, parenthesisStackLid)[0];
					}

				// Case of separator: ","
				} else if (token.type == Token.Type.EXPRESSION_SEPARATOR) {
					stack.push(separatorStackLid);
					itoken++;
					continue;

				// Case of operators: "+", "-", etc.
				} else if (token.type == Token.Type.OPERATOR) {
					operatorNode = new AstNode(token);
					int nextOpPrecedence = nextOperatorPrecedences[itoken];

					// Case of unary-prefix operators:
					// * Connect the node of right-token as an operand, if necessary (depending the next operator's precedence).
					if (token.operator.type == Operator.Type.UNARY_PREFIX) {
						if (this.shouldAddRightOperand(token.operator.precedence, nextOpPrecedence)) {
							operatorNode.childNodeList.add(new AstNode(tokens[itoken + 1]));
							itoken++;
						} // else: Operand will be connected later. See the bottom of this loop.

					// Case of binary operators:
					// * Always connect the node of left-token as an operand.
					// * Connect the node of right-token as an operand, if necessary (depending the next operator's precedence).
					} else if (token.operator.type == Operator.Type.BINARY) {
						operatorNode.childNodeList.add(stack.pop());
						if (this.shouldAddRightOperand(token.operator.precedence, nextOpPrecedence)) {
							operatorNode.childNodeList.add(new AstNode(tokens[itoken + 1]));
							itoken++;
						} // else: Right-operand will be connected later. See the bottom of this loop.

					// Case of function-call operators.
					} else if (token.operator.type == Operator.Type.CALL) {
						if (token.word.equals("(")) {
							operatorNode.childNodeList.add(stack.pop()); // Add function-identifier node at the top of the stack.
							stack.push(operatorNode);
							stack.push(callBeginStackLid); // The marker to correct partial expressions of args from the stack.
							itoken++;
							continue;
						} else { // Case of ")"
							AstNode[] argNodes = this.popPartialExprNodes(stack, callBeginStackLid);
							operatorNode = stack.pop();
							for (AstNode argNode: argNodes) {
								operatorNode.childNodeList.add(argNode);
							}
						}
					}
				}

				// If the precedence of the operator at the top of the stack is stronger than the next operator,
				// connect all "unconnected yet" operands and operators in the stack.
				while (this.shouldAddRightOperandToStackedOperator(stack, nextOperatorPrecedences[itoken])) {
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
		private boolean shouldAddRightOperand(int targetOperatorPrecedence, int nextOperatorPrecedence) {
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
		private boolean shouldAddRightOperandToStackedOperator(Deque<AstNode> stack, int nextOperatorPrecedence) {
			if (stack.size() == 0 || stack.peek().token.type != Token.Type.OPERATOR) {
				return false;
			}
			return this.shouldAddRightOperand(stack.peek().token.operator.precedence, nextOperatorPrecedence);
		}

		/**
		 * Pops root nodes of ASTs of partial expressions constructed on the stack.
		 *
		 * @param stack The working stack used for the parsing
		 * @param targetStackLidNode The temporary node pushed in the stack, at the end of partial expressions to be popped
		 * @return Root nodes of ASTs of partial expressions
		 */
		private AstNode[] popPartialExprNodes(Deque<AstNode> stack, AstNode endStackLidNode) {
			if (stack.size() == 0) {
				throw new ExevalatorException("Unexpected end of a partial expression");
			}
			List<AstNode> partialExprNodeList = new ArrayList<AstNode>();
			while(stack.size() != 0) {
				if (stack.peek().token.type == Token.Type.STACK_LID) {
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
		private int[] getNextOperatorPrecedences(Token[] tokens) {
			int tokenCount = tokens.length;
			int lastOperatorPrecedence = Integer.MAX_VALUE; // least prior
			int[] nextOperatorPrecedences = new int[tokenCount];

			for (int itoken=tokenCount-1; 0<=itoken; itoken--) {
				Token token = tokens[itoken];
				nextOperatorPrecedences[itoken] = lastOperatorPrecedence;

				if (token.type == Token.Type.OPERATOR) {
					lastOperatorPrecedence = token.operator.precedence;
				}

				if (token.type == Token.Type.PARENTHESIS) {
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
	 * The class storing information of an operator.
	 */
	private static final class Operator {

		/**
		 * The enum representing types of operators.
		 */
		public static enum Type {

			/** Represents unary operator, for example: - of -1.23 */
			UNARY_PREFIX,

			/** Represents binary operator, for example: + of 1+2 */
			BINARY,

			/** Represents function-call operator */
			CALL
		}

		/** The symbol of this operator (for example: '+'). */
		public final String symbol;

		/** The precedence of this operator (smaller value gives higher precedence). */
		public final int precedence;

		/** The type of operator tokens. */
		public final Type type;

		/**
		 * Create an Operator instance storing specified information.
		 *
		 * @param type The type of this operator
		 * @param symbol The symbol of this operator
		 * @param precedence The precedence of this operator
		 */
		public Operator(Type type, String symbol, int precedence) {
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
	 * The class storing information of an token.
	 */
	private static final class Token {

		/**
		 * The enum representing types of tokens.
		 */
		public static enum Type {

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

		/** The type of this token. */
		public final Type type;

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
		public Token(Type type, String word) {
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
		public Token(Type type, String word, Operator operator) {
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
	private static final class AstNode {

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
		 * Initializes the evaluator unit for evaluating the value of this AST node.
		 *
		 * @param interconnect The interconnect providing references to variables and functions.
		 */
		public void initializeEvaluatorUnit(Interconnect interconnect) {

			// Initialize evaluation units of child nodes, and store then into an array.
			int childCount = this.childNodeList.size();
			Evaluator.EvaluatorUnit childNodeUnits[] = new Evaluator.EvaluatorUnit[childCount];
			for (int ichild=0; ichild<childCount; ichild++) {
				this.childNodeList.get(ichild).initializeEvaluatorUnit(interconnect);
				childNodeUnits[ichild] = this.childNodeList.get(ichild).evaluatorUnit;
			}

			// Initialize evaluation units of this node.
			if (this.token.type == Token.Type.NUMBER_LITERAL) {
				this.evaluatorUnit = new Evaluator.NumberLiteralEvaluatorUnit(this.token.word);
			} else if (this.token.type == Token.Type.VARIABLE_IDENTIFIER) {
				this.evaluatorUnit = new Evaluator.VariableValueEvaluatorUnit(this.token.word, interconnect);
			} else if (this.token.type == Token.Type.OPERATOR) {
				if (this.token.operator == StaticSettings.MINUS_OPERATOR) {
					this.evaluatorUnit = new Evaluator.MinusEvaluatorUnit(childNodeUnits[0]);
				} else if (this.token.operator == StaticSettings.ADDITION_OPERATOR) {
					this.evaluatorUnit = new Evaluator.AdditionEvaluatorUnit(childNodeUnits[0], childNodeUnits[1]);
				} else if (this.token.operator == StaticSettings.SUBTRACTION_OPERATOR) {
					this.evaluatorUnit = new Evaluator.SubtractionEvaluatorUnit(childNodeUnits[0], childNodeUnits[1]);
				} else if (this.token.operator == StaticSettings.MULTIPLICATION_OPERATOR) {
					this.evaluatorUnit = new Evaluator.MultiplicationEvaluatorUnit(childNodeUnits[0], childNodeUnits[1]);
				} else if (this.token.operator == StaticSettings.DIVISION_OPERATOR) {
					this.evaluatorUnit = new Evaluator.DivisionEvaluatorUnit(childNodeUnits[0], childNodeUnits[1]);
				}
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
			if (this.token.type == Token.Type.OPERATOR) {
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
	private static final class Evaluator {

		/**
		 * The super class of evaluator units.
		 */
		private static abstract class EvaluatorUnit {

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
		private static abstract class BinaryOperationEvaluatorUnit extends EvaluatorUnit {

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
		private static final class AdditionEvaluatorUnit extends BinaryOperationEvaluatorUnit {

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
		private static final class SubtractionEvaluatorUnit extends BinaryOperationEvaluatorUnit {

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
		private static final class MultiplicationEvaluatorUnit extends BinaryOperationEvaluatorUnit {

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
		private static final class DivisionEvaluatorUnit extends BinaryOperationEvaluatorUnit {

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
		private static final class MinusEvaluatorUnit extends EvaluatorUnit {

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
		private static final class NumberLiteralEvaluatorUnit extends EvaluatorUnit {

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
					throw new ExevalatorException("Invalid number literal: " + literal);
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
		private static final class VariableValueEvaluatorUnit extends EvaluatorUnit {

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
					throw new ExevalatorException("No variable found: " + variableName);
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
	}


	/**
	 * The super class of variables available on this interpreter.
	 */
	public static abstract class AbstractVariable {

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
	public static abstract class AbstractFunction {

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
	 * The class to provide objects shared among multiple components of this interpreter.
	 */
	public static class Interconnect {

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
	}


	/**
	 * The class defining static setting values.
	 */
	private static final class StaticSettings {

		/** The indent used in text representations of ASTs. */
		public static final String AST_INDENT = "  ";

		/** The regular expression of number literals. */
		public static final String NUMBER_LITERAL_REGEX =
			"([0-9]+(\\.[0-9]+)?)" +    // Significand part
			"((e|E)(\\+|-)?[0-9]+)?";   // Exponent part

		/** The escaped representation of number literals in expressions */
		public static final String ESCAPED_NUMBER_LITERAL = "@NUMBER_LITERAL@";

		/** The instance of addition operator. */
		public static final Operator ADDITION_OPERATOR = new Operator(Operator.Type.BINARY, "+", 400);

		/** The instance of subtraction operator. */
		public static final Operator SUBTRACTION_OPERATOR = new Operator(Operator.Type.BINARY, "-", 400);

		/** The instance of multiplication operator. */
		public static final Operator MULTIPLICATION_OPERATOR = new Operator(Operator.Type.BINARY, "*", 300);

		/** The instance of division operator. */
		public static final Operator DIVISION_OPERATOR = new Operator(Operator.Type.BINARY, "/", 300);

		/** The instance of unary-minus operator. */
		public static final Operator MINUS_OPERATOR = new Operator(Operator.Type.UNARY_PREFIX, "-", 200);

		/** The instance of the beginning of call operator. */
		public static final Operator CALL_BEGIN_OPERATOR = new Operator(Operator.Type.CALL, "(", 100);

		/** The instance of the end of call operator. */
		public static final Operator CALL_END_OPERATOR = new Operator(Operator.Type.CALL, ")", Integer.MAX_VALUE); // least prior

		/** The list of available operators. */
		public static final List<Operator> OPERATOR_LIST = new ArrayList<Operator>();
		static {
			OPERATOR_LIST.add(ADDITION_OPERATOR);
			OPERATOR_LIST.add(SUBTRACTION_OPERATOR);
			OPERATOR_LIST.add(MULTIPLICATION_OPERATOR);
			OPERATOR_LIST.add(DIVISION_OPERATOR);
			OPERATOR_LIST.add(MINUS_OPERATOR);
			OPERATOR_LIST.add(CALL_BEGIN_OPERATOR);
			OPERATOR_LIST.add(CALL_END_OPERATOR);
		}

		/** The set of symbols of available operators. */
		@SuppressWarnings("serial")
		public static final Set<String> OPERATOR_SYMBOL_SET = new HashSet<String>() {{
			add("+");
			add("-");
			add("*");
			add("/");
			add("(");
			add(")");
		}};

		/** The list of symbols to split an expression into tokens. */
		public static final List<String> TOKEN_SPLITTER_SYMBOL_LIST = new ArrayList<String>();
		static {
			TOKEN_SPLITTER_SYMBOL_LIST.add("+");
			TOKEN_SPLITTER_SYMBOL_LIST.add("-");
			TOKEN_SPLITTER_SYMBOL_LIST.add("*");
			TOKEN_SPLITTER_SYMBOL_LIST.add("/");
			TOKEN_SPLITTER_SYMBOL_LIST.add("(");
			TOKEN_SPLITTER_SYMBOL_LIST.add(")");
			TOKEN_SPLITTER_SYMBOL_LIST.add(",");
		};

		/**
		 * Search an available Operator having specified information.
		 *
		 * @param type The type of the operator to be searched
		 * @param symbol The symbol of the operator to be searched
		 * @return The Operator matching specified conditions
		 */
		public static final Operator searchOperator(Operator.Type type, String symbol) {
			for (Operator operator: OPERATOR_LIST) {
				if (operator.type == type && operator.symbol.equals(symbol)) {
					return operator;
				}
			}
			throw new ExevalatorException("No operator found: (" + type + ", " + symbol + ")");
		}
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
	}
}
