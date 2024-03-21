class Interpreter {
    public static void Interpret(string input, Env env) {
        Parser parser = new();
        List<Stmt> ast = parser.BuildAST(input);
        foreach (Stmt stmt in ast) {
            stmt.Eval(env);
        }
    }
}