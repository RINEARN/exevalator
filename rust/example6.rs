mod exevalator;
use exevalator::Exevalator;
use std::io;

/// An example to compute the value of the inputted expression f(x) at the inputted x.
fn main() {

    println!("");
    println!("This program computes the value of f(x) at x.");
    println!("");

    // Get the expression from the standard-input
    println!("f(x) = ?               (default: 3*x*x + 2*x + 1)");
    let mut expression_string: String = String::new();
    io::stdin().read_line(&mut expression_string).expect("I/O error occurred for receiving the expression.");
    let mut expression_str: &str = expression_string.trim();
    if expression_str.is_empty() {
        expression_str = "3*x*x + 2*x + 1";
    }

    // Get the value of x from the standard-input
    println!("x = ?                  (default: 1)");
    let mut x_string: String = String::new();
    io::stdin().read_line(&mut x_string).expect("I/O error occurred for receiving the x value.");
    let mut x_str: &str = x_string.trim();
    if x_str.is_empty() {
        x_str = "1";
    }
    let x_value: f64 = x_str.parse::<f64>().expect("Invalid x value.");

    // Create an instance of Exevalator Engine
    let mut exevalator = Exevalator::new();

    // Set the value of x
    exevalator.declare_variable("x").expect("Failed to declare the variable x.");
    exevalator.write_variable("x", x_value).expect("Failed to set the value of the variable x.");

    // Evaluate the value of an expression
    let result: f64 = match exevalator.eval(expression_str) {
        Ok(eval_value) => eval_value,
        Err(eval_error) => panic!("{}", eval_error),
    };

    // Display the result
    println!("----------");
    println!("f(x)   = {}", expression_str);
    println!("x      = {}", x_value);
    println!("result = {}", result);
}
