using System;
using Rinearn.ExevalatorCS;

/// <summary>
/// An example to access a variable by its address.
/// </summary>
static class Example4
{
    static void Main()
    {
        // Create an instance of Exevalator Engine
        Exevalator exevalator = new Exevalator();

        // Declare a variable and set the value
        int address = exevalator.DeclareVariable("x");
        exevalator.WriteVariableAt(address, 1.25);
        // The above works faster than:
        //    exevalator.writeVariable("x", 1.25);

        // Evaluate the value of an expression
        double result = exevalator.Eval("x + 1");

        // Display the result
        Console.WriteLine("result: " + result);
    }
}
