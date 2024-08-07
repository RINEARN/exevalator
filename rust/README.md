# How to Use Exevalator in Rust

&raquo; [Japanese](./README_JAPANESE.md)


## Table of Contents
- [Requirements](#requirements)
- [How to Use](#how-to-use)
- [How to Compile and Run Example Code](#example-code)
- [Features](#features)
- [List of Methods/Specifications](#methods)
	- [Constructor](#methods-constructor)
	- [fn eval(&amp;mut self, expression: &amp;str) -&gt; Result&lt;f64, ExevalatorError&gt;](#methods-eval)
	- [fn reeval(&mut self) -&gt; Result&lt;f64, ExevalatorError&gt;](#methods-reeval)
	- [fn declare_variable(&mut self, name: &amp;str) -&gt; Result&lt;usize, ExevalatorError&gt;](#methods-declare-variable)
	- [fn write_variable(&amp;mut self, name: &amp;str, value: f64) -&gt; Option&lt;ExevalatorError&gt;](#methods-write-variable)
	- [fn write_variable_at(&amp;mut self, address: usize, value: f64) -&gt; Option&lt;ExevalatorError&gt;](#methods-write-variable-at)
	- [fn read_variable(&amp;mut self, name: &amp;str) -&gt; Result&lt;f64, ExevalatorError&gt;](#methods-read-variable)
	- [fn read_variable_at(&amp;mut self, address: usize) -&gt; Result&lt;f64, ExevalatorError&gt;](#methods-read-variable-at)
	- [fn connect_function(&amp;mut self, name: &amp;str, function_pointer: fn(Vec&lt;f64&gt;)->Result&lt;f64,ExevalatorError&gt;)) -&gt; Result&lt;usize, ExevalatorError&gt;](#methods-connect-function)




<a id="requirements"></a>
## Requirements

* rustc 1.57.0 or later (recommended)



<a id="how-to-use"></a>
## How to Use

Exevalator is an interpreter, compactly implemented in a single file, "rust/exevalator.rs." This streamlined design allows for easy integration in just two steps:


### 1. Place into Your Source Code Folder

For the simplest setup, place "exevalator.rs" in the root directory of your source code folders. As it is a single file, it won’t clutter your directory.

If you prefer to place "exevalator.rs" in a non-root directory, you will need to declare it as a module in that directory.


### 2. Load from Your Code and Use

You can then load Exevalator from any source file and compute expressions as follows:

	mod exevalator;
	use exevalator::Exevalator;
	...

	// Create an interpreter for evaluating expressions
	let mut exevalator = Exevalator::new();

	// Evaluate the value of an expression
	let result: f64 = match exevalator.eval("1.2 + 3.4") {
		Ok(eval_value) => eval_value,
		Err(eval_error) => panic!("{}", eval_error),
	};

	// Display the result
	println!("result: {}", result);	

Note that all numbers in expressions are handled as f64-type values in Exevalator. The evaluated value is always of type f64. However, since errors can occur based on the content of the expressions, the return value of the eval method is wrapped in a Result&lt;f64, ExevalatorError&gt; type.



<a id="example-code"></a>
## How to Compile and Run Example Code

Simple example code files such as "rust/example*.rs" are included in this repository. You can compile them using the following commands:

	cd rust
	rustc example1.rs

To execute the compiled program, use the following command on Linux&reg;:

	./example1

Or, on Microsoft® Windows®:

	example1.exe

"example1.rs" provides an example of computing the value "1.2 + 3.4" using Exevalator. Its code is similar to "YourClass" in the previous section:

	...
	let result: f64 = match exevalator.eval("1.2 + 3.4") {
		Ok(eval_value) => eval_value,
		Err(eval_error) => panic!("{}", eval_error),
	};

The result is:

	result: 4.6

This shows the computed value of "1.2 + 3.4". You can compile and run other examples in the same way.

Additionally, a benchmark program "rust/benchmark.rs" for measuring the processing speed of Exevalator is also included. To compile it, use the optimization option as shown:

	rustc -C opt-level=3 benchmark.rs

Here, "-C opt-level=3" specifies the optimization level. Omitting this may result in Exevalator not performing at full speed.



<a id="features"></a>
## Features

The following are the main features of Exevalator:

### 1. Evaluate (Compute) Expressions

As demonstrated in previous sections, Exevalator can evaluate the value of an expression:

	let result: f64 = match exevalator.eval("(-(1.2 + 3.4) * 5) / 2") {
		Ok(eval_value) => eval_value,
		Err(eval_error) => panic!("{}", eval_error),
	};
	// result: -11.5

	(See: rust/example2.rs)

You can use operators such as "+" (addition), "-" (subtraction and unary-minus operation), "\*" (multiplication), and "/" (division) in expressions. Multiplications and divisions are prioritized over additions and subtractions in the order of operations.


### 2. Use Variables

You can declare variables programmatically using Exevalator's "declareVariable" method during application development, allowing these variables to be freely used within expressions:

	// Declare a new variable and set the value
	match exevalator.declare_variable("x") {
		Ok(address) => address,
		Err(declaration_error) => panic!("{}", declaration_error)
	};
	match exevalator.write_variable("x", 1.25) {
		Some(access_error) => panic!("{}", access_error),
		None => {},
	};

	// Evaluate (compute) the value of an expression
	let result: f64 = match exevalator.eval("x + 1") {
		Ok(eval_value) => eval_value,
		Err(eval_error) => panic!("{}", eval_error),
	};
	// result: 2.25

	(See: rust/example3.rs)

For more frequent variable value updates, access the variable by address for faster performance:

	// Declare a new variable and set the value
    let address: usize = match exevalator.declare_variable("x") {
        Ok(declared_var_address) => declared_var_address,
        Err(declaration_error) => panic!("{}", declaration_error),
    };
    match exevalator.write_variable_at(address, 1.25) {
        Ok(_) => {},
        Err(access_error) => panic!("{}", access_error),
    };
	...

	(See: java/Example4.rs)

Access by address is quicker than by name.


### 3. Use Functions

You can create and use functions within expressions. Functions must have the signature **"fn function(arguments: Vec<f64>) -&gt; Result&lt;f64, ExevalatorError&gt;"** to be compatible. For example:

	/// The function available in expressions
	fn my_function(arguments: Vec<f64>) -> Result<f64, ExevalatorError> {
		if arguments.len() != 2 {
			return Err(ExevalatorError::new("Incorrect number of args"));
		}
		return Ok(arguments[0] + arguments[1]);
	} 

**CAUTION: In Ver.1.0, values of arguments passed from expressions were stored in the "arguments: Vec&lt;f64&gt;" array in reversed order. This behavior has been corrected in Ver.2.0. For details, please see issue #2.**

You can connect this function to Exevalator and use it in expressions as follows:

	// Connect the above function for use in expressions
	let address: usize = match exevalator.connect_function("fun", my_function) {
		Ok(connected_function_address) => connected_function_address,
		Err(connection_error) => panic!("{}", connection_error),
	};
	
	// Evaluate the value of an expression
	let result: f64 = match exevalator.eval("fun(1.2, 3.4)") {
		Ok(eval_value) => eval_value,
		Err(eval_error) => panic!("{}", eval_error),
	};
	// result: 4.6

	(See: rust/example5.rs)

This setup allows developers to programmatically define and integrate custom functions, enabling users to seamlessly utilize these functions within their expressions.





<a id="methods"></a>
## List of Methods/Specifications

Here are the methods of the Exevalator struct, along with their specifications:

- [Constructor](#methods-constructor)
- [fn eval(&amp;mut self, expression: &amp;str) -&gt; Result&lt;f64, ExevalatorError&gt;](#methods-eval)
- [fn reeval(&mut self) -&gt; Result&lt;f64, ExevalatorError&gt;](#methods-reeval)
- [fn declare_variable(&mut self, name: &amp;str) -&gt; Result&lt;usize, ExevalatorError&gt;](#methods-declare-variable)
- [fn write_variable(&amp;mut self, name: &amp;str, value: f64) -&gt; Option&lt;ExevalatorError&gt;](#methods-write-variable)
- [fn write_variable_at(&amp;mut self, address: usize, value: f64) -&gt; Option&lt;ExevalatorError&gt;](#methods-write-variable-at)
- [fn read_variable(&amp;mut self, name: &amp;str) -&gt; Result&lt;f64, ExevalatorError&gt;](#methods-read-variable)
- [fn read_variable_at(&amp;mut self, address: usize) -&gt; Result&lt;f64, ExevalatorError&gt;](#methods-read-variable-at)
- [fn connect_function(&amp;mut self, name: &amp;str, function_pointer: fn(Vec&lt;f64&gt;)->Result&lt;f64,ExevalatorError&gt;)) -&gt; Result&lt;usize, ExevalatorError&gt;](#methods-connect-function)


<a id="methods-constructor"></a>
| Signature | (constructor) Exevalator() |
|:---|:---|
| Description | Creates a new Exevalator interpreter instance. |
| Parameters | None |
| Return | The newly created instance. |


<a id="methods-eval"></a>
| Signature | fn eval(&amp;mut self, expression: &amp;str) -&gt; Result&lt;f64, ExevalatorError&gt; |
|:---|:---|
| Description | Evaluates the value of an expression. |
| Parameters | expression: The expression to be evaluated. |
| Return | Ok: The evaluated value.<br>Err: An error if any occurs during evaluation. |


<a id="methods-reeval"></a>
| Signature | double reeval() -&gt; Result&lt;f64, ExevalatorError&gt; |
|:---|:---|
| Description | Re-evaluates the value of the last evaluated expression.<br>This method may work slightly faster than repeatedly calling "eval" for the same expression.<br>Note that the result may differ from the last evaluated value if the values of variables or behavior of functions have changed. |
| Parameters | None |
| Return | Ok: The re-evaluated value.<br>Err: An error if any occurs during evaluation. |


<a id="methods-declare-variable"></a>
| Signature | fn declare_variable(&mut self, name: &amp;str) -&gt; Result&lt;usize, ExevalatorError&gt; |
|:---|:---|
| Description | Declares a new variable to be used in expressions. |
| Parameters | name: The name of the variable to be declared. |
| Return | Ok: The virtual address of the declared variable for faster access.<br>Err: An error if an invalid variable name is specified. |


<a id="methods-write-variable"></a>
| Signature | fn write_variable(&amp;mut self, name: &amp;str, value: f64) -&gt; Result&lt;(), ExevalatorError&gt; |
|:---|:---|
| Description | Writes a value to the specified variable. |
| Parameters | name: The name of the variable to be written.<br>value: The new value for the variable. |
| Return | Ok: Successfully written.<br>Err: An error if the specified variable is not found or the name is invalid. |


<a id="methods-write-variable-at"></a>
| Signature | fn write_variable_at(&amp;mut self, address: usize, value: f64) -&gt; Result&lt;(), ExevalatorError&gt; |
|:---|:---|
| Description | Writes a value to a variable at the specified virtual address, faster than using "write_variable". |
| Parameters | address: The virtual address of the variable.<br>value: The new value for the variable. |
| Return | Ok: Successfully written.<br>Err: An error if an invalid address is specified. |


<a id="methods-read-variable"></a>
| Signature | fn read_variable(&amp;mut self, name: &amp;str) -&gt; Result&lt;f64, ExevalatorError&gt; |
|:---|:---|
| Description | Reads the current value of the specified variable. |
| Parameters | name: The name of the variable. |
| Return | Ok: The current value of the variable.<br>Err: An error if the variable is not found or the name is invalid. |


<a id="methods-read-variable-at"></a>
| Signature | fn read_variable_at(&amp;mut self, address: usize) -&gt; Result&lt;f64, ExevalatorError&gt; |
|:---|:---|
| Description | Reads the current value of a variable at the specified virtual address, faster than using "read_variable". |
| Parameters | address: The virtual address of the variable. |
| Return | Ok: The current value of the variable.<br>Err: An error if an invalid address is specified. |


<a id="methods-connect-function"></a>
| Signature | fn connect_function(&amp;mut self, name: &amp;str, function_pointer: fn(Vec&lt;f64&gt;)->Result&lt;f64,ExevalatorError&gt;)) -&gt; Result&lt;usize, ExevalatorError&gt; |
|:---|:---|
| Description | Connects a function to be used in expressions. |
| Parameters | name: The name of the function used in expressions.<br>function_pointer: The function to be connected. |
| Return | Ok: Unused in this version..<br>Err: An error if an invalid function name is specified. |



<hr />

<a id="credits"></a>
## Credits

- Rust is registered trademarks of Mozilla Foundation and/or its affiliates 

- Microsoft Windows is either a registered trademarks or trademarks of Microsoft Corporation in the United States and/or other countries.

- Linux is a trademark of linus torvalds in the United States and/or other countries.

- Other names may be either a registered trademarks or trademarks of their respective owners. 


