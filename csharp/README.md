# How to Use Exevalator in C#

&raquo; [Japanese](./README_JAPANESE.md)


## English Index
- [Requirements](#requirements)
- [How to Use](#how-to-use)
- [Example Code (and How to Use in Command Lines)](#example-code)
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
* Or, other C# development environment (for C# 8.0 or later, recommended)



<a id="how-to-use"></a>
## How to Use

The interpreter of Exevalator is implemented in a single file "csharp/Exevalator.cs", so you can import and use it in your project by very easy 2 steps!

### 1. Add "Exevalator.cs" to any project

If you are using IDE of Visual Studio, add the file "csharp/Exevalator.cs" to your project, as follows:

At first, right-click your project on "Solution Exproler", and select "Add" > "Existing Item". Then a window will be popped-up for choosing a file, so choose "Exevalator.cs" exists in "csharp" folder of this repository. If you added it successfully, "Exevalator.cs" becomes visible on "Solution Exproler".

(If you want to compile/run in command lines, we will explain how to do it later.)


### 2. Load Exevalator from your code, and use it

Next, load Exevalator from your code as: "using Rinearn.ExevalatorCS;". Then you can use Exevalator in the code. The following is an example:

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

On Exevalator, all numbers in expressions will be handled as double-type values, so the result is always double-type.
However, the computation may fail when an incorrect expression is inputted, or when an undeclared variable is accessed, and so on. If a computation failed, "Eval" method throws an ExevalatorException, so catch and handle it if necessary.

<a id="example-code"></a>
## Example Code (and How to Use in Command-Lines)

Simple example code "csharp/Example*.cs" are bundled in this repository.

You can run these examples by adding them to projects on Visual Studio®, as we explained in the previous section.
On the other hand, you also can run them in command-lines. The latter is more simple way, so Let's do it here.

At first, launch "Developer Command Prompt" which is bundled in Visual Studio (Start button > Visual Studio 20** > ... ). Next, "cd" into the folder of this repository. Then compile example code and run it as follows:

	cd csharp
	csc Example1.cs Exevalator.cs
	Example1.exe

"Example1.cs" is an example for computing the value of "1.2 + 3.4" by using Exevalator. Its code is almost the same as "YourClass" in the previous section:

	...
	double result = exevalator.Eval("1.2 + 3.4");


The result is:

	result: 4.6

As the above, the computed value of "1.2 + 3.4" will be displayed. You can compile/run other examples in the same way.

Also, a benchmark program "csharp/Benchmark.cs" for measuring processing speed of Exevalator is bundled in this repository. For compiling it, specify the optimization-option as:

	csc -optimize Benchmark.cs Exevalator.cs

In the above, "-optimize" is the optimization-option. If you forget it, Exevalator can not work in full-speed.



<a id="features"></a>
## Features

The followings are main features of Exevalator.

### 1. Evaluate (Compute) Expressions

As shown in previous sections, Exevalator can evaluate (compute) the value of an expression:

	double result = exevalator.Eval("(-(1.2 + 3.4) * 5) / 2");
	// result: -11.5

	(See: csharp/Example2.cs)

As the above, you can use "+" (addition), "-" (subtraction and unary-minus operation), "\*" (multiplication), and "/" (division) operators in an expression. In order of operations, "multiplications and divisions" are prioritized than "additions and subtractions".


### 2. Use Variables

You can use variables in expressions:

	// Declare a variable available in expressions
	exevalator.DeclareVariable("x");
	exevalator.WriteVariable("x", 1.25);
	
	// Compute the expression in which the above variable is used
	double result = exevalator.Eval("x + 1");
	// result: 2.25

	(See: csharp/Example3.cs)

If you change the value of a variable very frequently, access to the variable by the address as follows:

	int address = exevalator.DeclareVariable("x");
	exevalator.WriteVariableAt(address, 1.25);
	...

	(See: csharp/Example4.cs)

This way works faster than accessings by the name.

### 3. Use Functions

You can create functions available in expressions, by implementing IExevalatorFunction interface:

	// Create a function available in expressions.
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

**CAUTION: In Ver.1.0, values of arguments passed from expressions had been stored in the above "double[] arguments" array in reversed order. This behavior has been fixed in Ver.2.0. For details, please see the issue #2.**



<a id="methods"></a>
## List of Methods/Specifications

The list of methods of Exevalator class, and their specifications.

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
| Description | Creates a new interpreter of the Exevalator. |
| Parameters | None |
| Return | The created instance. |


<a id="methods-eval"></a>
| Signature | double Eval(string expression) |
|:---|:---|
| Description | Evaluates (computes) the value of an expression. |
| Parameters | expression: The expression to be evaluated. |
| Return | The evaluated value. |
| Exception | ExevalatorException will be thrown if any error occurred when evaluating the expression. |


<a id="methods-reeval"></a>
| Signature | double Reeval() |
|:---|:---|
| Description | Re-evaluates (re-computes) the value of the expression evaluated by "eval" method last time.<br>This method may (slightly) work faster than calling "eval" method repeatedly for the same expression.<br>Note that, the result value may different with the last evaluated value, if values of variables or behaviour of functions had changed. |
| Parameters | None |
| Return | The evaluated value. |
| Exception | ExevalatorException will be thrown if any error occurred when evaluating the expression. |


<a id="methods-declare-variable"></a>
| Signature | int DeclareVariable(string name) |
|:---|:---|
| Description | Declares a new variable, for using the value of it in expressions. |
| Parameters | name: The name of the variable to be declared. |
| Return | The virtual address of the declared variable, which is useful for accessing to the variable faster.<br>See "WriteVariableAt" and "ReadVariableAt" method. |
| Exception | ExevalatorException will be thrown if invalid name is specified. |


<a id="methods-write-variable"></a>
| Signature | void WriteVariable(string name, double value) |
|:---|:---|
| Description | Writes the value to the variable having the specified name. |
| Parameters | name: The name of the variable to be written.<br>value: The new value of the variable. |
| Return | None |
| Exception | ExevalatorException will be thrown if the specified variable is not found, or invalid name is specified. |


<a id="methods-write-variable-at"></a>
| Signature | void WriteVariableAt(int address, double value) |
|:---|:---|
| Description | Writes the value to the variable at the specified virtual address.<br>This method works faster than "WriteVariable" method. |
| Parameters | address: The virtual address of the variable to be written.<br>value: The new value of the variable. |
| Return | None |
| Exception | ExevalatorException will be thrown if the invalid address is specified. |


<a id="methods-read-variable"></a>
| Signature | double ReadVariable(string name) |
|:---|:---|
| Description | Reads the value of the variable having the specified name. |
| Parameters | name: The name of the variable to be read. |
| Return | The current value of the variable. |
| Exception | ExevalatorException will be thrown if the specified variable is not found, or invalid name is specified. |


<a id="methods-read-variable-at"></a>
| Signature | double ReadVariableAt(int address) |
|:---|:---|
| Description | Reads the value of the variable at the specified virtual address.<br>This method works faster than "ReadVariable" method. |
| Parameters | address: The virtual address of the variable to be read. |
| Return | The current value of the variable. |
| Exception | ExevalatorException will be thrown if the invalid address is specified. |


<a id="methods-connect-function"></a>
| Signature | void ConnectFunction(string name, IExevalatorFunction function) |
|:---|:---|
| Description | Connects a function, for using it in expressions. |
| Parameters | name: The name of the function used in the expression.<br>function: The function to be connected. It is an instance of the class implementing IExevalatorFunction (only "double Invoke(double[] arguments)" method is defined, to implement the process of a function). |
| Return | None |
| Exception | ExevalatorException will be thrown if invalid name is specified. |


<hr />

<a id="credits"></a>
## Credits

- Microsoft Windows, C#, and Visual Studio are either a registered trademarks or trademarks of Microsoft Corporation in the United States and/or other countries.

- Other names may be either a registered trademarks or trademarks of their respective owners. 


