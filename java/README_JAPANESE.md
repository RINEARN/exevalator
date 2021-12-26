# Java&reg; 言語での Exevalator の使用方法

&raquo; [English](./README.md)


## 日本語版 目次
- <a href="#requirements">必要な環境</a>
- <a href="#how-to-use">使用方法</a>
- <a href="#example-code">サンプルコードの実行方法</a>
- <a href="#features">主な機能</a>
- <a href="#vnano">もっと機能が必要な場合は: Vnano</a>





<a id="requirements"></a>
## 必要な環境

* Java&reg; 開発環境 (JDK) 8 以降



<a id="how-to-use"></a>
## 使用方法

Exevalator のインタープリタは、単一のファイル「 java/Exevalator.java 」内に実装されています。そのため、以下のような 3 ステップで、簡単に import して使用できます。

### 1. 使用したいプロジェクトのソースコードフォルダ内に配置

まず「 java/Exevalator.java 」を、使用したいプロジェクトのソースコードフォルダ内の、好きな場所に配置します。ここでは例として、以下の場所に配置したとします：

	src/your/projects/package/anywhere/Exevalator.java

### 2. パッケージ文を記述

続いて、配置した「 Exevalator.java 」を開き、配置した場所に対応するように、先頭にパッケージ文を記述します。今の例の場合は：

	(Exevalator.java 内)
	package your.projects.package.anywhere;

### 3. 使用したいクラスから import

以上で準備は整いました！ あとは、式の計算を行いたいクラスから import して、以下のように使用できます：

	...
	import your.projects.package.anywhere.Exevalator;
	...

	public class YourClass {
		...
		public void yourMethod() {
			
			// Exevalator のインタープリタを生成
			Exevalator exevalator = new Exevalator();

			// 式の値を計算（評価）する
			double result = exevalator.eval("1.2 + 3.4");
			
			// 結果を表示
			System.out.println("Result: " + result);
		}
		...
	}

なお、Exevalator では、式の中のすべての数値は double 型で扱われます。従って、結果も常に double 型です。
ただし、式の内容がおかしい場合や、未宣言の変数を使った場合など、計算に失敗する場合もあり得ます。その場合は、eval メソッドを呼んでいる箇所で例外 Exevalator.Exception がスローされますので、必要に応じて catch してハンドルしてください。


<a id="example-code"></a>
## サンプルコードの使用方法

このリポジトリ内には、実際に Exevalator を使用する簡単なサンプルコード類「 java/Example*.java 」も同梱されています。
それらは、以下のようにコンパイル/実行できます:

	cd java
	javac Exevalator.java
	javac Example1.java
	java Example1

上記の「 Example1.java 」は、"1.2 + 3.4" の値を Exevalator で計算するサンプルコードです。内容は、すぐ前の節で掲載したサンプル「 YourClass 」とほぼ同一です：

	...
	double result = exevalator.eval("1.2 + 3.4");

実行結果は：

	4.6

この通り、"1.2 + 3.4" の計算結果が表示されます。


<a id="features"></a>
## 主な機能

以下では、Exevalator の主な機能を紹介します.

### 1. 式の評価（計算）

これまでのセクションでも見てきたように、Exevalator で式の値を計算できます: 

	double result = exevalator.eval("(-(1.2 + 3.4) * 5) / 2");
	// result: -11.5

	(参照: java/Example2.java)

上記のように、"+" (足し算)、 "-" (引き算や数値のマイナス化)、"\*" (掛け算)、"/" (割り算) の演算を行えます。なお、掛け算と割り算は、足し算と引き算よりも、順序的に優先されます。


### 2. 変数の使用

変数を宣言し、その値に式の中からアクセスできます：

	// 変数を宣言して値を設定
	exevalator.declareVariable("x");
	exevalator.writeVariable("x", 1.25);

	// 変数の値を使う式を計算する
	double result = exevalator.eval("x + 1");
	// result: 2.25

	(参照: java/Example3.java)

変数の値を非常に頻繁に書き変えるような用途では、書き変え対象の変数を、以下のようにアドレスによって指定する事も有用です：

	int address = exevalator.declareVariable("x");
	exevalator.writeVariableAt(address, 1.25);
	...

	(参照: java/Example4.java)

この方法は、書き変え対象変数を名前で指定するよりも高速です。

### 3. 関数の使用

式の中で使用するための関数も作成できます。それには、Exevalator.FunctionInterface インターフェースを実装したクラスを作成します：

	// 式内で使用できる関数を作成
	class MyFunction implements Exevalator.FunctionInterface {
		@Override
		public double invoke (double[] arguments) {
			if (arguments.length != 2) {
				throw new Exevalator.Exception("Incorrected number of args");
			}
			return arguments[0] + arguments[1];
		}
	}
	...
	
	// 上記の関数を式内で使用できるよう接続
	MyFunction fun = new MyFunction();
	exevalator.connectFunction("fun", fun);

	// 関数を使う式を計算する
	double result = exevalator.eval("fun(1.2, 3.4)");
	// result: 4.6

	(参照: java/Example5.java)


<a id="vnano"></a>
## もっと機能が必要な場合は: Vnano

Exevalator では、インタープリタの規模をコンパクトに抑える事を優先するため、サポートされる機能が絞られています。

もし、Exevalator を使用してみて、もう少し機能や処理速度が欲しいと思った方は、代わりにJava&reg;アプリケーション内組み込み用のスクリプトエンジン「 [Vnano](https://github.com/RINEARN/vnano) 」をお試しください。
Vnano では、条件分岐や繰り返しも含めた、それなりに複雑な処理を実行できます。



<a id="credits"></a>
## 本文中の商標など

- OracleとJavaは、Oracle Corporation 及びその子会社、関連会社の米国及びその他の国における登録商標です。文中の社名、商品名等は各社の商標または登録商標である場合があります。 

- その他、文中に使用されている商標は、その商標を保持する各社の各国における商標または登録商標です。


