Imports System
Imports Rinearn.ExevalatorVB

' IMPORTANT NOTE:
' In a VB.NET project, by default,
' the project name is implicitly added to the top hierarchy of the project's namespaces.
' If you execute this code in a project named "YourProject", you must modify the above Imports statement as follows:
'
'    Imports YourProject.Rinearn.ExevalatorVB

''' <summary>
''' A simple example to use Exevalator.
''' </summary>
Module Example1
    Sub Main()

        ' Create an instance of Exevalator Engine
        Dim exevalator As Exevalator = New Exevalator()

        ' Evaluate the value of an expression
        Dim result As Double = exevalator.Eval("1.2 + 3.4")

        ' Display the result
        Console.WriteLine("result: " + result.ToString())

    End Sub
End Module
