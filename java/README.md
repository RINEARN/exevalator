# How to Use Exevalator in Java&reg;

&raquo; [Japanese](./README_JAPANESE.md)


## English Index
- <a href="#requirements">Requirements</a>
- <a href="#how-to-use">How to Use</a>
- <a href="#example-code">Example Code</a>
- <a href="#features">Features</a>
- <a href="#vnano">If You Want More Features: Try to Use Vnano</a>



<a id="requirements"></a>
## Requirements

* Java&reg; Development Kit (JDK) 8 or later



<a id="how-to-use"></a>
## How to Use

The interpreter of Exevalator is implemented in a single file "java/Exevalator.java", so you can import and use it in your project by very easy 3 steps!

### 1. Put into your source code folder

Firstly, put "java/Exevalator.java" into anywere in your source code folder, e.g.:

	src/your/projects/package/anywhere/Exevalator.java

### 2. Write the package statement

Secondly, the package statement at the top of the content of "Exevalator.java" as the following example:

	(in Exevalator.java)
	package your.projects.package.anywhere;

### 3. Import from your code and use

Now you are ready to use Exevalator! You can import the Exevalator from any your source, code and compute expressions as follows:

	...
	import your.projects.package.anywhere.Exevalator;
	...

	public class YourClass {
		...
		public void yourMethod() {
			
			// Create an interpreter for evaluating expressions
			Exevalator exevalator = new Exevalator();

			// Evaluate (compute) the value of an expression
			double result = exevalator.eval("1.2 + 3.4");
			
			// Display the result
			System.out.println("Result: " + result);
		}
		...
	}

On Exevalator, all numbers in expressions will be handled as double-type values, so the result is always double-type.
However, the computation may fail when an incorrect expression is inputted, or when an undeclared variable is accessed, and so on. If a computation failed, "eval" method throws an Exevalator.Exception, so catch and handle it if necessary.


<a id="example-code"></a>
## Example Code

Simple example code "java/Example*.java" are bundled in this repository. You can compile and run them as follows:

	cd java
	javac Exevalator.java
	javac Example1.java
	java Example1

"Example1.java" is an example for computing the value of "1.2 + 3.4" by using Exevalator. Its code is almost the same as "YourClass" in the previous section:

	...
	double result = exevalator.eval("1.2 + 3.4");


The result is:

	4.6

As the above, the computed value of "1.2 + 3.4" will be displayed.


<a id="features"></a>
## Features

The followings are main features of Exevalator.

### 1. Evaluate (Compute) Expressions

As shown in previous sections, Exevalator can evaluate (compute) the value of an expression:

	double result = exevalator.eval("(-(1.2 + 3.4) * 5) / 2");
	// result: -11.5

	(See: java/Example2.java)

As the above, you can use "+" (addition), "-" (subtraction and unary-minus operation), "\*" (multiplication), and "/" (division) operators in an expression. In order of operations, "multiplications and divisions" are prioritized than "additions and subtractions".


### 2. Use Variables

You can use variables in expressions:

	// Declare a variable available in expressions
	exevalator.declareVariable("x");
	exevalator.writeVariable("x", 1.25);

	// Compute the expression in which the above variable is used
	double result = exevalator.eval("x + 1");
	// result: 2.25

	(See: java/Example3.java)

If you change the value of a variable very frequently, access to the variable by the address as follows:

	int address = exevalator.declareVariable("x");
	exevalator.writeVariableAt(address, 1.25);
	...

	(See: java/Example4.java)

This way works faster than accessings by the name.

### 3. Use Functions

You can create functions available in expressions, by implementing Exevalator.FunctionInterface:

	// Create a function available in expressions.
	class MyFunction implements Exevalator.FunctionInterface {
		@Override
		public double invoke (double[] arguments) {
			return arguments[0] + arguments[1];
		}
	}
	...

	// Connect the above function for using it in expressions
	MyFunction fun = new MyFunction();
	exevalator.connectFunction("fun", fun);

	// Compute the expression in which the above function is used
	double result = exevalator.eval("fun(1.2, 3.4)");
	// result: 4.6

	(See: java/Example5.java)


<a id="vnano"></a>
## If You Want More Features: Try to Use Vnano

Features of Exevalator is not so rich, because we prioritize to make the interpreter compact.

If you want more features, try to use the [Vnano](https://github.com/RINEARN/vnano) which is an script engine for embedded use in Java&reg; applications, instead of Exevalator.
Vnano can execute relatively complex expressions/scripts, containing conditional branches, loops, and so on.



<a id="credits"></a>
## Credits

- Oracle and Java are registered trademarks of Oracle and/or its affiliates. 

- Other names may be either a registered trademarks or trademarks of their respective owners. 


