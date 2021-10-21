/**
 * The class for testing Exevalator.
 */
public class Test {

	/** The minimum error between two double-type values to regard them almost equal. */
	private static final double ALLOWABLE_ERROR = 1.0E-12;


	/**
	 * The entry point of testings.
	 *
	 * @param args command-line arguments
	 */
	public static void main(String[] args) {
		Test test = new Test();
		test.testNumberLiterals();
		test.testOperationsOfOperators();
		test.testPrecedencesOfOperators();
		test.testParentheses();

		System.out.println("All tests have completed successfully.");
	}


	/**
	 * Tests number literals.
	 */
	private void testNumberLiterals() {
		Exevalator exevalator = new Exevalator();

		check(
			"Test of a Simple Number Literal 1",
			exevalator.eval("1"),
			1
		);

		check(
			"Test of a Simple Number Literal 2",
			exevalator.eval("2"),
			2
		);

		check(
			"Test of a Simple Number Literal 3",
			exevalator.eval("1.2"),
			1.2
		);
	}


	/**
	 * Tests each kind of operators.
	 */
	private void testOperationsOfOperators() {
		Exevalator exevalator = new Exevalator();

		check(
			"Test of Addition Operator",
			exevalator.eval("1.2 + 3.4"),
			1.2 + 3.4
		);

		check(
			"Test of Subtraction Operator",
			exevalator.eval("1.2 - 3.4"),
			1.2 - 3.4
		);

		check(
			"Test of Multiplication Operator",
			exevalator.eval("1.2 * 3.4"),
			1.2 * 3.4
		);

		check(
			"Test of Division Operator",
			exevalator.eval("1.2 / 3.4"),
			1.2 / 3.4
		);

		check(
			"Test of Unary Minus Operator",
			exevalator.eval("-1.2"),
			-1.2
		);
	}


	/**
	 * Tests precedences of operators.
	 */
	private void testPrecedencesOfOperators() {
		Exevalator exevalator = new Exevalator();

		check(
			"Test of Precedences of Operators 1",
			exevalator.eval("1.2 + 3.4 + 5.6 + 7.8"),
			1.2 + 3.4 + 5.6 + 7.8
		);

		check(
			"Test of Precedences of Operators 2",
			exevalator.eval("1.2 + 3.4 - 5.6 + 7.8"),
			1.2 + 3.4 - 5.6 + 7.8
		);

		check(
			"Test of Precedences of Operators 3",
			exevalator.eval("1.2 + 3.4 * 5.6 + 7.8"),
			1.2 + 3.4 * 5.6 + 7.8
		);

		check(
			"Test of Precedences of Operators 4",
			exevalator.eval("1.2 + 3.4 / 5.6 + 7.8"),
			1.2 + 3.4 / 5.6 + 7.8
		);

		check(
			"Test of Precedences of Operators 5",
			exevalator.eval("1.2 * 3.4 + 5.6 + 7.8"),
			1.2 * 3.4 + 5.6 + 7.8
		);

		check(
			"Test of Precedences of Operators 6",
			exevalator.eval("1.2 * 3.4 - 5.6 + 7.8"),
			1.2 * 3.4 - 5.6 + 7.8
		);

		check(
			"Test of Precedences of Operators 7",
			exevalator.eval("1.2 * 3.4 * 5.6 + 7.8"),
			1.2 * 3.4 * 5.6 + 7.8
		);

		check(
			"Test of Precedences of Operators 8",
			exevalator.eval("1.2 * 3.4 / 5.6 + 7.8"),
			1.2 * 3.4 / 5.6 + 7.8
		);

		check(
			"Test of Precedences of Operators 9",
			exevalator.eval("1.2 + 3.4 + 5.6 * 7.8"),
			1.2 + 3.4 + 5.6 * 7.8
		);

		check(
			"Test of Precedences of Operators 10",
			exevalator.eval("1.2 + 3.4 - 5.6 * 7.8"),
			1.2 + 3.4 - 5.6 * 7.8
		);

		check(
			"Test of Precedences of Operators 11",
			exevalator.eval("1.2 + 3.4 * 5.6 * 7.8"),
			1.2 + 3.4 * 5.6 * 7.8
		);

		check(
			"Test of Precedences of Operators 12",
			exevalator.eval("1.2 + 3.4 / 5.6 * 7.8"),
			1.2 + 3.4 / 5.6 * 7.8
		);

		check(
			"Test of Precedences of Operators 13",
			exevalator.eval("-1.2 + 3.4 / 5.6 * 7.8"),
			-1.2 + 3.4 / 5.6 * 7.8
		);

		check(
			"Test of Precedences of Operators 14",
			exevalator.eval("1.2 + 3.4 / -5.6 * 7.8"),
			1.2 + 3.4 / 5.6 * -7.8
		);

		check(
			"Test of Precedences of Operators 15",
			exevalator.eval("1.2 + 3.4 / 5.6 * -7.8"),
			1.2 + 3.4 / 5.6 * -7.8
		);
	}


