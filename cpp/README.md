# How to Use Exevalator in C++

&raquo; [Japanese](./README_JAPANESE.md)


## English Index
- <a href="#requirements">Requirements</a>
- <a href="#how-to-use">How to Use</a>
- <a href="#example-code">Example Code</a>
- <a href="#features">Features</a>




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
			std::cout << "Result: " << result << std::endl;

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

On many projects developing software in C++, source file are compiled separately as modules, and they are linked at the end of building steps.
If you want to use Exevalator in such project, include ONLY header file "exevalator.hpp" from your code. For example, modify the previous example code as follows:

	#include <iostream>
	#include <cstdlib>
	#include "exevalator.hpp" // don't include .cpp

	int main() {

		// Create an interpreter for evaluating expressions
		exevalator::Exevalator exevalator;
	...

You can compile the above code and "exevalator.cpp" separately as follows:

	clang++ -std=c++17 -o exevalator.cpp
	clang++ -std=c++17 -o example.cpp

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

As the above, the computed value of an expression "1.2 + 3.4" will be displayed.

You can execute/run other examples in the same way.


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
		double operator()(std::vector<double> arguments) {
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
	std::cout << "result: " << result << std::endl;
	// result: 4.6

	(See: cpp/example5.cpp)

Please note that, as variable-related functions, above function-related methods can throw ExevalatorException.
In addition, exception may occur in process of a functions called in an expression evaluated by "eval" method.
(For example, in the above example, the function implemented in MyFun class throws an exception when too many/few arguments are passed.)
In such case, "eval" method re-throws the exception by wrapping ExevalatorException.


<a id="credits"></a>
## Credits

- Linux is a trademark of linus torvalds in the United States and/or other countries.

- Other names may be either a registered trademarks or trademarks of their respective owners.


