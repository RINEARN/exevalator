# Exevalator (公式サイト フロントページ)

Exevalator（エグゼバレータ、Expression-Evaluator の略）は、プログラムやアプリ内に組み込んで、式の値の計算に使うための、コンパクトで高速なインタープリタです。 複数言語対応で、Java&trade;言語、C#、C++、Rust、Visual Basic&reg;、TypeScript 製のプログラムに組み込んで使用できます。


## 式の値を計算するライブラリ

Exevalator は、アプリやソフトを開発する際に部品として用いる、「ライブラリ」というものの一種です。

具体的には、"1 + 2" や "x + f(y)" など、「文字列として表されている式の値を計算する」という機能を提供するライブラリです。

このあたりの詳しい説明は、以下のお知らせ記事をご参照ください：

* [多言語対応＆著作権フリーの式計算ライブラリ「 Exevalator 」をリリース (2022/04/16のお知らせ)](https://www.rinearn.com/ja-jp/info/news/2022/0416-exevalator): Java/C#/C++/Rust製のソフトウェア開発で利用できる、著作権フリーの式計算ライブラリ「 Exevalator（エグゼバレータ）」を公開しました。その詳細を、用途や使い方も含めて掘り下げてお知らせします。


## 変数や関数も定義できる

式の中で使える変数や関数も、いくつでも自由な名前で定義できます。カッコなども自由に使えます。

    ((x + 1.2) * 3.4) / 5.6 + f( g(x + y) / (z - 1.23) )

なので、関数電卓のような、それなりに複雑な式を計算する処理もこなせます。

## 数億回演算/秒の処理速度

Exevalator は、毎回違う式を計算する場合でも、1秒あたり数万～数十万式ほどの速さで計算できます。

さらに、変数値を変えながら、同じ式を反復計算するような場合には、1秒あたり数千万式ほどの高速計算が可能です。 より正確に、式が含む加減算などの演算単位で表すと、最大で数億回演算/秒（数百MFLOPS）程度の速度を発揮します。

## 多言語対応
Exevalator は、複数の言語で利用できます。現在は Java&trade;、C++、C#、Rust、Visual Basic&reg;、および TypeScript で利用可能です。

コンパクトで移植が比較的容易なので、他のメジャーな言語にも、今後対応するかもしれません。

## 導入はソースコード 1枚
Exevalator は、ソースコード 1枚に収まるように実装されています（C++版のみヘッダ込みで2枚）。

なので、開発アプリ/ソフトのソースコードフォルダ内に、上記の1枚を放り込むだけで、すぐに使えます。

## 著作権フリー、改造/流用も自由

Exevalator のライセンスは、実質的な著作権フリーである「CC0」と「Unlicense」から自由に選べます。 アプリやソフトを公開・販売する際に、「Exevalator を使った」等と明記する必要はありません。

また、ソースコードの改造/流用も自由に行えます。その際、以下の記事が参考になるかもしれません：

* [Exevalator の内部アーキテクチャ解説 (2022/05/04のお知らせ)](https://www.rinearn.com/ja-jp/info/news/2022/0504-exevalator-architecture): オープンソースの式計算ライブラリ「 Exevalator（エグゼバレータ）」の内部構造を、全体像から各コンポーネントの役割まで、一つ一つ掘り下げながら解説します。

## さあ、あなたも

Exevalator は、どなたでも無料でご利用いただけます。

* [ダウンロード](https://github.com/RINEARN/exevalator/releases)


---

\- 本文中の商標等について -

* OracleとJavaは、Oracle Corporation 及びその子会社、関連会社の米国及びその他の国における登録商標です。文中の社名、商品名等は各社の商標または登録商標である場合があります。
* C#とVisual Basicは、米国 Microsoft Corporation の米国およびその他の国における登録商標です。
* Rustは、Mozilla Foundation の米国及びその他の国における登録商標です。
* その他、文中に使用されている商標は、その商標を保持する各社の各国における商標または登録商標です。

