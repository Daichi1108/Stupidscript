class Parser {

    List<Stmt> ast = new();
    List<Token> tokens = new();

    public List<Stmt> BuildAST(string input) {
        Lexer lexer = new();
        tokens = lexer.Tokenize(input);
        ast = new();
        while (At().type != TokenType.EOF) {
            ast.Add(ParseStmt());
        }

        return ast;
    }

    Token Eat() {
        Token temp = tokens[0];
        tokens.RemoveAt(0);
        return temp;
    }
    Token At() {
        return tokens[0];
    }
    Token Expect(TokenType tokenType, string err) {
        Token temp = tokens[0];
        tokens.RemoveAt(0);
        if (temp.type != tokenType) {
            throw new Exception(err);
        }
        return temp;
    }


    Stmt ParseStmt() {
        return ParseForStmt();
    }

    Stmt ParseForStmt() {
        if (At().type == TokenType.For) {
            Eat();
            List<Stmt> loopStmt = new();
            while (At().type != TokenType.OpenCurly) {
                loopStmt.Add(ParseStmt());
                if (At().type != TokenType.OpenCurly) {
                    Expect(TokenType.Comma, "comma pls");
                }
            }
            Eat();
            List<Stmt> forAst = new();
            while (At().type != TokenType.CloseCurly) {
                forAst.Add(ParseStmt());
            }
            Eat();
            return new ForStmt() { loopStmt=loopStmt, ast=forAst };
        }
        return ParseWhileStmt();
    }

    Stmt ParseWhileStmt() {
        if (At().type == TokenType.While) {
            Eat();
            Expr booleanVal = ParseExpr();
            Expect(TokenType.OpenCurly, "no open curly bracket breh");
            List<Stmt> whileAst = new();
            while (At().type != TokenType.CloseCurly) {
                whileAst.Add(ParseStmt());
            }
            Eat();
            return new WhileStmt() { boolean=booleanVal, ast=whileAst };
        }
        return ParseIfStmt();
    }

    Stmt ParseIfStmt() {
        if (At().type == TokenType.If) {
            Eat();
            Expr booleanVal = ParseExpr();
            Expect(TokenType.OpenCurly, "no open curly bracket breh");
            List<Stmt> ifAst = new();
            while (At().type != TokenType.CloseCurly) {
                ifAst.Add(ParseStmt());
            }
            Eat();
            return new IfStmt() { boolean=booleanVal, ast=ifAst };
        }
        return ParseVarDeclaration();
    }

    Stmt ParseVarDeclaration() {
        if (At().type == TokenType.Var) {
            Eat();
            string identifier = Expect(TokenType.Identifier, "I need like a varibale name breh").value;
            if (At().type == TokenType.Equals) {
                Eat();
                Expr value = ParseExpr();
                return new VarDeclaration() { identifier = identifier, value = value};
            }
            return new VarDeclaration() { identifier = identifier};
        }
        return ParseExpr();
    }

    Expr ParseExpr() {
        return ParseVarAssignment();
    }

    Expr ParseVarAssignment() {
        Expr left = ParseEqualityExpr();
        if (At().type == TokenType.Equals || At().type == TokenType.OperatorEquals || At().type == TokenType.IncrementOperation) {
            if (At().type == TokenType.Equals) {
                Eat();
                Expr value = ParseExpr();
                return new VarAssignment() { identifier=((Identifier)left).symbol, value=value };
            }
            if (At().type == TokenType.OperatorEquals) {
                string operation = Eat().value.Substring(0, 1);
                Expr value = ParseExpr();
                return new VarAssignment() { 
                    identifier=((Identifier)left).symbol, 
                    value=new BinaryExpr() { left=left, operation=operation, right=value } 
                };
            }
            if (At().type == TokenType.IncrementOperation) {
                string operation = Eat().value.Substring(0, 1);
                return new VarAssignment() {
                    identifier=((Identifier)left).symbol,
                    value=new BinaryExpr() { left=left, operation=operation, right=new NumLiteral() { value=1 }}
                };
            }
        }
        return left;
    }

    Expr ParseEqualityExpr() {
        Expr left = ParseRelationalExpr();
        while (At().value == "==" || At().value == "!=")  {
            string operation = Eat().value;
            Expr right = ParseRelationalExpr();
            left = new ConditionalExpr() {left = left, right = right, operation = operation};
        }
        return left;
    }

    Expr ParseRelationalExpr() {
        Expr left = ParseAdditiveExpr();
        while (At().value == ">" || At().value == "<" || At().value == ">=" || At().value == "<=")  {
            string operation = Eat().value;
            Expr right = ParseAdditiveExpr();
            left = new ConditionalExpr() {left = left, right = right, operation = operation};
        }
        return left;
    }

    Expr ParseAdditiveExpr() {
        Expr left = ParseMultiplicitiveExpr();
        while (At().value == "+" || At().value == "-")  {
            string operation = Eat().value;
            Expr right = ParseMultiplicitiveExpr();
            left = new BinaryExpr() {left = left, right = right, operation = operation};
        }
        return left;
    }

    Expr ParseMultiplicitiveExpr() {
        Expr left = ParseObjectExpr();
        while (At().value == "/" || At().value == "*" || At().value == "%")  {
            string operation = Eat().value;
            Expr right = ParseObjectExpr();
            left = new BinaryExpr() {left = left, right = right, operation = operation};
        }
        return left;
    }

    Expr ParseObjectExpr() {
        if (At().type == TokenType.OpenCurly) {
            Eat();
            ObjectLiteral obj = new();
            while (At().type != TokenType.CloseCurly) {
                string identifier = Expect(TokenType.Identifier, "I need an identifier dipshit").value;
                Expect(TokenType.Colon, "Colon required in object creation, why? idk");
                Expr value = ParseExpr();
                obj.properties.Add(identifier, value);
                if (At().type != TokenType.CloseCurly) {
                    Expect(TokenType.Comma, "comma wheresa???");
                }
            }
            Eat();
            return obj;
        }
        return ParseFuncDeclaration();
    }

    Expr ParseFuncDeclaration() {
        if (At().type == TokenType.FuncDeclaration) {
            Eat();
            List<string> parameters = new();
            List<string> returns = new();
            List<Stmt> body = new();
            while (At().type != TokenType.OpenCurly && At().type != TokenType.Arrow) {
                parameters.Add(Expect(TokenType.Identifier, "i need identifiers for return thingies").value);
                if (At().type != TokenType.OpenCurly && At().type != TokenType.Arrow) {
                    Expect(TokenType.Comma, "commmmmmaaaaaa");
                }
            }
            if (At().type == TokenType.Arrow) Eat();
            while (At().type != TokenType.OpenCurly) {
                returns.Add(Expect(TokenType.Identifier, "Expects identifiers for args").value);
                if (At().type != TokenType.OpenCurly) {
                    Expect(TokenType.Comma, "comma where????");
                }
            }
            Eat();
            while (At().type != TokenType.CloseCurly) {
                body.Add(ParseStmt());
            }
            Eat();
            return new FunctionDeclaration() { parameters=parameters, returns=returns, body=body };
        }
        return ParseMemberCallExpr();
    }

    Expr ParseMemberCallExpr() {
        Expr left = ParsePrimaryExpr();
        while (At().type == TokenType.Dot || At().type == TokenType.OpenParen || At().type == TokenType.OpenBracket) {
            if (At().type == TokenType.Dot) {
                Eat();
                string right = Expect(TokenType.Identifier, "right hand of member thingy needs an identifier").value;
                left = new MemberExpr() { left=left, right=right };
            }
            if (At().type == TokenType.OpenParen) {
                Eat();
                List<Expr> args = new();
                while (At().type != TokenType.CloseParen) {
                    args.Add(ParseExpr());
                    if (At().type != TokenType.CloseParen) {
                        Expect(TokenType.Comma, "need a commaaaaaaaa");
                    }
                }
                Eat();
                left = new CallExpr() { func=left, args=args };
            }
            if (At().type == TokenType.OpenBracket) {
                Eat();
                Expr right = ParseExpr();
                Expect(TokenType.CloseBracket, "wtf where is my closing bracket");
                left = new AccessExpr() { left=left, right=right };
            }
        }
        return left;
    }

    Expr ParsePrimaryExpr() {
        switch (At().type) {
            case TokenType.Identifier: return new Identifier() { symbol = Eat().value};
            case TokenType.NumLiteral: return new NumLiteral() { value = double.Parse(Eat().value) };
            case TokenType.StringLiteral: return new StringLiteral() { value = Eat().value };
            case TokenType.OpenParen:
                Eat();
                Expr expr = ParseExpr();
                Expect(TokenType.CloseParen, "I need some close parenthasese thank you idiot");
                return expr;
            case TokenType.OpenBracket:
                Eat();
                List<Expr> values = new();
                while (At().type != TokenType.CloseBracket) {
                    values.Add(ParseExpr());
                    if (At().type != TokenType.CloseBracket) {
                        Expect(TokenType.Comma, "COMMA COMMA COMMA COMMA");
                    }
                }
                Eat();
                return new ListLiteral() { values=values };
            case TokenType.BinaryOperator:
                if (At().value == "-") {
                    Eat();
                    return new BinaryExpr() {
                        left=new NumLiteral() { value=-1 },
                        operation="*",
                        right=ParsePrimaryExpr()
                    };
                }
                throw new Exception(At().value + " ya what is this doing here");
            default:
                throw new Exception($"wtf is that: {At().value}");
        }
    }
}