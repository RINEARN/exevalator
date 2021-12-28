# How to Use Exevalator in C#&reg;

&raquo; [Japanese](./README_JAPANESE.md)


## English Index
- <a href="#requirements">Requirements</a>
- <a href="#how-to-use">How to Use</a>
- <a href="#example-code">Example Code (and How to Use in Command Lines)</a>
- <a href="#features">Features</a>




<a id="requirements"></a>
## Requirements

* Microsoft&reg; Visual Studio&reg;, 2022 or later (recommended)
* Or, other C# development environment (for C# 8.0 or later, recommended)



<a id="how-to-use"></a>
## How to Use

The interpreter of Exevalator is implemented in a single file "csharp/Exevalator.cs", so you can import and use it in your project by very easy 2 steps!

### 1. Add "Exevalator.rs" to any project

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
	csc Exevalator.cs Example1.cs
	Example1.exe

"Example1.cs" is an example for computing the value of "1.2 + 3.4" by using Exevalator. Its code is almost the same as "YourClass" in the previous section:

	...
	double result = exevalator.Eval("1.2 + 3.4");


The result is:

	result: 4.6

As the above, the computed value of "1.2 + 3.4" will be displayed. You can compile/run other examples in the same way.

Also, a benchmark program "csharp/Benchmark.cs" for measuring processing speed of Exevalator is bundled in this repository. For compiling it, specify the optimization-option as:

	csc -optimize Exevalator.cs Benchmark.cs

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
		public double invoke(double[] arguments)
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



<a id="credits"></a>
## Credits

- Microsoft Windows, C#, and Visual Studio are either a registered trademarks or trademarks of Microsoft Corporation in the United States and/or other countries.

- Other names may be either a registered trademarks or trademarks of their respective owners. 


