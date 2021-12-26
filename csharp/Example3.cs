using System;
using Rinearn.ExevalatorCS;

/// <summary>
/// An example to use a variable.
/// </summary>
static class Example3
{
    static void Main()
    {
        // Create an instance of Exevalator Engine
        Exevalator exevalator = new Exevalator();

        // Declare a variable and set the value
        exevalator.DeclareVariable("x");
        exevalator.WriteVariable("x", 1.25);

        // Evaluate the value of an expression
        double result = exevalator.Eval("x + 1");

        // Display the result
        Console.WriteLine("result: " + result);
    }
}
