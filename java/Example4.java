// import anypackage.in.which.you.put.Exevalator;

/**
 * An example to access a variable by its address.
 */
public class Example4 {

    public static void main(String[] args) {

        // Create an instance of Exevalator Engine
        Exevalator exevalator = new Exevalator();

        // Declare a variable and set the value
        int address = exevalator.declareVariable("x");
        exevalator.writeVariableAt(address, 1.25);
        // The above works faster than:
        //    exevalator.writeVariable("x", 1.25);

        // Evaluate the value of an expression
        double result = exevalator.eval("x + 1");

        // Display the result
        System.out.println("result: " + result);
    }
}
