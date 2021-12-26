// import anypackage.in.which.you.put.Exevalator;

/**
 * Function available in expressions.
 */
class MyFunction implements Exevalator.FunctionInterface {

	/**
	 * Invoke the function.
	 * 
	 * @param arguments An array storing values of arguments.
	 * @return The return value of the function.
	 */
	@Override
	public double invoke(double[] arguments) {
		if (arguments.length != 2) {
			throw new Exevalator.Exception("Incorrected number of args");
		}
		return arguments[0] + arguments[1];
	}
}

/**
 * An example to create a function for available in expressions.
 */
public class Example5 {

	/**
	 * The entry point of this example code.
	 * @param args Command-line arguments (unused)
	 */
	public static void main(String[] args) {

		// Create an instance of Exevalator Engine
		Exevalator exevalator = new Exevalator();

		// Connects the function available for using it in expressions
		MyFunction fun = new MyFunction();
		exevalator.connectFunction("fun", fun);

		// Evaluate the value of an expression
		double result = exevalator.eval("fun(1.2, 3.4)");

		// Display the result
		System.out.println("Result: " + result);
	}
}
