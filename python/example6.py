from exevalator import Exevalator

def main() -> None:
    """
    An example to compute the value of the inputted expression f(x) at the inputted x.
    """

    # Get the expression from the standard input
    print("")
    print("This program computes the value of f(x) at x.")
    print("")
    print("f(x) = ?               (default: 3*x*x + 2*x + 1)")
    expression = input()
    if len(expression) == 0:
        expression = "3*x*x + 2*x + 1"

    # Get the value of x from the standard input
    print("x = ?                  (default: 1)")
    x_value_str = input()
    x_value = 1.0
    if len(x_value_str) != 0:
        x_value = float(x_value_str)  # （Java版同様、無効入力は例外に任せる）

    # Create an instance of Exevalator Engine
    ex = Exevalator()

    # Set the value of x
    ex.declare_variable("x")
    ex.write_variable("x", x_value)

    # Compute the value of the inputted expression
    result = ex.eval(expression)

    # Display the result
    print("----------")
    print(f"f(x)   = {expression}")
    print(f"x      = {x_value}")
    print(f"result = {result}")

if __name__ == "__main__":
    main()
