using System;
using System.Globalization;
using Rinearn.ExevalatorCS;

/// <summary>
/// An example to compute the numerical integration value of the inputted expression f(x).
/// For details of the numerical integration algorithm used in this code, see:
///   https://www.vcssl.org/en-us/code/archive/0001/7800-vnano-integral-output/   (English)
///   https://www.vcssl.org/ja-jp/code/archive/0001/7800-vnano-integral-output/   (Japanese)
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

        // Get the value of the lower limit from the standard-input
        Console.WriteLine("lower-limit = ?        (default: 0)");
        string lowerLimitStr = Console.ReadLine();
        double lowerLimit = 0.0;
        if (lowerLimitStr.Length != 0)
        {
            try
            {
                lowerLimit = double.Parse(lowerLimitStr, CultureInfo.InvariantCulture);
            }
            catch (System.FormatException)
            {
                Console.Error.WriteLine("Invalid lower-limit value:" + lowerLimitStr);
                return;
            }
        }

        // Get the value of the upper limit from the standard-input
        Console.WriteLine("upper-limit = ?        (default: 1)");
        string upperLimitStr = Console.ReadLine();
        double upperLimit = 1.0;
        if (lowerLimitStr.Length != 0)
        {
            try
            {
                upperLimit = double.Parse(upperLimitStr, CultureInfo.InvariantCulture);
            }
            catch (System.FormatException)
            {
                Console.Error.WriteLine("Invalid upper-limit value:" + upperLimitStr);
                return;
            }
        }

        // Other numerical integration parameters
        int numberOfSteps = 65536;
        double delta = (upperLimit - lowerLimit) / numberOfSteps;
        double result = 0.0;

        // Create an instance of Exevalator Engine
        Exevalator exevalator = new Exevalator();
        int xAddress = exevalator.DeclareVariable("x");

        // Traverse tiny intervals from lower-limit to upper-limit
        for (int i=0; i<numberOfSteps; i++)
        {
            // The x-coordinate value of the left-bottom point of i-th tiny interval
            double x = lowerLimit + i * delta;

            // Compute area of i-th tiny interval approximately by using Simpson's rule,
            // and add it to the variable "result"
            // (see: https://www.vcssl.org/en-us/code/archive/0001/7800-vnano-integral-output/ )

            exevalator.WriteVariableAt(xAddress, x);
            double fxLeft = exevalator.Eval(expression);

            exevalator.WriteVariableAt(xAddress, x + delta);
            double fxRight = exevalator.Eval(expression);

            exevalator.WriteVariableAt(xAddress, x + delta/2.0);
            double fxCenter = exevalator.Eval(expression);

            result += (fxLeft + fxRight + 4.0 * fxCenter) * delta / 6.0;
        }

        // Display the result
        Console.WriteLine("----------");
        Console.WriteLine("f(x)        = " + expression);
        Console.WriteLine("lower-limit = " + lowerLimit);
        Console.WriteLine("upper-limit = " + upperLimit);
        Console.WriteLine("result      = " + result);
    }
}
