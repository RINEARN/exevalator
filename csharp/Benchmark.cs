using System;
using System.Diagnostics;
using Rinearn.ExevalatorCS;

/// <summary>
/// A benchmark to measure the speed of repeated calculations.
/// </summary>
static class Example1
{
    static void Main()
    {
        Console.WriteLine("Please wait...");

        ulong loops = 100UL * 1000UL * 1000UL; // 100M LOOPS
        ulong flopPerLoop = 10UL;

        Exevalator exevalator = new Exevalator();
        int address = exevalator.DeclareVariable("x");
        double sum = 0.0;

        // Measure required time for evaluating a expression repeatedly for 100M times,
        // where each 10 numerical operations are required for each evaluation.
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        for (ulong i=1; i<=loops; ++i)
        {
            exevalator.WriteVariableAt(address, (double)i);
            sum += exevalator.Eval("x + 1 - 1 + 1 - 1 + 1 - 1 + 1 - 1 + 1 - 1");
        }
        stopwatch.Stop();
        double elapsedSec = stopwatch.ElapsedMilliseconds * 1.0E-3;

        // Display results:
        double evalSpeed = loops / elapsedSec;
        double megaFlops = flopPerLoop * loops / elapsedSec / (1000.0 * 1000.0);
        double correctSum = (loops * (loops + 1)) / 2.0;
        Console.WriteLine("-----");
        Console.WriteLine("EVAL-LOOP SPEED: " + evalSpeed + " [EVALS/SEC]");
        Console.WriteLine("OPERATION SPEED: " + megaFlops + " [M FLOPS]");
        Console.WriteLine("VALUE OF \"sum\" : " + sum + " (EXPECTED: "+ correctSum + ")");
    }
}
