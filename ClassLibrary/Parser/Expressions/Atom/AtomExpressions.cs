namespace ClassLibrary;

public class Number : AtomExpression
{
    public Number(ExpressionKind kind, double value) : base(kind,null!)
    {
        Value = value;
    }

    public override ExpressionKind Kind { get => ExpressionKind.Number; set { } }
    public override object? Value { get; set; }

    public override void Evaluate(Scope scope) { return; }

    public override object? GetValue() => Value;

    public override string ToString() => $"{Kind.GetType} : {Value}";
}

public class Bool : AtomExpression
{
    public Bool(ExpressionKind kind, bool value, Scope scope) : base(kind, null!)
    {
        Value = value;
    }

    public override ExpressionKind Kind { get => ExpressionKind.Bool; set { } }
    public override object? Value { get; set; }

    public override void Evaluate(Scope scope) { return; }

    public override object? GetValue() => Value;
}

public class String : AtomExpression
{
    public String(ExpressionKind kind, string value, Scope scope) : base(kind, null!)
    {
        Value = value;
    }

    public override ExpressionKind Kind { get => ExpressionKind.String; set { } }
    public override object? Value { get; set; }

    public override void Evaluate(Scope scope) { return; }

    public override object? GetValue() => Value;
}

public class Variable : AtomExpression
{
    public override Scope? Scope { get; set; }
    public string Name;

    public override object? Value { get; set; }
    public override ExpressionKind Kind { get => ExpressionKind.Temp; set { } }

    public Variable(ExpressionKind kind, string name, Scope scope) : base(kind, scope)
    {
        Name = name;
    }

    public void CheckSemantic(Scope localScope)
    {
        if (localScope is null)
        {
            Console.WriteLine($"!semantic error: variable \"{Name}\" does not exists.");
            throw new Exception();
        }
        if (localScope.Vars.ContainsKey(Name))
        {
            return;
        }
        else
        {
            CheckSemantic(localScope.Parent!);
        }
    }


    public override void Evaluate(Scope localScope)
    {
        Console.WriteLine($"{Name}");

        if (localScope.Vars.ContainsKey(Name))
        {
            localScope.Vars[Name].Evaluate(localScope!);
            Value = localScope.Vars[Name].GetValue();
            return;

        }
        else
        {
            Evaluate(localScope!.Parent!);
        }
    }

    public override object? GetValue() => Value;
}