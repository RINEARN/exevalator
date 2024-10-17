import Exevalator, { ExevalatorFunctionInterface, ExevalatorError } from "./exevalator";

/** The minimum error between two double-type values to regard them almost equal. */
const ALLOWABLE_ERROR: number = 1.0E-12;


/**
 * Called from the end of this script, after declaring all objects.
 */
function main(): void {
    testNumberLiterals();
    testOperationsOfOperators();
    testPrecedencesOfOperators();
    testParentheses();
    testComplicatedCases();
    testSyntaxChecksOfCorresponencesOfParentheses();
    testSyntaxChecksOfLocationsOfOperatorsAndLeafs();
    testVariables();
    testFunctions();
    testEmptyExpressions();
    testReeval();
    testTokenizations();
    testAddressFiltering();

    console.log("All tests have completed successfully.");
}


/**
 * The Error class thrown when any test has failed.
 */
class ExevalatorTestError extends Error {
    constructor(message: string) {
        super(message);
        this.name = "ExevalatorTestError";
    }
}

/**
 * Checks the evaluated (computed) value of the testing expression by the Exevalator.
 *
 * @param evaluatedValue The evaluated (computed) value of the testing expression by Exevalator
 * @param correctValue The correct value of the testing expression
 * @param testName The name of the testing
 */
function check(testName: string, evaluatedValue: number, correctValue: number) {
    if (Math.abs(evaluatedValue - correctValue) < ALLOWABLE_ERROR) {
        console.log(testName + ": OK.");
        return;
    }
    throw new ExevalatorTestError(
        `'${testName}' has failed. evaluatedValue=${evaluatedValue}, correctValue=${correctValue}`
    );
}


function testNumberLiterals() {
    let exevalator: Exevalator = new Exevalator();

    check(
        "Test of a Simple Number Literal 1",
        exevalator.eval("1"),
        1
    );

    check(
        "Test of a Simple Number Literal 2",
        exevalator.eval("2"),
        2
    );

    check(
        "Test of a Simple Number Literal 3",
        exevalator.eval("1.2"),
        1.2
    );

    check(
        "Test of a Number Literal with a Exponent Part 1",
        exevalator.eval("1.2E3"),
        1.2E3
    );

    check(
        "Test of a Number Literal with a Exponent Part 2",
        exevalator.eval("1.2E+3"),
        1.2E3
    );

    check(
        "Test of a Number Literal with a Exponent Part 3",
        exevalator.eval("1.2E-3"),
        1.2E-3
    );

    check(
        "Test of a Number Literal with a Exponent Part 4",
        exevalator.eval("123.4567E12"),
        123.4567E12
    );

    check(
        "Test of a Number Literal with a Exponent Part 5",
        exevalator.eval("123.4567E+12"),
        123.4567E+12
    );

    check(
        "Test of a Number Literal with a Exponent Part 6",
        exevalator.eval("123.4567E-12"),
        123.4567E-12
    );
}



function testOperationsOfOperators() {
    let exevalator: Exevalator = new Exevalator();

    check(
        "Test of Addition Operator",
        exevalator.eval("1.2 + 3.4"),
        1.2 + 3.4
    );

    check(
        "Test of Subtraction Operator",
        exevalator.eval("1.2 - 3.4"),
        1.2 - 3.4
    );

    check(
        "Test of Multiplication Operator",
        exevalator.eval("1.2 * 3.4"),
        1.2 * 3.4
    );

    check(
        "Test of Division Operator",
        exevalator.eval("1.2 / 3.4"),
        1.2 / 3.4
    );

    check(
        "Test of Unary Minus Operator",
        exevalator.eval("-1.2"),
        -1.2
    );
}


