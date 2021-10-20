/*
 * Copyright(C) 2021 RINEARN (Fumihiro Matsui)
 * This software is released under the MIT License.
 */

/*
 * Put this code in your source code folder, and write the package-statement as follows.
 */
// package your.projects.package.anywhere;

import java.util.List;
import java.util.Set;
import java.util.ArrayList;
import java.util.HashSet;


/**
 * Interpreter Engine of Exevalator.
 */
public class Exevalator {

	/**
	 * Evaluates (computes) the value of an expression.
	 *
	 * @param expression The expression to be evaluated
	 * @return The evaluated value
	 */
	public double eval(String expression) {
		/*
		// Temporary, for debugging
		Token[] tokens = new LexicalAnalyzer().analyze(expression);
		for (Token token: tokens) {
			System.out.println(token.toString());
		}
		*/
		return Double.NaN;
	}


	/**
	 * The class performing functions of a lexical analyzer.
	 */
	private static class LexicalAnalyzer {

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
				if (lastToken.type == Token.Type.NUMBER_LITERAL
						|| lastToken.type == Token.Type.IDENTIFIER
						|| lastToken.word.equals(")")) {
					operator = StaticSettings.searchOperator(Operator.Type.BINARY, token.word);
				} else {
					operator = StaticSettings.searchOperator(Operator.Type.UNARY, token.word);
				}
				tokens[itoken] = new Token(token.type, token.word, operator);
				lastToken = token;
			}
			return tokens;
		}
	}


	/**
	 * The class storing information of an operator.
	 */
	private static class Operator {

		/**
		 * The enum representing types of operators.
		 */
		private static enum Type {

			/** Represents unary operator, for example: - of -1.23 */
			UNARY,

			/** Represents binary operator, for example: + of 1+2 */
			BINARY,
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
	private static class Token {

		/**
		 * The enum representing types of tokens.
		 */
		private static enum Type {

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
	private static class AstNode {

		/** The token corresponding with this AST node. */
		public final Token token;

		/** The list of child nodes of this node. */
		public final List<AstNode> childNodeList;

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
	 * The class defining static setting values.
	 */
	private static class StaticSettings {

		/** The indent used in text representations of ASTs. */
		private static final String AST_INDENT = "  ";

		/** The regular expression of number literals. */
		private static final String NUMBER_LITERAL_REGEX =
			"^" +                       // Begin
			"([0-9]+\\.?[0-9]+)" +      // Significand part
			"((e|E)(\\+|-)?[0-9]+)?" +  // Exponent part
			"$";                        // End

		/** The list of available operators. */
		private static final List<Operator> OPERATOR_LIST = List.of(
			new Operator(Operator.Type.BINARY, "+", 300), // Addition
			new Operator(Operator.Type.BINARY, "-", 300), // Subtraction
			new Operator(Operator.Type.BINARY, "*", 200), // Multiplication
			new Operator(Operator.Type.BINARY, "/", 200), // Division
			new Operator(Operator.Type.UNARY,  "-", 100)  // Unary Minus
		);

		/** The set of symbols of available operators. */
		@SuppressWarnings("serial")
		private static final Set<String> OPERATOR_SYMBOL_SET = new HashSet<String>() {{
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
		private static final Operator searchOperator(Operator.Type type, String symbol) {
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
	public static class ExevalatorException extends RuntimeException {

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
