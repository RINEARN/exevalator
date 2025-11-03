from __future__ import annotations
from typing import Callable, Type
import math

from exevalator import Exevalator, ExevalatorException

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
        #self.test_syntax_checks_of_locations_of_operators_and_leafs()
        #self.test_variables()
        #self.test_functions()
        #self.test_empty_expressions()
        #self.test_reeval()
        #self.test_tokenization()

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


if __name__ == "__main__":
    Test().main()
