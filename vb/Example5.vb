Imports System
Imports Rinearn.ExevalatorVB

' IMPORTANT NOTE:
' In a VB.NET project, by default,
' the project name is implicitly added to the top hierarchy of the project's namespaces.
' If you execute this code in a project named "YourProject", you must modify the above Imports statement as follows:
'
'    Imports YourProject.Rinearn.ExevalatorVB

''' <summary>
''' Function available in expressions.
''' </summary>
Class MyFunction : Implements IExevalatorFunction

    ''' <summary>
    ''' Invoke the function.
    ''' </summary>
    ''' <param name="arguments" An array storing values of arguments.</paran>
    ''' <returns>The return value of the function.</returns>
    Public Function Invoke(arguments() As Double) As Double Implements IExevalatorFunction.Invoke
        If arguments.Length <> 2 Then
            Throw New ExevalatorException("Incorrected number of args")
        End If
        Return arguments(0) + arguments(1)
    End Function
End Class

''' <summary>
''' An example to create a function for available in expressions.
''' </summary>
Module Example5
    Sub Main()

        ' Create an instance of Exevalator Engine
        Dim exevalator As Exevalator = New Exevalator()

        ' Connects the function available for using it in expressions
        Dim fun As MyFunction = new MyFunction()
        exevalator.ConnectFunction("fun", fun)

        ' Evaluate the value of an expression
        Dim result As Double = exevalator.Eval("fun(1.2, 3.4)")

        ' Display the result
        Console.WriteLine("result: " + result.ToString())

    End Sub
End Module
