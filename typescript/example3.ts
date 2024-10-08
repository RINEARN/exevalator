import Exevalator from "./exevalator";

/*
 * An example to use a variable.
 */

// Create an instance of Exevalator Engine
let exevalator: Exevalator = new Exevalator();

// Declare a variable and set the value
exevalator.declareVariable("x");
exevalator.writeVariable("x", 1.25);

// Evaluate the value of an expression
const result: number = exevalator.eval("x + 1");

// Display the result
console.log(`result: ${result}`);
