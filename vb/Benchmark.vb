Imports System
Imports System.Diagnostics
Imports Rinearn.ExevalatorVB

' IMPORTANT NOTE:
' In a VB.NET project, by default,
' the project name is implicitly added to the top hierarchy of the project's namespaces.
' If you execute this code in a project named "YourProject", you must modify the above Imports statement as follows:
'
'    Imports YourProject.Rinearn.ExevalatorVB


' !!! DON'T FORGET TO SPECIFY OPTIMIZATION OPTIONS WHEN YOU COMPILE THIS CODE !!!
'     e.g.:
'           vbc -optimize Benchmark.vb Exevalator.vb


''' <summary>
''' A benchmark to measure the speed of repeated calculations.
''' </summary>
Module Benchmark
    Sub Main()

        Console.WriteLine("Please wait...")

        Dim loops As ULong = 100UL * 1000UL * 1000UL ' 100M LOOPS
        Dim flopPerLoop As ULong = 10UL

        Dim exevalator As Exevalator = New Exevalator()
        Dim address As Integer = exevalator.DeclareVariable("x")
        Dim sum As Double = 0.0

        ' Measure required time for evaluating a expression repeatedly for 100M times,
        ' where each 10 numerical operations are required for each evaluation.
        Dim stopwatch As Stopwatch = new Stopwatch()
        stopwatch.Start()
        For i As ULong = 1UL To loops
            exevalator.WriteVariableAt(address, CType(i, Double))
            sum += exevalator.Eval("x + 1 - 1 + 1 - 1 + 1 - 1 + 1 - 1 + 1 - 1")
        Next
        stopwatch.Stop()
        Dim elapsedSec As Double = stopwatch.ElapsedMilliseconds * 1.0E-3

        ' Display results:
        Dim evalSpeed As Double = loops / elapsedSec
        Dim megaFlops As Double = flopPerLoop * loops / elapsedSec / (1000.0 * 1000.0)
        Dim correctSum As Double = (loops * (loops + 1)) / 2.0
        Console.WriteLine("-----")
        Console.WriteLine("EVAL-LOOP SPEED: " + evalSpeed.ToString() + " [EVALS/SEC]")
        Console.WriteLine("OPERATION SPEED: " + megaFlops.ToString() + " [M FLOPS]")
        Console.WriteLine("VALUE OF ""sum"" : " + sum.ToString() + " (EXPECTED: "+ correctSum.ToString() + ")")

    End Sub
End Module
