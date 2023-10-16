namespace ClassLibrary;
public class Bool : AtomExpression
{
    public Bool(ExpressionKind kind, bool value) : base(kind)
    {
        Value = value;
    }

    public override ExpressionKind Kind { get => ExpressionKind.Bool; set { } }
    public override object? Value { get; set; }

    public override void CheckSemantic() { return; }

    public override object? Evaluate() => Value;

    public override object? GetValue() => Value;

    public override void VisitNode()
    {
        throw new NotImplementedException();
    }
}