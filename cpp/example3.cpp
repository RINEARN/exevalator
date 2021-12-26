#include <iostream>
#include <cstdlib>
#include "exevalator.hpp"
#include "exevalator.cpp"

/**
 * An example to use a variable.
 */
int main() {

    // Create an instance of Exevalator Engine
    exevalator::Exevalator exevalator;

    try {

        // Declare a variable and set the value
        exevalator.declare_variable("x");
        exevalator.write_variable("x", 1.25);

        // Evaluate the value of an expression
        double result = exevalator.eval("x + 1");

        // Display the result
        std::cout << "result: " << result << std::endl;

    } catch (exevalator::ExevalatorException &e) {
        std::cout << "Error occurred: " << e.what() << std::endl;
        return EXIT_FAILURE;
    }

    return EXIT_SUCCESS;
}
