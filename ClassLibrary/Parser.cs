// After we made our list of Tokens, we need to find the sense of the expression, if it exists.
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

public class Parser
{
    #region Parser Object
    // </Tokens List to Parse
    private List<Token> tokens;
    // </VarList to keep record of created Variables
    private Dictionary<string, object> variables;
    // </Call Stack
    private List<Funct> stack;


    // </Current Index
    private int currentTokenIndex;
    // </Current Token
    private Token currentToken;
    // </if-else tuples
    private List<(int, int)> ifElseMatches = new List<(int, int)>();

    // </Constructor
    public Parser(List<Token> tokens, Dictionary<string, object> variables, List<Funct> stack)
    {
        this.tokens = tokens;
        this.variables = variables;
        this.stack = stack;
        currentTokenIndex = 0;
        currentToken = tokens[currentTokenIndex];
    }


    // </Eat function: now we can move through our tokens list
    public void Eat(int positions)
    {
        currentTokenIndex += positions;
        if (currentTokenIndex < tokens.Count())
            currentToken = tokens[currentTokenIndex];
        else
            currentToken = new Keyword(TokenKind.EndOfFile);
    }

    public void ClearVariables() => variables.Clear();

    #endregion

    #region Parser Recursive Functions

    private object ParseTerm()
    {
        switch (currentToken.Kind)
        {
            // </Get number value
            case TokenKind.Number:
                object term = currentToken.GetValue();
                Eat(1);
                return (double)term;

            // </Get string value
            case TokenKind.String:
                term = currentToken.GetValue();
                Eat(1);
                return (string)term;

            // </Get false
            case TokenKind.falseKeyWord:
                Eat(1);
                return false;

            // </Get true
            case TokenKind.trueKeyWord:
                Eat(1);
                return true;

            // </Get variable value or evaluate function
            case TokenKind.Identifier:
                string identifierName = currentToken.GetName();

                if (variables.ContainsKey(identifierName))
                {
                    term = variables[identifierName];
                    Eat(1);
                    return term;
                }

                else if (Global.functions.ContainsKey(identifierName))
                {
                    stack.Add(Global.functions[identifierName]);
                    term = EvaluateFunction(stack.Last());
                    Eat(1);
                    return term;
                }

                else
                {
                    Diagnostics.Errors.Add($"!semantic error: function or variable \"{identifierName}\" does not exists.");
                    throw new Exception();
                }

            // </Evaluate conditional expressions
            case TokenKind.ifKeyWord:
                int elseIndex = FindElseIndex();
                int ifIndex = currentTokenIndex;

                // </Check if-else structure balance
                bool balanced = IfMatcher(ifIndex) & ElseMatcher(elseIndex);

                // </Evaluate if expression
                Eat(1);
                bool evaluation = EvaluateIfExpression();

                // </In case if expression returns true
                if (balanced && evaluation)
                {
                    // Execute if instruction
                    object ifInstruction = ParseExpression();
                    currentTokenIndex = tokens.Count() - 1;
                    Eat(1);
                    return ifInstruction;
                }

                // </In case if expression returns false
                else if (balanced && !evaluation)
                {
                    // Move to the corresponding "if" else match
                    StepIntoElse(ifIndex);
                    Eat(1);

                    // Execute else instruction
                    object elseInstruction = ParseExpression();
                    currentTokenIndex = tokens.Count() - 1;
                    Eat(1);
                    return elseInstruction;
                }

                // </In case if-else structure is not balanced
                else
                {
                    Diagnostics.Errors.Add("!syntax error: if-else instructions are not balanced.");
                    throw new Exception();
                }

            // </Expressions cannot start with else keyword
            case TokenKind.elseKeyWord:
                Diagnostics.Errors.Add("!syntax error: if-else instructions are not balanced.");
                throw new Exception();

            // </A left parenthesis means we stepped on a new inner expression
            case TokenKind.LeftParenthesis:
                Eat(1);
                term = ParseExpression();

                if (currentToken.Kind != TokenKind.RightParenthesis)
                {
                    Diagnostics.Errors.Add($"!syntax error: ) is missing after \"{tokens[currentTokenIndex - 1]}\" at index: {currentTokenIndex - 1}.");
                    throw new Exception();
                }

                Eat(1);
                return term;

            case TokenKind.PlusOperator:
                Eat(1);
                term = 0 + (double)ParseTerm();
                return term;

            case TokenKind.MinusOperator:
                Eat(1);
                term = 0 - (double)ParseTerm();
                return term;

            case TokenKind.Not:
                Eat(1);
                term = !(bool)ParseTerm();
                return term;

            // </Create variables
            case TokenKind.letKeyWord:
                CreateVar();
                term = ParseExpression();
                return term;

            // </In keyword means we stepped on a new expression 
            case TokenKind.inKeyWord:
                Eat(1);
                term = ParseExpression();
                return term;

            default:
                if (currentTokenIndex == 0)
                {
                    Diagnostics.Errors.Add($"!syntax error: factor or expression is missing before \"{tokens[currentTokenIndex]}\" at index: {currentTokenIndex}.");
                    throw new Exception();
                }
                else
                {
                    Diagnostics.Errors.Add($"!syntax error: factor or expression is missing after \"{tokens[currentTokenIndex - 1]}\" at index: {currentTokenIndex - 1}.");
                    throw new Exception();
                }
        }
    }

