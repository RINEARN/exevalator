# Visual Basic&reg; (VB.NET) での Exevalator の使用方法

&raquo; [English](./README.md)


## 日本語版 目次
- [必要な環境](#requirements)
- [使用方法](#how-to-use)
- [サンプルコードの実行方法（およびコマンドラインでの使用方法）](#example-code)
- [主な機能](#features)
- [メソッド仕様一覧](#methods)
	- [コンストラクタ](#methods-constructor)
	- [Eval(expression As String) As Double](#methods-eval)
	- [Reeval() As Double](#methods-reeval)
	- [DeclareVariable(name As String) As Integer](#methods-declare-variable)
	- [WriteVariable(name As String, value As Double)](#methods-write-variable)
	- [WriteVariableAt(address As Integer, value As Double)](#methods-write-variable-at)
	- [ReadVariable(name As String) As Double](#methods-read-variable)
	- [ReadVariableAt(address As Integer) As Double](#methods-read-variable-at)
	- [ConnectFunction(name As String, function As IExevalatorFunction)](#methods-connect-function)




<a id="requirements"></a>
## 必要な環境

* Microsoft&reg; Visual Studio&reg;、 2022 以降推奨
* またはその他の Visual Basic .NET 処理系、.NET6.0 以降推奨



<a id="how-to-use"></a>
## 使用方法

Exevalator のインタープリタは、単一のファイル「 vb/Exevalator.vb 」内に実装されています。以下のような 3 ステップで読み込んでして使用できます。

### 1. 使用したいプロジェクトに Exevalator.vb を追加する

開発に Visual Studio のIDEを使用する場合（コマンドラインの場合は後述）は、まず「 vb/Exevalator.vb 」を、使用したいプロジェクト内に追加してください。

これには、ソリューションエクスプローラでプロジェクトを右クリックして、メニューから「追加」>「既存の項目」を選び、そしてファイル選択画面で Exevalator.vb を選びます。プロジェクト内のリストに Exevalator.vb が表示されるようになれば追加成功です。

### 2. プロジェクト名を確認しておく（それが暗黙的な名前空間になっている事に注意する）

Visual Basic .NET のプロジェクトのデフォルト設定では、プロジェクト内に追加したクラスやモジュールなどの要素は、暗黙的な名前空間で包まれるようになっています。その名前空間は、プロジェクト名そのものです。この事を見落とすと、直後で Exevalator を Imports して読み込む際に、エラーに悩む事になってしまします。

ここでは簡単な例として、Exevalator を使いたいプロジェクトの名前が

    YourProject

という名前であると仮定し、先のステップに進みます。実際のプロジェクト名はここで確認しておいてください。

### 3. 使用したいコードから読み込む

あとは、式の計算を行いたいコードのファイルから、以下のように Imports する事で、読み込んで使用できます：

	...
    ' 以下の YourProject の部分は、実際のプロジェクト名で置き換えてください
	using YourProject.Rinearn.ExevalatorCS;
	...

	Module YourModule

		...
		Sub YourProcess()

			' Exevalator のインタープリタを生成
			Dim exevalator As Exevalator = New Exevalator()

			' 式の値を計算（評価）する
			Dim result As Double = exevalator.Eval("1.2 + 3.4")

			' 結果を表示
			Console.WriteLine("result: " + result.ToString())

		End Sub
		...
	End Module

なお、Exevalator では、式の中のすべての数値は Double 型で扱われます。従って、結果も常に Double 型です。
ただし、式の内容がおかしい場合や、未宣言の変数を使った場合など、計算に失敗する場合もあり得ます。その場合は、Eval メソッドを呼んでいる箇所で例外 ExevalatorException がスローされますので、必要に応じて Try ... Catch ... End Try で囲ってハンドルしてください。


<a id="example-code"></a>
## サンプルコードの使用方法（およびコマンドラインでの使用方法）

このリポジトリ内には、実際に Exevalator を使用する簡単なサンプルコード類「 vb/Example*.vb 」も同梱されています。

これらも先と同様、Visual Studio のIDE上で適当なプロジェクト内に取り込んでも実行できますが、ここで即席で動かして確認するには、コマンドライン実行が便利です。方法としては、Visual Studio 付属の Developer Command Prompt（スタートボタン > Visual Studio 20** > … ）を起動し、その上でリポジトリのフォルダに cd して、以下のようにコンパイル/実行できます：

	cd vb
	vbc Example1.vb Exevalator.vb
	Example1.exe

上記の「 Example1.vb 」は、"1.2 + 3.4" の値を Exevalator で計算するサンプルコードです。内容は、すぐ前の節で掲載したサンプル「 YourModule 」とほぼ同一です：

	...
	Dim result As Double = exevalator.Eval("1.2 + 3.4")

実行結果は：

	result: 4.6

この通り、"1.2 + 3.4" の計算結果が表示されます。他のサンプルコードも、全く同様にコンパイル/実行できます。

なお、このリポジトリ内には、処理速度を測定するためのベンチマークプログラム「 vb/Benchmark.vb 」も同梱されています。これをコンパイルする際には、以下のように最適化オプションを指定してください：

	vbc -optimize Benchmark.vb Exevalator.vb

上での「 -optimize 」が最適化オプションで、忘れると Exevalator がフル性能を発揮できないため、処理速度が遅くなります。


<a id="features"></a>
## 主な機能

以下では、Exevalator の主な機能を紹介します.

### 1. 式の評価（計算）

これまでのセクションでも見てきたように、Exevalator で式の値を計算できます: 

	Dim result As Double = exevalator.Eval("(-(1.2 + 3.4) * 5) / 2")
	' result: -11.5

	(参照: vb/Example2.vb)

上記のように、"+" (足し算)、 "-" (引き算や数値のマイナス化)、"\*" (掛け算)、"/" (割り算) の演算を行えます。なお、掛け算と割り算は、足し算と引き算よりも、順序的に優先されます。


### 2. 変数の使用

変数を宣言し、その値に式の中からアクセスできます：

	' 変数を宣言して値を設定
	exevalator.DeclareVariable("x")
	exevalator.WriteVariable("x", 1.25)

	' 変数の値を使う式を計算する
	Dim result As Double = exevalator.Eval("x + 1")
	' result: 2.25

	(参照: vb/Example3.vb)

変数の値を非常に頻繁に書き変えるような用途では、書き変え対象の変数を、以下のようにアドレスによって指定する事も有用です：

	Dim address As Integer = exevalator.DeclareVariable("x")
	exevalator.WriteVariableAt(address, 1.25)
	...

	(参照: vb/Example4.vb)

この方法は、書き変え対象変数を名前で指定するよりも高速です。

### 3. 関数の使用

式の中で使用するための関数も作成できます。それには、IExevalatorFunction インターフェースを実装したクラスを作成します：

	' 式内で使用できる関数を作成
	Class MyFunction : Implements IExevalatorFunction
		Public Function invoke(arguments() As double) As Double
			If arguments.Length <> 2
				Throw New ExevalatorException("Incorrected number of args")
			End If
			return arguments(0) + arguments(1)
		End Function
	End Class
	...

	' 上記の関数を式内で使用できるよう接続
	Dim exevalator As Exevalator = New Exevalator()
	Dim fun As MyFunction = New MyFunction()
	exevalator.ConnectFunction("fun", fun)

	' 関数を使う式を計算する
	Dim result As Double = exevalator.Eval("fun(1.2, 3.4)")
	Console.WriteLine("Result: " + result.ToString())
	' result: 4.6

	(参照: vb/Example5.vb)




<a id="methods"></a>
## メソッド仕様一覧

Exevalator クラスで提供されている各メソッドの一覧と詳細仕様です。

- [コンストラクタ](#methods-constructor)
- [Eval(expression As String) As Double](#methods-eval)
- [Reeval() As Double](#methods-reeval)
- [DeclareVariable(name As string) As Integer](#methods-declare-variable)
- [WriteVariable(name As String, value As Double)](#methods-write-variable)
- [WriteVariableAt(address As Integer, value As Double)](#methods-write-variable-at)
- [ReadVariable(name As String) As Double](#methods-read-variable)
- [ReadVariableAt(address As Integer) As Double](#methods-read-variable-at)
- [ConnectFunction(name As String, function As IExevalatorFunction)](#methods-connect-function)


<a id="methods-constructor"></a>
| 形式 | (コンストラクタ) New() |
|:---|:---|
| 説明 | 新しい Exevalator のインタープリタ インスタンスを生成します。 |
| 引数 | なし |
| 戻り値 | 生成されたインスタンス |


<a id="methods-eval"></a>
| 形式 | Eval(expression As String) As Double |
|:---|:---|
| 説明 | 式の値を評価（計算）します。 |
| 引数 | expression: 評価（計算）対象の式 |
| 戻り値 | 評価（計算）結果の値 |
| 例外 | 式の評価中にエラーが発生した場合に ExevalatorException がスローされます。 |


<a id="methods-reeval"></a>
| 形式 | Reeval() As Double |
|:---|:---|
| 説明 | 前回 eval メソッドによって評価されたのと同じ式を、再評価（再計算）します。<br>このメソッドは、繰り返し使用した場合に eval メソッドよりも僅かに高速な場合があります。<br>なお、変数の値や関数の振る舞いが、前回評価時から変化している場合、式の評価結果も前回とは変わり得る事に留意してください。 |
| 引数 | なし |
| 戻り値 | 評価（計算）結果の値 |
| 例外 | 式の評価中にエラーが発生した場合に ExevalatorException がスローされます。 |


<a id="methods-declare-variable"></a>
| 形式 | DeclareVariable(name As String) As Integer |
|:---|:---|
| 説明 | 式の中で使用するための変数を、新規に宣言します。 |
| 引数 | name: 宣言する変数の名前 |
| 戻り値 | 宣言した変数に割り当てられた仮想アドレス<br>（高速に読み書きしたい場合に "WriteVariableAt" や "ReadVariableAt" メソッドで使用） |
| 例外 | 無効な変数名が指定された場合に ExevalatorException がスローされます。 |


<a id="methods-write-variable"></a>
| 形式 | WriteVariable(name As String, value As Double) |
|:---|:---|
| 説明 | 指定された名前の変数に、値を書き込みます。 |
| 引数 | name: 書き込み対象の変数の名前<br>value: 書き込む値 |
| 戻り値 | なし |
| 例外 | 指定された名前の変数が存在しない場合や、無効な変数名が指定された場合に ExevalatorException がスローされます。 |


<a id="methods-write-variable-at"></a>
| 形式 | WriteVariableAt(address As Integer, value As Double) |
|:---|:---|
| 説明 | 指定された仮想アドレスの位置にある変数に、値を書き込みます。<br>なお、このメソッドは "WriteVariable" メソッドよりも高速です。 |
| 引数 | address: 書き込み対象の変数の仮想アドレス<br>value: 書き込む値 |
| 戻り値 | なし |
| 例外 | 無効なアドレスが指定された場合に ExevalatorException がスローされます。 |


<a id="methods-read-variable"></a>
| 形式 | ReadVariable(name As String) As Double |
|:---|:---|
| 説明 | 指定された名前の変数の値を読み込みます。 |
| 引数 | name: 読み込み対称の変数の名前 |
| 戻り値 | 変数の現在の値 |
| 例外 | 指定された名前の変数が存在しない場合や、無効な変数名が指定された場合に ExevalatorException がスローされます。 |


<a id="methods-read-variable-at"></a>
| 形式 | ReadVariableAt(address As Integer) As Double |
|:---|:---|
| 説明 | 指定された仮想アドレスの位置にある変数の値を読み込みます。<br>なお、このメソッドは "ReadVariable" メソッドよりも高速です。 |
| 引数 | address: 読み込み対象の変数の仮想アドレス
| 戻り値 | 変数の現在の値 |
| 例外 | 無効なアドレスが指定された場合に ExevalatorException がスローされます。 |


<a id="methods-connect-function"></a>
| 形式 | ConnectFunction(name As String, function As IExevalatorFunction) |
|:---|:---|
| 説明 | 式の中で使用するための関数を接続します。 |
| 引数 | name: 接続する関数の名前<br>function: 関数の処理を提供する IExevalatorFunction 実装クラスのインスタンス<br>（IExevalatorFunctionインターフェースには「 Invoke(arguments() As Double) As Double 」メソッドのみが定義されており、このメソッドに関数処理を実装します） |
| 戻り値 | なし |
| 例外 | 無効な関数名が指定された場合に ExevalatorException がスローされます。 |




<hr />

<a id="credits"></a>
## 本文中の商標など

- Windows、Visual Basic、.NET、Visual Studio は、米国 Microsoft Corporation の米国およびその他の国における登録商標です。

- その他、文中に使用されている商標は、その商標を保持する各社の各国における商標または登録商標です。


