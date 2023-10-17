using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ClassLibrary;

public class ASTBuilder
{
    List<Token> tokens;
    int currentTokenIndex;
    Token currentToken;
    private Scope scope;

    public ASTBuilder(List<Token> tokens, Scope scope)
    {
        this.tokens = tokens;
        this.scope = scope;
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
            leftNode = BuildBinaryExpression(leftNode, operation, rightNode);
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
            leftNode = BuildBinaryExpression(leftNode, operation, rightNode);
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
            leftNode = BuildBinaryExpression(leftNode, operation, rightNode);
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
            leftNode = BuildBinaryExpression(leftNode, operation, rightNode);
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
            leftNode = BuildBinaryExpression(leftNode, operation, rightNode);
        }

        Expression node = leftNode;
        return node;
    }

    Expression BuildBinaryExpression(Expression leftNode, TokenKind operation, Expression expressionNode)
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
                leftNode = addition;
                break;

            case TokenKind.Substraction:
                Expression substraction = new Substraction(ExpressionKind.Number, operation, leftNode, expressionNode);
                substraction.CheckSemantic();
                leftNode = substraction;
                break;

            case TokenKind.Concat:
                Expression concatenation = new Concat(ExpressionKind.String, operation, leftNode, expressionNode);
                leftNode = concatenation;
                break;

            case TokenKind.Multiplication:
                Expression Multiplication = new Multiplication(ExpressionKind.Number, operation, leftNode, expressionNode);
                Multiplication.CheckSemantic();
                leftNode = Multiplication;
                break;

            case TokenKind.Division:
                Expression Division = new Division(ExpressionKind.Number, operation, leftNode, expressionNode);
                Division.CheckSemantic();
                leftNode = Division;
                break;

            case TokenKind.Modulus:
                Expression Modulus = new Modulus(ExpressionKind.Number, operation, leftNode, expressionNode);
                Modulus.CheckSemantic();
                leftNode = Modulus;
                break;

            case TokenKind.Power:
                BinaryExpression power = new Power(ExpressionKind.Number, operation, leftNode, expressionNode);
                power.CheckSemantic();
                leftNode = power;
                break;
        }

        return leftNode;
    }

    Expression BuildConditionalExpression(IfElse conditionalExpression)
    {
        conditionalExpression.Condition = BuildLevel1();
        if (currentToken.Kind == TokenKind.ElseKeyWord)
        {
            Console.WriteLine($"!syntax error: if-else expression is incomplete.");
            throw new Exception();
        }
        conditionalExpression.LeftNode = BuildLevel1();
        Expect(TokenKind.ElseKeyWord);
        conditionalExpression.RightNode = BuildLevel1();
        return conditionalExpression;
    }

    Expression BuildLetInStructure()
    {
        Consume(1);
        Dictionary<string, Expression> enviroment = new Dictionary<string, Expression>();
        scope.Vars.Add(enviroment);
        CreateVar();
        LetIn expression = new LetIn(ExpressionKind.Temp, null!);
        Expect(TokenKind.InKeyWord);
        expression.Execution = BuildLevel1();
        expression.Execution.CheckSemantic();
        return expression;
    }

    void CreateVar()
    {
        Expect(TokenKind.Identifier);
        string varName = tokens[currentTokenIndex-1].GetName();
        Expect(TokenKind.Equals);
        scope.Vars.Last().Add(varName, BuildLevel1());
        // Expression varExpression = BuildLevel1();
        // varExpression.CheckSemantic();

        Console.WriteLine(currentToken);

        if (currentToken.Kind == TokenKind.Comma)
        {
            Consume(1);
            CreateVar();
        }
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
                Expect(TokenKind.RightParenthesis);
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

            case TokenKind.IfKeyWord:
                Console.Write($"{currentToken}");
                Consume(1);
                IfElse conditionalExpression = new IfElse(ExpressionKind.Temp, null!, null!, null!);
                return BuildConditionalExpression(conditionalExpression);

            case TokenKind.ElseKeyWord:
                Console.WriteLine($"!syntax error: if-else structure is not balanced");
                throw new Exception();

            case TokenKind.LetKeyWord:
                Console.WriteLine($"{currentToken}");
                return BuildLetInStructure();

            case TokenKind.Identifier:
                Console.WriteLine($"{currentToken}");
                Identifier variable = new Identifier(ExpressionKind.Identifier,currentToken.GetName(),null!,scope);
                variable.CheckSemantic();
                Consume(1);
                return variable;

            default:
                Console.WriteLine("Invalid Expression");
                throw new Exception();
        }
    }

    private void Expect(TokenKind expected)
    {
        if (currentToken.Kind != expected)
        {
            Console.WriteLine($"!syntax error: unexpected token: \"{currentToken}\" at index: {currentTokenIndex} expected: \"{expected}\".");
            throw new Exception();
        }

        Consume(1);
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