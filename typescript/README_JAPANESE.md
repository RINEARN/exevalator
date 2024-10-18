# TypeScript での Exevalator の使用方法

&raquo; [English](./README.md)

&raquo; [AIに使い方を聞く（ChatGPTのアカウントが必要）](https://chatgpt.com/g/g-UjkEJeO8x-exevalator-assistant)

## 日本語版 目次

- [必要な環境](#requirements)
- [使用方法](#how-to-use)
- [サンプルコードの実行方法](#example-code)
- [主な機能](#features)
- [メソッド仕様一覧](#methods)
	- [コンストラクタ](#methods-constructor)
	- [eval(expression: string): number](#methods-eval)
	- [reeval(): number](#methods-reeval)
	- [declareVariable(name: string): number](#methods-declare-variable)
	- [writeVariable(name: string, value: number): void](#methods-write-variable)
	- [writeVariableAt(address: number, value: number): void](#methods-write-variable-at)
	- [readVariable(name: string): number](#methods-read-variable)
	- [readVariableAt(address: number): number](#methods-read-variable-at)
	- [connectFunction(name: string, function: ExevalatorFunctionInterface): void](#methods-connect-function)



<a id="requirements"></a>
## 必要な環境

* Node.js
* tsc (TypeScriptコンパイラ)
* esbuild (ブラウザ上で動かしたい場合のためのバンドラ)

まず PC に Node.js をインストールした上で、以下のように残りを導入します：

	cd <作業フォルダ>
	npm init
	npm install --save-dev typescript
	npm install --save-dev @types/node 
	npm install --save-dev esbuild

上記で、「 --save-dev 」オプションを付けている作用により、作業フォルダ内にある「 node_modules 」フォルダの中に、TypeScript コンパイラ「 tsc 」やバンドラツール「 esbuild 」が導入されます。

こうする事で、作業フォルダ内限定で、「npx」コマンドを頭に付ける事で、これらのツールを使えるようになります（別のフォルダの環境を汚さずに済みます）。

実際のコンパイル・実行方法は後の節で例示します。


<a id="how-to-use"></a>
## 使用方法

Exevalator のインタープリタは、単一のファイル「 typescript/exevalator.ts 」内に実装されています。そのため、以下のような 3 ステップで、簡単に import して使用できます。

### 1. 使用したいプロジェクトのソースコードフォルダ内に配置

まず「 typescript/exevalator.ts 」を、使用したいプロジェクトのソースコードフォルダ内の、好きな場所に配置します。

ここでは単純に、Exevalator を呼び出して使うコードと同じフォルダ内に配置したとします。

### 2. 使用したいコードから import

あとは、式の計算を行いたいコードから import して、以下のように使用できます：

	// Exevalator を読み込む（実際は配置に合った相対パスを指定）
	import Exevalator from "./exevalator";

	// Exevalator のインタープリタを生成
	let exevalator: Exevalator = new Exevalator();

	// 式の値を計算（評価）する
	const result: number = exevalator.eval("1.2 + 3.4");

	// 結果を表示
	console.log(`result: ${result}`);

なお、Exevalator では、式の中のすべての数値は number 型で扱われます。従って、結果も常に number 型です。

ところで、式の内容がおかしい場合や、未宣言の変数を使った場合など、計算に失敗する場合もあり得ます。その場合は、eval メソッドを呼んでいる箇所で ExevalatorError がスローされますので、実用用途では、必要に応じて catch してハンドルしてください。

	import Exevalator, { ExevalatorError } from "./exevalator"

	...

	try {
		// 式の値を計算（評価）する
		const result: number = exevalator.eval(expression);

	} catch (error) {
		if (error instanceof ExevalatorError) {
    		// Errors that are typically expected
			// (e.g., syntax errors in the expression)
		} else {
    		// Unexpected errors
			// (e.g., bugs)
		}
	}

<a id="example-code"></a>
## サンプルコードの使用方法

このリポジトリ内には、実際に Exevalator を使用する簡単なサンプルコード類「 typescript/example*.ts 」も同梱されています。
例えば example1 は、以下のようにコンパイル/実行できます:

	(あらかじめ typescript フォルダ内に環境構築を済ませた状態で)

	cd typescript
	npx tsc example1.ts
	node example1.js

上記の「 example1.ts 」は、"1.2 + 3.4" の値を Exevalator で計算するサンプルコードです。内容は、すぐ前の節で掲載したサンプル「 YourClass 」とほぼ同一です：

	...
	const result: number = exevalator.eval("1.2 + 3.4");
	...

実行結果は：

	result: 4.6

この通り、"1.2 + 3.4" の計算結果が表示されます。 example1～5 までは、全く同様にコンパイル/実行できます。

一方、example6 と 7 は、Webブラウザ上で動作するサンプルであるため、バンドラツールの「 esbuild 」を使って、Exevalator 込みで一枚の JavaScript ファイルとしてビルドします：

	(あらかじめ typescript フォルダ内に環境構築を済ませた状態で)

	cd typescript
	npx esbuild example6.ts --bundle --outfile=example6.bundle.js

これで「 example6.bundle.js 」が生成されます。あとは、同じフォルダ内にあるHTMLファイル「 example6.html 」をWebブラウザで開くと、上記が読み込まれて実行されます。実行すると、式 f(x) の内容と、変数 x の値の入力を求められるので、入力すると計算され、値が表示されます。

example7 も上記と同様にビルドし、Webブラウザで実行してください。すると、式 f(x) の内容と、積分の下端・上端の入力を求められます。入力すると、ブラウザ上で Exevalator を用いた数値積分の処理が走り、結果の値が表示されます。

なお、このリポジトリ内には、処理速度を測定するためのベンチマークプログラム「 ts/benchmark.ts 」も同梱されています。そちらは example1.ts と同様の手順でコンパイル/実行できます。


<a id="features"></a>
## 主な機能

以下では、Exevalator の主な機能を紹介します.

### 1. 式の評価（計算）

これまでのセクションでも見てきたように、Exevalator で式の値を計算できます: 

	const result: number = exevalator.eval("(-(1.2 + 3.4) * 5) / 2");
	// result: -11.5

(参照: typescript/example2.ts)

上記のように、"+" (足し算)、 "-" (引き算や数値のマイナス化)、"\*" (掛け算)、"/" (割り算) の演算を行えます。なお、掛け算と割り算は、足し算と引き算よりも、順序的に優先されます。


### 2. 変数の使用

アプリの実装時に、Exevalator の declareVariable メソッドを使用して、変数を宣言できます。宣言した変数は、式の中で自由にアクセスできます：

	// 変数を宣言して値を設定
	exevalator.declareVariable("x");
	exevalator.writeVariable("x", 1.25);

	// 変数の値を使う式を計算する
	const result: number = exevalator.eval("x + 1");
	// result: 2.25

(参照: typescript/example3.ts)

変数の値を非常に頻繁に書き変えるような用途では、書き変え対象の変数を、以下のようにアドレスによって指定する事も有用です：

	const address: number = exevalator.declareVariable("x");
	exevalator.writeVariableAt(address, 1.25);
	...

(参照: typescript/example4.ts)

この方法は、書き変え対象変数を名前で指定するよりも高速です。

### 3. 関数の使用

式の中で使用するための関数も作成できます。それには、Exevalator.FunctionInterface インターフェースを実装したクラスを作成します：

	import Exevalator, { ExevalatorFunctionInterface, ExevalatorError } from "./exevalator";

	...

	// 式内で使用できる関数を作成
	class MyFunction implements ExevalatorFunctionInterface {
	    public invoke(args: number[]): number {
    	    if (args.length !== 2) {
        	    throw new ExevalatorError(`Incorrected number of args: ${args.length}`);
        	}
	        return args[0] + args[1];
    	}
	}

	...
		
	// Exevalator のインタープリタを生成
	let exevalator: Exevalator = new Exevalator();

	// 上記の関数を式内で使用できるよう接続
	const fun: MyFunction = new MyFunction();
	exevalator.connectFunction("fun", fun);

	// 関数を使った式を計算する
	const result: number = exevalator.eval("fun(1.2, 3.4)");
	// result: 4.6

(参照: typescript/example5.ts)



<a id="methods"></a>
## メソッド仕様一覧

Exevalator クラスで提供されている各メソッドの一覧と詳細仕様です。

* [コンストラクタ](#methods-constructor)
* [eval(expression: string): number](#methods-eval)
* [reeval(): number](#methods-reeval)
* [declareVariable(name: string): number](#methods-declare-variable)
* [writeVariable(name: string, value: number): void](#methods-write-variable)
* [writeVariableAt(address: number, value: number): void](#methods-write-variable-at)
* [readVariable(name: string): number](#methods-read-variable)
* [readVariableAt(address: number): number](#methods-read-variable-at)
* [connectFunction(name: string, function: ExevalatorFunctionInterface): void](#methods-connect-function)


<a id="methods-constructor"></a>
| 形式 | (コンストラクタ) Exevalator() |
|:---|:---|
| 説明 | 新しい Exevalator のインタープリタ インスタンスを生成します。 |
| 引数 | なし |
| 戻り値 | 生成されたインスタンス |


<a id="methods-eval"></a>
| 形式 | eval(expression: string): number |
|:---|:---|
| 説明 | 式の値を評価（計算）します。 |
| 引数 | expression: 評価（計算）対象の式 |
| 戻り値 | 評価（計算）結果の値 |
| 例外 | 式の評価中にエラーが発生した場合に ExevalatorError がスローされます。 |


<a id="methods-reeval"></a>
| 形式 | reeval(): number |
|:---|:---|
| 説明 | 前回 eval メソッドによって評価されたのと同じ式を、再評価（再計算）します。<br>このメソッドは、繰り返し使用した場合に eval メソッドよりも僅かに高速な場合があります。<br>なお、変数の値や関数の振る舞いが、前回評価時から変化している場合、式の評価結果も前回とは変わり得る事に留意してください。 |
| 引数 | なし |
| 戻り値 | 評価（計算）結果の値 |
| 例外 | 式の評価中にエラーが発生した場合に ExevalatorError がスローされます。 |


<a id="methods-declare-variable"></a>
| 形式 | declareVariable(name: string): number |
|:---|:---|
| 説明 | 式の中で使用するための変数を、新規に宣言します。 |
| 引数 | name: 宣言する変数の名前 |
| 戻り値 | 宣言した変数に割り当てられた仮想アドレス<br>（高速に読み書きしたい場合に "writeVariableAt" や "readVariableAt" メソッドで使用） |
| 例外 | 無効な変数名が指定された場合に ExevalatorError がスローされます。 |


<a id="methods-write-variable"></a>
| 形式 | writeVariable(name: string, value: number): void |
|:---|:---|
| 説明 | 指定された名前の変数に、値を書き込みます。 |
| 引数 | name: 書き込み対象の変数の名前<br>value: 書き込む値 |
| 戻り値 | なし |
| 例外 | 指定された名前の変数が存在しない場合や、無効な変数名が指定された場合に ExevalatorError がスローされます。 |


<a id="methods-write-variable-at"></a>
| 形式 | writeVariableAt(address: number, value: number): void |
|:---|:---|
| 説明 | 指定された仮想アドレスの位置にある変数に、値を書き込みます。<br>なお、このメソッドは "writeVariable" メソッドよりも高速です。 |
| 引数 | address: 書き込み対象の変数の仮想アドレス<br>value: 書き込む値 |
| 戻り値 | なし |
| 例外 | 無効なアドレスが指定された場合に ExevalatorError がスローされます。 |


<a id="methods-read-variable"></a>
| 形式 | double readVariable(name: string) |
|:---|:---|
| 説明 | 指定された名前の変数の値を読み込みます。 |
| 引数 | name: 読み込み対称の変数の名前 |
| 戻り値 | 変数の現在の値 |
| 例外 | 指定された名前の変数が存在しない場合や、無効な変数名が指定された場合に ExevalatorError がスローされます。 |


<a id="methods-read-variable-at"></a>
| 形式 | readVariableAt(address: number): number |
|:---|:---|
| 説明 | 指定された仮想アドレスの位置にある変数の値を読み込みます。<br>なお、このメソッドは "readVariable" メソッドよりも高速です。 |
| 引数 | address: 読み込み対象の変数の仮想アドレス
| 戻り値 | 変数の現在の値 |
| 例外 | 無効なアドレスが指定された場合に ExevalatorError がスローされます。 |


<a id="methods-connect-function"></a>
| 形式 | connectFunction(name: string, function: ExevalatorFunctionInterface): void |
|:---|:---|
| 説明 | 式の中で使用するための関数を接続します。 |
| 引数 | name: 接続する関数の名前<br>function: 関数の処理を提供する ExevalatorFunctionInterface 実装クラスのインスタンス<br>（ExevalatorFunctionInterface には「 invoke(args: number[]): number 」メソッドのみが定義されており、このメソッドに関数処理を実装します） |
| 戻り値 | なし |
| 例外 | 無効な関数名が指定された場合に ExevalatorError がスローされます。 |





<hr />

<a id="credits"></a>
## 本文中の商標など

- Node.js は、OpenJS Foundation による米国またはその他の国における商標または登録商標です。

- JavaScript は、Oracle Corporation 及びその子会社、関連会社の米国及びその他の国における登録商標です。文中の社名、商品名等は各社の商標または登録商標である場合があります。 

- ChatGPT は、米国 OpenAI OpCo, LLC による米国またはその他の国における商標または登録商標です。

- その他、文中に使用されている商標は、その商標を保持する各社の各国における商標または登録商標です。


