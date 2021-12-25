using System;
using Rinearn.ExevalatorCS;

class Test
{
    // The minimum error between two double-type values to regard them almost equal.
    private const double AllowableError = 1.0E-12;

    static void Main()
    {
        TestNumberLiterals();
        TestOperationsOfOperators();
        TestPrecedencesOfOperators();
        TestParentheses();
        TestComplicatedCases();
        TestSyntaxChecksOfCorresponencesOfParentheses();
        TestSyntaxChecksOfLocationsOfOperatorsAndLeafs();

        Console.WriteLine("All tests have completed successfully.");
    }


    // Tests number literals.
    private static void TestNumberLiterals() {
        Exevalator exevalator = new Exevalator();

        Check(
            "Test of a Simple Number Literal 1",
            exevalator.Eval("1"),
            1.0
        );

        Check(
            "Test of a Simple Number Literal 2",
            exevalator.Eval("2"),
            2.0
        );

        Check(
            "Test of a Simple Number Literal 3",
            exevalator.Eval("1.2"),
            1.2
        );

        Check(
            "Test of a Number Literal with a Exponent Part 1",
            exevalator.Eval("1.2E3"),
            1.2E3
        );

        Check(
            "Test of a Number Literal with a Exponent Part 2",
            exevalator.Eval("1.2E+3"),
            1.2E3
        );

        Check(
            "Test of a Number Literal with a Exponent Part 3",
            exevalator.Eval("1.2E-3"),
            1.2E-3
        );

        Check(
            "Test of a Number Literal with a Exponent Part 4",
            exevalator.Eval("123.4567E12"),
            123.4567E12
        );

        Check(
            "Test of a Number Literal with a Exponent Part 5",
            exevalator.Eval("123.4567E+12"),
            123.4567E+12
        );

        Check(
            "Test of a Number Literal with a Exponent Part 6",
            exevalator.Eval("123.4567E-12"),
            123.4567E-12
        );
    }


    // Tests each kind of operators.
    private static void TestOperationsOfOperators() {
        Exevalator exevalator = new Exevalator();

        Check(
            "Test of Addition Operator",
            exevalator.Eval("1.2 + 3.4"),
            1.2 + 3.4
        );

        Check(
            "Test of Subtraction Operator",
            exevalator.Eval("1.2 - 3.4"),
            1.2 - 3.4
        );

        Check(
            "Test of Multiplication Operator",
            exevalator.Eval("1.2 * 3.4"),
            1.2 * 3.4
        );

        Check(
            "Test of Division Operator",
            exevalator.Eval("1.2 / 3.4"),
            1.2 / 3.4
        );

        Check(
            "Test of Unary Minus Operator",
            exevalator.Eval("-1.2"),
            -1.2
        );
    }


    // Tests precedences of operators.
    private static void TestPrecedencesOfOperators() {
        Exevalator exevalator = new Exevalator();

        Check(
            "Test of Precedences of Operators 1",
            exevalator.Eval("1.2 + 3.4 + 5.6 + 7.8"),
            1.2 + 3.4 + 5.6 + 7.8
        );

        Check(
            "Test of Precedences of Operators 2",
            exevalator.Eval("1.2 + 3.4 - 5.6 + 7.8"),
            1.2 + 3.4 - 5.6 + 7.8
        );

        Check(
            "Test of Precedences of Operators 3",
            exevalator.Eval("1.2 + 3.4 * 5.6 + 7.8"),
            1.2 + 3.4 * 5.6 + 7.8
        );

        Check(
            "Test of Precedences of Operators 4",
            exevalator.Eval("1.2 + 3.4 / 5.6 + 7.8"),
            1.2 + 3.4 / 5.6 + 7.8
        );

        Check(
            "Test of Precedences of Operators 5",
            exevalator.Eval("1.2 * 3.4 + 5.6 + 7.8"),
            1.2 * 3.4 + 5.6 + 7.8
        );

        Check(
            "Test of Precedences of Operators 6",
            exevalator.Eval("1.2 * 3.4 - 5.6 + 7.8"),
            1.2 * 3.4 - 5.6 + 7.8
        );

        Check(
            "Test of Precedences of Operators 7",
            exevalator.Eval("1.2 * 3.4 * 5.6 + 7.8"),
            1.2 * 3.4 * 5.6 + 7.8
        );

        Check(
            "Test of Precedences of Operators 8",
            exevalator.Eval("1.2 * 3.4 / 5.6 + 7.8"),
            1.2 * 3.4 / 5.6 + 7.8
        );

        Check(
            "Test of Precedences of Operators 9",
            exevalator.Eval("1.2 + 3.4 + 5.6 * 7.8"),
            1.2 + 3.4 + 5.6 * 7.8
        );

        Check(
            "Test of Precedences of Operators 10",
            exevalator.Eval("1.2 + 3.4 - 5.6 * 7.8"),
            1.2 + 3.4 - 5.6 * 7.8
        );

        Check(
            "Test of Precedences of Operators 11",
            exevalator.Eval("1.2 + 3.4 * 5.6 * 7.8"),
            1.2 + 3.4 * 5.6 * 7.8
        );

        Check(
            "Test of Precedences of Operators 12",
            exevalator.Eval("1.2 + 3.4 / 5.6 * 7.8"),
            1.2 + 3.4 / 5.6 * 7.8
        );

        Check(
            "Test of Precedences of Operators 13",
            exevalator.Eval("-1.2 + 3.4 / 5.6 * 7.8"),
            -1.2 + 3.4 / 5.6 * 7.8
        );

        Check(
            "Test of Precedences of Operators 14",
            exevalator.Eval("1.2 + 3.4 / -5.6 * 7.8"),
            1.2 + 3.4 / 5.6 * -7.8
        );

        Check(
            "Test of Precedences of Operators 15",
            exevalator.Eval("1.2 + 3.4 / 5.6 * -7.8"),
            1.2 + 3.4 / 5.6 * -7.8
        );
    }


