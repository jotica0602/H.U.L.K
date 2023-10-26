namespace ClassLibrary;

public class Concat : BinaryExpression
{
    public Concat(ExpressionKind kind, TokenKind operator_, Expression leftNode, Expression rightNode, Scope scope) :
    base(kind, operator_, leftNode, rightNode, scope)
    { }

    public override ExpressionKind Kind { get; set; }
    public override object? Value { get; set; }

    public override void Evaluate(Scope scope)
    {
        LeftNode!.Evaluate(scope);
        RightNode!.Evaluate(scope);
        Value = LeftNode.GetValue()!.ToString()! +  RightNode.GetValue()!.ToString()!;
    }

    public override object? GetValue() => Value;
}