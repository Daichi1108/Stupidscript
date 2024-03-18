using System.Reflection.Metadata.Ecma335;

class Stmt {
    public virtual RuntimeVal Eval(Env env) {
        throw new Exception("tbh this is prolly my fault mb mb");
    }
}
class Expr : Stmt {}

//STMT
class ForStmt : Stmt {
    public List<Stmt> loopStmt = new();
    public List<Stmt> ast = new();

    public override RuntimeVal Eval(Env env)
    {
        if (loopStmt.Count != 3) throw new Exception("this loop is wrong wtf are you doing");
        Env forEnv = new() { parent=env };
        loopStmt[0].Eval(forEnv);
        while (((BoolVal)loopStmt[1].Eval(forEnv)).value) {
            Env newEnv = new() { parent=forEnv };
            foreach (Stmt stmt in ast) {
                stmt.Eval(newEnv);
            }
            loopStmt[2].Eval(forEnv);
        }
        return new NullVal();
    }
}

class WhileStmt : Stmt {
    public Expr boolean = new();
    public List<Stmt> ast = new();

    public override RuntimeVal Eval(Env env)
    {
        while (((BoolVal)boolean.Eval(env)).value) {
            Env newEnv = new() { parent=env };
            foreach (Stmt stmt in ast) {
                stmt.Eval(newEnv);
            }
        }
        return new NullVal();
    }
}

class IfStmt : Stmt {
    public Expr boolean = new();
    public List<Stmt> ast = new();

    public override RuntimeVal Eval(Env env)
    {
        if (((BoolVal)boolean.Eval(env)).value) {
            Env newEnv = new() { parent=env };
            foreach (Stmt stmt in ast) {
                stmt.Eval(newEnv);
            }
        }
        return new NullVal();
    }
}

class VarDeclaration : Stmt {
    public string identifier = "";
    public Expr? value;

    public override RuntimeVal Eval(Env env)
    {
        return env.DeclareVar(identifier, value == null ? new NullVal() : value.Eval(env));
    }
}

//EXPR
class VarAssignment : Expr {
    public string identifier = "";
    public Expr value = new();

    public override RuntimeVal Eval(Env env)
    {
        return env.AssignVal(identifier, value.Eval(env));
    }
}

class ConditionalExpr : Expr {
    public Expr left = new();
    public Expr right = new();
    public string operation = "";

    public override RuntimeVal Eval(Env env)
    {
        RuntimeVal leftVal = left.Eval(env);
        RuntimeVal rightVal = right.Eval(env);
        if (leftVal.GetType() != rightVal.GetType()) {
            throw new Exception($"{leftVal.GetType()} and {rightVal.GetType()} cannot be compared because they are different");
        }
        if (leftVal is NumVal) {
            switch (operation) {
                case ">": return new BoolVal() { value=((NumVal)leftVal).value > ((NumVal)rightVal).value };
                case "<": return new BoolVal() { value=((NumVal)leftVal).value < ((NumVal)rightVal).value };
                case ">=": return new BoolVal() { value=((NumVal)leftVal).value >= ((NumVal)rightVal).value };
                case "<=": return new BoolVal() { value=((NumVal)leftVal).value <= ((NumVal)rightVal).value };
                case "==": return new BoolVal() { value=((NumVal)leftVal).value == ((NumVal)rightVal).value };
                case "!=": return new BoolVal() { value=((NumVal)leftVal).value != ((NumVal)rightVal).value };
            }
        }
        if (leftVal is StringVal) {
            switch (operation) {
                case ">": return new BoolVal() { value=((StringVal)leftVal).value.Length > ((StringVal)rightVal).value.Length };
                case "<": return new BoolVal() { value=((StringVal)leftVal).value.Length < ((StringVal)rightVal).value.Length };
                case ">=": return new BoolVal() { value=((StringVal)leftVal).value.Length >= ((StringVal)rightVal).value.Length };
                case "<=": return new BoolVal() { value=((StringVal)leftVal).value.Length <= ((StringVal)rightVal).value.Length };
                case "==": return new BoolVal() { value=((StringVal)leftVal).value == ((StringVal)rightVal).value };
                case "!=": return new BoolVal() { value=((StringVal)leftVal).value != ((StringVal)rightVal).value };
            }
        }
        if (leftVal is BoolVal) {
            leftVal = new NumVal() { value=Convert.ToInt32(((BoolVal)leftVal).value) };
            rightVal = new NumVal() { value=Convert.ToInt32(((BoolVal)rightVal).value) };
            switch (operation) {
                case ">": return new BoolVal() { value=((NumVal)leftVal).value > ((NumVal)rightVal).value };
                case "<": return new BoolVal() { value=((NumVal)leftVal).value < ((NumVal)rightVal).value };
                case ">=": return new BoolVal() { value=((NumVal)leftVal).value >= ((NumVal)rightVal).value };
                case "<=": return new BoolVal() { value=((NumVal)leftVal).value <= ((NumVal)rightVal).value };
                case "==": return new BoolVal() { value=((NumVal)leftVal).value == ((NumVal)rightVal).value };
                case "!=": return new BoolVal() { value=((NumVal)leftVal).value != ((NumVal)rightVal).value };
            }
        }
        throw new Exception($"{leftVal.GetType()} is not supported for conditional operations");
    }
}

