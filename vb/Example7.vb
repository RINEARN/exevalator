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
''' An example to compute the numerical integration value of the inputted expression f(x).
''' For details of the numerical integration algorithm used in this code, see:
'''   https://www.vcssl.org/en-us/code/archive/0001/7800-vnano-integral-output/   (English)
'''   https://www.vcssl.org/ja-jp/code/archive/0001/7800-vnano-integral-output/   (Japanese)
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

        ' Get the value of the lower limit from the standard-input
        Console.WriteLine("lower-limit = ?        (default: 0)")
        Dim lowerLimitStr As String = Console.ReadLine()
        Dim lowerLimit As Double = 0.0
        If lowerLimitStr.Length <> 0 Then
            Try
                lowerLimit = System.Double.Parse(lowerLimitStr, CultureInfo.InvariantCulture)
            Catch ex As System.FormatException
                Console.Error.WriteLine("Invalid lower-limite value:" + lowerLimitStr)
                Return
            End Try
        End If

        ' Get the value of the upper limit from the standard-input
        Console.WriteLine("upper-limit = ?        (default: 1)")
        Dim upperLimitStr As String = Console.ReadLine()
        Dim upperLimit As Double = 1.0
        If upperLimitStr.Length <> 0 Then
            Try
                upperLimit = System.Double.Parse(upperLimitStr, CultureInfo.InvariantCulture)
            Catch ex As System.FormatException
                Console.Error.WriteLine("Invalid upper-limite value:" + upperLimitStr)
                Return
            End Try
        End If

        ' Other numerical integration parameters
        Dim numberOfSteps As Integer = 65536
        Dim delta As Double = (upperLimit - lowerLimit) / numberOfSteps
        Dim result As Double = 0.0

        ' Create an instance of Exevalator Engine
        Dim exevalator As Exevalator = new Exevalator()
        Dim xAddress As Integer = exevalator.DeclareVariable("x")

        ' Traverse tiny intervals from lower-limit to upper-limit
        For i As Integer = 0 To numberOfSteps - 1

            ' The x-coordinate value of the left-bottom point of i-th tiny interval
            Dim x As Double = lowerLimit + i * delta

            ' Compute area of i-th tiny interval approximately by using Simpson's rule,
            ' and add it to the variable "result"
            ' (see: https://www.vcssl.org/en-us/code/archive/0001/7800-vnano-integral-output/ )

            exevalator.WriteVariableAt(xAddress, x)
            Dim fxLeft As Double = exevalator.Eval(expression)

            exevalator.WriteVariableAt(xAddress, x + delta)
            Dim fxRight As Double = exevalator.Eval(expression)

            exevalator.WriteVariableAt(xAddress, x + delta/2.0)
            Dim fxCenter As Double = exevalator.Eval(expression)

            result += (fxLeft + fxRight + 4.0 * fxCenter) * delta / 6.0
        Next

        ' Display the result
        Console.WriteLine("----------")
        Console.WriteLine("f(x)        = " + expression)
        Console.WriteLine("lower-limit = " + lowerLimit.ToString())
        Console.WriteLine("upper-limit = " + upperLimit.ToString())
        Console.WriteLine("result      = " + result.ToString())

    End Sub
End Module
