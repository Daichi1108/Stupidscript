using System.Security.Permissions;

namespace Stupidscript
{

    enum TokenType {
        Var, FuncDeclaration,
        Identifier,
        Equals, BinaryOperator, OperatorEquals, IncrementOperation,
        OpenParen, CloseParen,
        OpenCurly, CloseCurly,
        OpenBracket, CloseBracket,
        NumLiteral, StringLiteral,
        Dot, Comma,
        Arrow, Colon,
        LineEnd,
        ConditionalOperator,
        If, While, For,
        EOF
    }
    struct Token {
        public string value {get;}
        public TokenType type {get;}

        public Token(string value, TokenType type) {
            this.value = value;
            this.type = type;
        }

        public override string ToString() {
            return $"|{value} : {type}|";
        }
    }

    class Lexer {

        public List<Token> Tokenize(string code) {
            code = ToLowerCase(code) + " ";
            List<Token> tokens = new List<Token>();

            while (code.Length > 0) {
                LexToken();
            }
            tokens.Add(new Token("EOF", TokenType.EOF));

            return tokens;

            void LexToken() {
                if (FirstIndexed("hippopotamus", TokenType.Var)) return;
                if (FirstIndexed("ptrodactyl", TokenType.FuncDeclaration)) return;
                if (FirstIndexed("whil", TokenType.While)) return;
                if (FirstIndexed("for", TokenType.For)) return;
                if (FirstIndexed("if", TokenType.If)) return;
                if (FirstIndexed("==", TokenType.ConditionalOperator)) return;
                if (FirstIndexed(">=", TokenType.ConditionalOperator)) return;
                if (FirstIndexed("<=", TokenType.ConditionalOperator)) return;
                if (FirstIndexed("!=", TokenType.ConditionalOperator)) return;
                if (FirstIndexed("+=", TokenType.OperatorEquals)) return;
                if (FirstIndexed("-=", TokenType.OperatorEquals)) return;
                if (FirstIndexed("*=", TokenType.OperatorEquals)) return;
                if (FirstIndexed("/=", TokenType.OperatorEquals)) return;
                if (FirstIndexed("%=", TokenType.OperatorEquals)) return;
                if (FirstIndexed("++", TokenType.IncrementOperation)) return;
                if (FirstIndexed("--", TokenType.IncrementOperation)) return;
                if (FirstIndexed("->", TokenType.Arrow)) return;
                if (FirstIndexed(">", TokenType.ConditionalOperator)) return;
                if (FirstIndexed("<", TokenType.ConditionalOperator)) return;
                if (FirstIndexed("+", TokenType.BinaryOperator)) return;
                if (FirstIndexed("-", TokenType.BinaryOperator)) return;
                if (FirstIndexed("*", TokenType.BinaryOperator)) return;
                if (FirstIndexed("/", TokenType.BinaryOperator)) return;
                if (FirstIndexed("%", TokenType.BinaryOperator)) return;
                if (FirstIndexed("=", TokenType.Equals)) return;
                if (FirstIndexed(":", TokenType.Colon)) return;
                if (FirstIndexed(".", TokenType.Dot)) return;
                if (FirstIndexed(",", TokenType.Comma)) return;
                if (FirstIndexed("(", TokenType.OpenParen)) return;
                if (FirstIndexed(")", TokenType.CloseParen)) return;
                if (FirstIndexed("{", TokenType.OpenCurly)) return;
                if (FirstIndexed("}", TokenType.CloseCurly)) return;
                if (FirstIndexed("[", TokenType.OpenBracket)) return;
                if (FirstIndexed("]", TokenType.CloseBracket)) return;
                if (FirstIndexed("e", TokenType.LineEnd)) return;
                if (code[0] == '"') {
                    int i = 1;
                    while (code[i] != '"') i++;
                    tokens.Add(new Token(code.Substring(1, i-1), TokenType.StringLiteral));
                    code = code.Substring(i+1);
                    return;
                }
                if (IsLetter(code[0])) {
                    int i = 1;
                    while (IsLetter(code[i]) || char.IsDigit(code[i])) i++;
                    tokens.Add(new Token(code.Substring(0, i), TokenType.Identifier));
                    code = code.Substring(i);
                    return;
                }
                if (char.IsDigit(code[0]) || code[0] == '-') {
                    int i = 1;
                    while (char.IsDigit(code[i]) || code[i] == '.' && char.IsDigit(code[i+1]) && !code.Substring(i).Contains(".")) i++;
                    tokens.Add(new Token(code.Substring(0, i), TokenType.NumLiteral));
                    code = code.Substring(i);
                    return;
                }
                code = code.Substring(1);
            }

            bool IsLetter(char c) {
                return char.IsLetter(c) && c != 'e';
            }

            bool FirstIndexed(string str, TokenType tokenType) {
                if (code.IndexOf(str) == 0) {
                    tokens.Add(new Token(str, tokenType));
                    code = code.Substring(str.Length);
                    return true;
                }
                return false;
            }

            string ToLowerCase(string str) {
                string o = "";
                while (str.Contains("\"")) {
                    o += str.Substring(0, str.IndexOf("\"")+1).ToLower();
                    str = str.Substring(str.IndexOf("\"")+1);
                    o += str.Substring(0, str.IndexOf("\"")+1);
                    str = str.Substring(str.IndexOf("\"")+1);
                }
                o += str.ToLower();
                return o;
            }
        }
    }
}