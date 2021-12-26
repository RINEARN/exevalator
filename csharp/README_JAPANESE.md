# C#&reg; での Exevalator の使用方法

&raquo; [English](./README.md)


## 日本語版 目次
- <a href="#requirements">必要な環境</a>
- <a href="#how-to-use">使用方法</a>
- <a href="#example-code">サンプルコードの実行方法（およびコマンドラインでの使用方法）</a>
- <a href="#features">主な機能</a>





<a id="requirements"></a>
## 必要な環境

* Microsoft&reg; Visual Studio&reg;、 2022 以降推奨
* またはその他の C# 処理系、C# 8.0 以降推奨



<a id="how-to-use"></a>
## 使用方法

Exevalator のインタープリタは、単一のファイル「 csharp/Exevalator.cs 」内に実装されています。そのため、以下のような 2 ステップで、簡単に読み込んでして使用できます。

### 1. 使用したいプロジェクトに Exevalator.rs を追加する

開発に Visual Studio のIDEを使用する場合（コマンドラインの場合は後述）は、まず「 csharp/Exevalator.cs 」を、使用したいプロジェクト内に追加してください。

これには、ソリューションエクスプローラでプロジェクトを右クリックして、メニューから「追加」>「既存の項目」を選び、そしてファイル選択画面で Exevalator.cs を選びます。プロジェクト内のリストに Exevalator.cs が表示されるようになれば追加成功です。

### 2. 使用したいコードから読み込む

あとは、式の計算を行いたいコードのファイルから、以下のように using する事で、読み込んで使用できます：

	...
	using Rinearn.ExevalatorCS;
	...

	class YourClass
	{
		...
		void YourMethod()
		{
			// Exevalator のインタープリタを生成
			Exevalator exevalator = new Exevalator();

			// 式の値を計算（評価）する
			double result = exevalator.Eval("1.2 + 3.4");
			
			// 結果を表示
			Console.WriteLine("result: " + result);
		}
		...
	}

なお、Exevalator では、式の中のすべての数値は double 型で扱われます。従って、結果も常に double 型です。
ただし、式の内容がおかしい場合や、未宣言の変数を使った場合など、計算に失敗する場合もあり得ます。その場合は、Eval メソッドを呼んでいる箇所で例外 ExevalatorException がスローされますので、必要に応じて catch してハンドルしてください。


<a id="example-code"></a>
## サンプルコードの使用方法（およびコマンドラインでの使用方法）

このリポジトリ内には、実際に Exevalator を使用する簡単なサンプルコード類「 csharp/Example*.cs 」も同梱されています。

これらも先と同様、Visual Studio のIDE上で適当なプロジェクト内に取り込んでも実行できますが、ここで即席で動かして確認するには、コマンドライン実行が便利です。方法としては、Visual Studio 付属の Developer Command Prompt（スタートボタン > Visual Studio 20** > … ）を起動し、その上でリポジトリのフォルダに cd して、以下のようにコンパイル/実行できます：

	cd csharp
	csc Exevalator.cs Example1.cs
	Example1.exe

上記の「 Example1.cs 」は、"1.2 + 3.4" の値を Exevalator で計算するサンプルコードです。内容は、すぐ前の節で掲載したサンプル「 YourClass 」とほぼ同一です：

	...
	double result = exevalator.Eval("1.2 + 3.4");

実行結果は：

	result: 4.6

この通り、"1.2 + 3.4" の計算結果が表示されます。


<a id="features"></a>
## 主な機能

以下では、Exevalator の主な機能を紹介します.

### 1. 式の評価（計算）

これまでのセクションでも見てきたように、Exevalator で式の値を計算できます: 

	double result = exevalator.Eval("(-(1.2 + 3.4) * 5) / 2");
	// result: -11.5

	(参照: csharp/Example2.cs)

上記のように、"+" (足し算)、 "-" (引き算や数値のマイナス化)、"\*" (掛け算)、"/" (割り算) の演算を行えます。なお、掛け算と割り算は、足し算と引き算よりも、順序的に優先されます。


### 2. 変数の使用

変数を宣言し、その値に式の中からアクセスできます：

	// 変数を宣言して値を設定
	exevalator.DeclareVariable("x");
	exevalator.WriteVariable("x", 1.25);

	// 変数の値を使う式を計算する
	double result = exevalator.Eval("x + 1");
	// result: 2.25

	(参照: csharp/Example3.cs)

変数の値を非常に頻繁に書き変えるような用途では、書き変え対象の変数を、以下のようにアドレスによって指定する事も有用です：

	int address = exevalator.DeclareVariable("x");
	exevalator.WriteVariableAt(address, 1.25);
	...

	(参照: csharp/Example4.cs)

この方法は、書き変え対象変数を名前で指定するよりも高速です。

### 3. 関数の使用

式の中で使用するための関数も作成できます。それには、Exevalator.FunctionInterface インターフェースを実装したクラスを作成します：

	// 式内で使用できる関数を作成
	class MyFunction : IExevalatorFunction
	{
		public double invoke(double[] arguments)
		{
			if (arguments.Length != 2)
			{
				throw new ExevalatorException("Incorrected number of args");
			}
			return arguments[0] + arguments[1];
		}
	}
	...

	// 上記の関数を式内で使用できるよう接続
	Exevalator exevalator = new Exevalator();
	MyFunction fun = new MyFunction();
	exevalator.ConnectFunction("fun", fun);

	// 関数を使う式を計算する
	double result = exevalator.Eval("fun(1.2, 3.4)");
	Console.WriteLine("Result: " + result);
	// result: 4.6

	(参照: csharp/Example5.cs)



<a id="credits"></a>
## 本文中の商標など

- Windows、C#、Visual Studio は、米国 Microsoft Corporation の米国およびその他の国における登録商標です。

- その他、文中に使用されている商標は、その商標を保持する各社の各国における商標または登録商標です。


