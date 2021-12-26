using System;
using Rinearn.ExevalatorCS;

/// <summary>
/// A simple example to use Exevalator.
/// </summary>
static class Example1
{
    static void Main()
    {
        // Create an instance of Exevalator Engine
        Exevalator exevalator = new Exevalator();

        // Evaluate the value of an expression
        double result = exevalator.Eval("1.2 + 3.4");

        // Display the result
        Console.WriteLine("result: " + result);
    }
}
