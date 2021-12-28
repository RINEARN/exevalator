# How to Use Exevalator in Rust&reg;

&raquo; [Japanese](./README_JAPANESE.md)


## English Index
- [Requirements](#requirements)
- [How to Use](#how-to-use)
- [Example Code](#example-code)
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

The interpreter of Exevalator is implemented in a single file "rust/exevalator.rs", so you can use it by very easy 2-steps!

### 1. Put into your source code folder

If you want to load the Exevalator in the most easy way, locate "rust/exevalator.rs" into the root folder of source code folders. It is a single file so it does not make the directory messy.

If you want to locate "exevalator.rs" into non-root directory, it is necessary to do the module-declaration in the directory.


### 3. Load from your code and use

Then, you can load the Exevalator from any your source code and compute expressions as follows:

	mod exevalator;
	use exevalator::Exevalator;
	...

	// Create an interpreter for evaluating expressions
	let mut exevalator = Exevalator::new();

	// Evaluate (compute) the value of an expression
	let result: f64 = match exevalator.eval("1.2 + 3.4") {
		Ok(eval_value) => eval_value,
		Err(eval_error) => panic!("{}", eval_error),
	};

	// Display the result
	println!("result: {}", result);	

Note that, all numbers in expressions will be handled as f64-type values on Exevalator.
The evaluated (computed) value is always f64-type.
However, errors may occur depending the contents of expressions, so the return value of "eval" method is wrapped by Result<f64, ExevalatorError> type.


<a id="example-code"></a>
## Example Code

Simple example code "rust/example*.rs" are bundled in this repository. You can compile them as follows:

	cd rust
	rustc example1.rs

How to execute is, if you are using Linux&reg; and so on:

	./example1

If you are using Microsoft&reg; Windows&reg;:

	example1.exe

By the way, "example1.rs" is an example for computing the value of "1.2 + 3.4" by using Exevalator. Its code is almost the same as "YourClass" in the previous section:

	...
	let result: f64 = match exevalator.eval("1.2 + 3.4") {
		Ok(eval_value) => eval_value,
		Err(eval_error) => panic!("{}", eval_error),
	};

So the result is:

	result: 4.6

As the above, the computed value of "1.2 + 3.4" will be displayed. You can compile/run other examples in the same way.

Also, a benchmark program "rust/benchmark.rs" for measuring processing speed of Exevalator is bundled in this repository. For compiling it, specify the optimization-option as:

	rustc -C opt-level=3 benchmark.rs

In the above, "-C opt-level=3" is the specification of the level of the optimization. If you forget it, Exevalator can not work in full-speed.



<a id="features"></a>
## Features

The followings are main features of Exevalator.

### 1. Evaluate (Compute) Expressions

As shown in the previous sections, Exevalator can evaluate (compute) the value of an expression:

	let result: f64 = match exevalator.eval("(-(1.2 + 3.4) * 5) / 2") {
		Ok(eval_value) => eval_value,
		Err(eval_error) => panic!("{}", eval_error),
	};
	// result: -11.5

	(See: rust/example2.rs)

As the above, you can use "+" (addition), "-" (subtraction), "\*" (multiplication), and "/" (division) operators in an expression. Operations of "\*" and "/" are prioritized than "+" and "-".


### 2. Use Variables

You can use variables in expressions:

If you change the value of a variable very frequently, access to the variable by the address as follows:

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

This way works faster than accessings by the name.

### 3. Use Functions

You can define functions available in expressions. Functions having the signature **"fn function(arguments: Vec<f64>) -> Result<f64, ExevalatorError>"** is available. For example:

	/// The function available in expressions
	fn my_function(arguments: Vec<f64>) -> Result<f64, ExevalatorError> {
		if arguments.len() != 2 {
			return Err(ExevalatorError::new("Incorrect number of args"));
		}
		return Ok(arguments[0] + arguments[1]);
	} 

You can connect the above function to Exevalator, and use it in expressions:

	// Connect the above function to use it in expressions
	let address: usize = match exevalator.connect_function("fun", my_function) {
		Ok(connected_function_address) => connected_function_address,
		Err(connection_error) => panic!("{}", connection_error),
	};
	
	// Evaluate (compute) the value of an expression
	let result: f64 = match exevalator.eval("fun(1.2, 3.4)") {
		Ok(eval_value) => eval_value,
		Err(eval_error) => panic!("{}", eval_error),
	};
	// result: 4.6

	(See: rust/example5.rs)





<a id="methods"></a>
## List of Methods/Specifications

The list of methods of Exevalator struct, and their specifications.

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
| Description | Creates a new interpreter of the Exevalator. |
| Parameters | None |
| Return | The created instance. |


<a id="methods-eval"></a>
| Signature | fn eval(&amp;mut self, expression: &amp;str) -&gt; Result&lt;f64, ExevalatorError&gt; |
|:---|:---|
| Description | Evaluates (computes) the value of an expression. |
| Parameters | expression: The expression to be evaluated. |
| Return | Ok: The evaluated value.<br>Err: If any error occurred when evaluating the expression. |


<a id="methods-reeval"></a>
| Signature | double reeval() -&gt; Result&lt;f64, ExevalatorError&gt; |
|:---|:---|
| Description | Re-evaluates (re-computes) the value of the expression evaluated by "eval" method last time.<br>This method may (slightly) work faster than calling "eval" method repeatedly for the same expression.<br>Note that, the result value may different with the last evaluated value, if values of variables or behaviour of functions had changed. |
| Parameters | None |
| Return | Ok: The evaluated value.<br>Err: If any error occurred when evaluating the expression. |


<a id="methods-declare-variable"></a>
| Signature | fn declare_variable(&mut self, name: &amp;str) -&gt; Result&lt;usize, ExevalatorError&gt; |
|:---|:---|
| Description | Declares a new variable, for using the value of it in expressions. |
| Parameters | name: The name of the variable to be declared. |
| Return | Ok: The virtual address of the declared variable, which is useful for accessing to the variable faster (See "write_variable_at" and "read_variable_at" method).<br>Err: If invalid variable name is specified. |


<a id="methods-write-variable"></a>
| Signature | fn write_variable(&amp;mut self, name: &amp;str, value: f64) -&gt; Option&lt;ExevalatorError&gt; |
|:---|:---|
| Description | Writes the value to the variable having the specified name. |
| Parameters | name: The name of the variable to be written.<br>value: The new value of the variable. |
| Return | None: Normal result.<br>Some: If the specified variable is not found, or if invalid variable name is specified. |


<a id="methods-write-variable-at"></a>
| Signature | fn write_variable_at(&amp;mut self, address: usize, value: f64) -&gt; Option&lt;ExevalatorError&gt; |
|:---|:---|
| Description | Writes the value to the variable at the specified virtual address.<br>This method works faster than "write_variable" method. |
| Parameters | address: The virtual address of the variable to be written.<br>value: The new value of the variable. |
| Return | None: Normal result.<br>Some: If invalid address is specified. |


<a id="methods-read-variable"></a>
| Signature | fn read_variable(&amp;mut self, name: &amp;str) -&gt; Result&lt;f64, ExevalatorError&gt; |
|:---|:---|
| Description | Reads the value of the variable having the specified name. |
| Parameters | name: The name of the variable to be read. |
| Return | Ok: The current value of the variable.<br>Err: If the specified variable is not found, or if invalid variable name is specified. |


<a id="methods-read-variable-at"></a>
| Signature | fn read_variable_at(&amp;mut self, address: usize) -&gt; Result&lt;f64, ExevalatorError&gt; |
|:---|:---|
| Description | Reads the value of the variable at the specified virtual address.<br>This method works faster than "read_variable" method. |
| Parameters | address: The virtual address of the variable to be read. |
| Return | The current value of the variable. |
| Return | Ok: The current value of the variable.<br>Err: If the specified variable is not found, or if invalid variable name is specified. |


<a id="methods-connect-function"></a>
| Signature | fn connect_function(&amp;mut self, name: &amp;str, function_pointer: fn(Vec&lt;f64&gt;)->Result&lt;f64,ExevalatorError&gt;)) -&gt; Result&lt;usize, ExevalatorError&gt; |
|:---|:---|
| Description | Connects a function, for using it in expressions. |
| Parameters | name: The name of the function used in the expression.<br>function_pointer: The function to be connected. |
| Return | Ok: Unused in this version.<br>Err: If invalid function name is specified. |



<hr />

<a id="credits"></a>
## Credits

- Rust is registered trademarks of Mozilla Foundation and/or its affiliates 

- Microsoft Windows is either a registered trademarks or trademarks of Microsoft Corporation in the United States and/or other countries.

- Linux is a trademark of linus torvalds in the United States and/or other countries.

- Other names may be either a registered trademarks or trademarks of their respective owners. 


