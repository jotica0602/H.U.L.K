namespace ClassLibrary;

public class GreatherThan : BinaryExpression
{
    public GreatherThan(ExpressionKind kind, TokenKind operator_, Expression leftNode, Expression rightNode) :
    base(kind, operator_, leftNode, rightNode)
    { }


    public override ExpressionKind Kind { get; set; }
    public override object? Value { get; set; }

    public override void CheckSemantic()
    {
        if (LeftNode!.Kind != ExpressionKind.Number || RightNode!.Kind != ExpressionKind.Number)
        {
            Console.WriteLine($"!semantic error: \"{Operator}\" cannot be applied between \"{LeftNode.Kind}\" and \"{RightNode!.Kind}\"");
            throw new Exception();
        }
    }

    public override object? Evaluate() => Value = (double)LeftNode!.Evaluate()! > (double)RightNode!.Evaluate()!;

    public override object? GetValue() => Value;

    public override void VisitNode()
    {
        throw new NotImplementedException();
    }
}