namespace ClassLibrary;

public abstract class AtomExpression : Expression
{
    public override Scope? Scope { get; set; }
    public AtomExpression(ExpressionKind kind, Scope scope) : base(kind, scope) { }
}

