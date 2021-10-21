# Exevalator

Exevalator（**Ex**pression-**Eval**u**ator** の略） は、アプリケーション内に組み込んで、式の値を計算するための、コンパクトなインタープリタです。

&raquo; [English README](./README.md)


## 日本語版 README 目次
- <a href="#license">ライセンス</a>
- <a href="#requirements">必要な環境</a>
- <a href="#how-to-use">使用方法</a>
- <a href="#how-to-use-example-code">サンプルコードの使用方法</a>
- <a href="#related">関連ソフトウェア</a>
- <a href="#about-us">開発元について</a>


<a id="license"></a>
## ライセンス

このソフトウェアはMITライセンスで公開されています。


<a id="requirements"></a>
## 必要な環境

* Java&reg; 開発環境 (JDK) 8 以降


<a id="how-to-use"></a>
## 使用方法

Exevalator のインタープリタは、単一のファイル「 Exevalator.java 」内に実装されています。そのため、以下のような 3 ステップで、簡単に import して使用できます。

### 1. 使用したいプロジェクトのソースコードフォルダ内に配置

まず「 Exevalator.java 」を、使用したいプロジェクトのソースコードフォルダ内の、好きな場所に配置します。ここでは例として、以下の場所に配置したとします：

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


<a id="how-to-use-example-code"></a>
## サンプルコードの使用方法

このリポジトリ内には、実際に Exevalator を使用する簡単なサンプルコード類「 Example*.java 」も同梱されています。
それらは、以下のようにコンパイル/実行できます:

	javac Exevalator.java
	javac Example1.java
	java Example1

上記の「 Example1.java 」は、"1.2 + 3.4" の値を Exevalator で計算するサンプルコードです。内容は、すぐ前の節で掲載したサンプル「 YourClass 」とほぼ同一です。実行結果は：

	4.6

この通り、"1.2 + 3.4" の計算結果が表示されます。


<a id="related"></a>
## 関連ソフトウェア

Exevalator では、インタープリタの規模をコンパクトに抑える事を優先するため、サポートされる機能が絞られています。

もし、Exevalator を使用してみて、もう少し機能や処理速度が欲しいと思った方は、代わりにアプリケーション内組み込み用のスクリプトエンジン「 [Vnano](https://github.com/RINEARN/vnano) 」をお試しください。
Vnano では、条件分岐や繰り返しも含めた、それなりに複雑な処理を実行できます。


<a id="about-us"></a>
## 開発元について

Exevalator は、日本の個人運営の開発スタジオ [RINEARN](https://www.rinearn.com/) が開発しています。著者は [松井文宏](https://fumihiro-matsui.xnea.net/) です。ご質問やフィードバックなどをお持ちの方は、ぜひ御気軽にどうぞ！


<a id="credits"></a>
## 本文中の商標など

- OracleとJavaは、Oracle Corporation 及びその子会社、関連会社の米国及びその他の国における登録商標です。文中の社名、商品名等は各社の商標または登録商標である場合があります。 

- その他、文中に使用されている商標は、その商標を保持する各社の各国における商標または登録商標です。


