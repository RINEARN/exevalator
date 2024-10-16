#include <iostream>
#include <cstdlib>
#include "exevalator.hpp"
#include "exevalator.cpp"

/**
 * An example to compute the value of the inputted expression f(x) at the inputted x.
 */
int main() {

    // Get the expression from the standard-input
    std::string expression;
    std::cout << "" << std::endl;
    std::cout << "This program computes the value of f(x) at x." << std::endl;
    std::cout << "" << std::endl;
    std::cout << "f(x) = ?               (default: 3*x*x + 2*x + 1)" << std::endl;
    std::getline(std::cin, expression);
    if (expression.empty()) {
        expression = "3*x*x + 2*x + 1";
    }

    // Get the value of the x from the standard-input
    std::string x_str;
    std::cout << "x = ?                  (default: 1)" << std::endl;
    std::getline(std::cin, x_str);
    if (x_str.empty()) {
        x_str = "1";
    }
    double x_value = std::stod(x_str);

    // Create an instance of Exevalator Engine
    exevalator::Exevalator exevalator;

    // Set the value of x
    exevalator.declare_variable("x");
    exevalator.write_variable("x", x_value);

    try {

        // Evaluate the value of an expression
        double result = exevalator.eval(expression);

        // Display the result
        std::cout << "----------" << std::endl;
        std::cout << "f(x)   = " << expression << std::endl;
        std::cout << "x      = " << x_value << std::endl;
        std::cout << "result = " << result << std::endl;

    } catch (exevalator::ExevalatorException &e) {
        std::cout << "Error occurred: " << e.what() << std::endl;
        return EXIT_FAILURE;
    }

    return EXIT_SUCCESS;
}
