mod exevalator;
use exevalator::Exevalator;

/// An example to use various operators and parentheses.
fn main() {

    // Create an instance of Exevalator Engine
    let mut exevalator = Exevalator::new();

    // Evaluate the value of an expression
    let result: f64 = match exevalator.eval("(-(1.2 + 3.4) * 5) / 2") {
        Ok(eval_value) => eval_value,
        Err(eval_error) => panic!("{}", eval_error),
    };

    // Display the result
    println!("result = {}", result);    
}
