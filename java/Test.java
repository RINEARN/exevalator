/**
 * The class for testing Exevalator.
 */
public class Test {

    /** The minimum error between two double-type values to regard them almost equal. */
    private static final double ALLOWABLE_ERROR = 1.0E-12;


    public static void main(String[] args) {
        Test test = new Test();
        test.testNumberLiterals();
        test.testOperationsOfOperators();
        test.testPrecedencesOfOperators();
        test.testParentheses();
        test.testComplicatedCases();
        test.testSyntaxChecksOfCorresponencesOfParentheses();
        test.testSyntaxChecksOfLocationsOfOperatorsAndLeafs();
        test.testVariables();
        test.testFunctions();
        test.testEmptyExpressions();
        test.testReeval();
        test.testTokenization();

        System.out.println("All tests have completed successfully.");
    }


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

        check(
            "Test of Precedences of Operators 15",
            exevalator.eval("1.2*--3.4"),
            1.2*(-(-3.4))
        );
    }


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

        check(
            "Test of Parenthesis 11",
            exevalator.eval("1.2 + 3.4 + (5.6)"),
            1.2 + 3.4 + (5.6)
        );
    }


    private void testComplicatedCases() {
        Exevalator exevalator = new Exevalator();

        check(
            "Test of Complicated Case 1: The Expression Containing Many Parentheses and Many Literals having Exponent Parts",
            exevalator.eval("(-(1.2E1 + 3.4E-2 - 5.6E2) * ((7.8E0 + 9.0) / 10.1E-3) / 11.2 + 12.3E-1 * ((13.4 + -(15.6E-12 - 17.8E-10)) * 18.9E-5)) + 19.0E-2 * 20.1E0"),
            (-(1.2E1 + 3.4E-2 - 5.6E2) * ((7.8E0 + 9.0) / 10.1E-3) / 11.2 + 12.3E-1 * ((13.4 + -(15.6E-12 - 17.8E-10)) * 18.9E-5)) + 19.0E-2 * 20.1E0
        );
    }


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


    private void testVariables() {
        Exevalator exevalator = new Exevalator();

        try {
            exevalator.eval("x");
            throw new ExevalatorTestException("Expected exception has not been thrown");
        } catch (Exevalator.Exception ee) {
            // Expected to be thrown
            System.out.println("Test of Variables 1: OK.");
        }

        int xAddress = exevalator.declareVariable("x");

        check(
            "Test of Variables 2",
            exevalator.eval("x"),
            0.0
        );

        exevalator.writeVariable("x", 1.25);

        check(
            "Test of Variables 3",
            exevalator.eval("x"),
            1.25
        );

        exevalator.writeVariableAt(xAddress, 2.5);

        check(
            "Test of Variables 4",
            exevalator.eval("x"),
            2.5
        );

        try {
            exevalator.writeVariableAt(100, 5.0);
            throw new ExevalatorTestException("Expected exception has not been thrown");
        } catch (Exevalator.Exception ee) {
            // Expected to be thrown
            System.out.println("Test of Variables 5: OK.");
        }

        try {
            exevalator.eval("y");
            throw new ExevalatorTestException("Expected exception has not been thrown");
        } catch (Exevalator.Exception ee) {
            // Expected to be thrown
            System.out.println("Test of Variables 6: OK.");
        }

        int yAddress = exevalator.declareVariable("y");

        check(
            "Test of Variables 7",
            exevalator.eval("y"),
            0.0
        );

        exevalator.writeVariable("y", 0.25);

        check(
            "Test of Variables 8",
            exevalator.eval("y"),
            0.25
        );

        exevalator.writeVariableAt(yAddress, 0.5);

        check(
            "Test of Variables 9",
            exevalator.eval("y"),
            0.5
        );

        check(
            "Test of Variables 10",
            exevalator.eval("x + y"),
            2.5 + 0.5
        );

        // Variables having names containing numbers
        exevalator.declareVariable("x2");
        exevalator.declareVariable("y2");
        exevalator.writeVariable("x2", 22.5);
        exevalator.writeVariable("y2", 32.5);
        check(
            "Test of Variables 11",
            exevalator.eval("x + y + 2 + x2 + 2 * y2"),
            2.5 + 0.5 + 2.0 + 22.5 + 2.0 * 32.5
        );
    }


    class FunctionA implements Exevalator.FunctionInterface {
        @Override
        public double invoke(double[] args) {
            if (args.length != 0) {
                throw new Exevalator.Exception("Incorrect number of arguments");
            }
            return 1.25;
        }
    }

    class FunctionB implements Exevalator.FunctionInterface {
        @Override
        public double invoke(double[] args) {
            if (args.length != 1) {
                throw new Exevalator.Exception("Incorrect number of arguments");
            }
            return args[0];
        }
    }

    class FunctionC implements Exevalator.FunctionInterface {
        @Override
        public double invoke(double[] args) {
            if (args.length != 2) {
                throw new Exevalator.Exception("Incorrect number of arguments");
            }
            return args[0] + args[1];
        }
    }

    class FunctionD implements Exevalator.FunctionInterface {
        @Override
        public double invoke(double[] args) {
            if (args.length != 3) {
                throw new Exevalator.Exception("Incorrect number of arguments");
            }
            if (args[0] != 1.25) {
                throw new Exevalator.Exception("The value of args[0] is incorrect");
            }
            if (args[1] != 2.5) {
                throw new Exevalator.Exception("The value of args[1] is incorrect");
            }
            if (args[2] != 5.0) {
                throw new Exevalator.Exception("The value of args[2] is incorrect");
            }
            return 0.0;
        }
    }

    private void testFunctions() {
        Exevalator exevalator = new Exevalator();

        try {
            exevalator.eval("funA()");
            throw new ExevalatorTestException("Expected exception has not been thrown");
        } catch (Exevalator.Exception ee) {
            // Expected to be thrown
            System.out.println("Test of Functions 1: OK.");
        }

        exevalator.connectFunction("funA", new FunctionA());
        check(
            "Test of Functions 2",
            exevalator.eval("funA()"),
            1.25
        );

        try {
            exevalator.eval("funB(2.5)");
            throw new ExevalatorTestException("Expected exception has not been thrown");
        } catch (Exevalator.Exception ee) {
            // Expected to be thrown
            System.out.println("Test of Functions 3: OK.");
        }

        exevalator.connectFunction("funB", new FunctionB());
        check(
            "Test of Functions 4",
            exevalator.eval("funB(2.5)"),
            2.5
        );

        exevalator.connectFunction("funC", new FunctionC());
        check(
            "Test of Functions 5",
            exevalator.eval("funC(1.25, 2.5)"),
            1.25 + 2.5
        );

        check(
            "Test of Functions 6",
            exevalator.eval("funC(funA(), funB(2.5))"),
            1.25 + 2.5
        );

        check(
            "Test of Functions 7",
            exevalator.eval("funC(funC(funA(), funB(2.5)), funB(1.0))"),
            1.25 + 2.5 + 1.0
        );
        
        check(
            "Test of Function 8",
            exevalator.eval("funC(1.0, 3.5 * funB(2.5) / 2.0)"),
            1.0 + 3.5 * 2.5 / 2.0
        );
        
        check(
            "Test of Functions 9",
            exevalator.eval("funA() * funC(funC(funA(), 3.5 * funB(2.5) / 2.0), funB(1.0))"),
            1.25 * (1.25 + 3.5 * 2.5 / 2.0 + 1.0)
        );

        check(
            "Test of Functions 10",
            exevalator.eval("2 + 256 * funA() * funC(funC(funA(), 3.5 * funB(2.5) / 2.0), funB(1.0)) * 128"),
            2.0 + 256.0 * (1.25 * (1.25 + 3.5 * 2.5 / 2.0 + 1.0)) * 128.0
        );

        exevalator.connectFunction("funD", new FunctionD());
        check(
            "Test of Functions 11",
            exevalator.eval("funD(1.25, 2.5, 5.0)"),
            0.0
        );

        check(
            "Test of Functions 12",
            exevalator.eval("-funC(-1.25, -2.5)"),
            - (-1.25 + -2.5)
        );
    }

    private void testEmptyExpressions() {
        Exevalator exevalator = new Exevalator();

        try {
            exevalator.eval("");
            throw new ExevalatorTestException("Expected exception has not been thrown");
        } catch (Exevalator.Exception ee) {
            // Expected to be thrown
            System.out.println("Test of Empty Functions 1: OK.");
        }

        try {
            exevalator.eval(" ");
            throw new ExevalatorTestException("Expected exception has not been thrown");
        } catch (Exevalator.Exception ee) {
            // Expected to be thrown
            System.out.println("Test of Empty Functions 2: OK.");
        }

        try {
            exevalator.eval("  ");
            throw new ExevalatorTestException("Expected exception has not been thrown");
        } catch (Exevalator.Exception ee) {
            // Expected to be thrown
            System.out.println("Test of Empty Functions 3: OK.");
        }

        try {
            exevalator.eval("   ");
            throw new ExevalatorTestException("Expected exception has not been thrown");
        } catch (Exevalator.Exception ee) {
            // Expected to be thrown
            System.out.println("Test of Empty Functions 4: OK.");
        }
    }


    private void testReeval() {
        Exevalator exevalator = new Exevalator();

        check(
            "Test of reval() Method 1",
            exevalator.eval("1.2 + 3.4"),
            1.2 + 3.4
        );

        check(
            "Test of reval() Method 2",
            exevalator.reeval(),
            1.2 + 3.4
        );

        check(
            "Test of reval() Method 3",
            exevalator.reeval(),
            1.2 + 3.4
        );

        check(
            "Test of reval() Method 4",
            exevalator.eval("5.6 - 7.8"),
            5.6 - 7.8
        );

        check(
            "Test of reval() Method 5",
            exevalator.reeval(),
            5.6 - 7.8
        );

        check(
            "Test of reval() Method 6",
            exevalator.reeval(),
            5.6 - 7.8
        );

        check(
            "Test of reval() Method 7",
            exevalator.eval("(1.23 + 4.56) * 7.89"),
            (1.23 + 4.56) * 7.89
        );

        check(
            "Test of reval() Method 8",
            exevalator.reeval(),
            (1.23 + 4.56) * 7.89
        );

        check(
            "Test of reval() Method 9",
            exevalator.reeval(),
            (1.23 + 4.56) * 7.89
        );
    }


    private void testTokenization() {
        Exevalator exevalator = new Exevalator();

        check(
            "Test of Tokenization 1",
            exevalator.eval("1.2345678"),
            1.2345678
        );

        try {
            exevalator.eval("1.234\n5678");
            throw new ExevalatorTestException("Expected exception has not been thrown");
        } catch (Exevalator.Exception ee) {
            // Expected to be 
            System.out.println("Test of Tokenization 2: OK.");
        }

        try {
            exevalator.eval("1.234\r\n5678");
            throw new ExevalatorTestException("Expected exception has not been thrown");
        } catch (Exevalator.Exception ee) {
            // Expected to be thrown
            System.out.println("Test of Tokenization 3: OK.");
        }

        try {
            exevalator.eval("1.234\t5678");
            throw new ExevalatorTestException("Expected exception has not been thrown");
        } catch (Exevalator.Exception ee) {
            // Expected to be thrown
            System.out.println("Test of Tokenization 4: OK.");
        }

        try {
            exevalator.eval("1.234 5678");
            throw new ExevalatorTestException("Expected exception has not been thrown");
        } catch (Exevalator.Exception ee) {
            // Expected to be thrown
            System.out.println("Test of Tokenization 5: OK.");
        }

        check(
            "Test of Tokenization 6",
            exevalator.eval("1+2*3-4/5"),
            1.0 + 2.0 * 3.0 - 4.0 / 5.0
        );

        check(
            "Test of Tokenization 7",
            exevalator.eval("1+\n2*3\r\n-4/5"),
            1.0 + 2.0 * 3.0 - 4.0 / 5.0
        );

        check(
            "Test of Tokenization 8",
            exevalator.eval("((1+2)*3)-(4/5)"),
            ((1.0 + 2.0) * 3.0) - (4.0 / 5.0)
        );

        FunctionC funC = new FunctionC();
        exevalator.connectFunction("funC", funC);

        check(
            "Test of Tokenization 9",
            exevalator.eval("funC(1,2)"),
            1.0 + 2.0
        );

        check(
            "Test of Tokenization 10",
            exevalator.eval("funC(\n1,\r\n2\t)"),
            1.0 + 2.0
        );

        check(
            "Test of Tokenization 11",
            exevalator.eval("3*funC(1,2)/2"),
            3.0 * (1.0 + 2.0) / 2.0
        );

        check(
            "Test of Tokenization 12",
            exevalator.eval("3*(-funC(1,2)+2)"),
            3.0 * (-(1.0 + 2.0) + 2.0)
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
