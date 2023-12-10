using System.Linq.Expressions;
namespace ClassLibrary;
public class Not : UnaryExpression
{
    public override Expression Node { get; set; }
    public override ExpressionKind Kind { get; set; }
    public override object? Value { get; set; }
    public override Scope? Scope { get; set; }

    public Not(Expression node) : base(node)
    {
        Kind = ExpressionKind.Bool;
        Node = node;
    }

    public override void CheckNodeSemantic(Expression node)
    {
        if (Node.Kind != ExpressionKind.Bool && Node.Kind != ExpressionKind.Temp)
        {
            Console.WriteLine($"!semantic error: operator \"{TokenKind.Not}\" cannot be applied to \"{Node.Kind}\".");
            throw new Exception();
        }
    }

    public override void Evaluate(Scope scope)
    {
        Node.Evaluate(Scope!);
        Value = !(bool)Node.GetValue()!;
    }

    public override object? GetValue() => Value;
}