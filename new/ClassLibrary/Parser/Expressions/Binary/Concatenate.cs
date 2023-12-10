namespace ClassLibrary;

public class Concatenation : BinaryExpression
{
    public Concatenation(TokenKind operator_, Expression leftNode, Expression rightNode) :
     base(operator_, leftNode, rightNode)
    {
        Kind = ExpressionKind.String;
    }

    public override ExpressionKind Kind { get; set; }
    public override object? Value { get; set; }

    public override void Evaluate(Scope scope)
    {
        LeftNode!.Evaluate(scope);
        RightNode!.Evaluate(scope);
        Value = LeftNode.GetValue()!.ToString()! + RightNode.GetValue()!.ToString()!;
    }

    public override object? GetValue() => Value;
}