function testPrecedencesOfOperators() {
    let exevalator: Exevalator = new Exevalator();

    check(
        "Test of Precedences of Operators 1",
        exevalator.eval("1.2 + 3.4 + 5.6 + 7.8"),
        1.2 + 3.4 + 5.6 + 7.8
    );

    check(
        "Test of Precedences of Operators 2",
        exevalator.eval("1.2 + 3.4 - 5.6 + 7.8"),
        1.2 + 3.4 - 5.6 + 7.8
    );

    check(
        "Test of Precedences of Operators 3",
        exevalator.eval("1.2 + 3.4 * 5.6 + 7.8"),
        1.2 + 3.4 * 5.6 + 7.8
    );

    check(
        "Test of Precedences of Operators 4",
        exevalator.eval("1.2 + 3.4 / 5.6 + 7.8"),
        1.2 + 3.4 / 5.6 + 7.8
    );

    check(
        "Test of Precedences of Operators 5",
        exevalator.eval("1.2 * 3.4 + 5.6 + 7.8"),
        1.2 * 3.4 + 5.6 + 7.8
    );

    check(
        "Test of Precedences of Operators 6",
        exevalator.eval("1.2 * 3.4 - 5.6 + 7.8"),
        1.2 * 3.4 - 5.6 + 7.8
    );

    check(
        "Test of Precedences of Operators 7",
        exevalator.eval("1.2 * 3.4 * 5.6 + 7.8"),
        1.2 * 3.4 * 5.6 + 7.8
    );

    check(
        "Test of Precedences of Operators 8",
        exevalator.eval("1.2 * 3.4 / 5.6 + 7.8"),
        1.2 * 3.4 / 5.6 + 7.8
    );

    check(
        "Test of Precedences of Operators 9",
        exevalator.eval("1.2 + 3.4 + 5.6 * 7.8"),
        1.2 + 3.4 + 5.6 * 7.8
    );

    check(
        "Test of Precedences of Operators 10",
        exevalator.eval("1.2 + 3.4 - 5.6 * 7.8"),
        1.2 + 3.4 - 5.6 * 7.8
    );

    check(
        "Test of Precedences of Operators 11",
        exevalator.eval("1.2 + 3.4 * 5.6 * 7.8"),
        1.2 + 3.4 * 5.6 * 7.8
    );

    check(
        "Test of Precedences of Operators 12",
        exevalator.eval("1.2 + 3.4 / 5.6 * 7.8"),
        1.2 + 3.4 / 5.6 * 7.8
    );

    check(
        "Test of Precedences of Operators 13",
        exevalator.eval("-1.2 + 3.4 / 5.6 * 7.8"),
        -1.2 + 3.4 / 5.6 * 7.8
    );

    check(
        "Test of Precedences of Operators 14",
        exevalator.eval("1.2 + 3.4 / -5.6 * 7.8"),
        1.2 + 3.4 / 5.6 * -7.8
    );

    check(
        "Test of Precedences of Operators 15",
        exevalator.eval("1.2 + 3.4 / 5.6 * -7.8"),
        1.2 + 3.4 / 5.6 * -7.8
    );
}


function testParentheses() {
    let exevalator: Exevalator = new Exevalator();

    check(
        "Test of Parentheses 1",
        exevalator.eval("(1.2 + 3.4)"),
        (1.2 + 3.4)
    );

    check(
        "Test of Parentheses 2",
        exevalator.eval("(1.2 + 3.4) + 5.6"),
        (1.2 + 3.4) + 5.6
    );

    check(
        "Test of Parentheses 3",
        exevalator.eval("1.2 + (3.4 + 5.6)"),
        1.2 + (3.4 + 5.6)
    );

    check(
        "Test of Parentheses 4",
        exevalator.eval("1.2 + -(3.4 + 5.6)"),
        1.2 + -(3.4 + 5.6)
    );

    check(
        "Test of Parentheses 5",
        exevalator.eval("1.2 + -(-3.4 + 5.6)"),
        1.2 + -(-3.4 + 5.6)
    );

    check(
        "Test of Parentheses 4",
        exevalator.eval("(1.2 * 3.4) + 5.6"),
        (1.2 * 3.4) + 5.6
    );

    check(
        "Test of Parentheses 5",
        exevalator.eval("(1.2 + 3.4) * 5.6"),
        (1.2 + 3.4) * 5.6
    );

    check(
        "Test of Parentheses 6",
        exevalator.eval("1.2 + (3.4 * 5.6)"),
        1.2 + (3.4 * 5.6)
    );

    check(
        "Test of Parentheses 7",
        exevalator.eval("1.2 + (3.4 * 5.6) + 7.8"),
        1.2 + (3.4 * 5.6) + 7.8
    );

    check(
        "Test of Parentheses 8",
        exevalator.eval("1.2 * (3.4 + 5.6) / 7.8"),
        1.2 * (3.4 + 5.6) / 7.8
    );

    check(
        "Test of Parentheses 9",
        exevalator.eval("(1.2 + 3.4 - 5.6) * ((7.8 + 9.0) / 10.1)"),
        (1.2 + 3.4 - 5.6) * ((7.8 + 9.0) / 10.1)
    );

    check(
        "Test of Parentheses 10",
        exevalator.eval("(-(1.2 + 3.4 - 5.6) * ((7.8 + 9.0) / 10.1) / 11.2 + 12.3 * ((13.4 + -(15.6 - 17.8)) * 18.9)) + 19.0 * 20.1"),
        (-(1.2 + 3.4 - 5.6) * ((7.8 + 9.0) / 10.1) / 11.2 + 12.3 * ((13.4 + -(15.6 - 17.8)) * 18.9)) + 19.0 * 20.1
    );
}


