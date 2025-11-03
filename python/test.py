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
        #self.test_precedences_of_operators()
        #self.test_parentheses()
        #self.test_complicated_cases()
        #self.test_syntax_checks_of_correspondences_of_parentheses()
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


if __name__ == "__main__":
    Test().main()
