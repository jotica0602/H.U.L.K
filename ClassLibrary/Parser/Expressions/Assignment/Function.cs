namespace ClassLibrary;

public class Function : Expression
{
    public Function(ExpressionKind kind, Scope scope, Expression body) : base(kind, scope)
    {
        Body = body;
    }

    public Expression Body { get; set; }
    public override ExpressionKind Kind { get; set; }
    public override object? Value { get; set; }
    public override Scope? Scope { get; set; }

    public override void Evaluate(Scope scope)
    {
        throw new NotImplementedException();
    }

    public override object? GetValue()
    {
        throw new NotImplementedException();
    }
}