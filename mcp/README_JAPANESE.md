# MCP での Exevalator の使用方法

&raquo; [English](./README.md)

&raquo; [AIに使い方を聞く（ChatGPTのアカウントが必要）](https://chatgpt.com/g/g-UjkEJeO8x-exevalator-assistant)


## 日本語版 目次

- [はじめに](#introduction)
- [必要な環境](#requirements)
- [使用方法](#how-to-use)
- [主な機能](#features)
- [謝辞（使用している外部コンポーネント、SDK等）](#acknowledgement)


<a id="introduction"></a>
## はじめに

### MCPによって、Exevalator をAI用の計算ツールとして使える

近年はAIの進化が著しく、AI自身が各種ツールを使いこなして、自律的にタスクを進行する時代になりつつあります。そのような用途で、「AI用のツール」を実装して提供するための枠組みの一つが、MCP: Model Context Protocol です。

Exevalator はMCPでの操作をサポートしており、AI用の計算ツールとして使用する事が可能です。

### スクリプト処理と比べて、Exevalator の使用が有効な場合

なお、現在のAIは、Python等で複雑なスクリプトを記述・実行する能力も十分にあります。そのため、わざわざ「式の計算」だけのために Exevalator を使わせる必要性は無いように思えるかもしれません。

しかし、用途や運用環境によっては、セキュリティ等の観点から、「AIに、任意のスクリプト処理が可能な権限を与えたくない」というケースもあり得ます。そのような条件下で、「式の計算だけは可能にしたい」といった場合に、Exevalator が有効です。

Exevalator は、式の計算以外の処理を行わせようとするとエラーになります。また、式の中でも、あらかじめ登録した関数以外は呼べないようになっています。そのため、安全な計算ツールとしてAIに提供できます。


<a id="requirements"></a>
## 必要な環境

* MCPをサポートしているAI利用環境（例： Visual Studio Code + Cline 拡張）
* uv
* Git

依存環境として（後述の手順で導入）：

* Python 3.9 – 3.13, またはそれ以降
* MCP Python SDK



<a id="how-to-use"></a>
## 使用方法

ここでは、Ubuntu Linux 上で、AI利用環境として Visual Studio Code + Cline 拡張を使用している場合について例示します。ご自身の実際の環境に応じて、細部を調整してください。

### MCPツールとして、定位置に配置して環境構築する

まず、MCPツールの置き場を、ユーザーの home 直下に作ります：

    cd ~
    mkdir mcp-tools
    cd mcp-tools/

ここに Exevalator のリポジトリを clone します：

    git clone https://github.com/RINEARN/exevalator.git

    # `exevalator` というディレクトリができます。

Exevalator のMCP用ディレクトリ内に移動します：

    cd ./exevalator/mcp/

このディレクトリ内に、uv を用いて必要な動作環境を構築します：

    uv init -p 3.13 --name exevalator-mcp .
    uv add "mcp[cli]"

上の `-p 3.13` の箇所は、Python のバージョンを指定しています。必要に応じて自由に指定してください（3.9以降で動作可能です）。

これで導入は完了です。実行してみます：

    uv run exevalator_mcp.py

これで何もエラーが出ず、無言待機状態になれば成功です。`Ctrl+C` の連打で終了させます。

### AI利用環境に登録する

続いて、上記をAI利用環境から使えるように登録します。

登録UIの開き方は環境によりますが、Ubuntu Linux 上での Visual Studio Code + Cline 拡張では以下の通りです：

- まず Visual Studio Code の左端のアイコン列から、Cline 拡張のアイコンをクリックし、Cline の操作カラムを出現させる
- その左下付近にある「Manage MCP Servers」のアイコンをクリック
- 立ち上がるパネルの右上にある歯車アイコンをクリック
- 「Configure MCP Servers」をクリック

これで `cline_mcp_settings.json` が開きます。その中の "mcpServers" 配下の末尾に、先ほど配置した Exevalator の起動コマンドを追記します：

    {
        "mcpServers": {
            ...
            ここに既存のMCPツールの記述が記載されている（あれば）
            ...

            "Exevalator": {
                "command": "uv",
                "args": [
                    "--directory", "/home/your-user-name/mcp-tools/exevalator/mcp/",
                    "run", "exevalator_mcp.py"
                ]
            }

        }
    }

ここで `/home/your-user-name/mcp-tools/exevalator/mcp/` の部分は、実際に Exevalator を配置した場所で置き換えてください。先ほどの手順の通りだと、 `your-user-name` の箇所はPCのユーザー名になるはずです。

なお、上の設定内容では、数学関数などが使用できない、最小構成の Exevalator MCPツールが登録されます。式の中で数学関数を使用したい場合には、"exevalator_mcp.py" の代わりに "exevalator_mcp_math_preset.py" を指定してください：

    {
        "mcpServers": {
            ...
            ここに既存のMCPツールの記述が記載されている（あれば）
            ...

            "Exevalator": {
                "command": "uv",
                "args": [
                    "--directory", "/home/your-user-name/mcp-tools/exevalator/mcp/",
                    "run", "exevalator_mcp_math_preset.py"
                ]
            }

        }
    }

以上のように編集が完了したら、画面左のCline操作列から「Done」ボタンを押すと、MCPツールとして登録されるはずです。

### 使用する

実際に使用してみましょう。Cline のチャット欄に、以下のように入力します：

    MCPサーバーの Exevalator を用いて、「1.2 + 3.4」の値を計算してください。
    他の方法ではなく、必ず exevalator を用いてください。

このように「Exevalator を用いて」と念押ししているのは、そうしないと、スクリプトを書いて実行しようとしがちだからです。

さて、権限設定によっては、以下のようなメッセージと共に、MCPツールを使用する許可がリクエストされます：

    Cline wants to use a tool on the `Exevalator` MCP server:

    evaluate_expression
    ...
    ARGUMENTS

    {
        "expression": "1.2 + 3.4"
    }

ここで「Approve (許可)」を押します。すると

    Responce
    4.6

のようにレスポンスが返った旨が表示され、続けてAIが、以下のように最終回答をしてくれます：

    Exevalator MCPサーバーの evaluate_expression で「1.2 + 3.4」を計算し、結果は 4.6 であることを確認しました。
    指定どおり Exevalator を用いて算出しています。

このように、AI側からも正しく結果を取得できている事がわかります。




<a id="features"></a>
## 主な機能

以下では、Exevalator MCPツールの主な機能を紹介します.


### 1. 式の評価（計算）

これまでのセクションでも見てきたように、Exevalator で式の値を計算できます: 

    MCPサーバーの Exevalator を用いて、「1.2 + 3.4」の値を計算してください。
    他の方法ではなく、必ず Exevalator を用いてください。

応答は：

    Exevalator MCPサーバーの evaluate_expression で「1.2 + 3.4」を計算し、結果は 4.6 であることを確認しました。
    指定どおり Exevalator を用いて算出しています。

上記のように、"+" (足し算)、 "-" (引き算や数値のマイナス化)、"\*" (掛け算)、"/" (割り算) の演算を行えます。なお、掛け算と割り算は、足し算と引き算よりも、順序的に優先されます。


### 2. 変数の使用

変数を宣言し、値を設定して、式の計算で使用できます：

    MCPサーバーの Exevalator を用いて、以下の一連の計算手続きを行ってください。

    - 変数 x, result1, result2 を宣言します。
    - 変数 x の値を 3.4 に設定します。
    - 式「1.2 + x」を計算し、結果を result1 に設定します。
    - 変数 x の値を 100 に設定します。
    - 式「2 * x」を計算し、結果を result2 に設定します。

    完了時に、変数 result1 と result2 の値を教えてください。

すると、変数の操作や計算のリクエストが次々と走り、AIが以下の最終回答をしてくれます。

    計算手順を完了し、以下の値を報告します。

    - result1 = 4.6
    - result2 = 200.0

正しい結果が得られています。

ただ、使用するAIモデルの賢さや性格によっては、AIが手順の一部を勝手にサボったりする事があります。例えば上記で、計算結果を result1, result2 に格納する箇所などです。サボっても回答できてしまうため、不要と判断されがちなようです。

手順の一部がサボられると、後でその結果を使った計算を行う際に影響するので、指示の言い回しやモデル選択などで対処・テストする必要があります。


### 3. 関数の登録と使用

Exevalator では、いくつでも自作の関数を登録し、計算式の中で使用できます。

ただし、関数の自作や登録は、AI側からの操作ではできません（セキュリティ上もその方が安全です）。

そのため、最初にPC内に配置した `/home/your-user-name/mcp-tools/exevalator/mcp/` 内にある、`exevalator_mcp.py` を開いて、中身を編集して登録します。冒頭付近に init_exevalator() 関数があり、その中に、自作関数の登録例がコメントアウトされた形で記載されています：

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

ここのコメントアウトを外すと、「 引数 a, b の和を返す関数 myfunc(a,b) 」の実装と登録処理が走るようになります：

        class MyFunction(ExevalatorFunctionInterface):
            def invoke(self, arguments: list[float]) -> float:
                if len(arguments) != 2:
                    raise ExevalatorException("Incorrect number of arguments")
                return arguments[0] + arguments[1]
        
        exevalator.connect_function("myfunc", MyFunction())

このように、関数の自作や登録は：

- 抽象クラス ExevalatorFunctionInterface を継承したクラスを作成して、
- 関数の内部処理を実装し、
- それを Exevalator に connect_function で登録する

という流れで行います。これを参考に、必要な関数を実装・登録してください。何個でも可能です。

上で登録した myfunc 関数を、実際に使用してみましょう：

    MCPサーバーの Exevalator を用いて、式「myfunc(1.2, 3.4)」の値を計算してください。
    関数 myfunc は、登録されていて使えるようになっているはずです。

すると計算リクエストが走り、以下のようにAIが最終回答してくれます：

    式 myfunc(1.2, 3.4) の値は 4.6 です。
    Exevalator MCPサーバー上で登録済みの myfunc を用いて正しく評価しました。

正しく関数 myfunc を呼べている事がわかります。


### 4. 数学関数の使用

sqrt(x) や sin(x) などの数学関数は頻繁に使用されるため、いちいち登録するのは面倒ですよね。

そこで、AI利用環境に Exevalator を登録する際、 `exevalator_mcp.py` の代わりに `exevalator_mcp_math_preset.py` を登録すると、よく使われる数学関数があらかじめ登録されている状態になります。

    {
        "mcpServers": {
            ...
            ここに既存のMCPツールの記述が記載されている（あれば）
            ...

            "Exevalator": {
                "command": "uv",
                "args": [
                    "--directory", "/home/your-user-name/mcp-tools/exevalator/mcp/",
                    "run", "exevalator_mcp_math_preset.py"
                ]
            }

        }
    }

使える数学関数の一覧は sin(x), cos(x), tan(x), asin(x), acos(x), atan(x), abs(x), sqrt(x), pow(x, p), exp(x), ln(x), log10(x), log2(x) です。また、円周率の変数 pi も使用できます。

> 自然対数は log(x) ではなく ln(x) である事に注意してください。

実際に使用してみましょう：

    MCPサーバーの Exevalator を用いて、以下の式を計算してください：
    pow(sin(1.2), 2) + pow(cos(1.2), 2)

すると Exevalator に計算リクエストが投げられ、レスポンスが返ります。
AIの最終回答は：

    式 pow(sin(1.2), 2) + pow(cos(1.2), 2) の計算結果は 1.0 です。三角恒等式 sin^2(x) + cos^2(x) = 1 に従い、任意の実数 x で 1 になります。

きちんと計算できていますね。

前項で解説したのと同様、`exevalator_mcp_math_preset.py` を開いて編集する事で、使える関数をさらに追加する事も可能です。自由にカスタマイズしてください！


<a id="acknowledgement"></a>
## 謝辞（使用している外部コンポーネント、SDK等）

本ツールは、実行時に以下のサードパーティSDKを利用しています（本パッケージ内に同梱はしていません）：

### MCP Python SDK
[https://github.com/modelcontextprotocol/python-sdk](https://github.com/modelcontextprotocol/python-sdk)

- Copyright (c) 2024 Anthropic, PBC
- License：[MIT](https://github.com/modelcontextprotocol/python-sdk/blob/main/LICENSE)


<hr />

<a id="credits"></a>
## 本文中の商標など

- MCP (Model Context Protocol) は、Anthropic 社 (Anthropic, PBC) が提唱した通信プロトコルです。

- Visual Studio Code は、米国 Microsoft Corporation の米国およびその他の国における登録商標です。

- Cline は、米国 Cline Bot Inc. によるAIツールです。

- Ubuntu は、Canonical Ltd. の米国およびその他の国における商標または登録商標です。 

- Linux は、Linus Torvalds 氏の米国およびその他の国における商標または登録商標です。 

- Git は、Software Freedom Conservancy, Inc. の米国およびその他の国における商標または登録商標です。

- Python は、Python Software Foundation の米国及びその他の国における登録商標です。

- uv は、Astral社によるPython用のパッケージ管理ツールです。

- ChatGPT は、米国 OpenAI OpCo, LLC による米国またはその他の国における商標または登録商標です。

- その他、文中に使用されている商標は、その商標を保持する各社の各国における商標または登録商標です。
