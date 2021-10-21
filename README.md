# Exevalator

Exevalator (an abbreviation for **Ex**pression-**Eval**u**ator**) is a compact interpreter embeddable in applications, for computing values of expressions.

&raquo; [Japanese README](./README_JAPANESE.md)


## English README Index
- <a href="#license">License</a>
- <a href="#requirements">Requirements</a>
- <a href="#how-to-use">How to Use</a>
- <a href="#how-to-use-example-code">How to Use Example Code</a>
- <a href="#related">Related Software</a>
- <a href="#about-us">About Us</a>


<a id="license"></a>
## License

This software is released under the MIT License.


<a id="requirements"></a>
## Requirements

* Java&reg; Development Kit (JDK) 8 or later


<a id="how-to-use"></a>
## How to Use

The interpreter of Exevalator is implemented in a single file "Exevalator.java", so you can import and use it in your project by very easy 3 steps!

### 1. Put into your source code folder

Firstly, put "Exevalator.java" into anywere in your source code folder, e.g.:

	src/your/projects/package/anywhere/Exevalator.java

### 2. Write the package statement

Secondly, the package statement at the top of the content of "Exevalator.java" as the following example:

	(in Exevalator.java)
	package your.projects.package.anywhere;

### 3. Import from your code and use

Now you are ready to use Exevalator! You can import the Exevalator from any your source, code and compute expressions as follows:

	...
	import your.projects.package.anywhere.Exevalator;
	...

	public class YourClass {
		...
		public void yourMethod() {
			
			// Create an interpreter for evaluating expressions
			Exevalator exevalator = new Exevalator();

			// Evaluate (compute) the value of an expression
			double result = exevalator.eval("1.2 + 3.4");
			
			// Display the result
			System.out.println("Result: " + result);
		}
		...
	}

Note that, all numbers in expressions will be handled as double-type values on Exevalator.
The result is also double-type.


<a id="how-to-use-example-code"></a>
## How to Use Example Code

Simple example code "Example*.java" are bundled in this repository. You can compile and run them as follows:

	javac Exevalator.java
	javac Example1.java
	java Example1

"Example1.java" is an example for computing the value of "1.2 + 3.4" by using Exevalator. Its code is almost the same as "YourClass" in the previous section. The result is:

	4.6

As the above, the computed value of "1.2 + 3.4" will be displayed.


<a id="related"></a>
## Related Software

Features of Exevalator is not so rich, because we prioritize to make the interpreter compact.

If you want more features, try to use the [Vnano](https://github.com/RINEARN/vnano) which is an script engine for embedded use in applications, instead of Exevalator.
Vnano can execute relatively complex expressions/scripts, containing conditional branches, loops, and so on.


<a id="about-us"></a>
## About Us

Exevalator is developed by a Japanese software development studio: [RINEARN](https://www.rinearn.com/). The author is [Fumihiro Matsui](https://fumihiro-matsui.xnea.net/).

Please free to contact us if you have any questions, feedbacks, and so on!


<a id="credits"></a>
## Credits

- Oracle and Java are registered trademarks of Oracle and/or its affiliates. 

- Other names may be either a registered trademarks or trademarks of their respective owners. 


