# C++ での Exevalator の使用方法

&raquo; [English](./README.md)

&raquo; [AIに使い方を聞く（ChatGPTのアカウントが必要）](https://chatgpt.com/g/g-UjkEJeO8x-exevalator-assistant)

## 日本語版 目次
- [必要な環境](#requirements)
- [使用方法](#how-to-use)
- [サンプルコードの実行方法](#example-code)
- [主な機能](#features)
- [メソッド仕様一覧](#methods)
	- [コンストラクタ](#methods-constructor)
	- [double eval(const std::string &expression)](#methods-eval)
	- [double reeval()](#methods-reeval)
	- [size_t declare_variable(const std::string &name)](#methods-declare-variable)
	- [void write_variable(const std::string &name, double value)](#methods-write-variable)
	- [void write_variable_at(size_t address, double value)](#methods-write-variable-at)
	- [double read_variable(const std::string &name)](#methods-read-variable)
	- [double read_variable_at(size_t address)](#methods-read-variable-at)
	- [size_t connect_function(const std::string &name, const std::shared_ptr&lt;ExevalatorFunctionInterface&gt; &function_ptr)](#methods-connect-function)




<a id="requirements"></a>
## 必要な環境

* C++17以降に対応可能なC++コンパイラ<br />( このドキュメント内の作業例では、Linux 上で clang++ を使用します。 )


<a id="how-to-use"></a>
## 使用方法

Exevalator のインタープリタは、ファイル「 cpp/exevalator.cpp 」内に実装されています。ヘッダファイルは「 cpp/exevalator.hpp 」として分割されています。Exevalator を使用するのに必要なのは、これら 2 つのファイルだけです。以下では例として、実際に使用してみるまでの流れを掲載します。


### 1. 開発プログラムのソースコードフォルダ内に配置

まずは上記のファイルを、Exevalator を使用したいプログラムの、ソースコードフォルダ内に配置してください。

フォルダ分けやビルド処理などが、ある程度整理されたプロジェクトでは、恐らく「 exevalator.hpp 」はヘッダ用のフォルダ内に、「 cpp/exevalator.cpp 」は実装コード用のフォルダ内に配置して、ビルドファイル等を適切に編集する必要があります。

しかし、もっと非常に単純な、例えば「 1 個のフォルダ内に数枚のソースファイルがあって、main 関数のあるソースからそれらを include しているだけ 」といった場合には、そのフォルダ内に「 exevalator.hpp 」と「 exevalator.cpp 」を一緒に放り込んでください。


### 2. 使用したいコードから読み込んで使用する

あとは、式の計算を行いたいコードから include すれば使用できます。特に分割コンパイル等や行儀作法を深く考えない場合は、以下のように「 exevalator.hpp 」と「 exevalator.cpp 」を一緒に読み込んでしまうのが手短です：

	#include <iostream>
	#include <cstdlib>
	#include "exevalator.hpp"
	#include "exevalator.cpp"

	int main() {

		// Exevalator のインタープリタを生成
		exevalator::Exevalator exevalator;

		try {

			// 式の値を計算（評価）する
			double result = exevalator.eval("1.2 + 3.4");
			
			// 結果を表示
			std::cout << "result: " << result << std::endl;

		// 式の計算でエラーが発生した場合
		} catch (exevalator::ExevalatorException &e) {
			std::cout << "Error occurred: " << e.what() << std::endl;
			return EXIT_FAILURE;
		}

		return EXIT_SUCCESS;
	}

上記のコードは、Exevalator を用いて、式「 1.2 + 3.4 」の値を計算し、結果を表示する内容になっています。このコードのファイル名を example.cpp としましょう。コンパイル/実行するには：

	clang++ -std=c++17 -o example example.cpp
	./example

結果は:

	result: 4.6

と、この通り「 1.2 + 3.4 」の正しい値を計算できた事がわかります。

なお、Exevalator では、式の中のすべての数値は double 型で扱われます。従って、結果も常に double 型です。
ただし、式の内容がおかしい場合や、未宣言の変数を使った場合など、計算に失敗する場合もあり得ます。その際には eval メソッドを呼んでいる箇所で例外 ExevalatorException がスローされます。従って、上記のように catch してハンドルするようにしてください。

### 3. 分割コンパイルしたい場合（必須の内容ではありません）

実務的なC++のプロジェクトにおいては、まず各ソースファイルをモジュールとして分割コンパイルし、それらを後工程でリンクする事で、実行ファイルを生成する場合も多いと思います。そのような場合は、Exevalator を使用するソースファイルからはヘッダ「 exevalator.hpp 」のみを include します。そして、exevalator.cpp は単体でコンパイルしてモジュールにし、最後に実行ファイル生成時にリンクするようにします。

この例に沿ってビルドする例を示しましょう。まず先ほどの example.cpp の include 部分を以下のように改変します：

	#include <iostream>
	#include <cstdlib>
	#include "exevalator.hpp" // .cpp の方は include しない

	int main() {

		// Exevalator のインタープリタを生成
		exevalator::Exevalator exevalator;
	...

そして上記と exevalator.cpp は別々に、それぞれモジュールとしてコンパイルします。

	clang++ -std=c++17 -c exevalator.cpp
	clang++ -std=c++17 -c example.cpp

これで各ファイルのコンパイル結果モジュール「 exevalator.o 」と「 example.o 」ができるので、最終的にそれらをリンクして実行ファイルを生成します：

	clang++ -o linked_example example.o exevalator.o

これで実行ファイル「 linked_example 」ができるので、実行すると：

	./linked_example

	result: 4.6

と、この通り先ほどの場合と同じ結果が得られました。正しく動作した事がわかります。

実際には、分割コンパイルとリンクをするような場面では、各手順は Makefile 等のビルドファイルに記載されており、ビルドツールによって自動で行われる事が多いと思います。上記はあくまでも手動での一例ですので、それを参考として、ビルドファイルを適切に編集してください。


<a id="example-code"></a>
## サンプルコードの使用方法

このリポジトリ内には、実際に Exevalator を使用する簡単なサンプルコード類「 cpp/example*.cpp 」も同梱されています。

これらは、先のセクションの 2. で例示した通り、特に分割コンパイル等を考えなくても単純にコンパイル/実行できるようになっています。例えば「 example1.cpp 」の場合は：

	clang++ -std=c++17 -o example1 example1.cpp
	./example1

上記の「 example1.cpp 」は、"1.2 + 3.4" の値を Exevalator で計算するサンプルコードで、先ほど例示した「 example.cpp 」とほぼ同一です。従って実行結果は：

	result: 4.6

この通り、"1.2 + 3.4" の計算結果が表示されます。他のサンプルコードも、全く同様にコンパイル/実行できます。

なお、このリポジトリ内には、処理速度を測定するためのベンチマークプログラム「 cpp/benchmark.cpp 」も同梱されています。これをコンパイルする際には、以下のように最適化オプションを指定してください：

	clang++ -std=c++17 -O2 -o benchmark benchmark.cpp

上での「 -O2 」が最適化オプションで、忘れると Exevalator がフル性能を発揮できないため、処理速度が遅くなります。


<a id="features"></a>
## 主な機能

以下では、Exevalator の主な機能を紹介します.

### 1. 式の評価（計算）

これまでのセクションでも見てきたように、Exevalator で式の値を計算できます: 

	double result = exevalator.eval("(-(1.2 + 3.4) * 5) / 2");
	// result: -11.5

	(参照: cpp/example2.cpp)

上記のように、"+" (足し算)、 "-" (引き算や数値のマイナス化)、"\*" (掛け算)、"/" (割り算) の演算を行えます。なお、掛け算と割り算は、足し算と引き算よりも、順序的に優先されます。


### 2. 変数の使用

変数を宣言し、その値に式の中からアクセスできます：

	// 変数を宣言して値を設定
	exevalator.declare_variable("x");
	exevalator.write_variable("x", 1.25);

	// 変数の値を使う式を計算する
	double result = exevalator.eval("x + 1");
	// result: 2.25

	(参照: cpp/example3.cpp)

変数の値を非常に頻繁に書き変えるような用途では、書き変え対象の変数を、以下のようにアドレスによって指定する事も有用です：

	size_t address = exevalator.declare_variable("x");
	exevalator.write_variable_at(address, 1.25);
	...

	(参照: cpp/example4.cpp)

この方法は、書き変え対象変数を名前で指定するよりも高速です。

なお、上記のように変数にアクセスする処理は、eval メソッド同様、失敗時に例外 ExevalatorException をスローする場合がある事に留意してください。


### 3. 関数の使用

式の中で使用するための関数も作成できます。それには、ExevalatorFunctionInterface（抽象クラス）を継承したクラスを作成します：

	// 式内で使用できる関数を作成
	class MyFun : public exevalator::ExevalatorFunctionInterface {
		double operator()(const std::vector<double> &arguments) {
			if (arguments.size() != 2) {
				throw new exevalator::ExevalatorException("Incorrect number of args");
			}
			return arguments[0] + arguments[1];
		}
	}
	...

	// 上記の関数を式内で使用できるよう接続
	//（インターフェース継承クラスのインスタンスを shared_ptr を介して渡します）
	exevalator::Exevalator exevalator;
	exevalator.connect_function("fun", std::make_shared<MyFun>());

	// 関数を使う式を計算する
	double result = exevalator.eval("fun(1.2, 3.4)");
	// result: 4.6

	(参照: cpp/example5.cpp)

**注意: Ver.1.0までは、式から渡された引数が、上記の "const std::vector\<double\> &arguments" 配列内に逆順で格納されていました。Ver.2.0以降では、式で渡したままの順序となるように修正されました。詳細は Issue #2 をご参照ください。**

なお、上記のように関数を接続したりする箇所でも、eval メソッド同様、失敗時に例外 ExevalatorException がスローされる場合がある事に留意してください。
さらに、eval メソッドで計算している式の中で関数が呼ばれ、その関数処理の中で例外が発生するかもしれません
（実際に上の例では、作成した MyFun クラスの関数処理の中で、引数の個数が想定外だった場合に例外をスローしています）。
そういった場合、eval メソッドは例外を ExevalatorException でラップして、呼び出し元に再スローしてきます。



<a id="methods"></a>
## メソッド仕様一覧

Exevalator クラスで提供されている各メソッドの一覧と詳細仕様です。

- [コンストラクタ](#methods-constructor)
- [double eval(const std::string &amp;expression)](#methods-eval)
- [double reeval()](#methods-reeval)
- [size_t declare_variable(const std::string &amp;name)](#methods-declare-variable)
- [void write_variable(const std::string &amp;name, double value)](#methods-write-variable)
- [void write_variable_at(size_t address, double value)](#methods-write-variable-at)
- [double read_variable(const std::string &amp;name)](#methods-read-variable)
- [double read_variable_at(size_t address)](#methods-read-variable-at)
- [size_t connect_function(const std::string &amp;name, const std::shared_ptr&lt;ExevalatorFunctionInterface&gt; &amp;function_ptr)](#methods-connect-function)


<a id="methods-constructor"></a>
| 形式 | (コンストラクタ) Exevalator() |
|:---|:---|
| 説明 | 新しい Exevalator のインタープリタ インスタンスを生成します。 |
| 引数 | なし |
| 戻り値 | 生成されたインスタンス |


<a id="methods-eval"></a>
| 形式 | double eval(const std::string &amp;expression) |
|:---|:---|
| 説明 | 式の値を評価（計算）します。 |
| 引数 | expression: 評価（計算）対象の式 |
| 戻り値 | 評価（計算）結果の値 |
| 例外 | 式の評価中にエラーが発生した場合に ExevalatorException がスローされます。 |


<a id="methods-reeval"></a>
| 形式 | double reeval() |
|:---|:---|
| 説明 | 前回 eval メソッドによって評価されたのと同じ式を、再評価（再計算）します。<br>このメソッドは、繰り返し使用した場合に eval メソッドよりも高速な動作が見込めます。<br>なお、変数の値や関数の振る舞いが、前回評価時から変化している場合、式の評価結果も前回とは変わり得る事に留意してください。 |
| 引数 | なし |
| 戻り値 | 評価（計算）結果の値 |
| 例外 | 式の評価中にエラーが発生した場合に ExevalatorException がスローされます。 |


<a id="methods-declare-variable"></a>
| 形式 | size_t declare_variable(const std::string &amp;name) |
|:---|:---|
| 説明 | 式の中で使用するための変数を、新規に宣言します。 |
| 引数 | name: 宣言する変数の名前 |
| 戻り値 | 宣言した変数に割り当てられた仮想アドレス<br>（高速に読み書きしたい場合に "write_variable_at" や "read_variable_at" メソッドで使用） |
| 例外 | 無効な変数名が指定された場合に ExevalatorException がスローされます。 |


<a id="methods-write-variable"></a>
| 形式 | void write_variable(const std::string &amp;name, double value) |
|:---|:---|
| 説明 | 指定された名前の変数に、値を書き込みます。 |
| 引数 | name: 書き込み対象の変数の名前<br>value: 書き込む値 |
| 戻り値 | なし |
| 例外 | 指定された名前の変数が存在しない場合や、無効な変数名が指定された場合に ExevalatorException がスローされます。 |


<a id="methods-write-variable-at"></a>
| 形式 | void write_variable_at(size_t address, double value) |
|:---|:---|
| 説明 | 指定された仮想アドレスの位置にある変数に、値を書き込みます。<br>なお、このメソッドは "write_variable" メソッドよりも高速です。 |
| 引数 | address: 書き込み対象の変数の仮想アドレス<br>value: 書き込む値 |
| 戻り値 | なし |
| 例外 | 無効なアドレスが指定された場合に ExevalatorException がスローされます。 |


<a id="methods-read-variable"></a>
| 形式 | double read_variable(const std::string &amp;name) |
|:---|:---|
| 説明 | 指定された名前の変数の値を読み込みます。 |
| 引数 | name: 読み込み対称の変数の名前 |
| 戻り値 | 変数の現在の値 |
| 例外 | 指定された名前の変数が存在しない場合や、無効な変数名が指定された場合に ExevalatorException がスローされます。 |


<a id="methods-read-variable-at"></a>
| 形式 | double read_variable_at(size_t address) |
|:---|:---|
| 説明 | 指定された仮想アドレスの位置にある変数の値を読み込みます。<br>なお、このメソッドは "read_variable" メソッドよりも高速です。 |
| 引数 | address: 読み込み対象の変数の仮想アドレス
| 戻り値 | 変数の現在の値 |
| 例外 | 無効なアドレスが指定された場合に ExevalatorException がスローされます。 |


<a id="methods-connect-function"></a>
| 形式 | size_t connect_function(const std::string &amp;name, const std::shared_ptr&lt;ExevalatorFunctionInterface&gt; &amp;function_ptr) |
|:---|:---|
| 説明 | 式の中で使用するための関数を接続します。 |
| 引数 | name: 接続する関数の名前<br>function: 関数の処理を提供する ExevalatorFunctionInterface 継承クラスのインスタンス<br>（ExevalatorFunctionInterface には「 double invoke(const std::vector&lt;double&gt; &arguments) 」メソッドのみが定義されており、このメソッドに関数処理を実装します） |
| 戻り値 | このバージョンでは使用しません |
| 例外 | 無効な関数名が指定された場合に ExevalatorException がスローされます。 |



<hr />

<a id="credits"></a>
## 本文中の商標など

- Linux は、Linus Torvalds 氏の米国およびその他の国における商標または登録商標です。

- ChatGPT は、米国 OpenAI OpCo, LLC による米国またはその他の国における商標または登録商標です。

- その他、文中に使用されている商標は、その商標を保持する各社の各国における商標または登録商標です。