function testComplicatedCases() {
    let exevalator: Exevalator = new Exevalator();

    check(
        "Test of Complicated Case 1: The Expression Containing Many Parentheses and Many Literals having Exponent Parts",
        exevalator.eval("(-(1.2E1 + 3.4E-2 - 5.6E2) * ((7.8E0 + 9.0) / 10.1E-3) / 11.2 + 12.3E-1 * ((13.4 + -(15.6E-12 - 17.8E-10)) * 18.9E-5)) + 19.0E-2 * 20.1E0"),
        (-(1.2E1 + 3.4E-2 - 5.6E2) * ((7.8E0 + 9.0) / 10.1E-3) / 11.2 + 12.3E-1 * ((13.4 + -(15.6E-12 - 17.8E-10)) * 18.9E-5)) + 19.0E-2 * 20.1E0
    );
}


function testSyntaxChecksOfCorresponencesOfParentheses() {
    let exevalator: Exevalator = new Exevalator();

    check(
        "Test of Detection of Mismatching of Open/Closed Parentheses 1",
        exevalator.eval("(1 + 2)"),
        (1 + 2)
    );

    try {
        check(
            "Test of Detection of Mismatching of Open/Closed Parentheses 2",
            exevalator.eval("((1 + 2)"),
            (1 + 2)
        );
        throw new ExevalatorTestError("Expected exception has not been thrown");
    } catch (error) {
        // Expected to thrown
        console.log("Test of Detection of Mismatching of Open/Closed Parentheses 2: OK.");
    }

    try {
        check(
            "Test of Detection of Mismatching of Open/Closed Parentheses 3",
            exevalator.eval("(1 + 2))"),
            (1 + 2)
        );
        throw new ExevalatorTestError("Expected exception has not been thrown");
    } catch (error) {
        // Expected to thrown
        console.log("Test of Detection of Mismatching of Open/Closed Parentheses 3: OK.");
    }

    check(
        "Test of Detection of Mismatching of Open/Closed Parentheses 4",
        exevalator.eval("(1 + 2) + (3 + 4)"),
        (1 + 2) + (3 + 4)
    );

    try {
        check(
            "Test of Detection of Mismatching of Open/Closed Parentheses 5",
            exevalator.eval("1 + 2) + (3 + 4"),
            (1 + 2) + (3 + 4)
        );
        throw new ExevalatorTestError("Expected exception has not been thrown");
    } catch (error) {
        // Expected to thrown
        console.log("Test of Detection of Mismatching of Open/Closed Parentheses 5: OK.");
    }

    check(
        "Test of Detection of Mismatching of Open/Closed Parentheses 6",
        exevalator.eval("1 + ((2 + (3 + 4) + 5) + 6)"),
        1 + ((2 + (3 + 4) + 5) + 6)
    );

    try {
        check(
            "Test of Detection of Mismatching of Open/Closed Parentheses 7",
            exevalator.eval("1 + ((2 + (3 + 4) + 5) + 6"),
            1 + ((2 + (3 + 4) + 5) + 6)
        );
        throw new ExevalatorTestError("Expected exception has not been thrown");
    } catch (error) {
        // Expected to thrown
        console.log("Test of Detection of Mismatching of Open/Closed Parentheses 7: OK.");
    }

    try {
        check(
            "Test of Detection of Mismatching of Open/Closed Parentheses 8",
            exevalator.eval("1 + (2 + (3 + 4) + 5) + 6)"),
            1 + ((2 + (3 + 4) + 5) + 6)
        );
        throw new ExevalatorTestError("Expected exception has not been thrown");
    } catch (error) {
        // Expected to thrown
        console.log("Test of Detection of Mismatching of Open/Closed Parentheses 8: OK.");
    }

    try {
        check(
            "Test of Detection of Empty Parentheses 1",
            exevalator.eval("()"),
            Number.NaN
        );
        throw new ExevalatorTestError("Expected exception has not been thrown");
    } catch (error) {
        // Expected to thrown
        console.log("Test of Detection of Empty Parentheses 1: OK.");
    }

    try {
        check(
            "Test of Detection of Empty Parentheses 2",
            exevalator.eval("1 + ()"),
            Number.NaN
        );
        throw new ExevalatorTestError("Expected exception has not been thrown");
    } catch (error) {
        // Expected to thrown
        console.log("Test of Detection of Empty Parentheses 2: OK.");
    }

    try {
        check(
            "Test of Detection of Empty Parentheses 3",
            exevalator.eval("() + 1"),
            Number.NaN
        );
        throw new ExevalatorTestError("Expected exception has not been thrown");
    } catch (error) {
        // Expected to thrown
        console.log("Test of Detection of Empty Parentheses 3: OK.");
    }
}


