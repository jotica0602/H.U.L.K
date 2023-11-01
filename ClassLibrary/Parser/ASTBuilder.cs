using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

namespace ClassLibrary;

public class ASTBuilder
{
    List<Token> tokens;
    int currentTokenIndex;
    Token currentToken;
    private Scope GlobalScope;

    public ASTBuilder(List<Token> tokens, Scope scope)
    {
        this.tokens = tokens;
        GlobalScope = scope;
        currentTokenIndex = 0;
        currentToken = tokens[currentTokenIndex];
    }

    private void Consume(int positions)
    {
        currentTokenIndex += positions;

        if (currentTokenIndex < tokens.Count)
        {
            currentToken = tokens[currentTokenIndex];
        }

        else return;
    }

    private Token NextToken() => tokens[currentTokenIndex + 1];

    private Token PreviousToken() => tokens[currentTokenIndex - 1];

    public Expression Build()
    {
        if (tokens.Count <= 1)
        {
            Console.WriteLine("Invalid input.");
            throw new Exception();
        }

        Expression ast = BuildLevel1(GlobalScope);

        if (currentToken.Kind != TokenKind.Semicolon)
        {
            Console.WriteLine($"syntax error: operator or expression is missing after \"{currentToken}\"");
            throw new Exception();
        }

        return ast;
    }

    private Expression BuildLevel1(Scope scope)
    {
        Expression leftNode = BuildLevel2(scope);

        while (IsALevel1Operator(currentToken.Kind))
        {
            TokenKind operation = currentToken.Kind;
            Consume(1);
            Expression rightNode = BuildLevel2(scope);
            leftNode = BuildBinaryExpression(leftNode, operation, rightNode);
        }

        Expression node = leftNode;
        return node;
    }

    private Expression BuildLevel2(Scope scope)
    {
        Expression leftNode = BuildLevel3(scope);

        while (IsALevel2Operator(currentToken.Kind))
        {
            TokenKind operation = currentToken.Kind;
            Consume(1);
            Expression rightNode = BuildLevel3(scope);
            leftNode = BuildBinaryExpression(leftNode, operation, rightNode);
        }

        Expression node = leftNode;
        return node;
    }

    private Expression BuildLevel3(Scope scope)
    {
        Expression leftNode = BuildLevel4(scope);

        while (IsALevel3Operator(currentToken.Kind))
        {
            TokenKind operation = currentToken.Kind;
            Consume(1);
            Expression rightNode = BuildLevel4(scope);
            leftNode = BuildBinaryExpression(leftNode, operation, rightNode);
        }

        Expression node = leftNode;
        return node;
    }

    private Expression BuildLevel4(Scope scope)
    {
        Expression leftNode = BuildLevel5(scope);

        while (IsALevel4Operator(currentToken.Kind))
        {
            TokenKind operation = currentToken.Kind;
            Consume(1);
            Expression rightNode = BuildLevel5(scope);
            leftNode = BuildBinaryExpression(leftNode, operation, rightNode);
        }

        Expression node = leftNode;
        return node;
    }

    private Expression BuildLevel5(Scope scope)
    {
        Expression leftNode = GetAtom(scope);

        while (IsALevel5Operator(currentToken.Kind))
        {
            TokenKind operation = currentToken.Kind;
            Consume(1);
            Expression rightNode = GetAtom(scope);
            leftNode = BuildBinaryExpression(leftNode, operation, rightNode);
        }

        Expression node = leftNode;
        return node;
    }

    Expression BuildBinaryExpression(Expression leftNode, TokenKind operation, Expression rightNode)
    {
        switch (operation)
        {
            case TokenKind.And:
                BinaryExpression and = new And(operation, leftNode, rightNode);
                and.CheckNodesSemantic(leftNode, operation, rightNode);
                leftNode = and;
                break;

            case TokenKind.Or:
                BinaryExpression or = new Or(operation, leftNode, rightNode);
                or.CheckNodesSemantic(leftNode, operation, rightNode);
                leftNode = or;
                break;

            case TokenKind.GreatherOrEquals:
                BinaryExpression greatherOrEquals = new GreatherOrEquals(operation, leftNode, rightNode);
                greatherOrEquals.CheckNodesSemantic(leftNode, operation, rightNode);
                leftNode = greatherOrEquals;
                break;

            case TokenKind.GreatherThan:
                BinaryExpression greatherThan = new GreatherThan(operation, leftNode, rightNode);
                greatherThan.CheckNodesSemantic(leftNode, operation, rightNode);
                leftNode = greatherThan;
                break;

            case TokenKind.LessOrEquals:
                BinaryExpression lessOrEquals = new LesserOrEquals(operation, leftNode, rightNode);
                lessOrEquals.CheckNodesSemantic(leftNode, operation, rightNode);
                leftNode = lessOrEquals;
                break;

            case TokenKind.LessThan:
                BinaryExpression lesserThan = new LesserThan(operation, leftNode, rightNode);
                lesserThan.CheckNodesSemantic(leftNode, operation, rightNode);
                leftNode = lesserThan;
                break;

            case TokenKind.EqualsTo:
                BinaryExpression equalsTo = new EqualsTo(operation, leftNode, rightNode);
                equalsTo.CheckNodesSemantic(leftNode, operation, rightNode);
                leftNode = equalsTo;
                break;

            case TokenKind.Addition:
                BinaryExpression addition = new Addition(operation, leftNode, rightNode);
                addition.CheckNodesSemantic(leftNode, operation, rightNode);
                leftNode = addition;
                break;

            case TokenKind.Substraction:
                BinaryExpression substraction = new Substraction(operation, leftNode, rightNode);
                substraction.CheckNodesSemantic(leftNode, operation, rightNode);
                leftNode = substraction;
                break;

            case TokenKind.Concat:
                BinaryExpression concatenation = new Concatenation(operation, leftNode, rightNode);
                leftNode = concatenation;
                break;

            case TokenKind.Multiplication:
                BinaryExpression Multiplication = new Multiplication(operation, leftNode, rightNode);
                Multiplication.CheckNodesSemantic(leftNode, operation, rightNode);
                leftNode = Multiplication;
                break;

            case TokenKind.Division:
                BinaryExpression Division = new Division(operation, leftNode, rightNode);
                Division.CheckNodesSemantic(leftNode, operation, rightNode);
                leftNode = Division;
                break;

            case TokenKind.Modulus:
                BinaryExpression Modulus = new Modulus(operation, leftNode, rightNode);
                Modulus.CheckNodesSemantic(leftNode, operation, rightNode);
                leftNode = Modulus;
                break;

            case TokenKind.Power:
                BinaryExpression power = new Power(operation, leftNode, rightNode);
                power.CheckNodesSemantic(leftNode, operation, rightNode);
                leftNode = power;
                break;
        }

        return leftNode;
    }

