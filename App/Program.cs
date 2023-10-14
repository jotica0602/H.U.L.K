using ClassLibrary;
namespace Interpreter
{
    class Interpreter
    {
        static void Main()
        {
            Run();
            // Auto();
        }

        public static void Auto()
        {
            int i = 0;
            while (true)
            {
                try
                {
                    // Automatic tests
                    string[] strings = {"function f(n)=>if(n<=1) 1 else n*f(n-1);","f(5);"};
                    var Lexer = new Lexer(strings[i]);

                    if (Lexer.sourceCode == string.Empty)
                    {
                        Diagnostics.Errors.Add("Empty Entry");
                        throw new Exception();
                    }

                    List<Token> tokens = Lexer.Tokenize();
                    // Console.WriteLine(string.Join('\n',tokens));

                    Parser parser = new Parser(tokens, new List<Dictionary<string, object>>());
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


        public static void Run()
        {

            Console.Clear();
            Welcome();
            Console.WriteLine("Code:");
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

        private static void Welcome()
        {
            Console.WriteLine(@" 
                                 __  __       __  __       __         __  __                                   
                                /\ \/\ \     /\ \/\ \     /\ \       /\ \/\ \                                  
                                \ \ \_\ \    \ \ \ \ \    \ \ \      \ \ \/'/'                                 
                                 \ \  _  \    \ \ \ \ \    \ \ \  __  \ \ , <                                  
                                  \ \ \ \ \  __\ \ \_\ \  __\ \ \L\ \__\ \ \\`\                                
                                   \ \_\ \_\/\_\\ \_____\/\_\\ \____/\_\\ \_\ \_\                              
                                    \/_/\/_/\/_/ \/_____/\/_/ \/___/\/_/ \/_/\/_/                              
                                                                               
                     ______          __                                       __
                    /\__  _\        /\ \__                                   /\ \__                
                    \/_/\ \/     ___\ \ ,_\    __   _ __   _____   _ __    __\ \ ,_\    __   _ __  
                       \ \ \   /' _ `\ \ \/  /'__`\/\`'__\/\ '__`\/\`'__\/'__`\ \ \/  /'__`\/\`'__\
                        \_\ \__/\ \/\ \ \ \_/\  __/\ \ \/ \ \ \L\ \ \ \//\  __/\ \ \_/\  __/\ \ \/ 
                        /\_____\ \_\ \_\ \__\ \____\\ \_\  \ \ ,__/\ \_\\ \____\\ \__\ \____\\ \_\ 
                        \/_____/\/_/\/_/\/__/\/____/ \/_/   \ \ \/  \/_/ \/____/ \/__/\/____/ \/_/ 
                                                             \ \_\                                 
                                                              \/_/
                                                            ");
        }
    }
}


