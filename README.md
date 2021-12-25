# Exevalator

Exevalator (an abbreviation for **Ex**pression-**Eval**u**ator**) is a compact interpreter embeddable in your programs/apps, for computing values of expressions.
Exevalator is currently available for programs/apps written in Java&reg;, Rust&reg;, C#&reg;, and C++.


&raquo; [Japanese README](./README_JAPANESE.md)


## English README Index
- <a href="#license">License</a>
- <a href="#how-to-use">How to Use for Each Language</a>
	- <a href="#how-to-use-java">How to Use in Java</a>
	- <a href="#how-to-use-rust">How to Use in Rust</a>
	- <a href="#how-to-use-csharp">How to Use in C#</a>
	- <a href="#how-to-use-cpp">How to Use in C++</a>
- <a href="#performance">How to Tune Performance</a>
- <a href="#about-us">About Us</a>


<a id="license"></a>
## License

This software is released under the MIT License.



<a id="how-to-use"></a>
## How to Use for Each Language

<a id="how-to-use-java"></a>
### How to Use in Java

In "java" folder, the Java implementation version of Exevalator, various example code, and [README for using in Java](./java/README.md) are locating. Most simple example code is "Example1.java", which computes the value of a simple expression "1.2 + 3.4" as follows:

	(in java/Example1.java)

	Exevalator exevalator = new Exevalator();
	double result = exevalator.eval("1.2 + 3.4");
	System.out.println("Result: " + result);

How to compile/run this code is:

	cd java
	javac Exevalator.java
	javac Example1.java
	Example1

The result is:

	4.6

For more details, see [README for using in Java](./java/README.md).


<a id="how-to-use-rust"></a>
### How to Use in Rust

In "rust" folder, the Rust implementation version of Exevalator, various example code, and [README for using in Rust](./rust/README.md) are locating. Most simple example code is "example1.rs", which computes the value of a simple expression "1.2 + 3.4" as follows:

	(in rust/example1.rs)

	// Create an instance of Exevalator Engine
	let mut exevalator = Exevalator::new();

	// Evaluate the value of an expression
	let result: f64 = match exevalator.eval("1.2 + 3.4") {
		Ok(eval_value) => eval_value,
		Err(eval_error) => panic!("{}", eval_error),
	};

	// Display the result
	println!("result = {}", result);

How to compile/run this code is:

	cd rust
	rustc example1.rs

	./example1
	  or
	example1.exe

The result is:

	4.6

For more details, see [README for using in Rust](./rust/README.md).


<a id="how-to-use-csharp"></a>
### How to Use in C#

In "csharp" folder, the C# implementation version of Exevalator, various example code, and [README for using in C#](./csharp/README.md) are locating. Most simple example code is "Example1.cs", which computes the value of a simple expression "1.2 + 3.4" as follows:

	(in csharp/Example1.cs)

	Exevalator exevalator = new Exevalator();
	double result = exevalator.Eval("1.2 + 3.4");
	Console.WriteLine("Result: " + result);

You can run the above code by importing it from any project on Visual Studio&reg;, and you also can run it in command-lines. The latter is more simple way, so Let's do it here. At first, launch "Developer Command Prompt" which is bundled in Visual Studio (Start button > Visual Studio 20** > ... ). Next, "cd" into the folder of this repository. Then compile example code and run it as follows:

	cd csharp
	csc Exevalator.cs Example1.cs
	Example1.exe

The result is:

	4.6

For more details, see [README for using in C#](./csharp/README.md).



<a id="how-to-use-cpp"></a>
### How to Use in C++

In "cpp" folder, the C++ implementation version of Exevalator and various example code are locating. Most simple example code is "example1.cpp", which computes the value of a simple expression "1.2 + 3.4" as follows:

	(in cpp/example1.cpp)

	exevalator::Exevalator exevalator;
	try {
		double result = exevalator.eval("1.2 + 3.4");
		std::cout << "Result: " << result << std::endl;
	} catch (exevalator::ExevalatorException &e) {
		std::cerr << e.what() << std::endl;
	}

How to compile/run this code depends on your environment and compiler. As an example:

	(If you are using clang++ on Linux)

	cd cpp
	clang++ -std=c++17 -Wall -o example1 example1.cpp
	./example1

The result is:

	4.6




<a id="performance"></a>
## How to Tune Performance

One of the assumed use of Exevalator is calculation/data-analysis software, so when we designed internal architecture of Exevalator, we placed importance on processing speed.

Especially, Exevalator works in high speed when evaluating (computing) the same expression repeatedly. For example:

	// Declate an variable "x" and get the address
	int varAddress = exevalator.declareVariable("x");

	// Loop taking sum of values of an expression, with changing the value of "x"
	// (10 numerical operations per cycle)
	double result = 0.0;
	for (long i=0; i<loops; ++i) {
		exevalator.writeVariableAt(varAddress, (double)i);
		result += exevalator.eval("x + 1 - 1 + 1 - 1 + 1 - 1 + 1 - 1 + 1 - 1");
	}

The above code is practically useless, but there are many practical calculation code having similar pattern everywhere.

The for-loop in the above code runs in the speed of **some tens of millions of cycles per second** (depending on your environment). In the above loop, 10 numerical operations are performed for each cycle, so the operating speed is **some hundreds of MFLOPS**. We think that this speed is enough for converting values of arrays, or sampling coordinates of curves of expressions, and so on.

Exevalator realizes the above processing speed by caching results of parsing and lexical-analysis of the previously-inputted expression. Hence, if an different expressions are inputted into an instance of Exevalator frequently, the cache does not work effective. For example:

	...
	for (long i=0; i<loops; ++i) {
		exevalator.writeVariableAt(varAddress, (double)i);
		result += exevalator.eval("x + 1 - 1 + 1 - 1 + 1"); // Different with the below
		result += exevalator.eval("x - 1 + 1 - 1 + 1 - 1"); // Different with the above
	}

In the above code, the for-loop runs in the speed of **some tens/hundreds of thousands of times cycles second**. It is about 100 times slower than the previous example code.

You can avoid this kind of performance-down by creating an independent instance of Exevalator for each expression:

	...
	for (long i=0; i<loops; ++i) {
		exevalatorA.writeVariableAt(varAddressA, (double)i);
		exevalatorB.writeVariableAt(varAddressB, (double)i);
		result += exevalatorA.eval("x + 1 - 1 + 1 - 1 + 1"); // Different with the below
		result += exevalatorB.eval("x - 1 + 1 - 1 + 1 - 1"); // Different with the above
	}

In the above code, caches in each instances of Exevalator work well, so it runs about 100 times faster than the previous code.



<a id="about-us"></a>
## About Us

Exevalator is developed by a Japanese software development studio: [RINEARN](https://www.rinearn.com/). The author is [Fumihiro Matsui](https://fumihiro-matsui.xnea.net/).

Please free to contact us if you have any questions, feedbacks, and so on!


<a id="credits"></a>
## Credits

- Oracle and Java are registered trademarks of Oracle and/or its affiliates. 

- Rust is registered trademarks of Mozilla Foundation and/or its affiliates. 

- Microsoft Windows, C#, and Visual Studio are either a registered trademarks or trademarks of Microsoft Corporation in the United States and/or other countries.

- Linux is a trademark of linus torvalds in the United States and/or other countries.

- Other names may be either a registered trademarks or trademarks of their respective owners. 


