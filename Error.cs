class Error {
    public static void ParsingError(string message, Token token) {
        Console.WriteLine($"(At Line {token.line})Parsing Error: {message}");
        Console.WriteLine();
        Environment.Exit(1);
    }
}