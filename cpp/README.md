# How to Use Exevalator in C++

&raquo; [Japanese](./README_JAPANESE.md)


## English Index
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

The interpreter of Exevalator is implemented in the file "cpp/exevalator.cpp". The header file is "cpp/exevalator.hpp". Only these two files are required to use Exevalator in your programs.
In this section, we will use the above in a simple example code.

### 1. Put above files into the source code folder of your program

At first, put above files into the source code folder(s) of your program, in which you want to use Exevalator.

When you want to use Exevalator in projects in which source files and build steps are tidy organized, you should probably put "exevalator.hpp" into the folder storing header files, and put "exevalator.cpp" into the folder storing implementation files. Then modify the content of a build file appropriately.

On the other hand, when you want to use Exevalator in more small anf simple project
<span style="text-decoration: line-through">&nbsp;&nbsp;&nbsp;&nbsp;</span>
e.g: all source files are locating in one folder, and they are "include"ed directly from a source file having a main function
<span style="text-decoration: line-through">&nbsp;&nbsp;&nbsp;&nbsp;</span>,
put both "exevalator.app" and "exevalator.cpp" into the folder.



### 2. Load Exevalator from your code, and use it


Next, load Exevalator from your code. If you don't care much about manners, it is most simple way that "include" directly both "exevalator.hpp" and "exevalator.cpp" from your code in which you want to use Exevalator:


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


The above example code computes the value of an expression "1.2 + 3.4", and displays the result. You can compile and run the above code (saved as "example.cpp") as follows:

	clang++ -std=c++17 -o example example.cpp
	./example

The result is:

	result: 4.6

As the above, the correct value of "1.2 + 3.4" will be computed and displayed.

By the way, on Exevalator, all numbers in expressions will be handled as double-type values, so the result is always double-type.
However, the computation may fail when an incorrect expression is inputted, or when an undeclared variable is accessed, and so on. If a computation failed, "eval" method throws an ExevalatorException, so catch and handle it as the above example code.


### 3. How to compile separately and link

