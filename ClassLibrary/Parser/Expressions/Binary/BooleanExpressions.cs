namespace ClassLibrary;

#region Level 1

public class And : BinaryExpression
{
    public And(TokenKind operator_, Expression leftNode, Expression rightNode) :
    base(operator_, leftNode, rightNode)
    {
        Kind = ExpressionKind.Bool;
    }

    public override ExpressionKind Kind { get; set; }
    public override object? Value { get; set; }

    public override void Evaluate(Scope scope)
    {
        LeftNode!.Evaluate(scope);
        RightNode!.Evaluate(scope);

        if (LeftNode.Kind != ExpressionKind.Bool || RightNode.Kind != ExpressionKind.Bool)
        {
            Console.WriteLine($"!semantic error: \"{Operator}\" cannot be applied between \"{LeftNode.Kind}\" and \"{RightNode.Kind}\".");
            throw new Exception();
        }

        Value = (bool)LeftNode.GetValue()! && (bool)RightNode.GetValue()!;
    }

    public override void CheckNodesSemantic(Expression leftNode, TokenKind operator_, Expression rightNode)
    {
        if ((LeftNode.Kind != ExpressionKind.Bool && LeftNode.Kind != ExpressionKind.Temp) || (RightNode.Kind != ExpressionKind.Bool && RightNode.Kind != ExpressionKind.Temp))
        {
            Console.WriteLine($"!semantic error: operator \"{operator_}\" cannot be applied between \"{LeftNode.Kind}\" and \"{RightNode!.Kind}\" data types.");
            throw new Exception();
        }
    }

    public override object? GetValue() => Value;
}


public class Or : BinaryExpression
{
    public Or(TokenKind operator_, Expression leftNode, Expression rightNode) :
    base(operator_, leftNode, rightNode)
    {
        Kind = ExpressionKind.Bool;
    }

    public override ExpressionKind Kind { get; set; }
    public override object? Value { get; set; }
    public override void Evaluate(Scope scope)
    {
        LeftNode!.Evaluate(scope);
        RightNode!.Evaluate(scope);

        if (LeftNode.Kind != ExpressionKind.Bool || RightNode.Kind != ExpressionKind.Bool)
        {
            Console.WriteLine($"!semantic error: \"{Operator}\" cannot be applied between \"{LeftNode.Kind}\" and \"{RightNode.Kind}\".");
            throw new Exception();
        }

        Value = (bool)LeftNode.GetValue()! || (bool)RightNode.GetValue()!;
    }

    public override void CheckNodesSemantic(Expression leftNode, TokenKind operator_, Expression rightNode)
    {
        if ((LeftNode.Kind != ExpressionKind.Bool && LeftNode.Kind != ExpressionKind.Temp) || (RightNode.Kind != ExpressionKind.Bool && RightNode.Kind != ExpressionKind.Temp))
        {
            Console.WriteLine($"!semantic error: operator \"{operator_}\" cannot be applied between \"{LeftNode.Kind}\" and \"{RightNode!.Kind}\" data types.");
            throw new Exception();
        }
    }

    public override object? GetValue() => Value;
}

#endregion

#region Level 2
public class EqualsTo : BinaryExpression
{
    public EqualsTo(TokenKind operator_, Expression leftNode, Expression rightNode) :
    base(operator_, leftNode, rightNode)
    {
        Kind = ExpressionKind.Bool;
    }

    public override ExpressionKind Kind { get; set; }
    public override object? Value { get; set; }

    public override void CheckNodesSemantic(Expression leftNode, TokenKind operator_, Expression rightNode)
    {
        if (LeftNode!.Kind == ExpressionKind.Temp || RightNode!.Kind == ExpressionKind.Temp) { return; }
        if (LeftNode.Kind != RightNode.Kind)
        {
            Console.WriteLine($"!semantic error: operator \"{operator_}\" cannot be applied between different data types.");
            throw new Exception();
        }
    }

