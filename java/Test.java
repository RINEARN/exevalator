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
		test.testComplicatedCases();
		test.testSyntaxChecksOfCorresponencesOfParentheses();
		test.testSyntaxChecksOfLocationsOfOperatorsAndLeafs();

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

		check(
			"Test of a Number Literal with a Exponent Part 1",
			exevalator.eval("1.2E3"),
			1.2E3
		);

		check(
			"Test of a Number Literal with a Exponent Part 2",
			exevalator.eval("1.2E+3"),
			1.2E3
		);

		check(
			"Test of a Number Literal with a Exponent Part 3",
			exevalator.eval("1.2E-3"),
			1.2E-3
		);

		check(
			"Test of a Number Literal with a Exponent Part 4",
			exevalator.eval("123.4567E12"),
			123.4567E12
		);

		check(
			"Test of a Number Literal with a Exponent Part 5",
			exevalator.eval("123.4567E+12"),
			123.4567E+12
		);

		check(
			"Test of a Number Literal with a Exponent Part 6",
			exevalator.eval("123.4567E-12"),
			123.4567E-12
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
	 * Tests of complicated cases.
	 */
	private void testComplicatedCases() {
		Exevalator exevalator = new Exevalator();

		check(
			"Test of Complicated Case 1: The Expression Containing Many Parentheses and Many Literals having Exponent Parts",
			exevalator.eval("(-(1.2E1 + 3.4E-2 - 5.6E2) * ((7.8E0 + 9.0) / 10.1E-3) / 11.2 + 12.3E-1 * ((13.4 + -(15.6E-12 - 17.8E-10)) * 18.9E-5)) + 19.0E-2 * 20.1E0"),
			(-(1.2E1 + 3.4E-2 - 5.6E2) * ((7.8E0 + 9.0) / 10.1E-3) / 11.2 + 12.3E-1 * ((13.4 + -(15.6E-12 - 17.8E-10)) * 18.9E-5)) + 19.0E-2 * 20.1E0
		);
	}


	/**
	 * Tests syntax checks by the interpreter for expressions.
	 */
	private void testSyntaxChecksOfCorresponencesOfParentheses() {
		Exevalator exevalator = new Exevalator();

		check(
			"Test of Detection of Mismatching of Open/Closed Parentheses 1",
			exevalator.eval("(1 + 2)"),
			(1 + 2)
		);

		try {
			check(
				"Test of Detection of Mismatching of Open/Closed Parentheses 2",
				exevalator.eval("((1 + 2)"),
				(1 + 2)
			);
			throw new ExevalatorTestException("Expected exception has not been thrown");
		} catch (Exevalator.Exception ee) {
			// Expected to thrown
			System.out.println("Test of Detection of Mismatching of Open/Closed Parentheses 2: OK.");
		}

		try {
			check(
				"Test of Detection of Mismatching of Open/Closed Parentheses 3",
				exevalator.eval("(1 + 2))"),
				(1 + 2)
			);
			throw new ExevalatorTestException("Expected exception has not been thrown");
		} catch (Exevalator.Exception ee) {
			// Expected to thrown
			System.out.println("Test of Detection of Mismatching of Open/Closed Parentheses 3: OK.");
		}

		check(
			"Test of Detection of Mismatching of Open/Closed Parentheses 4",
			exevalator.eval("(1 + 2) + (3 + 4)"),
			(1 + 2) + (3 + 4)
		);

		try {
			check(
				"Test of Detection of Mismatching of Open/Closed Parentheses 5",
				exevalator.eval("1 + 2) + (3 + 4"),
				(1 + 2) + (3 + 4)
			);
			throw new ExevalatorTestException("Expected exception has not been thrown");
		} catch (Exevalator.Exception ee) {
			// Expected to thrown
			System.out.println("Test of Detection of Mismatching of Open/Closed Parentheses 5: OK.");
		}

		check(
			"Test of Detection of Mismatching of Open/Closed Parentheses 6",
			exevalator.eval("1 + ((2 + (3 + 4) + 5) + 6)"),
			1 + ((2 + (3 + 4) + 5) + 6)
		);

		try {
			check(
				"Test of Detection of Mismatching of Open/Closed Parentheses 7",
				exevalator.eval("1 + ((2 + (3 + 4) + 5) + 6"),
				1 + ((2 + (3 + 4) + 5) + 6)
			);
			throw new ExevalatorTestException("Expected exception has not been thrown");
		} catch (Exevalator.Exception ee) {
			// Expected to thrown
			System.out.println("Test of Detection of Mismatching of Open/Closed Parentheses 7: OK.");
		}

		try {
			check(
				"Test of Detection of Mismatching of Open/Closed Parentheses 8",
				exevalator.eval("1 + (2 + (3 + 4) + 5) + 6)"),
				1 + ((2 + (3 + 4) + 5) + 6)
			);
			throw new ExevalatorTestException("Expected exception has not been thrown");
		} catch (Exevalator.Exception ee) {
			// Expected to thrown
			System.out.println("Test of Detection of Mismatching of Open/Closed Parentheses 8: OK.");
		}

		try {
			check(
				"Test of Detection of Empty Parentheses 1",
				exevalator.eval("()"),
				Double.NaN
			);
			throw new ExevalatorTestException("Expected exception has not been thrown");
		} catch (Exevalator.Exception ee) {
			// Expected to thrown
			System.out.println("Test of Detection of Empty Parentheses 1: OK.");
		}

		try {
			check(
				"Test of Detection of Empty Parentheses 2",
				exevalator.eval("1 + ()"),
				Double.NaN
			);
			throw new ExevalatorTestException("Expected exception has not been thrown");
		} catch (Exevalator.Exception ee) {
			// Expected to thrown
			System.out.println("Test of Detection of Empty Parentheses 2: OK.");
		}

		try {
			check(
				"Test of Detection of Empty Parentheses 3",
				exevalator.eval("() + 1"),
				Double.NaN
			);
			throw new ExevalatorTestException("Expected exception has not been thrown");
		} catch (Exevalator.Exception ee) {
			// Expected to thrown
			System.out.println("Test of Detection of Empty Parentheses 3: OK.");
		}
	}


	/**
	 * Tests syntax checks by the interpreter for locations of operators and leaf elements (literals and identifiers).
	 */
	private void testSyntaxChecksOfLocationsOfOperatorsAndLeafs() {
		Exevalator exevalator = new Exevalator();

		check(
			"Test of Detection of Left Operand of Unary-Prefix Operator 1",
			exevalator.eval("1 + -123"),
			1 + -123
		);

		try {
			check(
				"Test of Detection of Left Operand of Unary-Prefix Operator 2",
				exevalator.eval("1 + -"),
				1 + -123
			);
			throw new ExevalatorTestException("Expected exception has not been thrown");
		} catch (Exevalator.Exception ee) {
			// Expected to thrown
			System.out.println("Test of Detection of Left Operand of Unary-Prefix Operator 2: OK.");
		}

		try {
			check(
				"Test of Detection of Left Operand of Unary-Prefix Operator 3",
				exevalator.eval("(1 + -)"),
				1 + -123
			);
			throw new ExevalatorTestException("Expected exception has not been thrown");
		} catch (Exevalator.Exception ee) {
			// Expected to thrown
			System.out.println("Test of Detection of Left Operand of Unary-Prefix Operator 3: OK.");
		}

		check(
			"Test of Detection of Left Operand of Binary Operator 1",
			exevalator.eval("123 + 456"),
			123 + 456
		);

		try {
			check(
				"Test of Detection of Left Operand of Binary Operator 2",
				exevalator.eval("123 *"),
				123 * 456
			);
			throw new ExevalatorTestException("Expected exception has not been thrown");
		} catch (Exevalator.Exception ee) {
			// Expected to thrown
			System.out.println("Test of Detection of Left Operand of Binary Operator 2: OK.");
		}

		try {
			check(
				"Test of Detection of Left Operand of Binary Operator 3",
				exevalator.eval("* 456"),
				123 * 456
			);
			throw new ExevalatorTestException("Expected exception has not been thrown");
		} catch (Exevalator.Exception ee) {
			// Expected to thrown
			System.out.println("Test of Detection of Left Operand of Binary Operator 3: OK.");
		}

		try {
			check(
				"Test of Detection of Left Operand of Binary Operator 4",
				exevalator.eval("123 + ( * 456)"),
				123 * 456
			);
			throw new ExevalatorTestException("Expected exception has not been thrown");
		} catch (Exevalator.Exception ee) {
			// Expected to thrown
			System.out.println("Test of Detection of Left Operand of Binary Operator 4: OK.");
		}

		try {
			check(
				"Test of Detection of Left Operand of Binary Operator 5",
				exevalator.eval("(123 *) + 456"),
				123 * 456
			);
			throw new ExevalatorTestException("Expected exception has not been thrown");
		} catch (Exevalator.Exception ee) {
			// Expected to thrown
			System.out.println("Test of Detection of Left Operand of Binary Operator 5: OK.");
		}

		try {
			check(
				"Test of Detection of Lacking Operator",
				exevalator.eval("123 456"),
				123 * 456
			);
			throw new ExevalatorTestException("Expected exception has not been thrown");
		} catch (Exevalator.Exception ee) {
			// Expected to thrown
			System.out.println("Test of Detection of Lacking Operator: OK.");
		}

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
