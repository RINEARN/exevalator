Imports System
Imports Rinearn.ExevalatorVB

' IMPORTANT NOTE:
' In a VB.NET project, by default,
' the project name is implicitly added to the top hierarchy of the project's namespaces.
' If you execute this code in a project named "YourProject", you must modify the above Imports statement as follows:
'
'    Imports YourProject.Rinearn.ExevalatorVB

''' <summary>
''' An example to access a variable by its address.
''' </summary>
Module Example4
    Sub Main()

        ' Create an instance of Exevalator Engine
        Dim exevalator As Exevalator = New Exevalator()

        ' Declare a variable and set the value
        Dim address As Integer = exevalator.DeclareVariable("x")
        exevalator.WriteVariableAt(address, 1.25)
        ' The above works faster than:
        '    exevalator.writeVariable("x", 1.25)

        ' Evaluate the value of an expression
        Dim result As double = exevalator.Eval("x + 1")

        ' Display the result
        Console.WriteLine("result: " + result.ToString())

    End Sub
End Module