    Expression BuildConditionalExpression(Scope localScope)
    {
        IfElse conditionalExpression = new(null!, null!, null!, localScope);
        conditionalExpression.Condition = BuildLevel1(localScope);
        if (currentToken.Kind == TokenKind.ElseKeyWord)
        {
            Console.WriteLine($"!syntax error: if-else expression is incomplete.");
            throw new Exception();
        }
        conditionalExpression.LeftNode = BuildLevel1(localScope);
        Expect(TokenKind.ElseKeyWord);
        conditionalExpression.RightNode = BuildLevel1(localScope);
        return conditionalExpression;
    }

    Expression BuildLetInStructure(Scope localScope)
    {
        Consume(1);
        LetIn expression = new(null!, localScope.MakeChild());
        CreateVar(expression.Scope!);
        Expect(TokenKind.InKeyWord);
        expression.Instruction = BuildLevel1(expression.Scope!);
        return expression;
    }

    void CreateVar(Scope localScope)
    {
        Expect(TokenKind.Identifier);
        string varName = PreviousToken().GetName();
        Expect(TokenKind.Equals);
        localScope.Vars.Add(varName, BuildLevel1(localScope));

        Console.WriteLine(currentToken);

        if (currentToken.Kind == TokenKind.Comma)
        {
            Consume(1);
            CreateVar(localScope);
        }
    }

    private Expression GetAtom(Scope localScope)
    {
        switch (currentToken.Kind)
        {
            // <Basic Data Types
            case TokenKind.Number:
                Number num = new((double)currentToken.GetValue());
                Console.WriteLine($"{currentToken}");
                Consume(1);
                return num;

            case TokenKind.LeftParenthesis:
                Console.WriteLine($"{currentToken}");
                Consume(1);
                Expression expression = BuildLevel1(localScope);
                Console.WriteLine($"{currentToken}");
                Expect(TokenKind.RightParenthesis);
                return expression;

            case TokenKind.String:
                Console.WriteLine($"{currentToken}");
                String str = new((string)currentToken.GetValue());
                Consume(1);
                return str;

            case TokenKind.TrueKeyWord:
                Console.WriteLine($"{currentToken}");
                Bool bool_ = new(true);
                Consume(1);
                return bool_;

            case TokenKind.FalseKeyWord:
                Console.WriteLine($"{currentToken}");
                bool_ = new Bool(false);
                Consume(1);
                return bool_;
            //>

            // <Unary Expressions

            // Negative numbers
            case TokenKind.Substraction:
                Console.WriteLine($"{currentToken.Kind}");
                Consume(1);
                if (NextToken().Kind == TokenKind.Number)
                {
                    Negative negativeNumber = new Negative(BuildLevel3(localScope));
                    negativeNumber.CheckNodeSemantic(negativeNumber.Node);
                    return negativeNumber;
                }
                else
                {
                    Negative negativeNumber = new Negative(GetAtom(localScope));
                    negativeNumber.CheckNodeSemantic(negativeNumber.Node);
                    return negativeNumber;
                }

            // Negative Boolean Expressions
            case TokenKind.Not:
                Console.WriteLine($"{currentToken.Kind}");
                Consume(1);
                if (NextToken().Kind == TokenKind.Number)
                {
                    Not notExpression = new Not(BuildLevel1(localScope));
                    notExpression.CheckNodeSemantic(notExpression.Node);
                    return notExpression;
                }
                else
                {
                    Not notExpression = new Not(GetAtom(localScope));
                    notExpression.CheckNodeSemantic(notExpression.Node);
                    return notExpression;
                }
            //>

            // <Conditional Expressions
            case TokenKind.IfKeyWord:
                Console.WriteLine($"{currentToken}");
                Consume(1);
                return BuildConditionalExpression(localScope);

            case TokenKind.ElseKeyWord:
                Console.WriteLine($"!syntax error: if-else structure is not balanced");
                throw new Exception();
            //>

            // <Assignment Expressions
            case TokenKind.LetKeyWord:
                Console.WriteLine($"{currentToken}");
                return BuildLetInStructure(localScope);
            //>

            // <Variables
            case TokenKind.Identifier:
                Console.WriteLine($"{currentToken}");
                // if (NextToken().Kind == TokenKind.LeftParenthesis) { return FunctionInstance(localScope); }
                Variable variable = new(currentToken.GetName(), localScope);
                variable.CheckSemantic(localScope);
                Consume(1);
                return variable;
            //>

            // <Function declarations
            case TokenKind.FunctionKeyWord:
                throw new NotImplementedException();
            //>

            // <Incomplete expressions
            default:
                Console.WriteLine("Invalid Expression");
                throw new Exception();
                //>
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