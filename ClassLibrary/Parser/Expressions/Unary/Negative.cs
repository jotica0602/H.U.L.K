using System.Linq.Expressions;
namespace ClassLibrary;
public class Negative : UnaryExpression
{
    public override Expression Node { get; set; }
    public override ExpressionKind Kind { get; set; }
    public override object? Value { get; set; }
    public override Scope? Scope { get; set; }

    public Negative(ExpressionKind kind, Scope scope, Expression node) : base(kind, scope, node)
    {
        Node = node;
    }

    public override void CheckNodeSemantic(Expression node)
    {
        if(Node.Kind != ExpressionKind.Number && Node.Kind != ExpressionKind.Temp)
        {
            Console.WriteLine($"!semantic error: operator \"{TokenKind.Substraction}\" cannot be applied to \"{Node.Kind}\".");
        }
    }

    public override void Evaluate(Scope scope)
    {
        Node.Evaluate(Scope!);
        Value = -(double)Node.GetValue()!;
    }

    public override object? GetValue() => Value;
}