using System;
using System.Globalization;
using Rinearn.ExevalatorCS;

/// <summary>
/// An example to compute the value of the inputted expression f(x) at the inputted x.
/// </summary>
static class Example5
{
    static void Main()
    {

        // Get the expression from the standard-input
        Console.WriteLine("");
        Console.WriteLine("This program computes the value of f(x) at x.");
        Console.WriteLine("");
        Console.WriteLine("f(x) = ?               (default: 3*x*x + 2*x + 1)");
        string expression = Console.ReadLine();
        if (expression.Length == 0)
        {
            expression = "3*x*x + 2*x + 1";
        }

        // Get the value of the x from the standard-input
        Console.WriteLine("x = ?        (default: 1)");
        string xStr = Console.ReadLine();
        double xValue = 1.0;
        if (xStr.Length != 0)
        {
            try
            {
                xValue = double.Parse(xStr, CultureInfo.InvariantCulture);
            }
            catch (System.FormatException)
            {
                Console.Error.WriteLine("Invalid x value:" + xStr);
                return;
            }
        }
        
        // Create an instance of Exevalator Engine
        Exevalator exevalator = new Exevalator();

        // Set the value of x
        exevalator.DeclareVariable("x");
        exevalator.WriteVariable("x", xValue);

        // Evaluate the value of an expression
        double result = exevalator.Eval(expression);

        // Display the result
        Console.WriteLine("----------");
        Console.WriteLine("f(x)   = " + expression);
        Console.WriteLine("x      = " + xValue);
        Console.WriteLine("result = " + result);
    }
}
