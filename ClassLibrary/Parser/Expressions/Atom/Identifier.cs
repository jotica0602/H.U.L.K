namespace ClassLibrary;
public class Identifier : AtomExpression
{
    Scope Scope;
    public string Name;

    public override object? Value { get; set; }
    public override ExpressionKind Kind { get => ExpressionKind.Identifier; set { } }

    public Identifier(ExpressionKind kind, string name, object? value, Scope scope) : base(kind)
    {
        Name = name;
        Value = value;
        Scope = scope;
    }

    public override void CheckSemantic()
    {
        for (int i = Scope.Vars.Count - 1; i > -1; i--)
        {
            if (Scope.Vars[i].ContainsKey((string)Name!))
            {
                return;
            }
        }

        Console.WriteLine($"!semantic error: identifier \"{Name}\" does not exists.");
        throw new Exception();
    }

    public override object? Evaluate(Scope scope)
    {
        Console.WriteLine($"{Name}");
        for (int i = scope.Vars.Count-1; i > -1; i--)
        {
            if (Scope.Vars[i].ContainsKey((string)Name!))
            {
                Value = Scope.Vars[i][Name].Evaluate(scope);
                Scope.Vars[i].Remove(Name);
                break;
            }
        }
        
        return Value;
    }

    public override object? GetValue() => Value;

    public override void VisitNode()
    {
        throw new NotImplementedException();
    }
}