    // Tests parentheses.
    private static void TestParentheses() {
        Exevalator exevalator = new Exevalator();

        Check(
            "Test of Parentheses 1",
            exevalator.Eval("(1.2 + 3.4)"),
            (1.2 + 3.4)
        );

        Check(
            "Test of Parentheses 2",
            exevalator.Eval("(1.2 + 3.4) + 5.6"),
            (1.2 + 3.4) + 5.6
        );

        Check(
            "Test of Parentheses 3",
            exevalator.Eval("1.2 + (3.4 + 5.6)"),
            1.2 + (3.4 + 5.6)
        );

        Check(
            "Test of Parentheses 4",
            exevalator.Eval("1.2 + -(3.4 + 5.6)"),
            1.2 + -(3.4 + 5.6)
        );

        Check(
            "Test of Parentheses 5",
            exevalator.Eval("1.2 + -(-3.4 + 5.6)"),
            1.2 + -(-3.4 + 5.6)
        );

        Check(
            "Test of Parentheses 4",
            exevalator.Eval("(1.2 * 3.4) + 5.6"),
            (1.2 * 3.4) + 5.6
        );

        Check(
            "Test of Parentheses 5",
            exevalator.Eval("(1.2 + 3.4) * 5.6"),
            (1.2 + 3.4) * 5.6
        );

        Check(
            "Test of Parentheses 6",
            exevalator.Eval("1.2 + (3.4 * 5.6)"),
            1.2 + (3.4 * 5.6)
        );

        Check(
            "Test of Parentheses 7",
            exevalator.Eval("1.2 + (3.4 * 5.6) + 7.8"),
            1.2 + (3.4 * 5.6) + 7.8
        );

        Check(
            "Test of Parentheses 8",
            exevalator.Eval("1.2 * (3.4 + 5.6) / 7.8"),
            1.2 * (3.4 + 5.6) / 7.8
        );

        Check(
            "Test of Parentheses 9",
            exevalator.Eval("(1.2 + 3.4 - 5.6) * ((7.8 + 9.0) / 10.1)"),
            (1.2 + 3.4 - 5.6) * ((7.8 + 9.0) / 10.1)
        );

        Check(
            "Test of Parentheses 10",
            exevalator.Eval("(-(1.2 + 3.4 - 5.6) * ((7.8 + 9.0) / 10.1) / 11.2 + 12.3 * ((13.4 + -(15.6 - 17.8)) * 18.9)) + 19.0 * 20.1"),
            (-(1.2 + 3.4 - 5.6) * ((7.8 + 9.0) / 10.1) / 11.2 + 12.3 * ((13.4 + -(15.6 - 17.8)) * 18.9)) + 19.0 * 20.1
        );
    }


