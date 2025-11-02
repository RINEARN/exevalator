# !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
# エラーメッセージの言語を日本語にするには、
# 下記のコードをコピーして、
# exevalator.py 内の ErrorMessages クラスを上書きしてください。
# !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

class ErrorMessages:
    """
    Error messages for ExevalatorException.

    You can customize messages by editing or overriding these class attributes.
    Placeholders `$0`, `$1`, ... are replaced by positional arguments during formatting.
    """

    EMPTY_EXPRESSION = "入力された計算式が空です。"
    TOO_MANY_TOKENS = "入力トークンの数が、許容上限を超過しています (StaticSettings.MAX_TOKEN_COUNT: '$0')"
    DEFICIENT_OPEN_PARENTHESIS = "開き括弧 '(' の数が足りません。"
    DEFICIENT_CLOSED_PARENTHESIS = "閉じ括弧 ')' の数が足りません。"
    EMPTY_PARENTHESIS = "括弧 '()' の中が空になっていますが、何かが必要です。"
    RIGHT_OPERAND_REQUIRED = "'$0' の右に、演算対象の値や変数が必要です。"
    LEFT_OPERAND_REQUIRED = "'$0' の左に、演算対象の値や変数が必要です。"
    RIGHT_OPERATOR_REQUIRED = "'$0' の右に、演算子（ + や - 等の演算記号）が必要です。"
    LEFT_OPERATOR_REQUIRED = "'$0' の左に、演算子（ + や - 等の演算記号）が必要です。"
    UNKNOWN_UNARY_PREFIX_OPERATOR = "'$0' は文法的に前置演算子と解釈されましたが、サポートされていない記号です。"
    UNKNOWN_BINARY_OPERATOR = "'$0' は文法的に二項演算子と解釈されましたが、サポートされていない記号です。"
    UNKNOWN_OPERATOR_SYNTAX = "'$0' は文法的に演算子と推測されますが、サポートされていない書き方や記号です。"
    EXCEEDS_MAX_AST_DEPTH = "抽象構文木の深さが、許容上限を超過しています (StaticSettings.MAX_AST_DEPTH: '$0')"
    UNEXPECTED_PARTIAL_EXPRESSION = "部分式が、予期しない形で終わっています。"
    INVALID_NUMBER_LITERAL = "数値リテラル '$0' は、記法が想定外の形になっています。"
    INVALID_MEMORY_ADDRESS = "アドレス '$0' は未割当か、許容領域外です。"
    FUNCTION_ERROR = "関数エラー ('$0'): $1"
    VARIABLE_NOT_FOUND = "変数が見つかりません: '$0'"
    FUNCTION_NOT_FOUND = "関数が見つかりません: '$0'"
    UNEXPECTED_OPERATOR = "'$0' は文法的に演算子と推測されますが、種類や文法などを解釈できませんでした。"
    UNEXPECTED_TOKEN = "トークン '$0' の種類や文法などを解釈できませんでした。"
    TOO_LONG_EXPRESSION = "式の長さが、許容上限を超過しています (StaticSettings.MAX_EXPRESSION_CHAR_COUNT: '$0')"
    UNEXPECTED_ERROR = "通常想定されていないエラーが発生しました: $0"
    REEVAL_NOT_AVAILABLE = "'reeval' は、 'eval' を一度も使用する前にコールする事はできません。"
    TOO_LONG_VARIABLE_NAME = "変数名の長さが、許容上限を超過しています (StaticSettings.MAX_NAME_CHAR_COUNT: '$0')"
    TOO_LONG_FUNCTION_NAME = "関数名の長さが、許容上限を超過しています (StaticSettings.MAX_NAME_CHAR_COUNT: '$0')"
    VARIABLE_ALREADY_DECLARED = "変数 '$0' は既に宣言されています。"
    FUNCTION_ALREADY_CONNECTED = "関数 '$0' は既に登録されています。"
    INVALID_VARIABLE_ADDRESS = "変数のアドレス '$0' は未割当か、許容領域外です。"

