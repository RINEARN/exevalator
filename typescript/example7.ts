import Exevalator from "./exevalator";

/*
 * An example to compute the numerical integration value of the inputted expression f(x).
 * For details of the numerical integration algorithm used in this code, see:
 *   https://www.vcssl.org/en-us/code/archive/0001/7800-vnano-integral-output/   (English)
 *   https://www.vcssl.org/ja-jp/code/archive/0001/7800-vnano-integral-output/   (Japanese)
 *
 * This code must be executed on web browsers, not on Node.js.
 * To run this code on web browsers, first install esbuild (if not already installed):
 *
 *     npm install esbuild --save-dev
 * 
 * Then, compile and bundle "example7.ts" and "exevalator.ts" into single JavasScript file by:
 * 
 *     npx esbuild example7.ts --bundle --outfile=example7.bundle.js
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

// Get the value of the lower limit from the user
const defaultLowerLimit: string = "0";
promptMessage = "lower-limit = ?      (default: 0)"
let lowerLimitStr: string | null = window.prompt(promptMessage, defaultLowerLimit);
if (lowerLimitStr === null) {
    lowerLimitStr = defaultLowerLimit;
}
const lowerLimit: number = Number(lowerLimitStr);

// Get the value of the upper limit from the user
const defaultUpperLimit: string = "1";
promptMessage = "upper-limit = ?      (default: 1)"
let upperLimitStr: string | null = window.prompt(promptMessage, defaultUpperLimit);
if (upperLimitStr === null) {
    upperLimitStr = defaultUpperLimit;
}
const upperLimit: number = Number(upperLimitStr);

// Other numerical integration parameters
const numberOfSteps: number = 65536;
const delta: number = (upperLimit - lowerLimit) / numberOfSteps;
let result: number = 0.0;

// Create an instance of Exevalator Engine
let exevalator: Exevalator = new Exevalator();
const xAddress: number = exevalator.declareVariable("x");

// Traverse tiny intervals from lower-limit to upper-limit
for (let i: number = 0; i < numberOfSteps; i++) {

    // The x-coordinate value of the left-bottom point of i-th tiny interval
    const x: number = lowerLimit + i * delta;

    // Compute area of i-th tiny interval approximately by using Simpson's rule,
    // and add it to the variable "result"
    // (see: https://www.vcssl.org/en-us/code/archive/0001/7800-vnano-integral-output/ )

    exevalator.writeVariableAt(xAddress, x);
    const fxLeft: number = exevalator.eval(expression);

    exevalator.writeVariableAt(xAddress, x + delta);
    const fxRight: number = exevalator.eval(expression);

    exevalator.writeVariableAt(xAddress, x + delta/2.0);
    const fxCenter: number = exevalator.eval(expression);

    result += (fxLeft + fxRight + 4.0 * fxCenter) * delta / 6.0;
}

// Display the result
console.log("----------");
console.log(`f(x)        = ${expression}`);
console.log(`lower-limit = ${lowerLimit}`);
console.log(`upper-limit = ${upperLimit}`);
console.log(`result      = ${result}`);
window.alert(`result = ${result}`);
