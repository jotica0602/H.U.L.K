namespace ClassLibrary;

public class IfElse : Expression
{
    public Expression Condition;
    public Expression LeftNode;
    public Expression RightNode;

    public IfElse(ExpressionKind kind, Expression condition, Expression leftNode, Expression rightNode, Scope scope) : base(kind, scope)
    {
        Condition = condition;
        LeftNode = leftNode;
        RightNode = rightNode;
    }

    public override ExpressionKind Kind { get; set; }
    public override object? Value { get; set; }
    public override Scope? Scope { get; set; }

    public override void Evaluate(Scope scope)
    {
        Condition!.Evaluate(scope);

        if (Condition.Value is true)
        {
            Value = LeftNode!.GetValue();
            Kind = LeftNode.Kind;
        }
        else
        {
            Value = RightNode!.GetValue();
            Kind = RightNode.Kind;
        }
    }

    public override object? GetValue() => Value;
}