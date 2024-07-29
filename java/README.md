# How to Use Exevalator in Java&trade;

&raquo; [Japanese](./README_JAPANESE.md)


## English Index
- [Requirements](#requirements)
- [How to Use](#how-to-use)
- [Example Code](#example-code)
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
			System.out.println("result: " + result);
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

	result: 4.6

As the above, the computed value of "1.2 + 3.4" will be displayed. You can compile/run other examples in the same way.

Also, a benchmark program "java/Benchmark.java" for measuring processing speed of Exevalator is bundled in this repository. You can compile/run it in the same way as other example code.


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

**CAUTION: In Ver.1.0, values of arguments passed from expressions had been stored in the above "double[] arguments" array in reversed order. This behavior has been fixed in Ver.2.0. For details, please see the issue #2.**


<a id="methods"></a>
## List of Methods/Specifications

The list of methods of Exevalator class, and their specifications.

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
| Description | Creates a new interpreter of the Exevalator. |
| Parameters | None |
| Return | The created instance. |


<a id="methods-eval"></a>
| Signature | double eval(String expression) |
|:---|:---|
| Description | Evaluates (computes) the value of an expression. |
| Parameters | expression: The expression to be evaluated. |
| Return | The evaluated value. |
| Exception | Exevalator.Exception will be thrown if any error occurred when evaluating the expression. |


<a id="methods-reeval"></a>
| Signature | double reeval() |
|:---|:---|
| Description | Re-evaluates (re-computes) the value of the expression evaluated by "eval" method last time.<br>This method may (slightly) work faster than calling "eval" method repeatedly for the same expression.<br>Note that, the result value may different with the last evaluated value, if values of variables or behaviour of functions had changed. |
| Parameters | None |
| Return | The evaluated value. |
| Exception | Exevalator.Exception will be thrown if any error occurred when evaluating the expression. |


<a id="methods-declare-variable"></a>
| Signature | int declareVariable(String name) |
|:---|:---|
| Description | Declares a new variable, for using the value of it in expressions. |
| Parameters | name: The name of the variable to be declared. |
| Return | The virtual address of the declared variable, which is useful for accessing to the variable faster.<br>See "writeVariableAt" and "readVariableAt" method. |
| Exception | Exevalator.Exception will be thrown if invalid name is specified. |


<a id="methods-write-variable"></a>
| Signature | void writeVariable(String name, double value) |
|:---|:---|
| Description | Writes the value to the variable having the specified name. |
| Parameters | name: The name of the variable to be written.<br>value: The new value of the variable. |
| Return | None |
| Exception | Exevalator.Exception will be thrown if the specified variable is not found, or invalid name is specified. |


<a id="methods-write-variable-at"></a>
| Signature | void writeVariableAt(int address, double value) |
|:---|:---|
| Description | Writes the value to the variable at the specified virtual address.<br>This method works faster than "writeVariable" method. |
| Parameters | address: The virtual address of the variable to be written.<br>value: The new value of the variable. |
| Return | None |
| Exception | Exevalator.Exception will be thrown if the invalid address is specified. |


<a id="methods-read-variable"></a>
| Signature | double readVariable(String name) |
|:---|:---|
| Description | Reads the value of the variable having the specified name. |
| Parameters | name: The name of the variable to be read. |
| Return | The current value of the variable. |
| Exception | Exevalator.Exception will be thrown if the specified variable is not found, or invalid name is specified. |


<a id="methods-read-variable-at"></a>
| Signature | double readVariableAt(int address) |
|:---|:---|
| Description | Reads the value of the variable at the specified virtual address.<br>This method works faster than "readVariable" method. |
| Parameters | address: The virtual address of the variable to be read. |
| Return | The current value of the variable. |
| Exception | Exevalator.Exception will be thrown if the invalid address is specified. |


<a id="methods-connect-function"></a>
| Signature | void connectFunction(String name, Exevalator.FunctionInterface function) |
|:---|:---|
| Description | Connects a function, for using it in expressions. |
| Parameters | name: The name of the function used in the expression.<br>function: The function to be connected. It is an instance of the class implementing Exevalator.FunctionInterface (only "double invoke(double[] arguments)" method is defined, to implement the process of a function). |
| Return | None |
| Exception | Exevalator.Exception will be thrown if invalid name is specified. |





<a id="vnano"></a>
## If You Want More Features: Try to Use Vnano

Features of Exevalator is not so rich, because we prioritize to make the interpreter compact.

If you want more features, try to use the [Vnano](https://github.com/RINEARN/vnano) which is an script engine for embedded use in Java&reg; applications, instead of Exevalator.
Vnano can execute relatively complex expressions/scripts, containing conditional branches, loops, and so on.


<hr />

<a id="credits"></a>
## Credits

- Oracle and Java are registered trademarks of Oracle and/or its affiliates. 

- Other names may be either a registered trademarks or trademarks of their respective owners. 


