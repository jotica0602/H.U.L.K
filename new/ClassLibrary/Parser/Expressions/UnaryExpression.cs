namespace ClassLibrary;

public abstract class UnaryExpression : Expression
{
    public abstract Expression Node { get; set; }
    public UnaryExpression(Expression node) : base(null!)
    {
        Node = node;
    }

    public abstract void CheckNodeSemantic(Expression node);
}