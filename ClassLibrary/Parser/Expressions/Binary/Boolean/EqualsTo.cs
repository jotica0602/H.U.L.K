namespace ClassLibrary;

public class EqualsTo : BinaryExpression
{
    public EqualsTo(ExpressionKind kind, TokenKind operator_, Expression leftNode, Expression rightNode) :
    base(kind, operator_, leftNode, rightNode)
    { }


    public override ExpressionKind Kind { get; set; }
    public override object? Value { get; set; }

    public override void CheckSemantic()
    {
        if (LeftNode!.Kind != RightNode!.Kind)
        {
            Console.WriteLine($"!semantic error: \"{Operator}\" cannot be applied between \"{LeftNode.Kind}\" and \"{RightNode!.Kind}\"");
            throw new Exception();
        }
    }

    public override object? Evaluate(Scope scope) => Value = LeftNode!.Evaluate(scope)!.ToString()! == RightNode!.Evaluate(scope)!.ToString()!;

    public override object? GetValue() => Value;

    public override void VisitNode()
    {
        throw new NotImplementedException();
    }
}