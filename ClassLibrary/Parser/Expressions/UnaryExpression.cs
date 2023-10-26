namespace ClassLibrary;

public abstract class UnaryExpression : Expression
{
    public abstract Expression Node { get; set; }
    public UnaryExpression(ExpressionKind kind, Scope scope, Expression node) : base(kind, scope)
    {
        Node = node;
    }

    public abstract void CheckNodeSemantic(Expression node);
}