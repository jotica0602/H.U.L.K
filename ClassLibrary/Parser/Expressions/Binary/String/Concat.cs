namespace ClassLibrary;

public class Concat : BinaryExpression
{
    public Concat(ExpressionKind kind, TokenKind operator_, Expression leftNode, Expression rightNode) :
    base(kind, operator_, leftNode, rightNode)
    { }


    public override ExpressionKind Kind { get; set; }
    public override object? Value { get; set; }

    public override void CheckSemantic()
    {
        return;
    }

    public override object? Evaluate() => Value = (LeftNode!.Evaluate()!.ToString() + RightNode!.Evaluate()!.ToString());

    public override object? GetValue() => Value;

    public override void VisitNode()
    {
        throw new NotImplementedException();
    }
}