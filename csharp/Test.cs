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
        TestVariables();
        TestFunctions();
        TestEmptyExpressions();
        TestReeval();
        TestTokenization();

        Console.WriteLine("All tests have completed successfully.");
    }


    // Tests number literals.
    private static void TestNumberLiterals()
    {
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
    private static void TestOperationsOfOperators()
    {
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
    private static void TestPrecedencesOfOperators()
    {
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
    private static void TestParentheses()
    {
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
    private static void TestComplicatedCases()
    {
        Exevalator exevalator = new Exevalator();

        Check(
            "Test of Complicated Case 1: The Expression Containing Many Parentheses and Many Literals having Exponent Parts",
            exevalator.Eval("(-(1.2E1 + 3.4E-2 - 5.6E2) * ((7.8E0 + 9.0) / 10.1E-3) / 11.2 + 12.3E-1 * ((13.4 + -(15.6E-12 - 17.8E-10)) * 18.9E-5)) + 19.0E-2 * 20.1E0"),
            (-(1.2E1 + 3.4E-2 - 5.6E2) * ((7.8E0 + 9.0) / 10.1E-3) / 11.2 + 12.3E-1 * ((13.4 + -(15.6E-12 - 17.8E-10)) * 18.9E-5)) + 19.0E-2 * 20.1E0
        );
    }


    // Tests syntax checks by the interpreter for expressions.
    private static void TestSyntaxChecksOfCorresponencesOfParentheses()
    {
        Exevalator exevalator = new Exevalator();

        Check(
            "Test of Detection of Mismatching of Open/Closed Parentheses 1",
            exevalator.Eval("(1 + 2)"),
            (1 + 2)
        );

        try
        {
            Check(
                "Test of Detection of Mismatching of Open/Closed Parentheses 2",
                exevalator.Eval("((1 + 2)"),
                (1 + 2)
            );
            throw new ExevalatorTestException("Expected exception has not been thrown");
        }
        catch (ExevalatorException)
        {
            // Expected to thrown
            Console.WriteLine("Test of Detection of Mismatching of Open/Closed Parentheses 2: OK.");
        }

        try
        {
            Check(
                "Test of Detection of Mismatching of Open/Closed Parentheses 3",
                exevalator.Eval("(1 + 2))"),
                (1 + 2)
            );
            throw new ExevalatorTestException("Expected exception has not been thrown");
        }
        catch (ExevalatorException)
        {
            // Expected to thrown
            Console.WriteLine("Test of Detection of Mismatching of Open/Closed Parentheses 3: OK.");
        }

        Check(
            "Test of Detection of Mismatching of Open/Closed Parentheses 4",
            exevalator.Eval("(1 + 2) + (3 + 4)"),
            (1 + 2) + (3 + 4)
        );

        try
        {
            Check(
                "Test of Detection of Mismatching of Open/Closed Parentheses 5",
                exevalator.Eval("1 + 2) + (3 + 4"),
                (1 + 2) + (3 + 4)
            );
            throw new ExevalatorTestException("Expected exception has not been thrown");
        }
        catch (ExevalatorException)
        {
            // Expected to thrown
            Console.WriteLine("Test of Detection of Mismatching of Open/Closed Parentheses 5: OK.");
        }

        Check(
            "Test of Detection of Mismatching of Open/Closed Parentheses 6",
            exevalator.Eval("1 + ((2 + (3 + 4) + 5) + 6)"),
            1 + ((2 + (3 + 4) + 5) + 6)
        );

        try
        {
            Check(
                "Test of Detection of Mismatching of Open/Closed Parentheses 7",
                exevalator.Eval("1 + ((2 + (3 + 4) + 5) + 6"),
                1 + ((2 + (3 + 4) + 5) + 6)
            );
            throw new ExevalatorTestException("Expected exception has not been thrown");
        }
        catch (ExevalatorException)
        {
            // Expected to thrown
            Console.WriteLine("Test of Detection of Mismatching of Open/Closed Parentheses 7: OK.");
        }

        try
        {
            Check(
                "Test of Detection of Mismatching of Open/Closed Parentheses 8",
                exevalator.Eval("1 + (2 + (3 + 4) + 5) + 6)"),
                1 + ((2 + (3 + 4) + 5) + 6)
            );
            throw new ExevalatorTestException("Expected exception has not been thrown");
        }
        catch (ExevalatorException)
        {
            // Expected to thrown
            Console.WriteLine("Test of Detection of Mismatching of Open/Closed Parentheses 8: OK.");
        }

        try
        {
            Check(
                "Test of Detection of Empty Parentheses 1",
                exevalator.Eval("()"),
                Double.NaN
            );
            throw new ExevalatorTestException("Expected exception has not been thrown");
        }
        catch (ExevalatorException)
        {
            // Expected to thrown
            Console.WriteLine("Test of Detection of Empty Parentheses 1: OK.");
        }

        try
        {
            Check(
                "Test of Detection of Empty Parentheses 2",
                exevalator.Eval("1 + ()"),
                Double.NaN
            );
            throw new ExevalatorTestException("Expected exception has not been thrown");
        }
        catch (ExevalatorException)
        {
            // Expected to thrown
            Console.WriteLine("Test of Detection of Empty Parentheses 2: OK.");
        }

        try
        {
            Check(
                "Test of Detection of Empty Parentheses 3",
                exevalator.Eval("() + 1"),
                Double.NaN
            );
            throw new ExevalatorTestException("Expected exception has not been thrown");
        }
        catch (ExevalatorException)
        {
            // Expected to thrown
            Console.WriteLine("Test of Detection of Empty Parentheses 3: OK.");
        }
    }


    // Tests syntax checks by the interpreter for locations of operators and leaf elements (literals and identifiers).
    private static void TestSyntaxChecksOfLocationsOfOperatorsAndLeafs()
    {
        Exevalator exevalator = new Exevalator();

        Check(
            "Test of Detection of Left Operand of Unary-Prefix Operator 1",
            exevalator.Eval("1 + -123"),
            1 + -123
        );

        try
        {
            Check(
                "Test of Detection of Left Operand of Unary-Prefix Operator 2",
                exevalator.Eval("1 + -"),
                1 + -123
            );
            throw new ExevalatorTestException("Expected exception has not been thrown");
        }
        catch (ExevalatorException)
        {
            // Expected to thrown
            Console.WriteLine("Test of Detection of Left Operand of Unary-Prefix Operator 2: OK.");
        }

        try
        {
            Check(
                "Test of Detection of Left Operand of Unary-Prefix Operator 3",
                exevalator.Eval("(1 + -)"),
                1 + -123
            );
            throw new ExevalatorTestException("Expected exception has not been thrown");
        }
        catch (ExevalatorException)
        {
            // Expected to thrown
            Console.WriteLine("Test of Detection of Left Operand of Unary-Prefix Operator 3: OK.");
        }

        Check(
            "Test of Detection of Left Operand of Binary Operator 1",
            exevalator.Eval("123 + 456"),
            123 + 456
        );

        try
        {
            Check(
                "Test of Detection of Left Operand of Binary Operator 2",
                exevalator.Eval("123 *"),
                123 * 456
            );
            throw new ExevalatorTestException("Expected exception has not been thrown");
        }
        catch (ExevalatorException)
        {
            // Expected to thrown
            Console.WriteLine("Test of Detection of Left Operand of Binary Operator 2: OK.");
        }

        try
        {
            Check(
                "Test of Detection of Left Operand of Binary Operator 3",
                exevalator.Eval("* 456"),
                123 * 456
            );
            throw new ExevalatorTestException("Expected exception has not been thrown");
        }
        catch (ExevalatorException)
        {
            // Expected to thrown
            Console.WriteLine("Test of Detection of Left Operand of Binary Operator 3: OK.");
        }

        try
        {
            Check(
                "Test of Detection of Left Operand of Binary Operator 4",
                exevalator.Eval("123 + ( * 456)"),
                123 * 456
            );
            throw new ExevalatorTestException("Expected exception has not been thrown");
        }
        catch (ExevalatorException)
        {
            // Expected to thrown
            Console.WriteLine("Test of Detection of Left Operand of Binary Operator 4: OK.");
        }

        try
        {
            Check(
                "Test of Detection of Left Operand of Binary Operator 5",
                exevalator.Eval("(123 *) + 456"),
                123 * 456
            );
            throw new ExevalatorTestException("Expected exception has not been thrown");
        }
        catch (ExevalatorException)
        {
            // Expected to thrown
            Console.WriteLine("Test of Detection of Left Operand of Binary Operator 5: OK.");
        }

        try
        {
            Check(
                "Test of Detection of Lacking Operator",
                exevalator.Eval("123 456"),
                123 * 456
            );
            throw new ExevalatorTestException("Expected exception has not been thrown");
        }
        catch (ExevalatorException)
        {
            // Expected to thrown
            Console.WriteLine("Test of Detection of Lacking Operator: OK.");
        }

    }


    private static void TestVariables()
    {
        Exevalator exevalator = new Exevalator();

        try
        {
            exevalator.Eval("x");
            throw new ExevalatorTestException("Expected exception has not been thrown");
        }
        catch (ExevalatorException)
        {
            // Expected to be thrown
            Console.WriteLine("Test of Variables 1: OK.");
        }

        int xAddress = exevalator.DeclareVariable("x");

        Check(
            "Test of Variables 2",
            exevalator.Eval("x"),
            0.0
        );

        exevalator.WriteVariable("x", 1.25);

        Check(
            "Test of Variables 3",
            exevalator.Eval("x"),
            1.25
        );

        exevalator.WriteVariableAt(xAddress, 2.5);

        Check(
            "Test of Variables 4",
            exevalator.Eval("x"),
            2.5
        );

        try
        {
            exevalator.WriteVariableAt(100, 5.0);
            throw new ExevalatorTestException("Expected exception has not been thrown");
        }
        catch (ExevalatorException)
        {
            // Expected to be thrown
            Console.WriteLine("Test of Variables 5: OK.");
        }

        try
        {
            exevalator.Eval("y");
            throw new ExevalatorTestException("Expected exception has not been thrown");
        }
        catch (ExevalatorException)
        {
            // Expected to be thrown
            Console.WriteLine("Test of Variables 6: OK.");
        }

        int yAddress = exevalator.DeclareVariable("y");

        Check(
            "Test of Variables 7",
            exevalator.Eval("y"),
            0.0
        );

        exevalator.WriteVariable("y", 0.25);

        Check(
            "Test of Variables 8",
            exevalator.Eval("y"),
            0.25
        );

        exevalator.WriteVariableAt(yAddress, 0.5);

        Check(
            "Test of Variables 9",
            exevalator.Eval("y"),
            0.5
        );

        Check(
            "Test of Variables 10",
            exevalator.Eval("x + y"),
            2.5 + 0.5
        );

        // Variables having names containing numbers
        exevalator.DeclareVariable("x2");
        exevalator.DeclareVariable("y2");
        exevalator.WriteVariable("x2", 22.5);
        exevalator.WriteVariable("y2", 32.5);
        Check(
            "Test of Variables 11",
            exevalator.Eval("x + y + 2 + x2 + 2 * y2"),
            2.5 + 0.5 + 2.0 + 22.5 + 2.0 * 32.5
        );
    }


    class FunctionA : IExevalatorFunction
    {
        public double Invoke(double[] args)
        {
            if (args.Length != 0)
            {
                throw new ExevalatorException("Incorrect number of arguments");
            }
            return 1.25;
        }
    }

    class FunctionB : IExevalatorFunction
    {
        public double Invoke(double[] args)
        {
            if (args.Length != 1)
            {
                throw new ExevalatorException("Incorrect number of arguments");
            }
            return args[0];
        }
    }

    class FunctionC : IExevalatorFunction
    {
        public double Invoke(double[] args)
        {
            if (args.Length != 2)
            {
                throw new ExevalatorException("Incorrect number of arguments");
            }
            return args[0] + args[1];
        }
    }

    class FunctionD : IExevalatorFunction
    {
        public double Invoke(double[] args)
        {
            if (args.Length != 3)
            {
                throw new ExevalatorException("Incorrect number of arguments");
            }
            if (args[0] != 1.25)
            {
                throw new ExevalatorException("The value of args[0] is incorrect");
            }
            if (args[1] != 2.5)
            {
                throw new ExevalatorException("The value of args[1] is incorrect");
            }
            if (args[2] != 5.0)
            {
                throw new ExevalatorException("The value of args[2] is incorrect");
            }
            return 0.0;
        }
    }

    private static void TestFunctions()
    {
        Exevalator exevalator = new Exevalator();

        try
        {
            exevalator.Eval("funA()");
            throw new ExevalatorTestException("Expected exception has not been thrown");
        }
        catch (ExevalatorException)
        {
            // Expected to be thrown
            Console.WriteLine("Test of Functions 1: OK.");
        }

        exevalator.ConnectFunction("funA", new FunctionA());
        Check(
            "Test of Functions 2",
            exevalator.Eval("funA()"),
            1.25
        );

        try
        {
            exevalator.Eval("funB(2.5)");
            throw new ExevalatorTestException("Expected exception has not been thrown");
        }
        catch (ExevalatorException)
        {
            // Expected to be thrown
            Console.WriteLine("Test of Functions 3: OK.");
        }

        exevalator.ConnectFunction("funB", new FunctionB());
        Check(
            "Test of Functions 4",
            exevalator.Eval("funB(2.5)"),
            2.5
        );

        exevalator.ConnectFunction("funC", new FunctionC());
        Check(
            "Test of Functions 5",
            exevalator.Eval("funC(1.25, 2.5)"),
            1.25 + 2.5
        );

        Check(
            "Test of Functions 6",
            exevalator.Eval("funC(funA(), funB(2.5))"),
            1.25 + 2.5
        );

        Check(
            "Test of Functions 7",
            exevalator.Eval("funC(funC(funA(), funB(2.5)), funB(1.0))"),
            1.25 + 2.5 + 1.0
        );

        Check(
            "Test of Function 8",
            exevalator.Eval("funC(1.0, 3.5 * funB(2.5) / 2.0)"),
            1.0 + 3.5 * 2.5 / 2.0
        );

        Check(
            "Test of Functions 9",
            exevalator.Eval("funA() * funC(funC(funA(), 3.5 * funB(2.5) / 2.0), funB(1.0))"),
            1.25 * (1.25 + 3.5 * 2.5 / 2.0 + 1.0)
        );

        Check(
            "Test of Functions 10",
            exevalator.Eval("2 + 256 * funA() * funC(funC(funA(), 3.5 * funB(2.5) / 2.0), funB(1.0)) * 128"),
            2.0 + 256.0 * (1.25 * (1.25 + 3.5 * 2.5 / 2.0 + 1.0)) * 128.0
        );

        exevalator.ConnectFunction("funD", new FunctionD());
        Check(
            "Test of Functions 11",
            exevalator.Eval("funD(1.25, 2.5, 5.0)"),
            0.0
        );

        Check(
            "Test of Functions 12",
            exevalator.Eval("-funC(-1.25, -2.5)"),
            - (-1.25 + -2.5)
        );
    }

    private static void TestEmptyExpressions()
    {
        Exevalator exevalator = new Exevalator();

        try
        {
            exevalator.Eval("");
            throw new ExevalatorTestException("Expected exception has not been thrown");
        }
        catch (ExevalatorException)
        {
            // Expected to be thrown
            Console.WriteLine("Test of Empty Expressions 1: OK.");
        }

        try
        {
            exevalator.Eval(" ");
            throw new ExevalatorTestException("Expected exception has not been thrown");
        }
        catch (ExevalatorException)
        {
            // Expected to be thrown
            Console.WriteLine("Test of Empty Expressions 2: OK.");
        }

        try
        {
            exevalator.Eval("  ");
            throw new ExevalatorTestException("Expected exception has not been thrown");
        }
        catch (ExevalatorException)
        {
            // Expected to be thrown
            Console.WriteLine("Test of Empty Expressions 3: OK.");
        }

        try
        {
            exevalator.Eval("   ");
            throw new ExevalatorTestException("Expected exception has not been thrown");
        }
        catch (ExevalatorException)
        {
            // Expected to be thrown
            Console.WriteLine("Test of Empty Expressions 4: OK.");
        }
    }


    private static void TestReeval()
    {
        Exevalator exevalator = new Exevalator();

        Check(
            "Test of reval() Method 1",
            exevalator.Eval("1.2 + 3.4"),
            1.2 + 3.4
        );

        Check(
            "Test of reval() Method 2",
            exevalator.Reeval(),
            1.2 + 3.4
        );

        Check(
            "Test of reval() Method 3",
            exevalator.Reeval(),
            1.2 + 3.4
        );

        Check(
            "Test of reval() Method 4",
            exevalator.Eval("5.6 - 7.8"),
            5.6 - 7.8
        );

        Check(
            "Test of reval() Method 5",
            exevalator.Reeval(),
            5.6 - 7.8
        );

        Check(
            "Test of reval() Method 6",
            exevalator.Reeval(),
            5.6 - 7.8
        );

        Check(
            "Test of reval() Method 7",
            exevalator.Eval("(1.23 + 4.56) * 7.89"),
            (1.23 + 4.56) * 7.89
        );

        Check(
            "Test of reval() Method 8",
            exevalator.Reeval(),
            (1.23 + 4.56) * 7.89
        );

        Check(
            "Test of reval() Method 9",
            exevalator.Reeval(),
            (1.23 + 4.56) * 7.89
        );
    }


    private static void TestTokenization() {
        Exevalator exevalator = new Exevalator();

        Check(
            "Test of Tokenization 1",
            exevalator.Eval("1.2345678"),
            1.2345678
        );

        try
        {
            exevalator.Eval("1.234\n5678");
            throw new ExevalatorTestException("Expected exception has not been thrown");
        }
        catch (ExevalatorException)
        {
            // Expected to be 
            Console.WriteLine("Test of Tokenization 2: OK.");
        }

        try
        {
            exevalator.Eval("1.234\r\n5678");
            throw new ExevalatorTestException("Expected exception has not been thrown");
        }
        catch (ExevalatorException)
        {
            // Expected to be thrown
            Console.WriteLine("Test of Tokenization 3: OK.");
        }

        try
        {
            exevalator.Eval("1.234\t5678");
            throw new ExevalatorTestException("Expected exception has not been thrown");
        }
        catch (ExevalatorException)
        {
            // Expected to be thrown
            Console.WriteLine("Test of Tokenization 4: OK.");
        }

        try
        {
            exevalator.Eval("1.234 5678");
            throw new ExevalatorTestException("Expected exception has not been thrown");
        }
        catch (ExevalatorException)
        {
            // Expected to be thrown
            Console.WriteLine("Test of Tokenization 5: OK.");
        }

        Check(
            "Test of Tokenization 6",
            exevalator.Eval("1+2*3-4/5"),
            1.0 + 2.0 * 3.0 - 4.0 / 5.0
        );

        Check(
            "Test of Tokenization 7",
            exevalator.Eval("1+\n2*3\r\n-4/5"),
            1.0 + 2.0 * 3.0 - 4.0 / 5.0
        );

        Check(
            "Test of Tokenization 8",
            exevalator.Eval("((1+2)*3)-(4/5)"),
            ((1.0 + 2.0) * 3.0) - (4.0 / 5.0)
        );

        FunctionC funC = new FunctionC();
        exevalator.ConnectFunction("funC", funC);

        Check(
            "Test of Tokenization 9",
            exevalator.Eval("funC(1,2)"),
            1.0 + 2.0
        );

        Check(
            "Test of Tokenization 10",
            exevalator.Eval("funC(\n1,\r\n2\t)"),
            1.0 + 2.0
        );

        Check(
            "Test of Tokenization 11",
            exevalator.Eval("3*funC(1,2)/2"),
            3.0 * (1.0 + 2.0) / 2.0
        );

        Check(
            "Test of Tokenization 12",
            exevalator.Eval("3*(-funC(1,2)+2)"),
            3.0 * (-(1.0 + 2.0) + 2.0)
        );
    }


    // Checks the evaluated (computed) value of the testing expression by the Exevalator.
    // evaluatedValue: The evaluated (computed) value of the testing expression by Exevalator
    // correctValue: The correct value of the testing expression
    // testName: The name of the testing
    private static void Check(String testName, double evaluatedValue, double correctValue)
    {
        if (Math.Abs(evaluatedValue - correctValue) < AllowableError)
        {
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
        public ExevalatorTestException(string errorMessage) : base(errorMessage)
        {
        }
    }
}
