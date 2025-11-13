# Local MCP server providing Exevalator as a tool for AI agents.
# (Math Functions Preset Edition)

from mcp.server.fastmcp import FastMCP
from exevalator import Exevalator, ExevalatorException, ExevalatorFunctionInterface
import math

mcp = FastMCP("Exevalator MCP (Math Preset)")
exevalator = None


def init_exevalator() -> None:
    """
    Initializes the exevalator instance.
    """
    global exevalator
    exevalator = Exevalator()

    # As needed, implement and register any functions you want to use in expressions.  
    # The example below defines and registers `myfunc(a, b)`, which returns the sum a + b.
    # ---
    #
    # class MyFunction(ExevalatorFunctionInterface):
    #     def invoke(self, arguments: list[float]) -> float:
    #         if len(arguments) != 2:
    #             raise ExevalatorException("Incorrect number of arguments")
    #         return arguments[0] + arguments[1]
    #
    # exevalator.connect_function("myfunc", MyFunction())

    # Register sin(x) function
    class SinFunction(ExevalatorFunctionInterface):
        def invoke(self, arguments: list[float]) -> float:
            if len(arguments) != 1:
                raise ExevalatorException("Incorrect number of arguments")
            return math.sin(arguments[0])
    exevalator.connect_function("sin", SinFunction())

    # Register cos(x) function
    class CosFunction(ExevalatorFunctionInterface):
        def invoke(self, arguments: list[float]) -> float:
            if len(arguments) != 1:
                raise ExevalatorException("Incorrect number of arguments")
            return math.cos(arguments[0])
    exevalator.connect_function("cos", CosFunction())

    # Register tan(x) function
    class TanFunction(ExevalatorFunctionInterface):
        def invoke(self, arguments: list[float]) -> float:
            if len(arguments) != 1:
                raise ExevalatorException("Incorrect number of arguments")
            return math.tan(arguments[0])
    exevalator.connect_function("tan", TanFunction())

    # Register asin(x) function
    class AsinFunction(ExevalatorFunctionInterface):
        def invoke(self, arguments: list[float]) -> float:
            if len(arguments) != 1:
                raise ExevalatorException("Incorrect number of arguments")
            return math.asin(arguments[0])
    exevalator.connect_function("asin", AsinFunction())

    # Register acos(x) function
    class AcosFunction(ExevalatorFunctionInterface):
        def invoke(self, arguments: list[float]) -> float:
            if len(arguments) != 1:
                raise ExevalatorException("Incorrect number of arguments")
            return math.acos(arguments[0])
    exevalator.connect_function("acos", AcosFunction())

    # Register atan(x) function
    class AtanFunction(ExevalatorFunctionInterface):
        def invoke(self, arguments: list[float]) -> float:
            if len(arguments) != 1:
                raise ExevalatorException("Incorrect number of arguments")
            return math.atan(arguments[0])
    exevalator.connect_function("atan", AtanFunction())

    # Register abs(x) function
    class AbsFunction(ExevalatorFunctionInterface):
        def invoke(self, arguments: list[float]) -> float:
            if len(arguments) != 1:
                raise ExevalatorException("Incorrect number of arguments")
            return abs(arguments[0])
    exevalator.connect_function("abs", AbsFunction())

    # Register sqrt(x) function
    class SqrtFunction(ExevalatorFunctionInterface):
        def invoke(self, arguments: list[float]) -> float:
            if len(arguments) != 1:
                raise ExevalatorException("Incorrect number of arguments")
            return math.sqrt(arguments[0])
    exevalator.connect_function("sqrt", SqrtFunction())

    # Register pow(x, p) function
    class PowFunction(ExevalatorFunctionInterface):
        def invoke(self, arguments: list[float]) -> float:
            if len(arguments) != 2:
                raise ExevalatorException("Incorrect number of arguments")
            return math.pow(arguments[0], arguments[1])
    exevalator.connect_function("pow", PowFunction())

    # Register exp(x) function
    class ExpFunction(ExevalatorFunctionInterface):
        def invoke(self, arguments: list[float]) -> float:
            if len(arguments) != 1:
                raise ExevalatorException("Incorrect number of arguments")
            return math.exp(arguments[0])
    exevalator.connect_function("exp", ExpFunction())

    # Register ln(x) functions
    class LnFunction(ExevalatorFunctionInterface):
        def invoke(self, arguments: list[float]) -> float:
            if len(arguments) != 1:
                raise ExevalatorException("Incorrect number of arguments")
            return math.log(arguments[0])
    exevalator.connect_function("ln", LnFunction())

    # Register log10(x) function
    class Log10Function(ExevalatorFunctionInterface):
        def invoke(self, arguments: list[float]) -> float:
            if len(arguments) != 1:
                raise ExevalatorException("Incorrect number of arguments")
            return math.log10(arguments[0])
    exevalator.connect_function("log10", Log10Function())

    # Register log2(x) function
    class Log2Function(ExevalatorFunctionInterface):
        def invoke(self, arguments: list[float]) -> float:
            if len(arguments) != 1:
                raise ExevalatorException("Incorrect number of arguments")
            return math.log2(arguments[0])
    exevalator.connect_function("log2", Log2Function())

    # Register variable `pi` (or `PI`)
    exevalator.declare_variable("pi")
    exevalator.write_variable("pi", math.pi)
    exevalator.declare_variable("PI")
    exevalator.write_variable("PI", math.pi)


@mcp.tool()
def evaluate_expression(expression: str) -> float:
    """
    Evaluates an arithmetic/math expression.

    In expressions, you can use the following functions:
    sin(x), cos(x), tan(x), asin(x), acos(x), atan(x), 
    abs(x), sqrt(x), pow(x, p), exp(x), ln(x), log10(x), log2(x).
    The variable `pi` (or `PI`) is also available.

    Important note: Use ln(x) instead of log(x).

    Args:
        expression (str): The expression to evaluate (e.g., "1.2 + 3.4", "ln(1.2) + pow(3,4)", "sin(pi/2)", etc.).

    Returns:
        float: The computed value (double-precision).
    """
    result = exevalator.eval(expression)
    return result


@mcp.tool()
def declare_new_variable(variable_name: str) -> None:
    """
    Declares a new variable, for using the value of it in expressions.

    Args:
        variable_name (str): The name of the variable to be declared.
    """
    exevalator.declare_variable(variable_name)


@mcp.tool()
def set_variable_value(variable_name: str, variable_value: float) -> None:
    """
    Sets the value to the variable having the specified name.

    Args:
        variable_name (str): The name of the variable.
        variable_value (float): The new value of the variable.
    """
    exevalator.write_variable(variable_name, variable_value)


@mcp.tool()
def get_variable_value(variable_name: str) -> float:
    """
    Gets the current value of the variable having the specified name.

    Args:
        name (str): The name of the variable.

    Returns:
        float: The current value of the variable.
    """
    return exevalator.read_variable(variable_name)


if __name__ == "__main__":
    init_exevalator()
    mcp.run(transport="stdio")
