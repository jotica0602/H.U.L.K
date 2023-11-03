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
        // "(let a = 1 in a) + (let b = 2 in b) > 30 | if(let c = (let x = 5*6 in x) in c == 30) 1 else 0;"
        int i = 0;
        Scope GlobalScope = new Scope();
        SetUpGlobalScope(GlobalScope);
        while (true)
        {

            string[] strings = { "function f(x)=> if(x>1) f(x-1) + f(x-2) else 1;","f(5);" };

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

            if (!(Tree is null))
            {
                Tree.Evaluate(GlobalScope);
                Console.WriteLine(Tree.GetValue());
            }

            crono.Stop();
            Console.WriteLine(crono.Elapsed);
            i++;
            if (i >= strings.Length) { break; }

        }
    }


    public static void Run()
    {
        Scope GlobalScope = new Scope();
        SetUpGlobalScope(GlobalScope);
        // Console.Clear();
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

                ASTBuilder ast = new ASTBuilder(tokens, GlobalScope);
                Stopwatch crono = new Stopwatch();
                crono.Start();
                Expression Tree = ast.Build();
                if (!(Tree is null))
                {
                    Tree.Evaluate(GlobalScope);
                    Console.WriteLine(Tree.GetValue());
                }
                crono.Stop();
                Console.WriteLine(crono.Elapsed);
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
        GlobalScope.Vars.Add("Pi", new Number(Math.PI));
        GlobalScope.Vars.Add("Tau", new Number(Math.Tau));
        // GlobalScope.Functions.Add("print",new Function(ExpressionKind.Temp))
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


