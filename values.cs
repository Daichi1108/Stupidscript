class RuntimeVal {}

class NullVal : RuntimeVal {
    public override string ToString()
    {
        return "null";
    }
}

class NumVal : RuntimeVal {
    public double value = 0;

    public override string ToString()
    {
        return value.ToString();
    }
}

class StringVal : RuntimeVal {
    public string value = "";

    public override string ToString()
    {
        return value;
    }
}

class BoolVal : RuntimeVal {

    public bool value = true;

    public override string ToString()
    {
        if (value) return "tru"; return "fals";
    }
}

class Object : RuntimeVal {
    public Dictionary<string, RuntimeVal> properties = new();

    public override string ToString()
    {
        string str = "{";

        foreach (string key in properties.Keys) {
            str += key + ": " + properties[key] + ", ";
        }

        return str.Substring(0, str.Length-2) + "}";
    }
}

class Function : RuntimeVal {
    public List<string> parameters = new();
    public List<string> returns = new();
    public List<Stmt> body = new();

    public override string ToString()
    {
        string str = "Function: ";
        foreach (string s in parameters) {
            str += s + ", ";
        }
        str = str.Substring(0, str.Length-2) + " -> ";
        foreach (string s in returns) {
            str += s + ", ";
        }
        return str.Substring(0, str.Length-2);
    }
}

delegate Object Func(List<RuntimeVal> args);
class NativeFn : RuntimeVal {
    public required Func func;
}