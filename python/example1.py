from exevalator import Exevalator

def main() -> None:
    """ A minimal example of Exevalator usage. """

    # Create an instance of Exevalator Engine
    exevalator = Exevalator()

    # Evaluate an expression
    result = exevalator.eval("1.2 + 3.4")

    # Show the result
    print(f"result: {result}")

if __name__ == "__main__":
    main()
