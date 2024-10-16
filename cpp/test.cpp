#include <iostream>
#include <cstdlib>
#include <cmath>
#include "exevalator.hpp"
#include "exevalator.cpp"

using exevalator::Exevalator;
using exevalator::ExevalatorException;

void check(const char *test_name, double evaluated_value, double correct_value);
void test_number_literals();
void test_operations_of_operators();
void test_precedences_of_oerators();
void test_parentheses();
void test_complicated_cases();
void test_syntax_checks_of_corresponences_of_parentheses();
void test_syntax_checks_of_locations_of_operators_and_leafs();
void test_variables();
void test_functions();
void test_empty_expressions();
void test_reeval();
void test_tokenizations();

/// The minimum error between two double-type values to regard them almost equal.
const double ALLOWABLE_ERROR = 1.0E-12;

int main() {
    try {
        test_number_literals();
        test_operations_of_operators();
        test_precedences_of_oerators();
        test_parentheses();
        test_complicated_cases();
        test_syntax_checks_of_corresponences_of_parentheses();
        test_syntax_checks_of_locations_of_operators_and_leafs();
        test_variables();
        test_functions();
        test_empty_expressions();
        test_reeval();
        test_tokenizations();

        std::cout << "All tests have completed successfully." << std::endl;
        return EXIT_SUCCESS;
    } catch (...) {
        std::cout << "Unexpected error occurred." << std::endl;
        return EXIT_FAILURE;
    }
}

void test_number_literals() {
    Exevalator exevalator;

    check(
        "Test of a Simple Number Literal 1",
        exevalator.eval("1"),
        1.0
    );

    check(
        "Test of a Simple Number Literal 2",
        exevalator.eval("2"),
        2.0
    );

    check(
        "Test of a Simple Number Literal 3",
        exevalator.eval("1.2"),
        1.2
    );

    check(
        "Test of a Number Literal with a Exponent Part 1",
        exevalator.eval("1.2E3"),
        1.2E3
    );

    check(
        "Test of a Number Literal with a Exponent Part 2",
        exevalator.eval("1.2E+3"),
        1.2E3
    );

    check(
        "Test of a Number Literal with a Exponent Part 3",
        exevalator.eval("1.2E-3"),
        1.2E-3
    );

    check(
        "Test of a Number Literal with a Exponent Part 4",
        exevalator.eval("123.4567E12"),
        123.4567E12
    );

    check(
        "Test of a Number Literal with a Exponent Part 5",
        exevalator.eval("123.4567E+12"),
        123.4567E+12
    );

    check(
        "Test of a Number Literal with a Exponent Part 6",
        exevalator.eval("123.4567E-12"),
        123.4567E-12
    );
}


void test_operations_of_operators() {
    Exevalator exevalator;

    check(
        "Test of Addition Operator",
        exevalator.eval("1.2 + 3.4"),
        1.2 + 3.4
    );

    check(
        "Test of Subtraction Operator",
        exevalator.eval("1.2 - 3.4"),
        1.2 - 3.4
    );

    check(
        "Test of Multiplication Operator",
        exevalator.eval("1.2 * 3.4"),
        1.2 * 3.4
    );

    check(
        "Test of Division Operator",
        exevalator.eval("1.2 / 3.4"),
        1.2 / 3.4
    );

    check(
        "Test of Unary Minus Operator",
        exevalator.eval("-1.2"),
        -1.2
    );
}


