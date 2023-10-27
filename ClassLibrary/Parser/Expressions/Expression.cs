namespace ClassLibrary;
public abstract class Expression
{
    public abstract ExpressionKind Kind { get; set; }

    public abstract object? Value { get; set; }

    public virtual Scope? Scope { get; set; }

    public Expression(ExpressionKind kind, Scope scope)
    {
        Kind = kind;
        Scope = scope;
    }

    public virtual void Evaluate(Scope scope) {return;}

    public abstract object? GetValue();
}