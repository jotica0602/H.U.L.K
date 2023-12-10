namespace ClassLibrary;
public abstract class Expression
{
    public abstract ExpressionKind Kind { get; set; }

    public abstract object? Value { get; set; }

    public virtual Scope? Scope { get; set; }

    public Expression(Scope scope)
    {
        Scope = scope;
    }

    public virtual void Evaluate(Scope scope) { return; }

    public override string ToString() => $"{Value}";
    public abstract object? GetValue();
}