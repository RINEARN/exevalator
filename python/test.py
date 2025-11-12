from __future__ import annotations
from typing import Callable, Type
import math

from exevalator import Exevalator, ExevalatorException, ExevalatorFunctionInterface

ALLOWABLE_ERROR: float = 1.0e-12


class ExevalatorTestException(RuntimeError):
    """The Exception class thrown when any test has failed."""
    pass


class Test:
    """The class for testing Exevalator."""

    def main(self) -> None:
        self.test_number_literals()
        self.test_operations_of_operators()
        self.test_precedences_of_operators()
        self.test_parentheses()
        self.test_complicated_cases()
        self.test_syntax_checks_of_correspondences_of_parentheses()
        self.test_syntax_checks_of_locations_of_operators_and_leafs()
        self.test_variables()
        self.test_functions()
        self.test_empty_expressions()
        self.test_reeval()
        self.test_tokenization()

        print("All tests have completed successfully.")


    @staticmethod
    def check(testName: str, evaluatedValue: float, correctValue: float) -> None:
        """
        Checks the evaluated value of the testing expression by the Exevalator.
        """
        if abs(evaluatedValue - correctValue) < ALLOWABLE_ERROR:
            print(f"{testName}: OK.")
            return
        raise ExevalatorTestException(
            f"\"{testName}\" has failed. "
            f"evaluatedValue={evaluatedValue}, correctValue={correctValue}."
        )


    def test_number_literals(self) -> None:
        ex = Exevalator()

        self.check(
            "Test of a Simple Number Literal 1",
            ex.eval("1"),
            1.0
        )

        self.check(
            "Test of a Simple Number Literal 2",
            ex.eval("2"),
            2.0
        )

        self.check(
            "Test of a Simple Number Literal 3",
            ex.eval("1.2"),
            1.2
        )

        self.check(
            "Test of a Number Literal with a Exponent Part 1",
            ex.eval("1.2E3"),
            1.2E3
        )

        self.check(
            "Test of a Number Literal with a Exponent Part 2",
            ex.eval("1.2E+3"),
            1.2E3
        )

        self.check(
            "Test of a Number Literal with a Exponent Part 3",
            ex.eval("1.2E-3"),
            1.2E-3
        )

        self.check(
            "Test of a Number Literal with a Exponent Part 4",
            ex.eval("123.4567E12"),
            123.4567E12
        )

        self.check(
            "Test of a Number Literal with a Exponent Part 5",
            ex.eval("123.4567E+12"),
            123.4567E+12
        )

        self.check(
            "Test of a Number Literal with a Exponent Part 6",
            ex.eval("123.4567E-12"),
            123.4567E-12
        )


    def test_operations_of_operators(self) -> None:
        ex = Exevalator()

        self.check(
            "Test of Addition Operator",
            ex.eval("1.2 + 3.4"),
            1.2 + 3.4
        )

        self.check(
            "Test of Subtraction Operator",
            ex.eval("1.2 - 3.4"),
            1.2 - 3.4
        )

        self.check(
            "Test of Multiplication Operator",
            ex.eval("1.2 * 3.4"),
            1.2 * 3.4
        )

        self.check(
            "Test of Division Operator",
            ex.eval("1.2 / 3.4"),
            1.2 / 3.4
        )

        self.check(
            "Test of Unary Minus Operator",
            ex.eval("-1.2"),
            -1.2
        )


    def test_precedences_of_operators(self) -> None:
        ex = Exevalator()

        self.check(
            "Test of Precedences of Operators 1",
            ex.eval("1.2 + 3.4 + 5.6 + 7.8"),
            1.2 + 3.4 + 5.6 + 7.8
        )

        self.check(
            "Test of Precedences of Operators 2",
            ex.eval("1.2 + 3.4 - 5.6 + 7.8"),
            1.2 + 3.4 - 5.6 + 7.8
        )

        self.check(
            "Test of Precedences of Operators 3",
            ex.eval("1.2 + 3.4 * 5.6 + 7.8"),
            1.2 + 3.4 * 5.6 + 7.8
        )

        self.check(
            "Test of Precedences of Operators 4",
            ex.eval("1.2 + 3.4 / 5.6 + 7.8"),
            1.2 + 3.4 / 5.6 + 7.8
        )

        self.check(
            "Test of Precedences of Operators 5",
            ex.eval("1.2 * 3.4 + 5.6 + 7.8"),
            1.2 * 3.4 + 5.6 + 7.8
        )

        self.check(
            "Test of Precedences of Operators 6",
            ex.eval("1.2 * 3.4 - 5.6 + 7.8"),
            1.2 * 3.4 - 5.6 + 7.8
        )

        self.check(
            "Test of Precedences of Operators 7",
            ex.eval("1.2 * 3.4 * 5.6 + 7.8"),
            1.2 * 3.4 * 5.6 + 7.8
        )

        self.check(
            "Test of Precedences of Operators 8",
            ex.eval("1.2 * 3.4 / 5.6 + 7.8"),
            1.2 * 3.4 / 5.6 + 7.8
        )

        self.check(
            "Test of Precedences of Operators 9",
            ex.eval("1.2 + 3.4 + 5.6 * 7.8"),
            1.2 + 3.4 + 5.6 * 7.8
        )

        self.check(
            "Test of Precedences of Operators 10",
            ex.eval("1.2 + 3.4 - 5.6 * 7.8"),
            1.2 + 3.4 - 5.6 * 7.8
        )

        self.check(
            "Test of Precedences of Operators 11",
            ex.eval("1.2 + 3.4 * 5.6 * 7.8"),
            1.2 + 3.4 * 5.6 * 7.8
        )

        self.check(
            "Test of Precedences of Operators 12",
            ex.eval("1.2 + 3.4 / 5.6 * 7.8"),
            1.2 + 3.4 / 5.6 * 7.8
        )

        self.check(
            "Test of Precedences of Operators 13",
            ex.eval("-1.2 + 3.4 / 5.6 * 7.8"),
            -1.2 + 3.4 / 5.6 * 7.8
        )

        self.check(
            "Test of Precedences of Operators 14",
            ex.eval("1.2 + 3.4 / -5.6 * 7.8"),
            1.2 + 3.4 / 5.6 * -7.8
        )

        self.check(
            "Test of Precedences of Operators 15",
            ex.eval("1.2 + 3.4 / 5.6 * -7.8"),
            1.2 + 3.4 / 5.6 * -7.8
        )

        self.check(
            "Test of Precedences of Operators 16",
            ex.eval("1.2*--3.4"),
            1.2*(-(-3.4))
        )

        self.check(
            "Test of Precedences of Operators 17",
            ex.eval("1.2*---3.4"),
            1.2*(-(-(-3.4)))
        )

        self.check(
            "Test of Precedences of Operators 18",
            ex.eval("1.2*----3.4"),
            1.2*(-(-(-(-3.4))))
        )

        self.check(
            "Test of Precedences of Operators 19",
            ex.eval("1.2*----3.4-5.6"),
            1.2*(-(-(-(-3.4))))-5.6
        )

        self.check(
            "Test of Precedences of Operators 20",
            ex.eval("1.2-----3.4-5.6"),
            1.2-(-(-(-(-3.4))))-5.6
        )


    def test_parentheses(self) -> None:
        ex = Exevalator()

        self.check(
            "Test of Parentheses 1",
            ex.eval("(1.2 + 3.4)"),
            (1.2 + 3.4)
        )

        self.check(
            "Test of Parentheses 2",
            ex.eval("(1.2 + 3.4) + 5.6"),
            (1.2 + 3.4) + 5.6
        )

        self.check(
            "Test of Parentheses 3",
            ex.eval("1.2 + (3.4 + 5.6)"),
            1.2 + (3.4 + 5.6)
        )

        self.check(
            "Test of Parentheses 4",
            ex.eval("1.2 + -(3.4 + 5.6)"),
            1.2 + -(3.4 + 5.6)
        )

        self.check(
            "Test of Parentheses 5",
            ex.eval("1.2 + -(-3.4 + 5.6)"),
            1.2 + -(-3.4 + 5.6)
        )

        self.check(
            "Test of Parentheses 4",
            ex.eval("(1.2 * 3.4) + 5.6"),
            (1.2 * 3.4) + 5.6
        )

        self.check(
            "Test of Parentheses 5",
            ex.eval("(1.2 + 3.4) * 5.6"),
            (1.2 + 3.4) * 5.6
        )

        self.check(
            "Test of Parentheses 6",
            ex.eval("1.2 + (3.4 * 5.6)"),
            1.2 + (3.4 * 5.6)
        )

        self.check(
            "Test of Parentheses 7",
            ex.eval("1.2 + (3.4 * 5.6) + 7.8"),
            1.2 + (3.4 * 5.6) + 7.8
        )

        self.check(
            "Test of Parentheses 8",
            ex.eval("1.2 * (3.4 + 5.6) / 7.8"),
            1.2 * (3.4 + 5.6) / 7.8
        )

        self.check(
            "Test of Parentheses 9",
            ex.eval("(1.2 + 3.4 - 5.6) * ((7.8 + 9.0) / 10.1)"),
            (1.2 + 3.4 - 5.6) * ((7.8 + 9.0) / 10.1)
        )

        self.check(
            "Test of Parentheses 10",
            ex.eval("(-(1.2 + 3.4 - 5.6) * ((7.8 + 9.0) / 10.1) / 11.2 + 12.3 * ((13.4 + -(15.6 - 17.8)) * 18.9)) + 19.0 * 20.1"),
            (-(1.2 + 3.4 - 5.6) * ((7.8 + 9.0) / 10.1) / 11.2 + 12.3 * ((13.4 + -(15.6 - 17.8)) * 18.9)) + 19.0 * 20.1
        )

        self.check(
            "Test of Parenthesis 11",
            ex.eval("1.2 + 3.4 + (5.6)"),
            1.2 + 3.4 + (5.6)
        )


    def test_complicated_cases(self) -> None:
        ex = Exevalator()

        self.check(
            "Test of Complicated Case 1: The Expression Containing Many Parentheses and Many Literals having Exponent Parts",
            ex.eval("(-(1.2E1 + 3.4E-2 - 5.6E2) * ((7.8E0 + 9.0) / 10.1E-3) / 11.2 + 12.3E-1 * ((13.4 + -(15.6E-12 - 17.8E-10)) * 18.9E-5)) + 19.0E-2 * 20.1E0"),
            (-(1.2E1 + 3.4E-2 - 5.6E2) * ((7.8E0 + 9.0) / 10.1E-3) / 11.2 + 12.3E-1 * ((13.4 + -(15.6E-12 - 17.8E-10)) * 18.9E-5)) + 19.0E-2 * 20.1E0
        )


    def test_syntax_checks_of_correspondences_of_parentheses(self) -> None:
        ex = Exevalator()

        self.check(
            "Test of Detection of Mismatching of Open/Closed Parentheses 1",
            ex.eval("(1 + 2)"),
            (1 + 2)
        )

        try:
            self.check(
                "Test of Detection of Mismatching of Open/Closed Parentheses 2",
                ex.eval("((1 + 2)"),
                (1 + 2)
            )
            raise ExevalatorTestException("Expected exception has not been thrown");
        except ExevalatorException as ee:
            # Expected to be thrown
            print("Test of Detection of Mismatching of Open/Closed Parentheses 2: OK.");

        try:
            self.check(
                "Test of Detection of Mismatching of Open/Closed Parentheses 3",
                ex.eval("(1 + 2))"),
                (1 + 2)
            )
            raise ExevalatorTestException("Expected exception has not been thrown");
        except ExevalatorException as ee:
            # Expected to be thrown
            print("Test of Detection of Mismatching of Open/Closed Parentheses 3: OK.");

        self.check(
            "Test of Detection of Mismatching of Open/Closed Parentheses 4",
            ex.eval("(1 + 2) + (3 + 4)"),
            (1 + 2) + (3 + 4)
        )

        try:
            self.check(
                "Test of Detection of Mismatching of Open/Closed Parentheses 5",
                ex.eval("1 + 2) + (3 + 4"),
                (1 + 2) + (3 + 4)
            )
            raise ExevalatorTestException("Expected exception has not been thrown");
        except ExevalatorException as ee:
            # Expected to be thrown
            print("Test of Detection of Mismatching of Open/Closed Parentheses 5: OK.");

        self.check(
            "Test of Detection of Mismatching of Open/Closed Parentheses 6",
            ex.eval("1 + ((2 + (3 + 4) + 5) + 6)"),
            1 + ((2 + (3 + 4) + 5) + 6)
        )

        try:
            self.check(
                "Test of Detection of Mismatching of Open/Closed Parentheses 7",
                ex.eval("1 + ((2 + (3 + 4) + 5) + 6"),
                1 + ((2 + (3 + 4) + 5) + 6)
            )
            raise ExevalatorTestException("Expected exception has not been thrown");
        except ExevalatorException as ee:
            # Expected to be thrown
            print("Test of Detection of Mismatching of Open/Closed Parentheses 7: OK.");

        try:
            self.check(
                "Test of Detection of Mismatching of Open/Closed Parentheses 8",
                ex.eval("1 + (2 + (3 + 4) + 5) + 6)"),
                1 + ((2 + (3 + 4) + 5) + 6)
            )
            raise ExevalatorTestException("Expected exception has not been thrown");
        except ExevalatorException as ee:
            # Expected to be thrown
            print("Test of Detection of Mismatching of Open/Closed Parentheses 8: OK.");

        try:
            self.check(
                "Test of Detection of Empty Parentheses 1",
                ex.eval("()"),
                math.nan
            )
            raise ExevalatorTestException("Expected exception has not been thrown");
        except ExevalatorException as ee:
            # Expected to be thrown
            print("Test of Detection of Empty Parentheses 1: OK.");

        try:
            self.check(
                "Test of Detection of Empty Parentheses 2",
                ex.eval("1 + ()"),
                math.nan
            )
            raise ExevalatorTestException("Expected exception has not been thrown");
        except ExevalatorException as ee:
            # Expected to be thrown
            print("Test of Detection of Empty Parentheses 2: OK.");

        try:
            self.check(
                "Test of Detection of Empty Parentheses 3",
                ex.eval("() + 1"),
                math.nan
            )
            raise ExevalatorTestException("Expected exception has not been thrown");
        except ExevalatorException as ee:
            # Expected to be thrown
            print("Test of Detection of Empty Parentheses 3: OK.");


    def test_syntax_checks_of_locations_of_operators_and_leafs(self) -> None:
        ex = Exevalator()

        self.check(
            "Test of Detection of Left Operand of Unary-Prefix Operator 1",
            ex.eval("1 + -123"),
            1 + -123
        )

        try:
            self.check(
                "Test of Detection of Left Operand of Unary-Prefix Operator 2",
                ex.eval("1 + -"),
                1 + -123
            )
            raise ExevalatorTestException("Expected exception has not been thrown")
        except ExevalatorException as ee:
            # Expected to be thrown
            print("Test of Detection of Left Operand of Unary-Prefix Operator 2: OK.")

        try:
            self.check(
                "Test of Detection of Left Operand of Unary-Prefix Operator 3",
                ex.eval("(1 + -)"),
                1 + -123
            )
            raise ExevalatorTestException("Expected exception has not been thrown")
        except ExevalatorException as ee:
            # Expected to be thrown
            print("Test of Detection of Left Operand of Unary-Prefix Operator 3: OK.")

        self.check(
            "Test of Detection of Left Operand of Binary Operator 1",
            ex.eval("123 + 456"),
            123 + 456
        )

        try:
            self.check(
                "Test of Detection of Left Operand of Binary Operator 2",
                ex.eval("123 *"),
                123 * 456
            )
            raise ExevalatorTestException("Expected exception has not been thrown")
        except ExevalatorException as ee:
            # Expected to be thrown
            print("Test of Detection of Left Operand of Binary Operator 2: OK.")

        try:
            self.check(
                "Test of Detection of Left Operand of Binary Operator 3",
                ex.eval("* 456"),
                123 * 456
            )
            raise ExevalatorTestException("Expected exception has not been thrown")
        except ExevalatorException as ee:
            # Expected to be thrown
            print("Test of Detection of Left Operand of Binary Operator 3: OK.")

        try:
            self.check(
                "Test of Detection of Left Operand of Binary Operator 4",
                ex.eval("123 + ( * 456)"),
                123 * 456
            )
            raise ExevalatorTestException("Expected exception has not been thrown")
        except ExevalatorException as ee:
            # Expected to be thrown
            print("Test of Detection of Left Operand of Binary Operator 4: OK.")

        try:
            self.check(
                "Test of Detection of Left Operand of Binary Operator 5",
                ex.eval("(123 *) + 456"),
                123 * 456
            )
            raise ExevalatorTestException("Expected exception has not been thrown")
        except ExevalatorException as ee:
            # Expected to be thrown
            print("Test of Detection of Left Operand of Binary Operator 5: OK.")

        try:
            self.check(
                "Test of Detection of Lacking Operator",
                ex.eval("123 456"),
                123 * 456
            )
            raise ExevalatorTestException("Expected exception has not been thrown")
        except ExevalatorException as ee:
            # Expected to be thrown
            print("Test of Detection of Lacking Operator: OK.")


    def test_variables(self) -> None:
        ex = Exevalator()

        try:
            ex.eval("x")
            raise ExevalatorTestException("Expected exception has not been thrown")
        except ExevalatorException as ee:
            # Expected to be thrown
            print("Test of Variables 1: OK.")

        xAddress = ex.declare_variable("x")

        self.check(
            "Test of Variables 2",
            ex.eval("x"),
            0.0
        )

        ex.write_variable("x", 1.25)

        self.check(
            "Test of Variables 3",
            ex.eval("x"),
            1.25
        )

        ex.write_variable_at(xAddress, 2.5)

        self.check(
            "Test of Variables 4",
            ex.eval("x"),
            2.5
        )

        try:
            ex.write_variable_at(100, 5.0)
            raise ExevalatorTestException("Expected exception has not been thrown")
        except ExevalatorException as ee:
            # Expected to be thrown
            print("Test of Variables 5: OK.")

        try:
            ex.eval("y")
            raise ExevalatorTestException("Expected exception has not been thrown")
        except ExevalatorException as ee:
            # Expected to be thrown
            print("Test of Variables 6: OK.")

        yAddress = ex.declare_variable("y")

        self.check(
            "Test of Variables 7",
            ex.eval("y"),
            0.0
        )

        ex.write_variable("y", 0.25)

        self.check(
            "Test of Variables 8",
            ex.eval("y"),
            0.25
        )

        ex.write_variable_at(yAddress, 0.5)

        self.check(
            "Test of Variables 9",
            ex.eval("y"),
            0.5
        )

        self.check(
            "Test of Variables 10",
            ex.eval("x + y"),
            2.5 + 0.5
        )

        # Variables having names containing numbers
        ex.declare_variable("x2")
        ex.declare_variable("y2")
        ex.write_variable("x2", 22.5)
        ex.write_variable("y2", 32.5)
        self.check(
            "Test of Variables 11",
            ex.eval("x + y + 2 + x2 + 2 * y2"),
            2.5 + 0.5 + 2.0 + 22.5 + 2.0 * 32.5
        )


    def test_functions(self) -> None:
        ex = Exevalator()

        try:
            ex.eval("funA()");
            raise ExevalatorTestException("Expected exception has not been thrown")
        except ExevalatorException as ee:
            # Expected to be thrown
            print("Test of Functions 1: OK.")

        ex.connect_function("funA", FunctionA())
        self.check(
            "Test of Functions 2",
            ex.eval("funA()"),
            1.25
        )

        try:
            ex.eval("funB(2.5)")
            raise ExevalatorTestException("Expected exception has not been thrown")
        except ExevalatorException as ee:
            # Expected to be thrown
            print("Test of Functions 3: OK.")

        ex.connect_function("funB", FunctionB())
        self.check(
            "Test of Functions 4",
            ex.eval("funB(2.5)"),
            2.5
        )

        ex.connect_function("funC", FunctionC())
        self.check(
            "Test of Functions 5",
            ex.eval("funC(1.25, 2.5)"),
            1.25 + 2.5
        )

        self.check(
            "Test of Functions 6",
            ex.eval("funC(funA(), funB(2.5))"),
            1.25 + 2.5
        )

        self.check(
            "Test of Functions 7",
            ex.eval("funC(funC(funA(), funB(2.5)), funB(1.0))"),
            1.25 + 2.5 + 1.0
        )
        
        self.check(
            "Test of Function 8",
            ex.eval("funC(1.0, 3.5 * funB(2.5) / 2.0)"),
            1.0 + 3.5 * 2.5 / 2.0
        )
        
        self.check(
            "Test of Functions 9",
            ex.eval("funA() * funC(funC(funA(), 3.5 * funB(2.5) / 2.0), funB(1.0))"),
            1.25 * (1.25 + 3.5 * 2.5 / 2.0 + 1.0)
        )

        self.check(
            "Test of Functions 10",
            ex.eval("2 + 256 * funA() * funC(funC(funA(), 3.5 * funB(2.5) / 2.0), funB(1.0)) * 128"),
            2.0 + 256.0 * (1.25 * (1.25 + 3.5 * 2.5 / 2.0 + 1.0)) * 128.0
        )

        ex.connect_function("funD", FunctionD())
        self.check(
            "Test of Functions 11",
            ex.eval("funD(1.25, 2.5, 5.0)"),
            0.0
        )

        self.check(
            "Test of Functions 12",
            ex.eval("-funC(-1.25, -2.5)"),
            - (-1.25 + -2.5)
        )


    def test_empty_expressions(self) -> None:
        ex = Exevalator()

        try:
            ex.eval("")
            raise ExevalatorTestException("Expected exception has not been thrown")
        except ExevalatorException as ee:
            # Expected to be thrown
            print("Test of Empty Expression 1: OK.")

        try:
            ex.eval(" ")
            raise ExevalatorTestException("Expected exception has not been thrown")
        except ExevalatorException as ee:
            # Expected to be thrown
            print("Test of Empty Expression 2: OK.")

        try:
            ex.eval("  ")
            raise ExevalatorTestException("Expected exception has not been thrown")
        except ExevalatorException as ee:
            # Expected to be thrown
            print("Test of Empty Expression 3: OK.")

        try:
            ex.eval("   ")
            raise ExevalatorTestException("Expected exception has not been thrown")
        except ExevalatorException as ee:
            # Expected to be thrown
            print("Test of Empty Expression 4: OK.")


    def test_reeval(self) -> None:
        ex = Exevalator()

        self.check(
            "Test of reval() Method 1",
            ex.eval("1.2 + 3.4"),
            1.2 + 3.4
        )

        self.check(
            "Test of reval() Method 2",
            ex.reeval(),
            1.2 + 3.4
        )

        self.check(
            "Test of reval() Method 3",
            ex.reeval(),
            1.2 + 3.4
        )

        self.check(
            "Test of reval() Method 4",
            ex.eval("5.6 - 7.8"),
            5.6 - 7.8
        )

        self.check(
            "Test of reval() Method 5",
            ex.reeval(),
            5.6 - 7.8
        )

        self.check(
            "Test of reval() Method 6",
            ex.reeval(),
            5.6 - 7.8
        )

        self.check(
            "Test of reval() Method 7",
            ex.eval("(1.23 + 4.56) * 7.89"),
            (1.23 + 4.56) * 7.89
        )

        self.check(
            "Test of reval() Method 8",
            ex.reeval(),
            (1.23 + 4.56) * 7.89
        )

        self.check(
            "Test of reval() Method 9",
            ex.reeval(),
            (1.23 + 4.56) * 7.89
        )


    def test_tokenization(self) -> None:
        ex = Exevalator()

        self.check(
            "Test of Tokenization 1",
            ex.eval("1.2345678"),
            1.2345678
        )

        try:
            ex.eval("1.234\n5678")
            raise ExevalatorTestException("Expected exception has not been thrown");
        except ExevalatorException as ee:
            # Expected to be thrown
            print("Test of Tokenization 2: OK.")

        try:
            ex.eval("1.234\r\n5678")
            raise ExevalatorTestException("Expected exception has not been thrown");
        except ExevalatorException as ee:
            # Expected to be thrown
            print("Test of Tokenization 3: OK.")

        try:
            ex.eval("1.234\t5678")
            raise ExevalatorTestException("Expected exception has not been thrown");
        except ExevalatorException as ee:
            # Expected to be thrown
            print("Test of Tokenization 4: OK.")

        try:
            ex.eval("1.234 5678")
            raise ExevalatorTestException("Expected exception has not been thrown");
        except ExevalatorException as ee:
            # Expected to be thrown
            print("Test of Tokenization 5: OK.")

        self.check(
            "Test of Tokenization 6",
            ex.eval("1+2*3-4/5"),
            1.0 + 2.0 * 3.0 - 4.0 / 5.0
        )

        self.check(
            "Test of Tokenization 7",
            ex.eval("1+\n2*3\r\n-4/5"),
            1.0 + 2.0 * 3.0 - 4.0 / 5.0
        )

        self.check(
            "Test of Tokenization 8",
            ex.eval("((1+2)*3)-(4/5)"),
            ((1.0 + 2.0) * 3.0) - (4.0 / 5.0)
        )

        funC: FunctionC = FunctionC()
        ex.connect_function("funC", funC)

        self.check(
            "Test of Tokenization 9",
            ex.eval("funC(1,2)"),
            1.0 + 2.0
        )

        self.check(
            "Test of Tokenization 10",
            ex.eval("funC(\n1,\r\n2\t)"),
            1.0 + 2.0
        )

        self.check(
            "Test of Tokenization 11",
            ex.eval("3*funC(1,2)/2"),
            3.0 * (1.0 + 2.0) / 2.0
        )

        self.check(
            "Test of Tokenization 12",
            ex.eval("3*(-funC(1,2)+2)"),
            3.0 * (-(1.0 + 2.0) + 2.0)
        )


class FunctionA(ExevalatorFunctionInterface):
    def invoke(self, arguments: list[float]) -> float:
        if len(arguments) != 0:
            raise ExevalatorException("Incorrect number of arguments")
        return 1.25


class FunctionB(ExevalatorFunctionInterface):
    def invoke(self, arguments: list[float]) -> float:
        if len(arguments) != 1:
            raise ExevalatorException("Incorrect number of arguments")
        return arguments[0]


class FunctionC(ExevalatorFunctionInterface):
    def invoke(self, arguments: list[float]) -> float:
        if len(arguments) != 2:
            raise ExevalatorException("Incorrect number of arguments")
        return arguments[0] + arguments[1]


class FunctionD(ExevalatorFunctionInterface):
    def invoke(self, arguments: list[float]) -> float:
        if len(arguments) != 3:
            raise ExevalatorException("Incorrect number of arguments")
        if arguments[0] != 1.25:
            raise ExevalatorException("The value of args[0] is incorrect")
        if arguments[1] != 2.5:
            raise ExevalatorException("The value of args[1] is incorrect")
        if arguments[2] != 5.0:
            raise ExevalatorException("The value of args[2] is incorrect")
        return 0.0


if __name__ == "__main__":
    Test().main()