    private object ParsePower()
    {

        object factor = ParseTerm();

        if (factor is string)
            return factor;

        else if (factor is null)
            return factor!;

        else if (factor is bool)
            return factor;

        else
        {
            double _factor = (double)factor;
            while (currentToken.Kind == TokenKind.Power)
            {
                Token operatorToken = currentToken;
                Eat(1);
                object nextToken = ParsePower();

                if (currentToken.Kind == TokenKind.LeftParenthesis && nextToken is double)
                {
                    _factor = Math.Pow((double)_factor, (double)nextToken);
                }
                else if (currentToken.Kind != TokenKind.LeftParenthesis && nextToken is double)
                {
                    _factor = Math.Pow((double)_factor, (double)nextToken);
                }
                else
                {
                    Diagnostics.Errors.Add($"!semantic error: \"{operatorToken}\" cannot be used between strings and numbers.");
                    throw new InvalidOperationException();
                }
            }
            return _factor;
        }
    }

    private object ParseFactor()
    {
        object factor = ParsePower();
        if (factor is string)
            return factor;

        else if (factor is null)
            return factor!;

        else if (factor is bool)
            return factor;

        else
        {
            double _factor = (double)factor;
            while (currentToken.Kind == TokenKind.MultOperator || currentToken.Kind == TokenKind.DivideOperator || currentToken.Kind == TokenKind.Modulus)
            {
                Token operatorToken = currentToken;
                Eat(1);

                object nextToken = ParseFactor();

                if (operatorToken.Kind == TokenKind.MultOperator && nextToken is double)
                    _factor *= (double)nextToken;

                else if (operatorToken.Kind == TokenKind.DivideOperator && nextToken is double)
                    _factor /= (double)nextToken;

                else if (operatorToken.Kind == TokenKind.Modulus && nextToken is double)
                    _factor %= (double)nextToken;

                else
                {
                    Diagnostics.Errors.Add($"!semantic error: \"{operatorToken}\" cannot be used between strings and numbers.");
                    throw new Exception();
                }
            }
            return _factor;
        }
    }

    private object ParseSum()
    {
        object expressionResult = ParseFactor();

        if (expressionResult is string)
            return expressionResult;

        else if (expressionResult is null)
            return expressionResult!;

        else if (expressionResult is bool)
            return expressionResult;

        else
        {
            double _expressionResult = (double)expressionResult;
            while (currentToken.Kind == TokenKind.PlusOperator || currentToken.Kind == TokenKind.MinusOperator)
            {
                Token operatorToken = currentToken;
                Eat(1);

                object nextToken = ParseSum();

                if (operatorToken.Kind == TokenKind.PlusOperator && nextToken is double)
                    _expressionResult += (double)nextToken;

                else if (operatorToken.Kind == TokenKind.MinusOperator && nextToken is double)
                    _expressionResult -= (double)nextToken;

                else
                {
                    Diagnostics.Errors.Add($"!semantic error: \"{operatorToken}\" cannot be used between number and string index: {currentTokenIndex - 1}.");
                    throw new Exception();
                }
            }

            return _expressionResult;
        }
    }

