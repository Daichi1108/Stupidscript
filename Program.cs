//foreach(Token token in tokens) Console.WriteLine(token);
class Program {
    static void Main(string[] args)
    {
        Env env = Env.DeclareGlobalEnv();
        string script = @"C:\Users\daich\Documents\GitHub\Stupidscript\CodeInput.txt";
        Console.WriteLine();
        Interpreter.Interpret(File.ReadAllText(script), env);
        Console.WriteLine();
    }
}