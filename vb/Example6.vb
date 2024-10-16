Imports System
Imports System.Globalization
Imports Rinearn.ExevalatorVB

' IMPORTANT NOTE:
' In a VB.NET project, by default,
' the project name is implicitly added to the top hierarchy of the project's namespaces.
' If you execute this code in a project named "YourProject", you must modify the above Imports statement as follows:
'
'    Imports YourProject.Rinearn.ExevalatorVB

''' <summary>
''' An example to create a function for available in expressions.
''' </summary>
Module Example5
    Sub Main()

        ' Get the expression from the standard-input
        Console.WriteLine("")
        Console.WriteLine("This program computes the value of f(x) at x.")
        Console.WriteLine("")
        Console.WriteLine("f(x) = ?               (default: 3*x*x + 2*x + 1)")
        Dim expression As String = Console.ReadLine()
        If expression.Length = 0 Then
            expression = "3*x*x + 2*x + 1"
        End If

        ' Get the value of the x from the standard-input
        Console.WriteLine("x = ?        (default: 1)")
        Dim xStr As String = Console.ReadLine()
        Dim xValue As Double = 1.0
        If xStr.Length <> 0 Then
            Try
                xValue = System.Double.Parse(xStr, CultureInfo.InvariantCulture)
            Catch ex As System.FormatException
                Console.Error.WriteLine("Invalid x value:" + xStr)
                Return
            End Try
        End If

        ' Create an instance of Exevalator Engine
        Dim exevalator As Exevalator = New Exevalator()

        ' Set the value of x
        exevalator.DeclareVariable("x")
        exevalator.WriteVariable("x", xValue)

        ' Evaluate the value of an expression
        Dim result As Double = exevalator.Eval(expression)

        ' Display the result
        Console.WriteLine("----------")
        Console.WriteLine("f(x)   = " + expression)
        Console.WriteLine("x      = " + xValue.ToString())
        Console.WriteLine("result = " + result.ToString())

    End Sub
End Module
