#include <iostream>
#include <cstdlib>
#include "exevalator.hpp"
#include "exevalator.cpp"

/**
 * The class providing a function available in expressions.
 */
class MyFun : public exevalator::ExevalatorFunctionInterface {

    /**
     * Invokes the function.
     * 
     * @param arguments The vector storing values of arguments.
     * @return double The return value of this function.
     */
    double operator()(const std::vector<double> &arguments) {
        if (arguments.size() != 2) {
            throw new exevalator::ExevalatorException("Incorrect number of args");
        }
        return arguments[0] + arguments[1];
    }
};

/**
 * An example to use the function defined above in an expression.
 */
int main() {

    // Create an instance of Exevalator Engine
    exevalator::Exevalator exevalator;

    try {

        // Connects the function available for using it in expressions
        exevalator.connect_function("fun", std::make_shared<MyFun>());

        // Evaluate the value of an expression
        double result = exevalator.eval("fun(1.2, 3.4)");

        // Display the result
        std::cout << "result: " << result << std::endl;

    } catch (exevalator::ExevalatorException &e) {
        std::cout << "Error occurred: " << e.what() << std::endl;
        return EXIT_FAILURE;
    }

    return EXIT_SUCCESS;
}
