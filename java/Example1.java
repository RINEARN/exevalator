// import anypackage.in.which.you.put.Exevalator;

/**
 * A simple example to use Exevalator.
 */
public class Example1 {

    public static void main(String[] args) {

        // Create an instance of Exevalator Engine
        Exevalator exevalator = new Exevalator();

        // Evaluate the value of an expression
        double result = exevalator.eval("1.2 + 3.4");

        // Display the result
        System.out.println("result: " + result);
    }
}
