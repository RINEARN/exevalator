# Using Exevalator with MCP

&raquo; [Japanese](./README_JAPANESE.md)

&raquo; [Ask the AI for help (ChatGPT account required)](https://chatgpt.com/g/g-UjkEJeO8x-exevalator-assistant)


## Table of Contents

- [Introduction](#introduction)
- [Requirements](#requirements)
- [How to Use](#how-to-use)
- [Features](#features)
- [Acknowledgements (External Components / SDKs)](#acknowledgement)



<a id="introduction"></a>
## Introduction

### With MCP, you can use Exevalator as a calculation tool for AI agents

AI has advanced rapidly in recent years, and we're entering an era where AI systems can use various tools and autonomously drive tasks. One framework for implementing and providing such "AI-usable tools" is MCP: Model Context Protocol.

Exevalator supports MCP, so it can be used as a calculation tool by AI agents.


### When Exevalator is preferable to script execution

Modern AI can already write and run complex scripts in Python and other languages. You might therefore think there's no need to use Exevalator "just for expression evaluation."

However, depending on your use case and environment, you may not want to grant AI the permission to execute arbitrary scripts for security (or governance) reasons. Under such constraints, if you want to allow only expression evaluation, Exevalator is a good fit.

Exevalator raises an error if anything other than expression evaluation is attempted. Even within expressions, only pre-registered functions can be called. This makes it suitable to offer as a safe calculation tool for AI.


<a id="requirements"></a>
## Requirements

* An AI environment that supports MCP (e.g., Visual Studio Code + Cline extension)
* uv
* Git

Installed as dependencies (via the steps below):

* Python 3.9 – 3.13, or later
* MCP Python SDK



<a id="how-to-use"></a>
## How to Use

The following example assumes **Ubuntu Linux with Visual Studio Code + Cline** as the AI environment. Adjust details to match your actual setup.

### Set up the tool in a fixed location and prepare the environment

Create a directory for MCP tools under your home:

    cd ~
    mkdir mcp-tools
    cd mcp-tools/

Clone the Exevalator repository here:

    git clone https://github.com/RINEARN/exevalator.git

    # A directory named `exevalator` will be created.

Move into Exevalator's MCP directory:

    cd ./exevalator/mcp/

Use uv to set up the required runtime:

    uv init -p 3.13 --name exevalator-mcp .
    uv add "mcp[cli]"

The -p 3.13 part specifies the Python version. Change it as needed (Exevalator works with 3.9 or later).

Now try running it:

    uv run exevalator-mcp.py

If you see no errors and it sits idle waiting, it's working. Press Ctrl+C a couple of times to exit.

### Register it in your AI environment

Next, register the tool so your AI environment can use it.

The UI to register MCP servers varies by environment. On Ubuntu Linux with VS Code + Cline, do the following:

- In VS Code's left icon rail, click the Cline icon to open the Cline side panel.
- Click **Manage MCP Servers** near the lower-left of that panel.
- In the panel that opens, click the gear icon (top-right).
- Click **Configure MCP Servers**.

This opens `cline_mcp_settings.json`. Under "mcpServers", append an entry for the Exevalator command you set up:

    {
        "mcpServers": {
            ...
            // Existing MCP tool entries (if any)
            ...

            "Exevalator": {
                "command": "uv",
                "args": [
                    "--directory", "/home/your-user-name/mcp-tools/exevalator/mcp/",
                    "run", "exevalator-mcp.py"
                ]
            }

        }
    }
Replace "/home/your-user-name/mcp-tools/exevalator/mcp/" with the actual path where you placed Exevalator. Following the steps above, `your-user-name` should be your system user name.

When you're done editing, click **Done** in the Cline panel on the left. The tool should now be registered as an MCP server.


### How to Use

Let's try it out. In Cline's chat panel, enter something like:

    Using the `Exevalator` MCP server, compute the value of "1.2 + 3.4". 
    Do not use any other method. Make sure to use `Exevalator`.

We explicitly say "use Exevalator" because otherwise the AI may prefer to write and run a script.

Depending on your permission settings, Cline may request approval to use the MCP tool, showing a message like:

    Cline wants to use a tool on the `Exevalator` MCP server:

    evaluate_expression
    ...
    ARGUMENTS

    {
        "expression": "1.2 + 3.4"
    }

Click **Approve**. You should then see a response, for example:

    Responce
    4.6

And the AI's final answer might read:

    I computed "1.2 + 3.4" via the `evaluate_expression` tool on the `Exevalator` MCP server and confirmed the result is 4.6.
    As requested, the calculation was performed using `Exevalator`.

This confirms the AI can correctly obtain results through the tool.




<a id="features"></a>
## Features

Below are the main features of the Exevalator MCP tool.


### 1. Expression evaluation

As shown above, Exevalator can evaluate expressions such as:

    Using the `Exevalator` MCP server, compute the value of "1.2 + 3.4". 
    Do not use any other method. Make sure to use `Exevalator`.

A typical final answer would be:

    I computed "1.2 + 3.4" via the `evaluate_expression` tool on the `Exevalator` MCP server and confirmed the result is 4.6.
    As requested, the calculation was performed using `Exevalator`.

The operators `+` (addition), `-` (subtraction and unary minus), `*` (multiplication), and `/` (division) are supported. Multiplication and division take precedence over addition and subtraction.


### 2. Using variables

You can declare variables, assign values, and use them in expressions.

    Using the `Exevalator` MCP server, perform the following steps:

    - Declare variables x, result1, and result2.
    - Set x = 3.4.
    - Evaluate "1.2 + x" and store the result in result1.
    - Set x = 100.
    - Evaluate "2 * x" and store the result in result2.

    When done, report the values of result1 and result2.

The AI will issue a sequence of tool calls to manipulate variables and evaluate expressions, then respond, for example:

    The procedure is complete. The values are:

    - result1 = 4.6
    - result2 = 200.0

Note: Depending on the model's capabilities and behavior, it may try to skip steps it deems unnecessary (e.g., storing intermediate results in result1/result2 if it can answer without them). Skipping such steps can affect later computations that rely on stored values, so you may need to adjust instructions and/or the model choice and test accordingly.


### 3. Registering and using functions

You can register any number of custom functions and call them from expressions.

For security reasons, custom function implementation/registration is not exposed via the AI. Instead, edit your local file at
`/home/your-user-name/mcp-tools/exevalator/mcp/exevalator-mcp.py`.

Open that file and look for the `init_exevalator()` function near the top. It includes a commented example showing how to register a custom function:

    ...

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

Uncommenting the above will implement and register `myfunc(a, b)`, which returns the sum of `a` and `b`:

        class MyFunction(ExevalatorFunctionInterface):
            def invoke(self, arguments: list[float]) -> float:
                if len(arguments) != 2:
                    raise ExevalatorException("Incorrect number of arguments")
                return arguments[0] + arguments[1]
        
        exevalator.connect_function("myfunc", MyFunction())

In short, to add a function:

- Create a class that inherits from `ExevalatorFunctionInterface`.
- Implement the function’s logic.
- Register it in Exevalator via `connect_function`.

You can add as many functions as you need.

Try using the registered `myfunc`:

    Using the `Exevalator` MCP server, evaluate the expression "myfunc(1.2, 3.4)". 
    The function `myfunc` should already be registered and available.

A typical final answer will be:

    The value of myfunc(1.2, 3.4) is 4.6. It was correctly evaluated using the registered `myfunc` on the `Exevalator` MCP server.

This confirms that `myfunc` is being called correctly.


<a id="acknowledgement"></a>
## Acknowledgements (External Components / SDKs)

This tool uses the following third-party SDK at runtime (not bundled in this package):

### MCP Python SDK
[https://github.com/modelcontextprotocol/python-sdk](https://github.com/modelcontextprotocol/python-sdk)

- Copyright (c) 2024 Anthropic, PBC
- License：[MIT](https://github.com/modelcontextprotocol/python-sdk/blob/main/LICENSE)


<hr />

<a id="credits"></a>
## Credits, Trademarks and Attributions

- MCP (Model Context Protocol) is a communication protocol proposed by Anthropic, PBC.

- Visual Studio Code is either a registered trademarks or trademarks of Microsoft Corporation in the United States and/or other countries.

- Cline is an AI tool by Cline Bot Inc.

- Ubuntu is a trademark or registered trademark of Canonical Ltd. in the United States and/or other countries.

- Linux is a trademark or registered trademark of Linus Torvalds in the United States and/or other countries. 

- Git is a trademark or registered trademark of Software Freedom Conservancy, Inc. in the United States and/or other countries.

- Python is a registered trademark of the Python Software Foundation in the United States and other countries.

- uv is a Python package management tool by Astral.

- ChatGPT is a trademark or registered trademark of OpenAI OpCo, LLC in the United States and/or other countries.

- Other names and marks may be trademarks or registered trademarks of their respective owners.
