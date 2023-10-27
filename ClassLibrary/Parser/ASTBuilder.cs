using System.ComponentModel;
using System.ComponentModel.Design;
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
            leftNode = BuildBinaryExpression(leftNode, operation, rightNode, scope);
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
            leftNode = BuildBinaryExpression(leftNode, operation, rightNode, scope);
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
            leftNode = BuildBinaryExpression(leftNode, operation, rightNode, scope);
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
            leftNode = BuildBinaryExpression(leftNode, operation, rightNode, scope);
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
            leftNode = BuildBinaryExpression(leftNode, operation, rightNode, scope);
        }

        Expression node = leftNode;
        return node;
    }


    Expression BuildBinaryExpression(Expression leftNode, TokenKind operation, Expression rightNode, Scope scope)
    {
        switch (operation)
        {
            case TokenKind.And:
                BinaryExpression and = new And(ExpressionKind.Bool, operation, leftNode, rightNode, scope.MakeChild());
                and.CheckNodesSemantic(leftNode, operation, rightNode);
                leftNode = and;
                break;

            case TokenKind.Or:
                BinaryExpression or = new Or(ExpressionKind.Bool, operation, leftNode, rightNode, scope.MakeChild());
                or.CheckNodesSemantic(leftNode, operation, rightNode);
                leftNode = or;
                break;

            case TokenKind.GreatherOrEquals:
                BinaryExpression greatherOrEquals = new GreatherOrEquals(ExpressionKind.Bool, operation, leftNode, rightNode, scope.MakeChild());
                greatherOrEquals.CheckNodesSemantic(leftNode, operation, rightNode);
                leftNode = greatherOrEquals;
                break;

            case TokenKind.GreatherThan:
                BinaryExpression greatherThan = new GreatherThan(ExpressionKind.Bool, operation, leftNode, rightNode, scope.MakeChild());
                greatherThan.CheckNodesSemantic(leftNode, operation, rightNode);
                leftNode = greatherThan;
                break;

            case TokenKind.LessOrEquals:
                BinaryExpression lessOrEquals = new LesserOrEquals(ExpressionKind.Bool, operation, leftNode, rightNode, scope.MakeChild());
                lessOrEquals.CheckNodesSemantic(leftNode, operation, rightNode);
                leftNode = lessOrEquals;
                break;

            case TokenKind.LessThan:
                BinaryExpression lesserThan = new LesserThan(ExpressionKind.Bool, operation, leftNode, rightNode, scope.MakeChild());
                lesserThan.CheckNodesSemantic(leftNode, operation, rightNode);
                leftNode = lesserThan;
                break;

            case TokenKind.EqualsTo:
                BinaryExpression equalsTo = new EqualsTo(ExpressionKind.Bool, operation, leftNode, rightNode, scope.MakeChild());
                equalsTo.CheckNodesSemantic(leftNode, operation, rightNode);
                leftNode = equalsTo;
                break;

            case TokenKind.Addition:
                BinaryExpression addition = new Addition(ExpressionKind.Number, operation, leftNode, rightNode, scope.MakeChild());
                addition.CheckNodesSemantic(leftNode, operation, rightNode);
                leftNode = addition;
                break;

            case TokenKind.Substraction:
                BinaryExpression substraction = new Substraction(ExpressionKind.Number, operation, leftNode, rightNode, scope.MakeChild());
                substraction.CheckNodesSemantic(leftNode, operation, rightNode);
                leftNode = substraction;
                break;

            case TokenKind.Concat:
                BinaryExpression concatenation = new Concat(ExpressionKind.String, operation, leftNode, rightNode, scope.MakeChild());
                leftNode = concatenation;
                break;

            case TokenKind.Multiplication:
                BinaryExpression Multiplication = new Multiplication(ExpressionKind.Number, operation, leftNode, rightNode, scope.MakeChild());
                Multiplication.CheckNodesSemantic(leftNode, operation, rightNode);
                leftNode = Multiplication;
                break;

            case TokenKind.Division:
                BinaryExpression Division = new Division(ExpressionKind.Number, operation, leftNode, rightNode, scope.MakeChild());
                Division.CheckNodesSemantic(leftNode, operation, rightNode);
                leftNode = Division;
                break;

            case TokenKind.Modulus:
                BinaryExpression Modulus = new Modulus(ExpressionKind.Number, operation, leftNode, rightNode, scope.MakeChild());
                Modulus.CheckNodesSemantic(leftNode, operation, rightNode);
                leftNode = Modulus;
                break;

            case TokenKind.Power:
                BinaryExpression power = new Power(ExpressionKind.Number, operation, leftNode, rightNode, scope.MakeChild());
                power.CheckNodesSemantic(leftNode, operation, rightNode);
                leftNode = power;
                break;
        }

        return leftNode;
    }

    Expression BuildConditionalExpression(IfElse conditionalExpression, Scope localScope)
    {
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
        LetIn expression = new(ExpressionKind.Temp, null!, localScope.MakeChild());
        CreateVar(expression.Scope!);
        Expect(TokenKind.InKeyWord);
        expression.Instruction = BuildLevel1(expression.Scope!);
        return expression;
    }

    void CreateVar(Scope localScope)
    {
        Expect(TokenKind.Identifier);
        string varName = tokens[currentTokenIndex - 1].GetName();
        Expect(TokenKind.Equals);
        localScope.Vars.Add(varName, BuildLevel1(localScope));

        Console.WriteLine(currentToken);

        if (currentToken.Kind == TokenKind.Comma)
        {
            Consume(1);
            CreateVar(localScope);
        }
    }

    // void BuildFunction(Scope localScope)
    // {
    //     Consume(1);
    //     Expect(TokenKind.Identifier);

    //     string functionName = tokens[currentTokenIndex - 1].GetName();
    //     Scope functionScope = new Scope();
    //     functionScope.Parent = GlobalScope;
    //     List<string> args = new List<string>();
    //     Function foo = new Function(ExpressionKind.Temp, functionScope, functionName, null!);
    //     GlobalScope.Functions.Add(functionName, foo);

    //     Expect(TokenKind.LeftParenthesis);
    //     GetArgs(functionScope);
    //     Expect(TokenKind.RightParenthesis);
    //     Expect(TokenKind.Arrow);

    //     Expression functionBody = BuildLevel1(functionScope);
    //     foo.Body = functionBody;
    //     GlobalScope.Functions.Remove(functionName);
    //     GlobalScope.Functions.Add(functionName, foo);
    // }

    private void GetArgs(Scope functionScope)
    {
        Expect(TokenKind.Identifier);
        functionScope.Vars.Add(tokens[currentTokenIndex - 1].GetName(), null!);
        if (currentToken.Kind == TokenKind.Comma)
        {
            Consume(1);
            GetArgs(functionScope);
        }
    }

    // Expression FunctionInstance(Scope localScope)
    // {
    //     string functionName = currentToken.GetName();
    //     List<Expression> argsValues = new List<Expression>();
    //     List<string> argsKeys = new List<string>();
    //     Scope functionScope = new Scope();
    //     functionScope.Parent = GlobalScope;
    //     Consume(2);
    //     SetArgs(localScope, ref argsValues);
    //     Expect(TokenKind.RightParenthesis);
    //     Function functionInstance = new Function(ExpressionKind.Temp, functionScope, functionName, null!);
    //     functionInstance.CheckSemantic(GlobalScope, functionName);
    //     MatchArgs(functionInstance, functionName, argsKeys, argsValues);

    //     return functionInstance;
    // }

    // private void SetArgs(Scope localScope, ref List<Expression> args)
    // {
    //     args.Add(BuildLevel1(localScope));
    //     if (currentToken.Kind == TokenKind.Comma)
    //     {
    //         Consume(1);
    //         SetArgs(localScope, ref args);
    //     }
    // }

    // private void MatchArgs(Function functionInstance, string functionName, List<string> argsKeys, List<Expression> argsValues)
    // {
    //     foreach (var arg in GlobalScope.Functions[functionName].Scope!.Vars)
    //     {
    //         argsKeys.Add(arg.Key);
    //     }

    //     for (int i = 0; i < argsKeys.Count; i++)
    //         functionInstance.Scope!.Vars.Add(argsKeys[i], argsValues[i]);

    //     if (argsValues.Count != argsKeys.Count)
    //     {
    //         Console.WriteLine($"!semantic error: function \"{functionName}\" recieves {argsKeys.Count} argument(s) but {argsValues.Count} were given.");
    //         throw new Exception();
    //     }

    // }

    private Expression GetAtom(Scope localScope)
    {
        switch (currentToken.Kind)
        {

            // <Basic Data Types
            case TokenKind.Number:
                Number num = new(ExpressionKind.Number, (double)currentToken.GetValue());
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
                String str = new(ExpressionKind.String, (string)currentToken.GetValue(), null!);
                Consume(1);
                return str;

            case TokenKind.TrueKeyWord:
                Console.WriteLine($"{currentToken}");
                Bool bool_ = new(ExpressionKind.Bool, true, null!);
                Consume(1);
                return bool_;

            case TokenKind.FalseKeyWord:
                Console.WriteLine($"{currentToken}");
                bool_ = new Bool(ExpressionKind.Bool, false, null!);
                Consume(1);
                return bool_;
            //>

            // <Unary Expressions
            case TokenKind.Substraction:
                Console.WriteLine($"{currentToken.Kind}");
                Negative negativeNumber = new Negative(ExpressionKind.Number, localScope.MakeChild(), null!);
                Consume(1);
                negativeNumber.Node = BuildLevel3(negativeNumber.Scope!);
                negativeNumber.CheckNodeSemantic(negativeNumber.Node);
                return negativeNumber;

            case TokenKind.Not:
                Console.WriteLine($"{currentToken.Kind}");
                Not notExpression = new Not(ExpressionKind.Number, localScope.MakeChild(), null!);
                Consume(1);
                notExpression.Node = BuildLevel1(notExpression.Scope!);
                notExpression.CheckNodeSemantic(notExpression.Node);
                return notExpression;
            //>

            // <Conditional Expressions
            case TokenKind.IfKeyWord:
                Console.WriteLine($"{currentToken}");
                Consume(1);
                IfElse conditionalExpression = new(ExpressionKind.Temp, null!, null!, null!, localScope);
                return BuildConditionalExpression(conditionalExpression, localScope);

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
                // if (tokens[currentTokenIndex + 1].Kind == TokenKind.LeftParenthesis) { return FunctionInstance(localScope); }
                Variable variable = new(ExpressionKind.Temp, currentToken.GetName(), localScope);
                variable.CheckSemantic(localScope);
                Consume(1);
                return variable;
            //>

            // <Function declarations
            case TokenKind.FunctionKeyWord:
                // BuildFunction(localScope);
                return null!;
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