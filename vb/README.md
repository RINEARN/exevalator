# How to Use Exevalator in Visual Basic&reg; (VB.NET)

&raquo; [Japanese](./README_JAPANESE.md)


## English Index
- [Requirements](#requirements)
- [How to Use](#how-to-use)
- [Example Code (and How to Use in Command Lines)](#example-code)
- [Features](#features)
- [List of Methods/Specifications](#methods)
	- [Constructor](#methods-constructor)
	- [Eval(expression As String) As Double](#methods-eval)
	- [Reeval() As Double](#methods-reeval)
	- [DeclareVariable(name As String) As Integer](#methods-declare-variable)
	- [WriteVariable(name As String, value As Double)](#methods-write-variable)
	- [WriteVariableAt(address As Integer, value As Double)](#methods-write-variable-at)
	- [ReadVariable(name As String) As Double](#methods-read-variable)
	- [ReadVariableAt(address As Integer) As Double](#methods-read-variable-at)
	- [ConnectFunction(name As String, function As IExevalatorFunction)](#methods-connect-function)


<a id="requirements"></a>
## Requirements

* Microsoft&reg; Visual Studio&reg;, 2022 or later (recommended)
* Or, other Visual Basic .NET development environment (for .NET 6.0 later, recommended)



<a id="how-to-use"></a>
## How to Use

The interpreter of Exevalator is implemented in a single file "vb/Exevalator.vb", so you can import and use it in your project by very easy 3 steps!

### 1. Add "Exevalator.vb" to any project

If you are using IDE of Visual Studio, add the file "vb/Exevalator.vb" to your project, as follows:

At first, right-click your project on "Solution Exproler", and select "Add" > "Existing Item". Then a window will be popped-up for choosing a file, so choose "Exevalator.vb" exists in "csharp" folder of this repository. If you added it successfully, "Exevalator.vb" becomes visible on "Solution Exproler".

(If you want to compile/run in command lines, we will explain how to do it later.)


### 2. Check the name of the project

In a VB.NET project, by default, the project name is implicitly added to the top hierarchy of the project's namespaces. If you overlook this, you will be troubled by errors when you refer a library added to the project.

Here, for simplicity, let assume that the project name in which you added Exevalator.vb is:

	YourProject

Please check the actual project name here, and in the next step, replace "YourProject" to the actual name.

### 3. Load Exevalator from your code, and use it

Next, load Exevalator from your code by "Imports" as follows. Then you can use Exevalator in the code:

	...
	' Please replace "YourProject" in the following to the actual project name.
	Imports YourProject.Rinearn.ExevalatorVB
	...

	Module YourModule
		...
		Sub YourProcess() 
			' Create an interpreter for evaluating expressions
			Dim exevalator As Exevalator = New Exevalator()

			' Evaluate (compute) the value of an expression
			Dim result As Double = exevalator.Eval("1.2 + 3.4")
	
			' Display the result
			Console.WriteLine("result: " + result.ToString())
		End Sub
		...
	End Module

On Exevalator, all numbers in expressions will be handled as Double-type values, so the result is always Double-type.
However, the computation may fail when an incorrect expression is inputted, or when an undeclared variable is accessed, and so on. If a computation failed, "Eval" method throws an ExevalatorException, so catch and handle it if necessary.

<a id="example-code"></a>
## Example Code (and How to Use in Command-Lines)

Simple example code "vb/Example*.vb" are bundled in this repository.

You can run these examples by adding them to projects on Visual Studio®, as we explained in the previous section.
On the other hand, you also can run them in command-lines. The latter is more simple way, so Let's do it here.

At first, launch "Developer Command Prompt" which is bundled in Visual Studio (Start button > Visual Studio 20** > ... ). Next, "cd" into the folder of this repository. Then compile example code and run it as follows:

	cd csharp
	vbc Example1.vb Exevalator.vb
	Example1.exe

"Example1.vb" is an example for computing the value of "1.2 + 3.4" by using Exevalator. Its code is almost the same as "YourClass" in the previous section:

	...
	Dim result As Double = exevalator.Eval("1.2 + 3.4")

The result is:

	result: 4.6

As the above, the computed value of "1.2 + 3.4" will be displayed. You can compile/run other examples in the same way.

Also, a benchmark program "vb/Benchmark.vb" for measuring processing speed of Exevalator is bundled in this repository. For compiling it, specify the optimization-option as:

	csc -optimize Benchmark.vb Exevalator.vb

In the above, "-optimize" is the optimization-option. If you forget it, Exevalator can not work in full-speed.



<a id="features"></a>
## Features

The followings are main features of Exevalator.

### 1. Evaluate (Compute) Expressions

As shown in previous sections, Exevalator can evaluate (compute) the value of an expression:

	Dim result As Double = exevalator.Eval("(-(1.2 + 3.4) * 5) / 2")
	' result: -11.5

	(See: vb/Example2.vb)

As the above, you can use "+" (addition), "-" (subtraction and unary-minus operation), "\*" (multiplication), and "/" (division) operators in an expression. In order of operations, "multiplications and divisions" are prioritized than "additions and subtractions".


### 2. Use Variables

You can use variables in expressions:

	' Declare a variable available in expressions
	exevalator.DeclareVariable("x")
	exevalator.WriteVariable("x", 1.25)
	
	' Compute the expression in which the above variable is used
	Dim result As Double = exevalator.Eval("x + 1")
	' result: 2.25

	(See: vb/Example3.vb)

If you change the value of a variable very frequently, access to the variable by the address as follows:

	Dim address As Integer = exevalator.DeclareVariable("x")
	exevalator.WriteVariableAt(address, 1.25)
	...

	(See: vb/Example4.vb)

This way works faster than accessings by the name.

### 3. Use Functions

You can create functions available in expressions, by implementing IExevalatorFunction interface:

	' Create a function available in expressions.
	Class MyFunction : Implements IExevalatorFunction
	{
		Public Function Invoke(arguments() As Double) As Double
			If arguments.Length <> 2 Then
				Throw New ExevalatorException("Incorrected number of args")
			End If
			Return arguments(0) + arguments(1)
		End Function
	End Class
	...

	' Connect the above function for using it in expressions
	Dim fun As MyFunction = New MyFunction()
	exevalator.connectFunction("fun", fun)

	' Compute the expression in which the above function is used
	Dim result As Double = exevalator.eval("fun(1.2, 3.4)")
	' result: 4.6

	(See: vb/Example5.vb)




<a id="methods"></a>
## List of Methods/Specifications

The list of methods of Exevalator class, and their specifications.

- [Constructor](#methods-constructor)
- [Eval(expression As String) As Double](#methods-eval)
- [Reeval() As Double](#methods-reeval)
- [DeclareVariable(name As String) As Integer](#methods-declare-variable)
- [WriteVariable(name As String, value As Double)](#methods-write-variable)
- [WriteVariableAt(address As Integer, value As Double)](#methods-write-variable-at)
- [ReadVariable(name As String) As Double](#methods-read-variable)
- [ReadVariableAt(address As Integer) As Double](#methods-read-variable-at)
- [ConnectFunction(name As String, function As IExevalatorFunction)](#methods-connect-function)





<a id="methods-constructor"></a>
| Signature | (constructor) New() |
|:---|:---|
| Description | Creates a new interpreter of the Exevalator. |
| Parameters | None |
| Return | The created instance. |


<a id="methods-eval"></a>
| Signature | Eval(expression As String) As Double |
|:---|:---|
| Description | Evaluates (computes) the value of an expression. |
| Parameters | expression: The expression to be evaluated. |
| Return | The evaluated value. |
| Exception | ExevalatorException will be thrown if any error occurred when evaluating the expression. |


<a id="methods-reeval"></a>
| Signature | Reeval() As Double |
|:---|:---|
| Description | Re-evaluates (re-computes) the value of the expression evaluated by "eval" method last time.<br>This method may (slightly) work faster than calling "eval" method repeatedly for the same expression.<br>Note that, the result value may different with the last evaluated value, if values of variables or behaviour of functions had changed. |
| Parameters | None |
| Return | The evaluated value. |
| Exception | ExevalatorException will be thrown if any error occurred when evaluating the expression. |


<a id="methods-declare-variable"></a>
| Signature | DeclareVariable(name As String) As Integer |
|:---|:---|
| Description | Declares a new variable, for using the value of it in expressions. |
| Parameters | name: The name of the variable to be declared. |
| Return | The virtual address of the declared variable, which is useful for accessing to the variable faster.<br>See "WriteVariableAt" and "ReadVariableAt" method. |
| Exception | ExevalatorException will be thrown if invalid name is specified. |


<a id="methods-write-variable"></a>
| Signature | WriteVariable(name As String, value As Double) |
|:---|:---|
| Description | Writes the value to the variable having the specified name. |
| Parameters | name: The name of the variable to be written.<br>value: The new value of the variable. |
| Return | None |
| Exception | ExevalatorException will be thrown if the specified variable is not found, or invalid name is specified. |


<a id="methods-write-variable-at"></a>
| Signature | WriteVariableAt(address As Integer, value As Double) |
|:---|:---|
| Description | Writes the value to the variable at the specified virtual address.<br>This method works faster than "WriteVariable" method. |
| Parameters | address: The virtual address of the variable to be written.<br>value: The new value of the variable. |
| Return | None |
| Exception | ExevalatorException will be thrown if the invalid address is specified. |


<a id="methods-read-variable"></a>
| Signature | ReadVariable(name As String) As Double |
|:---|:---|
| Description | Reads the value of the variable having the specified name. |
| Parameters | name: The name of the variable to be read. |
| Return | The current value of the variable. |
| Exception | ExevalatorException will be thrown if the specified variable is not found, or invalid name is specified. |


<a id="methods-read-variable-at"></a>
| Signature | ReadVariableAt(address As Integer) As Double |
|:---|:---|
| Description | Reads the value of the variable at the specified virtual address.<br>This method works faster than "ReadVariable" method. |
| Parameters | address: The virtual address of the variable to be read. |
| Return | The current value of the variable. |
| Exception | ExevalatorException will be thrown if the invalid address is specified. |


<a id="methods-connect-function"></a>
| Signature | ConnectFunction(name As String, function As IExevalatorFunction) |
|:---|:---|
| Description | Connects a function, for using it in expressions. |
| Parameters | name: The name of the function used in the expression.<br>function: The function to be connected. It is an instance of the class implementing IExevalatorFunction (only "Invoke(arguments() As Double) As Double" method is defined, to implement the process of a function). |
| Return | None |
| Exception | ExevalatorException will be thrown if invalid name is specified. |


<hr />

<a id="credits"></a>
## Credits

- Microsoft Windows, Visual Basic, .NET, and Visual Studio are either a registered trademarks or trademarks of Microsoft Corporation in the United States and/or other countries.

- Other names may be either a registered trademarks or trademarks of their respective owners. 


