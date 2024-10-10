// import anypackage.in.which.you.put.Exevalator;

import java.util.Scanner;

/**
 * An example to compute the numerical integration value of the inputted expression f(x).
 * For details of the numerical integration algorithm used in this code, see:
 *   https://www.vcssl.org/en-us/code/archive/0001/7800-vnano-integral-output/   (English)
 *   https://www.vcssl.org/ja-jp/code/archive/0001/7800-vnano-integral-output/   (Japanese)
 */
public class Example7 {

    public static void main(String[] args) {

        // Get the expression from the standard-input
        Scanner scanner = new Scanner(System.in);
        System.out.println("");
        System.out.println("This program computes integrated value of f(x) from lower-limit to upper-limit.");
        System.out.println("");
        System.out.println("f(x) = ?               (default: 3*x*x + 2*x + 1)");
        String expression = scanner.nextLine();
        if (expression.length() == 0) {
            expression = "3*x*x + 2*x + 1";
        }

        // Get the value of the lower limit from the standard-input
        System.out.println("lower-limit = ?        (default: 0)");
        double lowerLimit = 0.0;
        String lowerLimitStr = scanner.nextLine();
        if (lowerLimitStr.length() != 0) {
            try {
                lowerLimit = Double.parseDouble(lowerLimitStr);
            } catch (NumberFormatException nfe) {
                System.err.println("Invalid lower-limit value:" + lowerLimitStr);
                return;
            }
        }

        // Get the value of the upper limit from the standard-input
        System.out.println("upper-limit = ?        (default: 1)");
        String upperLimitStr = scanner.nextLine();
        double upperLimit = 1.0;
        if (upperLimitStr.length() != 0) {
            try {
                upperLimit = Double.parseDouble(upperLimitStr);
            } catch (NumberFormatException nfe) {
                System.err.println("Invalid upper-limit value:" + upperLimitStr);
                return;
            }
        }
        scanner.close();

        int numberOfSteps = 65536;
        double delta = (upperLimit - lowerLimit) / numberOfSteps;
        double result = 0.0;

        // Create an instance of Exevalator Engine
        Exevalator exevalator = new Exevalator();
        int xAddress = exevalator.declareVariable("x");

        // Traverse tiny intervals from lower-limit to upper-limit
        for (int i=0; i<numberOfSteps; i++) {

            // The x-coordinate value of the left-bottom point of i-th tiny interval
            double x = lowerLimit + i * delta;

            // Compute area of i-th tiny interval approximately by using Simpson's rule,
            // and add it to the variable "result"
            // (see: https://www.vcssl.org/en-us/code/archive/0001/7800-vnano-integral-output/ )

            exevalator.writeVariableAt(xAddress, x);
            double fxLeft = exevalator.eval(expression);

            exevalator.writeVariableAt(xAddress, x + delta);
            double fxRight = exevalator.eval(expression);

            exevalator.writeVariableAt(xAddress, x + delta/2.0);
            double fxCenter = exevalator.eval(expression);

            result += (fxLeft + fxRight + 4.0 * fxCenter) * delta / 6.0;
        }

        // Display the result
        System.out.println("----------");
        System.out.println("f(x)        = " + expression);
        System.out.println("lower-limit = " + lowerLimit);
        System.out.println("upper-limit = " + upperLimit);
        System.out.println("result      = " + result);
    }
}
