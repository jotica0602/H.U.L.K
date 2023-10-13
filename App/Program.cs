using ClassLibrary;
namespace Interpreter
{
    class Interpreter
    {
        static void Main()
        {
            // Auto();
            Manual();
            static void Auto()
            {
                int i = 0;
                while (true)
                {
                    try
                    {
                        // Automatic tests
                        string[] strings = { "function fib(n) => if(n>=1) fib(n-1) + fib(n-2) else 1;","print(fib(5));" };
                        var Lexer = new Lexer(strings[i]);

                        if (Lexer.sourceCode == string.Empty)
                        {
                            Diagnostics.Errors.Add("Empty Entry");
                            throw new Exception();
                        }

                        List<Token> tokens = Lexer.Tokenize();
                        // Console.WriteLine(string.Join('\n',tokens));

                        Parser parser = new Parser(tokens, new List<Dictionary<string,object>>());
                        parser.Parse();
                        parser.ClearVariables();

                        i++;

                        if (i >= strings.Length)
                        {
                            break;
                        }
                    }

                    catch (Exception)
                    {
                        Console.WriteLine(Diagnostics.Errors[0]);
                        Diagnostics.Errors.Clear();
                        break;
                    }
                }
            }

            void Manual()
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


                        Parser parser = new Parser(tokens,new List<Dictionary<string,object>>());
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

}