function testSyntaxChecksOfLocationsOfOperatorsAndLeafs() {
    let exevalator: Exevalator = new Exevalator();

    check(
        "Test of Detection of Left Operand of Unary-Prefix Operator 1",
        exevalator.eval("1 + -123"),
        1 + -123
    );

    try {
        check(
            "Test of Detection of Left Operand of Unary-Prefix Operator 2",
            exevalator.eval("1 + -"),
            1 + -123
        );
        throw new ExevalatorTestError("Expected exception has not been thrown");
    } catch (error) {
        // Expected to thrown
        console.log("Test of Detection of Left Operand of Unary-Prefix Operator 2: OK.");
    }

    try {
        check(
            "Test of Detection of Left Operand of Unary-Prefix Operator 3",
            exevalator.eval("(1 + -)"),
            1 + -123
        );
        throw new ExevalatorTestError("Expected exception has not been thrown");
    } catch (error) {
        // Expected to thrown
        console.log("Test of Detection of Left Operand of Unary-Prefix Operator 3: OK.");
    }

    check(
        "Test of Detection of Left Operand of Binary Operator 1",
        exevalator.eval("123 + 456"),
        123 + 456
    );

    try {
        check(
            "Test of Detection of Left Operand of Binary Operator 2",
            exevalator.eval("123 *"),
            123 * 456
        );
        throw new ExevalatorTestError("Expected exception has not been thrown");
    } catch (error) {
        // Expected to thrown
        console.log("Test of Detection of Left Operand of Binary Operator 2: OK.");
    }

    try {
        check(
            "Test of Detection of Left Operand of Binary Operator 3",
            exevalator.eval("* 456"),
            123 * 456
        );
        throw new ExevalatorTestError("Expected exception has not been thrown");
    } catch (error) {
        // Expected to thrown
        console.log("Test of Detection of Left Operand of Binary Operator 3: OK.");
    }

    try {
        check(
            "Test of Detection of Left Operand of Binary Operator 4",
            exevalator.eval("123 + ( * 456)"),
            123 * 456
        );
        throw new ExevalatorTestError("Expected exception has not been thrown");
    } catch (error) {
        // Expected to thrown
        console.log("Test of Detection of Left Operand of Binary Operator 4: OK.");
    }

    try {
        check(
            "Test of Detection of Left Operand of Binary Operator 5",
            exevalator.eval("(123 *) + 456"),
            123 * 456
        );
        throw new ExevalatorTestError("Expected exception has not been thrown");
    } catch (error) {
        // Expected to thrown
        console.log("Test of Detection of Left Operand of Binary Operator 5: OK.");
    }

    try {
        check(
            "Test of Detection of Lacking Operator",
            exevalator.eval("123 456"),
            123 * 456
        );
        throw new ExevalatorTestError("Expected exception has not been thrown");
    } catch (error) {
        // Expected to thrown
        console.log("Test of Detection of Lacking Operator: OK.");
    }

}


