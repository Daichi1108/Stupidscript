//foreach(Token token in tokens) Console.WriteLine(token);
namespace Stupidscript
{
    class Program {
        static void Main(string[] args)
        {
            Env env = Env.DeclareGlobalEnv();
            Interpreter interpreter = new();
            string script = @"C:\Users\daich\Documents\GitHub\Stupidscript\CodeInput.txt";
            Console.WriteLine();
            interpreter.Interpret(File.ReadAllText(script), env);
            Console.WriteLine();
        }
    }
}