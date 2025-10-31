    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    // エラーメッセージの言語を日本語にするには、
    // 下記のコードをコピーして、
    // exevalator.cs 内の ErrorMessage 構造体を上書きしてください。
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

    /// <summary>
    /// Error messages of ExevalatorException,
    /// which is thrown by Exevalator if the input expression is syntactically incorrect, or uses undeclared variables/functions.
    /// You can customize the error message of Exevalator by modifying the values of the properties of this class.
    /// </summary>
    public struct ErrorMessages
    {
        public static readonly string EMPTY_EXPRESSION = "入力された計算式が空です。";
        public static readonly string TOO_MANY_TOKENS = "入力トークンの数が、許容上限を超過しています (StaticSettings.MaxTokenCount: '$0')";
        public static readonly string DEFICIENT_OPEN_PARENTHESIS = "開き括弧 '(' の数が足りません。";
        public static readonly string DEFICIENT_CLOSED_PARENTHESIS = "閉じ括弧 ')' の数が足りません。";
        public static readonly string EMPTY_PARENTHESIS = "括弧 '()' の中が空になっていますが、何かが必要です。";
        public static readonly string RIGHT_OPERAND_REQUIRED = "'$0' の右に、演算対象の値や変数が必要です。";
        public static readonly string LEFT_OPERAND_REQUIRED = "'$0' の左に、演算対象の値や変数が必要です。";
        public static readonly string RIGHT_OPERATOR_REQUIRED = "'$0' の右に、演算子（ + や - 等の演算記号）が必要です。";
        public static readonly string LEFT_OPERATOR_REQUIRED = "'$0' の左に、演算子（ + や - 等の演算記号）が必要です。";
        public static readonly string UNKNOWN_UNARY_PREFIX_OPERATOR = "'$0' は文法的に前置演算子と解釈されましたが、サポートされていない記号です。";
        public static readonly string UNKNOWN_BINARY_OPERATOR = "'$0' は文法的に二項演算子と解釈されましたが、サポートされていない記号です。";
        public static readonly string UNKNOWN_OPERATOR_SYNTAX = "'$0' は文法的に演算子と推測されますが、サポートされていない書き方や記号です。";
        public static readonly string EXCEEDS_MAX_AST_DEPTH = "抽象構文木の深さが、許容上限を超過しています (StaticSettings.MaxAstDepth: '$0')";
        public static readonly string UNEXPECTED_PARTIAL_EXPRESSION = "部分式が、予期しない形で終わっています。";
        public static readonly string INVALID_NUMBER_LITERAL = "数値リテラル '$0' は、記法が想定外の形になっています。";
        public static readonly string INVALID_MEMORY_ADDRESS = "アドレス '$0' は未割当か、許容領域外です。";
        public static readonly string FUNCTION_ERROR = "関数エラー ('$0'): $1";
        public static readonly string VARIABLE_NOT_FOUND = "変数が見つかりません: '$0'";
        public static readonly string FUNCTION_NOT_FOUND = "関数が見つかりません: '$0'";
        public static readonly string UNEXPECTED_OPERATOR = "'$0' は文法的に演算子と推測されますが、種類や文法などを解釈できませんでした。";
        public static readonly string UNEXPECTED_TOKEN = "トークン '$0' の種類や文法などを解釈できませんでした。";
        public static readonly string TOO_LONG_EXPRESSION = "式の長さが、許容上限を超過しています (StaticSettings.MaxExpressionCharCount: '$0')";
        public static readonly string UNEXPECTED_ERROR = "通常想定されていないエラーが発生しました: $0";
        public static readonly string REEVAL_NOT_AVAILABLE = "\"reeval\" は、 \"eval\" を一度も使用する前にコールする事はできません。";
        public static readonly string TOO_LONG_VARIABLE_NAME = "変数名の長さが、許容上限を超過しています (StaticSettings.MaxNameCharCount: '$0')";
        public static readonly string TOO_LONG_FUNCTION_NAME = "関数名の長さが、許容上限を超過しています (StaticSettings.MaxNameCharCount: '$0')";
        public static readonly string VARIABLE_ALREADY_DECLARED = "変数 '$0' は既に宣言されています。";
        public static readonly string FUNCTION_ALREADY_CONNECTED = "関数 '$0' は既に登録されています。";
        public static readonly string INVALID_VARIABLE_ADDRESS = "変数のアドレス '$0' は未割当か、許容領域外です。";
    }
