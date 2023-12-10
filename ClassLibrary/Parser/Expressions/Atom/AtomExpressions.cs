namespace ClassLibrary;

public class Number : AtomExpression
{
    public Number(double value) : base(null!)
    {
        Value = value;
        Kind = ExpressionKind.Number;
    }

    public override ExpressionKind Kind { get; set; }

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

    public Variable(string name) : base(null!)
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
        // Console.WriteLine($"{Name}");

        //localScope.Vars[Name].Scope is null ? localScope : localScope.Vars[Name].Scope!
        // if the actual scope contains this variable name as a key
        // we need to get the evaluation of that key's value
        // so we give as a argument that key's value scope

        if (localScope!.Vars.ContainsKey(Name))
        {
            //super important !
            if (localScope.Vars[Name].Value is not null)
                Value = localScope.Vars[Name].Value;

            else
            {
                localScope.Vars[Name].Evaluate(localScope.Vars[Name].Scope!);
                Value = localScope.Vars[Name].Value;
            }
        }
        else
        {
            Evaluate(localScope!.Parent!);
        }
    }

    public override object? GetValue() => Value;
}
public class PrintNode : AtomExpression
{
    public override ExpressionKind Kind { get; set; }

    public override object? Value { get; set; }

    List<Expression> Arguments;

    public PrintNode(List<Expression> Arguments, Scope scope) : base(scope)
    {
        this.Arguments = Arguments;
        Kind = ExpressionKind.Temp;
        if (Arguments.Count != 1)
        {
            Console.WriteLine($"!semantic error: function \"print\" recieves 1 argument(s), but {Arguments.Count} were given.");
            throw new Exception();
        }
    }

    public override void Evaluate(Scope scope)
    {
        Arguments[0].Evaluate(scope);
        Console.WriteLine(Arguments[0].Value);
        Value = Arguments[0].Value;
        if (Value is string) Kind = ExpressionKind.String;
        if (Value is double) Kind = ExpressionKind.Number;
        if (Value is bool) Kind = ExpressionKind.Bool;
    }

    public override object? GetValue() => Value;
}

public class SinNode : AtomExpression
{
    public override ExpressionKind Kind { get; set; }

    public override object? Value { get; set; }

    List<Expression> Arguments;

    public SinNode(List<Expression> Arguments, Scope scope) : base(scope)
    {
        this.Arguments = Arguments;
        Kind = ExpressionKind.Number;
        if (Arguments.Count != 1)
        {
            Console.WriteLine($"!semantic error: function \"sin\" recieves 1 argument(s), but {Arguments.Count} were given.");
            throw new Exception();
        }
    }

    public override void Evaluate(Scope scope)
    {
        Arguments[0].Evaluate(scope);
        Value = Math.Sin((double)Arguments[0].Value! * Math.PI / 180);
    }

    public override object? GetValue() => Value;

}
public class CosNode : AtomExpression
{
    public override ExpressionKind Kind { get; set; }

    public override object? Value { get; set; }

    List<Expression> Arguments;

    public CosNode(List<Expression> Arguments, Scope scope) : base(scope)
    {
        this.Arguments = Arguments;
        Kind = ExpressionKind.Number;
        if (Arguments.Count != 1)
        {
            Console.WriteLine($"!semantic error: function \"cos\" recieves 1 argument(s), but {Arguments.Count} were given.");
            throw new Exception();
        }
    }

    public override void Evaluate(Scope scope)
    {
        Arguments[0].Evaluate(scope);
        Value = Math.Cos((double)Arguments[0].Value! * Math.PI / 180);
    }

    public override object? GetValue() => Value;
}
public class ExpNode : AtomExpression
{
    public override ExpressionKind Kind { get; set; }

    public override object? Value { get; set; }

    List<Expression> Arguments;

    public ExpNode(List<Expression> Arguments, Scope scope) : base(scope)
    {
        this.Arguments = Arguments;
        Kind = ExpressionKind.Number;
        if (Arguments.Count != 1)
        {
            Console.WriteLine($"!semantic error: function \"exp\" recieves 1 argument(s), but {Arguments.Count} were given.");
            throw new Exception();
        }
    }

    public override void Evaluate(Scope scope)
    {
        Arguments[0].Evaluate(scope);
        Value = Math.Pow(Math.E, (double)Arguments[0].Value!);
    }

    public override object? GetValue() => Value;

}
public class SqrtNode : AtomExpression
{
    public override ExpressionKind Kind { get; set; }

    public override object? Value { get; set; }

    List<Expression> Arguments;

    public SqrtNode(List<Expression> Arguments, Scope scope) : base(scope)
    {
        this.Arguments = Arguments;
        Kind = ExpressionKind.Number;
        if (Arguments.Count != 1)
        {
            Console.WriteLine($"!semantic error: function \"sqrt\" recieves 1 argument(s), but {Arguments.Count} were given.");
            throw new Exception();
        }
    }

    public override void Evaluate(Scope scope)
    {
        Arguments[0].Evaluate(scope);
        Value = Math.Sqrt((double)Arguments[0].Value!);
    }

    public override object? GetValue() => Value;

}

public class LogNode : AtomExpression
{
    public override ExpressionKind Kind { get; set; }

    public override object? Value { get; set; }

    List<Expression> Arguments;

    public LogNode(List<Expression> arguments, Scope scope) : base(scope)
    {
        Arguments = arguments;
        Kind = ExpressionKind.Number;
        if (Arguments.Count != 2)
        {
            Console.WriteLine($"!semantic error: function \"log\" recieves 1 argument(s), but {Arguments.Count} were given.");
            throw new Exception();
        }
    }

    public override void Evaluate(Scope scope)
    {
        Arguments[0].Evaluate(scope);
        Arguments[1].Evaluate(scope);
        Value = Math.Log((double)Arguments[1].Value!, (double)Arguments[0].Value!);
    }

    public override object? GetValue() => Value;
}