    private object ParseComparation()
    {
        object leftExpression = ParseSum();

        if (leftExpression is string)
            return leftExpression;

        else if (leftExpression is null)
            return leftExpression!;

        else if (leftExpression is bool)
            return leftExpression;

        else
        {
            bool evaluation;

            if (leftExpression is bool)
                return (bool)leftExpression;

            TokenKind _operator = currentToken.Kind;


            if (!IsBooleanOperator(_operator))
            {
                return leftExpression;
            }

            else
            {
                Eat(1);
                object rightExpression = ParseSum();


                switch (_operator)
                {
                    case TokenKind.LessThan:
                        evaluation = (double)leftExpression < (double)rightExpression;
                        break;

                    case TokenKind.LessOrEquals:
                        evaluation = (double)leftExpression <= (double)rightExpression;
                        break;

                    case TokenKind.GreatherThan:
                        evaluation = (double)leftExpression > (double)rightExpression;
                        break;

                    case TokenKind.GreatherOrEquals:
                        evaluation = (double)leftExpression >= (double)rightExpression;
                        break;

                    case TokenKind.EqualsTo:
                        evaluation = leftExpression.ToString() == rightExpression.ToString();
                        break;

                    case TokenKind.NotEquals:
                        evaluation = leftExpression.ToString() != rightExpression.ToString();
                        break;

                    default:
                        Diagnostics.Errors.Add($"!syntax error: invalid conditional expression after \"{tokens[currentTokenIndex - 1]}\" at index: {currentTokenIndex - 1}.");
                        throw new Exception();
                }

                return evaluation;
            }
        }
    }

    private object ParseAndOr()
    {
        object leftExpression = ParseComparation();
        if (!(leftExpression is bool))
            return leftExpression;

        bool _leftExpression = (bool)leftExpression;

        if (leftExpression is null)
            return leftExpression!;

        while (currentToken.Kind == TokenKind.Or || currentToken.Kind == TokenKind.And)
        {
            TokenKind _operator = currentToken.Kind;
            Eat(1);

            bool rightExpression = (bool)ParseComparation();

            if (_operator == TokenKind.Or)
            {
                _leftExpression |= rightExpression;
            }
            if (_operator == TokenKind.And)
            {
                _leftExpression &= rightExpression;
            }
        }
        return _leftExpression;
    }


    public object ParseExpression()
    {
        if (currentToken.Kind == TokenKind.functionKeyWord)
        {
            CreateFunction(Global.functions);
            return null!;
        }

        object expressionResult = ParseAndOr();

        if (expressionResult is string && (currentToken.Kind == TokenKind.PlusOperator || currentToken.Kind == TokenKind.MinusOperator || currentToken.Kind == TokenKind.MultOperator || currentToken.Kind == TokenKind.DivideOperator || currentToken.Kind == TokenKind.Power))
        {
            Diagnostics.Errors.Add($"!semantic: error \"{currentToken}\" cannot be used between string and number index: {currentTokenIndex - 1}");
            throw new Exception();
        }

        while (currentToken.Kind == TokenKind.Concat)
        {
            Eat(1);
            string stringExpression = expressionResult.ToString()!;
            stringExpression += ParseExpression().ToString();
            return stringExpression;
        }
        return expressionResult;
    }

    #region Boolean Expressions Evaluator

