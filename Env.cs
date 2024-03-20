class Env {
    public Env? parent;
    public Dictionary<string, RuntimeVal> vars = new();

    public static Env DeclareGlobalEnv() {
        Env env = new();

        env.DeclareVar("true", new BoolVal(true));
        env.DeclareVar("false", new BoolVal(false));
        env.DeclareVar("null", new NullVal());

        static RuntimeVal Print(List<RuntimeVal> args) {
            foreach (RuntimeVal val in args) {
                Console.Write(val);
            }
            return new NullVal();
        }
        env.DeclareVar("print", new NativeFn(Print));

        static RuntimeVal PrintLn(List<RuntimeVal> args) {
            foreach (RuntimeVal val in args) {
                Console.WriteLine(val);
            }
            return new NullVal();
        }
        env.DeclareVar("println", new NativeFn(PrintLn));

        return env;
    }

    public RuntimeVal DeclareVar(string name, RuntimeVal value) {
        if (vars.ContainsKey(name)) {
            throw new Exception($"My mans, {name} is already like a variable stop tryna trick me");
        }
        vars.Add(name, value);
        return value;
    }

    public RuntimeVal GetVal(string name) {
        if (vars.ContainsKey(name)) {
            return vars[name];
        }
        if (parent != null) {
            return parent.GetVal(name);
        }
        throw new Exception($"My guy, {name} does just not exist idk what kind of schitzophrenia you got");
    }

    public RuntimeVal AssignVal(string name, RuntimeVal value) {
        if (vars.ContainsKey(name)) {
            vars[name] = value;
            return value;
        }
        if (parent != null) {
            return parent.AssignVal(name, value);
        }
        throw new Exception($"My guy, {name} does just not exist idk what kind of schitzophrenia you got");
    }
}