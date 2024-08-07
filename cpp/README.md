# How to Use Exevalator in C++

&raquo; [Japanese](./README_JAPANESE.md)


## Table of Contents
- [Requirements](#requirements)
- [How to Use](#how-to-use)
- [Example Code](#example-code)
- [Features](#features)
- [List of Methods/Specifications](#methods)
	- [Constructor](#methods-constructor)
	- [double eval(const std::string &expression)](#methods-eval)
	- [double reeval()](#methods-reeval)
	- [size_t declare_variable(const std::string &name)](#methods-declare-variable)
	- [void write_variable(const std::string &name, double value)](#methods-write-variable)
	- [void write_variable_at(size_t address, double value)](#methods-write-variable-at)
	- [double read_variable(const std::string &name)](#methods-read-variable)
	- [double read_variable_at(size_t address)](#methods-read-variable-at)
	- [size_t connect_function(const std::string &name, const std::shared_ptr&lt;ExevalatorFunctionInterface&gt; &function_ptr)](#methods-connect-function)




<a id="requirements"></a>
## Requirements

* Compilers supporting C++17 or later<br />( We use clang++ on Linux in this document )




<a id="how-to-use"></a>
## How to Use

Exevalator is an interpreter, compactly implemented across two files: "cpp/exevalator.cpp" and "cpp/exevalator.hpp" (the header file). Below, we demonstrate how to use these files in a simple example.


### 1. Place the Files in Your Program's Source Code Folder

First, place the files in the source code folder(s) of your program where you want to use Exevalator.

For projects with strictly organized source directories, you should place "exevalator.hpp" in the headers directory and "exevalator.cpp" in the implementation files directory. Then, adjust your build configuration files accordingly.

For smaller, simpler projects, such as those where all source files are in a single folder and directly included from a file containing the main function, place both "exevalator.hpp" and "exevalator.cpp" in the same folder.


### 2. Load Exevalator from Your Code, and Use it

Next, include Exevalator in your code. The simplest way, though not best practice, is to directly include both "exevalator.hpp" and "exevalator.cpp" as shown:

	#include <iostream>
	#include <cstdlib>
	#include "exevalator.hpp"
	#include "exevalator.cpp"

	int main() {

		// Create an interpreter for evaluating expressions
		exevalator::Exevalator exevalator;

		try {

			// Evaluate (compute) the value of an expression
			double result = exevalator.eval("1.2 + 3.4");

			// Display the result
			std::cout << "result: " << result << std::endl;

		// If any error occurred when evaluating the expression, handle it
		} catch (exevalator::ExevalatorException &e) {
			std::cout << "Error occurred: " << e.what() << std::endl;
			return EXIT_FAILURE;
		}

		return EXIT_SUCCESS;
	}

This example evaluates "1.2 + 3.4" and displays the result. Compile and run the code (saved as "example.cpp") as follows:

	clang++ -std=c++17 -o example example.cpp
	./example

The output will be:

	result: 4.6

This demonstrates the correct computation of "1.2 + 3.4".

In Exevalator, all numbers are handled as double-type values, and the return value is also double-type. Note, however, that the computation may fail if the expression is incorrect, or if an undeclared variable is accessed, among other reasons. If a computation fails, the "eval" method throws an ExevalatorException, so it's important to catch and handle it as shown in the example above.


### 3. (Additional) How to Compile Separately and Link

Additionally, if your project uses separate compilation, which compiles each source file into modules and then links them, you should only include "exevalator.hpp" in the source files that utilize Exevalator. "exevalator.cpp" should be compiled as a separate module, which will then be linked during the final build step to generate the executable.

For example, modify the previous example code as follows:

	#include <iostream>
	#include <cstdlib>
	#include "exevalator.hpp" // Do not include the .cpp file

	int main() {

		// Create an interpreter for evaluating expressions
		exevalator::Exevalator exevalator;
	...

You can compile the above code and "exevalator.cpp" separately as follows:

	clang++ -std=c++17 -c exevalator.cpp
	clang++ -std=c++17 -c example.cpp

The above commands generate two compiled module files: "exevalator.o" and "example.o". You can then link them as follows:

	clang++ -o linked_example example.o exevalator.o

This command generates the executable file "linked_example". Execute it to see the result:

	./linked_example

	result: 4.6

As shown above, we've achieved the same (and correct) result as in the previous section.

Typically, in practical projects that require separate compilation, build tools are used to automate the compilation and linking steps. These tools execute these steps based on instructions written in build configuration files, such as Makefiles. While we have demonstrated how to compile and link sources manually here, in practical projects, you would need to write or modify the contents of the build file appropriately.


<a id="example-code"></a>
## Example Code

Simple example code files named "cpp/example*.cpp" are bundled in this repository.

You can compile and run them in exactly the same way as described in step 2 of the previous section. For example, to compile and run "example1.cpp", use the following commands:

	clang++ -std=c++17 -o example1 example1.cpp
	./example1

The code in "example1.cpp" is almost identical to that in "example.cpp" from the previous section, so the result will be:

	result: 4.6

As shown above, the computed value of the expression "1.2 + 3.4" will be displayed. You can compile and run other examples in the same way.

Additionally, a benchmark program named "cpp/benchmark.cpp" for measuring the processing speed of Exevalator is included in this repository. To compile it, include the optimization option as follows:

	clang++ -std=c++17 -O2 -o benchmark benchmark.cpp

In the above command, "-O2" specifies the optimization level. Omitting this may prevent Exevalator from operating at full speed.


<a id="features"></a>
## Features

The following are the main features of Exevalator:

### 1. Evaluate (Compute) Expressions

As demonstrated in previous sections, Exevalator can evaluate the value of an expression:

	double result = exevalator.eval("(-(1.2 + 3.4) * 5) / 2");
	// result: -11.5

	(See: cpp/example2.cpp)

You can use operators such as "+" (addition), "-" (subtraction and unary-minus operation), "\*" (multiplication), and "/" (division) in expressions. Multiplications and divisions are prioritized over additions and subtractions in the order of operations.


### 2. Use Variables

You can declare variables programmatically using Exevalator's "declareVariable" method during application development, allowing these variables to be freely used within expressions:

	// Declare a variable to be used in expressions
	exevalator.declare_variable("x");
	exevalator.write_variable("x", 1.25);

	// Evaluate the expression using the declared variable
	double result = exevalator.eval("x + 1");
	// result: 2.25

	(See: cpp/example3.cpp)

For more frequent variable value updates, access the variable by address for faster performance:

	#include <cstddef>
	...

	size_t address = exevalator.declare_variable("x");
	exevalator.write_variable_at(address, 1.25);
	...

	(See: cpp/example4.cpp)

Access by address is quicker than by name.

Please note that the above variable-related methods can throw an "ExevalatorException" when they fail to declare or access variables.


### 3. Use Functions

You can create functions by implementing the "exevalator::ExevalatorFunctionInterface", allowing these functions to be used within expressions:

	// Create a function available in expressions
	class MyFun : public exevalator::ExevalatorFunctionInterface {
		double operator()(const std::vector<double> &arguments) {
			if (arguments.size() != 2) {
				throw new exevalator::ExevalatorException("Incorrect number of args");
			}
			return arguments[0] + arguments[1];
		}
	}
	...

	// Connect the above function for using it in expressions
	// (Passing an instance of the class through "shared_ptr")
	exevalator::Exevalator exevalator;
	exevalator.connect_function("fun", std::make_shared<MyFun>());

	// Compute the expression in which the above function is used
	double result = exevalator.eval("fun(1.2, 3.4)");
	// result: 4.6

	(See: cpp/example5.cpp)

This setup ensures that developers can programmatically define and integrate custom functions, which users can then utilize seamlessly within their expressions.

**CAUTION: In Ver.1.0, values of arguments passed from expressions were stored in the "std::vector<double> &arguments" in reversed order. This behavior has been corrected in Ver.2.0. For details, please refer to issue #2.**

Please note that, like variable-related methods, the above function-related methods can also throw an ExevalatorException. In addition, an exception may occur in the process of functions called in an expression evaluated by the "eval" method (for example, in the above example, the function implemented in the MyFun class throws an exception when too many or few arguments are passed). In such cases, the "eval" method re-throws the exception by wrapping it in an ExevalatorException.


<a id="methods"></a>
## List of Methods/Specifications

Here is a list of methods for the Exevalator class, along with their specifications:

- [Constructor](#methods-constructor)
- [double eval(const std::string &amp;expression)](#methods-eval)
- [double reeval()](#methods-reeval)
- [size_t declare_variable(const std::string &amp;name)](#methods-declare-variable)
- [void write_variable(const std::string &amp;name, double value)](#methods-write-variable)
- [void write_variable_at(size_t address, double value)](#methods-write-variable-at)
- [double read_variable(const std::string &amp;name)](#methods-read-variable)
- [double read_variable_at(size_t address)](#methods-read-variable-at)
- [size_t connect_function(const std::string &amp;name, const std::shared_ptr&lt;ExevalatorFunctionInterface&gt; &amp;function_ptr)](#methods-connect-function)


<a id="methods-constructor"></a>
| Signature | (constructor) Exevalator() |
|:---|:---|
| Description | Creates a new Exevalator interpreter instance. |
| Parameters | None |
| Return | The newly created instance. |


<a id="methods-eval"></a>
| Signature | double eval(const std::string &amp;expression) |
|:---|:---|
| Description | Evaluates the value of a given expression. |
| Parameters | expression: The expression to be evaluated. |
| Return | The resulting value of the expression. |
| Exception | ExevalatorException is thrown if an error occurs during the evaluation. |


<a id="methods-reeval"></a>
| Signature | double reeval() |
|:---|:---|
| Description | Re-evaluates the last expression processed by the "eval" method.<br>This method may work slightly faster than repeatedly calling "eval" for the same expression.<br> Note that the result may differ from the last evaluation if variables or function behaviors have changed. |
| Parameters | None |
| Return | The re-evaluated value. |
| Exception | ExevalatorException is thrown if an error occurs during the evaluation. |


<a id="methods-declare-variable"></a>
| Signature | size_t declare_variable(const std::string &amp;name) |
|:---|:---|
| Description | Declares a new variable for use in expressions. |
| Parameters | name: The name of the variable. |
| Return | 	The virtual address of the declared variable, facilitating faster access.<br>See the "write_variable_at" and "read_variable_at" methods. |
| Exception | ExevalatorException will be thrown if invalid name is specified. |


<a id="methods-write-variable"></a>
| Signature | void write_variable(const std::string &amp;name, double value) |
|:---|:---|
| Description | Assigns a new value to the specified variable. |
| Parameters | name: The name of the variable.<br>value: The new value to assign. |
| Return | None |
| Exception | ExevalatorException is thrown if the variable does not exist or the name is invalid. |


<a id="methods-write-variable-at"></a>
| Signature | void write_variable_at(size_t address, double value) |
|:---|:---|
| Description | Assigns a value to a variable at a specified virtual address, which is faster than using "write_variable." |
| Parameters | address: The virtual address of the variable.<br>value: The new value to assign. |
| Return | None |
| Exception | ExevalatorException is thrown if the address is invalid. |


<a id="methods-read-variable"></a>
| Signature | double read_variable(const std::string &amp;name) |
|:---|:---|
| Description | Retrieves the current value of the specified variable. |
| Parameters | name: The name of the variable. |
| Return | The current value of the variable. |
| Exception | ExevalatorException is thrown if the variable does not exist or the name is invalid. |


<a id="methods-read-variable-at"></a>
| Signature | double read_variable_at(size_t address) |
|:---|:---|
| Description | Retrieves the value of a variable at the specified virtual address, which is faster than using "read_variable." |
| Parameters | address: The virtual address of the variable. |
| Return | The current value of the variable. |
| Exception | ExevalatorException is thrown if the address is invalid. |


<a id="methods-connect-function"></a>
| Signature | size_t connect_function(const std::string &amp;name, const std::shared_ptr&lt;ExevalatorFunctionInterface&gt; &amp;function_ptr) |
|:---|:---|
| Description | Connects a function for use in expressions. |
| Parameters | name: The function name as used in expressions.<br>function_ptr: The shared pointer to the class implementing "exevalator::ExevalatorFunctionInterface", which defines the method "double invoke(const std::vector&lt;double&gt; &amp;arguments)" to process the function. |
| Return | None |
| Exception | ExevalatorException is thrown if an invalid name is specified. |




<hr />

<a id="credits"></a>
## Credits

- Linux is a trademark of linus torvalds in the United States and/or other countries.

- Other names may be either a registered trademarks or trademarks of their respective owners.