    // Tests of complicated cases.
    private static void TestComplicatedCases() {
        Exevalator exevalator = new Exevalator();

        Check(
            "Test of Complicated Case 1: The Expression Containing Many Parentheses and Many Literals having Exponent Parts",
            exevalator.Eval("(-(1.2E1 + 3.4E-2 - 5.6E2) * ((7.8E0 + 9.0) / 10.1E-3) / 11.2 + 12.3E-1 * ((13.4 + -(15.6E-12 - 17.8E-10)) * 18.9E-5)) + 19.0E-2 * 20.1E0"),
            (-(1.2E1 + 3.4E-2 - 5.6E2) * ((7.8E0 + 9.0) / 10.1E-3) / 11.2 + 12.3E-1 * ((13.4 + -(15.6E-12 - 17.8E-10)) * 18.9E-5)) + 19.0E-2 * 20.1E0
        );
    }


    // Tests syntax checks by the interpreter for expressions.
    private static void TestSyntaxChecksOfCorresponencesOfParentheses() {
        Exevalator exevalator = new Exevalator();

        Check(
            "Test of Detection of Mismatching of Open/Closed Parentheses 1",
            exevalator.Eval("(1 + 2)"),
            (1 + 2)
        );

        try {
            Check(
                "Test of Detection of Mismatching of Open/Closed Parentheses 2",
                exevalator.Eval("((1 + 2)"),
                (1 + 2)
            );
            throw new ExevalatorTestException("Expected exception has not been thrown");
        } catch (ExevalatorException) {
            // Expected to thrown
            Console.WriteLine("Test of Detection of Mismatching of Open/Closed Parentheses 2: OK.");
        }

        try {
            Check(
                "Test of Detection of Mismatching of Open/Closed Parentheses 3",
                exevalator.Eval("(1 + 2))"),
                (1 + 2)
            );
            throw new ExevalatorTestException("Expected exception has not been thrown");
        } catch (ExevalatorException) {
            // Expected to thrown
            Console.WriteLine("Test of Detection of Mismatching of Open/Closed Parentheses 3: OK.");
        }

        Check(
            "Test of Detection of Mismatching of Open/Closed Parentheses 4",
            exevalator.Eval("(1 + 2) + (3 + 4)"),
            (1 + 2) + (3 + 4)
        );

        try {
            Check(
                "Test of Detection of Mismatching of Open/Closed Parentheses 5",
                exevalator.Eval("1 + 2) + (3 + 4"),
                (1 + 2) + (3 + 4)
            );
            throw new ExevalatorTestException("Expected exception has not been thrown");
        } catch (ExevalatorException) {
            // Expected to thrown
            Console.WriteLine("Test of Detection of Mismatching of Open/Closed Parentheses 5: OK.");
        }

        Check(
            "Test of Detection of Mismatching of Open/Closed Parentheses 6",
            exevalator.Eval("1 + ((2 + (3 + 4) + 5) + 6)"),
            1 + ((2 + (3 + 4) + 5) + 6)
        );

        try {
            Check(
                "Test of Detection of Mismatching of Open/Closed Parentheses 7",
                exevalator.Eval("1 + ((2 + (3 + 4) + 5) + 6"),
                1 + ((2 + (3 + 4) + 5) + 6)
            );
            throw new ExevalatorTestException("Expected exception has not been thrown");
        } catch (ExevalatorException) {
            // Expected to thrown
            Console.WriteLine("Test of Detection of Mismatching of Open/Closed Parentheses 7: OK.");
        }

        try {
            Check(
                "Test of Detection of Mismatching of Open/Closed Parentheses 8",
                exevalator.Eval("1 + (2 + (3 + 4) + 5) + 6)"),
                1 + ((2 + (3 + 4) + 5) + 6)
            );
            throw new ExevalatorTestException("Expected exception has not been thrown");
        } catch (ExevalatorException) {
            // Expected to thrown
            Console.WriteLine("Test of Detection of Mismatching of Open/Closed Parentheses 8: OK.");
        }

        try {
            Check(
                "Test of Detection of Empty Parentheses 1",
                exevalator.Eval("()"),
                Double.NaN
            );
            throw new ExevalatorTestException("Expected exception has not been thrown");
        } catch (ExevalatorException) {
            // Expected to thrown
            Console.WriteLine("Test of Detection of Empty Parentheses 1: OK.");
        }

        try {
            Check(
                "Test of Detection of Empty Parentheses 2",
                exevalator.Eval("1 + ()"),
                Double.NaN
            );
            throw new ExevalatorTestException("Expected exception has not been thrown");
        } catch (ExevalatorException) {
            // Expected to thrown
            Console.WriteLine("Test of Detection of Empty Parentheses 2: OK.");
        }

        try {
            Check(
                "Test of Detection of Empty Parentheses 3",
                exevalator.Eval("() + 1"),
                Double.NaN
            );
            throw new ExevalatorTestException("Expected exception has not been thrown");
        } catch (ExevalatorException) {
            // Expected to thrown
            Console.WriteLine("Test of Detection of Empty Parentheses 3: OK.");
        }
    }


