namespace ClassLibrary;

public class ASTBuilder
{
    List<Token> tokens;
    int currentTokenIndex;
    Token currentToken;

    public ASTBuilder(List<Token> tokens)
    {
        this.tokens = tokens;
        currentTokenIndex = 0;
        currentToken = tokens[currentTokenIndex];
    }

    public void Consume(int positions)
    {
        currentTokenIndex += positions;

        if (currentTokenIndex < tokens.Count)
        {
            currentToken = tokens[currentTokenIndex];
        }

        else return;
    }

    public Expression Build()
    {
        if (tokens.Count <= 1)
        {
            Console.WriteLine("Invalid input.");
            throw new Exception();
        }

        Expression ast = BuildLevel1();

        // Console.WriteLine(ast.Evaluate());

        if (currentToken.Kind != TokenKind.Semicolon)
        {
            Console.WriteLine($"syntax error: operator or expression is missing after \"{currentToken}\"");
            throw new Exception();
        }

        return ast;
    }

    private Expression BuildLevel1()
    {
        Expression leftNode = BuildLevel2();

        while (IsALevel1Operator(currentToken.Kind))
        {
            TokenKind operation = currentToken.Kind;
            Consume(1);
            Expression rightNode = BuildLevel2();
            leftNode = BuildExpression(leftNode, operation, rightNode);
        }

        Expression node = leftNode;
        return node;
    }

    private Expression BuildLevel2()
    {
        Expression leftNode = BuildLevel3();

        while (IsALevel2Operator(currentToken.Kind))
        {
            TokenKind operation = currentToken.Kind;
            Consume(1);
            Expression rightNode = BuildLevel3();
            leftNode = BuildExpression(leftNode, operation, rightNode);
        }

        Expression node = leftNode;
        return node;
    }

    private Expression BuildLevel3()
    {
        Expression leftNode = BuildLevel4();

        while (IsALevel3Operator(currentToken.Kind))
        {
            TokenKind operation = currentToken.Kind;
            Consume(1);
            Expression rightNode = BuildLevel4();
            leftNode = BuildExpression(leftNode, operation, rightNode);
        }

        Expression node = leftNode;
        return node;
    }

    private Expression BuildLevel4()
    {
        Expression leftNode = BuildLevel5();

        while (IsALevel4Operator(currentToken.Kind))
        {
            TokenKind operation = currentToken.Kind;
            Consume(1);
            Expression rightNode = BuildLevel5();
            leftNode = BuildExpression(leftNode, operation, rightNode);
        }

        Expression node = leftNode;
        return node;
    }

    private Expression BuildLevel5()
    {
        Expression leftNode = GetAtom();

        while (IsALevel5Operator(currentToken.Kind))
        {
            TokenKind operation = currentToken.Kind;
            Consume(1);
            Expression rightNode = GetAtom();
            leftNode = BuildExpression(leftNode, operation, rightNode);
        }

        Expression node = leftNode;
        return node;
    }

    Expression BuildExpression(Expression leftNode, TokenKind operation, Expression expressionNode)
    {
        switch (operation)
        {
            case TokenKind.And:
                Expression and = new And(ExpressionKind.Bool, operation, leftNode, expressionNode);
                and.CheckSemantic();
                leftNode = and;
                break;

            case TokenKind.Or:
                Expression or = new Or(ExpressionKind.Bool, operation, leftNode, expressionNode);
                or.CheckSemantic();
                leftNode = or;
                break;

            case TokenKind.GreatherOrEquals:
                Expression greatherOrEquals = new GreatherOrEquals(ExpressionKind.Bool, operation, leftNode, expressionNode);
                greatherOrEquals.CheckSemantic();
                leftNode = greatherOrEquals;
                break;

            case TokenKind.GreatherThan:
                Expression greatherThan = new GreatherThan(ExpressionKind.Bool, operation, leftNode, expressionNode);
                greatherThan.CheckSemantic();
                leftNode = greatherThan;
                break;

            case TokenKind.LessOrEquals:
                Expression lessOrEquals = new LessOrEquals(ExpressionKind.Bool, operation, leftNode, expressionNode);
                lessOrEquals.CheckSemantic();
                leftNode = lessOrEquals;
                break;

            case TokenKind.LessThan:
                Expression lesserThan = new LesserThan(ExpressionKind.Bool, operation, leftNode, expressionNode);
                lesserThan.CheckSemantic();
                leftNode = lesserThan;
                break;

            case TokenKind.EqualsTo:
                Expression equalsTo = new EqualsTo(ExpressionKind.Bool, operation, leftNode, expressionNode);
                equalsTo.CheckSemantic();
                leftNode = equalsTo;
                break;

            case TokenKind.Addition:
                Expression addition = new Addition(ExpressionKind.Number, operation, leftNode, expressionNode);
                addition.CheckSemantic();
                // addition.Evaluate();
                leftNode = addition;
                break;

            case TokenKind.Substraction:
                Expression substraction = new Substraction(ExpressionKind.Number, operation, leftNode, expressionNode);
                substraction.CheckSemantic();
                // substraction.Evaluate();
                leftNode = substraction;
                break;

            case TokenKind.Concat:
                Expression concatenation = new Concat(ExpressionKind.String, operation, leftNode, expressionNode);
                leftNode = concatenation;
                break;

            case TokenKind.Multiplication:
                Expression Multiplication = new Multiplication(ExpressionKind.Number, operation, leftNode, expressionNode);
                Multiplication.CheckSemantic();
                // Multiplication.Evaluate();
                leftNode = Multiplication;
                break;

            case TokenKind.Division:
                Expression Division = new Division(ExpressionKind.Number, operation, leftNode, expressionNode);
                Division.CheckSemantic();
                // Division.Evaluate();
                leftNode = Division;
                break;

            case TokenKind.Modulus:
                Expression Modulus = new Modulus(ExpressionKind.Number, operation, leftNode, expressionNode);
                Modulus.CheckSemantic();
                // Modulus.Evaluate();
                leftNode = Modulus;
                break;

            case TokenKind.Power:
                BinaryExpression power = new Power(ExpressionKind.Number, operation, leftNode, expressionNode);
                power.CheckSemantic();
                // power.Evaluate();
                leftNode = power;
                break;
        }

        return leftNode;
    }