void test_precedences_of_oerators() {
    Exevalator exevalator;

    check(
        "Test of Precedences of Operators 1",
        exevalator.eval("1.2 + 3.4 + 5.6 + 7.8"),
        1.2 + 3.4 + 5.6 + 7.8
    );

    check(
        "Test of Precedences of Operators 2",
        exevalator.eval("1.2 + 3.4 - 5.6 + 7.8"),
        1.2 + 3.4 - 5.6 + 7.8
    );

    check(
        "Test of Precedences of Operators 3",
        exevalator.eval("1.2 + 3.4 * 5.6 + 7.8"),
        1.2 + 3.4 * 5.6 + 7.8
    );

    check(
        "Test of Precedences of Operators 4",
        exevalator.eval("1.2 + 3.4 / 5.6 + 7.8"),
        1.2 + 3.4 / 5.6 + 7.8
    );

    check(
        "Test of Precedences of Operators 5",
        exevalator.eval("1.2 * 3.4 + 5.6 + 7.8"),
        1.2 * 3.4 + 5.6 + 7.8
    );

    check(
        "Test of Precedences of Operators 6",
        exevalator.eval("1.2 * 3.4 - 5.6 + 7.8"),
        1.2 * 3.4 - 5.6 + 7.8
    );

    check(
        "Test of Precedences of Operators 7",
        exevalator.eval("1.2 * 3.4 * 5.6 + 7.8"),
        1.2 * 3.4 * 5.6 + 7.8
    );

    check(
        "Test of Precedences of Operators 8",
        exevalator.eval("1.2 * 3.4 / 5.6 + 7.8"),
        1.2 * 3.4 / 5.6 + 7.8
    );

    check(
        "Test of Precedences of Operators 9",
        exevalator.eval("1.2 + 3.4 + 5.6 * 7.8"),
        1.2 + 3.4 + 5.6 * 7.8
    );

    check(
        "Test of Precedences of Operators 10",
        exevalator.eval("1.2 + 3.4 - 5.6 * 7.8"),
        1.2 + 3.4 - 5.6 * 7.8
    );

    check(
        "Test of Precedences of Operators 11",
        exevalator.eval("1.2 + 3.4 * 5.6 * 7.8"),
        1.2 + 3.4 * 5.6 * 7.8
    );

    check(
        "Test of Precedences of Operators 12",
        exevalator.eval("1.2 + 3.4 / 5.6 * 7.8"),
        1.2 + 3.4 / 5.6 * 7.8
    );

    check(
        "Test of Precedences of Operators 13",
        exevalator.eval("-1.2 + 3.4 / 5.6 * 7.8"),
        -1.2 + 3.4 / 5.6 * 7.8
    );

    check(
        "Test of Precedences of Operators 14",
        exevalator.eval("1.2 + 3.4 / -5.6 * 7.8"),
        1.2 + 3.4 / -5.6 * 7.8
    );

    check(
        "Test of Precedences of Operators 15",
        exevalator.eval("1.2 + 3.4 / 5.6 * -7.8"),
        1.2 + 3.4 / 5.6 * -7.8
    );
}


void test_parentheses() {
    Exevalator exevalator;

    check(
        "Test of Parentheses 1",
        exevalator.eval("(1.2 + 3.4)"),
        1.2 + 3.4
    );

    check(
        "Test of Parentheses 2",
        exevalator.eval("(1.2 + 3.4) + 5.6"),
        (1.2 + 3.4) + 5.6
    );

    check(
        "Test of Parentheses 3",
        exevalator.eval("1.2 + (3.4 + 5.6)"),
        1.2 + (3.4 + 5.6)
    );

    check(
        "Test of Parentheses 4",
        exevalator.eval("1.2 + -(3.4 + 5.6)"),
        1.2 + -(3.4 + 5.6)
    );

    check(
        "Test of Parentheses 5",
        exevalator.eval("1.2 + -(-3.4 + 5.6)"),
        1.2 + -(-3.4 + 5.6)
    );

    check(
        "Test of Parentheses 4",
        exevalator.eval("(1.2 * 3.4) + 5.6"),
        (1.2 * 3.4) + 5.6
    );

    check(
        "Test of Parentheses 5",
        exevalator.eval("(1.2 + 3.4) * 5.6"),
        (1.2 + 3.4) * 5.6
    );

    check(
        "Test of Parentheses 6",
        exevalator.eval("1.2 + (3.4 * 5.6)"),
        1.2 + (3.4 * 5.6)
    );

    check(
        "Test of Parentheses 7",
        exevalator.eval("1.2 + (3.4 * 5.6) + 7.8"),
        1.2 + (3.4 * 5.6) + 7.8
    );

    check(
        "Test of Parentheses 8",
        exevalator.eval("1.2 * (3.4 + 5.6) / 7.8"),
        1.2 * (3.4 + 5.6) / 7.8
    );

    check(
        "Test of Parentheses 9",
        exevalator.eval("(1.2 + 3.4 - 5.6) * ((7.8 + 9.0) / 10.1)"),
        (1.2 + 3.4 - 5.6) * ((7.8 + 9.0) / 10.1)
    );

    check(
        "Test of Parentheses 10",
        exevalator.eval("(-(1.2 + 3.4 - 5.6) * ((7.8 + 9.0) / 10.1) / 11.2 + 12.3 * ((13.4 + -(15.6 - 17.8)) * 18.9)) + 19.0 * 20.1"),
        (-(1.2 + 3.4 - 5.6) * ((7.8 + 9.0) / 10.1) / 11.2 + 12.3 * ((13.4 + -(15.6 - 17.8)) * 18.9)) + 19.0 * 20.1
    );
}


