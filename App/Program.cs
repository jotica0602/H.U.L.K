using System.Diagnostics;
using System.Runtime.CompilerServices;
using ClassLibrary;
class Interpreter
{
    static void Main()
    {
        // Run();
        Auto();
    }


    public static void Auto()
    {
        int i = 0;
        while (true)
        {
            try
            {
                // Automatic tests
                // "let a = 42 in (let b = 4 in 4) + a;"
                // "let a = (let b = 42 in b) in b ;"
                string[] strings = { "let a = (let b = 2 in b) in a;" };
                var Lexer = new Lexer(strings[i]);

                if (strings[i] == string.Empty)
                {
                    Console.WriteLine("Empty Entry");
                    throw new Exception();
                }

                List<Token> tokens = Lexer.Tokenize();
                Scope scope = new Scope();
                ASTBuilder AST = new ASTBuilder(tokens, scope);
                Expression AST_ = AST.Build();
                Console.WriteLine(AST_.Evaluate(scope));
                i++;
                if (i >= strings.Length) { break; }
            }

            catch (Exception) { break; }
        }
    }


    public static void Run()
    {

        Console.Clear();
        Welcome();
        Console.WriteLine("Write your code below :) ");

        while (true)
        {
            try
            {
                Console.Write(">");
                string sourceCode = Console.ReadLine()!;
                if (sourceCode == string.Empty)
                    throw new Exception("Empty Entry");

                var Lexer = new Lexer(sourceCode);

                List<Token> tokens = Lexer.Tokenize();
                Scope scope = new Scope();
                ASTBuilder ast = new ASTBuilder(tokens, scope);
                ast.Build();
            }
            catch (Exception) { continue; }
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


