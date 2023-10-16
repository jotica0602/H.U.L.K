namespace ClassLibrary;

public abstract class BinaryExpression : Expression
{
    public Expression? LeftNode;
    public Expression? RightNode;
    public TokenKind Operator;

    public BinaryExpression(ExpressionKind kind, TokenKind operator_, Expression leftNode, Expression rightNode) : base(kind)
    {
        LeftNode = leftNode;
        RightNode = rightNode;
        Operator = operator_;
    }
}