class BinaryExpr : Expr {
    public Expr left = new();
    public Expr right = new();
    public string operation = "";

    public override RuntimeVal Eval(Env env)
    {
        RuntimeVal leftVal = left.Eval(env);
        if (leftVal is BoolVal) {
            leftVal = new NumVal() { value=Convert.ToInt32(((BoolVal)leftVal).value) };
        }
        RuntimeVal rightVal = right.Eval(env);
        if (rightVal is BoolVal) {
            rightVal = new NumVal() { value=Convert.ToInt32(((BoolVal)rightVal).value) };
        }

        if (leftVal is StringVal && rightVal is StringVal) {
            switch (operation) {
                case "+": return new StringVal() { value=((StringVal)leftVal).value + ((StringVal)rightVal).value };
            }
        }
        if (leftVal is NumVal && rightVal is StringVal) {
            switch (operation) {
                case "*":
                    string value = "";
                    for (int i = 0; i < ((NumVal)leftVal).value; i++) {
                        value += ((StringVal)rightVal).value;
                    }
                    return new StringVal() { value=value };
                case "+": return new StringVal() { value=((NumVal)leftVal).value + ((StringVal)rightVal).value };
            }
        }
        if (leftVal is StringVal && rightVal is NumVal) {
            switch (operation) {
                case "*":
                    string value = "";
                    for (int i = 0; i < ((NumVal)rightVal).value; i++) {
                        value += ((StringVal)leftVal).value;
                    }
                    return new StringVal() { value=value };
                case "+": return new StringVal() { value=((StringVal)leftVal).value + ((NumVal)rightVal).value };
            }
        }
        if (rightVal is NumVal && leftVal is NumVal) {
            switch (operation) {
                case "*": return new NumVal() { value=((NumVal)leftVal).value * ((NumVal)rightVal).value };
                case "/": return new NumVal() { value=((NumVal)leftVal).value / ((NumVal)rightVal).value };
                case "%": return new NumVal() { value=((NumVal)leftVal).value % ((NumVal)rightVal).value };
                case "+": return new NumVal() { value=((NumVal)leftVal).value + ((NumVal)rightVal).value };
                case "-": return new NumVal() { value=((NumVal)leftVal).value - ((NumVal)rightVal).value };
            }
        }
        throw new Exception($"{leftVal.GetType()} {operation} {rightVal.GetType()} doesnt work at all stupid");
    }
}

class CallExpr : Expr {
    public Expr func = new();
    public List<Expr> args = new();

    public override RuntimeVal Eval(Env env)
    {
        RuntimeVal fn = func.Eval(env);
        if (fn is Function) {
            Function function = (Function)fn;
            Env fenv = new() { parent=env };
            for (int i = 0; i < function.parameters.Count; i++) {
                fenv.DeclareVar(function.parameters[i], args[i].Eval(fenv));
            }
            foreach (string r in function.returns) {
                fenv.DeclareVar(r, new NullVal());
            }
            foreach (Stmt stmt in function.body) {
                stmt.Eval(fenv);
            }
            Dictionary<string, RuntimeVal> returns = new();
            foreach (string r in function.returns) {
                returns.Add(r, fenv.GetVal(r));
            }
            return new Object() { properties=returns };
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
    public Expr left = new();
    public string right = "";

    public override RuntimeVal Eval(Env env)
    {
        return ((Object)left.Eval(env)).properties[right];
    }
}

class AccessExpr : Expr {
    public Expr left = new();
    public Expr right = new();

    public override RuntimeVal Eval(Env env)
    {
        RuntimeVal leftVal = left.Eval(env);
        RuntimeVal rightVal = right.Eval(env);

        if (leftVal is StringVal) {
            return new StringVal() { value=((StringVal)leftVal).value.Substring((int)((NumVal)rightVal).value,1) };
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

    public override RuntimeVal Eval(Env env)
    {
        return new NumVal() { value=value };
    }
}

class StringLiteral : Expr {
    public string value = "";

    public override RuntimeVal Eval(Env env)
    {
        return new StringVal() { value = value};
    }
}

class ObjectLiteral : Expr {
    public Dictionary<string, Expr> properties = new();

    public override RuntimeVal Eval(Env env)
    {
        Object obj = new();
        foreach (string key in properties.Keys) {
            obj.properties.Add(key, properties[key].Eval(env));
        }
        return obj;
    }
}

class ListLiteral : Expr {
    public List<Expr> values = new();

    public override RuntimeVal Eval(Env env)
    {
        List<RuntimeVal> listVals = new();
        foreach (Expr expr in values) {
            listVals.Add(expr.Eval(env));
        }
        return new ListVal() { values=listVals };
    }
}

class FunctionDeclaration : Expr {
    public List<string> parameters = new();
    public List<string> returns = new();
    public List<Stmt> body = new();

    public override RuntimeVal Eval(Env env)
    {
        return new Function() { parameters=parameters, returns=returns, body=body };
    }
}

class Identifier : Expr {
    public string symbol = "";

    public override RuntimeVal Eval(Env env)
    {
        return env.GetVal(symbol);
    }
}