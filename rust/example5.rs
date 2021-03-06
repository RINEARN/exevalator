mod exevalator;
use exevalator::Exevalator;
use exevalator::ExevalatorError;

/// Function available in expressions.
fn my_function(arguments: Vec<f64>) -> Result<f64, ExevalatorError> {
    if arguments.len() != 2 {
        return Err(ExevalatorError::new("Incorrect number of args"));
    }
    return Ok(arguments[0] + arguments[1]);
} 

/// An example to connect a function available in expressions.
fn main() {

    // Create an instance of Exevalator Engine
    let mut exevalator = Exevalator::new();

    // Connects the function available for using it in expressions
    match exevalator.connect_function("fun", my_function) {
        Ok(_) => {},
        Err(connection_error) => panic!("{}", connection_error),
    };

    // Evaluate the value of an expression
    let result: f64 = match exevalator.eval("fun(1.2, 3.4)") {
        Ok(eval_value) => eval_value,
        Err(eval_error) => panic!("{}", eval_error),
    };

    // Display the result
    println!("result: {}", result);    
}
