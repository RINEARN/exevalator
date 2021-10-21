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


/**
 * Interpreter Engine of Exevalator.
 */
public final class Exevalator {

	/**
	 * Evaluates (computes) the value of an expression.
	 *
	 * @param expression The expression to be evaluated
	 * @return The evaluated value
	 */
	public double eval(String expression) {

		// Split the expression into tokens, and analyze them.
		Token[] tokens = new LexicalAnalyzer().analyze(expression);

		// Construct AST (Abstract Syntax Tree) by parsing tokens.
		AstNode ast = new Parser().parse(tokens);

		// Evaluate (compute) the value of the root node of the AST.
		ast.initializeEvaluatorUnit();
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

			// Split (tokenize) the expression into token words.
			expression = expression.replace("(", " ( ");
			expression = expression.replace(")", " ) ");
			for (Operator operator: StaticSettings.OPERATOR_LIST) {
				String symbol = operator.symbol;
				expression = expression.replace(symbol, " " + symbol + " ");
			}
			String[] tokenWords = expression.trim().split("\\s+");
			int tokenCount = tokenWords.length;

			// Create Token instances.
			Token[] tokens = new Token[tokenCount];
			for (int itoken=0; itoken<tokenCount; itoken++) {
				String word = tokenWords[itoken];

				if (word.equals("(") || word.equals(")")) {
					tokens[itoken] = new Token(Token.Type.PARENTHESIS, word);
				} else if (StaticSettings.OPERATOR_SYMBOL_SET.contains(word)) {
					tokens[itoken] = new Token(Token.Type.OPERATOR, word);
				} else if (word.matches(StaticSettings.NUMBER_LITERAL_REGEX)) {
					tokens[itoken] = new Token(Token.Type.NUMBER_LITERAL, word);
				} else {
					tokens[itoken] = new Token(Token.Type.IDENTIFIER, word);
				}
			}

			// Analyze detailed information for operator tokens.
			Token lastToken = null;
			for (int itoken=0; itoken<tokenCount; itoken++) {
				Token token = tokens[itoken];
				if (token.type != Token.Type.OPERATOR) {
					lastToken = token;
					continue;
				}
				Operator operator = null;
				if (lastToken == null
						|| lastToken.type == Token.Type.OPERATOR
						|| lastToken.word.equals("(")) {
					operator = StaticSettings.searchOperator(Operator.Type.UNARY, token.word);
				} else if (lastToken.type == Token.Type.NUMBER_LITERAL
						|| lastToken.type == Token.Type.IDENTIFIER
						|| lastToken.word.equals(")")) {
					operator = StaticSettings.searchOperator(Operator.Type.BINARY, token.word);
				} else {
					throw new ExevalatorException("Unexpected operator syntax: " + token.word);
				}
				tokens[itoken] = new Token(token.type, token.word, operator);
				lastToken = token;
			}
			return tokens;
		}
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

			// Temporary node used in the above working stack, for isolating ASTs of partial expressions.
			AstNode parenthesisStackLid = new AstNode(new Token(Token.Type.STACK_LID, "(PARENTHESIS_STACK_LID)"));

			// The array storing next operator's precedence for each token.
			// At [i], it is stored that the precedence of the first operator of which token-index is greater than i.
			int[] nextOperatorPrecedences = this.getNextOperatorPrecedences(tokens);

			// Read tokens from left to right.
			int itoken = 0;
			do {
				Token token = tokens[itoken];
				AstNode operatorNode = null;

				// Case of operands: "1.23", "x", etc.
				if (token.type == Token.Type.NUMBER_LITERAL || token.type == Token.Type.IDENTIFIER) {
					stack.push(new AstNode(token));
					itoken++;
					continue;

				// Case of parenthesis: "(" or ")"
				} else if (token.type == Token.Type.PARENTHESIS) {
					if (token.word.equals("(")) {
						stack.push(parenthesisStackLid);
						itoken++;
						continue;
					} else {
						operatorNode = this.popPartialExprNodes(stack, parenthesisStackLid)[0];
					}

				// Case of operators: "+", "-", etc.
				} else if (token.type == Token.Type.OPERATOR) {
					operatorNode = new AstNode(token);
					int nextOpPrecedence = nextOperatorPrecedences[itoken];

					// Case of unary-prefix operators:
					// * Connect the node of right-token as an operand, if necessary (depending the next operator's precedence).
					if (token.operator.type == Operator.Type.UNARY) {
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
			UNARY,

			/** Represents binary operator, for example: + of 1+2 */
			BINARY
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

			/** Represents parenthesis, for example: ( and ) of (1*(2+3)) */
			PARENTHESIS,

			/** Represents identifier tokens, for example: x */
			IDENTIFIER,

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
		 */
		public void initializeEvaluatorUnit() {

			// Initialize evaluation units of child nodes, and store then into an array.
			int childCount = this.childNodeList.size();
			Evaluator.EvaluatorUnit childNodeUnits[] = new Evaluator.EvaluatorUnit[childCount];
			for (int ichild=0; ichild<childCount; ichild++) {
				this.childNodeList.get(ichild).initializeEvaluatorUnit();
				childNodeUnits[ichild] = this.childNodeList.get(ichild).evaluatorUnit;
			}

			// Initialize evaluation units of this node.
			if (this.token.type == Token.Type.NUMBER_LITERAL) {
				this.evaluatorUnit = new Evaluator.NumberLiteralEvaluatorUnit(this.token.word);
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
	}


	/**
	 * The class defining static setting values.
	 */
	private static final class StaticSettings {

		/** The indent used in text representations of ASTs. */
		public static final String AST_INDENT = "  ";

		/** The regular expression of number literals. */
		public static final String NUMBER_LITERAL_REGEX =
			"^" +                       // Begin
			"([0-9]+\\.?[0-9]+)" +      // Significand part
			"((e|E)(\\+|-)?[0-9]+)?" +  // Exponent part
			"$";                        // End

		/** The instance of addition operator. */
		public static final Operator ADDITION_OPERATOR = new Operator(Operator.Type.BINARY, "+", 300);

		/** The instance of subtraction operator. */
		public static final Operator SUBTRACTION_OPERATOR = new Operator(Operator.Type.BINARY, "-", 300);

		/** The instance of multiplication operator. */
		public static final Operator MULTIPLICATION_OPERATOR = new Operator(Operator.Type.BINARY, "*", 200);

		/** The instance of division operator. */
		public static final Operator DIVISION_OPERATOR = new Operator(Operator.Type.BINARY, "/", 200);

		/** The instance of unary-minus operator. */
		public static final Operator MINUS_OPERATOR = new Operator(Operator.Type.UNARY, "-", 100);

		/** The list of available operators. */
		public static final List<Operator> OPERATOR_LIST = List.of(
			ADDITION_OPERATOR,
			SUBTRACTION_OPERATOR,
			MULTIPLICATION_OPERATOR,
			DIVISION_OPERATOR,
			MINUS_OPERATOR
		);

		/** The set of symbols of available operators. */
		@SuppressWarnings("serial")
		public static final Set<String> OPERATOR_SYMBOL_SET = new HashSet<String>() {{
			add("+");
			add("-");
			add("*");
			add("/");
		}};

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
