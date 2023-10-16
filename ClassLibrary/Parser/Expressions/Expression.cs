namespace ClassLibrary;
public abstract class Expression
{

    public abstract ExpressionKind Kind { get; set; }
    public abstract object? Value { get; set; }

    public Expression(ExpressionKind kind)
    {
        Kind = kind;
    }

    public abstract void VisitNode();
    
    public abstract void CheckSemantic();

    public abstract object? Evaluate();

    public abstract object? GetValue();
}