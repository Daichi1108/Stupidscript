using System.ComponentModel.DataAnnotations;
using System.Windows.Markup;

class RuntimeVal {
    public Dictionary<string, RuntimeVal> properties = new();
}

class ReturnVal : RuntimeVal {}

class NullVal : RuntimeVal {
    public override string ToString()
    {
        return "null";
    }
}

class NumVal : RuntimeVal {
    public double value;

    public NumVal(double value) {
        this.value = value;
    }

    public override string ToString()
    {
        return value.ToString();
    }
}

class StringVal : RuntimeVal {
    public string value;

    public StringVal(string value) {
        this.value = value;

        RuntimeVal Len(List<RuntimeVal> args) {
            return new NumVal(value.Length);
        }
        properties.Add("len", new NativeFn(Len));

        RuntimeVal Substring(List<RuntimeVal> args) {
            return new StringVal(value.Substring((int)((NumVal)args[0]).value, (int)((NumVal)args[1]).value - (int)((NumVal)args[0]).value));
        }
        properties.Add("substring", new NativeFn(Substring));

        RuntimeVal ParseNum(List<RuntimeVal> args) {
            return new NumVal(int.Parse(value));
        }
        properties.Add("parsenum", new NativeFn(ParseNum));

        RuntimeVal IsNum(List<RuntimeVal> args) {
            int num;
            return new BoolVal(int.TryParse(value, out num));
        }
        properties.Add("isnum", new NativeFn(IsNum));
    }

    public override string ToString()
    {
        return value;
    }
}

class BoolVal : RuntimeVal {

    public bool value;

    public BoolVal(bool value) {
        this.value = value;
    }

    public override string ToString()
    {
        if (value) return "tru"; return "fals";
    }
}

class ListVal : RuntimeVal {
    public List<RuntimeVal> values;
    
    public ListVal(List<RuntimeVal> values) {
        this.values = values;

        RuntimeVal Add(List<RuntimeVal> args) {
            foreach (RuntimeVal val in args) {
                values.Add(val);
            }
            return new NullVal();
        }
        properties.Add("add", new NativeFn(Add));

        RuntimeVal Pop(List<RuntimeVal> args) {
            RuntimeVal temp;
            if (args.Count == 0) {
                temp = values[values.Count-1];
                values.RemoveAt(values.Count-1);
                return temp;
            }
            temp = values[(int)((NumVal)args[0]).value];
            values.RemoveAt((int)((NumVal)args[0]).value);
            return temp;
        }
        properties.Add("pop", new NativeFn(Pop));

        RuntimeVal Len(List<RuntimeVal> args) {
            return new NumVal(values.Count);
        }
        properties.Add("len", new NativeFn(Len));
    }

    public override string ToString()
    {
        if (values.Count == 0) return "[]";
        string str = "[";
        foreach (RuntimeVal val in values) {
            str += val + ", ";
        }
        return str.Substring(0,str.Length-2) + "]";
    }
}

class Object : RuntimeVal {
    public Object(Dictionary<string, RuntimeVal> properties) {
        this.properties = properties;
    }

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
    public List<string> parameters;
    public List<string> returns;
    public List<Stmt> body;
    public Env env;

    public Function(List<string> parameters, List<string> returns, List<Stmt> body, Env env) {
        this.parameters = parameters;
        this.returns = returns;
        this.body = body;
        this.env = env;
    }

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

delegate RuntimeVal Func(List<RuntimeVal> args);
class NativeFn : RuntimeVal {
    public Func func;

    public NativeFn(Func func) {
        this.func = func;
    }

    public override string ToString()
    {
        return "Fuck you";
    }
}