void test_complicated_cases() {
    Exevalator exevalator;

    check(
        "Test of Complicated Case 1: The Expression Containing Many Parentheses and Many Literals having Exponent Parts",
        exevalator.eval("(-(1.2E1 + 3.4E-2 - 5.6E2) * ((7.8E0 + 9.0) / 10.1E-3) / 11.2 + 12.3E-1 * ((13.4 + -(15.6E-12 - 17.8E-10)) * 18.9E-5)) + 19.0E-2 * 20.1E0"),
        (-(1.2E1 + 3.4E-2 - 5.6E2) * ((7.8E0 + 9.0) / 10.1E-3) / 11.2 + 12.3E-1 * ((13.4 + -(15.6E-12 - 17.8E-10)) * 18.9E-5)) + 19.0E-2 * 20.1E0
    );
}


void test_syntax_checks_of_corresponences_of_parentheses() {
    Exevalator exevalator;

    check(
        "Test of Detection of Mismatching of Open/Closed Parentheses 1",
        exevalator.eval("(1 + 2)"),
        1.0 + 2.0
    );

    try {
        exevalator.eval("((1 + 2)"),
        std::cerr << "Expected error has not occurred." << std::endl;
        exit(EXIT_FAILURE);
    } catch (ExevalatorException &error) {
        std::cout << "Test of Detection of Mismatching of Open/Closed Parentheses 2: OK." << std::endl;
    }

    try {
        exevalator.eval("(1 + 2))"),
        std::cerr << "Expected error has not occurred." << std::endl;
        exit(EXIT_FAILURE);
    } catch (ExevalatorException &error) {
        std::cout << "Test of Detection of Mismatching of Open/Closed Parentheses 3: OK." << std::endl;
    }

    check(
        "Test of Detection of Mismatching of Open/Closed Parentheses 4",
        exevalator.eval("(1 + 2) + (3 + 4)"),
        (1.0 + 2.0) + (3.0 + 4.0)
    );

    try {
        exevalator.eval("1 + 2) + (3 + 4"),
        std::cerr << "Expected error has not occurred." << std::endl;
        exit(EXIT_FAILURE);
    } catch (ExevalatorException &error) {
        std::cout << "Test of Detection of Mismatching of Open/Closed Parentheses 5: OK." << std::endl;
    }

    check(
        "Test of Detection of Mismatching of Open/Closed Parentheses 6",
        exevalator.eval("1 + ((2 + (3 + 4) + 5) + 6)"),
        1.0 + ((2.0 + (3.0 + 4.0) + 5.0) + 6.0)
    );

    try {
        exevalator.eval("1 + ((2 + (3 + 4) + 5) + 6"),
        std::cerr << "Expected error has not occurred." << std::endl;
        exit(EXIT_FAILURE);
    } catch (ExevalatorException &error) {
        std::cout << "Test of Detection of Mismatching of Open/Closed Parentheses 7: OK." << std::endl;
    }

    try {
        exevalator.eval("1 + (2 + (3 + 4) + 5) + 6)"),
        std::cerr << "Expected error has not occurred." << std::endl;
        exit(EXIT_FAILURE);
    } catch (ExevalatorException &error) {
        std::cout << "Test of Detection of Mismatching of Open/Closed Parentheses 8: OK." << std::endl;
    }

    try {
        exevalator.eval("()"),
        std::cerr << "Expected error has not occurred." << std::endl;
        exit(EXIT_FAILURE);
    } catch (ExevalatorException &error) {
        std::cout << "Test of Detection of Empty Parentheses 1: OK." << std::endl;
    }

    try {
        exevalator.eval("1 + ()"),
        std::cerr << "Expected error has not occurred." << std::endl;
        exit(EXIT_FAILURE);
    } catch (ExevalatorException &error) {
        std::cout << "Test of Detection of Empty Parentheses 2: OK." << std::endl;
    }

    try {
        exevalator.eval("() + 1"),
        std::cerr << "Expected error has not occurred." << std::endl;
        exit(EXIT_FAILURE);
    } catch (ExevalatorException &error) {
        std::cout << "Test of Detection of Empty Parentheses 3: OK." << std::endl;
    }
}


