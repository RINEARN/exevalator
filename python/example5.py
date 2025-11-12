from exevalator import Exevalator, ExevalatorFunctionInterface, ExevalatorException

class MyFunction(ExevalatorFunctionInterface):
    """Function available in expressions: returns sum of two args."""
    def invoke(self, arguments: list[float]) -> float:
        if len(arguments) != 2:
            raise ExevalatorException("Incorrect number of arguments")
        return arguments[0] + arguments[1]

def main() -> None:
    """
    An example to create a function available in expressions.
    """

    # Create an instance of Exevalator Engine
    ex = Exevalator()

    # Connect the function to use it in expressions
    ex.connect_function("fun", MyFunction())

    # Evaluate the value of an expression
    result = ex.eval("fun(1.2, 3.4)")

    # Display the result
    print(f"result: {result}")   # -> result: 4.6

if __name__ == "__main__":
    main()
