# How to Use Exevalator in Rust&reg;

&raquo; [Japanese](./README_JAPANESE.md)


## English Index
- <a href="#requirements">Requirements</a>
- <a href="#how-to-use">How to Use</a>
- <a href="#example-code">Example Code</a>
- <a href="#features">Features</a>




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

As the above, the computed value of "1.2 + 3.4" will be displayed.


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



<a id="credits"></a>
## Credits

- Rust is registered trademarks of Mozilla Foundation and/or its affiliates 

- Microsoft Windows is either a registered trademarks or trademarks of Microsoft Corporation in the United States and/or other countries.

- Linux is a trademark of linus torvalds in the United States and/or other countries.

- Other names may be either a registered trademarks or trademarks of their respective owners. 