void test_syntax_checks_of_locations_of_operators_and_leafs() {
    Exevalator exevalator;

    check(
        "Test of Detection of Left Operand of Unary-Prefix Operator 1",
        exevalator.eval("1 + -123"),
        1.0 + -123.0
    );

    try {
        exevalator.eval("1 + -"),
        std::cerr << "Expected error has not occurred." << std::endl;
        exit(EXIT_FAILURE);
    } catch (ExevalatorException &error) {
        std::cout << "Test of Detection of Left Operand of Unary-Prefix Operator 2: OK." << std::endl;
    }

    try {
        exevalator.eval("(1 + -)"),
        std::cerr << "Expected error has not occurred." << std::endl;
        exit(EXIT_FAILURE);
    } catch (ExevalatorException &error) {
        std::cout << "Test of Detection of Left Operand of Unary-Prefix Operator 3: OK." << std::endl;
    }

    check(
        "Test of Detection of Left Operand of Binary Operator 1",
        exevalator.eval("123 + 456"),
        123.0 + 456.0
    );

    try {
        exevalator.eval("123 *"),
        std::cerr << "Expected error has not occurred." << std::endl;
        exit(EXIT_FAILURE);
    } catch (ExevalatorException &error) {
        std::cout << "Test of Detection of Left Operand of Binary Operator 2: OK." << std::endl;
    }

    try {
        exevalator.eval("* 456"),
        std::cerr << "Expected error has not occurred." << std::endl;
        exit(EXIT_FAILURE);
    } catch (ExevalatorException &error) {
        std::cout << "Test of Detection of Left Operand of Binary Operator 3: OK." << std::endl;
    }

    try {
        exevalator.eval("123 + ( * 456)"),
        std::cerr << "Expected error has not occurred." << std::endl;
        exit(EXIT_FAILURE);
    } catch (ExevalatorException &error) {
        std::cout << "Test of Detection of Left Operand of Binary Operator 4: OK." << std::endl;
    }

    try {
        exevalator.eval("(123 *) + 456"),
        std::cerr << "Expected error has not occurred." << std::endl;
        exit(EXIT_FAILURE);
    } catch (ExevalatorException &error) {
        std::cout << "Test of Detection of Left Operand of Binary Operator 5: OK." << std::endl;
    }

    try {
        exevalator.eval("123 456"),
        std::cerr << "Expected error has not occurred." << std::endl;
        exit(EXIT_FAILURE);
    } catch (ExevalatorException &error) {
        std::cout << "Test of Detection of Lacking Operator: OK." << std::endl;
    }
}


