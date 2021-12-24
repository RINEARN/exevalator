mod exevalator;
use exevalator::Exevalator;
use exevalator::ExevalatorError;

/// The minimum error between two double-type values to regard them almost equal.
static ALLOWABLE_ERROR: f64 = 1.0E-12;

fn main() {
    test_number_literals();
    test_operations_of_operators();
    test_precedences_of_oerators();
    test_parentheses();
    test_complicated_cases();
    test_syntax_checks_of_corresponences_of_parentheses();
    test_syntax_checks_of_locations_of_operators_and_leafs();

    println!("All tests have completed successfully.");
}


fn test_number_literals() {
    let mut exevalator: Exevalator = Exevalator::new();

    check(
        "Test of a Simple Number Literal 1",
        exevalator.eval("1"),
        1.0
    );

    check(
        "Test of a Simple Number Literal 2",
        exevalator.eval("2"),
        2.0
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


fn test_operations_of_operators() {
    let mut exevalator: Exevalator = Exevalator::new();

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


fn test_precedences_of_oerators() {
    let mut exevalator: Exevalator = Exevalator::new();

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
        1.2 + 3.4 / -5.6 * 7.8
    );

    check(
        "Test of Precedences of Operators 15",
        exevalator.eval("1.2 + 3.4 / 5.6 * -7.8"),
        1.2 + 3.4 / 5.6 * -7.8
    );
}


fn test_parentheses() {
    let mut exevalator: Exevalator = Exevalator::new();

    check(
        "Test of Parentheses 1",
        exevalator.eval("(1.2 + 3.4)"),
        1.2 + 3.4
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


fn test_complicated_cases() {
    let mut exevalator: Exevalator = Exevalator::new();

    check(
        "Test of Complicated Case 1: The Expression Containing Many Parentheses and Many Literals having Exponent Parts",
        exevalator.eval("(-(1.2E1 + 3.4E-2 - 5.6E2) * ((7.8E0 + 9.0) / 10.1E-3) / 11.2 + 12.3E-1 * ((13.4 + -(15.6E-12 - 17.8E-10)) * 18.9E-5)) + 19.0E-2 * 20.1E0"),
        (-(1.2E1 + 3.4E-2 - 5.6E2) * ((7.8E0 + 9.0) / 10.1E-3) / 11.2 + 12.3E-1 * ((13.4 + -(15.6E-12 - 17.8E-10)) * 18.9E-5)) + 19.0E-2 * 20.1E0
    );
}


fn test_syntax_checks_of_corresponences_of_parentheses() {
    let mut exevalator: Exevalator = Exevalator::new();

    check(
        "Test of Detection of Mismatching of Open/Closed Parentheses 1",
        exevalator.eval("(1 + 2)"),
        1.0 + 2.0
    );

    match exevalator.eval("((1 + 2)") {
        Ok(_eval_ok) => panic!("Expected error has not occurred."),
        Err(_eval_error) => println!("Test of Detection of Mismatching of Open/Closed Parentheses 2: OK."),
    }

    match exevalator.eval("(1 + 2))") {
        Ok(_eval_ok) => panic!("Expected error has not occurred."),
        Err(_eval_error) => println!("Test of Detection of Mismatching of Open/Closed Parentheses 3: OK."),
    }

    check(
        "Test of Detection of Mismatching of Open/Closed Parentheses 4",
        exevalator.eval("(1 + 2) + (3 + 4)"),
        (1.0 + 2.0) + (3.0 + 4.0)
    );

    match exevalator.eval("1 + 2) + (3 + 4") {
        Ok(_eval_ok) => panic!("Expected error has not occurred."),
        Err(_eval_error) => println!("Test of Detection of Mismatching of Open/Closed Parentheses 5: OK."),
    }

    check(
        "Test of Detection of Mismatching of Open/Closed Parentheses 6",
        exevalator.eval("1 + ((2 + (3 + 4) + 5) + 6)"),
        1.0 + ((2.0 + (3.0 + 4.0) + 5.0) + 6.0)
    );

    match exevalator.eval("1 + ((2 + (3 + 4) + 5) + 6") {
        Ok(_eval_ok) => panic!("Expected error has not occurred."),
        Err(_eval_error) => println!("Test of Detection of Mismatching of Open/Closed Parentheses 7: OK."),
    }

    match exevalator.eval("1 + (2 + (3 + 4) + 5) + 6)") {
        Ok(_eval_ok) => panic!("Expected error has not occurred."),
        Err(_eval_error) => println!("Test of Detection of Mismatching of Open/Closed Parentheses 8: OK."),
    }

    match exevalator.eval("()") {
        Ok(_eval_ok) => panic!("Expected error has not occurred."),
        Err(_eval_error) => println!("Test of Detection of Empty Parentheses 1: OK."),
    }

    match exevalator.eval("1 + ()") {
        Ok(_eval_ok) => panic!("Expected error has not occurred."),
        Err(_eval_error) => println!("Test of Detection of Empty Parentheses 2: OK."),
    }

    match exevalator.eval("() + 1") {
        Ok(_eval_ok) => panic!("Expected error has not occurred."),
        Err(_eval_error) => println!("Test of Detection of Empty Parentheses 3: OK."),
    }
}


fn test_syntax_checks_of_locations_of_operators_and_leafs() {
    let mut exevalator: Exevalator = Exevalator::new();
    check(
        "Test of Detection of Left Operand of Unary-Prefix Operator 1",
        exevalator.eval("1 + -123"),
        1.0 + -123.0
    );

    match exevalator.eval("1 + -") {
        Ok(_eval_ok) => panic!("Expected error has not occurred."),
        Err(_eval_error) => println!("Test of Detection of Left Operand of Unary-Prefix Operator 2: OK."),
    }

    match exevalator.eval("(1 + -)") {
        Ok(_eval_ok) => panic!("Expected error has not occurred."),
        Err(_eval_error) => println!("Test of Detection of Left Operand of Unary-Prefix Operator 3: OK."),
    }

    check(
        "Test of Detection of Left Operand of Binary Operator 1",
        exevalator.eval("123 + 456"),
        123.0 + 456.0
    );

    match exevalator.eval("123 *") {
        Ok(_eval_ok) => panic!("Expected error has not occurred."),
        Err(_eval_error) => println!("Test of Detection of Left Operand of Binary Operator 2: OK."),
    }

    match exevalator.eval("* 456") {
        Ok(_eval_ok) => panic!("Expected error has not occurred."),
        Err(_eval_error) => println!("Test of Detection of Left Operand of Binary Operator 3: OK."),
    }

    match exevalator.eval("123 + ( * 456)") {
        Ok(_eval_ok) => panic!("Expected error has not occurred."),
        Err(_eval_error) => println!("Test of Detection of Left Operand of Binary Operator 4: OK."),
    }

    match exevalator.eval("(123 *) + 456") {
        Ok(_eval_ok) => panic!("Expected error has not occurred."),
        Err(_eval_error) => println!("Test of Detection of Left Operand of Binary Operator 5: OK."),
    }

    match exevalator.eval("123 456") {
        Ok(_eval_ok) => panic!("Expected error has not occurred."),
        Err(_eval_error) => println!("Test of Detection of Lacking Operator: OK."),
    }
}


/// Checks the evaluated (computed) value of the testing expression by the Exevalator.
///
/// * `test_name` - The name of the testing.
/// * `evaluated_result` - The evaluated (computed) result of the testing expression by Exevalator.
/// * `correct_value` - The correct value of the testing expression.
///
fn check(test_name: &str, evaluated_result: Result<f64, ExevalatorError>, correct_value: f64) {
    let evaluated_value: f64 = match evaluated_result {
        Ok(result_ok) => result_ok,
        Err(result_err) => panic!(
            "\"{}\" has failed. Error occurred: {}", test_name, result_err
        ),
    };

    if (evaluated_value - correct_value).abs() < ALLOWABLE_ERROR {
        println!("{}: OK.", test_name);
        return;
    } else {
        panic!(
            "\"{}\" has failed. evaluated_value={}, correct_value={}.",
            test_name, evaluated_value, correct_value
        );
    }
}


