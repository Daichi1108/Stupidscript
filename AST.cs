using System.Security.AccessControl;

class Stmt {
    public virtual RuntimeVal Eval(Env env) {
        throw new Exception("tbh this is prolly my fault mb mb");
    }
}
class Expr : Stmt {}

//STMT
class Return : Stmt {

    public override RuntimeVal Eval(Env env)
    {
        return new ReturnVal();
    }
}

class ForStmt : Stmt {
    public List<Stmt> loopStmt;
    public List<Stmt> ast;

    public ForStmt(List<Stmt> loopStmt, List<Stmt> ast) {
        this.loopStmt = loopStmt;
        this.ast = ast;
    }

    public override RuntimeVal Eval(Env env)
    {
        if (loopStmt.Count != 3) throw new Exception("this loop is wrong wtf are you doing");
        Env forEnv = new() { parent=env };
        loopStmt[0].Eval(forEnv);
        while (((BoolVal)loopStmt[1].Eval(forEnv)).value) {
            Env newEnv = new() { parent=forEnv };
            foreach (Stmt stmt in ast) {
                if (stmt.Eval(newEnv) is ReturnVal) return new ReturnVal();
            }
            loopStmt[2].Eval(forEnv);
        }
        return new NullVal();
    }
}

class WhileStmt : Stmt {
    public Expr boolean;
    public List<Stmt> ast;

    public WhileStmt(Expr boolean, List<Stmt> ast) {
        this.boolean = boolean;
        this.ast = ast;
    }

    public override RuntimeVal Eval(Env env)
    {
        while (((BoolVal)boolean.Eval(env)).value) {
            Env newEnv = new() { parent=env };
            foreach (Stmt stmt in ast) {
                if (stmt.Eval(newEnv) is ReturnVal) return new ReturnVal();
            }
        }
        return new NullVal();
    }
}

class IfStmt : Stmt {
    public Expr boolean;
    public List<Stmt> ast;

    public IfStmt(Expr boolean, List<Stmt> ast) {
        this.boolean = boolean;
        this.ast = ast;
    }

    public override RuntimeVal Eval(Env env)
    {
        if (((BoolVal)boolean.Eval(env)).value) {
            Env newEnv = new() { parent=env };
            foreach (Stmt stmt in ast) {
                if (stmt.Eval(newEnv) is ReturnVal) return new ReturnVal();
            }
        }
        return new NullVal();
    }
}

class VarDeclaration : Stmt {
    public string identifier;
    public Expr? value;

    public VarDeclaration(string identifier, Expr? value = null) {
        this.identifier = identifier;
        this.value = value;
    }

    public override RuntimeVal Eval(Env env)
    {
        return env.DeclareVar(identifier, value == null ? new NullVal() : value.Eval(env));
    }
}

//EXPR
class VarAssignment : Expr {
    public Expr var;
    public Expr value;

    public VarAssignment(Expr var, Expr value) {
        this.var = var;
        this.value = value;
    }

    public override RuntimeVal Eval(Env env)
    {
        if (var is Identifier) {
            return env.AssignVal(((Identifier)var).symbol , value.Eval(env));
        }
        if (var is MemberExpr) {
            return ((MemberExpr)var).left.Eval(env).properties[((MemberExpr)var).right] = value.Eval(env);
        }
        if (var is AccessExpr) {
            RuntimeVal left = ((AccessExpr)var).left.Eval(env);
            if (left is ListVal) {
                ((ListVal)left).values[(int)((NumVal)((AccessExpr)var).right.Eval(env)).value] = value.Eval(env);
            }
        }
        return new NullVal();
    }
}

class ConditionalExpr : Expr {
    public Expr left;
    public Expr right;
    public string operation;

    public ConditionalExpr(Expr left, string operation, Expr right) {
        this.left = left;
        this.operation = operation;
        this.right = right;
    }

