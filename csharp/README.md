# How to Use Exevalator in C#

&raquo; [Japanese](./README_JAPANESE.md)

&raquo; [Ask the AI for help (ChatGPT account required)](https://chatgpt.com/g/g-UjkEJeO8x-exevalator-assistant)

## Table of Contents
- [Requirements](#requirements)
- [How to Use](#how-to-use)
- [Example Code (and How to Use in Command-Lines)](#example-code)
- [Features](#features)
- [List of Methods/Specifications](#methods)
	- [Constructor](#methods-constructor)
	- [double Eval(string expression)](#methods-eval)
	- [double Reeval()](#methods-reeval)
	- [int DeclareVariable(string name)](#methods-declare-variable)
	- [void WriteVariable(string name, double value)](#methods-write-variable)
	- [void WriteVariableAt(int address, double value)](#methods-write-variable-at)
	- [double ReadVariable(string name)](#methods-read-variable)
	- [double ReadVariableAt(int address)](#methods-read-variable-at)
	- [void ConnectFunction(string name, IExevalatorFunction function)](#methods-connect-function)



<a id="requirements"></a>
## Requirements

* Microsoft&reg; Visual Studio&reg;, 2022 or later (recommended)
* Or, any other C# development environment supporting C# 8.0 or later



<a id="how-to-use"></a>
## How to Use

Exevalator is an interpreter, compactly implemented in just a single file, "csharp/Exevalator.cs." This streamlined design allows for easy integration in the following two steps:


### 1. Add "Exevalator.cs" to Any Project

If you are using Visual Studio, simply right-click on your project in the "Solution Explorer", select "Add" > "Existing Item", then navigate to and select "Exevalator.cs" from the "csharp" folder of this repository. Once added, "Exevalator.cs" should appear in your "Solution Explorer".

(Instructions for compiling and running from the command line will be provided later.)


### 2. Load Exevalator from Your Code, and Use It

In your code, include Exevalator with the following statement: "using Rinearn.ExevalatorCS;". Here’s how you can use it within your application:

	...
	using Rinearn.ExevalatorCS;
	...

	class YourClass
	{
		...
		void YourMethod() 
		{	
			// Create an interpreter for evaluating expressions
			Exevalator exevalator = new Exevalator();

			// Evaluate (compute) the value of an expression
			double result = exevalator.Eval("1.2 + 3.4");
			
			// Display the result
			Console.WriteLine("result: " + result);
		}
		...
	}

In Exevalator, all numbers in expressions are handled as double-type values, and the return value is also double-type. However, computations may fail due to incorrect expressions, access to undeclared variables, etc. If an error occurs, the "Eval" method throws an ExevalatorException, which should be caught and handled appropriately in practical applications.


<a id="example-code"></a>
## Example Code (and How to Use in Command-Lines)

Simple example codes such as "csharp/Example*.cs" are included in this repository.

You can run these examples in Visual Studio as described previously, or you can execute them from the command line, which is often simpler. Here’s how:

First, launch the "Developer Command Prompt" bundled with Visual Studio (Start button > Visual Studio 20** > ... ). Then navigate to the repository folder and compile and run the example code as follows:

	cd csharp
	csc Example1.cs Exevalator.cs
	Example1.exe

"Example1.cs" computes the value "1.2 + 3.4" using Exevalator, similar to "YourClass" in the previous section:

	...
	double result = exevalator.Eval("1.2 + 3.4");

The result will be:

	result: 4.6

This shows the computed value of "1.2 + 3.4". You can compile and run other examples in the same manner.

Additionally, a benchmark program "csharp/Benchmark.cs" for measuring the processing speed of Exevalator is also included. When compiling it, be sure to use the optimization option:

	csc -optimize Benchmark.cs Exevalator.cs

In the above command, "-optimize" is essential for achieving full speed. Omitting this option may result in suboptimal performance.



<a id="features"></a>
## Features

The following are the main features of Exevalator:

### 1. Evaluate (Compute) Expressions

As demonstrated in previous sections, Exevalator can evaluate the value of an expression:

	double result = exevalator.Eval("(-(1.2 + 3.4) * 5) / 2");
	// result: -11.5

	(See: csharp/Example2.cs)

You can use operators such as "+" (addition), "-" (subtraction and unary-minus operation), "\*" (multiplication), and "/" (division) in expressions. Multiplications and divisions are prioritized over additions and subtractions in the order of operations.


### 2. Use Variables

You can declare variables programmatically using Exevalator's "declareVariable" method during application development, allowing these variables to be freely used within expressions:

	// Declare a variable to be used in expressions
	exevalator.DeclareVariable("x");
	exevalator.WriteVariable("x", 1.25);
	
	// Evaluate the expression using the declared variable
	double result = exevalator.Eval("x + 1");
	// result: 2.25

	(See: csharp/Example3.cs)

For more frequent variable value updates, access the variable by address for faster performance:

	int address = exevalator.DeclareVariable("x");
	exevalator.WriteVariableAt(address, 1.25);
	...

	(See: csharp/Example4.cs)

Access by address is quicker than by name.



### 3. Use Functions

You can create functions by implementing the "IExevalatorFunction" interface, allowing these functions to be used within expressions:

	// Create a function available in expressions
	class MyFunction : IExevalatorFunction
	{
		public double Invoke(double[] arguments)
		{
			if (arguments.Length != 2)
			{
				throw new ExevalatorException("Incorrected number of args");
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

	(See: csharp/Example5.cs)

This setup ensures that developers can programmatically define and integrate custom functions, which users can then utilize seamlessly within their expressions.

**CAUTION: In Ver.1.0, values of arguments passed from expressions were stored in the "double[] arguments" array in reversed order. This behavior has been corrected in Ver.2.0. For details, please refer to issue #2.**


<a id="methods"></a>
## List of Methods/Specifications

Here is a list of methods for the Exevalator class, along with their specifications:

- [Constructor](#methods-constructor)
- [double Eval(string expression)](#methods-eval)
- [double Reeval()](#methods-reeval)
- [int DeclareVariable(string name)](#methods-declare-variable)
- [void WriteVariable(string name, double value)](#methods-write-variable)
- [void WriteVariableAt(int address, double value)](#methods-write-variable-at)
- [double ReadVariable(string name)](#methods-read-variable)
- [double ReadVariableAt(int address)](#methods-read-variable-at)
- [void ConnectFunction(string name, IExevalatorFunction function)](#methods-connect-function)


<a id="methods-constructor"></a>
| Signature | (constructor) Exevalator() |
|:---|:---|
| Description | Creates a new Exevalator interpreter instance. |
| Parameters | None |
| Return | The newly created instance. |


<a id="methods-eval"></a>
| Signature | double Eval(string expression) |
|:---|:---|
| Description | Evaluates the value of a given expression. |
| Parameters | expression: The expression to be evaluated. |
| Return | The resulting value of the expression. |
| Exception | ExevalatorException is thrown if an error occurs during the evaluation. |


<a id="methods-reeval"></a>
| Signature | double Reeval() |
|:---|:---|
| Description | Re-evaluates the value of the expression last evaluated by the "Eval" method. <br>This method may work slightly faster than repeatedly calling "eval" for the same expression.<br> Note that the result may differ from the last evaluation if variables or function behaviors have changed. |
| Parameters | None |
| Return | The re-evaluated value. |
| Exception | ExevalatorException is thrown if an error occurs during the evaluation. |


<a id="methods-declare-variable"></a>
| Signature | int DeclareVariable(string name) |
|:---|:---|
| Description | Declares a new variable to be used in expressions. |
| Parameters | name: The name of the variable. |
| Return | The virtual address of the declared variable, facilitating faster access.<br>See the "WriteVariableAt" and "ReadVariableAt" methods. |
| Exception | ExevalatorException will be thrown if invalid name is specified. |


<a id="methods-write-variable"></a>
| Signature | void WriteVariable(string name, double value) |
|:---|:---|
| Description | Sets the specified variable to a new value. |
| Parameters | name: The name of the variable.<br>value: The new value to set. |
| Return | None |
| Exception | ExevalatorException is thrown if the variable does not exist or the name is invalid. |


<a id="methods-write-variable-at"></a>
| Signature | void WriteVariableAt(int address, double value) |
|:---|:---|
| Description | Sets the value of a variable at the specified virtual address, which is faster than using "WriteVariable." |
| Parameters | address: The virtual address of the variable.<br>value: The new value to set. |
| Return | None |
| Exception | ExevalatorException is thrown if the address is invalid. |


<a id="methods-read-variable"></a>
| Signature | double ReadVariable(string name) |
|:---|:---|
| Description | Retrieves the current value of the specified variable. |
| Parameters | name: The name of the variable. |
| Return | The current value of the variable. |
| Exception | ExevalatorException is thrown if the variable does not exist or the name is invalid. |


<a id="methods-read-variable-at"></a>
| Signature | double ReadVariableAt(int address) |
|:---|:---|
| Description | Retrieves the value of a variable at the specified virtual address, which is faster than using "ReadVariable." |
| Parameters | address: The virtual address of the variable. |
| Return | The current value of the variable. |
| Exception | Exevalator.Exception is thrown if the address is invalid. |


<a id="methods-connect-function"></a>
| Signature | void ConnectFunction(string name, IExevalatorFunction function) |
|:---|:---|
| Description | Connects a function to be used in expressions. |
| Parameters | name: The function name as used in expressions.<br>function: An instance of a class implementing the IExevalatorFunction interface, which must define the method "double invoke(double[] arguments)" to process the function. |
| Return | None |
| Exception | ExevalatorException is thrown if an invalid name is specified. |


<hr />

<a id="credits"></a>
## Credits

- Microsoft Windows, C#, and Visual Studio are either a registered trademarks or trademarks of Microsoft Corporation in the United States and/or other countries.

- ChatGPT is a trademark or a registered trademark of OpenAI OpCo, LLC in the United States and other countries.

- Other names may be either a registered trademarks or trademarks of their respective owners. 


