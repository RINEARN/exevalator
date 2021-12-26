mod exevalator;
use exevalator::Exevalator;

/// An example to use a variable.
fn main() {

    // Create an instance of Exevalator Engine
    let mut exevalator = Exevalator::new();

    // Declare a variable and set the value
    match exevalator.declare_variable("x") {
        Ok(address) => address,
        Err(declaration_error) => panic!("{}", declaration_error)
    };
    match exevalator.write_variable("x", 1.25) {
        Some(access_error) => panic!("{}", access_error),
        None => {},
    };

    // Evaluate the value of an expression
    let result: f64 = match exevalator.eval("x + 1") {
        Ok(eval_value) => eval_value,
        Err(eval_error) => panic!("{}", eval_error),
    };

    // Display the result
    println!("result: {}", result);    
}
