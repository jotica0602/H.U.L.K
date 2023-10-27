namespace ClassLibrary;

public class Function : Expression
{
    public Function(ExpressionKind kind, Scope scope, string name, Expression body) : base(kind, scope)
    {
        Name = name;
        Body = body;
    }

    public string Name { get; set; }
    public Expression Body { get; set; }
    public override ExpressionKind Kind { get; set; }
    public override object? Value { get; set; }
    public override Scope? Scope { get; set; }

    public override void Evaluate(Scope scope)
    {
        Body = Scope!.Parent!.Functions[Name].Body;
        Body.Scope!.Parent = Scope!;
        Body.Evaluate(Scope);
        Value = Body.Value;
    }

    public override object? GetValue() => Value;

    public void CheckSemantic(Scope globalScope, string functionName)
    {
        if (!globalScope.Functions.ContainsKey(functionName))
        {
            Console.WriteLine($"!semantic error: function \"{functionName}\" does not exists");
            throw new Exception();
        }
    }
}