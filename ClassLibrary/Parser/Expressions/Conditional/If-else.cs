namespace ClassLibrary;

public class IfElse : Expression
{
    public Expression Condition;
    public Expression LeftNode;
    public Expression RightNode;

    public IfElse(ExpressionKind kind, Expression condition, Expression leftNode, Expression rightNode):base(kind)
    {
        Condition = condition;
        LeftNode = leftNode;
        RightNode = rightNode;
    }


    public override ExpressionKind Kind { get; set; }
    public override object? Value { get; set; }

    public override void CheckSemantic()
    {
        return;
    }

    public override object? Evaluate(Scope scope) 
    {
        Condition.Evaluate(scope);
        if(Condition.Value is true)
        {
            Value = LeftNode.Evaluate(scope);
            Kind = LeftNode.Kind;
            return LeftNode.GetValue();
        }
        else 
        {
            Value = RightNode.Evaluate(scope);
            Kind = RightNode.Kind;
            return RightNode.GetValue();
        }
    }

    public override object? GetValue() => Value;

    public override void VisitNode()
    {
        throw new NotImplementedException();
    }
}