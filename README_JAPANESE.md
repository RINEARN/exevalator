# Exevalator

Exevalator（**Ex**pression-**Eval**u**ator** の略） は、アプリケーション内に組み込んで、式の値を計算するための、コンパクトなインタープリタです。Java&reg;言語、Rust&reg;、C#&reg;製のアプリケーションにおいて使用できます。

&raquo; [English README](./README.md)


## 日本語版 README 目次
- <a href="#license">ライセンス</a>
- <a href="#how-to-use">各言語ごとの使用方法</a>
	- <a href="#how-to-use-java">Java言語での使用方法</a>
	- <a href="#how-to-use-rust">Rustでの使用方法</a>
	- <a href="#how-to-use-csharp">C#での使用方法</a>
- <a href="#about-us">開発元について</a>



<a id="license"></a>
## ライセンス

このソフトウェアはMITライセンスで公開されています。


<a id="how-to-use"></a>
## 各言語ごとの使用方法

<a id="how-to-use-java"></a>
### Java言語での使用方法

「 java 」フォルダ内に、Java言語実装版の Exevalator と用例サンプルコード類、および [Java言語用README](./java/README_JAPANESE.md) があります。
最もシンプルな用例は「 Example1.java 」で、以下のように単純な式「 1.2 + 3.4 」を計算する内容になっています：

	(in java/Example1.java)

	Exevalator exevalator = new Exevalator();
	double result = exevalator.eval("1.2 + 3.4");
	System.out.println("Result: " + result);

このコードをコンパイルして実行するには：

	cd java
	javac Exevalator.java
	javac Example1.java
	Example1

結果は以下の通りです：

	4.6

より詳しい解説や機能一覧については [Java言語用README](./java/README_JAPANESE.md) をご参照ください。


<a id="how-to-use-rust"></a>
### Rustでの使用方法

「 rust 」フォルダ内に、Rust実装版の Exevalator と用例サンプルコード類、および [Rust用README](./rust/README_JAPANESE.md) があります。
最もシンプルな用例は「 example1.rs 」で、以下のように単純な式「 1.2 + 3.4 」を計算する内容になっています：

	(in rust/example1.rs)

    let mut exevalator = Exevalator::new();
    let result: f64 = match exevalator.eval("1.2 + 3.4") {
        Ok(eval_value) => eval_value,
        Err(eval_error) => panic!("{}", eval_error),
    };
    println!("result = {}", result);

このコードをコンパイルして実行するには：

	cd rust
	rustc example1.rs

	./example1
	  または
	example1.exe

結果は以下の通りです：

	4.6

より詳しい解説や機能一覧については [Rust用README](./rust/README_JAPANESE.md) をご参照ください。


<a id="how-to-use-csharp"></a>
### C#での使用方法

「 csharp 」フォルダ内に、C#実装版の Exevalator と用例サンプルコード類、および [C#用README](./csharp/README_JAPANESE.md) があります。
最もシンプルな用例は「 Example1.cs 」で、以下のように単純な式「 1.2 + 3.4 」を計算する内容になっています：

	(in csharp/Example1.cs)

    Exevalator exevalator = new Exevalator();
    double result = exevalator.Eval("1.2 + 3.4");
    Console.WriteLine("Result: " + result);

このコードは、Visual Studio&reg;上で適当なプロジェクト内に取り込んでも実行できますが、ここで即席で動かして確認するには、コマンドラインが便利です。Visual Studio 付属の Developer Command Prompt（スタートボタン > Visual Studio 20** > … ）を起動し、その上でリポジトリのフォルダに cd して、以下のようにコンパイル/実行できます：

	cd csharp
	csc Exevalator.cs Example1.cs
	Example1.exe

結果は以下の通りです：

	4.6

より詳しい解説や機能一覧については [C#用README](./csharp/README_JAPANESE.md) をご参照ください。


<a id="about-us"></a>
## 開発元について

Exevalator は、日本の個人運営の開発スタジオ [RINEARN](https://www.rinearn.com/) が開発しています。著者は [松井文宏](https://fumihiro-matsui.xnea.net/) です。ご質問やフィードバックなどをお持ちの方は、ぜひ御気軽にどうぞ！


<a id="credits"></a>
## 本文中の商標など

- OracleとJavaは、Oracle Corporation 及びその子会社、関連会社の米国及びその他の国における登録商標です。文中の社名、商品名等は各社の商標または登録商標である場合があります。 

- Rustは、Mozilla Foundation の米国及びその他の国における登録商標です。 

- Windows、C#、Visual Studio は米国 Microsoft Corporation の米国およびその他の国における登録商標です。

- その他、文中に使用されている商標は、その商標を保持する各社の各国における商標または登録商標です。