    public override RuntimeVal Eval(Env env)
    {
        RuntimeVal leftVal = left.Eval(env);
        RuntimeVal rightVal = right.Eval(env);
        if (leftVal.GetType() != rightVal.GetType()) {
            throw new Exception($"{leftVal.GetType()} and {rightVal.GetType()} cannot be compared because they are different");
        }
        if (leftVal is NumVal) {
            switch (operation) {
                case ">": return new BoolVal(((NumVal)leftVal).value > ((NumVal)rightVal).value);
                case "<": return new BoolVal(((NumVal)leftVal).value < ((NumVal)rightVal).value);
                case ">=": return new BoolVal(((NumVal)leftVal).value >= ((NumVal)rightVal).value);
                case "<=": return new BoolVal(((NumVal)leftVal).value <= ((NumVal)rightVal).value);
                case "==": return new BoolVal(((NumVal)leftVal).value == ((NumVal)rightVal).value);
                case "!=": return new BoolVal(((NumVal)leftVal).value != ((NumVal)rightVal).value);
            }
        }
        if (leftVal is StringVal) {
            switch (operation) {
                case ">": return new BoolVal(((StringVal)leftVal).value.Length > ((StringVal)rightVal).value.Length);
                case "<": return new BoolVal(((StringVal)leftVal).value.Length < ((StringVal)rightVal).value.Length);
                case ">=": return new BoolVal(((StringVal)leftVal).value.Length >= ((StringVal)rightVal).value.Length);
                case "<=": return new BoolVal(((StringVal)leftVal).value.Length <= ((StringVal)rightVal).value.Length);
                case "==": return new BoolVal(((StringVal)leftVal).value.Length == ((StringVal)rightVal).value.Length);
                case "!=": return new BoolVal(((StringVal)leftVal).value.Length != ((StringVal)rightVal).value.Length);
            }
        }
        if (leftVal is BoolVal) {
            leftVal = new NumVal(Convert.ToInt32(((BoolVal)leftVal).value));
            rightVal = new NumVal(Convert.ToInt32(((BoolVal)rightVal).value));
            switch (operation) {
                case ">": return new BoolVal(((NumVal)leftVal).value > ((NumVal)rightVal).value);
                case "<": return new BoolVal(((NumVal)leftVal).value < ((NumVal)rightVal).value);
                case ">=": return new BoolVal(((NumVal)leftVal).value >= ((NumVal)rightVal).value);
                case "<=": return new BoolVal(((NumVal)leftVal).value <= ((NumVal)rightVal).value);
                case "==": return new BoolVal(((NumVal)leftVal).value == ((NumVal)rightVal).value);
                case "!=": return new BoolVal(((NumVal)leftVal).value != ((NumVal)rightVal).value);
            }
        }
        throw new Exception($"{leftVal.GetType()} is not supported for conditional operations");
    }
}

class BinaryExpr : Expr {
    public Expr left;
    public Expr right;
    public string operation;

    public BinaryExpr(Expr left, string operation, Expr right) {
        this.left = left;
        this.operation = operation;
        this.right = right;
    }

    public override RuntimeVal Eval(Env env)
    {
        RuntimeVal leftVal = left.Eval(env);
        if (leftVal is BoolVal) {
            leftVal = new NumVal(Convert.ToInt32(((BoolVal)leftVal).value));
        }
        RuntimeVal rightVal = right.Eval(env);
        if (rightVal is BoolVal) {
            rightVal = new NumVal(Convert.ToInt32(((BoolVal)rightVal).value));
        }

        if (leftVal is StringVal && rightVal is StringVal) {
            switch (operation) {
                case "+": return new StringVal(((StringVal)leftVal).value + ((StringVal)rightVal).value);
            }
        }
        if (leftVal is NumVal && rightVal is StringVal) {
            switch (operation) {
                case "*":
                    string value = "";
                    for (int i = 0; i < ((NumVal)leftVal).value; i++) {
                        value += ((StringVal)rightVal).value;
                    }
                    return new StringVal(value);
                case "+": return new StringVal(((NumVal)leftVal).value + ((StringVal)rightVal).value);
            }
        }
        if (leftVal is StringVal && rightVal is NumVal) {
            switch (operation) {
                case "*":
                    string value = "";
                    for (int i = 0; i < ((NumVal)rightVal).value; i++) {
                        value += ((StringVal)leftVal).value;
                    }
                    return new StringVal(value);
                case "+": return new StringVal(((StringVal)leftVal).value + ((NumVal)rightVal).value);
            }
        }
        if (rightVal is NumVal && leftVal is NumVal) {
            switch (operation) {
                case "*": return new NumVal(((NumVal)leftVal).value * ((NumVal)rightVal).value);
                case "/": return new NumVal(((NumVal)leftVal).value / ((NumVal)rightVal).value);
                case "%": return new NumVal(((NumVal)leftVal).value % ((NumVal)rightVal).value);
                case "+": return new NumVal(((NumVal)leftVal).value + ((NumVal)rightVal).value);
                case "-": return new NumVal(((NumVal)leftVal).value - ((NumVal)rightVal).value);
            }
        }
        throw new Exception($"{leftVal.GetType()} {operation} {rightVal.GetType()} doesnt work at all stupid");
    }
}

