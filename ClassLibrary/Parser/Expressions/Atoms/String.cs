namespace ClassLibrary;
public class String : AtomExpression
{
    public String(ExpressionKind kind, string value) : base(kind)
    {
        Value = value;
    }

    public override ExpressionKind Kind { get => ExpressionKind.String; set { } }
    public override object? Value { get; set; }

    public override void CheckSemantic() { return; }

    public override object? Evaluate() => Value;

    public override object? GetValue() => Value;

    public override void VisitNode()
    {
        throw new NotImplementedException();
    }
}