using System;
using Rinearn.ExevalatorCS;

/// <summary>
/// Function available in expressions.
/// </summary>
class MyFunction : IExevalatorFunction
{
    /// <summary>
    /// Invoke the function.
    /// </summary>
    /// <param name="arguments" An array storing values of arguments.</paran>
    /// <returns>The return value of the function.</returns>
    public double invoke(double[] arguments)
    {
        if (arguments.Length != 2)
        {
            throw new ExevalatorException("Incorrected number of args");
        }
        return arguments[0] + arguments[1];
    }
}

/// <summary>
/// An example to create a function for available in expressions.
/// </summary>
static class Example5
{
    static void Main()
    {
        // Create an instance of Exevalator Engine
        Exevalator exevalator = new Exevalator();

        // Connects the function available for using it in expressions
        MyFunction fun = new MyFunction();
        exevalator.ConnectFunction("fun", fun);

        // Evaluate the value of an expression
        double result = exevalator.Eval("fun(1.2, 3.4)");

        // Display the result
        Console.WriteLine("result: " + result);
    }
}
