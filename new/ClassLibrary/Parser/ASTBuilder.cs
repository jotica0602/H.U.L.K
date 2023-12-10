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
                BinaryExpression andNode = new And(operation, leftNode, rightNode);
                andNode.CheckNodesSemantic(leftNode, operation, rightNode);
                leftNode = andNode;
                break;

            case TokenKind.Or:
                BinaryExpression orNode = new Or(operation, leftNode, rightNode);
                orNode.CheckNodesSemantic(leftNode, operation, rightNode);
                leftNode = orNode;
                break;

            case TokenKind.GreatherOrEquals:
                BinaryExpression greatherOrEqualsNode = new GreatherOrEquals(operation, leftNode, rightNode);
                greatherOrEqualsNode.CheckNodesSemantic(leftNode, operation, rightNode);
                leftNode = greatherOrEqualsNode;
                break;

            case TokenKind.GreatherThan:
                BinaryExpression greatherThanNode = new GreatherThan(operation, leftNode, rightNode);
                greatherThanNode.CheckNodesSemantic(leftNode, operation, rightNode);
                leftNode = greatherThanNode;
                break;

            case TokenKind.LessOrEquals:
                BinaryExpression lessOrEqualsNode = new LesserOrEquals(operation, leftNode, rightNode);
                lessOrEqualsNode.CheckNodesSemantic(leftNode, operation, rightNode);
                leftNode = lessOrEqualsNode;
                break;

            case TokenKind.LessThan:
                BinaryExpression lesserThanNode = new LesserThan(operation, leftNode, rightNode);
                lesserThanNode.CheckNodesSemantic(leftNode, operation, rightNode);
                leftNode = lesserThanNode;
                break;

            case TokenKind.EqualsTo:
                BinaryExpression equalsTo = new EqualsTo(operation, leftNode, rightNode);
                equalsTo.CheckNodesSemantic(leftNode, operation, rightNode);
                leftNode = equalsTo;
                break;

            case TokenKind.Addition:
                BinaryExpression additionNode = new Addition(operation, leftNode, rightNode);
                additionNode.CheckNodesSemantic(leftNode, operation, rightNode);
                leftNode = additionNode;
                break;

            case TokenKind.Substraction:
                BinaryExpression substractionNode = new Substraction(operation, leftNode, rightNode);
                substractionNode.CheckNodesSemantic(leftNode, operation, rightNode);
                leftNode = substractionNode;
                break;

            case TokenKind.Concat:
                BinaryExpression concatenationNode = new Concatenation(operation, leftNode, rightNode);
                leftNode = concatenationNode;
                break;

            case TokenKind.Multiplication:
                BinaryExpression multiplicationNode = new Multiplication(operation, leftNode, rightNode);
                multiplicationNode.CheckNodesSemantic(leftNode, operation, rightNode);
                leftNode = multiplicationNode;
                break;

            case TokenKind.Division:
                BinaryExpression divisionNode = new Division(operation, leftNode, rightNode);
                divisionNode.CheckNodesSemantic(leftNode, operation, rightNode);
                leftNode = divisionNode;
                break;

            case TokenKind.Modulus:
                BinaryExpression modulusNode = new Modulus(operation, leftNode, rightNode);
                modulusNode.CheckNodesSemantic(leftNode, operation, rightNode);
                leftNode = modulusNode;
                break;

            case TokenKind.Power:
                BinaryExpression powerNode = new Power(operation, leftNode, rightNode);
                powerNode.CheckNodesSemantic(leftNode, operation, rightNode);
                leftNode = powerNode;
                break;
        }

        return leftNode;
    }

    Expression BuildConditionalExpression(Scope localScope)
    {
        Consume(1);
        IfElse conditionalExpression = new(null!, null!, null!);
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
        LetIn letInExpression = new(null!, localScope.MakeChild());
        CreateVar(letInExpression.Scope!);
        Expect(TokenKind.InKeyWord);
        letInExpression.Instruction = BuildLevel1(letInExpression.Scope!);
        return letInExpression;
    }

    void CreateVar(Scope letInScope)
    {
        Expect(TokenKind.Identifier);
        string varName = PreviousToken().GetName();
        Expect(TokenKind.Equals);
        letInScope.Vars.Add(varName, BuildLevel1(letInScope));

        Console.WriteLine(currentToken);

        if (currentToken.Kind == TokenKind.Comma)
        {
            Consume(1);
            CreateVar(letInScope);
        }
    }

    private void BuildFunction()
    {
        Consume(1);
        Expect(TokenKind.Identifier);

        string functionName = PreviousToken().GetName();
        FunctionBody body = new FunctionBody();

        Expect(TokenKind.LeftParenthesis);
        SetArgs(body.ArgsVars!);
        Expect(TokenKind.RightParenthesis);
        Expect(TokenKind.Arrow);

        for (int i = currentTokenIndex; i < tokens.Count; i++)
            body.Tokens!.Add(tokens[i]);

        GlobalScope.Functions.Add(functionName, body);
        Consume(tokens.Count - currentTokenIndex - 1);
    }

    private void SetArgs(List<string> argsVariables)
    {
        Expect(TokenKind.Identifier);
        argsVariables.Add(PreviousToken().GetName());
        if (currentToken.Kind == TokenKind.Comma)
        {
            Consume(1);
            SetArgs(argsVariables);
        }
    }

    private Function FunctionInstance(Scope localScope)
    {
        string functId = currentToken.GetName();
        FunctionCall foo = new FunctionCall(functId, new List<Expression>(), GlobalScope);
        foo.CheckSemantic();
        Consume(2);
        GetArgs(localScope, foo.ArgsValues);
        foo.CheckArgsCount(localScope);
        Expect(TokenKind.RightParenthesis);
        return foo;
    }

    private void GetArgs(Scope localScope, List<Expression> arguments)
    {
        arguments.Add(BuildLevel1(localScope));
        if (currentToken.Kind == TokenKind.Comma)
        {
            Consume(1);
            GetArgs(localScope, arguments);
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
                if (NextToken().Kind == TokenKind.LeftParenthesis) { return FunctionInstance(localScope); }
                Variable variable = new(currentToken.GetName());
                variable.CheckSemantic(localScope);
                Consume(1);
                return variable;
            //>

            // <Function declarations
            case TokenKind.FunctionKeyWord:
                BuildFunction();
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