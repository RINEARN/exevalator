import Exevalator from "./exevalator";

/*
 * An example to use various operators and parentheses.
 */

// Create an instance of Exevalator Engine
let exevalator: Exevalator = new Exevalator();

// Evaluate the value of an expression
const result: number = exevalator.eval("(-(1.2 + 3.4) * 5) / 2");

// Display the result
console.log(`result: ${result}`);
