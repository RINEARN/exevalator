from exevalator import Exevalator

def main() -> None:
    """
    An example to use various operators and parentheses.
    """

    # Create an instance of Exevalator Engine
    ex = Exevalator()

    # Evaluate the value of an expression
    result = ex.eval("(-(1.2 + 3.4) * 5) / 2")

    # Display the result
    print(f"result: {result}")

if __name__ == "__main__":
    main()
