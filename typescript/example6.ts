import Exevalator from "./exevalator";

/*
 * An example to compute the value of the inputted expression f(x) at the inputted x.
 *
 * This code must be executed on web browsers, not on Node.js.
 * To run this code on web browsers, first install esbuild (if not already installed):
 *
 *     npm install esbuild --save-dev
 * 
 * Then, compile and bundle "example6.ts" and "exevalator.ts" into single JavasScript file by:
 * 
 *     npx esbuild example6.ts --bundle --outfile=example6.bundle.js
 * 
 * Then, open "example6.html" by your web browser.
 * It loads and execute "example6.bundle.js" generated above.
 */

// Get the expression from the user
const defaultExpression: string = "3*x*x + 2*x + 1";
let promptMessage: string = "This program computes the value of f(x) at x.\n"
        + `f(x) = ?      (default: ${defaultExpression})`
let expression: string | null = window.prompt(promptMessage, defaultExpression);
if (expression === null) {
    expression = defaultExpression;
}

// Get the value of x from the user
const defaultX: string = "1";
promptMessage = "x = ?      (default: 1)"
let xValueStr: string | null = window.prompt(promptMessage, defaultX);
if (xValueStr === null) {
    xValueStr = defaultX;
}
const xValue: number = Number(xValueStr);

// Create an instance of Exevalator Engine
let exevalator: Exevalator = new Exevalator();

// Set the value of x
exevalator.declareVariable("x");
exevalator.writeVariable("x", xValue);

// Compute the value of the inputted expression
const result: number = exevalator.eval(expression);

// Display the result
console.log("----------");
console.log(`f(x)   = ${expression}`);
console.log(`x      = ${xValue}`);
console.log(`result = ${result}`);
window.alert(`result = ${result}`);
