namespace ClassLibrary;
public abstract class Expression
{
    public abstract ExpressionKind Kind { get; set; }

    public abstract object? Value { get; set; }

    public abstract Scope? Scope { get; set; }

    public Expression(ExpressionKind kind, Scope scope)
    {
        Kind = kind;
        Scope = scope;
    }

    public abstract void Evaluate(Scope scope);

    public abstract object? GetValue();
}