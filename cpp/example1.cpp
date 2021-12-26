#include <iostream>
#include <cstdlib>
#include "exevalator.hpp"
#include "exevalator.cpp"

/**
 * A simple example to use Exevalator.
 */
int main() {

    // Create an instance of Exevalator Engine
    exevalator::Exevalator exevalator;

    try {

        // Evaluate the value of an expression
        double result = exevalator.eval("1.2 + 3.4");

        // Display the result
        std::cout << "result=" << result << std::endl;

    } catch (exevalator::ExevalatorException &e) {
        std::cout << "Error occurred: " << e.what() << std::endl;
        return EXIT_FAILURE;
    }

    return EXIT_SUCCESS;
}