function testVariables() {
    let exevalator: Exevalator = new Exevalator();

    try {
        exevalator.eval("x");
        throw new ExevalatorTestError("Expected exception has not been thrown");
    } catch (error) {
        // Expected to be thrown
        console.log("Test of Variables 1: OK.");
    }

    const xAddress: number = exevalator.declareVariable("x");

    check(
        "Test of Variables 2",
        exevalator.eval("x"),
        0.0
    );

    exevalator.writeVariable("x", 1.25);

    check(
        "Test of Variables 3",
        exevalator.eval("x"),
        1.25
    );

    exevalator.writeVariableAt(xAddress, 2.5);

    check(
        "Test of Variables 4",
        exevalator.eval("x"),
        2.5
    );

    try {
        exevalator.writeVariableAt(100, 5.0);
        throw new ExevalatorTestError("Expected exception has not been thrown");
    } catch (error) {
        // Expected to be thrown
        console.log("Test of Variables 5: OK.");
    }

    try {
        exevalator.eval("y");
        throw new ExevalatorTestError("Expected exception has not been thrown");
    } catch (error) {
        // Expected to be thrown
        console.log("Test of Variables 6: OK.");
    }

    const yAddress: number = exevalator.declareVariable("y");

    check(
        "Test of Variables 7",
        exevalator.eval("y"),
        0.0
    );

    exevalator.writeVariable("y", 0.25);

    check(
        "Test of Variables 8",
        exevalator.eval("y"),
        0.25
    );

    exevalator.writeVariableAt(yAddress, 0.5);

    check(
        "Test of Variables 9",
        exevalator.eval("y"),
        0.5
    );

    check(
        "Test of Variables 10",
        exevalator.eval("x + y"),
        2.5 + 0.5
    );

    // Variables having names containing numbers
    exevalator.declareVariable("x2");
    exevalator.declareVariable("y2");
    exevalator.writeVariable("x2", 22.5);
    exevalator.writeVariable("y2", 32.5);
    check(
        "Test of Variables 11",
        exevalator.eval("x + y + 2 + x2 + 2 * y2"),
        2.5 + 0.5 + 2.0 + 22.5 + 2.0 * 32.5
    );
}


class FunctionA implements ExevalatorFunctionInterface {
    public invoke(args: number[]): number {
        if (args.length != 0) {
            throw new ExevalatorError("Incorrect number of arguments");
        }
        return 1.25;
    }
}

class FunctionB implements ExevalatorFunctionInterface {
    public invoke(args: number[]): number {
        if (args.length != 1) {
            throw new ExevalatorError("Incorrect number of arguments");
        }
        return args[0];
    }
}

class FunctionC implements ExevalatorFunctionInterface {
    public invoke(args: number[]): number {
        if (args.length != 2) {
            throw new ExevalatorError("Incorrect number of arguments");
        }
        return args[0] + args[1];
    }
}

class FunctionD implements ExevalatorFunctionInterface {
    public invoke(args: number[]): number {
        if (args.length != 3) {
            throw new ExevalatorError("Incorrect number of arguments");
        }
        if (args[0] != 1.25) {
            throw new ExevalatorError("The value of args[0] is incorrect");
        }
        if (args[1] != 2.5) {
            throw new ExevalatorError("The value of args[1] is incorrect");
        }
        if (args[2] != 5.0) {
            throw new ExevalatorError("The value of args[2] is incorrect");
        }
        return 0.0;
    }
}

