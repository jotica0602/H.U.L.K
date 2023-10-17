namespace ClassLibrary;

public class Division : BinaryExpression
{
    public Division(ExpressionKind kind, TokenKind operator_, Expression leftNode, Expression rightNode) :
    base(kind, operator_, leftNode, rightNode)
    { }


    public override ExpressionKind Kind { get; set; }
    public override object? Value { get; set; }

    public override void CheckSemantic()
    {
        if ((LeftNode!.Kind != ExpressionKind.Number && LeftNode!.Kind != ExpressionKind.Identifier) || 
            (RightNode!.Kind != ExpressionKind.Number && RightNode!.Kind != ExpressionKind.Identifier))
        {
            Console.WriteLine($"!semantic error: \"{Operator}\" cannot be applied between \"{LeftNode.Kind}\" and \"{RightNode!.Kind}\"");
            throw new Exception();
        }
        if ((double)RightNode.Value! == 0)
        {
            Console.WriteLine("We don't do that here.");
            throw new Exception();
        }
    }

    public override object? Evaluate(Scope scope) => Value = (double)LeftNode!.Evaluate(scope)! / (double)RightNode!.Evaluate(scope)!;

    public override object? GetValue() => Value;

    public override void VisitNode()
    {
        throw new NotImplementedException();
    }
}