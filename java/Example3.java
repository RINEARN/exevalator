// import anypackage.in.which.you.put.Exevalator;

/**
 * An example to use a variable.
 */
public class Example3 {

	/**
	 * The entry point of this example code.
	 * @param args Command-line arguments (unused)
	 */
	public static void main(String[] args) {

		// Create an instance of Exevalator Engine
		Exevalator exevalator = new Exevalator();

		// Declare a variable and set the value
		exevalator.declareVariable("x");
		exevalator.writeVariable("x", 1.25);

		// Evaluate the value of an expression
		double result = exevalator.eval("x + 1");

		// Display the result
		System.out.println("Result: " + result);
	}
}
