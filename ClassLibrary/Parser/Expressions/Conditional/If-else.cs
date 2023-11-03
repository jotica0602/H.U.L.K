namespace ClassLibrary;

public class IfElse : Expression
{
    public Expression Condition;
    public Expression LeftNode;
    public Expression RightNode;

    public IfElse(Expression condition, Expression leftNode, Expression rightNode) : base(null!)
    {
        Condition = condition;
        LeftNode = leftNode;
        RightNode = rightNode;
        Kind = ExpressionKind.Temp;
    }

    public override ExpressionKind Kind { get; set; }
    public override object? Value { get; set; }
    public override Scope? Scope { get; set; }

    public override void Evaluate(Scope scope)
    {
        Condition!.Evaluate(scope);

        if (Condition.Value is true)
        {
            LeftNode.Evaluate(scope);
            Value = LeftNode.Value;
            Kind = LeftNode.Kind;
        }
        else
        {
            RightNode.Evaluate(scope);
            Value = RightNode!.GetValue();
            Kind = RightNode.Kind;
        }
    }

    public override object? GetValue() => Value;
}