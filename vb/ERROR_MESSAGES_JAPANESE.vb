    ''' !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    ''' エラーメッセージの言語を日本語にするには、
    ''' 下記のコードをコピーして、
    ''' Exevalator.vb 内の ErrorMessages 構造体を上書きしてください。
    ''' !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

    ''' <summary>
    ''' Error messages of ExevalatorException,
    ''' which is thrown by Exevalator if the input expression is syntactically incorrect, or uses undeclared variables/functions.
    ''' You can customize the error message of Exevalator by modifying the values of the properties of this class.
    ''' </summary>
    Public Structure ErrorMessages
        Public Const EMPTY_EXPRESSION As String = "入力された計算式が空です。"
        Public Const TOO_MANY_TOKENS As String = "入力トークンの数が、許容上限を超過しています (StaticSettings.MaxTokenCount: '$0')"
        Public Const DEFICIENT_OPEN_PARENTHESIS As String = "開き括弧 '(' の数が足りません。"
        Public Const DEFICIENT_CLOSED_PARENTHESIS As String = "閉じ括弧 ')' の数が足りません。"
        Public Const EMPTY_PARENTHESIS As String = "括弧 '()' の中が空になっていますが、何かが必要です。"
        Public Const RIGHT_OPERAND_REQUIRED As String = "'$0' の右に、演算対象の値や変数が必要です。"
        Public Const LEFT_OPERAND_REQUIRED As String = "'$0' の左に、演算対象の値や変数が必要です。"
        Public Const RIGHT_OPERATOR_REQUIRED As String = "'$0' の右に、演算子（ + や - 等の演算記号）が必要です。"
        Public Const LEFT_OPERATOR_REQUIRED As String = "'$0' の左に、演算子（ + や - 等の演算記号）が必要です。"
        Public Const UNKNOWN_UNARY_PREFIX_OPERATOR As String = "'$0' は文法的に前置演算子と解釈されましたが、サポートされていない記号です。"
        Public Const UNKNOWN_BINARY_OPERATOR As String = "'$0' は文法的に二項演算子と解釈されましたが、サポートされていない記号です。"
        Public Const UNKNOWN_OPERATOR_SYNTAX As String = "'$0' は文法的に演算子と推測されますが、サポートされていない書き方や記号です。"
        Public Const EXCEEDS_MAX_AST_DEPTH As String = "抽象構文木の深さが、許容上限を超過しています (StaticSettings.MaxAstDepth: '$0')"
        Public Const UNEXPECTED_PARTIAL_EXPRESSION As String = "部分式が、予期しない形で終わっています。"
        Public Const INVALID_NUMBER_LITERAL As String = "数値リテラル '$0' は、記法が想定外の形になっています。"
        Public Const INVALID_MEMORY_ADDRESS As String = "アドレス '$0' は未割当か、許容領域外です。"
        Public Const FUNCTION_ERROR As String = "関数エラー ('$0'): $1"
        Public Const VARIABLE_NOT_FOUND As String = "変数が見つかりません: '$0'"
        Public Const FUNCTION_NOT_FOUND As String = "関数が見つかりません: '$0'"
        Public Const UNEXPECTED_OPERATOR As String = "'$0' は文法的に演算子と推測されますが、種類や文法などを解釈できませんでした。"
        Public Const UNEXPECTED_TOKEN As String = "トークン '$0' の種類や文法などを解釈できませんでした。"
        Public Const TOO_LONG_EXPRESSION As String = "式の長さが、許容上限を超過しています (StaticSettings.MaxExpressionCharCount: '$0')"
        Public Const UNEXPECTED_ERROR As String = "通常想定されていないエラーが発生しました: $0"
        Public Const REEVAL_NOT_AVAILABLE As String = """reeval"" は、 ""eval"" を一度も使用する前にコールする事はできません。"
        Public Const TOO_LONG_VARIABLE_NAME As String = "変数名の長さが、許容上限を超過しています (StaticSettings.MaxNameCharCount: '$0')"
        Public Const TOO_LONG_FUNCTION_NAME As String = "関数名の長さが、許容上限を超過しています (StaticSettings.MaxNameCharCount: '$0')"
        Public Const VARIABLE_ALREADY_DECLARED As String = "変数 '$0' は既に宣言されています。"
        Public Const FUNCTION_ALREADY_CONNECTED As String = "関数 '$0' は既に登録されています。"
        Public Const INVALID_VARIABLE_ADDRESS As String = "変数のアドレス '$0' は未割当か、許容領域外です。"
    End Structure
