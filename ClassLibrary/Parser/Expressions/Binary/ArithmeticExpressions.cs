namespace ClassLibrary;

#region  Level 3

public class Addition : BinaryExpression
{
    public Addition(TokenKind operator_, Expression leftNode, Expression rightNode) :
    base(operator_, leftNode, rightNode)
    {
        Kind = ExpressionKind.Number;
    }

    public override ExpressionKind Kind { get; set; }
    public override object? Value { get; set; }

    public override void Evaluate(Scope scope)
    {
        LeftNode!.Evaluate(scope);
        RightNode!.Evaluate(scope);
        Value = (double)LeftNode.GetValue()! + (double)RightNode.GetValue()!;
    }

    public override object? GetValue() => Value;
}

public class Substraction : BinaryExpression
{
    public Substraction(TokenKind operator_, Expression leftNode, Expression rightNode) :
    base(operator_, leftNode, rightNode)
    {
        Kind = ExpressionKind.Number;
    }

    public override ExpressionKind Kind { get; set; }
    public override object? Value { get; set; }

    public override void Evaluate(Scope scope)
    {
        LeftNode!.Evaluate(scope);
        RightNode!.Evaluate(scope);
        Value = (double)LeftNode.GetValue()! - (double)RightNode.GetValue()!;
    }

    public override object? GetValue() => Value;
}

#endregion


#region Level 4

public class Multiplication : BinaryExpression
{
    public Multiplication(TokenKind operator_, Expression leftNode, Expression rightNode) :
    base(operator_, leftNode, rightNode)
    {
        Kind = ExpressionKind.Number;
    }

    public override ExpressionKind Kind { get; set; }
    public override object? Value { get; set; }

    public override void Evaluate(Scope scope)
    {
        LeftNode!.Evaluate(scope);
        RightNode!.Evaluate(scope);

        Value = (double)LeftNode.GetValue()! * (double)RightNode.GetValue()!;
    }

    public override object? GetValue() => Value;
}

public class Division : BinaryExpression
{
    public Division(TokenKind operator_, Expression leftNode, Expression rightNode) :
    base(operator_, leftNode, rightNode)
    {
        Kind = ExpressionKind.Number;
    }

    public override ExpressionKind Kind { get; set; }
    public override object? Value { get; set; }

    public override void Evaluate(Scope scope)
    {
        LeftNode!.Evaluate(scope);
        RightNode!.Evaluate(scope);
        Value = (double)LeftNode.GetValue()! / (double)RightNode.GetValue()!;
    }

    public override object? GetValue() => Value;
}

public class Modulus : BinaryExpression
{
    public Modulus(TokenKind operator_, Expression leftNode, Expression rightNode) :
    base(operator_, leftNode, rightNode)
    {
        Kind = ExpressionKind.Number;
    }

    public override ExpressionKind Kind { get; set; }
    public override object? Value { get; set; }

    public override void Evaluate(Scope scope)
    {
        LeftNode!.Evaluate(scope);
        RightNode!.Evaluate(scope);
        Value = (double)LeftNode.GetValue()! % (double)RightNode.GetValue()!;
    }

    public override object? GetValue() => Value;
}

#endregion


#region Level 5

public class Power : BinaryExpression
{
    public Power(TokenKind operator_, Expression leftNode, Expression rightNode) :
    base(operator_, leftNode, rightNode)
    {
        Kind = ExpressionKind.Number;
    }

    public override ExpressionKind Kind { get; set; }
    public override object? Value { get; set; }

    public override void Evaluate(Scope scope)
    {
        LeftNode!.Evaluate(scope);
        RightNode!.Evaluate(scope);
        Value = Math.Pow((double)LeftNode.GetValue()!, (double)RightNode.GetValue()!);
    }

    public override object? GetValue() => Value;
}

#endregion