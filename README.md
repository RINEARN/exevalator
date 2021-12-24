# Exevalator

Exevalator (an abbreviation for **Ex**pression-**Eval**u**ator**) is a compact interpreter embeddable in applications, for computing values of expressions.

&raquo; [Japanese README](./README_JAPANESE.md)


## English README Index
- <a href="#license">License</a>
- <a href="#how-to-use">How to Use for Each Language</a>
	- <a href="#how-to-use-java">How to Use in Java&reg;</a>
- <a href="#about-us">About Us</a>


<a id="license"></a>
## License

This software is released under the MIT License.



<a id="how-to-use"></a>
## How to Use for Each Language

<a id="how-to-use-java"></a>
### 1. How to Use in Java&reg;

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


<a id="about-us"></a>
## About Us

Exevalator is developed by a Japanese software development studio: [RINEARN](https://www.rinearn.com/). The author is [Fumihiro Matsui](https://fumihiro-matsui.xnea.net/).

Please free to contact us if you have any questions, feedbacks, and so on!


<a id="credits"></a>
## Credits

- Oracle and Java are registered trademarks of Oracle and/or its affiliates. 

- Other names may be either a registered trademarks or trademarks of their respective owners. 