function testFunctions() {
    let exevalator: Exevalator = new Exevalator();

    try {
        exevalator.eval("funA()");
        throw new ExevalatorTestError("Expected exception has not been thrown");
    } catch (error) {
        // Expected to be thrown
        console.log("Test of Functions 1: OK.");
    }

    exevalator.connectFunction("funA", new FunctionA());
    check(
        "Test of Functions 2",
        exevalator.eval("funA()"),
        1.25
    );

    try {
        exevalator.eval("funB(2.5)");
        throw new ExevalatorTestError("Expected exception has not been thrown");
    } catch (error) {
        // Expected to be thrown
        console.log("Test of Functions 3: OK.");
    }

    exevalator.connectFunction("funB", new FunctionB());
    check(
        "Test of Functions 4",
        exevalator.eval("funB(2.5)"),
        2.5
    );

    exevalator.connectFunction("funC", new FunctionC());
    check(
        "Test of Functions 5",
        exevalator.eval("funC(1.25, 2.5)"),
        1.25 + 2.5
    );

    check(
        "Test of Functions 6",
        exevalator.eval("funC(funA(), funB(2.5))"),
        1.25 + 2.5
    );

    check(
        "Test of Functions 7",
        exevalator.eval("funC(funC(funA(), funB(2.5)), funB(1.0))"),
        1.25 + 2.5 + 1.0
    );
    
    check(
        "Test of Function 8",
        exevalator.eval("funC(1.0, 3.5 * funB(2.5) / 2.0)"),
        1.0 + 3.5 * 2.5 / 2.0
    );
    
    check(
        "Test of Functions 9",
        exevalator.eval("funA() * funC(funC(funA(), 3.5 * funB(2.5) / 2.0), funB(1.0))"),
        1.25 * (1.25 + 3.5 * 2.5 / 2.0 + 1.0)
    );

    check(
        "Test of Functions 10",
        exevalator.eval("2 + 256 * funA() * funC(funC(funA(), 3.5 * funB(2.5) / 2.0), funB(1.0)) * 128"),
        2.0 + 256.0 * (1.25 * (1.25 + 3.5 * 2.5 / 2.0 + 1.0)) * 128.0
    );

    exevalator.connectFunction("funD", new FunctionD());
    check(
        "Test of Functions 11",
        exevalator.eval("funD(1.25, 2.5, 5.0)"),
        0.0
    );

    check(
        "Test of Functions 12",
        exevalator.eval("-funC(-1.25, -2.5)"),
        - (-1.25 + -2.5)
    );
}


function testEmptyExpressions() {
    let exevalator: Exevalator = new Exevalator();

    try {
        exevalator.eval("");
        throw new ExevalatorTestError("Expected exception has not been thrown");
    } catch (error) {
        // Expected to be thrown
        console.log("Test of Empty Functions 1: OK.");
    }

    try {
        exevalator.eval(" ");
        throw new ExevalatorTestError("Expected exception has not been thrown");
    } catch (error) {
        // Expected to be thrown
        console.log("Test of Empty Functions 2: OK.");
    }

    try {
        exevalator.eval("  ");
        throw new ExevalatorTestError("Expected exception has not been thrown");
    } catch (error) {
        // Expected to be thrown
        console.log("Test of Empty Functions 3: OK.");
    }

    try {
        exevalator.eval("   ");
        throw new ExevalatorTestError("Expected exception has not been thrown");
    } catch (error) {
        // Expected to be thrown
        console.log("Test of Empty Functions 4: OK.");
    }
}


function testReeval() {
    let exevalator: Exevalator = new Exevalator();

    check(
        "Test of reval() Method 1",
        exevalator.eval("1.2 + 3.4"),
        1.2 + 3.4
    );

    check(
        "Test of reval() Method 2",
        exevalator.reeval(),
        1.2 + 3.4
    );

    check(
        "Test of reval() Method 3",
        exevalator.reeval(),
        1.2 + 3.4
    );

    check(
        "Test of reval() Method 4",
        exevalator.eval("5.6 - 7.8"),
        5.6 - 7.8
    );

    check(
        "Test of reval() Method 5",
        exevalator.reeval(),
        5.6 - 7.8
    );

    check(
        "Test of reval() Method 6",
        exevalator.reeval(),
        5.6 - 7.8
    );

    check(
        "Test of reval() Method 7",
        exevalator.eval("(1.23 + 4.56) * 7.89"),
        (1.23 + 4.56) * 7.89
    );

    check(
        "Test of reval() Method 8",
        exevalator.reeval(),
        (1.23 + 4.56) * 7.89
    );

    check(
        "Test of reval() Method 9",
        exevalator.reeval(),
        (1.23 + 4.56) * 7.89
    );
}


