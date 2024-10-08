import Exevalator, { FunctionInterface, ExevalatorError } from "./exevalator";

/*
 * An example to create a function for available in expressions.
 */

/**
 * Function available in expressions.
 */
class MyFunction implements FunctionInterface {

    /**
     * Invoke the function.
     * 
     * @param arguments An array storing values of arguments.
     * @return The return value of the function.
     */
    public invoke(args: number[]): number {
        if (arguments.length != 2) {
            throw new ExevalatorError("Incorrected number of args");
        }
        return args[0] + args[1];
    }
}

// Create an instance of Exevalator Engine
let exevalator: Exevalator = new Exevalator();

// Connects the function available for using it in expressions
const fun: MyFunction = new MyFunction();
exevalator.connectFunction("fun", fun);

// Evaluate the value of an expression
const result: number = exevalator.eval("fun(1.2, 3.4)");

// Display the result
console.log(`result: ${result}`);