    bool EvaluateIfExpression()
    {
        if (currentToken.Kind != TokenKind.LeftParenthesis)
        {
            Diagnostics.Errors.Add($"!syntax error: \"Left Parenthesis\" expected after \"{tokens[currentTokenIndex - 1]}\" at index: {currentTokenIndex - 1}.");
            throw new Exception();
        }

        Eat(1);

        bool evaluation = (bool)ParseAndOr();

        if (currentToken.Kind == TokenKind.RightParenthesis)
        {
            Eat(1);
            return evaluation;
        }
        else
        {
            Diagnostics.Errors.Add($"!syntax error: \"Right Parenthesis\" expected after \"{tokens[currentTokenIndex - 1]}\" at index: {currentTokenIndex - 1}.");
            throw new Exception();
        }
    }

    bool IfMatcher(int ifIndex)
    {
        for (int i = ifIndex + 1; i < tokens.Count(); i++)
        {
            if (tokens[i].Kind == TokenKind.ifKeyWord)
            {
                IfMatcher(i);
                i = ifElseMatches.Last().Item2 + 1;
            }

            if (tokens[i].Kind == TokenKind.elseKeyWord)
            {
                ifElseMatches.Add((ifIndex, i));
                return true;
            }
        }
        return false;
    }

    bool ElseMatcher(int elseIndex)
    {
        for (int i = elseIndex - 1; i >= 0; i--)
        {
            if (tokens[i].Kind == TokenKind.elseKeyWord)
            {
                ElseMatcher(i);
                i = ifElseMatches.Last().Item1 - 1;
            }

            if (tokens[i].Kind == TokenKind.ifKeyWord)
            {
                ifElseMatches.Add((i, elseIndex));
                return true;
            }
        }
        return false;
    }

    int FindElseIndex()
    {
        int elseIndex = 0;
        for (int i = tokens.Count() - 1; i > 0; i--)
        {
            if (tokens[i].Kind == TokenKind.elseKeyWord)
            {
                elseIndex = i;
                break;
            }
        }
        return elseIndex;
    }

    void StepIntoElse(int ifIndex)
    {
        foreach (var match in ifElseMatches)
        {
            if (ifIndex == match.Item1)
            {
                currentTokenIndex = match.Item2;
            }
        }
    }

    #endregion

    #endregion

    #region Parser Main Function
    public void Parse()
    {
        if (tokens.Count() == 0)
        {
            variables.Clear();
            Diagnostics.Errors.Add("there is nothing to parse.");
            throw new Exception();
        }

        else if (tokens.Count() == 1)
        {
            variables.Clear();
            Diagnostics.Errors.Add($"!syntax error: invalid expression.");
            throw new Exception();

        }

        else if (tokens.Count() > 1)
        {
            object result = ParseExpression();

            if (!(result is null))
                Console.WriteLine(result);

            if (currentToken.Kind != TokenKind.Semicolon && currentToken.Kind != TokenKind.EndOfFile)
            {
                variables.Clear();
                Diagnostics.Errors.Add($"!syntax error: operator or expression is missing after \"{tokens[currentTokenIndex - 1]}\" at index: {currentTokenIndex - 1}.");
                throw new Exception();
            }
        }
    }

    #endregion

    bool IsBooleanOperator(TokenKind _operator)
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

