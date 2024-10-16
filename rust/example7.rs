mod exevalator;
use exevalator::Exevalator;
use std::io;

/// An example to compute the numerical integration value of the inputted expression f(x).
/// For details of the numerical integration algorithm used in this code, see:
///   https://www.vcssl.org/en-us/code/archive/0001/7800-vnano-integral-output/   (English)
///   https://www.vcssl.org/ja-jp/code/archive/0001/7800-vnano-integral-output/   (Japanese)
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

    // Get the value of lower-limit from the standard-input
    println!("lower-limit = ?                  (default: 0)");
    let mut lower_limit_string: String = String::new();
    io::stdin().read_line(&mut lower_limit_string).expect("I/O error occurred for receiving the lower-limit value.");
    let mut lower_limit_str: &str = lower_limit_string.trim();
    if lower_limit_str.is_empty() {
        lower_limit_str = "0";
    }
    let lower_limit_value: f64 = lower_limit_str.parse::<f64>().expect("Invalid lower-limit value.");

    // Get the value of upper-limit from the standard-input
    println!("upper-limit = ?                  (default: 1)");
    let mut upper_limit_string: String = String::new();
    io::stdin().read_line(&mut upper_limit_string).expect("I/O error occurred for receiving the upper-limit value.");
    let mut upper_limit_str: &str = upper_limit_string.trim();
    if upper_limit_str.is_empty() {
        upper_limit_str = "1";
    }
    let upper_limit_value: f64 = upper_limit_str.parse::<f64>().expect("Invalid upper-limit value.");

    // Other numerical integration parameters
    let number_of_steps: i64 = 65536;
    let delta: f64 = (upper_limit_value - lower_limit_value) / (number_of_steps as f64);
    let mut result: f64 = 0.0;

    // Create an instance of Exevalator Engine
    let mut exevalator = Exevalator::new();
    let x_address = exevalator.declare_variable("x").expect("Failed to declare the variable x.");

    // Traverse tiny intervals from lower-limit to upper-limit
    for i in 0..number_of_steps {

        // The x-coordinate value of the left-bottom point of i-th tiny interval
        let x: f64 = lower_limit_value + (i as f64) * delta;

        // Compute area of i-th tiny interval approximately by using Simpson's rule,
        // and add it to the variable "result"
        // (see: https://www.vcssl.org/en-us/code/archive/0001/7800-vnano-integral-output/ )

        exevalator.write_variable_at(x_address, x).expect("Failed to set the value of the variable x.");
        let fx_left: f64 = exevalator.eval(expression_str).expect("Failed to evaluate the value of f(x).");

        exevalator.write_variable_at(x_address, x + delta).expect("Failed to set the value of the variable x.");
        let fx_right: f64 = exevalator.eval(expression_str).expect("Failed to evaluate the value of f(x).");

        exevalator.write_variable_at(x_address, x + delta/2.0).expect("Failed to set the value of the variable x.");
        let fx_center: f64 = exevalator.eval(expression_str).expect("Failed to evaluate the value of f(x).");

        result += (fx_left + fx_right + 4.0 * fx_center) * delta / 6.0;
    }

    // Display the result
    println!("----------");
    println!("f(x)        = {}", expression_str);
    println!("lower-limit = {}", lower_limit_value);
    println!("upper-limit = {}", upper_limit_value);
    println!("result      = {}", result);
}
