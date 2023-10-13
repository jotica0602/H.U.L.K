using ClassLibrary;
namespace Interpreter
{
    class Interpreter
    {
        static void Main()
        {
            Run();
        }

        public static void Run()
        {
            Console.Clear();
            while (true)
            {
                try
                {
                    Console.Write(">");
                    string sourceCode = Console.ReadLine()!;
                    var Lexer = new Lexer(sourceCode);


                    if (Lexer.sourceCode == string.Empty)
                    {
                        Diagnostics.Errors.Add("Empty Entry");
                        throw new Exception();
                    }

                    List<Token> tokens = Lexer.Tokenize();


                    Parser parser = new Parser(tokens, new List<Dictionary<string, object>>());
                    parser.Parse();
                    parser.ClearVariables();

                }
                catch (Exception)
                {
                    Console.WriteLine(Diagnostics.Errors[0]);
                    Diagnostics.Errors.Clear();
                    continue;
                }
            }
        }
    }
}


