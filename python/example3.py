from exevalator import Exevalator

def main() -> None:
    """
    An example to use a variable.
    """

    # Create an instance of Exevalator Engine
    ex = Exevalator()

    # Declare a variable and set the value
    ex.declare_variable("x")
    ex.write_variable("x", 1.25)

    # Evaluate the value of an expression
    result = ex.eval("x + 1")

    # Display the result
    print(f"result: {result}")

if __name__ == "__main__":
    main()
