# Python での Exevalator の使用方法

&raquo; [English](./README.md)

&raquo; [AIに使い方を聞く（ChatGPTのアカウントが必要）](https://chatgpt.com/g/g-UjkEJeO8x-exevalator-assistant)

## 日本語版 目次

- [必要な環境](#requirements)
- [使用方法](#how-to-use)
- [サンプルコードの実行方法](#example-code)
- [主な機能](#features)
- [メソッド仕様一覧](#methods)
	- [Constructor](#methods-constructor)
	- [eval(expression: str) -> float](#methods-eval)
	- [reeval() -> float](#methods-reeval)
	- [declare_variable(name: str) -> int](#methods-declare-variable)
	- [write_variable(name: str, value: float) -> None](#methods-write-variable)
	- [write_variable_at(address: int, value: float) -> None](#methods-write-variable-at)
	- [read_variable(name: str) -> float](#methods-read-variable)
	- [read_variable_at(address: int) -> float](#methods-read-variable-at)
	- [void connect_function(name: str, function: FunctionInterface) -> None](#methods-connect-function)




<a id="requirements"></a>
## 必要な環境

* Python 3.9 – 3.13, またはそれ以降 (CPython).



<a id="how-to-use"></a>
## 使用方法

Exevalator のインタープリタは、単一のファイル「 python/exevalator.py 」内に実装されています。そのため、以下のような 3 ステップで、簡単に導入して使用できます。

### 1. 使用したいプロジェクトのソースコードフォルダ内に配置

まず「 python/exevalator.java 」を、使用したいプロジェクトのソースコードフォルダ内の、好きな場所に配置します：

	your_project/src/exevalator.py

### 2. 使いたいコードから import する

	from exevalator import Exevalator, ExevalatorException

### 3. 式を計算する

以上で準備完了です！ これで、以下のように式の計算を実行できます：

	# Exevalator のインタープリタを生成
	ex = Exevalator()

	# 式の値を計算（評価）する
	result = ex.eval("1.2 + 3.4")

	# 結果を表示
	print(f"result: {result}")   # -> result: 4.6

なお、Exevalator では、式の中のすべての数値は double 型で扱われます。従って、結果も常に double 型です。

ただし、式の内容がおかしい場合や、未宣言の変数を使った場合など、計算に失敗する場合もあり得ます。その場合は、eval メソッドを呼んでいる箇所で例外 ExevalatorException が raise されますので、実用用途では、必要に応じて try: ～ except ExevalatorException で囲ってエラーハンドルを行ってください。


<a id="example-code"></a>
## サンプルコードの使用方法

このリポジトリ内には、実際に Exevalator を使用する簡単なサンプルコード類「 python/example*.py 」も同梱されています。
それらは、以下のように実行できます:

	cd python
	python example1.py

上記の「 example1.py 」は、"1.2 + 3.4" の値を Exevalator で計算するサンプルコードです。内容は、すぐ前の節で掲載したサンプルコードとほぼ同一です：

	...
	result = ex.eval("1.2 + 3.4")

実行結果は：

	result: 4.6

この通り、"1.2 + 3.4" の計算結果が表示されます。他のサンプルコードも、全く同様にコンパイル/実行できます。

なお、このリポジトリ内には、処理速度を測定するためのベンチマークプログラム「 python/benchmark.py 」も同梱されています。そちらも全く同様にコンパイル/実行できます。

> 処理速度に関する留意点:
>
> Exevalator は、コンパイラ型言語での実装版や、JITを備えるスクリプト言語処理系用の実装版では、数百 MFLOPS程度の水準の速度が出ます。
>
> 一方、Python版では、処理速度は 1 ～ 2 桁程度遅くなります（数 M ～ 10 MFLOPS 程度の水準、標準的なCPythonの環境下）。この速度差には、採用時に留意が必要です。ただし、言語標準の eval よりは格段に高速です。


<a id="features"></a>
## 主な機能

以下では、Exevalator の主な機能を紹介します.

### 1. 式の評価（計算）

これまでのセクションでも見てきたように、Exevalator で式の値を計算できます: 

	from exevalator import Exevalator

	ex = Exevalator()
	result = ex.eval("(-(1.2 + 3.4) * 5) / 2")

	print(result)  # -> -11.5

	# 参照: python/example2.py

上記のように、"+" (足し算)、 "-" (引き算や数値のマイナス化)、"\*" (掛け算)、"/" (割り算) の演算を行えます。なお、掛け算と割り算は、足し算と引き算よりも、順序的に優先されます。


### 2. 変数の使用

アプリの実装時に、Exevalator の declareVariable メソッドを使用して、変数を宣言できます。宣言した変数は、式の中で自由にアクセスできます：

	# 変数を宣言して値を設定
	ex.declare_variable("x")
	ex.write_variable("x", 1.25)

	# 変数の値を使う式を計算する
	result = ex.eval("x + 1")
	print(result)  # -> 2.25

	# 参照: python/example3.py

変数の値を非常に頻繁に書き変えるような用途では、書き変え対象の変数を、以下のようにアドレスによって指定する事も有用です：

	addr = ex.declare_variable("x")
	ex.write_variable_at(addr, 1.25)  # faster than name-based write

	result = ex.eval("x + 1")

	# 参照: python/example4.py

この方法は、書き変え対象変数を名前で指定するよりも高速です。


### 3. 関数の使用

式の中で使用するための関数も作成できます。それには、抽象クラス FunctionInterface を実装したクラスを作成します：

	from exevalator import Exevalator, FunctionInterface, ExevalatorException

	# 式内で使用できる関数を作成
	class MyFunction(FunctionInterface):
    	def invoke(self, arguments: list[float]) -> float:
		if len(arguments) != 2:
			raise ExevalatorException("Incorrect number of arguments")
		return arguments[0] + arguments[1]

	# 上記の関数を式内で使用できるよう接続
	ex = Exevalator()
	ex.connect_function("fun", MyFunction())

	# 関数を使う式を計算する
	result = ex.eval("fun(1.2, 3.4)")
	print(result)  # -> 4.6

	# 参照: python/example5.py


<a id="methods"></a>
## メソッド仕様一覧

Exevalator クラスで提供されている各メソッドの一覧と詳細仕様です。

* [コンストラクタ](#methods-constructor)
- [eval(expression: str) -> float](#methods-eval)
- [reeval() -> float](#methods-reeval)
- [declare_variable(name: str) -> int](#methods-declare-variable)
- [write_variable(name: str, value: float) -> None](#methods-write-variable)
- [write_variable_at(address: int, value: float) -> None](#methods-write-variable-at)
- [read_variable(name: str) -> float](#methods-read-variable)
- [read_variable_at(address: int) -> float](#methods-read-variable-at)
- [connect_function(name: str, function: FunctionInterface) -> None](#methods-connect-function)


<a id="methods-constructor"></a>
| 形式 | (コンストラクタ) Exevalator() |
|:---|:---|
| 説明 | 新しい Exevalator のインタープリタ インスタンスを生成します。 |
| 引数 | なし |
| 戻り値 | 生成されたインスタンス |


<a id="methods-eval"></a>
| 形式 | eval(expression: str) -> float |
|:---|:---|
| 説明 | 式の値を評価（計算）します。 |
| 引数 | expression (str型): 評価（計算）対象の式 |
| 戻り値 | float型: 評価（計算）結果の値 |
| 例外 | 式の評価中にエラーが発生した場合に ExevalatorException がスローされます。 |


<a id="methods-reeval"></a>
| 形式 | reeval() -> float |
|:---|:---|
| 説明 | 前回 `eval` メソッドによって評価されたのと同じ式を、再評価（再計算）します。<br>このメソッドは、繰り返し使用した場合に `eval` メソッドよりも僅かに高速な場合があります。<br>なお、変数の値や関数の振る舞いが、前回評価時から変化している場合、式の評価結果も前回とは変わり得る事に留意してください。 |
| 引数 | なし |
| 戻り値 | float型: 評価（計算）結果の値 |
| 例外 | 式の評価中にエラーが発生した場合に ExevalatorException がスローされます。 |


<a id="methods-declare-variable"></a>
| 形式 | declare_variable(name: str) -> int |
|:---|:---|
| 説明 | 式の中で使用するための変数を、新規に宣言します。 |
| 引数 | name: 宣言する変数の名前 |
| 戻り値 | 宣言した変数に割り当てられた仮想アドレス<br>（高速に読み書きしたい場合に write_variable_at や read_variable_at メソッドで使用） |
| 例外 | 無効な変数名が指定された場合に ExevalatorException がスローされます。 |


<a id="methods-write-variable"></a>
| 形式 | write_variable(name: str, value: float) -> None |
|:---|:---|
| 説明 | 指定された名前の変数に、値を書き込みます。 |
| 引数 | name: 書き込み対象の変数の名前<br>value: 書き込む値 |
| 戻り値 | なし |
| 例外 | 指定された名前の変数が存在しない場合や、無効な変数名が指定された場合に ExevalatorException がスローされます。 |


<a id="methods-write-variable-at"></a>
| 形式 | write_variable_at(address: int, value: float) -> None |
|:---|:---|
| 説明 | 指定された仮想アドレスの位置にある変数に、値を書き込みます。<br>なお、このメソッドは write_variable メソッドよりも高速です。 |
| 引数 | address: 書き込み対象の変数の仮想アドレス<br>value: 書き込む値 |
| 戻り値 | なし |
| 例外 | 無効なアドレスが指定された場合に ExevalatorException がスローされます。 |


<a id="methods-read-variable"></a>
| 形式 | read_variable(name: str) -> float |
|:---|:---|
| 説明 | 指定された名前の変数の値を読み込みます。 |
| 引数 | name: 読み込み対称の変数の名前 |
| 戻り値 | 変数の現在の値 |
| 例外 | 指定された名前の変数が存在しない場合や、無効な変数名が指定された場合に ExevalatorException がスローされます。 |


<a id="methods-read-variable-at"></a>
| 形式 | read_variable_at(address: int) -> float |
|:---|:---|
| 説明 | 指定された仮想アドレスの位置にある変数の値を読み込みます。<br>なお、このメソッドは read_variable メソッドよりも高速です。 |
| 引数 | address: 読み込み対象の変数の仮想アドレス
| 戻り値 | 変数の現在の値 |
| 例外 | 無効なアドレスが指定された場合に ExevalatorException がスローされます。 |


<a id="methods-connect-function"></a>
| 形式 | connect_function(name: str, function: FunctionInterface) -> None |
|:---|:---|
| 説明 | 式の中で使用するための関数を接続します。 |
| 引数 | name: 接続する関数の名前<br>function: 関数の処理を提供する ExevalatorFunctionInterface 実装クラスのインスタンス<br>（FunctionInterface には `invoke(self, arguments: List[float]) -> float` メソッドのみが定義されており、このメソッドに関数処理を実装します） |
| 戻り値 | なし |
| 例外 | 無効な関数名が指定された場合に ExevalatorException がスローされます。 |





<hr />

<a id="credits"></a>
## 本文中の商標など

- Python は、 Python Software Foundation の米国及びその他の国における登録商標です。

- ChatGPT は、米国 OpenAI OpCo, LLC による米国またはその他の国における商標または登録商標です。

- その他、文中に使用されている商標は、その商標を保持する各社の各国における商標または登録商標です。


