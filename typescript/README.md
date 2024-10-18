# How to Use Exevalator in TypeScript

&raquo; [Japanese](./README_JAPANESE.md)

&raquo; [Ask the AI for help (ChatGPT account required)](https://chatgpt.com/g/g-UjkEJeO8x-exevalator-assistant)

## Table of Contents
- [Requirements](#requirements)
- [How to Use](#how-to-use)
- [How to Compile and Run Example Code](#example-code)
- [Features](#features)
- [List of Methods/Specifications](#methods)
	- [Constructor](#methods-constructor)
	- [eval(expression: string): number](#methods-eval)
	- [reeval(): number](#methods-reeval)
	- [declareVariable(name: string): number](#methods-declare-variable)
	- [writeVariable(name: string, value: number): void](#methods-write-variable)
	- [writeVariableAt(address: number, value: number): void](#methods-write-variable-at)
	- [readVariable(name: string): number](#methods-read-variable)
	- [readVariableAt(address: number): number](#methods-read-variable-at)
	- [connectFunction(name: string, function: ExevalatorFunctionInterface): void](#methods-connect-function)
- [If You Want More Features: Try to Use Vnano](#vnano)



<a id="requirements"></a>
## Requirements

* Node.js
* tsc (TypeScript Compiler)
* esbuild (a bundler for running in browsers)

First, install Node.js on your PC, and then set up the remaining tools by running the following commands:

	cd <working_folder>
	npm init
	npm install --save-dev typescript
	npm install --save-dev @types/node 
	npm install --save-dev esbuild

With the '--save-dev' option, TypeScript compiler 'tsc' and bundler tool 'esbuild' will be installed inside the 'node_modules' folder of your working directory.

This way, you can use these tools with the 'npx' command inside the working folder, without affecting the environment of other folders.

Actual compilation and execution methods will be explained in later sections.


<a id="how-to-use"></a>
## How to Use

The Exevalator interpreter is implemented in a single file, 'typescript/exevalator.ts'. Therefore, you can easily import and use it in just three steps.


### 1. Place the File in the Source Folder of Your Project

First, place 'typescript/exevalator.ts' in any location you like within your project's source folder.

For simplicity, let's assume you place it in the same folder as the code that will import and use Exevalator.

### 2. Import it from Your Code

Next, import it from the code where you want to perform expression evaluations, and use it as follows:

	// Import Exevalator (specify the relative path based on where you placed it)
	import Exevalator from "./exevalator";

	// Create an instance of the Exevalator interpreter
	let exevalator: Exevalator = new Exevalator();

	// Evaluate an expression
	const result: number = exevalator.eval("1.2 + 3.4");

	// Output the result
	console.log(`result: ${result}`);

In Exevalator, all numerical values within expressions are treated as 'number' types. Therefore, the result will always be of type 'number'.

Incidentally, if the expression is incorrect or uses an undeclared variable, evaluation may fail. In such cases, an 'ExevalatorError' will be thrown at the point where the 'eval' method is called. For practical use, you can handle this by catching the error as needed.
import Exevalator, { ExevalatorError } from "./exevalator"

	...

	try {
		// Evaluate the expression
		const result: number = exevalator.eval(expression);

	} catch (error) {
		if (error instanceof ExevalatorError) {
			// Handle the error here
		}
	}



<a id="example-code"></a>
## How to Compile and Run Example Code

This repository includes some simple sample code under 'typescript/example*.ts' that demonstrates how to use Exevalator. For example, 'example1' can be compiled and run as follows:

	cd typescript
	npx tsc example1.ts
	node example1.js

The 'example1.ts' file is a sample code that calculates the value of "1.2 + 3.4" using Exevalator. Its content is almost identical to the sample "YourClass" shown in the previous section:

	...
	const result: number = exevalator.eval("1.2 + 3.4");
	...

The execution result will be:

	result: 4.6

As shown above, the result of the calculation for "1.2 + 3.4" is displayed. 'example1' through 'example5' can be compiled and run in exactly the same way.

On the other hand, 'example6' and 'example7' are samples designed to run in a web browser. For these, you'll need to bundle the code, including Exevalator, into a single JavaScript file using the bundler tool esbuild:

(Assuming you've already set up the environment within the typescript folder)

	cd typescript
	npx esbuild example6.ts --bundle --outfile=example6.bundle.js

This will generate 'example6.bundle.js'. Then, open the HTML file 'example6.html' in the same folder using a web browser, and the script will be loaded and executed. When you run it, you'll be prompted to input the contents of the expression f(x) and the value of the variable x. After entering the values, the calculation will be performed and the result will be displayed.

Similarly, you can build and run 'example7' in the browser. This time, you'll be prompted to input the contents of the expression f(x) along with the lower and upper bounds for integration. After entering the values, numerical integration will be performed using Exevalator in the browser, and the result will be displayed.

Additionally, a benchmark script "ts/benchmark.ts" for measuring the processing speed of Exevalator is also included in this repository. You can compile and run it in the same way as 'example1.ts'.


<a id="features"></a>
## Features

Here, we introduce the main features of Exevalator.

### 1. Expression Evaluation (Calculation)

As we've seen in previous sections, Exevalator allows you to evaluate expressions:

	double result = exevalator.eval("(-(1.2 + 3.4) * 5) / 2");
	// result: -11.5

(See: typescript/example2.ts)

As shown above, you can perform operations like '+' (addition), '-' (subtraction or negation), '*' (multiplication), and '/' (division). Multiplications and divisions are prioritized over additions and subtractions in the order of operations.


### 2. Using Variables

When implementing your application, you can declare variables in Exevalator using the 'declareVariable' method. Declared variables can then be freely accessed within expressions:

	// Declare a variable and assign a value
	exevalator.declareVariable("x");
	exevalator.writeVariable("x", 1.25);

	// Evaluate an expression using the variable's value
	const result: number = exevalator.eval("x + 1");
	// result: 2.25

(See: typescript/example3.ts)

If you need to frequently update the values of variables, it can be useful to specify the target variable by its address rather than its name, as shown below:

	int address = exevalator.declareVariable("x");
	exevalator.writeVariableAt(address, 1.25);
	...

(See: typescript/example4.ts)

This method is faster than specifying the target variable by name.


### 3. Using Functions

You can also create functions to use within expressions. To do this, implement a class that follows the 'ExevalatorFunctionInterface':

	import Exevalator, { ExevalatorFunctionInterface, ExevalatorError } from "./exevalator";

	...

	// Create a function that can be used within expressions
	class MyFunction implements ExevalatorFunctionInterface {
		public invoke(args: number[]): number {
			if (args.length !== 2) {
				throw new ExevalatorError(`Incorrect number of args: ${args.length}`);
			}
			return args[0] + args[1];
		}
	}

	...

	// Create an instance of the Exevalator interpreter
	let exevalator: Exevalator = new Exevalator();

	// Connect the above function so that it can be used in expressions
	const fun: MyFunction = new MyFunction();
	exevalator.connectFunction("fun", fun);

	// Evaluate an expression using the function
	const result: number = exevalator.eval("fun(1.2, 3.4)");
	// result: 4.6

(See: typescript/example5.ts)


<a id="methods"></a>
## List of Methods/Specifications

This section provides a list and detailed specifications of the methods offered by the 'Exevalator' class.

- [Constructor](#methods-constructor)
- [eval(expression: string): number](#methods-eval)
- [reeval(): number](#methods-reeval)
- [declareVariable(name: string): number](#methods-declare-variable)
- [writeVariable(name: string, value: number): void](#methods-write-variable)
- [writeVariableAt(address: number, value: number): void](#methods-write-variable-at)
- [readVariable(name: string): number](#methods-read-variable)
- [readVariableAt(address: number): number](#methods-read-variable-at)
- [connectFunction(name: string, function: ExevalatorFunctionInterface): void](#methods-connect-function)


<a id="methods-constructor"></a>
| Signature | (Constructor) Exevalator() |
|:---|:---|
| Description | Creates a new instance of the Exevalator interpreter. |
| Parameters | None |
| Return | The created instance |


<a id="methods-eval"></a>
| Signature | eval(expression: string): number |
|:---|:---|
| Description | Evaluates (calculates) the value of an expression. |
| Parameters | expression: The expression to be evaluated (calculated). |
| Return | The result of the evaluation (calculation) |
| Exception | ExevalatorError is thrown if an error occurs during the evaluation of the expression. |


<a id="methods-reeval"></a>
| Signature | reeval(): number |
|:---|:---|
| Description | Re-evaluates (recalculates) the same expression that was last evaluated by the 'eval' method.<br>This method can be slightly faster than using the 'eval' method repeatedly.<br>Note that if the values of variables or the behavior of functions have changed since the last evaluation, the result of the evaluation may also change. |
| Parameters | None |
| Return | The re-evaluated value. |
| Exception | ExevalatorError is thrown if an error occurs during the evaluation. |


<a id="methods-declare-variable"></a>
| Signature | declareVariable(name: string): number |
|:---|:---|
| Description | Declares a new variable to be used within expressions. |
| Parameters | name: The name of the variable. |
| Return | The virtual address assigned to the declared variable (can be used with the 'writeVariableAt' and 'readVariableAt' methods for faster read/write operations). |
| Exception | ExevalatorError is thrown if an invalid variable name is specified. |


<a id="methods-write-variable"></a>
| Signature | writeVariable(name: string, value: number): void |
|:---|:---|
| Description | Writes a value to the variable with the specified name. |
| Parameters | name: The name of the variable to write to.<br>value:  The value to write. |
| Return | None |
| Exception | ExevalatorError is thrown if the variable does not exist or the name is invalid. |


<a id="methods-write-variable-at"></a>
| Signature | writeVariableAt(address: number, value: number): void |
|:---|:---|
| Description | Writes a value to the variable located at the specified virtual address. This method is faster than using the 'writeVariable' method. |
| Parameters | address: The virtual address of the variable to write to.<br>value: The value to write. |
| Return | None |
| Exception | ExevalatorError is thrown if an invalid address is specified. |


<a id="methods-read-variable"></a>
| Signature | readVariable(name: string): number |
|:---|:---|
| Description | Reads the value of the variable with the specified name. |
| Parameters | name: The name of the variable to read. |
| Return | The current value of the variable. |
| Exception | ExevalatorError is thrown if the specified variable does not exist or if an invalid variable name is specified. |


<a id="methods-read-variable-at"></a>
| Signature | readVariableAt(address: number): number |
|:---|:---|
| Description | Reads the value of the variable located at the specified virtual address. This method is faster than using the 'readVariable' method. |
| Parameters | address: The virtual address of the variable to read. |
| Return | The current value of the variable. |
| Exception | ExevalatorError is thrown if an invalid address is specified. |


<a id="methods-connect-function"></a>
| Signature | connectFunction(name: string, function: ExevalatorFunctionInterface): void |
|:---|:---|
| Description | Connects a function to be used within expressions. |
| Parameters | name: The name of the function to connect.<br>function: An instance of a class implementing the 'ExevalatorFunctionInterface' to provide the function's logic, which must define the method "invoke(args: number[]): number" to process the function. |
| Return | None |
| Exception | ExevalatorError is thrown if an invalid function name is specified. |





<hr />

<a id="credits"></a>
## Credits

- Node.js is a trademark or a registered trademark of OpenJS Foundation in the United States and other countries.

- JavaScript is a registered trademarks of Oracle and/or its affiliates. 

- ChatGPT is a trademark or a registered trademark of OpenAI OpCo, LLC in the United States and other countries.

- Other names may be either a registered trademarks or trademarks of their respective owners. 


