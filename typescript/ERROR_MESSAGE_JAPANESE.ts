// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
// エラーメッセージの言語を日本語にするには、
// 下記のコードをコピーして、
// exevalator.ts 内の ErrorMessage クラスを上書きしてください。
// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

/**
 * Error messages of ExevalatorError,
 * which is thrown by Exevalator if the input expression is syntactically incorrect, or uses undeclared variables/functions.
 * You can customize the error message of Exevalator by modifying the values of the properties of this class.
 */
class ErrorMessages {
    public static readonly EMPTY_EXPRESSION: string = "入力された計算式が空です。";
    public static readonly TOO_MANY_TOKENS: string = "入力トークンの数が、許容上限を超過しています (StaticSettings.MAX_TOKEN_COUNT: '$0')";
    public static readonly DEFICIENT_OPEN_PARENTHESIS: string = "開き括弧 '(' の数が足りません。";
    public static readonly DEFICIENT_CLOSED_PARENTHESIS: string = "閉じ括弧 ')' の数が足りません。";
    public static readonly EMPTY_PARENTHESIS: string = "括弧 '()' の中が空になっていますが、何かが必要です。";
    public static readonly RIGHT_OPERAND_REQUIRED: string = "'$0' の右に、演算対象の値や変数が必要です。";
    public static readonly LEFT_OPERAND_REQUIRED: string = "'$0' の左に、演算対象の値や変数が必要です。";
    public static readonly RIGHT_OPERATOR_REQUIRED: string = "'$0' の右に、演算子（ + や - 等の演算記号）が必要です。";
    public static readonly LEFT_OPERATOR_REQUIRED: string = "'$0' の左に、演算子（ + や - 等の演算記号）が必要です。";
    public static readonly UNKNOWN_UNARY_PREFIX_OPERATOR: string = "'$0' は文法的に前置演算子と解釈されましたが、サポートされていない記号です。";
    public static readonly UNKNOWN_BINARY_OPERATOR: string = "'$0' は文法的に二項演算子と解釈されましたが、サポートされていない記号です。";
    public static readonly UNKNOWN_OPERATOR_SYNTAX: string = "'$0' は文法的に演算子と推測されますが、サポートされていない書き方や記号です。";
    public static readonly EXCEEDS_MAX_AST_DEPTH: string = "抽象構文木の深さが、許容上限を超過しています (StaticSettings.MAX_AST_DEPTH: '$0')";
    public static readonly UNEXPECTED_PARTIAL_EXPRESSION: string = "部分式が、予期しない形で終わっています。";
    public static readonly INVALID_NUMBER_LITERAL: string = "数値リテラル '$0' は、記法が想定外の形になっています。";
    public static readonly INVALID_MEMORY_ADDRESS: string = "アドレス '$0' は未割当か、許容領域外です。";
    public static readonly ADDRESS_MUST_BE_ZERO_OR_POSITIVE_INT32: string = "アドレスは、正の32ビット整数の範囲内（0を含む）である必要があります: '$0'";
    public static readonly FUNCTION_ERROR: string = "関数エラー ('$0'): $1";
    public static readonly VARIABLE_NOT_FOUND: string = "変数が見つかりません: '$0'";
    public static readonly FUNCTION_NOT_FOUND: string = "関数が見つかりません: '$0'";
    public static readonly UNEXPECTED_OPERATOR: string = "'$0' は文法的に演算子と推測されますが、種類や文法などを解釈できませんでした。";
    public static readonly UNEXPECTED_TOKEN: string = "トークン '$0' の種類や文法などを解釈できませんでした。";
    public static readonly ARGS_MUST_NOT_BE_NULL_OR_UNDEFINED: string = "関数の引数 '$0' が、null か undefined になっているため、渡せませんでした。";
    public static readonly TOO_LONG_EXPRESSION: string = "式の長さが、許容上限を超過しています (StaticSettings.MAX_EXPRESSION_CHAR_COUNT: '$0')";
    public static readonly UNEXPECTED_ERROR: string = "通常想定されていないエラーが発生しました: $0";
    public static readonly REEVAL_NOT_AVAILABLE: string = "\"reeval\" は、 \"eval\" を一度も使用する前にコールする事はできません。";
    public static readonly TOO_LONG_VARIABLE_NAME: string = "変数名の長さが、許容上限を超過しています (StaticSettings.MAX_NAME_CHAR_COUNT: '$0')";
    public static readonly TOO_LONG_FUNCTION_NAME: string = "関数名の長さが、許容上限を超過しています (StaticSettings.MAX_NAME_CHAR_COUNT: '$0')";
    public static readonly VARIABLE_ALREADY_DECLARED: string = "変数 '$0' は既に宣言されています。";
    public static readonly FUNCTION_ALREADY_CONNECTED: string = "関数 '$0' は既に登録されています。";
    public static readonly INVALID_VARIABLE_ADDRESS: string = "変数のアドレス '$0' は未割当か、許容領域外です。";
    public static readonly VARIABLE_COUNT_EXCEEDED_LIMIT: string = "変数の宣言数が、許容上限を超過しています: '$0'";
}
