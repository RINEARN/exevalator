# Rust&reg; での Exevalator の使用方法

&raquo; [English](./README.md)


## 日本語版 目次
- [必要な環境](#requirements)
- [使用方法](#how-to-use)
- [サンプルコードの実行方法](#example-code)
- [主な機能](#features)
- [メソッド仕様一覧](#methods)
	- [コンストラクタ](#methods-constructor)
	- [fn eval(&amp;mut self, expression: &amp;str) -&gt; Result&lt;f64, ExevalatorError&gt;](#methods-eval)
	- [fn reeval(&mut self) -&gt; Result&lt;f64, ExevalatorError&gt;](#methods-reeval)
	- [fn declare_variable(&mut self, name: &amp;str) -&gt; Result&lt;usize, ExevalatorError&gt;](#methods-declare-variable)
	- [fn write_variable(&amp;mut self, name: &amp;str, value: f64) -&gt; Option&lt;ExevalatorError&gt;](#methods-write-variable)
	- [fn write_variable_at(&amp;mut self, address: usize, value: f64) -&gt; Option&lt;ExevalatorError&gt;](#methods-write-variable-at)
	- [fn read_variable(&amp;mut self, name: &amp;str) -&gt; Result&lt;f64, ExevalatorError&gt;](#methods-read-variable)
	- [fn read_variable_at(&amp;mut self, address: usize) -&gt; Result&lt;f64, ExevalatorError&gt;](#methods-read-variable-at)
	- [fn connect_function(&amp;mut self, name: &amp;str, function_pointer: fn(Vec&lt;f64&gt;)->Result&lt;f64,ExevalatorError&gt;)) -&gt; Result&lt;usize, ExevalatorError&gt;](#methods-connect-function)




<a id="requirements"></a>
## 必要な環境

* rustc 1.57.0 以降（推奨）



<a id="how-to-use"></a>
## 使用方法

Exevalator のインタープリタは、単一のファイル「 rust/exevalator.rs 」内に実装されています。そのため、以下のような 2 ステップで、簡単に使用できます。

### 1. 使用したいプロジェクトのソースコードフォルダ内に配置

まず「 rust/exevalator.rs 」を、使用したいプロジェクトのソースコードフォルダ内に配置します。

単一ファイルなので邪魔にならないため、ソースコードフォルダ内の最上位の階層に置くいておくと、簡単に読み込めます。
もしも、最上位ではないフォルダ階層に配置した場合は、そのフォルダなどでモジュール宣言を適切に行ってください。

### 2. 使用したいクラスから読み込んで使う

あとは、式の計算を行いたいコードのファイルから読み込んでして、以下のように使用できます：

	mod exevalator;
	use exevalator::Exevalator;
	...

	// Exevalator のインスタンスを生成
	let mut exevalator = Exevalator::new();

	// 式の値を評価（計算）する
	let result: f64 = match exevalator.eval("1.2 + 3.4") {
		Ok(eval_value) => eval_value,
		Err(eval_error) => panic!("{}", eval_error),
	};

	// 結果を画面に表示
	println!("result: {}", result);	


なお、Exevalator では、式の中のすべての数値は f64 型で扱われます。従って、計算結果も常に f64 型です。
ただし、式の内容によってはエラーが発生する可能性があるため、eval メソッドの戻り値は Result<f64, ExevalatorError> 型でラップされています。


<a id="example-code"></a>
## サンプルコードの使用方法

このリポジトリ内には、実際に Exevalator を使用する簡単なサンプルコード類「 rust/example*.java 」も同梱されています。
それらは、以下のようにコンパイルできます:

	cd rust
	rustc example1.rs

実行は、Linux&reg; などを使用している場合は:

	./example1

Microsoft&reg; Windows&reg; を使用している場合は:

	example1.exe

などとします。なお、上記の「 Example1.java 」は、"1.2 + 3.4" の値を Exevalator で計算するサンプルコードです。内容は、すぐ前の節で掲載したサンプル「 YourClass 」とほぼ同一です：

	...
	let result: f64 = match exevalator.eval("1.2 + 3.4") {
		Ok(eval_value) => eval_value,
		Err(eval_error) => panic!("{}", eval_error),
	};

従って実行結果は：

	result: 4.6

この通り、"1.2 + 3.4" の計算結果が表示されます。他のサンプルコードも、全く同様にコンパイル/実行できます。

なお、このリポジトリ内には、処理速度を測定するためのベンチマークプログラム「 rust/benchmark.rs 」も同梱されています。これをコンパイルする際には、以下のように最適化オプションを指定してください：

	rustc -C opt-level=3 benchmark.rs

上での「 -C opt-level=3 」が最適化オプションの指定箇所で、忘れると Exevalator がフル性能を発揮できないため、処理速度が遅くなります。



<a id="features"></a>
## 主な機能

以下では、Exevalator の主な機能を紹介します.

### 1. 式の評価（計算）

これまでのセクションでも見てきたように、Exevalator で式の値を計算できます: 

	let result: f64 = match exevalator.eval("(-(1.2 + 3.4) * 5) / 2") {
		Ok(eval_value) => eval_value,
		Err(eval_error) => panic!("{}", eval_error),
	};
	// result: -11.5

	(参照: rust/example2.rs)

上記のように、"+" (足し算)、 "-" (引き算や数値のマイナス化)、"\*" (掛け算)、"/" (割り算) の演算を行えます。なお、掛け算と割り算は、足し算と引き算よりも、順序的に優先されます。


### 2. 変数の使用

変数を宣言し、その値に式の中からアクセスできます：

	// 変数を宣言して値を設定
	match exevalator.declare_variable("x") {
		Ok(address) => address,
		Err(declaration_error) => panic!("{}", declaration_error)
	};
	match exevalator.write_variable("x", 1.25) {
		Some(access_error) => panic!("{}", access_error),
		None => {},
	};

	// 式の値を評価（計算）
	let result: f64 = match exevalator.eval("x + 1") {
		Ok(eval_value) => eval_value,
		Err(eval_error) => panic!("{}", eval_error),
	};
	// result: 2.25

	(参照: rust/example3.rs)

変数の値を非常に頻繁に書き変えるような用途では、書き変え対象の変数を、以下のようにアドレスによって指定する事も有用です：

	// 変数を宣言して値を設定
	let address: usize = match exevalator.declare_variable("x") {
		Ok(declared_var_address) => declared_var_address,
		Err(declaration_error) => panic!("{}", declaration_error),
	};
	exevalator.write_variable_at(address, 1.25);
	// The above works faster than:
	//	 exevalator.write_variable("x", 1.25);
	...

	(参照: rust/example4.rs)

この方法は、書き変え対象変数を名前で指定するよりも高速です。

### 3. 関数の使用

式の中で使用するための関数も作成できます。具体的には、シグネチャ **"fn function(arguments: Vec<f64>) -> Result<f64, ExevalatorError>"** を持つ関数が使用できます。具体例としては：

	/// 式内で使用可能な関数を定義
	fn my_function(arguments: Vec<f64>) -> Result<f64, ExevalatorError> {
		if arguments.len() != 2 {
			return Err(ExevalatorError::new("Incorrect number of args"));
		}
		return Ok(arguments[0] + arguments[1]);
	} 

これを Exevalator に接続すると、式の中で使用できます：

	// 上記の関数を、式内で使用するために接続
	let address: usize = match exevalator.connect_function("fun", my_function) {
		Ok(connected_function_address) => connected_function_address,
		Err(connection_error) => panic!("{}", connection_error),
	};
	
	// 式の値を評価（計算）
	let result: f64 = match exevalator.eval("fun(1.2, 3.4)") {
		Ok(eval_value) => eval_value,
		Err(eval_error) => panic!("{}", eval_error),
	};
	// result: 4.6

	(参照: rust/example5.rs)





<a id="methods"></a>
## メソッド仕様一覧

Exevalator 構造体で提供されている各メソッドの一覧と詳細仕様です。

- [コンストラクタ](#methods-constructor)
- [fn eval(&amp;mut self, expression: &amp;str) -&gt; Result&lt;f64, ExevalatorError&gt;](#methods-eval)
- [fn reeval(&mut self) -&gt; Result&lt;f64, ExevalatorError&gt;](#methods-reeval)
- [fn declare_variable(&mut self, name: &amp;str) -&gt; Result&lt;usize, ExevalatorError&gt;](#methods-declare-variable)
- [fn write_variable(&amp;mut self, name: &amp;str, value: f64) -&gt; Option&lt;ExevalatorError&gt;](#methods-write-variable)
- [fn write_variable_at(&amp;mut self, address: usize, value: f64) -&gt; Option&lt;ExevalatorError&gt;](#methods-write-variable-at)
- [fn read_variable(&amp;mut self, name: &amp;str) -&gt; Result&lt;f64, ExevalatorError&gt;](#methods-read-variable)
- [fn read_variable_at(&amp;mut self, address: usize) -&gt; Result&lt;f64, ExevalatorError&gt;](#methods-read-variable-at)
- [fn connect_function(&amp;mut self, name: &amp;str, function_pointer: fn(Vec&lt;f64&gt;)->Result&lt;f64,ExevalatorError&gt;)) -&gt; Result&lt;usize, ExevalatorError&gt;](#methods-connect-function)


<a id="methods-constructor"></a>
| 形式 | (コンストラクタ) Exevalator() |
|:---|:---|
| 説明 | 新しい Exevalator のインタープリタ インスタンスを生成します。 |
| 引数 | なし |
| 戻り値 | 生成されたインスタンス |


<a id="methods-eval"></a>
| 形式 | fn eval(&amp;mut self, expression: &amp;str) -&gt; Result&lt;f64, ExevalatorError&gt; |
|:---|:---|
| 説明 | 式の値を評価（計算）します。 |
| 引数 | expression: 評価（計算）対象の式 |
| 戻り値 | Ok: 評価（計算）結果の値<br>Err: 式の評価中にエラーが発生した場合 |


<a id="methods-reeval"></a>
| 形式 | double reeval() -&gt; Result&lt;f64, ExevalatorError&gt; |
|:---|:---|
| 説明 | 前回 eval メソッドによって評価されたのと同じ式を、再評価（再計算）します。<br>このメソッドは、繰り返し使用した場合に eval メソッドよりも僅かに高速な場合があります。<br>なお、変数の値や関数の振る舞いが、前回評価時から変化している場合、式の評価結果も前回とは変わり得る事に留意してください。 |
| 引数 | なし |
| 戻り値 | Ok: 評価（計算）結果の値<br>Err: 式の評価中にエラーが発生した場合 |


<a id="methods-declare-variable"></a>
| 形式 | fn declare_variable(&mut self, name: &amp;str) -&gt; Result&lt;usize, ExevalatorError&gt; |
|:---|:---|
| 説明 | 式の中で使用するための変数を、新規に宣言します。 |
| 引数 | name: 宣言する変数の名前 |
| 戻り値 | Ok: 宣言した変数に割り当てられた仮想アドレス（高速に読み書きしたい場合に "write_variable_at" や "read_variable_at" メソッドで使用）<br>Err: 無効な変数名が指定された場合 |


<a id="methods-write-variable"></a>
| 形式 | fn write_variable(&amp;mut self, name: &amp;str, value: f64) -&gt; Option&lt;ExevalatorError&gt; |
|:---|:---|
| 説明 | 指定された名前の変数に、値を書き込みます。 |
| 引数 | name: 書き込み対象の変数の名前<br>value: 書き込む値 |
| 戻り値 | None: 通常の場合<br>Some: 指定された名前の変数が存在しない場合や、無効な変数名が指定された場合 |


<a id="methods-write-variable-at"></a>
| 形式 | fn write_variable_at(&amp;mut self, address: usize, value: f64) -&gt; Option&lt;ExevalatorError&gt; |
|:---|:---|
| 説明 | 指定された仮想アドレスの位置にある変数に、値を書き込みます。<br>なお、このメソッドは "write_variable" メソッドよりも高速です。 |
| 引数 | address: 書き込み対象の変数の仮想アドレス<br>value: 書き込む値 |
| 戻り値 | None: 通常の場合<br>Some: 無効なアドレスが指定された場合 |


<a id="methods-read-variable"></a>
| 形式 | fn read_variable(&amp;mut self, name: &amp;str) -&gt; Result&lt;f64, ExevalatorError&gt; |
|:---|:---|
| 説明 | 指定された名前の変数の値を読み込みます。 |
| 引数 | name: 読み込み対称の変数の名前 |
| 戻り値 | Ok: 変数の現在の値<br>Err: 指定された名前の変数が存在しない場合や、無効な変数名が指定された場合 |


<a id="methods-read-variable-at"></a>
| 形式 | fn read_variable_at(&amp;mut self, address: usize) -&gt; Result&lt;f64, ExevalatorError&gt; |
|:---|:---|
| 説明 | 指定された仮想アドレスの位置にある変数の値を読み込みます。<br>なお、このメソッドは "read_variable" メソッドよりも高速です。 |
| 引数 | address: 読み込み対象の変数の仮想アドレス |
| 戻り値 | Ok: 変数の現在の値<br>Err: 無効なアドレスが指定された場合 |


<a id="methods-connect-function"></a>
| 形式 | fn connect_function(&amp;mut self, name: &amp;str, function_pointer: fn(Vec&lt;f64&gt;)->Result&lt;f64,ExevalatorError&gt;)) -&gt; Result&lt;usize, ExevalatorError&gt; |
|:---|:---|
| 説明 | 式の中で使用するための関数を接続します。 |
| 引数 | name: 接続する関数の名前<br>function_pointer: 接続対象の関数 |
| 戻り値 | Ok: このバージョンでは使用しません<br>Err: 無効な関数名が指定された場合 |


<hr />

<a id="credits"></a>
## 本文中の商標など

- Rustは、Mozilla Foundation の米国及びその他の国における登録商標です。

- Windows は、米国 Microsoft Corporation の米国およびその他の国における登録商標です。

- Linux は、Linus Torvalds 氏の米国およびその他の国における商標または登録商標です。

- その他、文中に使用されている商標は、その商標を保持する各社の各国における商標または登録商標です。


