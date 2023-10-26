namespace ClassLibrary;

#region Level 1

public class And : BinaryExpression
{
    public And(ExpressionKind kind, TokenKind operator_, Expression leftNode, Expression rightNode, Scope scope) :
    base(kind, operator_, leftNode, rightNode, scope)
    { }


    public override ExpressionKind Kind { get; set; }
    public override object? Value { get; set; }



    public override void Evaluate(Scope scope)
    {
        LeftNode!.Evaluate(scope);
        RightNode!.Evaluate(scope);
        Value = (bool)LeftNode.GetValue()! && (bool)RightNode.GetValue()!;
    }

    public override object? GetValue() => Value;
}


public class Or : BinaryExpression
{
    public Or(ExpressionKind kind, TokenKind operator_, Expression leftNode, Expression rightNode, Scope scope) :
    base(kind, operator_, leftNode, rightNode, scope)
    { }


    public override ExpressionKind Kind { get; set; }
    public override object? Value { get; set; }



    public override void Evaluate(Scope scope)
    {
        LeftNode!.Evaluate(scope);
        RightNode!.Evaluate(scope);
        Value = (bool)LeftNode.GetValue()! || (bool)RightNode.GetValue()!;
    }

    public override object? GetValue() => Value;
}

#endregion

#region Level 2
public class EqualsTo : BinaryExpression
{
    public EqualsTo(ExpressionKind kind, TokenKind operator_, Expression leftNode, Expression rightNode, Scope scope) :
    base(kind, operator_, leftNode, rightNode, scope)
    { }


    public override ExpressionKind Kind { get; set; }
    public override object? Value { get; set; }

    public override void CheckNodesSemantic(Expression leftNode, TokenKind operator_, Expression rightNode)
    {
        if(LeftNode!.Kind != RightNode!.Kind)
        {
            Console.WriteLine($"!semantic error: operator \"{operator_}\" cannot be applied between different data types.");
            throw new Exception();
        }
    }

    public override void Evaluate(Scope scope)
    {
        LeftNode!.Evaluate(scope);
        RightNode!.Evaluate(scope);
        Value = LeftNode.GetValue()!.ToString()! == RightNode.GetValue()!.ToString();
    }

    public override object? GetValue() => Value;
}


public class GreatherOrEquals : BinaryExpression
{
    public GreatherOrEquals(ExpressionKind kind, TokenKind operator_, Expression leftNode, Expression rightNode, Scope scope) :
    base(kind, operator_, leftNode, rightNode, scope)
    { }


    public override ExpressionKind Kind { get; set; }
    public override object? Value { get; set; }



    public override void Evaluate(Scope scope)
    {
        LeftNode!.Evaluate(scope);
        RightNode!.Evaluate(scope);
        Value = (double)LeftNode.GetValue()! >= (double)RightNode.GetValue()!;
    }

    public override object? GetValue() => Value;
}


public class GreatherThan : BinaryExpression
{
    public GreatherThan(ExpressionKind kind, TokenKind operator_, Expression leftNode, Expression rightNode, Scope scope) :
    base(kind, operator_, leftNode, rightNode, scope)
    { }


    public override ExpressionKind Kind { get; set; }
    public override object? Value { get; set; }



    public override void Evaluate(Scope scope)
    {
        LeftNode!.Evaluate(scope);
        RightNode!.Evaluate(scope);
        Value = (double)LeftNode.GetValue()! > (double)RightNode.GetValue()!;
    }

    public override object? GetValue() => Value;
}


public class LesserOrEquals : BinaryExpression
{
    public LesserOrEquals(ExpressionKind kind, TokenKind operator_, Expression leftNode, Expression rightNode, Scope scope) :
    base(kind, operator_, leftNode, rightNode, scope)
    { }


    public override ExpressionKind Kind { get; set; }
    public override object? Value { get; set; }

    public override void Evaluate(Scope scope)
    {
        LeftNode!.Evaluate(scope);
        RightNode!.Evaluate(scope);
        Value = (double)LeftNode.GetValue()! <= (double)RightNode.GetValue()!;
    }

    public override object? GetValue() => Value;
}


public class LesserThan : BinaryExpression
{
    public LesserThan(ExpressionKind kind, TokenKind operator_, Expression leftNode, Expression rightNode, Scope scope) :
    base(kind, operator_, leftNode, rightNode, scope)
    { }


    public override ExpressionKind Kind { get; set; }
    public override object? Value { get; set; }



    public override void Evaluate(Scope scope)
    {
        LeftNode!.Evaluate(scope);
        RightNode!.Evaluate(scope);
        Value = (double)LeftNode.GetValue()! < (double)RightNode.GetValue()!;
    }

    public override object? GetValue() => Value;
}

#endregion