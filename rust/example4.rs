mod exevalator;
use exevalator::Exevalator;

/// An example to use a variable.
fn main() {

    // Create an instance of Exevalator Engine
    let mut exevalator = Exevalator::new();

    // Declare a variable and set the value
    let address: usize = match exevalator.declare_variable("x") {
        Ok(declared_var_address) => declared_var_address,
        Err(declaration_error) => panic!("{}", declaration_error),
    };
    exevalator.write_variable_at(address, 1.25);
    // The above works faster than:
    //     exevalator.write_variable("x", 1.25);

    // Evaluate the value of an expression
    let result: f64 = match exevalator.eval("x + 1") {
        Ok(eval_value) => eval_value,
        Err(eval_error) => panic!("{}", eval_error),
    };

    // Display the result
    println!("result = {}", result);    
}
