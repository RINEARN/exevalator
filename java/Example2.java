// import anypackage.in.which.you.put.Exevalator;

/**
 * An example to use various operators and parentheses.
 */
public class Example2 {

    public static void main(String[] args) {

        // Create an instance of Exevalator Engine
        Exevalator exevalator = new Exevalator();

        // Evaluate the value of an expression
        double result = exevalator.eval("(-(1.2 + 3.4) * 5) / 2");

        // Display the result
        System.out.println("result: " + result);
    }
}