void test_variables() {
    Exevalator exevalator;

    try {
        exevalator.eval("x"),
        std::cerr << "Expected error has not occurred." << std::endl;
        exit(EXIT_FAILURE);
    } catch (ExevalatorException &error) {
        std::cout << "Test of Variables 1: OK." << std::endl;
    }

    size_t x_address = exevalator.declare_variable("x");

    check(
        "Test of Variables 2",
        exevalator.eval("x"),
        0.0
    );

    exevalator.write_variable("x", 1.25);

    check(
        "Test of Variables 3",
        exevalator.eval("x"),
        1.25
    );

    exevalator.write_variable_at(x_address, 2.5);

    check(
        "Test of Variables 4",
        exevalator.eval("x"),
        2.5
    );

    try {
        exevalator.write_variable_at(100, 5.0),
        std::cerr << "Expected error has not occurred." << std::endl;
        exit(EXIT_FAILURE);
    } catch (ExevalatorException &error) {
        std::cout << "Test of Variables 5: OK." << std::endl;
    }

    try {
        exevalator.eval("y"),
        std::cerr << "Expected error has not occurred." << std::endl;
        exit(EXIT_FAILURE);
    } catch (ExevalatorException &error) {
        std::cout << "Test of Variables 6: OK." << std::endl;
    }

    size_t y_address = exevalator.declare_variable("y");

    check(
        "Test of Variables 7",
        exevalator.eval("y"),
        0.0
    );

    exevalator.write_variable("y", 0.25);

    check(
        "Test of Variables 8",
        exevalator.eval("y"),
        0.25
    );

    exevalator.write_variable_at(y_address, 0.5);

    check(
        "Test of Variables 9",
        exevalator.eval("y"),
        0.5
    );

    check(
        "Test of Variables 10",
        exevalator.eval("x + y"),
        2.5 + 0.5
    );

    // Variables having names containing numbers
    exevalator.declare_variable("x2");
    exevalator.declare_variable("y2");
    exevalator.write_variable("x2", 22.5);
    exevalator.write_variable("y2", 32.5);
    check(
        "Test of Variables 11",
        exevalator.eval("x + y + 2 + x2 + 2 * y2"),
        2.5 + 0.5 + 2.0 + 22.5 + 2.0 * 32.5
    );
}


class FunctionA : public exevalator::ExevalatorFunctionInterface {
    double operator()(const std::vector<double> &arguments) {
        if (arguments.size() != 0) {
            throw new exevalator::ExevalatorException("Incorrect number of args");
        }
        return 1.25;
    }
};

class FunctionB : public exevalator::ExevalatorFunctionInterface {
    double operator()(const std::vector<double> &arguments) {
        if (arguments.size() != 1) {
            throw new exevalator::ExevalatorException("Incorrect number of args");
        }
        return arguments[0];
    }
};

class FunctionC : public exevalator::ExevalatorFunctionInterface {
    double operator()(const std::vector<double> &arguments) {
        if (arguments.size() != 2) {
            throw new exevalator::ExevalatorException("Incorrect number of args");
        }
        return arguments[0] + arguments[1];
    }
};

class FunctionD : public exevalator::ExevalatorFunctionInterface {
    double operator()(const std::vector<double> &arguments) {
        if (arguments.size() != 3) {
            throw new exevalator::ExevalatorException("Incorrect number of args");
        }
        if (arguments[0] != 1.25) {
            throw new exevalator::ExevalatorException("The value of arguments[0] is incorrect");
        }
        if (arguments[1] != 2.5) {
            throw new exevalator::ExevalatorException("The value of arguments[1] is incorrect");
        }
        if (arguments[2] != 5.0) {
            throw new exevalator::ExevalatorException("The value of arguments[2] is incorrect");
        }
        return 0.0;
    }
};

