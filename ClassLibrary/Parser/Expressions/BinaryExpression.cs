using System.Runtime.CompilerServices;

namespace ClassLibrary;

public abstract class BinaryExpression : Expression
{
    public Expression? LeftNode;
    public Expression? RightNode;
    public TokenKind Operator;
    public override Scope? Scope { get; set; }


    public BinaryExpression(ExpressionKind kind, TokenKind operator_, Expression leftNode, Expression rightNode, Scope scope) : base(kind, scope)
    {
        LeftNode = leftNode;
        RightNode = rightNode;
        Operator = operator_;
    }

    public virtual void CheckNodesSemantic(Expression leftNode, TokenKind operator_, Expression rightNode)
    {
        if ((leftNode!.Kind != ExpressionKind.Number && leftNode.Kind != ExpressionKind.Temp) || (rightNode!.Kind != ExpressionKind.Number && rightNode.Kind != ExpressionKind.Temp))
        {
            Console.WriteLine($"!semantic error: \"{operator_}\" cannot be applied between \"{leftNode.Kind}\" and \"{rightNode!.Kind}\".");
            throw new Exception();
        }
    }
}