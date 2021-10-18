// import anypackage.in.which.you.put.Exevalator;

/**
 * A simple example to use Exevalator.
 */
public class Example1 {

	/**
	 * The entry point of this example code.
	 * @param args Command-line arguments (unused)
	 */
	public static void main(String[] args) {

		// Create an instance of Exevalator Engine
		Exevalator exevalator = new Exevalator();

		// Evaluate the value of an expression
		double result = exevalator.eval("1.2 + 3.4");

		// Display the result
		System.out.println("Result: " + result);
	}
}
