// import anypackage.in.which.you.put.Exevalator;

import java.util.Scanner;

/**
 * An example to compute the value of the inputted expression f(x) at the inputted x.
 */
public class Example6 {

    public static void main(String[] args) {

        // Get the expression from the standard-input
        Scanner scanner = new Scanner(System.in);
        System.out.println("");
        System.out.println("This program computes the value of f(x) at x.");
        System.out.println("");
        System.out.println("f(x) = ?               (default: 3*x*x + 2*x + 1)");
        String expression = scanner.nextLine();
        if (expression.length() == 0) {
            expression = "3*x*x + 2*x + 1";
        }

        // Get the value of the x from the standard-input
        System.out.println("x = ?                  (default: 1)");
        double xValue = 0.0;
        String xValueStr = scanner.nextLine();
        if (xValueStr.length() != 0) {
            xValue = Double.parseDouble(xValueStr);
        }
        scanner.close();

        // Create an instance of Exevalator Engine
        Exevalator exevalator = new Exevalator();

        // Set the value of x
        exevalator.declareVariable("x");
        exevalator.writeVariable("x", xValue);

        // Compute the value of the inputted expression
        double result = exevalator.eval(expression);

        // Display the result
        System.out.println("----------");
        System.out.println("f(x)   = " + expression);
        System.out.println("x      = " + xValue);
        System.out.println("result = " + result);
    }
}
