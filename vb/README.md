# How to Use Exevalator in Visual Basic&reg; (VB.NET)

&raquo; [Japanese](./README_JAPANESE.md)


## Table of Contents
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
* Or, any other Visual Basic .NET development environment (for .NET 6.0 or later, recommended)



<a id="how-to-use"></a>
## How to Use

Exevalator is an interpreter, compactly implemented in just a single file, "vb/Exevalator.vb." This streamlined design allows for easy integration into your projects in the following three steps:


### 1. Add "Exevalator.vb" to Any Project

If you are using Visual Studio, simply right-click on your project in the "Solution Explorer", select "Add" > "Existing Item", then navigate to and choose "Exevalator.vb" from the "vb" folder of this repository. Once added, "Exevalator.vb" will be visible in your "Solution Explorer".

(Instructions for compiling and running from the command line will be provided later.)


### 2. Check the Name of the Project

In VB.NET projects, the project name is by default added to the top hierarchy of the project's namespaces. Overlooking this detail can lead to errors when referencing a library added to the project.

For simplicity, let's assume the project name where you added "Exevalator.vb" is:

	YourProject

Please check the actual project name at this stage, and use it to replace "YourProject" in the next step.


### 3. Load Exevalator from Your Code, and Use It

Next, load Exevalator in your code with the following "Imports" statement. Then, you can begin using Exevalator:

	...
	' Replace "YourProject" with your actual project name.
	Imports YourProject.Rinearn.ExevalatorVB
	...

	Module YourModule
		...
		Sub YourProcess() 
			' Create an interpreter for evaluating expressions
			Dim exevalator As Exevalator = New Exevalator()

			' Evaluate the value of an expression
			Dim result As Double = exevalator.Eval("1.2 + 3.4")
	
			' Display the result
			Console.WriteLine("result: " + result.ToString())
		End Sub
		...
	End Module

In Exevalator, all numerical values in expressions are treated as Double-type, and the result is also a Double-type. However, computations may fail due to incorrect expressions, access to undeclared variables, etc. If an error occurs, the "Eval" method will throw an ExevalatorException, which should be caught and handled appropriately in practical applications.




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

The following are the main features of Exevalator:

### 1. Evaluate (Compute) Expressions

As demonstrated in previous sections, Exevalator can evaluate the value of an expression:

	Dim result As Double = exevalator.Eval("(-(1.2 + 3.4) * 5) / 2")
	' result: -11.5

	(See: vb/Example2.vb)

You can use operators such as "+" (addition), "-" (subtraction and unary-minus operation), "\*" (multiplication), and "/" (division) in expressions. Multiplications and divisions are prioritized over additions and subtractions in the order of operations.


### 2. Use Variables

You can declare variables programmatically using Exevalator's "declareVariable" method during application development, allowing these variables to be freely used within expressions:

	' Declare a variable to be used in expressions
	exevalator.DeclareVariable("x")
	exevalator.WriteVariable("x", 1.25)
	
	' Evaluate the expression using the declared variable
	Dim result As Double = exevalator.Eval("x + 1")
	' result: 2.25

	(See: vb/Example3.vb)

For more frequent variable value updates, access the variable by address for faster performance:

	Dim address As Integer = exevalator.DeclareVariable("x")
	exevalator.WriteVariableAt(address, 1.25)
	...

	(See: vb/Example4.vb)

Access by address is quicker than by name.



### 3. Use Functions

You can create functions by implementing the "IExevalatorFunction" interface, allowing these functions to be used within expressions:

	' Create a function available in expressions
	Class MyFunction : Implements IExevalatorFunction

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

This setup ensures that developers can programmatically define and integrate custom functions, which users can then utilize seamlessly within their expressions.




<a id="methods"></a>
## List of Methods/Specifications

Here is a list of methods for the Exevalator class, along with their specifications:

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
| Description | Creates a new Exevalator interpreter instance. |
| Parameters | None |
| Return | The newly created instance. |


<a id="methods-eval"></a>
| Signature | Eval(expression As String) As Double |
|:---|:---|
| Description | Evaluates the value of a given expression. |
| Parameters | expression: The expression to be evaluated. |
| Return | The resulting value of the expression. |
| Exception | ExevalatorException is thrown if an error occurs during the evaluation. |


<a id="methods-reeval"></a>
| Signature | Reeval() As Double |
|:---|:---|
| Description | Re-evaluates the value of the expression last evaluated by the "Eval" method. <br>This method may work slightly faster than repeatedly calling "eval" for the same expression.<br> Note that the result may differ from the last evaluation if variables or function behaviors have changed. |
| Parameters | None |
| Return | The re-evaluated value. |
| Exception | ExevalatorException is thrown if an error occurs during the evaluation. |


<a id="methods-declare-variable"></a>
| Signature | DeclareVariable(name As String) As Integer |
|:---|:---|
| Description | Declares a new variable to be used in expressions. |
| Parameters | name: The name of the variable. |
| Return | The virtual address of the declared variable, facilitating faster access.<br>See the "WriteVariableAt" and "ReadVariableAt" methods. |
| Exception | ExevalatorException will be thrown if invalid name is specified. |


<a id="methods-write-variable"></a>
| Signature | WriteVariable(name As String, value As Double) |
|:---|:---|
| Description | Sets the specified variable to a new value. |
| Parameters | name: The name of the variable.<br>value: The new value to set. |
| Return | None |
| Exception | ExevalatorException is thrown if the variable does not exist or the name is invalid. |


<a id="methods-write-variable-at"></a>
| Signature | WriteVariableAt(address As Integer, value As Double) |
|:---|:---|
| Description | Sets the value of a variable at the specified virtual address, which is faster than using "WriteVariable." |
| Parameters | address: The virtual address of the variable.<br>value: The new value to set. |
| Return | None |
| Exception | ExevalatorException is thrown if the address is invalid. |


<a id="methods-read-variable"></a>
| Signature | ReadVariable(name As String) As Double |
|:---|:---|
| Description | Retrieves the current value of the specified variable. |
| Parameters | name: The name of the variable. |
| Return | The current value of the variable. |
| Exception | ExevalatorException is thrown if the variable does not exist or the name is invalid. |


<a id="methods-read-variable-at"></a>
| Signature | ReadVariableAt(address As Integer) As Double |
|:---|:---|
| Description | Retrieves the value of a variable at the specified virtual address, which is faster than using "ReadVariable." |
| Parameters | address: The virtual address of the variable. |
| Return | The current value of the variable. |
| Exception | Exevalator.Exception is thrown if the address is invalid. |


<a id="methods-connect-function"></a>
| Signature | ConnectFunction(name As String, function As IExevalatorFunction) |
|:---|:---|
| Description | Connects a function to be used in expressions. |
| Parameters | name: The function name as used in expressions.<br>function: An instance of a class implementing the IExevalatorFunction interface, which must define the method "Invoke(arguments() As Double) As Double" to process the function. |
| Return | None |
| Exception | ExevalatorException is thrown if an invalid name is specified. |
| Return | None |
| Exception | ExevalatorException will be thrown if invalid name is specified. |


<hr />

<a id="credits"></a>
## Credits

- Microsoft Windows, Visual Basic, .NET, and Visual Studio are either a registered trademarks or trademarks of Microsoft Corporation in the United States and/or other countries.

- Other names may be either a registered trademarks or trademarks of their respective owners. 


