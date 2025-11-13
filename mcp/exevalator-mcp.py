# Local MCP server providing Exevalator as a tool for AI agents.

from mcp.server.fastmcp import FastMCP
from exevalator import Exevalator, ExevalatorException, ExevalatorFunctionInterface
import math

mcp = FastMCP("Exevalator MCP")
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


@mcp.tool()
def evaluate_expression(expression: str) -> float:
    """
    Evaluates an arithmetic/math expression.

    Args:
        expression (str): The expression to evaluate (e.g., "1.2 + 3.4").

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
