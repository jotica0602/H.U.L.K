using System.Diagnostics;
using System.Formats.Asn1;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
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
        Scope GlobalScope = new Scope();
        SetUpGlobalScope(GlobalScope);
        while (true)
        {

            string[] strings =
            { 
                
                "function fib(n) => if (n > 1) fib(n-1) + fib(n-2) else 1;",
                "let x = 3 in print(fib(2));"
        
            };

            var Lexer = new Lexer(strings[i]);

            if (strings[i] == string.Empty)
            {
                Console.WriteLine("Empty Entry");
                throw new Exception();
            }

            List<Token> tokens = Lexer.Tokenize();

            ASTBuilder AST = new ASTBuilder(tokens, GlobalScope);
            Stopwatch crono = new Stopwatch();
            crono.Start();
            Expression Tree = AST.Build();

            if (Tree is not null)
            {
                Tree.Evaluate(GlobalScope);
                Console.WriteLine(Tree.Value);
            }

            crono.Stop();
            Console.WriteLine($"Time elapsed {crono.Elapsed}");
            i++;
            if (i == strings.Length) { break; }

        }
    }


    public static void Run()
    {
        Scope GlobalScope = new Scope();
        SetUpGlobalScope(GlobalScope);
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

                ASTBuilder builder = new ASTBuilder(tokens, GlobalScope);
                Stopwatch crono = new Stopwatch();

                Expression AST = builder.Build();

                if (AST is not null)
                {
                    AST.Evaluate(GlobalScope);
                    Console.WriteLine(AST.Value);
                }
            }

            catch (Exception)
            {
                Console.WriteLine("error");
                continue;
            }
        }
    }

    private static void SetUpGlobalScope(Scope GlobalScope)
    {
        GlobalScope.Vars.Add("E", new Number(Math.E));
        GlobalScope.Vars.Add("PI", new Number(Math.PI));
        GlobalScope.Vars.Add("Tau", new Number(Math.Tau));
    }

    delegate void Evaluate();

    private static void Welcome()
    {
        Console.WriteLine(@" 
                                 __  __       __  __       __         __  __                                   
                                /\ \/\ \     /\ \/\ \     /\ \       /\ \/\ \                                  
                                \ \ \_\ \    \ \ \ \ \    \ \ \      \ \ \/'/'                                 
                                 \ \  _  \    \ \ \ \ \    \ \ \  __  \ \ , <                                  
                                  \ \ \ \ \  __\ \ \_\ \  __\ \ \_\ \__\ \ \\`\                                
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