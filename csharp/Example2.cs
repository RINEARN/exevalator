using System;
using Rinearn.ExevalatorCS;

/// <summary>
/// An example to use various operators and parentheses.
/// </summary>
static class Example2
{
    public static void Main()
    {
        // Create an instance of Exevalator Engine
        Exevalator exevalator = new Exevalator();

        // Evaluate the value of an expression
        double result = exevalator.Eval("(-(1.2 + 3.4) * 5) / 2");

        // Display the result
        Console.WriteLine("Result: " + result);
    }
}
