using System.Data;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Cryptography;

namespace ClassLibrary;
public abstract class Function : Expression
{
    public override ExpressionKind Kind { get; set; }
    public Function() : base(null!)
    {
        Kind = ExpressionKind.Temp;
    }
}

public class FunctionBody
{
    public List<string>? ArgsVars = new List<string>();
    public List<Token>? Tokens = new List<Token>();

    public FunctionBody() { }
}

public class FunctionCall : Function
{
    public override ExpressionKind Kind { get; set; }
    public override object? Value { get; set; }
    public string Name;
    public List<Expression> ArgsValues;
    public Scope GlobalScope;

    public FunctionCall(string funcId, List<Expression> args, Scope globalScope)
    {
        Name = funcId;
        ArgsValues = args;
        GlobalScope = globalScope;
    }

    public void CheckSemantic()
    {
        if(!GlobalScope.Functions.ContainsKey(Name))
        {
            Console.WriteLine($"!semantic error: function \"{Name}\" does not exists.");
            throw new Exception();
        }
    }

    public void CheckArgsCount(Scope scope)
    {
        if (ArgsValues.Count != scope.Functions[Name].ArgsVars!.Count)
        {
            Console.WriteLine($"!semantic error: function \"{Name}\" recieves {scope.Functions[Name].ArgsVars!.Count} argument(s) but {ArgsValues.Count} were given.");
            throw new Exception();
        }
    }

    public override void Evaluate(Scope scope)
    {
        Scope child = scope.MakeChild();
        child.Functions = scope.Functions;

        for (int i = 0; i < ArgsValues.Count; i++)
            child.Vars.Add(GlobalScope.Functions[Name].ArgsVars![i], ArgsValues[i]);

        foreach (var arg in child.Vars)
            child.Vars[arg.Key].Evaluate(scope);

        ASTBuilder builder = new ASTBuilder(GlobalScope.Functions[Name].Tokens!, child);
        Expression ast = builder.Build();
        ast.Evaluate(child);
        Value = ast.Value;
    }


    public override object? GetValue() => Value;
}