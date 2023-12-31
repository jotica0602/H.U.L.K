using System.Data;
using System.Net.Http.Headers;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
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
    public virtual List<string>? ArgsVars { get; set; }
    public List<Token>? Tokens = new List<Token>();

    public FunctionBody()
    {
        ArgsVars = new List<string>();
    }
}

public class FunctionCall : Function
{
    public override ExpressionKind Kind { get; set; }
    public override object? Value { get; set; }
    public string Name;
    public List<Expression> ArgsValues;

    int ArgsRealCount;

    public Scope GlobalScope;

    public FunctionCall(string funcId, List<Expression> args, Scope globalScope)
    {
        Name = funcId;
        ArgsValues = args;
        GlobalScope = globalScope;
    }

    public void CheckSemantic(Scope localScope)
    {
        if (!localScope.Functions.ContainsKey(Name))
        {
            CheckSemantic(localScope.Parent!);
        }
        else
        {
            ArgsRealCount = localScope.Functions[Name].ArgsVars!.Count;
            return;
        }
    }

    public void CheckArgsCount(Scope scope)
    {
        if (ArgsValues.Count != ArgsRealCount)
        {
            Console.WriteLine($"!semantic error: function \"{Name}\" recieves {scope.Functions[Name].ArgsVars!.Count} argument(s), but {ArgsValues.Count} were given.");
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

        if (ast.Value is string) this.Kind = ExpressionKind.String;
        if (ast.Value is double) this.Kind = ExpressionKind.Number;
        if (ast.Value is bool) this.Kind = ExpressionKind.Bool;

        Value = ast.Value;
    }

    public override object? GetValue() => Value;
}

