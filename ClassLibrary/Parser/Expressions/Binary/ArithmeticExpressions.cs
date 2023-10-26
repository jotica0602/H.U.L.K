namespace ClassLibrary;

#region  Level 3

public class Addition : BinaryExpression
{
    public Addition(ExpressionKind kind, TokenKind operator_, Expression leftNode, Expression rightNode, Scope scope) :
    base(kind, operator_, leftNode, rightNode, scope)
    { }

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
    public Substraction(ExpressionKind kind, TokenKind operator_, Expression leftNode, Expression rightNode, Scope scope) :
    base(kind, operator_, leftNode, rightNode, scope)
    { }

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
    public Multiplication(ExpressionKind kind, TokenKind operator_, Expression leftNode, Expression rightNode, Scope scope) :
    base(kind, operator_, leftNode, rightNode, scope)
    { }

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
    public Division(ExpressionKind kind, TokenKind operator_, Expression leftNode, Expression rightNode, Scope scope) :
    base(kind, operator_, leftNode, rightNode, scope)
    { }

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
    public Modulus(ExpressionKind kind, TokenKind operator_, Expression leftNode, Expression rightNode, Scope scope) :
    base(kind, operator_, leftNode, rightNode, scope)
    { }

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
    public Power(ExpressionKind kind, TokenKind operator_, Expression leftNode, Expression rightNode, Scope scope) :
    base(kind, operator_, leftNode, rightNode, scope)
    { }

    public override ExpressionKind Kind { get; set; }
    public override object? Value { get; set; }

    public override void Evaluate(Scope scope)
    {
        LeftNode!.Evaluate(scope);
        RightNode!.Evaluate(scope);
        Value = Math.Pow((double)LeftNode.GetValue()!,(double)RightNode.GetValue()!);
    }

    public override object? GetValue() => Value;
}

#endregion