    private Expression GetAtom()
    {
        switch (currentToken.Kind)
        {
            case TokenKind.Number:
                Number num = new Number(ExpressionKind.Number, (double)currentToken.GetValue());
                Console.WriteLine($"{currentToken}");
                Consume(1);
                return num;

            case TokenKind.LeftParenthesis:
                Console.WriteLine($"{currentToken}");
                Consume(1);
                Expression expression = BuildLevel1();
                Console.WriteLine($"{currentToken}");

                if (currentToken.Kind != TokenKind.RightParenthesis)
                {
                    Console.WriteLine($"!syntax error: \"{TokenKind.RightParenthesis}\": \")\" is missing after {currentToken}");
                    throw new Exception();
                }

                Consume(1);

                return expression;

            case TokenKind.String:
                Console.WriteLine($"{currentToken}");
                String str = new String(ExpressionKind.String, (string)currentToken.GetValue());
                Consume(1);
                return str;

            case TokenKind.TrueKeyWord:
                Console.WriteLine($"{currentToken}");
                Bool bool_ = new Bool(ExpressionKind.Bool, true);
                Consume(1);
                return bool_;

            case TokenKind.FalseKeyWord:
                Console.WriteLine($"{currentToken}");
                bool_ = new Bool(ExpressionKind.Bool, false);
                Consume(1);
                return bool_;

            case TokenKind.ElseKeyWord:
                Console.WriteLine($"!syntax error: if-else structure is not balanced");
                throw new Exception();

            default:
                Console.WriteLine("Invalid Expression");
                throw new Exception();
        }
    }

    bool IsALevel1Operator(TokenKind operation)
    {
        List<TokenKind> operators = new List<TokenKind>()
            {
                TokenKind.And,
                TokenKind.Or
            };

        return operators.Contains(operation);
    }

    bool IsALevel2Operator(TokenKind operation)
    {
        List<TokenKind> operators = new List<TokenKind>()
            {
                TokenKind.LessThan,
                TokenKind.LessOrEquals,
                TokenKind.GreatherThan,
                TokenKind.GreatherOrEquals,
                TokenKind.EqualsTo,
                TokenKind.NotEquals
            };

        return operators.Contains(operation);
    }

    bool IsALevel3Operator(TokenKind operation)
    {
        List<TokenKind> operators = new List<TokenKind>()
            {
                TokenKind.Addition,
                TokenKind.Substraction,
                TokenKind.Concat
            };

        return operators.Contains(operation);
    }

    bool IsALevel4Operator(TokenKind operation)
    {
        List<TokenKind> operators = new List<TokenKind>()
            {
                TokenKind.Multiplication,
                TokenKind.Division,
                TokenKind.Modulus
            };

        return operators.Contains(operation);
    }

    bool IsALevel5Operator(TokenKind operation)
    {
        List<TokenKind> operators = new List<TokenKind>()
            {
                TokenKind.Power
            };

        return operators.Contains(operation);
    }

}