        return operators.Contains(_operator);

    }

    #region Variables and Functions Creation

    // </Create Variable Utility Function
    private void CreateVar()
    {
        Eat(1);
        if (currentToken.Kind != TokenKind.Identifier)
        {
            Diagnostics.Errors.Add($"!syntax error: variable not defined after: \"{tokens[currentTokenIndex - 1]}\" at index: {currentTokenIndex - 1}.");
            throw new Exception();
        }

        string varName = currentToken.GetName();

        Eat(1);
        if (currentToken.Kind != TokenKind.Equals)
        {
            Diagnostics.Errors.Add($"!syntax error: = is missing after \"{tokens[currentTokenIndex - 1].GetName()}\" at index: {currentTokenIndex - 1}.");
            throw new Exception();
        }

        Eat(1);
        if (currentToken.Kind != TokenKind.Number && currentToken.Kind != TokenKind.String && currentToken.Kind != TokenKind.Identifier && currentToken.Kind != TokenKind.LeftParenthesis && currentToken.Kind != TokenKind.falseKeyWord && currentToken.Kind != TokenKind.trueKeyWord && currentToken.Kind != TokenKind.MinusOperator)
        {
            Diagnostics.Errors.Add($"!syntax error: variables must have a value.");
            throw new Exception();
        }

        object varValue = ParseExpression();

        if (!variables.ContainsKey(varName))
            variables.Add(varName, varValue);

        else
            variables[varName] = varValue;

        if (currentToken.Kind == TokenKind.Comma)
            CreateVar();

        if (currentToken.Kind != TokenKind.inKeyWord)
        {
            Diagnostics.Errors.Add($"!syntax error: \"in\" is missing after \"{tokens[currentTokenIndex - 1]}\" at index: {currentTokenIndex - 1}.");
            throw new Exception();
        }
    }

    public void CreateFunction(Dictionary<string, Funct> functions)
    {
        // </Initialize function parameters
        string functionName;
        List<(string, object)> args = new List<(string, object)>();
        List<Token> body = new List<Token>();

        Eat(1);
        if (currentToken.Kind != TokenKind.Identifier)
        {
            Diagnostics.Errors.Add($"!syntax error: function name is not declared after \"{tokens[currentTokenIndex - 1]}\".");
            throw new Exception();
        }

        // </Get function name
        functionName = currentToken.GetName();

        Eat(1);

        if (currentToken.Kind != TokenKind.LeftParenthesis)
        {
            Diagnostics.Errors.Add($"!syntax error: \"Right Parenthesis\" is missing after \"{tokens[currentTokenIndex - 1]}\".");
            throw new Exception();
        }

        Eat(1);

        // </Get function arguments
        while (currentToken.Kind != TokenKind.RightParenthesis)
        {
            if (currentToken.Kind != TokenKind.Identifier)
            {
                Diagnostics.Errors.Add($"!syntax error: \"{currentToken}\" is not a valid argument at index {currentTokenIndex}");
                throw new Exception();
            }

            args.Add((currentToken.GetName(), null!));
            Eat(1);

            if (currentToken.Kind == TokenKind.Comma)
                Eat(1);
        }

        Eat(1);

        if (currentToken.Kind != TokenKind.Arrow)
        {
            Diagnostics.Errors.Add($"!syntax error: \"=>\" is missing after \"{tokens[currentTokenIndex - 1]}\".");
            throw new Exception();
        }

        Eat(1);

        // </Get function body
        while (currentToken.Kind != TokenKind.Semicolon)
        {
            body.Add(currentToken);
            Eat(1);
        }

        body.Add(new CommonToken(TokenKind.Semicolon, ";"));

        // </Build function
        Funct function = new Funct(args, body);

        if (!functions.ContainsKey(functionName))
        {
            functions.Add(functionName, function);
            Console.WriteLine($"!function: \"{functionName}\" created.");
        }

        else
        {
            functions[functionName] = function;
            Console.WriteLine($"!old function: \"{functionName}\" edited.");
        }
    }

    #endregion

    #region Function Evaluation 
    private object EvaluateFunction(Funct function)
    {
        Eat(1);

        if (currentToken.Kind != TokenKind.LeftParenthesis)
        {
            Diagnostics.Errors.Add($"!syntax error: \"Left Parenthesis\" is missing after \"{tokens[currentTokenIndex - 1]}\".");
            throw new Exception();
        }

        Eat(1);

        // </Get function argument values
        int index = 0;
        while (currentToken.Kind != TokenKind.RightParenthesis)
        {
            if (index == function.Args.Count)
                break;

            object seconditem = ParseExpression();
            function.Args[index] = (function.Args[index].Item1, seconditem);

            index++;

            if (currentToken.Kind == TokenKind.Comma)
                Eat(1);

        }

        // </Execute function body
        object evaluation = function.Execute();

        if (currentToken.Kind != TokenKind.RightParenthesis)
        {
            Diagnostics.Errors.Add($"!syntax error: \"Right Parenthesis\" is missing after \"{tokens[currentTokenIndex - 1]}\" at index {currentTokenIndex}");
            throw new Exception();
        }

        return evaluation;
    }
    #endregion
}