    public override void Evaluate(Scope scope)
    {
        LeftNode!.Evaluate(scope);
        RightNode!.Evaluate(scope);

        if (LeftNode.Kind != RightNode.Kind)
        {
            Console.WriteLine($"!semantic error: \"{Operator}\" cannot be applied between \"{LeftNode.Kind}\" and \"{RightNode.Kind}\".");
            throw new Exception();
        }

        Value = LeftNode.GetValue()!.ToString() == RightNode.GetValue()!.ToString();
    }

    public override object? GetValue() => Value;
}


public class GreatherOrEquals : BinaryExpression
{
    public GreatherOrEquals(TokenKind operator_, Expression leftNode, Expression rightNode) :
    base(operator_, leftNode, rightNode)
    {
        Kind = ExpressionKind.Bool;
    }


    public override ExpressionKind Kind { get; set; }
    public override object? Value { get; set; }



    public override void Evaluate(Scope scope)
    {
        LeftNode.Evaluate(scope);
        RightNode.Evaluate(scope);

        if (LeftNode.Kind != ExpressionKind.Number || RightNode.Kind != ExpressionKind.Number)
        {
            Console.WriteLine($"!semantic error: \"{Operator}\" cannot be applied between \"{LeftNode.Kind}\" and \"{RightNode.Kind}\".");
            throw new Exception();
        }

        Value = (double)LeftNode.GetValue()! >= (double)RightNode.GetValue()!;
    }

    public override object? GetValue() => Value;
}


public class GreatherThan : BinaryExpression
{
    public GreatherThan(TokenKind operator_, Expression leftNode, Expression rightNode) :
     base(operator_, leftNode, rightNode)
    {
        Kind = ExpressionKind.Bool;
    }


    public override ExpressionKind Kind { get; set; }
    public override object? Value { get; set; }



    public override void Evaluate(Scope scope)
    {
        LeftNode!.Evaluate(scope);
        RightNode!.Evaluate(scope);

        if (LeftNode.Kind != ExpressionKind.Number || RightNode.Kind != ExpressionKind.Number)
        {
            Console.WriteLine($"!semantic error: \"{Operator}\" cannot be applied between \"{LeftNode.Kind}\" and \"{RightNode.Kind}\".");
            throw new Exception();
        }

        Value = (double)LeftNode.GetValue()! > (double)RightNode.GetValue()!;
    }

    public override object? GetValue() => Value;
}


public class LesserOrEquals : BinaryExpression
{
    public LesserOrEquals(TokenKind operator_, Expression leftNode, Expression rightNode) :
    base(operator_, leftNode, rightNode)
    {
        Kind = ExpressionKind.Bool;
    }


    public override ExpressionKind Kind { get; set; }
    public override object? Value { get; set; }

    public override void Evaluate(Scope scope)
    {
        LeftNode!.Evaluate(scope);
        RightNode!.Evaluate(scope);

        if (LeftNode.Kind != ExpressionKind.Number || RightNode.Kind != ExpressionKind.Number)
        {
            Console.WriteLine($"!semantic error: \"{Operator}\" cannot be applied between \"{LeftNode.Kind}\" and \"{RightNode.Kind}\".");
            throw new Exception();
        }

        Value = (double)LeftNode.GetValue()! <= (double)RightNode.GetValue()!;
    }

    public override object? GetValue() => Value;
}


public class LesserThan : BinaryExpression
{
    public LesserThan(TokenKind operator_, Expression leftNode, Expression rightNode) :
    base(operator_, leftNode, rightNode)
    {
        Kind = ExpressionKind.Bool;
    }


    public override ExpressionKind Kind { get; set; }
    public override object? Value { get; set; }

    public override void Evaluate(Scope scope)
    {
        LeftNode!.Evaluate(scope);
        RightNode!.Evaluate(scope);

        if (LeftNode.Kind != ExpressionKind.Number || RightNode.Kind != ExpressionKind.Number)
        {
            Console.WriteLine($"!semantic error: \"{Operator}\" cannot be applied between \"{LeftNode.Kind}\" and \"{RightNode.Kind}\".");
            throw new Exception();
        }

        Value = (double)LeftNode.GetValue()! < (double)RightNode.GetValue()!;
    }

    public override object? GetValue() => Value;
}

#endregion