# How to Use Exevalator in Python

&raquo; [Japanese](./README_JAPANESE.md)

&raquo; [Ask the AI for help (ChatGPT account required)](https://chatgpt.com/g/g-UjkEJeO8x-exevalator-assistant)

## Table of Contents
- [Requirements](#requirements)
- [How to Use](#how-to-use)
- [How to Run the Example Code](#example-code)
- [Features](#features)
- [List of Methods/Specifications](#methods)
	- [Constructor](#methods-constructor)
	- [eval(expression: str) -> float](#methods-eval)
	- [reeval() -> float](#methods-reeval)
	- [declare_variable(name: str) -> int](#methods-declare-variable)
	- [write_variable(name: str, value: float) -> None](#methods-write-variable)
	- [write_variable_at(address: int, value: float) -> None](#methods-write-variable-at)
	- [read_variable(name: str) -> float](#methods-read-variable)
	- [read_variable_at(address: int) -> float](#methods-read-variable-at)
	- [void connect_function(name: str, function: FunctionInterface) -> None](#methods-connect-function)



<a id="requirements"></a>
## Requirements

* Python 3.9 – 3.13, or later (CPython).


<a id="how-to-use"></a>
## How to Use

Exevalator is an interpreter compactly implemented in a single file, `python/exevalator.py`.
This streamlined design lets you integrate it in three simple steps:


### 1. Place the File in Your Project

Copy `python/exevalator.py` into your source tree, for example:

	your_project/src/exevalator.py

### 2. Import It Where You Need

	from exevalator import Exevalator, ExevalatorException

### 3. Evaluate an Expression

Now, you are ready to use Exevalator! You can compute expressions as follows:

	# Create an instance of Exevalator Engine
	ex = Exevalator()

	# Evaluate the value of an expression
	result = ex.eval("1.2 + 3.4")

	# Show the result
	print(f"result: {result}")   # -> result: 4.6

All numbers in expressions are handled as Python `float` values, and `eval()` returns a `float`.

If computation fails (e.g., syntactic error, undeclared variable/function), eval() raises ExevalatorException, which you should catch and handle appropriately in real applications.



<a id="example-code"></a>
## How to Run the Example Code

Simple example programs such as `python/example*.py` are bundled in this repository.
You can run them as follows:

	cd python
	python example1.py

`example1.py` demonstrates how to compute the value `1.2 + 3.4` using Exevalator:

	result = ex.eval("1.2 + 3.4")

	# prints: result: 4.6

You can run the other examples (`example2.py` ... `example7.py`) in the same manner.

Additionally, a benchmark program `python/benchmark.py` is included for measuring Exevalator's processing speed.

> Performance considerations:
> 
> In implementations for compiled languages or scripting runtimes with a JIT, Exevalator typically reaches a performance level on the order of several hundred MFLOPS.
> 
> In the Python edition, performance is 1-2 orders of magnitude slower (on the order of a few to about 10 MFLOPS under standard CPython). 
> This gap should be taken into account when adopting Exevalator. That said, it is still significantly faster than the language’s built-in eval.


<a id="features"></a>
## Features

The following are the main features of Exevalator.

### 1. Evaluate (Compute) Expressions

As demonstrated above, Exevalator evaluates arithmetic expressions:

	from exevalator import Exevalator

	ex = Exevalator()
	result = ex.eval("(-(1.2 + 3.4) * 5) / 2")

	print(result)  # -> -11.5

	# See: python/example2.py

Available operators: "+" (addition), "-" (subtraction and unary minus), "*" (multiplication), and "/" (division).

Multiplication and division take precedence over addition and subtraction.


### 2. Use Variables

You can declare variables programmatically using Exevalator's `declare_variable` method during application development, allowing these variables to be freely used within expressions:

	# Declare a variable to be used in expressions
	ex.declare_variable("x")
	ex.write_variable("x", 1.25)

	# Evaluate the expression using the declared variable
	result = ex.eval("x + 1")
	print(result)  # -> 2.25

	# See: python/example3.py

For frequent updates, access variables by address for faster performance:

	addr = ex.declare_variable("x")
	ex.write_variable_at(addr, 1.25)  # faster than name-based write

	result = ex.eval("x + 1")

	# See: python/example4.py


### 3. Use Functions

Provide custom functions by implementing the simple callable protocol `FunctionInterface` and connect them by name:

	from typing import Sequence
	from exevalator import Exevalator, FunctionInterface, ExevalatorException

	# Create a function available in expressions
	class MyFunction(FunctionInterface):
    	def invoke(self, arguments: list[float]) -> float:
		if len(arguments) != 2:
			raise ExevalatorException("Incorrect number of arguments")
		return arguments[0] + arguments[1]

	# Connect the above function for using it in expressions
	ex = Exevalator()
	ex.connect_function("fun", MyFunction())

	# Compute the expression in which the above function is used
	result = ex.eval("fun(1.2, 3.4)")
	print(result)  # -> 4.6

	# See: python/example5.py

This allows developers to programmatically define and integrate custom functions that can be used seamlessly within expressions.


<a id="methods"></a>
## List of Methods/Specifications

Here is a list of methods for the Exevalator class, along with their specifications:

- [Constructor](#methods-constructor)
- [eval(expression: str) -> float](#methods-eval)
- [reeval() -> float](#methods-reeval)
- [declare_variable(name: str) -> int](#methods-declare-variable)
- [write_variable(name: str, value: float) -> None](#methods-write-variable)
- [write_variable_at(address: int, value: float) -> None](#methods-write-variable-at)
- [read_variable(name: str) -> float](#methods-read-variable)
- [read_variable_at(address: int) -> float](#methods-read-variable-at)
- [connect_function(name: str, function: FunctionInterface) -> None](#methods-connect-function)


<a id="methods-constructor"></a>
| Signature | (constructor) Exevalator() |
|:---|:---|
| Description | Creates a new Exevalator interpreter instance. |
| Parameters | None |
| Return | The newly created instance. |


<a id="methods-eval"></a>
| Signature | eval(expression: str) -> float |
|:---|:---|
| Description | Evaluates the value of a given expression. |
| Parameters | expression (str): The expression to be evaluated. |
| Return | float: The resulting value of the expression. |
| Exception | `ExevalatorException` is thrown if an error occurs during the evaluation. |


<a id="methods-reeval"></a>
| Signature | reeval() -> float |
|:---|:---|
| Description | Re-evaluates the value of the expression last evaluated by the `eval` method. <br>This method may work slightly faster than repeatedly calling `eval` for the same expression.<br> Note that the result may differ from the last evaluation if variables or function behaviors have changed. |
| Parameters | None |
| Return | float: The re-evaluated value. |
| Exception | `ExevalatorException` is thrown if an error occurs during the evaluation. |


<a id="methods-declare-variable"></a>
| Signature | declare_variable(name: str) -> int |
|:---|:---|
| Description | Declares a new variable to be used in expressions. |
| Parameters | name (str): The name of the variable. |
| Return | int: The virtual address of the declared variable, facilitating faster access.<br>See the `write_variable_at` and `read_variable_at` methods. |
| Exception | `ExevalatorException` will be thrown if invalid name is specified. |


<a id="methods-write-variable"></a>
| Signature | write_variable(name: str, value: float) -> None |
|:---|:---|
| Description | Sets the specified variable to a new value. |
| Parameters | name (str): The name of the variable.<br>value (float): The new value to set. |
| Return | None |
| Exception | `ExevalatorException` is thrown if the variable does not exist or the name is invalid. |


<a id="methods-write-variable-at"></a>
| Signature | write_variable_at(address: int, value: float) -> None |
|:---|:---|
| Description | Sets the value of a variable at the specified virtual address, which is faster than using `write_variable`. |
| Parameters | address (int): The virtual address of the variable.<br>value (float): The new value to set. |
| Return | None |
| Exception | `ExevalatorException` is thrown if the address is invalid. |


<a id="methods-read-variable"></a>
| Signature | read_variable(name: str) -> float |
|:---|:---|
| Description | Retrieves the current value of the specified variable. |
| Parameters | name (str): The name of the variable. |
| Return | float: The current value of the variable. |
| Exception | `ExevalatorException` is thrown if the variable does not exist or the name is invalid. |


<a id="methods-read-variable-at"></a>
| Signature | read_variable_at(address: int) -> float |
|:---|:---|
| Description | Retrieves the value of a variable at the specified virtual address, which is faster than using `read_variable`. |
| Parameters | address (int): The virtual address of the variable. |
| Return | float: The current value of the variable. |
| Exception | `ExevalatorException` is thrown if the address is invalid. |


<a id="methods-connect-function"></a>
| Signature | connect_function(name: str, function: FunctionInterface) -> None |
|:---|:---|
| Description | Connects a function to be used in expressions. |
| Parameters | name (str): The function name as used in expressions.<br>function (FunctionInterface): An instance of a class implementing the `FunctionInterface`, which must define the method `invoke(self, arguments: List[float]) -> float` to process the function. |
| Return | None |
| Exception | `ExevalatorException` is thrown if an invalid name is specified. |





<hr />

<a id="credits"></a>
## Credits

- Python is a trademark or a registered trademark of Python Software Foundation in the United States and other countries.

- ChatGPT is a trademark or a registered trademark of OpenAI OpCo, LLC in the United States and other countries.

- Other names may be either a registered trademarks or trademarks of their respective owners. 


