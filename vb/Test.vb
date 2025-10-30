Imports System
Imports Rinearn.ExevalatorVB

' IMPORTANT NOTE:
' In a VB.NET project, by default,
' the project name is implicitly added to the top hierarchy of the project's namespaces.
' If you execute this code in a project named "YourProject", you must modify the above Imports statement as follows:
'
'    Imports YourProject.Rinearn.ExevalatorVB

Module Test

    ' The minimum error between two double-type values to regard them almost equal.
    Private Const AllowableError As Double = 1.0E-12

    Sub Main()
        TestNumberLiterals()
        TestOperationsOfOperators()
        TestPrecedencesOfOperators()
        TestParentheses()
        TestComplicatedCases()
        TestSyntaxChecksOfCorresponencesOfParentheses()
        TestSyntaxChecksOfLocationsOfOperatorsAndLeafs()
        TestVariables()
        TestFunctions()
        TestEmptyExpressions()
        TestReeval()
        TestTokenization()

        Console.WriteLine("All tests have completed successfully.")
    End Sub


    ' Tests number literals.
    Private Sub TestNumberLiterals()
        Dim exevalator As Exevalator = New Exevalator()

        Check( _
            "Test of a Simple Number Literal 1", _
            exevalator.Eval("1"), _
            1.0 _
        )

        Check( _
            "Test of a Simple Number Literal 2", _
            exevalator.Eval("2"), _
            2.0 _
        )

        Check( _
            "Test of a Simple Number Literal 3", _
            exevalator.Eval("1.2"), _
            1.2 _
        )

        Check( _
            "Test of a Number Literal with a Exponent Part 1", _
            exevalator.Eval("1.2E3"), _
            1.2E3 _
        )

        Check( _
            "Test of a Number Literal with a Exponent Part 2", _
            exevalator.Eval("1.2E+3"), _
            1.2E3 _
        )

        Check( _
            "Test of a Number Literal with a Exponent Part 3", _
            exevalator.Eval("1.2E-3"), _
            1.2E-3 _
        )

        Check( _
            "Test of a Number Literal with a Exponent Part 4", _
            exevalator.Eval("123.4567E12"), _
            123.4567E12 _
        )

        Check( _
            "Test of a Number Literal with a Exponent Part 5", _
            exevalator.Eval("123.4567E+12"), _
            123.4567E+12 _
        )

        Check( _
            "Test of a Number Literal with a Exponent Part 6", _
            exevalator.Eval("123.4567E-12"), _
            123.4567E-12 _
        )
    End Sub


    ' Tests each kind of operators.
    Private Sub TestOperationsOfOperators()
        Dim exevalator As Exevalator = New Exevalator()

        Check( _
            "Test of Addition Operator", _
            exevalator.Eval("1.2 + 3.4"), _
            1.2 + 3.4 _
        )

        Check( _
            "Test of Subtraction Operator", _
            exevalator.Eval("1.2 - 3.4"), _
            1.2 - 3.4 _
        )

        Check( _
            "Test of Multiplication Operator", _
            exevalator.Eval("1.2 * 3.4"), _
            1.2 * 3.4 _
        )

        Check( _
            "Test of Division Operator", _
            exevalator.Eval("1.2 / 3.4"), _
            1.2 / 3.4 _
        )

        Check( _
            "Test of Unary Minus Operator", _
            exevalator.Eval("-1.2"), _
            -1.2 _
        )
    End Sub


    ' Tests precedences of operators.
    Private Sub TestPrecedencesOfOperators()
        Dim exevalator As Exevalator = New Exevalator()

        Check( _
            "Test of Precedences of Operators 1", _
            exevalator.Eval("1.2 + 3.4 + 5.6 + 7.8"), _
            1.2 + 3.4 + 5.6 + 7.8 _
        )

        Check( _
            "Test of Precedences of Operators 2", _
            exevalator.Eval("1.2 + 3.4 - 5.6 + 7.8"), _
            1.2 + 3.4 - 5.6 + 7.8 _
        )

        Check( _
            "Test of Precedences of Operators 3", _
            exevalator.Eval("1.2 + 3.4 * 5.6 + 7.8"), _
            1.2 + 3.4 * 5.6 + 7.8 _
        )

        Check( _
            "Test of Precedences of Operators 4", _
            exevalator.Eval("1.2 + 3.4 / 5.6 + 7.8"), _
            1.2 + 3.4 / 5.6 + 7.8 _
        )

        Check( _
            "Test of Precedences of Operators 5", _
            exevalator.Eval("1.2 * 3.4 + 5.6 + 7.8"), _
            1.2 * 3.4 + 5.6 + 7.8 _
        )

        Check( _
            "Test of Precedences of Operators 6", _
            exevalator.Eval("1.2 * 3.4 - 5.6 + 7.8"), _
            1.2 * 3.4 - 5.6 + 7.8 _
        )

        Check( _
            "Test of Precedences of Operators 7", _
             exevalator.Eval("1.2 * 3.4 * 5.6 + 7.8"), _
            1.2 * 3.4 * 5.6 + 7.8 _
        )

        Check( _
            "Test of Precedences of Operators 8", _
            exevalator.Eval("1.2 * 3.4 / 5.6 + 7.8"), _
            1.2 * 3.4 / 5.6 + 7.8 _
        )

        Check( _
            "Test of Precedences of Operators 9", _
            exevalator.Eval("1.2 + 3.4 + 5.6 * 7.8"), _
            1.2 + 3.4 + 5.6 * 7.8 _
        )

        Check( _
            "Test of Precedences of Operators 10", _
            exevalator.Eval("1.2 + 3.4 - 5.6 * 7.8"), _
            1.2 + 3.4 - 5.6 * 7.8 _
        )

        Check( _
            "Test of Precedences of Operators 11", _
            exevalator.Eval("1.2 + 3.4 * 5.6 * 7.8"), _
            1.2 + 3.4 * 5.6 * 7.8 _
        )

        Check( _
            "Test of Precedences of Operators 12", _
            exevalator.Eval("1.2 + 3.4 / 5.6 * 7.8"),
            1.2 + 3.4 / 5.6 * 7.8
        )

        Check( _
            "Test of Precedences of Operators 13", _
            exevalator.Eval("-1.2 + 3.4 / 5.6 * 7.8"),
            -1.2 + 3.4 / 5.6 * 7.8
        )

        Check( _
            "Test of Precedences of Operators 14", _
            exevalator.Eval("1.2 + 3.4 / -5.6 * 7.8"), _
            1.2 + 3.4 / 5.6 * -7.8 _
        )

        Check( _
            "Test of Precedences of Operators 15", _
            exevalator.Eval("1.2 + 3.4 / 5.6 * -7.8"), _
            1.2 + 3.4 / 5.6 * -7.8 _
        )

        Check( _
            "Test of Precedences of Operators 16", _
            exevalator.Eval("1.2*--3.4"), _
            1.2*(-(-3.4)) _
        )
    End Sub


    ' Tests parentheses.
    Private Sub TestParentheses()
        Dim exevalator As Exevalator = New Exevalator()

        Check( _
            "Test of Parentheses 1", _
            exevalator.Eval("(1.2 + 3.4)"), _
            (1.2 + 3.4) _
        )

        Check( _
            "Test of Parentheses 2", _
            exevalator.Eval("(1.2 + 3.4) + 5.6"), _
            (1.2 + 3.4) + 5.6 _
        )

        Check( _
            "Test of Parentheses 3", _
            exevalator.Eval("1.2 + (3.4 + 5.6)"), _
            1.2 + (3.4 + 5.6) _
        )

        Check( _
            "Test of Parentheses 4", _
            exevalator.Eval("1.2 + -(3.4 + 5.6)"), _
            1.2 + -(3.4 + 5.6) _
        )

        Check( _
            "Test of Parentheses 5", _
            exevalator.Eval("1.2 + -(-3.4 + 5.6)"), _
            1.2 + -(-3.4 + 5.6) _
        )

        Check( _
            "Test of Parentheses 4", _
            exevalator.Eval("(1.2 * 3.4) + 5.6"), _
            (1.2 * 3.4) + 5.6 _
        )

        Check( _
            "Test of Parentheses 5", _
            exevalator.Eval("(1.2 + 3.4) * 5.6"), _
            (1.2 + 3.4) * 5.6 _
        )

        Check( _
            "Test of Parentheses 6", _
            exevalator.Eval("1.2 + (3.4 * 5.6)"), _
            1.2 + (3.4 * 5.6) _
        )

        Check( _
            "Test of Parentheses 7", _
            exevalator.Eval("1.2 + (3.4 * 5.6) + 7.8"), _
            1.2 + (3.4 * 5.6) + 7.8 _
        )

        Check( _
            "Test of Parentheses 8", _
            exevalator.Eval("1.2 * (3.4 + 5.6) / 7.8"), _
            1.2 * (3.4 + 5.6) / 7.8 _
        )

        Check( _
            "Test of Parentheses 9", _
            exevalator.Eval("(1.2 + 3.4 - 5.6) * ((7.8 + 9.0) / 10.1)"), _
            (1.2 + 3.4 - 5.6) * ((7.8 + 9.0) / 10.1) _
        )

        Check( _
            "Test of Parentheses 10", _
            exevalator.Eval("(-(1.2 + 3.4 - 5.6) * ((7.8 + 9.0) / 10.1) / 11.2 + 12.3 * ((13.4 + -(15.6 - 17.8)) * 18.9)) + 19.0 * 20.1"), _
            (-(1.2 + 3.4 - 5.6) * ((7.8 + 9.0) / 10.1) / 11.2 + 12.3 * ((13.4 + -(15.6 - 17.8)) * 18.9)) + 19.0 * 20.1 _
        )

        Check( _
            "Test of Parentheses 11", _
            exevalator.Eval("1.2 + 3.4 + (5.6)"), _
            1.2 + 3.4 + (5.6) _
        )
    End Sub


    ' Tests of complicated cases.
    Private Sub TestComplicatedCases()
        Dim exevalator As Exevalator = New Exevalator()

        Check( _
            "Test of Complicated Case 1: The Expression Containing Many Parentheses and Many Literals having Exponent Parts", _
            exevalator.Eval("(-(1.2E1 + 3.4E-2 - 5.6E2) * ((7.8E0 + 9.0) / 10.1E-3) / 11.2 + 12.3E-1 * ((13.4 + -(15.6E-12 - 17.8E-10)) * 18.9E-5)) + 19.0E-2 * 20.1E0"), _
            (-(1.2E1 + 3.4E-2 - 5.6E2) * ((7.8E0 + 9.0) / 10.1E-3) / 11.2 + 12.3E-1 * ((13.4 + -(15.6E-12 - 17.8E-10)) * 18.9E-5)) + 19.0E-2 * 20.1E0 _
        )
    End Sub


    ' Tests syntax checks by the interpreter for expressions.
    Private Sub TestSyntaxChecksOfCorresponencesOfParentheses()
        Dim exevalator As Exevalator = New Exevalator()

        Check( _
            "Test of Detection of Mismatching of Open/Closed Parentheses 1", _
            exevalator.Eval("(1 + 2)"), _
            (1 + 2) _
        )

        Try
            Check( _
                "Test of Detection of Mismatching of Open/Closed Parentheses 2", _
                exevalator.Eval("((1 + 2)"), _
                (1 + 2) _
            )
            Throw New ExevalatorTestException("Expected exception has not been Thrown")
        Catch ee As ExevalatorException
            ' Expected to Thrown
            Console.WriteLine("Test of Detection of Mismatching of Open/Closed Parentheses 2: OK.")
        End Try

        Try
            Check( _
                "Test of Detection of Mismatching of Open/Closed Parentheses 3", _
                exevalator.Eval("(1 + 2))"), _
                (1 + 2) _
            )
            Throw New ExevalatorTestException("Expected exception has not been Thrown")
        Catch ee As ExevalatorException
            ' Expected to Thrown
            Console.WriteLine("Test of Detection of Mismatching of Open/Closed Parentheses 3: OK.")
        End Try

        Check( _
            "Test of Detection of Mismatching of Open/Closed Parentheses 4", _
            exevalator.Eval("(1 + 2) + (3 + 4)"), _
            (1 + 2) + (3 + 4) _
        )

        Try
            Check( _
                "Test of Detection of Mismatching of Open/Closed Parentheses 5", _
                exevalator.Eval("1 + 2) + (3 + 4"), _
                (1 + 2) + (3 + 4) _
            )
            Throw New ExevalatorTestException("Expected exception has not been Thrown")
        Catch ee As ExevalatorException
            ' Expected to Thrown
            Console.WriteLine("Test of Detection of Mismatching of Open/Closed Parentheses 5: OK.")
        End Try

        Check( _
            "Test of Detection of Mismatching of Open/Closed Parentheses 6", _
            exevalator.Eval("1 + ((2 + (3 + 4) + 5) + 6)"), _
            1 + ((2 + (3 + 4) + 5) + 6) _
        )

        Try
            Check( _
                "Test of Detection of Mismatching of Open/Closed Parentheses 7", _
                exevalator.Eval("1 + ((2 + (3 + 4) + 5) + 6"), _
                1 + ((2 + (3 + 4) + 5) + 6) _
            )
            Throw New ExevalatorTestException("Expected exception has not been Thrown")
        Catch ee As ExevalatorException
            ' Expected to Thrown
            Console.WriteLine("Test of Detection of Mismatching of Open/Closed Parentheses 7: OK.")
        End Try

        Try
            Check( _
                "Test of Detection of Mismatching of Open/Closed Parentheses 8", _
                exevalator.Eval("1 + (2 + (3 + 4) + 5) + 6)"), _
                1 + ((2 + (3 + 4) + 5) + 6) _
            )
            Throw New ExevalatorTestException("Expected exception has not been Thrown")
        Catch ee As ExevalatorException
            ' Expected to Thrown
            Console.WriteLine("Test of Detection of Mismatching of Open/Closed Parentheses 8: OK.")
        End Try

        Try
            Check( _
                "Test of Detection of Empty Parentheses 1", _
                exevalator.Eval("()"), _
                Double.NaN _
            )
            Throw New ExevalatorTestException("Expected exception has not been Thrown")
        Catch ee As ExevalatorException
            ' Expected to Thrown
            Console.WriteLine("Test of Detection of Empty Parentheses 1: OK.")
        End Try

        Try
            Check( _
                "Test of Detection of Empty Parentheses 2", _
                exevalator.Eval("1 + ()"), _
                Double.NaN _
            )
            Throw New ExevalatorTestException("Expected exception has not been Thrown")
        Catch ee As ExevalatorException
            ' Expected to Thrown
            Console.WriteLine("Test of Detection of Empty Parentheses 2: OK.")
        End Try

        Try
            Check( _
                "Test of Detection of Empty Parentheses 3", _
                exevalator.Eval("() + 1"), _
                Double.NaN _
            )
            Throw New ExevalatorTestException("Expected exception has not been Thrown")
        Catch ee As ExevalatorException
            ' Expected to Thrown
            Console.WriteLine("Test of Detection of Empty Parentheses 3: OK.")
        End Try
    End Sub


    ' Tests syntax checks by the interpreter for locations of operators and leaf elements (literals and identifiers).
    Private Sub TestSyntaxChecksOfLocationsOfOperatorsAndLeafs()
        Dim exevalator As Exevalator = New Exevalator()

        Check( _
            "Test of Detection of Left Operand of Unary-Prefix Operator 1", _
            exevalator.Eval("1 + -123"), _
            1 + -123 _
        )

        Try
            Check( _
                "Test of Detection of Left Operand of Unary-Prefix Operator 2", _
                exevalator.Eval("1 + -"), _
                1 + -123 _
            )
            Throw New ExevalatorTestException("Expected exception has not been Thrown")
        Catch ee As ExevalatorException
            ' Expected to Thrown
            Console.WriteLine("Test of Detection of Left Operand of Unary-Prefix Operator 2: OK.")
        End Try

        Try
            Check( _
                "Test of Detection of Left Operand of Unary-Prefix Operator 3", _
                exevalator.Eval("(1 + -)"), _
                1 + -123 _
            )
            Throw New ExevalatorTestException("Expected exception has not been Thrown")
        Catch ee As ExevalatorException
            ' Expected to Thrown
            Console.WriteLine("Test of Detection of Left Operand of Unary-Prefix Operator 3: OK.")
        End Try

        Check( _
            "Test of Detection of Left Operand of Binary Operator 1", _
            exevalator.Eval("123 + 456"), _
            123 + 456 _
        )

        Try
            Check( _
                "Test of Detection of Left Operand of Binary Operator 2", _
                exevalator.Eval("123 *"), _
                123 * 456 _
            )
            Throw New ExevalatorTestException("Expected exception has not been Thrown")
        Catch ee As ExevalatorException
            ' Expected to Thrown
            Console.WriteLine("Test of Detection of Left Operand of Binary Operator 2: OK.")
        End Try

        Try
            Check( _
                "Test of Detection of Left Operand of Binary Operator 3", _
                exevalator.Eval("* 456"), _
                123 * 456 _
            )
            Throw New ExevalatorTestException("Expected exception has not been Thrown")
        Catch ee As ExevalatorException
            ' Expected to Thrown
            Console.WriteLine("Test of Detection of Left Operand of Binary Operator 3: OK.")
        End Try

        Try
            Check( _
                "Test of Detection of Left Operand of Binary Operator 4", _
                exevalator.Eval("123 + ( * 456)"), _
                123 * 456 _
            )
            Throw New ExevalatorTestException("Expected exception has not been Thrown")
        Catch ee As ExevalatorException
            ' Expected to Thrown
            Console.WriteLine("Test of Detection of Left Operand of Binary Operator 4: OK.")
        End Try

        Try
            Check( _
                "Test of Detection of Left Operand of Binary Operator 5", _
                exevalator.Eval("(123 *) + 456"), _
                123 * 456 _
            )
            Throw New ExevalatorTestException("Expected exception has not been Thrown")
        Catch ee As ExevalatorException
            ' Expected to Thrown
            Console.WriteLine("Test of Detection of Left Operand of Binary Operator 5: OK.")
        End Try

        Try
            Check( _
                "Test of Detection of Lacking Operator", _
                exevalator.Eval("123 456"), _
                123 * 456 _
            )
            Throw New ExevalatorTestException("Expected exception has not been Thrown")
        Catch ee As ExevalatorException
            ' Expected to Thrown
            Console.WriteLine("Test of Detection of Lacking Operator: OK.")
        End Try
    End Sub


    Private Sub TestVariables()
        Dim exevalator As Exevalator = New Exevalator()

        Try
            exevalator.Eval("x")
            Throw New ExevalatorTestException("Expected exception has not been Thrown")
        Catch ee As ExevalatorException
            ' Expected to be Thrown
            Console.WriteLine("Test of Variables 1: OK.")
        End Try

        Dim xAddress As Integer = exevalator.DeclareVariable("x")

        Check( _
            "Test of Variables 2", _
            exevalator.Eval("x"), _
            0.0 _
        )

        exevalator.WriteVariable("x", 1.25)

        Check( _
            "Test of Variables 3", _
            exevalator.Eval("x"), _
            1.25 _
        )

        exevalator.WriteVariableAt(xAddress, 2.5)

        Check( _
            "Test of Variables 4", _
            exevalator.Eval("x"), _
            2.5 _
        )

        Try
            exevalator.WriteVariableAt(100, 5.0)
            Throw New ExevalatorTestException("Expected exception has not been Thrown")
        Catch ee As ExevalatorException
            ' Expected to be Thrown
            Console.WriteLine("Test of Variables 5: OK.")
        End Try

        Try
            exevalator.Eval("y")
            Throw New ExevalatorTestException("Expected exception has not been Thrown")
        Catch ee As ExevalatorException
            ' Expected to be Thrown
            Console.WriteLine("Test of Variables 6: OK.")
        End Try

        Dim yAddress As Integer = exevalator.DeclareVariable("y")

        Check( _
            "Test of Variables 7", _
            exevalator.Eval("y"), _
            0.0 _
        )

        exevalator.WriteVariable("y", 0.25)

        Check( _
            "Test of Variables 8", _
            exevalator.Eval("y"), _
            0.25 _
        )

        exevalator.WriteVariableAt(yAddress, 0.5)

        Check( _
            "Test of Variables 9", _
            exevalator.Eval("y"), _
            0.5 _
        )

        Check( _
            "Test of Variables 10", _
            exevalator.Eval("x + y"), _
            2.5 + 0.5 _
        )

        ' Variables having names containing numbers
        exevalator.DeclareVariable("x2")
        exevalator.DeclareVariable("y2")
        exevalator.WriteVariable("x2", 22.5)
        exevalator.WriteVariable("y2", 32.5)
        Check( _
            "Test of Variables 11", _
            exevalator.Eval("x + y + 2 + x2 + 2 * y2"), _
            2.5 + 0.5 + 2.0 + 22.5 + 2.0 * 32.5 _
        )
    End Sub


    Class FunctionA : Implements IExevalatorFunction
        Public Function Invoke(args() As Double) As Double Implements IExevalatorFunction.Invoke
            If args.Length <> 0 Then
                Throw New ExevalatorException("Incorrect number of arguments")
            End If
            Return 1.25
        End Function
    End Class

    Class FunctionB : Implements IExevalatorFunction
        Public Function Invoke(args() As Double) As Double Implements IExevalatorFunction.Invoke
            If args.Length <> 1 Then
                Throw New ExevalatorException("Incorrect number of arguments")
            End If
            return args(0)
        End Function
    End Class

    Class FunctionC : Implements IExevalatorFunction
        Public Function Invoke(args() As Double) As Double Implements IExevalatorFunction.Invoke
            If args.Length <> 2 Then
                Throw New ExevalatorException("Incorrect number of arguments")
            End If
            return args(0) + args(1)
        End Function
    End Class

    Class FunctionD : Implements IExevalatorFunction
        Public Function Invoke(args() as Double) As Double Implements IExevalatorFunction.Invoke
            If args.Length <> 3 Then
                Throw New ExevalatorException("Incorrect number of arguments")
            End If
            If args(0) <> 1.25 Then
                Throw New ExevalatorException("The value of args[0] is incorrect")
            End If
            If args(1) <> 2.5 Then
                Throw New ExevalatorException("The value of args[1] is incorrect")
            End If
            If args(2) <> 5.0 Then
                Throw New ExevalatorException("The value of args[2] is incorrect")
            End If
            Return 0.0
        End Function
    End Class

    Private Sub TestFunctions()
        Dim exevalator As Exevalator = New Exevalator()

        Try
            exevalator.Eval("funA()")
            Throw New ExevalatorTestException("Expected exception has not been Thrown")
        Catch ee As ExevalatorException
            ' Expected to be Thrown
            Console.WriteLine("Test of Functions 1: OK.")
        End Try

        exevalator.ConnectFunction("funA", New FunctionA())
        Check( _
            "Test of Functions 2", _
            exevalator.Eval("funA()"), _
            1.25 _
        )

        Try
            exevalator.Eval("funB(2.5)")
            Throw New ExevalatorTestException("Expected exception has not been Thrown")
        Catch ee As ExevalatorException
            ' Expected to be Thrown
            Console.WriteLine("Test of Functions 3: OK.")
        End Try

        exevalator.ConnectFunction("funB", new FunctionB())
        Check( _
            "Test of Functions 4", _
            exevalator.Eval("funB(2.5)"), _
            2.5 _
        )

        exevalator.ConnectFunction("funC", new FunctionC())
        Check( _
            "Test of Functions 5", _
            exevalator.Eval("funC(1.25, 2.5)"), _
            1.25 + 2.5 _
        )

        Check( _
            "Test of Functions 6", _
            exevalator.Eval("funC(funA(), funB(2.5))"), _
            1.25 + 2.5 _
        )

        Check( _
            "Test of Functions 7", _
            exevalator.Eval("funC(funC(funA(), funB(2.5)), funB(1.0))"), _
            1.25 + 2.5 + 1.0 _
        )

        Check( _
            "Test of Function 8", _
            exevalator.Eval("funC(1.0, 3.5 * funB(2.5) / 2.0)"), _
            1.0 + 3.5 * 2.5 / 2.0 _
        )

        Check( _
            "Test of Functions 9", _
            exevalator.Eval("funA() * funC(funC(funA(), 3.5 * funB(2.5) / 2.0), funB(1.0))"), _
            1.25 * (1.25 + 3.5 * 2.5 / 2.0 + 1.0) _
        )

        Check( _
            "Test of Functions 10", _
            exevalator.Eval("2 + 256 * funA() * funC(funC(funA(), 3.5 * funB(2.5) / 2.0), funB(1.0)) * 128"), _
            2.0 + 256.0 * (1.25 * (1.25 + 3.5 * 2.5 / 2.0 + 1.0)) * 128.0 _
        )

        exevalator.ConnectFunction("funD", new FunctionD())
        Check( _
            "Test of Functions 11", _
            exevalator.Eval("funD(1.25, 2.5, 5.0)"), _
            0.0 _
        )

        Check(
            "Test of Functions 12", _
            exevalator.Eval("-funC(-1.25, -2.5)"), _
            - (-1.25 + -2.5) _
        )
    End Sub

    Private Sub TestEmptyExpressions()
        Dim exevalator As Exevalator = New Exevalator()

        Try
            exevalator.Eval("")
            Throw New ExevalatorTestException("Expected exception has not been Thrown")
        Catch ee As ExevalatorException
            ' Expected to be Thrown
            Console.WriteLine("Test of Empty Expressions 1: OK.")
        End Try

        Try
            exevalator.Eval(" ")
            Throw New ExevalatorTestException("Expected exception has not been Thrown")
        Catch ee As ExevalatorException
            ' Expected to be Thrown
            Console.WriteLine("Test of Empty Expressions 2: OK.")
        End Try

        Try
            exevalator.Eval("  ")
            Throw New ExevalatorTestException("Expected exception has not been Thrown")
        Catch ee As ExevalatorException
            ' Expected to be Thrown
            Console.WriteLine("Test of Empty Expressions 3: OK.")
        End Try

        Try
            exevalator.Eval("   ")
            Throw New ExevalatorTestException("Expected exception has not been Thrown")
        Catch ee As ExevalatorException
            ' Expected to be Thrown
            Console.WriteLine("Test of Empty Expressions 4: OK.")
        End Try
    End Sub


    Private Sub TestReeval()
        Dim exevalator As Exevalator = New Exevalator()

        Check( _
            "Test of reval() Method 1", _
            exevalator.Eval("1.2 + 3.4"), _
            1.2 + 3.4 _
        )

        Check( _
            "Test of reval() Method 2", _
            exevalator.Reeval(), _
            1.2 + 3.4 _
        )

        Check( _
            "Test of reval() Method 3", _
            exevalator.Reeval(), _
            1.2 + 3.4 _
        )

        Check( _
            "Test of reval() Method 4", _
            exevalator.Eval("5.6 - 7.8"), _
            5.6 - 7.8 _
        )

        Check( _
            "Test of reval() Method 5", _
            exevalator.Reeval(), _
            5.6 - 7.8 _
        )

        Check( _
            "Test of reval() Method 6", _
            exevalator.Reeval(), _
            5.6 - 7.8 _
        )

        Check( _
            "Test of reval() Method 7", _
            exevalator.Eval("(1.23 + 4.56) * 7.89"), _
            (1.23 + 4.56) * 7.89 _
        )

        Check( _
            "Test of reval() Method 8", _
            exevalator.Reeval(), _
            (1.23 + 4.56) * 7.89 _
        )

        Check( _
            "Test of reval() Method 9", _
            exevalator.Reeval(), _
            (1.23 + 4.56) * 7.89 _
        )
    End Sub

    Private Sub TestTokenization()
        Dim exevalator As Exevalator = new Exevalator()

        Check( _
            "Test of Tokenization 1", _
            exevalator.Eval("1.2345678"), _
            1.2345678 _
        )

        Try
            exevalator.Eval("1.234\n5678")
            Throw new ExevalatorTestException("Expected exception has not been thrown")
        Catch ee As ExevalatorException
            ' Expected to be 
            Console.WriteLine("Test of Tokenization 2: OK.")
        End Try

        Try
            exevalator.Eval("1.234\r\n5678")
            Throw new ExevalatorTestException("Expected exception has not been thrown")
        Catch ee As ExevalatorException
            ' Expected to be thrown
            Console.WriteLine("Test of Tokenization 3: OK.")
        End Try

        Try
            exevalator.Eval("1.234\t5678")
            Throw new ExevalatorTestException("Expected exception has not been thrown")
        Catch ee As ExevalatorException
            ' Expected to be thrown
            Console.WriteLine("Test of Tokenization 4: OK.")
        End Try

        Try
            exevalator.Eval("1.234 5678")
            Throw new ExevalatorTestException("Expected exception has not been thrown")
        Catch ee As ExevalatorException
            ' Expected to be thrown
            Console.WriteLine("Test of Tokenization 5: OK.")
        End Try

        Check( _
            "Test of Tokenization 6", _
            exevalator.Eval("1+2*3-4/5"), _
            1.0 + 2.0 * 3.0 - 4.0 / 5.0 _
        )

        Check( _
            "Test of Tokenization 7", _
            exevalator.Eval("1+" & vbLf & "2*3" & vbCrLf & "-4/5"), _
            1.0 + 2.0 * 3.0 - 4.0 / 5.0 _
        )

        Check( _
            "Test of Tokenization 8", _
            exevalator.Eval("((1+2)*3)-(4/5)"), _
            ((1.0 + 2.0) * 3.0) - (4.0 / 5.0) _
        )

        Dim funC As FunctionC = new FunctionC()
        exevalator.ConnectFunction("funC", funC)

        Check( _
            "Test of Tokenization 9", _
            exevalator.Eval("funC(1,2)"), _
            1.0 + 2.0 _
        )

        Check( _
            "Test of Tokenization 10", _
            exevalator.Eval("funC(" & vbLf & "1," & vbCrLf & "2" & vbTab & ")"), _
            1.0 + 2.0 _
        )

        Check( _
            "Test of Tokenization 11", _
            exevalator.Eval("3*funC(1,2)/2"), _
            3.0 * (1.0 + 2.0) / 2.0 _
        )

        Check( _
            "Test of Tokenization 12", _
            exevalator.Eval("3*(-funC(1,2)+2)"), _
            3.0 * (-(1.0 + 2.0) + 2.0) _
        )
    End Sub


    ' Checks the evaluated (computed) value of the testing expression by the Exevalator.
    ' evaluatedValue: The evaluated (computed) value of the testing expression by Exevalator
    ' correctValue: The correct value of the testing expression
    ' testName: The name of the testing
    Private Sub Check(testName As String, evaluatedValue As Double, correctValue As Double)
        If Math.Abs(evaluatedValue - correctValue) < AllowableError Then
            Console.WriteLine(testName + ": OK.")
            return
        End If
        Throw New ExevalatorTestException( _
            "\"" + testName + "\" has failed. " + _
            "evaluatedValue=" + evaluatedValue + ", " + _
            "correctValue=" + correctValue + "." _
        )
    End Sub


    ' The Exception class Thrown when any test has failed.
    Private Class ExevalatorTestException
        Inherits Exception
        Public Sub New(errorMessage As String)
            MyBase.New(errorMessage)
        End Sub
    End Class
End Module