    // Tests syntax checks by the interpreter for locations of operators and leaf elements (literals and identifiers).
    private static void TestSyntaxChecksOfLocationsOfOperatorsAndLeafs() {
        Exevalator exevalator = new Exevalator();

        Check(
            "Test of Detection of Left Operand of Unary-Prefix Operator 1",
            exevalator.Eval("1 + -123"),
            1 + -123
        );

        try {
            Check(
                "Test of Detection of Left Operand of Unary-Prefix Operator 2",
                exevalator.Eval("1 + -"),
                1 + -123
            );
            throw new ExevalatorTestException("Expected exception has not been thrown");
        } catch (ExevalatorException) {
            // Expected to thrown
            Console.WriteLine("Test of Detection of Left Operand of Unary-Prefix Operator 2: OK.");
        }

        try {
            Check(
                "Test of Detection of Left Operand of Unary-Prefix Operator 3",
                exevalator.Eval("(1 + -)"),
                1 + -123
            );
            throw new ExevalatorTestException("Expected exception has not been thrown");
        } catch (ExevalatorException) {
            // Expected to thrown
            Console.WriteLine("Test of Detection of Left Operand of Unary-Prefix Operator 3: OK.");
        }

        Check(
            "Test of Detection of Left Operand of Binary Operator 1",
            exevalator.Eval("123 + 456"),
            123 + 456
        );

        try {
            Check(
                "Test of Detection of Left Operand of Binary Operator 2",
                exevalator.Eval("123 *"),
                123 * 456
            );
            throw new ExevalatorTestException("Expected exception has not been thrown");
        } catch (ExevalatorException) {
            // Expected to thrown
            Console.WriteLine("Test of Detection of Left Operand of Binary Operator 2: OK.");
        }

        try {
            Check(
                "Test of Detection of Left Operand of Binary Operator 3",
                exevalator.Eval("* 456"),
                123 * 456
            );
            throw new ExevalatorTestException("Expected exception has not been thrown");
        } catch (ExevalatorException) {
            // Expected to thrown
            Console.WriteLine("Test of Detection of Left Operand of Binary Operator 3: OK.");
        }

        try {
            Check(
                "Test of Detection of Left Operand of Binary Operator 4",
                exevalator.Eval("123 + ( * 456)"),
                123 * 456
            );
            throw new ExevalatorTestException("Expected exception has not been thrown");
        } catch (ExevalatorException) {
            // Expected to thrown
            Console.WriteLine("Test of Detection of Left Operand of Binary Operator 4: OK.");
        }

        try {
            Check(
                "Test of Detection of Left Operand of Binary Operator 5",
                exevalator.Eval("(123 *) + 456"),
                123 * 456
            );
            throw new ExevalatorTestException("Expected exception has not been thrown");
        } catch (ExevalatorException) {
            // Expected to thrown
            Console.WriteLine("Test of Detection of Left Operand of Binary Operator 5: OK.");
        }

        try {
            Check(
                "Test of Detection of Lacking Operator",
                exevalator.Eval("123 456"),
                123 * 456
            );
            throw new ExevalatorTestException("Expected exception has not been thrown");
        } catch (ExevalatorException) {
            // Expected to thrown
            Console.WriteLine("Test of Detection of Lacking Operator: OK.");
        }

    }


    // Checks the evaluated (computed) value of the testing expression by the Exevalator.
    // evaluatedValue: The evaluated (computed) value of the testing expression by Exevalator
    // correctValue: The correct value of the testing expression
    // testName: The name of the testing
    private static void Check(String testName, double evaluatedValue, double correctValue) {
        if (Math.Abs(evaluatedValue - correctValue) < AllowableError) {
            Console.WriteLine(testName + ": OK.");
            return;
        }
        throw new ExevalatorTestException(
            "\"" + testName + "\" has failed. " +
            "evaluatedValue=" + evaluatedValue + ", " +
            "correctValue=" + correctValue + "."
        );
    }


    // The Exception class thrown when any test has failed.
    private class ExevalatorTestException : Exception
    {
        public ExevalatorTestException(string errorMessage) : base (errorMessage)
        {
        }
    }
}
