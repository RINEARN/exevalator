import Exevalator from "./exevalator";

/*
 * An example to access a variable by its address.
 */

// Create an instance of Exevalator Engine
let exevalator: Exevalator = new Exevalator();

// Declare a variable and set the value
const address: number = exevalator.declareVariable("x");
exevalator.writeVariableAt(address, 1.25);
// The above works faster than:
//    exevalator.writeVariable("x", 1.25);

// Evaluate the value of an expression
const result: number = exevalator.eval("x + 1");

// Display the result
console.log(`result: ${result}`);