void test_functions() {
    Exevalator exevalator;

    try {
        exevalator.eval("funA()");
        std::cerr << "Expected error has not occurred." << std::endl;
        exit(EXIT_FAILURE);
    } catch (ExevalatorException &error) {
        std::cout << "Test of Functions 1: OK." << std::endl;
    }

    exevalator.connect_function("funA", std::make_shared<FunctionA>());
    check(
        "Test of Functions 2",
        exevalator.eval("funA()"),
        1.25
    );

    try {
        exevalator.eval("funB(2.5)");
        std::cerr << "Expected error has not occurred." << std::endl;
        exit(EXIT_FAILURE);
    } catch (ExevalatorException &error) {
        std::cout << "Test of Functions 3: OK." << std::endl;
    }

    exevalator.connect_function("funB", std::make_shared<FunctionB>());
    check(
        "Test of Functions 4",
        exevalator.eval("funB(2.5)"),
        2.5
    );

    exevalator.connect_function("funC", std::make_shared<FunctionC>());
    check(
        "Test of Functions 5",
        exevalator.eval("funC(1.25, 2.5)"),
        1.25 + 2.5
    );

    check(
        "Test of Functions 6",
        exevalator.eval("funC(funA(), funB(2.5))"),
        1.25 + 2.5
    );

    check(
        "Test of Functions 7",
        exevalator.eval("funC(funC(funA(), funB(2.5)), funB(1.0))"),
        1.25 + 2.5 + 1.0
    );

    check(
        "Test of Functions 8",
        exevalator.eval("funC(1.0, 3.5 * funB(2.5) / 2.0)"),
        1.0 + 3.5 * 2.5 / 2.0
    );

    check(
        "Test of Functions 9",
        exevalator.eval("funA() * funC(funC(funA(), 3.5 * funB(2.5) / 2.0), funB(1.0))"),
        1.25 * (1.25 + 3.5 * 2.5 / 2.0 + 1.0)
    );

    check(
        "Test of Functions 10",
        exevalator.eval("2 + 256 * funA() * funC(funC(funA(), 3.5 * funB(2.5) / 2.0), funB(1.0)) * 128"),
        2.0 + 256.0 * (1.25 * (1.25 + 3.5 * 2.5 / 2.0 + 1.0)) * 128.0
    );

    exevalator.connect_function("funD", std::make_shared<FunctionD>());
    check(
        "Test of Functions 11",
        exevalator.eval("funD(1.25, 2.5, 5.0)"),
        0.0
    );

    check(
        "Test of Functions 12",
        exevalator.eval("-funC(-1.25, -2.5)"),
        - (-1.25 + -2.5)
    );
}

void test_empty_expressions() {
    Exevalator exevalator;

    try {
        exevalator.eval("");
        std::cerr << "Expected error has not occurred." << std::endl;
        exit(EXIT_FAILURE);
    } catch (ExevalatorException &error) {
        //std::cout << error.what() << std::endl;
        std::cout << "Test of Empty Expression 1: OK." << std::endl;
    }

    try {
        exevalator.eval(" ");
        std::cerr << "Expected error has not occurred." << std::endl;
        exit(EXIT_FAILURE);
    } catch (ExevalatorException &error) {
        //std::cout << error.what() << std::endl;
        std::cout << "Test of Empty Expression 2: OK." << std::endl;
    }

    try {
        exevalator.eval("  ");
        std::cerr << "Expected error has not occurred." << std::endl;
        exit(EXIT_FAILURE);
    } catch (ExevalatorException &error) {
        //std::cout << error.what() << std::endl;
        std::cout << "Test of Empty Expression 3: OK." << std::endl;
    }

    try {
        exevalator.eval("   ");
        std::cerr << "Expected error has not occurred." << std::endl;
        exit(EXIT_FAILURE);
    } catch (ExevalatorException &error) {
        //std::cout << error.what() << std::endl;
        std::cout << "Test of Empty Expression 4: OK." << std::endl;
    }
}


void test_reeval() {
    Exevalator exevalator;

    check(
        "Test of reval() Method 1",
        exevalator.eval("1.2 + 3.4"),
        1.2 + 3.4
    );

    check(
        "Test of reval() Method 2",
        exevalator.reeval(),
        1.2 + 3.4
    );

    check(
        "Test of reval() Method 3",
        exevalator.reeval(),
        1.2 + 3.4
    );

    check(
        "Test of reval() Method 4",
        exevalator.eval("5.6 - 7.8"),
        5.6 - 7.8
    );

    check(
        "Test of reval() Method 5",
        exevalator.reeval(),
        5.6 - 7.8
    );

    check(
        "Test of reval() Method 6",
        exevalator.reeval(),
        5.6 - 7.8
    );

    check(
        "Test of reval() Method 7",
        exevalator.eval("(1.23 + 4.56) * 7.89"),
        (1.23 + 4.56) * 7.89
    );

    check(
        "Test of reval() Method 8",
        exevalator.reeval(),
        (1.23 + 4.56) * 7.89
    );

    check(
        "Test of reval() Method 9",
        exevalator.reeval(),
        (1.23 + 4.56) * 7.89
    );
}


