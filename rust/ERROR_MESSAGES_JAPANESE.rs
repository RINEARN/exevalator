// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
// エラーメッセージの言語を日本語にするには、
// 下記のコードをコピーして、
// exevalator.rs 内の ErrorMessages 構造体を上書きしてください。
// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

/// Error messages of ExevalatorException,
/// which is thrown by Exevalator if the input expression is syntactically incorrect, or uses undeclared variables/functions.
/// You can customize the error message of Exevalator by modifying the values of the properties of this class.
struct ErrorMessages;
impl ErrorMessages {
    pub const EMPTY_EXPRESSION: &'static str = "入力された計算式が空です。";
    pub const TOO_MANY_TOKENS: &'static str = "入力トークンの数が、許容上限を超過しています (StaticSettings.MAX_TOKEN_COUNT: '$0')";
    pub const DEFICIENT_OPEN_PARENTHESIS: &'static str = "開き括弧 '(' の数が足りません。";
    pub const DEFICIENT_CLOSED_PARENTHESIS: &'static str = "閉じ括弧 ')' の数が足りません。";
    pub const EMPTY_PARENTHESIS: &'static str = "括弧 '()' の中が空になっていますが、何かが必要です。";
    pub const RIGHT_OPERAND_REQUIRED: &'static str = "'$0' の右に、演算対象の値や変数が必要です。";
    pub const LEFT_OPERAND_REQUIRED: &'static str = "'$0' の左に、演算対象の値や変数が必要です。";
    pub const RIGHT_OPERATOR_REQUIRED: &'static str = "'$0' の右に、演算子（ + や - 等の演算記号）が必要です。";
    pub const LEFT_OPERATOR_REQUIRED: &'static str = "'$0' の左に、演算子（ + や - 等の演算記号）が必要です。";
    pub const UNKNOWN_UNARY_PREFIX_OPERATOR: &'static str = "'$0' は文法的に前置演算子と解釈されましたが、サポートされていない記号です。";
    pub const UNKNOWN_BINARY_OPERATOR: &'static str = "'$0' は文法的に二項演算子と解釈されましたが、サポートされていない記号です。";
    // pub const UNKNOWN_OPERATOR_SYNTAX: &'static str = "'$0' は文法的に演算子と推測されますが、サポートされていない書き方や記号です。";
    pub const EXCEEDS_MAX_AST_DEPTH: &'static str = "抽象構文木の深さが、許容上限を超過しています (StaticSettings.MAX_AST_DEPTH: '$0')";
    pub const UNEXPECTED_PARTIAL_EXPRESSION: &'static str = "部分式が、予期しない形で終わっています。";
    pub const INVALID_NUMBER_LITERAL: &'static str = "数値リテラル '$0' は、記法が想定外の形になっています。";
    // pub const INVALID_MEMORY_ADDRESS: &'static str = "アドレス '$0' は未割当か、許容領域外です。";
    pub const FUNCTION_ERROR: &'static str = "関数エラー ('$0'): $1";
    pub const VARIABLE_NOT_FOUND: &'static str = "変数が見つかりません: '$0'";
    pub const FUNCTION_NOT_FOUND: &'static str = "関数が見つかりません: '$0'";
    // pub const UNEXPECTED_OPERATOR: &'static str = "'$0' は文法的に演算子と推測されますが、種類や文法などを解釈できませんでした。";
    // pub const UNEXPECTED_TOKEN: &'static str = "トークン '$0' の種類や文法などを解釈できませんでした。";
    pub const TOO_LONG_EXPRESSION: &'static str = "式の長さが、許容上限を超過しています (StaticSettings.MAX_EXPRESSION_CHAR_COUNT: '$0')";
    // pub const UNEXPECTED_ERROR: &'static str = "通常想定されていないエラーが発生しました: $0";
    pub const REEVAL_NOT_AVAILABLE: &'static str = "\"reeval\" は、 \"eval\" を一度も使用する前にコールする事はできません。";
    pub const TOO_LONG_VARIABLE_NAME: &'static str = "変数名の長さが、許容上限を超過しています (StaticSettings.MAX_NAME_CHAR_COUNT: '$0')";
    pub const TOO_LONG_FUNCTION_NAME: &'static str = "関数名の長さが、許容上限を超過しています (StaticSettings.MAX_NAME_CHAR_COUNT: '$0')";
    pub const VARIABLE_ALREADY_DECLARED: &'static str = "変数 '$0' は既に宣言されています。";
    pub const FUNCTION_ALREADY_CONNECTED: &'static str = "関数 '$0' は既に登録されています。";
    pub const INVALID_VARIABLE_ADDRESS: &'static str = "変数のアドレス '$0' は未割当か、許容領域外です。";
}
