import Exevalator from "./exevalator";

/*
 * A simple example to use Exevalator.
 */

// Create an instance of Exevalator Engine
let exevalator: Exevalator = new Exevalator();

// Evaluate the value of an expression
const result: number = exevalator.eval("1.2 + 3.4");

// Display the result
console.log(`result: ${result}`);