void test_tokenizations() {
    Exevalator exevalator;

    check(
        "Test of Tokenization 1",
        exevalator.eval("1.2345678"),
        1.2345678
    );

    try {
        exevalator.eval("1.234\n5678");
        std::cerr << "Expected error has not occurred." << std::endl;
        exit(EXIT_FAILURE);
    } catch (ExevalatorException &error) {
        //std::cout << error.what() << std::endl;
        std::cout << "Test of Tokenization 2: OK." << std::endl;
    }

    try {
        exevalator.eval("1.234\r\n5678");
        std::cerr << "Expected error has not occurred." << std::endl;
        exit(EXIT_FAILURE);
    } catch (ExevalatorException &error) {
        //std::cout << error.what() << std::endl;
        std::cout << "Test of Tokenization 3: OK." << std::endl;
    }

    try {
        exevalator.eval("1.234\t5678");
        std::cerr << "Expected error has not occurred." << std::endl;
        exit(EXIT_FAILURE);
    } catch (ExevalatorException &error) {
        //std::cout << error.what() << std::endl;
        std::cout << "Test of Tokenization 4: OK." << std::endl;
    }

    try {
        exevalator.eval("1.234 5678");
        std::cerr << "Expected error has not occurred." << std::endl;
        exit(EXIT_FAILURE);
    } catch (ExevalatorException &error) {
        //std::cout << error.what() << std::endl;
        std::cout << "Test of Tokenization 5: OK." << std::endl;
    }

    check(
        "Test of Tokenization 6",
        exevalator.eval("1+2*3-4/5"),
        1.0 + 2.0 * 3.0 - 4.0 / 5.0
    );

    // !!!!! Bug about handling "\r" !!!!!
    try {
        std::cout << "|" << "1+\n2*3\r\n-4/5" << "|" << std::endl;
        exevalator.eval("1+\n2*3\r\n-4/5");
    } catch (ExevalatorException &error) {
        std::cout << error.what() << std::endl;
    }

    check(
        "Test of Tokenization 7",
        exevalator.eval("1+\n2*3\r\n-4/5"),
        1.0 + 2.0 * 3.0 - 4.0 / 5.0
    );

    check(
        "Test of Tokenization 8",
        exevalator.eval("((1+2)*3)-(4/5)"),
        ((1.0 + 2.0) * 3.0) - (4.0 / 5.0)
    );

    exevalator.connect_function("funC", std::make_shared<FunctionC>());

    check(
        "Test of Tokenization 9",
        exevalator.eval("funC(1,2)"),
        1.0 + 2.0
    );

    check(
        "Test of Tokenization 10",
        exevalator.eval("funC(\n1,\r\n2\t)"),
        1.0 + 2.0
    );

    check(
        "Test of Tokenization 11",
        exevalator.eval("3*funC(1,2)/2"),
        3.0 * (1.0 + 2.0) / 2.0
    );

    check(
        "Test of Tokenization 12",
        exevalator.eval("3*(-funC(1,2)+2)"),
        3.0 * (-(1.0 + 2.0) + 2.0)
    );
}


void check(const char *test_name, double evaluated_value, double correct_value) {
    if (fabs(evaluated_value - correct_value) < ALLOWABLE_ERROR) {
        std::cout << test_name << ": OK." << std::endl;
        return;
    } else {
        std::cerr << "\"" << test_name << "\" has failed.";
        std::cerr << " evaluated_value=" << evaluated_value << ", correct_value=" << correct_value << std::endl;
        exit(EXIT_FAILURE);
    }
}

