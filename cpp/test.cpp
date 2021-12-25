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

