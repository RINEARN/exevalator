#include <iostream>
#include <cstddef>
#include <cstdlib>
#include "exevalator.hpp"
#include "exevalator.cpp"

/**
 * An example to compute the numerical integration value of the inputted expression f(x).
 * For details of the numerical integration algorithm used in this code, see:
 *   https://www.vcssl.org/en-us/code/archive/0001/7800-vnano-integral-output/   (English)
 *   https://www.vcssl.org/ja-jp/code/archive/0001/7800-vnano-integral-output/   (Japanese)
 */
int main() {

    // Get the expression from the standard-input
    std::string expression;
    std::cout << "" << std::endl;
    std::cout << "This program computes the value of f(x) at x." << std::endl;
    std::cout << "" << std::endl;
    std::cout << "f(x) = ?               (example: 3*x*x)" << std::endl;
    std::cin >> expression;

    // Get the value of the lower limit from the standard-input
    double lower_limit;
    std::cout << "lower-limit = ?                  (example: 1)" << std::endl;
    std::cin >> lower_limit;

    // Get the value of the upper limit from the standard-input
    double upper_limit;
    std::cout << "upper-limit = ?                  (example: 2)" << std::endl;
    std::cin >> upper_limit;

    // Other numerical integration parameters
    long number_of_steps = 65536;
    double delta = (upper_limit - lower_limit) / number_of_steps;
    double result = 0.0;

    // Create an instance of Exevalator Engine
    exevalator::Exevalator exevalator;
    std::size_t x_address = exevalator.declare_variable("x");

    try {

        // Traverse tiny intervals from lower-limit to upper-limit
        for (long i=0; i<number_of_steps; ++i) {

            // The x-coordinate value of the left-bottom point of i-th tiny interval
            double x = lower_limit + i * delta;

            // Compute area of i-th tiny interval approximately by using Simpson's rule,
            // and add it to the variable "result"
            // (see: https://www.vcssl.org/en-us/code/archive/0001/7800-vnano-integral-output/ )

            exevalator.write_variable_at(x_address, x);
            double fx_left = exevalator.eval(expression);

            exevalator.write_variable_at(x_address, x + delta);
            double fx_right = exevalator.eval(expression);

            exevalator.write_variable_at(x_address, x + delta/2.0);
            double fx_center = exevalator.eval(expression);

            result += (fx_left + fx_right + 4.0 * fx_center) * delta / 6.0;
        }

        // Display the result
        std::cout << "----------" << std::endl;
        std::cout << "f(x)        = " << expression << std::endl;
        std::cout << "lower-limit = " << lower_limit << std::endl;
        std::cout << "upper-limit = " << upper_limit << std::endl;
        std::cout << "result      = " << result << std::endl;

    } catch (exevalator::ExevalatorException &e) {
        std::cout << "Error occurred: " << e.what() << std::endl;
        return EXIT_FAILURE;
    }

    return EXIT_SUCCESS;
}
