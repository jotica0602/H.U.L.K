using System.Diagnostics;
using System.Formats.Asn1;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using ClassLibrary;

class Interpreter
{
    static void Main()
    {
        SetUp();
        // Run();
        // Auto();
    }

    public static void SetUp()
    {
        Console.Clear();
        string text = "Do you want to:\n1)Write your code.\n2)Specify where your code is (only .txt).";
        Console.WriteLine(text);
        Console.Write(">");

        string answer = Console.ReadLine()!;
        while (answer != "1" && answer != "2")
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine("Not a valid answer.");
            Console.ResetColor();
            System.Console.WriteLine(text);
            Console.Write(">");
            answer = Console.ReadLine()!;
        }

        Console.Clear();
        switch (answer)
        {
            case "1":
                Run();
                break;
            case "2":
                LoadFile();
                break;
        }
    }

    public static void Auto()
    {
        int i = 0;
        Scope GlobalScope = new Scope();
        while (true)
        {
            try
            {

                string[] input =
                {
                    "print(\"Hello World\");",
                    "print((((1 + 2) ^ 3) * 4) / 5);",
                    "print(sin(2 * PI) ^ 2 + cos(3 * PI / log(4, 64)));",
                    "print(sin(PI));",
                    "function tan(x) => sin(x)/cos(x);",
                    "print(tan(PI/2));",
                    "let x = PI/2 in print(tan(x));",
                    "let number = 42, text = \"The meaning of life is \" in print(text @ number);",
                    "let number = 42 in (let text = \"The meaning of life is \" in (print(text @ number)));",
                    "print(7 + (let x = 2 in x * x));",
                    "let a = 42 in if (a % 2 == 0) print(\"Even\") else print(\"odd\");",
                    "let a = 42 in print(if (a % 2 == 0) \"even\" else \"odd\");",
                    "function fib(n) => if (n > 1) fib(n-1) + fib(n-2) else 1;",
                    "let x = 42 in print(x);",
                    "fib(5);",
                    "print(fib(6));",
                    "let 14a = 5 in print(14a);",
                    "let a = 5 in print(a;",
                    "let a = 5 inn print(a);",
                    "let a = in print(a);",
                    "let a = \"hello world\" in print(a + 5);",
                    "print(fib(\"hello world\"));",
                    "print(fib(4,3));"

                };

                var Lexer = new Lexer(input[i]);

                if (input[i] == string.Empty)
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
                    Console.WriteLine(Tree.GetValue());
                }

                crono.Stop();
                // Console.WriteLine($"Time elapsed {crono.Elapsed}");
                i++;
                if (i == input.Length) { break; }
            }
            catch (Exception)
            {
                i++;
                if (i == 22) break;
            }
        }

    }


    public static void Run()
    {
        Scope GlobalScope = new Scope();
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

    public static void LoadFile()
    {
        Console.WriteLine("Write your code path below:");
        Console.Write(">");
        string path = Console.ReadLine();
        var sourceCode = Path.Join(path);

        string[] inputs = File.ReadAllLines(sourceCode);
        Scope GlobalScope = new Scope();

        for (int i = 0; i < inputs.Length; i++)
        {
            try
            {
                Lexer lexer = new Lexer(inputs[i]);
                List<Token> tokens = lexer.Tokenize();
                ASTBuilder AST = new ASTBuilder(tokens, GlobalScope);
                Expression tree = AST.Build();
                if (tree is not null)
                {
                    tree.Evaluate(GlobalScope);
                    Console.WriteLine(tree.GetValue());
                }
            }
            catch (Exception)
            {
                continue;
            }
        }
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