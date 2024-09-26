# Exevalator (Official Website Frontpage)

Exevalator, an abbreviation for "Expression-Evaluator," is a compact and high-speed interpreter that can be embedded in your programs or apps for computing the values of expressions. Exevalator is currently available for programs and apps written in Java&trade;, Rust, C#, C++, and Visual Basic&reg;.


## An Expression Calculator Library

Exevalator is a library designed to calculate the values of expressions provided as strings in your software applications.

For example, with Exevalator, you can compute expressions like "1 + 2" or "x + f(y)" within your apps.

For more details, please refer to the following article:


* [Released "Exevalator": A Multilingual & Copyright-Free Expression Evaluator Library (News, 2022/04/16)](https://www.rinearn.com/en-us/info/news/2022/0416-exevalator): This introductory article was published at the launch of Exevalator. It explains the concept, features, and usage of the library.

## Flexible Use of Variables and Functions

You can define as many variables and functions as you need within your expressions, and use parentheses to structure them as necessary. For example:

    ((x + 1.2) * 3.4) / 5.6 + f( g(x + y) / (z - 1.23) )

Thus, Exevalator is capable of calculating complex expressions like the one above.

## High Speed Calculation

Exevalator can process tens to hundreds of thousands of expressions per second, even when a different expression is inputted for each calculation.

Furthermore, when recalculating the same expression with varying variable values, Exevalator can handle up to tens of millions of expressions per second.

This means that, assuming each expression contains approximately 10 operators (such as addition and subtraction), Exevalator can achieve a maximum processing speed of several hundred million floating-point operations per second (MFLOPS).

## Multilingual Support

Exevalator is compatible with software and applications written in Java, C++, C#, Rust, and Visual Basic.

The source code of Exevalator is compact and easily portable, which means we may extend support to other major programming languages in the future.

## Easy to Start Using

Exevalator consists of just one source file, making it exceptionally simple to introduce to your application.
(Note: The C++ version includes an additional header file, totaling two files.)

You can start using Exevalator by simply placing the appropriate source file into the source code folder of the application you are developing.

## Open Source & Copyright Free

Exevalator is an open-source library. You are free to choose between the CC0 and the Unlicense, both of which effectively place the software in the public domain.

As such, you can freely customize or repurpose the source code of Exevalator. For guidance, you might find the following article helpful:

* [The Internal Architecture of Exevalator (News, 2022/05/04)](https://www.rinearn.com/en-us/info/news/2022/0504-exevalator-architecture): This article details the internal architecture of Exevalator.

## Download

Experience Exevalator for yourself!

* [Download](https://github.com/RINEARN/exevalator/releases)


---

\- Trademarks and Credits -

* Oracle and Java are registered trademarks of Oracle and/or its affiliates.
* C# and Visual Basic are either registered trademarks or trademarks of Microsoft Corporation in the United States and/or other countries.
* Rust is registered trademarks of Mozilla Foundation and/or its affiliates.
* Other names may be either a registered trademarks or trademarks of their respective owners.


