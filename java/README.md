# How to Use Exevalator in Java&trade;

&raquo; [Japanese](./README_JAPANESE.md)


## English Index
- [Requirements](#requirements)
- [How to Use](#how-to-use)
- [How to Compile and Run Example Code](#example-code)
- [Features](#features)
- [List of Methods/Specifications](#methods)
	- [Constructor](#methods-constructor)
	- [double eval(String expression)](#methods-eval)
	- [double reeval()](#methods-reeval)
	- [int declareVariable(String name)](#methods-declare-variable)
	- [void writeVariable(String name, double value)](#methods-write-variable)
	- [void writeVariableAt(int address, double value)](#methods-write-variable-at)
	- [double readVariable(String name)](#methods-read-variable)
	- [double readVariableAt(int address)](#methods-read-variable-at)
	- [void connectFunction(String name, Exevalator.FunctionInterface function)](#methods-connect-function)
- [If You Want More Features: Try to Use Vnano](#vnano)



<a id="requirements"></a>
## Requirements

* Java Development Kit (JDK) 8 or later



<a id="how-to-use"></a>
## How to Use

Exevalator is an interpreter, compactly implemented in just a single file, "java/Exevalator.java." This unique design allows for easy integration in the following three steps:

### 1. Place the File in Your Source Code Folder

First, place "java/Exevalator.java" anywhere within your source code folder, for example:

	src/your/projects/package/anywhere/Exevalator.java

### 2. Add a Package Statement

Next, add a package statement at the top of "Exevalator.java" as shown in the following example:

	// in Exevalator.java
	package your.projects.package.anywhere;

### 3. Import and Use in Your Code

Now, you are ready to use Exevalator! Import it from any source file and compute expressions as follows:

	...
	import your.projects.package.anywhere.Exevalator;
	...

	public class YourClass {
		...
		public void yourMethod() {
			
			// Create an interpreter for evaluating expressions
			Exevalator exevalator = new Exevalator();

			// Evaluate the value of an expression
			double result = exevalator.eval("1.2 + 3.4");
			
			// Display the result
			System.out.println("result: " + result);
		}
		...
	}

In Exevalator, all numbers in expressions are handled as double-type values, and the return value is also double-type. However, computation may fail due to incorrect expressions, access to undeclared variables, etc. If a computation fails, the "eval" method will throw an Exevalator.Exception, which should be caught and handled appropriately in practical applications.



<a id="example-code"></a>
## How to Compile and Run Example Code

Simple example codes such as "java/Example*.java" are bundled in this repository. You can compile and run them as follows:

	cd java
	javac Exevalator.java
	javac Example1.java
	java Example1


"Example1.java" demonstrates how to compute the value "1.2 + 3.4" using Exevalator. Its code is similar to the "YourClass" example in the previous section:

	...
	double result = exevalator.eval("1.2 + 3.4");

The result will be displayed as:

	result: 4.6

As shown above, the computed value of "1.2 + 3.4" will be displayed. You can compile and run other examples in the same manner.

Additionally, a benchmark program "java/Benchmark.java" for measuring the processing speed of Exevalator is also included in this repository. You can compile and run it in the same way as the other example codes.


<a id="features"></a>
## Features

The following are the main features of Exevalator:

### 1. Evaluate (Compute) Expressions

As demonstrated in previous sections, Exevalator can evaluate the value of an expression:

	double result = exevalator.eval("(-(1.2 + 3.4) * 5) / 2");
	// result: -11.5

	(See: java/Example2.java)

You can use operators such as "+" (addition), "-" (subtraction and unary-minus operation), "*" (multiplication), and "/" (division) in expressions. Multiplications and divisions are prioritized over additions and subtractions in the order of operations.


### 2. Use Variables

You can declare variables programmatically using Exevalator's "declareVariable" method during application development, allowing these variables to be freely used within expressions:

	// Declare a variable to be used in expressions
	exevalator.declareVariable("x");
	exevalator.writeVariable("x", 1.25);

	// Evaluate the expression using the declared variable
	double result = exevalator.eval("x + 1");
	// result: 2.25

	(See: java/Example3.java)

For more frequent variable value updates, access the variable by address for faster performance:

	int address = exevalator.declareVariable("x");
	exevalator.writeVariableAt(address, 1.25);
	...

	(See: java/Example4.java)

Access by address is quicker than by name.


### 3. Use Functions

You can create functions by implementing the "Exevalator.FunctionInterface", allowing these functions to be used within expressions:

	// Create a function available in expressions
	class MyFunction implements Exevalator.FunctionInterface {
		@Override
		public double invoke (double[] arguments) {
			if (arguments.length != 2) {
				throw new Exevalator.Exception("Incorrected number of args");
			}
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

This setup ensures that developers can programmatically define and integrate custom functions, which users can then utilize seamlessly within their expressions.

**CAUTION: In Ver.1.0, values of arguments passed from expressions were stored in the "double[] arguments" array in reversed order. This behavior has been corrected in Ver.2.0. For details, please refer to issue #2.**


<a id="methods"></a>
## List of Methods/Specifications

Here is a list of methods for the Exevalator class, along with their specifications:

- [Constructor](#methods-constructor)
- [double eval(String expression)](#methods-eval)
- [double reeval()](#methods-reeval)
- [int declareVariable(String name)](#methods-declare-variable)
- [void writeVariable(String name, double value)](#methods-write-variable)
- [void writeVariableAt(int address, double value)](#methods-write-variable-at)
- [double readVariable(String name)](#methods-read-variable)
- [double readVariableAt(int address)](#methods-read-variable-at)
- [void connectFunction(String name, Exevalator.FunctionInterface function)](#methods-connect-function)


<a id="methods-constructor"></a>
| Signature | (constructor) Exevalator() |
|:---|:---|
| Description | Creates a new Exevalator interpreter instance. |
| Parameters | None |
| Return | The newly created instance. |


<a id="methods-eval"></a>
| Signature | double eval(String expression) |
|:---|:---|
| Description | Evaluates the value of a given expression. |
| Parameters | expression: The expression to be evaluated. |
| Return | The resulting value of the expression. |
| Exception | Exevalator.Exception is thrown if an error occurs during the evaluation. |


<a id="methods-reeval"></a>
| Signature | double reeval() |
|:---|:---|
| Description | Re-evaluates the value of the expression last evaluated by the "eval" method. <br>This method may work slightly faster than repeatedly calling "eval" for the same expression.<br> Note that the result may differ from the last evaluation if variables or function behaviors have changed. |
| Parameters | None |
| Return | The re-evaluated value. |
| Exception | Exevalator.Exception is thrown if an error occurs during the evaluation. |


<a id="methods-declare-variable"></a>
| Signature | int declareVariable(String name) |
|:---|:---|
| Description | Declares a new variable to be used in expressions. |
| Parameters | name: The name of the variable. |
| Return | 	The virtual address of the declared variable, facilitating faster access.<br>See the "writeVariableAt" and "readVariableAt" methods. |
| Exception | Exevalator.Exception will be thrown if invalid name is specified. |


<a id="methods-write-variable"></a>
| Signature | void writeVariable(String name, double value) |
|:---|:---|
| Description | Sets the specified variable to a new value. |
| Parameters | name: The name of the variable.<br>value: The new value to set. |
| Return | None |
| Exception | Exevalator.Exception is thrown if the variable does not exist or the name is invalid. |


<a id="methods-write-variable-at"></a>
| Signature | void writeVariableAt(int address, double value) |
|:---|:---|
| Description | Sets the value of a variable at the specified virtual address, which is faster than using "writeVariable." |
| Parameters | address: The virtual address of the variable.<br>value: The new value to set. |
| Return | None |
| Exception | Exevalator.Exception is thrown if the address is invalid. |


<a id="methods-read-variable"></a>
| Signature | double readVariable(String name) |
|:---|:---|
| Description | Retrieves the current value of the specified variable. |
| Parameters | name: The name of the variable. |
| Return | The current value of the variable. |
| Exception | Exevalator.Exception is thrown if the variable does not exist or the name is invalid. |


<a id="methods-read-variable-at"></a>
| Signature | double readVariableAt(int address) |
|:---|:---|
| Description | Retrieves the value of a variable at the specified virtual address, which is faster than using "readVariable." |
| Parameters | address: The virtual address of the variable. |
| Return | The current value of the variable. |
| Exception | Exevalator.Exception is thrown if the address is invalid. |


<a id="methods-connect-function"></a>
| Signature | void connectFunction(String name, Exevalator.FunctionInterface function) |
|:---|:---|
| Description | Connects a function to be used in expressions. |
| Parameters | name: The function name as used in expressions.<br>function: An instance of a class implementing the Exevalator.FunctionInterface, which must define the method "double invoke(double[] arguments)" to process the function. |
| Return | None |
| Exception | Exevalator.Exception is thrown if an invalid name is specified. |





<a id="vnano"></a>
## If You Want More Features: Consider Using Vnano

While Exevalator is designed to be compact, its feature set is accordingly limited.

If you require more advanced features, consider using Vnano, which is a scripting engine designed for embedding in Java applications. Vnano supports relatively complex expressions and scripts, including conditional branches, loops, and more.



<hr />

<a id="credits"></a>
## Credits

- Oracle and Java are registered trademarks of Oracle and/or its affiliates. 

- Other names may be either a registered trademarks or trademarks of their respective owners. 


