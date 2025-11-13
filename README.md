# Exevalator

![logo](logo.png)

Exevalator, an abbreviation for "**Ex**pression-**Eval**u**ator**," is a compact and high-speed interpreter that can be embedded in your programs or apps for computing the values of expressions.

Exevalator is currently available for programs and apps written in Java&trade;, Rust, C#, C++, Visual Basic&reg;, TypeScript, and Python.

In addition, Exevalator supports MCP and can be used as a calculation tool for AI agents.


&raquo; [Japanese README](./README_JAPANESE.md)

&raquo; [Official Website](https://www.rinearn.com/en-us/exevalator/)

&raquo; [Ask the AI for help (ChatGPT account required)](https://chatgpt.com/g/g-UjkEJeO8x-exevalator-assistant)

<hr />

## English README Index
- <a href="#what-is">What is Exevalator ?</a>
- <a href="#license">License (Public Domain)</a>
- <a href="#how-to-use">How to Use in Each Language</a>
	- <a href="#how-to-use-java">How to Use in Java</a>
	- <a href="#how-to-use-rust">How to Use in Rust</a>
	- <a href="#how-to-use-csharp">How to Use in C#</a>
	- <a href="#how-to-use-cpp">How to Use in C++</a>
	- <a href="#how-to-use-vb">How to Use in Visual Basic</a>
	- <a href="#how-to-use-typescript">How to Use in TypeScript</a>
	- <a href="#how-to-use-python">How to Use in Python</a>
	- <a href="#how-to-use-mcp">How to Use in MCP</a>
- <a href="#customize-error-languages">Localizing or Customizing Error Messages</a>
- <a href="#performance">Performance</a>
- <a href="#about-us">About Us</a>
- <a href="#references">References</a>


<a id="what-is"></a>
## What is Exevalator ?

Are you developing software and need to calculate the value of a numerical expression stored in a string-type variable? For example:

	"1 + 2"
	"(1.2 + 3.4) * 5.6"
	"x + f(y)"

Most compiled languages, as opposed to scripting languages, do not support this feature by default. You are usually required to implement your own calculation routine or use a library that provides this capability.

Exevalator is a very compact library designed to offer this functionality in Java, Rust, C#, C++, Visual Basic, and TypeScript programs.


<a id="license"></a>
## License (Public Domain)

This library is released under the "Unlicense," which is effectively equivalent to releasing it into the public domain.

Alternatively, you can opt for the [CC0](https://creativecommons.org/publicdomain/zero/1.0/deed.en) license, which is similar to the Unlicense in that it also facilitates releasing works into the public domain, though there are minor differences in the terms. Feel free to choose the one that best suits your needs.


<a id="how-to-use"></a>
## How to Use in Each Language

<a id="how-to-use-java"></a>
### How to Use in Java

In the "java" folder, you'll find the Java implementation of Exevalator, various example codes, and a [README for using in Java](./java/README.md). The simplest example code is "Example1.java", which computes the value of the expression "1.2 + 3.4" as follows:

	(in java/Example1.java)

	Exevalator exevalator = new Exevalator();
	double result = exevalator.eval("1.2 + 3.4");
	System.out.println("result: " + result);

To compile and run this code, follow these steps:

	cd java
	javac Exevalator.java
	javac Example1.java
	java Example1

The result is:

	result: 4.6

For more details, see the [README for using in Java](./java/README.md).


<a id="how-to-use-rust"></a>
### How to Use in Rust

In the "rust" folder, the Rust implementation of Exevalator, various example codes, and a [README for using in Rust](./rust/README.md) are provided. The simplest example code is "example1.rs", which computes the value of the expression "1.2 + 3.4" as follows:

	(in rust/example1.rs)

	let mut exevalator = Exevalator::new();
	let result: f64 = match exevalator.eval("1.2 + 3.4") {
		Ok(eval_value) => eval_value,
		Err(eval_error) => panic!("{}", eval_error),
	};
	println!("result: {}", result);

To compile and run this code, use the following commands:

	cd rust
	rustc example1.rs

	./example1
	  or
	example1.exe

The result is:

	result: 4.6

For more details, see the [README for using in Rust](./rust/README.md).


<a id="how-to-use-csharp"></a>
### How to Use in C#

In the "csharp" folder, the C# implementation of Exevalator, various example codes, and a [README for using in C#](./csharp/README.md) are available. The simplest example code is "Example1.cs", which computes the value of the expression "1.2 + 3.4" as follows:

	(in csharp/Example1.cs)

	Exevalator exevalator = new Exevalator();
	double result = exevalator.Eval("1.2 + 3.4");
	Console.WriteLine("result: " + result);

This code can be run in any project on Visual Studio&reg;, or in command lines. For command line execution, use the Developer Command Prompt provided with Visual Studio:

	cd csharp
	csc Example1.cs Exevalator.cs
	Example1.exe

The result is:

	result: 4.6

For more details, the see [README for using in C#](./csharp/README.md).



<a id="how-to-use-cpp"></a>
### How to Use in C++

In the "cpp" folder, the C++ implementation of Exevalator and various example codes, along with a [README for using in C++](./cpp/README.md), are provided. The simplest example code is "example1.cpp", which computes the value of the expression "1.2 + 3.4" as follows:

	(in cpp/example1.cpp)

	exevalator::Exevalator exevalator;
	try {
		double result = exevalator.eval("1.2 + 3.4");
		std::cout << "result: " << result << std::endl;
	} catch (exevalator::ExevalatorException &e) {
		std::cerr << e.what() << std::endl;
	}

To compile and run this code, depending on your environment and compiler, use:

	cd cpp
	clang++ -std=c++17 -Wall -o example1 example1.cpp
	./example1

The result is:

	result: 4.6

For more details, see the [README for using in C++](./cpp/README.md).



<a id="how-to-use-vb"></a>
### How to Use in Visual Basic

In the "vb" folder, the Visual Basic (VB.NET) implementation of Exevalator, various example codes, and a [README for using in Visual Basic](./vb/README.md) are provided. The simplest example code is "Example1.vb", which computes the value of the expression "1.2 + 3.4" as follows:

	(in vb/Example1.vb)

	Dim exevalator As Exevalator = New Exevalator()
	Dim result As Double = exevalator.Eval("1.2 + 3.4")
	Console.WriteLine("result: " + result.ToString())

This code can be run in any project on Visual Studio&reg;, or in command lines. For command line execution, use the Developer Command Prompt provided with Visual Studio:

	cd vb
	vbc Example1.vb Exevalator.vb
	Example1.exe

The result is:

	result: 4.6

For more details, see the [README for using in Visual Basic](./vb/README.md).


<a id="how-to-use-typescript"></a>
### How to Use in TypeScript

In the "typescript" folder, you'll find the TypeScript implementation of Exevalator, along with code examples and a [README for TypeScript](./typescript/README.md). The simplest example is 'example1.ts', which evaluates a basic expression "1.2 + 3.4" as follows:

	(in typescript/example1.ts)

	let exevalator: Exevalator = new Exevalator();
	const result: number = exevalator.eval("1.2 + 3.4");
	console.log(`result: ${result}`);

To compile and run this code:

	npx tsc example1.ts
	node example1.js

The result is:

	result: 4.6

If you want to run it in a web browser, you can easily bundle the code into a single JavaScript file using a bundler tool like esbuild and load it from an HTML file (e.g., 'example6.ts' &amp; 'example7.ts').

For more detailed explanations and a list of features, please refer to the [README for TypeScript](./typescript/README.md).


<a id="how-to-use-python"></a>
### How to Use in Python

In the "python" folder, you'll find the Python implementation of Exevalator, various example codes, and a [README for using in Python](./python/README.md). The simplest example code is "example1.py", which computes the value of the expression "1.2 + 3.4" as follows:

	(in python/Example1.py)

	ex = Exevalator()
	result = ex.eval("1.2 + 3.4")
	print(f"result: {result}")

To run this code:

	cd python
	python example1.py

The result is:

	result: 4.6

For more details, see the [README for using in Python](./python/README.md).


<a id="how-to-use-mcp"></a>
### Using with MCP

Exevalator supports MCP (Model Context Protocol) and can be used as a calculation tool for AI agents.
The `mcp` folder contains the code and an [README for MCP Use](./mcp/README.md)

In short: place the Exevalator package somewhere convenient (e.g., directly under your home), then set up the environment inside the `mcp` folder:

	# Create a place to keep MCP tools (here, under home)
	cd ~
	mkdir mcp-tools
	cd mcp-tools
	
	# Place the Exevalator package
	git clone https://github.com/RINEARN/exevalator.git

	# Set up the environment inside the mcp folder
	cd exevalator/mcp
	uv init -p 3.13 --name exevalator-mcp .
	uv add "mcp[cli]"

Next, in your AI environment (e.g., Visual Studio Code + Cline extension), 
open the MCP server settings file, append an entry like the following, and register it:

	{
		"mcpServers": {
			...
			// Existing MCP tool entries (if any)
			...
			"Exevalator": {
				"command": "uv",
				"args": [
					"--directory", "/home/your-user-name/mcp-tools/exevalator/mcp/",
					"run", "exevalator_mcp.py"
				]
			}
		}
	}

Once registered, try prompting your AI as follows:

	Using the Exevalator MCP server, evaluate "1.2 + 3.4".
	Do not use any other method. Make sure to use Exevalator.

If everything is working, you should see a reply like:

    I computed "1.2 + 3.4" via the `evaluate_expression` tool 
    on the `Exevalator` MCP server and confirmed the result is 4.6.
    As requested, the calculation was performed using `Exevalator`.

For more details and a list of features, see the [README for MCP Use](./mcp/README.md)



<a id="customize-error-languages"></a>
## Localizing or Customizing Error Messages

By default, error messages are shown in English. To switch them to Japanese, edit the source as follows:

* In the folder for your language-specific implementation, copy the contents of `ERROR_MESSAGES_JAPANESE.*` (`*` is the extension for that implementation).

* In the Exevalator core source file (`Exevalator.*` or `exevalator.*`), near the top, overwrite the ErrorMessage class (or structure, depending on the language) with the copied contents.

If you want to customize the messages, edit the ErrorMessage class (or structure) directly.



<a id="performance"></a>
## Performance

### Processing Speed of Exevalator

Exevalator is designed with applications in calculation and data analysis software in mind. Consequently, we have prioritized processing speed in its internal architecture design.

Exevalator is particularly efficient when evaluating the same expression repeatedly. For example:

	// Declare a variable "x" and get its address
	int varAddress = exevalator.declareVariable("x");

	// Loop to sum values of an expression while changing the value of "x"
	// (10 numerical operations per cycle)
	double result = 0.0;
	for (long i=1; i<=loops; ++i) {
		exevalator.writeVariableAt(varAddress, (double)i);
		result += exevalator.eval("x + 1 - 1 + 1 - 1 + 1 - 1 + 1 - 1 + 1 - 1");
	}

	(Refer to: java/Benchmark.java)

While the example above may seem trivial or practically useless, it serves to demonstrate how Exevalator handles repetitive numerical operations efficiently. Many practical calculation tasks follow a similar pattern, even though they perform meaningful computations.

In this code, the for-loop can process **tens of millions of cycles per second**, depending on your environment. Since each cycle includes 10 numerical operations, the effective processing speed reaches **several hundred MFLOPS**. We believe this speed is sufficient for a variety of tasks, including transforming array values and sampling curve coordinates from expressions, among others.

> Note: Performance varies significantly depending on the implementation language. "Hundreds of MFLOPS" is a reference level for implementations in compiled languages or scripting runtimes with a JIT.
> 
> By contrast, in the Python edition on a standard environment without those (i.e., CPython), throughput is typically 1-2 orders of magnitude lower (on the order of a few to about 10 MFLOPS).


### How to Tune Performance When Different Expressions Are Frequently Inputted

Exevalator achieves the previously mentioned processing speeds by caching the results of parsing and lexical analysis of previously inputted expressions. Therefore, if different expressions are frequently inputted into an instance of Exevalator, the cache does not work effectively. For example:

	...
	for (long i=0; i<loops; ++i) {
		exevalator.writeVariableAt(varAddress, (double)i);
		result += exevalator.eval("x + 1 - 1 + 1 - 1 + 1"); // Different from the next
		result += exevalator.eval("x - 1 + 1 - 1 + 1 - 1"); // Different from the previous
	}

In the code above, the for-loop runs at the speed of **some tens or hundreds of thousands of cycles per second**, which is about 100 times slower than the previous example.

You can avoid this kind of performance degradation by creating an independent instance of Exevalator for each expression:

	...
	for (long i=0; i<loops; ++i) {
		exevalatorA.writeVariableAt(varAddressA, (double)i);
		exevalatorB.writeVariableAt(varAddressB, (double)i);
		result += exevalatorA.eval("x + 1 - 1 + 1 - 1 + 1"); // Different from the next
		result += exevalatorB.eval("x - 1 + 1 - 1 + 1 - 1"); // Different from the previous
	}

In the scenario above, caches in each instance of Exevalator function effectively, enabling the code to run approximately 100 times faster than the previous code.


### If You Need More Speed, Consider Using the "reeval()" Method

When you need to re-evaluate the same expression that was previously evaluated using the "eval()" method, you can also use the "reeval()" method instead. The "reeval()" method allows Exevalator to skip checking for changes between the inputted expression and the cached expression, which in principle allows it to operate faster than the "eval()" method.

For instance, in the benchmark programs included in this repository, the "reeval()" method in the C++ version of Exevalator operates approximately 1.4 times faster than "eval()". For the Rust version, it is about 1.1 to 1.2 times faster. However, the "reeval()" method does not provide a significant advantage in the Java and C# versions.


<a id="about-us"></a>
## About Us

Exevalator is developed by a Japanese software development studio, [RINEARN](https://www.rinearn.com/). The author is Fumihiro Matsui.

Please feel free to contact us if you have any questions, feedback, or other inquiries!


<a id="references"></a>
## References

The following webpages may be useful if you need more information about Exevalator:

* [Official Website](https://www.rinearn.com/en-us/exevalator/)

* [Released "Exevalator": A Multilingual & Copyright-Free Expression Evaluator Library
](https://www.rinearn.com/en-us/info/news/2022/0416-exevalator) - RINEARN News 2022/04/16

* [The Internal Architecture of Exevalator](https://www.rinearn.com/en-us/info/news/2022/0504-exevalator-architecture) - RINEARN News 2022/05/04


<a id="credits"></a>
## Credits

- Oracle, Java, and JavaScript are registered trademarks of Oracle and/or its affiliates. 

- Rust is registered trademarks of Mozilla Foundation and/or its affiliates. 

- Microsoft Windows, C#, Visual Basic, .NET, and Visual Studio are either a registered trademarks or trademarks of Microsoft Corporation in the United States and/or other countries.

- Linux is a trademark of linus torvalds in the United States and/or other countries.

- Node.js is a trademark or a registered trademark of OpenJS Foundation in the United States and other countries.

- Python is a trademark or a registered trademark of Python Software Foundation in the United States and other countries.

- MCP (Model Context Protocol) is a communication protocol proposed by Anthropic, PBC.

- Visual Studio Code is either a registered trademarks or trademarks of Microsoft Corporation in the United States and/or other countries.

- Cline is an AI tool by Cline Bot Inc.

- uv is a Python package management tool by Astral.

- Git is a trademark or registered trademark of Software Freedom Conservancy, Inc. in the United States and/or other countries.

- ChatGPT is a trademark or a registered trademark of OpenAI OpCo, LLC in the United States and other countries.

- Other names may be either a registered trademarks or trademarks of their respective owners. 


