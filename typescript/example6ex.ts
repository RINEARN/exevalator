import Exevalator, {ExevalatorFunctionInterface, ExevalatorError} from "./exevalator";

/*
 * An example to compute the value of the inputted expression f(x) at the inputted x.
 *
 * Compared to example6, this example provides a more interactive user interface,
 * and some practical functions are supported: sin(x), cos(x), and tan(x).
 *
 * This code must be executed on web browsers, not on Node.js.
 * To run this code on web browsers, first install esbuild (if not already installed):
 *
 *     npm install esbuild --save-dev
 * 
 * Then, compile and bundle "example6ex.ts" and "exevalator.ts" into single JavasScript file by:
 * 
 *     npx esbuild example6ex.ts --bundle --outfile=example6ex.bundle.js
 * 
 * Then, open "example6ex.html" by your web browser.
 * It loads and execute "example6ex.bundle.js" generated above.
 */

// DOM Elements.
const expressionInput: HTMLInputElement = document.getElementById("expression")! as HTMLInputElement;
const xValueInput: HTMLInputElement = document.getElementById("xValue")! as HTMLInputElement;
const resultDiv: HTMLElement = document.getElementById("result")! as HTMLElement;
const calculateButton: HTMLButtonElement = document.getElementById("calculateButton")! as HTMLButtonElement;

// Functions available in expressions: sin(x), cos(x), and tan(x).
class SinFunction implements ExevalatorFunctionInterface {
    public invoke(args: number[]): number {
        if (args.length != 1) throw new ExevalatorError("Unexpected number of arguments.");
        return Math.sin(args[0]);
    }
}
class CosFunction implements ExevalatorFunctionInterface {
    public invoke(args: number[]): number {
        if (args.length != 1) throw new ExevalatorError("Unexpected number of arguments.");
        return Math.cos(args[0]);
    }
}
class TanFunction implements ExevalatorFunctionInterface {
    public invoke(args: number[]): number {
        if (args.length != 1) throw new ExevalatorError("Unexpected number of arguments.");
        return Math.tan(args[0]);
    }
}

// The event handler which is called when "Calculate" button is clicked.
calculateButton.addEventListener("click", () => {

    // Get the expression from the UI.
    const expression: string = expressionInput.value;

    // Get the value of x from the UI.
    const xValueStr: string = xValueInput.value.trim();
    const xValue: number = Number(xValueStr);
    if (xValueStr === "" || Number.isNaN(xValue)) {
        resultDiv.textContent = "The value of 'x' is invalid!";
        return;
    }

    // Create an instance of Exevalator Engine.
    let exevalator: Exevalator = new Exevalator();

    // Set the value of the variable 'x'.
    exevalator.declareVariable("x");
    exevalator.writeVariable("x", xValue);

    // Connect functions: sin(x), cos(x), and tan(x).
    exevalator.connectFunction("sin", new SinFunction());
    exevalator.connectFunction("cos", new CosFunction());
    exevalator.connectFunction("tan", new TanFunction());

    // Compute the value of the inputted expression.
    try {
        const result: number = exevalator.eval(expression);
        const roundedResult: string = result.toPrecision(10);
        resultDiv.textContent = `result: ${roundedResult}`;
    } catch (error) {
        resultDiv.textContent = "ERROR\n(See the browser's console for details.)";
        console.log(error);
    }
});