function testTokenizations() {
    let exevalator: Exevalator = new Exevalator();

    check(
        "Test of Tokenization 1",
        exevalator.eval("1.2345678"),
        1.2345678
    );

    try {
        exevalator.eval("1.234\n5678");
        throw new ExevalatorTestError("Expected exception has not been thrown");
    } catch (error) {
        // Expected to be thrown
        console.log("Test of Tokenization 2: OK.");
    }

    try {
        exevalator.eval("1.234\r\n5678");
        throw new ExevalatorTestError("Expected exception has not been thrown");
    } catch (error) {
        // Expected to be thrown
        console.log("Test of Tokenization 3: OK.");
    }

    try {
        exevalator.eval("1.234\t5678");
        throw new ExevalatorTestError("Expected exception has not been thrown");
    } catch (error) {
        // Expected to be thrown
        console.log("Test of Tokenization 4: OK.");
    }

    try {
        exevalator.eval("1.234 5678");
        throw new ExevalatorTestError("Expected exception has not been thrown");
    } catch (error) {
        // Expected to be thrown
        console.log("Test of Tokenization 5: OK.");
    }

    check(
        "Test of Tokenization 6",
        exevalator.eval("1+2*3-4/5"),
        1.0 + 2.0 * 3.0 - 4.0 / 5.0
    );

    check(
        "Test of Tokenization 7",
        exevalator.eval("1+\n2*3\r\n-4/5"),
        1.0 + 2.0 * 3.0 - 4.0 / 5.0
    );

    check(
        "Test of Tokenization 8",
        exevalator.eval("((1+2)*3)-(4/5)"),
        ((1.0 + 2.0) * 3.0) - (4.0 / 5.0)
    );

    exevalator.connectFunction("funC", new FunctionC());

    check(
        "Test of Tokenization 9",
        exevalator.eval("funC(1,2)"),
        1.0 + 2.0
    );

    check(
        "Test of Tokenization 10",
        exevalator.eval("funC(\n1,\r\n2\t)"),
        1.0 + 2.0
    );

    check(
        "Test of Tokenization 11",
        exevalator.eval("3*funC(1,2)/2"),
        3.0 * (1.0 + 2.0) / 2.0
    );

    check(
        "Test of Tokenization 12",
        exevalator.eval("3*(-funC(1,2)+2)"),
        3.0 * (-(1.0 + 2.0) + 2.0)
    );
}


function testAddressFiltering() {
    const maxMemoryLength = 2147483647 + 1; // max "signed" int32 value + 1

    for (let rawAddress: number = -1000000; rawAddress < 0; rawAddress++) {
        const filteredAddress = ((rawAddress | 0) & ~(rawAddress >> 31)) & ((maxMemoryLength - 1) | 0);
        if (filteredAddress !== 0) {
            throw new ExevalatorTestError(`Address filtering failed: ${rawAddress} filtered to ${filteredAddress}`);
        }
    }
    console.log("Test of Address Filtering 1: OK");

    for (let rawAddress: number = maxMemoryLength; rawAddress < maxMemoryLength + 1000000; rawAddress++) {
        const filteredAddress = ((rawAddress | 0) & ~(rawAddress >> 31)) & ((maxMemoryLength - 1) | 0);
        if (filteredAddress !== 0) {
            throw new ExevalatorTestError(`Address filtering failed: ${rawAddress} filtered to ${filteredAddress}`);
        }
    }
    console.log("Test of Address Filtering 2: OK");

    for (let rawAddress: number = 0; rawAddress < 1000000; rawAddress++) {
        const filteredAddress = ((rawAddress | 0) & ~(rawAddress >> 31)) & ((maxMemoryLength - 1) | 0);
        if (rawAddress !== filteredAddress) {
            throw new ExevalatorTestError(`Address filtering failed: ${rawAddress} filtered to ${filteredAddress}`);
        }
    }
    console.log("Test of Address Filtering 3: OK");

    for (let rawAddress: number = maxMemoryLength - 1000000; rawAddress < maxMemoryLength; rawAddress++) {
        const filteredAddress = ((rawAddress | 0) & ~(rawAddress >> 31)) & ((maxMemoryLength - 1) | 0);
        if (rawAddress !== filteredAddress) {
            throw new ExevalatorTestError(`Address filtering failed: ${rawAddress} filtered to ${filteredAddress}`);
        }
    }
    console.log("Test of Address Filtering 4: OK");
}


main();
