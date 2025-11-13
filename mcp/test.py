from __future__ import annotations
import math

import exevalator_mcp
import exevalator_mcp_math_preset

ALLOWABLE_ERROR: float = 1.0e-12


class ExevalatorTestException(RuntimeError):
    """The Exception class thrown when any test has failed."""
    pass


class Test:
    """The class for testing Exevalator."""

    def main(self) -> None:
        exevalator_mcp.init_exevalator()
        exevalator_mcp_math_preset.init_exevalator()

        self.test_default_impl()
        self.test_math_preset()

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


    def test_default_impl(self) -> None:
        self.check(
            "Test of Expression Evaluation",
            exevalator_mcp.evaluate_expression("1.2 + 3.4"),
            1.2 + 3.4
        )

        exevalator_mcp.declare_new_variable("x")
        exevalator_mcp.set_variable_value("x", 123.0)
        self.check(
            "Test of Variable Set/Get",
            exevalator_mcp.get_variable_value("x"),
            123.0
        )

        exevalator_mcp.set_variable_value("x", 456.0)
        self.check(
            "Test of Variable Re-Set/Get",
            exevalator_mcp.get_variable_value("x"),
            456.0
        )


    def test_math_preset(self) -> None:
        self.check(
            "Test of pi",
            exevalator_mcp_math_preset.evaluate_expression("pi"),
            math.pi
        )

        self.check(
            "Test of sin(x)",
            exevalator_mcp_math_preset.evaluate_expression("sin(1.23)"),
            math.sin(1.23)
        )

        self.check(
            "Test of cos(x)",
            exevalator_mcp_math_preset.evaluate_expression("cos(1.23)"),
            math.cos(1.23)
        )

        self.check(
            "Test of tan(x)",
            exevalator_mcp_math_preset.evaluate_expression("tan(1.23)"),
            math.tan(1.23)
        )

        self.check(
            "Test of asin(x)",
            exevalator_mcp_math_preset.evaluate_expression("asin(0.5)"),
            math.asin(0.5)
        )

        self.check(
            "Test of acos(x)",
            exevalator_mcp_math_preset.evaluate_expression("acos(0.5)"),
            math.acos(0.5)
        )

        self.check(
            "Test of atan(x)",
            exevalator_mcp_math_preset.evaluate_expression("atan(0.5)"),
            math.atan(0.5)
        )

        self.check(
            "Test of abs(x)",
            exevalator_mcp_math_preset.evaluate_expression("abs(-0.5)"),
            abs(-0.5)
        )

        self.check(
            "Test of sqrt(x)",
            exevalator_mcp_math_preset.evaluate_expression("sqrt(1.23)"),
            math.sqrt(1.23)
        )

        self.check(
            "Test of pow(x,p)",
            exevalator_mcp_math_preset.evaluate_expression("pow(1.23, 4.56)"),
            math.pow(1.23, 4.56)
        )

        self.check(
            "Test of ln(x)",
            exevalator_mcp_math_preset.evaluate_expression("ln(1.23)"),
            math.log(1.23)
        )

        self.check(
            "Test of log10(x)",
            exevalator_mcp_math_preset.evaluate_expression("log10(1.23)"),
            math.log10(1.23)
        )

        self.check(
            "Test of log2(x)",
            exevalator_mcp_math_preset.evaluate_expression("log2(1.23)"),
            math.log2(1.23)
        )


if __name__ == "__main__":
    Test().main()
