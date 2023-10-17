namespace ClassLibrary;
public class Number : AtomExpression
{
    public Number(ExpressionKind kind, double value) : base(kind)
    {
        Value = value;
    }

    public override ExpressionKind Kind { get => ExpressionKind.Number; set { } }
    public override object? Value { get; set; }

    public override void CheckSemantic() { return; }

    public override object? Evaluate(Scope scope) => Value;

    public override object? GetValue() => Value;

    public override string ToString() => $"{Kind.GetType} : {Value}";

    public override void VisitNode()
    {
        throw new NotImplementedException();
    }
}