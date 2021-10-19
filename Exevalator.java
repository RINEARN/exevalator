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
		return Double.NaN;
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
			IDENTIFIER
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
	 * The class defining static setting values.
	 */
	private static class StaticSettings {

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

		/** Search an available Operator having specified information. */
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
		 */
		public ExevalatorException(String errorMessage) {
			super(errorMessage);
		}
	}
}
