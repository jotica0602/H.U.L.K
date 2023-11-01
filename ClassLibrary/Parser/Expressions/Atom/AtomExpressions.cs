namespace ClassLibrary;

public class Number : AtomExpression
{
    public Number(double value) : base(null!)
    {
        Value = value;
        Kind = ExpressionKind.Number;
    }

    public override ExpressionKind Kind { get => ExpressionKind.Number; set { } }
    public override object? Value { get; set; }

    public override void Evaluate(Scope scope) { return; }

    public override object? GetValue() => Value;

    public override string ToString() => $"{Kind.GetType} : {Value}";
}

public class Bool : AtomExpression
{
    public Bool(bool value) : base(null!)
    {
        Value = value;
        Kind = ExpressionKind.Bool;
    }

    public override ExpressionKind Kind { get => ExpressionKind.Bool; set { } }
    public override object? Value { get; set; }

    public override void Evaluate(Scope scope) { return; }
    
    public override object? GetValue() => Value;
}

public class String : AtomExpression
{
    public String(string value) : base(null!)
    {
        Value = value;
        Kind = ExpressionKind.String;
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

    public Variable(string name, Scope scope) : base(scope)
    {
        Name = name;
        Kind = ExpressionKind.Temp;
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
        // if the actual scope contains this variable name as a key
        // we need to get the evaluation of that key's value
        // so we give as a argument that key's value scope
        if (localScope!.Vars.ContainsKey(Name))
        {                                       //super important !
            localScope.Vars[Name].Evaluate(localScope.Vars[Name].Scope!);
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