	/**
	 * Tests parentheses.
	 */
	private void testParentheses() {
		Exevalator exevalator = new Exevalator();

		check(
			"Test of Parentheses 1",
			exevalator.eval("(1.2 + 3.4)"),
			(1.2 + 3.4)
		);

		check(
			"Test of Parentheses 2",
			exevalator.eval("(1.2 + 3.4) + 5.6"),
			(1.2 + 3.4) + 5.6
		);

		check(
			"Test of Parentheses 3",
			exevalator.eval("1.2 + (3.4 + 5.6)"),
			1.2 + (3.4 + 5.6)
		);

		check(
			"Test of Parentheses 4",
			exevalator.eval("1.2 + -(3.4 + 5.6)"),
			1.2 + -(3.4 + 5.6)
		);

		check(
			"Test of Parentheses 5",
			exevalator.eval("1.2 + -(-3.4 + 5.6)"),
			1.2 + -(-3.4 + 5.6)
		);

		check(
			"Test of Parentheses 4",
			exevalator.eval("(1.2 * 3.4) + 5.6"),
			(1.2 * 3.4) + 5.6
		);

		check(
			"Test of Parentheses 5",
			exevalator.eval("(1.2 + 3.4) * 5.6"),
			(1.2 + 3.4) * 5.6
		);

		check(
			"Test of Parentheses 6",
			exevalator.eval("1.2 + (3.4 * 5.6)"),
			1.2 + (3.4 * 5.6)
		);

		check(
			"Test of Parentheses 7",
			exevalator.eval("1.2 + (3.4 * 5.6) + 7.8"),
			1.2 + (3.4 * 5.6) + 7.8
		);

		check(
			"Test of Parentheses 8",
			exevalator.eval("1.2 * (3.4 + 5.6) / 7.8"),
			1.2 * (3.4 + 5.6) / 7.8
		);

		check(
			"Test of Parentheses 9",
			exevalator.eval("(1.2 + 3.4 - 5.6) * ((7.8 + 9.0) / 10.1)"),
			(1.2 + 3.4 - 5.6) * ((7.8 + 9.0) / 10.1)
		);

		check(
			"Test of Parentheses 10",
			exevalator.eval("(-(1.2 + 3.4 - 5.6) * ((7.8 + 9.0) / 10.1) / 11.2 + 12.3 * ((13.4 + -(15.6 - 17.8)) * 18.9)) + 19.0 * 20.1"),
			(-(1.2 + 3.4 - 5.6) * ((7.8 + 9.0) / 10.1) / 11.2 + 12.3 * ((13.4 + -(15.6 - 17.8)) * 18.9)) + 19.0 * 20.1
		);
	}


	/**
	 * Checks the evaluated (computed) value of the testing expression by the Exevalator.
	 *
	 * @param evaluatedValue The evaluated (computed) value of the testing expression by Exevalator
	 * @param correctValue The correct value of the testing expression
	 * @param testName The name of the testing
	 */
	private static void check(String testName, double evaluatedValue, double correctValue) {
		if (StrictMath.abs(evaluatedValue - correctValue) < ALLOWABLE_ERROR) {
			System.out.println(testName + ": OK.");
			return;
		}
		throw new ExevalatorTestException(
			"\"" + testName + "\" has failed. " +
			"evaluatedValue=" + evaluatedValue + ", " +
			"correctValue=" + correctValue + "."
		);
	}


	/**
	 * The Exception class thrown when any test has failed.
	 */
	@SuppressWarnings("serial")
	public static class ExevalatorTestException extends RuntimeException {

		/**
		 * Create an instance having the specified error message.
		 *
		 * @param errorMessage The error message explaining the cause of this exception
		 */
		public ExevalatorTestException(String errorMessage) {
			super(errorMessage);
		}
	}
}
