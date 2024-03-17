using System.Reflection;

namespace Stupidscript {
    class Env {
        public Env? parent;
        public Dictionary<string, RuntimeVal> vars = new();

        public static Env DeclareGlobalEnv() {
            Env env = new();

            env.DeclareVar("tru", new BoolVal() { value=true });
            env.DeclareVar("fals", new BoolVal() { value=false });
            env.DeclareVar("null", new NullVal());

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
}