class CallExpr : Expr {
    public Expr func;
    public List<Expr> args;

    public CallExpr(Expr func, List<Expr> args) {
        this.func = func;
        this.args = args;
    }

    public override RuntimeVal Eval(Env env)
    {
        RuntimeVal fn = func.Eval(env);
        if (fn is Function) {
            Function function = (Function)fn;
            Env fenv = new Env() { parent=function.env };
            for (int i = 0; i < function.parameters.Count; i++) {
                fenv.DeclareVar(function.parameters[i], args[i].Eval(fenv));
            }
            foreach (string r in function.returns) {
                fenv.DeclareVar(r, new NullVal());
            }

            foreach (Stmt stmt in function.body) {
                if (stmt.Eval(fenv) is ReturnVal) break;
            }


            Dictionary<string, RuntimeVal> returns = new();
            foreach (string r in function.returns) {
                returns.Add(r, fenv.GetVal(r));
            }
            if (returns.Count == 0) {
                return new NullVal();
            }
            if (returns.Count == 1) {
                foreach (RuntimeVal val in returns.Values) {
                    return val;
                }
            }
            return new Object(returns);
        }
        if (fn is NativeFn) {
            List<RuntimeVal> argVals = new();
            foreach (Expr expr in args) {
                argVals.Add(expr.Eval(env));
            }
            return ((NativeFn)fn).func(argVals);
        }
        throw new Exception("ya no i dont feel like it");
    }
}

class MemberExpr : Expr {
    public Expr left;
    public string right;

    public MemberExpr(Expr left, string right) {
        this.left = left;
        this.right = right;
    }

    public override RuntimeVal Eval(Env env)
    {
        return left.Eval(env).properties[right];
    }
}

class AccessExpr : Expr {
    public Expr left;
    public Expr right;

    public AccessExpr(Expr left, Expr right) {
        this.left = left;
        this.right = right;
    }

    public override RuntimeVal Eval(Env env)
    {
        RuntimeVal leftVal = left.Eval(env);
        RuntimeVal rightVal = right.Eval(env);

        if (leftVal is StringVal) {
            return new StringVal(((StringVal)leftVal).value.Substring((int)((NumVal)rightVal).value,1));
        }
        if (leftVal is ListVal) {
            return ((ListVal)leftVal).values[(int)((NumVal)rightVal).value];
        }
        throw new Exception($"{leftVal.GetType()} cant be used for the uhh um whats it called like accessing");
    }
}

//LITERALS
class NumLiteral : Expr {
    public double value;

    public NumLiteral(double value) {
        this.value = value;
    }

    public override RuntimeVal Eval(Env env)
    {
        return new NumVal(value);
    }
}

class StringLiteral : Expr {
    public string value;

    public StringLiteral(string value) {
        this.value = value;
    }

    public override RuntimeVal Eval(Env env)
    {
        return new StringVal(value);
    }
}

class ObjectLiteral : Expr {
    public Dictionary<string, Expr> properties;

    public ObjectLiteral(Dictionary<string, Expr> properties) {
        this.properties = properties;
    }

    public override RuntimeVal Eval(Env env)
    {
        Object obj = new(new());
        foreach (string key in properties.Keys) {
            obj.properties.Add(key, properties[key].Eval(env));
        }
        return obj;
    }
}

class ListLiteral : Expr {
    public List<Expr> values;

    public ListLiteral(List<Expr> values) {
        this.values = values;
    }

    public override RuntimeVal Eval(Env env)
    {
        List<RuntimeVal> listVals = new();
        foreach (Expr expr in values) {
            listVals.Add(expr.Eval(env));
        }
        return new ListVal(listVals);
    }
}

class FunctionDeclaration : Expr {
    public List<string> parameters;
    public List<string> returns;
    public List<Stmt> body;

    public FunctionDeclaration(List<string> parameters, List<string> returns, List<Stmt> body) {
        this.parameters = parameters;
        this.returns = returns;
        this.body = body;
    }

    public override RuntimeVal Eval(Env env)
    {
        return new Function(parameters, returns, body, env);
    }
}

class Identifier : Expr {
    public string symbol;

    public Identifier(string symbol) {
        this.symbol = symbol;
    }

    public override RuntimeVal Eval(Env env)
    {
        return env.GetVal(symbol);
    }
}