from __future__ import annotations
from exevalator import Exevalator
import sys

def main() -> None:
    """
    An example to compute the numerical integration value of the inputted expression f(x).
    Integration uses Simpson's rule over uniform partitions.
    """

    # Get the expression from the standard input
    print("")
    print("This program computes integrated value of f(x) from lower-limit to upper-limit.")
    print("")
    print("f(x) = ?               (default: 3*x*x + 2*x + 1)")
    expression = input()
    if len(expression) == 0:
        expression = "3*x*x + 2*x + 1"

    # Get the lower limit
    print("lower-limit = ?        (default: 0)")
    lower_limit_str = input()
    lower_limit = 0.0
    if len(lower_limit_str) != 0:
        try:
            lower_limit = float(lower_limit_str)
        except ValueError:
            print(f"Invalid lower-limit value: {lower_limit_str}", file=sys.stderr)
            return

    # Get the upper limit
    print("upper-limit = ?        (default: 1)")
    upper_limit_str = input()
    upper_limit = 1.0
    if len(upper_limit_str) != 0:
        try:
            upper_limit = float(upper_limit_str)
        except ValueError:
            print(f"Invalid upper-limit value: {upper_limit_str}", file=sys.stderr)
            return

    # Other numerical integration parameters
    number_of_steps = 65536
    delta = (upper_limit - lower_limit) / number_of_steps
    result = 0.0

    # Create an instance of Exevalator Engine and declare x
    ex = Exevalator()
    x_addr = ex.declare_variable("x")

    # Traverse tiny intervals from lower-limit to upper-limit (Simpson's rule)
    for i in range(number_of_steps):
        x = lower_limit + i * delta

        ex.write_variable_at(x_addr, x)
        fx_left = ex.eval(expression)

        ex.write_variable_at(x_addr, x + delta)
        fx_right = ex.eval(expression)

        ex.write_variable_at(x_addr, x + delta / 2.0)
        fx_center = ex.eval(expression)

        result += (fx_left + fx_right + 4.0 * fx_center) * delta / 6.0

    # Display the result
    print("----------")
    print(f"f(x)        = {expression}")
    print(f"lower-limit = {lower_limit}")
    print(f"upper-limit = {upper_limit}")
    print(f"result      = {result}")

if __name__ == "__main__":
    main()