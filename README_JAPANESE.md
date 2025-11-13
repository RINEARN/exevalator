# Exevalator

![logo](logo.png)

Exevalator（**Ex**pression-**Eval**u**ator** の略） は、プログラムやアプリ内に組み込んで、式の値の計算に使うための、コンパクトで高速なインタープリタです。

複数言語対応で、Java&trade;言語、Rust、C#、C++、Visual Basic&reg;, TypeScript, Python 製のプログラムに組み込んで使用できます。

加えて MCP もサポートしており、AI用の計算ツールとしても使用できます。

&raquo; [English README](./README.md)

&raquo; [公式サイト](https://www.rinearn.com/ja-jp/exevalator/)

&raquo; [AIに使い方を聞く（ChatGPTのアカウントが必要）](https://chatgpt.com/g/g-UjkEJeO8x-exevalator-assistant)

<hr />

## 日本語版 README 目次
- <a href="#what-is">Exevalator とは？</a>
- <a href="#license">ライセンス（著作権フリー）</a>
- <a href="#how-to-use">各言語ごとの使用方法</a>
	- <a href="#how-to-use-java">Java言語での使用方法</a>
	- <a href="#how-to-use-rust">Rustでの使用方法</a>
	- <a href="#how-to-use-csharp">C#での使用方法</a>
	- <a href="#how-to-use-cpp">C++での使用方法</a>
	- <a href="#how-to-use-vb">Visual Basic での使用方法</a>
	- <a href="#how-to-use-typescript">TypeScript での使用方法</a>
	- <a href="#how-to-use-python">Python での使用方法</a>
	- <a href="#how-to-use-mcp">MCP での使用方法</a>
- <a href="#customize-error-languages">エラーメッセージの日本語化やカスタマイズ</a>
- <a href="#performance">処理速度</a>
- <a href="#about-us">開発元について</a>
- <a href="#references">参考情報</a>


<a id="what-is"></a>
## Exevalator とは？

何らかのソフトウェアを開発している際に、「文字列変数などに格納された式の値を計算（評価）したい 」 といった場面に出会った事はありませんか？ 例えば：

	"1 + 2"
	"(1.2 + 3.4) * 5.6"
	"x + f(y)"

などです。

大抵のコンパイラ型の（スクリプト言語ではない）言語では、そのような機能は、標準ではサポートされていません。そのため、計算処理を自力で実装するか、そのような機能を提供するライブラリを使用する必要があります。

Exevalator は、 そのような機能を提供する、非常にコンパクトなライブラリです。多言語に対応していて、現在は Java / Rust / C# / C++ / Visual Basic / TypeScript 製のソフトウェア開発で使用できます。


<a id="license"></a>
## ライセンス（著作権フリー）

このライブラリは、実質的な著作権フリー（パブリックドメイン）宣言である「 Unlicense 」ライセンスの下で公開されています。

なお、ユーザーの希望に応じて、代わりに [CC0](https://creativecommons.org/publicdomain/zero/1.0/deed.ja) ライセンスも選択できます。CC0もパブリックドメイン宣言を行うためのものですが、Unlicense と微妙な差異があります。好きな方を選んでください。


<a id="how-to-use"></a>
## 各言語ごとの使用方法

<a id="how-to-use-java"></a>
### Java言語での使用方法

「 java 」フォルダ内に、Java言語実装版の Exevalator と用例サンプルコード類、および [Java言語用README](./java/README_JAPANESE.md) があります。
最もシンプルな用例は「 Example1.java 」で、以下のように単純な式「 1.2 + 3.4 」を計算する内容になっています：

	(in java/Example1.java)

	Exevalator exevalator = new Exevalator();
	double result = exevalator.eval("1.2 + 3.4");
	System.out.println("result: " + result);

このコードをコンパイルして実行するには：

	cd java
	javac Exevalator.java
	javac Example1.java
	java Example1

結果は以下の通りです：

	result: 4.6

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
	println!("result: {}", result);

このコードをコンパイルして実行するには：

	cd rust
	rustc example1.rs

	./example1
	  または
	example1.exe

結果は以下の通りです：

	result: 4.6

より詳しい解説や機能一覧については [Rust用README](./rust/README_JAPANESE.md) をご参照ください。


<a id="how-to-use-csharp"></a>
### C#での使用方法

「 csharp 」フォルダ内に、C#実装版の Exevalator と用例サンプルコード類、および [C#用README](./csharp/README_JAPANESE.md) があります。
最もシンプルな用例は「 Example1.cs 」で、以下のように単純な式「 1.2 + 3.4 」を計算する内容になっています：

	(in csharp/Example1.cs)

	Exevalator exevalator = new Exevalator();
	double result = exevalator.Eval("1.2 + 3.4");
	Console.WriteLine("result: " + result);

このコードは、Visual Studio&reg;上で適当なプロジェクト内に取り込んでも実行できますが、ここで即席で動かして確認するには、コマンドラインが便利です。その場合、Visual Studio に付属している Developer Command Prompt 上で、以下のようにコンパイル/実行できます：

	cd csharp
	csc Example1.cs Exevalator.cs
	Example1.exe

結果は以下の通りです：

	result: 4.6

より詳しい解説や機能一覧については [C#用README](./csharp/README_JAPANESE.md) をご参照ください。


<a id="how-to-use-cpp"></a>
### C++での使用方法

「 cpp 」フォルダ内に、C++実装版の Exevalator と用例サンプルコード類、および [C++用README](./cpp/README_JAPANESE.md) があります。
最もシンプルな用例は「 example1.cpp 」で、以下のように単純な式「 1.2 + 3.4 」を計算する内容になっています：

	(in cpp/example1.cpp)

	exevalator::Exevalator exevalator;
	try {
		double result = exevalator.eval("1.2 + 3.4");
		std::cout << "result: " << result << std::endl;
	} catch (exevalator::ExevalatorException &e) {
		std::cerr << e.what() << std::endl;
	}

このコードをコンパイルして実行する方法は、環境やコンパイラによって異なりますが、一例として Linux 上で clang++ を使用する場合は：

	cd cpp
	clang++ -std=c++17 -Wall -o example1 example1.cpp
	./example1

結果は以下の通りです：

	result: 4.6

より詳しい解説や機能一覧については [C++用README](./cpp/README_JAPANESE.md) をご参照ください。


<a id="how-to-use-vb"></a>
### Visual Basic での使用方法

「 vb 」フォルダ内に、Visual Basic（VB.NET）実装版の Exevalator と用例サンプルコード類、および [Visual Basic用README](./vb/README_JAPANESE.md) があります。
最もシンプルな用例は「 Example1.vb 」で、以下のように単純な式「 1.2 + 3.4 」を計算する内容になっています：

	(in vb/Example1.vb)

	exevalator As Exevalator = New Exevalator()
	result As Double = exevalator.Eval("1.2 + 3.4")
	Console.WriteLine("result: " + result.ToString())

このコードは、Visual Studio&reg;上で適当なプロジェクト内に取り込んでも実行できますが、ここで即席で動かして確認するには、コマンドラインが便利です。その場合、Visual Studio に付属している Developer Command Prompt 上で、以下のようにコンパイル/実行できます：

	cd vb
	vbc Example1.vb Exevalator.vb
	Example1.exe

結果は以下の通りです：

	result: 4.6

より詳しい解説や機能一覧については [Visual Basic 用README](./vb/README_JAPANESE.md) をご参照ください。


<a id="how-to-use-typescript"></a>
### TypeScript での使用方法

「 typescript 」フォルダ内に、Typescript実装版の Exevalator と用例サンプルコード類、および [TypeScript 用README](./typescript/README_JAPANESE.md) があります。
最もシンプルな用例は「 example1.ts 」で、以下のように単純な式「 1.2 + 3.4 」を計算する内容になっています：

	(in typescript/example1.ts)

	let exevalator: Exevalator = new Exevalator();
	const result: number = exevalator.eval("1.2 + 3.4");
	console.log(`result: ${result}`);

このコードをコンパイルして実行するには：

	npx tsc example1.ts
	node example1.js

結果は以下の通りです：

	result: 4.6

Webブラウザ上で動作させる場合は、esbuild 等のバンドラツールで一枚の JavaScript ファイルにビルドした上で、HTMLファイルから簡単に読み込んで使えます（例: example6 & 7）。

より詳しい解説や機能一覧については [TypeScript 用README](./typescript/README_JAPANESE.md) をご参照ください。


<a id="how-to-use-python"></a>
### Python での使用方法

「 python 」フォルダ内に、Python実装版の Exevalator と用例サンプルコード類、および [Python用README](./python/README_JAPANESE.md) があります。
最もシンプルな用例は「 example1.py 」で、以下のように単純な式「 1.2 + 3.4 」を計算する内容になっています：

	(in python/example1.java)

	ex = Exevalator()
	result = ex.eval("1.2 + 3.4")
	print(f"result: {result}")

このコードを実行するには：

	cd python
	python example1.py

結果は以下の通りです：

	result: 4.6

より詳しい解説や機能一覧については [Python用README](./python/README_JAPANESE.md) をご参照ください。


<a id="how-to-use-mcp"></a>
### MCP での使用方法

Exevalator は MCP (Model Context Protocol) をサポートしており、AI用の計算ツールとしても利用可能です。
「 mcp 」フォルダ内に、そのためのコード類と [MCP用README](./mcp/README_JAPANESE.md) があります。

要点のみ抜き出すと、まず home 直下などの適当な場所に Exealator のパッケージを配置し、その中の mcp フォルダ内に環境を構築します：

	# 適当な場所（ここでは home 直下）にMCPツール置き場を作る
	cd ~
	mkdir mcp-tools
	cd mcp-tools
	
	# Exevalator のパッケージを配置
	git clone https://github.com/RINEARN/exevalator.git

	# その中の mcp フォルダ内に環境を構築
	cd exevalator/mcp
	uv init -p 3.13 --name exevalator-mcp .
	uv add "mcp[cli]"

続いて、AI利用環境（例: Visual Studio Code + Cline 拡張など）のMCPサーバ指定ファイルに、以下の例のように追記して登録します：

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

登録できたら、AIに以下のように聞いてみましょう：

	MCPサーバーの Exevalator を用いて、「1.2 + 3.4」の値を計算してください。
	他の方法ではなく、必ず exevalator を用いてください。

正常に動いている場合の返答例は：

	Exevalator MCPサーバーの evaluate_expression で「1.2 + 3.4」を計算し、結果は 4.6 であることを確認しました。
	指定どおり Exevalator を用いて算出しています。

より詳しい解説や機能一覧については [MCP用README](./mcp/README_JAPANESE.md) をご参照ください。


<a id="customize-error-languages"></a>
## エラーメッセージの日本語化やカスタマイズ

エラーメッセージは、標準状態では英語になっています。日本語化するには、下記の手順でソースコードを編集してください：

* まず、各言語実装版のフォルダ内にある「ERROR_MESSAGES_JAPANESE.\* (\*の箇所は実装言語に応じた拡張子)」の内容をコピーする。

* Exevalator 本体のソースコード（Exevalator.\* または exevalator.\* ）内の先頭付近にある、ErrorMessage クラス/構造体の内容を、上記内容で上書きする。

エラーメッセージの内容をカスタマイズしたい場合は、上記の ErrorMessage クラス/構造体の内容を編集してください。


<a id="performance"></a>
## 処理速度

### 大まかな処理速度の目安

Exevalator は、計算/データ解析用のソフトなどでの利用を考慮して、処理速度をある程度重視した設計を採用しています。

特に、同じ式を繰り返し計算する際などに、Exevalator は高速に動作します。以下は典型例です：


	// 変数「x」を宣言してアドレスを取得
	int varAddress = exevalator.declareVariable("x");

	// 変数の値を変えながら、式の値を計算して積算するループ（10演算/周）
	double result = 0.0;
	for (long i=1; i<=loops; ++i) {
		exevalator.writeVariableAt(varAddress, (double)i);
		result += exevalator.eval("x + 1 - 1 + 1 - 1 + 1 - 1 + 1 - 1 + 1 - 1");
	}

	(参照: java/Benchmark.java)

上記自体は意味のないコードですが、似たような形の処理は、計算用途などではよくあります。

このコードのループは、環境にもよりますが、だいたい数千万回/秒くらいのスピードで回ります。従って演算速度は数億回/秒（数百 MFLOPS, ※）程度の水準になります。これは、それなりの長さの配列データに変換をかけたり、密に刻んだ点で式の座標値をサンプリングしたりするのに実用的な速度です。

> ※ 処理速度は、どの言語での実装版かによって大きく異なります。数百 MFLOPS は、コンパイラ型言語や、JITを備えるスクリプト言語処理系での目安です。
> 
> 逆に、Python版で、それらを備えない標準環境では、処理速度は 1 ～ 2 桁程度遅くなります（数 M ～ 10 MFLOPS 程度の水準）。


### 式が頻繁に変化する場合のパフォーマンスチューニング

一方で、上記の速度水準は、式が毎回同じであり、従って構文解析結果などの大部分を Exevalator 内部でキャッシュして、毎回流用できる事が大きく効いています。それが無理な場合、例えば：

	...
	for (long i=0; i<loops; ++i) {
		exevalator.writeVariableAt(varAddress, (double)i);
		result += exevalator.eval("x + 1 - 1 + 1 - 1 + 1"); // 微妙に下と違う式
		result += exevalator.eval("x - 1 + 1 - 1 + 1 - 1"); // 微妙に上と違う式
	}

このような場合には、ループはだいたい数万～数十万回/秒のスピードで回ります。先ほどの100倍以上も遅い水準ですね。

しかしこの種の速度低下は、Exevalator のインスタンスを式ごとに複数用意して、それぞれは同じ式を計算するようにすれば回避できます：

	...
	for (long i=0; i<loops; ++i) {
		exevalatorA.writeVariableAt(varAddressA, (double)i);
		exevalatorB.writeVariableAt(varAddressB, (double)i);
		result += exevalatorA.eval("x + 1 - 1 + 1 - 1 + 1"); // 微妙に下と違う式
		result += exevalatorB.eval("x - 1 + 1 - 1 + 1 - 1"); // 微妙に上と違う式
	}

このようにインスタンスを分けると、キャッシュがよく効き、再び100倍くらい速くなります。


### より明示的にキャッシュを使う reeval( ) メソッド

なお、ここで行っているように同じ式を反復計算する際には、引数無しで前回 eval 時と同じ式を計算する「 reeval( ) 」というメソッドも用意されています。
eval メソッドの代わりにこれを使うと、いわば明示的に Exevalator 内部のキャッシュを使った計算を行える（新規で構文解析などが発生する余地がない）上に、式の同一性の判定処理なども不要となるため、原理的には少し速くなる事が見込めます。

具体的には、同梱のベンチマークコードにおいて、C++ 実装版では 1.4 倍程度、Rust 実装版では 1.1 ～ 1.2 倍程度の速度向上が見込めます。一方で Java 言語実装版と C# 実装版では、誤差程度しか変わらないようです（従って呼び方によっては、初回計算と2回目以降の分岐が増えて、かえって遅くなる場合もあると思います）。


<a id="about-us"></a>
## 開発元について

Exevalator は、日本の個人運営の開発スタジオ [RINEARN](https://www.rinearn.com/) が開発しています。著者は松井文宏です。ご質問やフィードバックなどをお持ちの方は、ぜひ御気軽にどうぞ！


<a id="references"></a>
## 参考情報

Exevalator についての情報をもっと知りたい場合は、以下のウェブサイトや記事などが参考になるかもしれません。

* [公式サイト](https://www.rinearn.com/ja-jp/exevalator/)

* [多言語対応＆著作権フリーの式計算ライブラリ「 Exevalator（エグゼバレータ）」をリリース](https://www.rinearn.com/ja-jp/info/news/2022/0416-exevalator) - RINEARN お知らせ 2022/04/16

* [Exevalator の内部アーキテクチャ解説](https://www.rinearn.com/ja-jp/info/news/2022/0504-exevalator-architecture) - RINEARN お知らせ 2022/05/04


<a id="credits"></a>
## 本文中の商標など

- OracleとJava、JavaScriptは、Oracle Corporation 及びその子会社、関連会社の米国及びその他の国における登録商標です。文中の社名、商品名等は各社の商標または登録商標である場合があります。 

- Rustは、Mozilla Foundation の米国及びその他の国における登録商標です。 

- Windows、C#、Visual Basic、.NET、Visual Studio は米国 Microsoft Corporation の米国およびその他の国における登録商標です。

- Linux は、Linus Torvalds 氏の米国およびその他の国における商標または登録商標です。

- Node.js は、OpenJS Foundation による米国またはその他の国における商標または登録商標です。

- Python は、 Python Software Foundation の米国及びその他の国における登録商標です。

- MCP (Model Context Protocol) は、Anthropic 社 (Anthropic, PBC) が提唱した通信プロトコルです。

- Visual Studio Code は、米国 Microsoft Corporation の米国およびその他の国における登録商標です。

- Cline は、米国 Cline Bot Inc. によるAIツールです。

- uv は、Astral社によるPython用のパッケージ管理ツールです。

- Git は、Software Freedom Conservancy, Inc. の米国およびその他の国における商標または登録商標です。

- ChatGPT は、米国 OpenAI OpCo, LLC による米国またはその他の国における商標または登録商標です。

- その他、文中に使用されている商標は、その商標を保持する各社の各国における商標または登録商標です。