On many projects developing software in C++, source files are compiled separately as modules, and they are linked at the end of building steps.
If you want to use Exevalator in such project, include ONLY header file "exevalator.hpp" from your code. For example, modify the previous example code as follows:

	#include <iostream>
	#include <cstdlib>
	#include "exevalator.hpp" // don't include .cpp

	int main() {

		// Create an interpreter for evaluating expressions
		exevalator::Exevalator exevalator;
	...

You can compile the above code and "exevalator.cpp" separately as follows:

	clang++ -std=c++17 -c exevalator.cpp
	clang++ -std=c++17 -c example.cpp

Above command lines generates two compiled module files: "exevalator.o" and "example.o". You can link them as:

	clang++ -o linked_example example.o exevalator.o

Above command generates an executable file "linked_example". Let's execute it:

	./linked_example

	result: 4.6

As the above, we've got the same (and correct) result with the previous section.

By the way, ordinarily, on practical projects requiring separate compilation, build tools are used for doing compilation/link steps automatically.
Build tools perform compilation/link steps based on code written in build files, such as "Makefile".
Here we have demonstrated how to compile/link sources manually, but on practical projects, you are required to write/modify contents of the build file appropriately.


<a id="example-code"></a>
## Example Code

Simple example code "cpp/example*.cpp" are bundled in this repository.

You can compile and run them by completely same way as step 2. in the previous section. For example, how to compile/run "example1.cpp" is:

	clang++ -std=c++17 -o example1 example1.cpp
	./example1

The code of "example1.cpp" is almost the same as "example.cpp" in the previous section, so the result is:

	result: 4.6

As the above, the computed value of an expression "1.2 + 3.4" will be displayed. You can compile/run other examples in the same way. 

Also, a benchmark program "cpp/benchmark.cpp" for measuring processing speed of Exevalator is bundled in this repository. For compiling it, specify the optimization-option as:

	clang++ -std=c++17 -O2 -o benchmark benchmark.cpp

In the above, "-O2" is the optimization option. If you forget it, Exevalator can not work in full-speed.


<a id="features"></a>
## Features

The followings are main features of Exevalator.

### 1. Evaluate (Compute) Expressions

As shown in previous sections, Exevalator can evaluate (compute) the value of an expression:

	double result = exevalator.eval("(-(1.2 + 3.4) * 5) / 2");
	// result: -11.5

	(See: cpp/example2.cpp)

As the above, you can use "+" (addition), "-" (subtraction and unary-minus operation), "\*" (multiplication), and "/" (division) operators in an expression. In order of operations, "multiplications and divisions" are prioritized than "additions and subtractions".


### 2. Use Variables

You can use variables in expressions:

	// Declare a variable available in expressions
	exevalator.declare_variable("x");
	exevalator.write_variable("x", 1.25);

	// Compute the expression in which the above variable is used
	double result = exevalator.eval("x + 1");
	// result: 2.25

	(See: cpp/example3.cpp)

If you change the value of a variable very frequently, access to the variable by the address as follows:

	int address = exevalator.declare_variable("x");
	exevalator.write_variable_at(address, 1.25);
	...

	(See: cpp/example4.cpp)

This way works faster than accessings by the name.

Please note that, above variable-related methods can throw ExevalatorException when they failed to declare or access to variables.


### 3. Use Functions

You can create functions available in expressions, by inheriting the abstract class "ExevalatorFunctionInterface" as follows:

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
	// (Pass an instance of the class through "shared_ptr")
	exevalator::Exevalator exevalator;
	exevalator.connect_function("fun", std::make_shared<MyFun>());

	// Compute the expression in which the above function is used
	double result = exevalator.eval("fun(1.2, 3.4)");
	// result: 4.6

	(See: cpp/example5.cpp)

**CAUTION: In Ver.1.0, values of arguments passed from expressions had been stored in the above "const std::vector\<double\> &arguments" array in reversed order. This behavior has been fixed in Ver.2.0. For details, please see the issue #2.**

Please note that, as variable-related methods, above function-related methods can throw ExevalatorException.
In addition, exception may occur in process of a functions called in an expression evaluated by "eval" method.
(For example, in the above example, the function implemented in MyFun class throws an exception when too many/few arguments are passed.)
In such case, "eval" method re-throws the exception by wrapping ExevalatorException.


<a id="methods"></a>
## List of Methods/Specifications

The list of methods of Exevalator class, and their specifications.

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
| Description | Creates a new interpreter of the Exevalator. |
| Parameters | None |
| Return | The created instance. |


<a id="methods-eval"></a>
| Signature | double eval(const std::string &amp;expression) |
|:---|:---|
| Description | Evaluates (computes) the value of an expression. |
| Parameters | expression: The expression to be evaluated. |
| Return | The evaluated value. |
| Exception | ExevalatorException will be thrown if any error occurred when evaluating the expression. |


<a id="methods-reeval"></a>
| Signature | double reeval() |
|:---|:---|
| Description | Re-evaluates (re-computes) the value of the expression evaluated by "eval" method last time.<br>This method works faster than calling "eval" method repeatedly for the same expression.<br>Note that, the result value may different with the last evaluated value, if values of variables or behaviour of functions had changed. |
| Parameters | None |
| Return | The evaluated value. |
| Exception | ExevalatorException will be thrown if any error occurred when evaluating the expression. |


<a id="methods-declare-variable"></a>
| Signature | size_t declare_variable(const std::string &amp;name) |
|:---|:---|
| Description | Declares a new variable, for using the value of it in expressions. |
| Parameters | name: The name of the variable to be declared. |
| Return | The virtual address of the declared variable, which is useful for accessing to the variable faster.<br>See "write_variable_at" and "read_variable_at" method. |
| Exception | ExevalatorException will be thrown if invalid name is specified. |


<a id="methods-write-variable"></a>
| Signature | void write_variable(const std::string &amp;name, double value) |
|:---|:---|
| Description | Writes the value to the variable having the specified name. |
| Parameters | name: The name of the variable to be written.<br>value: The new value of the variable. |
| Return | None |
| Exception | ExevalatorException will be thrown if the specified variable is not found, or invalid name is specified. |


<a id="methods-write-variable-at"></a>
| Signature | void write_variable_at(size_t address, double value) |
|:---|:---|
| Description | Writes the value to the variable at the specified virtual address.<br>This method works faster than "write_variable" method. |
| Parameters | address: The virtual address of the variable to be written.<br>value: The new value of the variable. |
| Return | None |
| Exception | ExevalatorException will be thrown if the invalid address is specified. |


<a id="methods-read-variable"></a>
| Signature | double read_variable(const std::string &amp;name) |
|:---|:---|
| Description | Reads the value of the variable having the specified name. |
| Parameters | name: The name of the variable to be read. |
| Return | The current value of the variable. |
| Exception | ExevalatorException will be thrown if the specified variable is not found, or invalid name is specified. |


<a id="methods-read-variable-at"></a>
| Signature | double read_variable_at(size_t address) |
|:---|:---|
| Description | Reads the value of the variable at the specified virtual address.<br>This method works faster than "read_variable" method. |
| Parameters | address: The virtual address of the variable to be read. |
| Return | The current value of the variable. |
| Exception | ExevalatorException will be thrown if the invalid address is specified. |


<a id="methods-connect-function"></a>
| Signature | size_t connect_function(const std::string &amp;name, const std::shared_ptr&lt;ExevalatorFunctionInterface&gt; &amp;function_ptr) |
|:---|:---|
| Description | Connects a function, for using it in expressions. |
| Parameters | name: The name of the function used in the expression.<br>function_ptr: The shared_ptr pointing to the function to be connected. The function is an instance of the class inheriting ExevalatorFunctionInterface (only "double invoke(const std::vector&lt;double&gt; &amp;arguments)" method is defined, to implement the process of a function). |
| Return | Unused in this version |
| Exception | ExevalatorException will be thrown if invalid name is specified. |



<hr />

<a id="credits"></a>
## Credits

- Linux is a trademark of linus torvalds in the United States and/or other countries.

- Other names may be either a registered trademarks or trademarks of their respective owners.


