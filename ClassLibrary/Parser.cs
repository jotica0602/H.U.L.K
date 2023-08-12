// After we made our list of Tokens, we need to find the sense of the expression, if it exists.
public class Parser
{
    #region Parser Object
    // This is the List
    private List<Token> tokens;
    // Current Index
    private int currentTokenIndex;
    // Current Token
    private Token currentToken;
    // VarList to keep record of created Variables
    public List<Token> variables = new List<Token>();
    // if-else tuples
    public static List<(int, int)> ifElseMatches = new List<(int, int)>();

    // Previous Token
    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
        currentTokenIndex = 0;
        currentToken = tokens[currentTokenIndex];
    }

    // Now we can move through our Tokens List
    public void Next(int positions)
    {
        currentTokenIndex += positions;
        if (currentTokenIndex < tokens.Count())
            currentToken = tokens[currentTokenIndex];
        else
            currentToken = new Token(TokenKind.EndOfFile, "", null!);
    }

    public TokenKind LookAhead(int positions)
    {
        return tokens[currentTokenIndex + positions].Kind;
    }

    #endregion

    #region Parser Recursive Functions

    private object ParseFactor()
    {
        // if the token we are looking at is a number, we parse it, return it and move to the next
        if (currentToken.Kind == TokenKind.Number)
        {
            double factor = (double)currentToken.Value;
            Next(1);
            return factor;
        }

        // same with strings
        else if (currentToken.Kind == TokenKind.String)
        {
            string factor = (string)currentToken.Value;
            Next(1);
            return factor;
        }

        else if (currentToken.Kind == TokenKind.falseKeyWord)
        {
            Next(1);
            return false;
        }
        else if (currentToken.Kind == TokenKind.trueKeyWord)
        {
            Next(1);
            return true;
        }
        // if there is an identifier, we check if it is an existent variable and return its value
        else if (currentToken.Kind == TokenKind.Identifier)
        {
            object factor = null!;
            bool exists = false;
            foreach (var variable in variables)
            {
                if (currentToken.Name == variable.Name)
                {
                    exists = true;
                    factor = currentToken.Value;
                    currentToken.Kind = TokenKind.Variable;
                    break;
                }
            }

            Next(1);
            if (exists == false)
            {
                Diagnostics.Errors.Add($"!semantic error: variable \"{tokens[currentTokenIndex - 1].Name}\" does not exists ");
                throw new Exception();
            }
            else return factor;
        }

        else if (currentToken.Kind == TokenKind.ifKeyWord)
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
            int ifIndex = currentTokenIndex;

            bool balanced = IfMatcher(ifIndex) & ElseMatcher(elseIndex);

            if (balanced == true)
            {
                Next(1);
                bool evaluation = EvaluateIfExpression();
                if (evaluation == true)
                {
                    object ifInstruction = ParseExpression();
                    currentTokenIndex = tokens.Count() - 2;
                    Next(1);
                    return ifInstruction;
                }
                else
                {
                    foreach (var match in ifElseMatches)
                    {
                        if (ifIndex == match.Item1)
                        {
                            currentTokenIndex = match.Item2;
                        }
                    }
                    Next(1);
                    object elseInstruction = ParseExpression();
                    currentTokenIndex = tokens.Count() - 2;
                    Next(1);
                    return elseInstruction;
                }
            }
            else
            {
                Diagnostics.Errors.Add("!syntax error: if-else instructions are not balanced");
                throw new Exception();
            }
        }

        else if (currentToken.Kind == TokenKind.elseKeyWord)
        {
            Diagnostics.Errors.Add("!syntax error: if-else instructions are not balanced");
            throw new Exception();
        }

        // in keyword means that we evaluating an expression
        else if (currentToken.Kind == TokenKind.inKeyWord)
        {
            Next(1);
            object factor = ParseExpression();
            return factor;
        }

        // let keyword means that we are declaring a variable
        else if (currentToken.Kind == TokenKind.letKeyWord)
        {
            CreateVar(variables);

            // after creating vars, we stepped into in keyword
            // and need to parse the next 1expression

            object factor = ParseExpression();

            return factor;
        }
        // if we find a left parenthesis we will parse the next 1expression until we find a right parenthesis, if we don't find any
        // it will be a syntax.
        else if (currentToken.Kind == TokenKind.LeftParenthesis)
        {
            Next(1);
            object factor = ParseExpression();
            if (currentToken.Kind != TokenKind.RightParenthesis)
            {
                Diagnostics.Errors.Add($"!syntax error: ) is missing after \"{tokens[currentTokenIndex - 1]}\" at index: {currentTokenIndex - 1}");
                throw new Exception();
            }
            Next(1);
            return factor;
        }

        // for negative arithmetic expressions
        else if (currentToken.Kind == TokenKind.MinusOperator)
        {
            Next(1);
            double factor = 0 - (double)ParseFactor();
            return factor;
        }
        // for missing expressions to operate with
        else
        {
            Diagnostics.Errors.Add($"!syntax error: factor or expression is missing after \"{tokens[currentTokenIndex - 1]}\" at index: {currentTokenIndex - 1}");
            throw new Exception();
        }
    }
    
    private object _ParseTerm()
    {
        object term = ParseFactor();
        if (term is string)
            return term;
        else if (term is bool)
            return term;
        else
        {
            double _term = (double)term;
            while (currentToken.Kind == TokenKind.Power)
            {
                Token operatorToken = currentToken;
                Next(1);
                object nextToken = _ParseExpression();

                if (operatorToken.Kind != TokenKind.LeftParenthesis && nextToken is double)
                    _term = Math.Pow((double)_term, (double)nextToken);
                else
                {
                    Diagnostics.Errors.Add($"!semantic error: \"{operatorToken}\" cannot be used between string and number index: {currentTokenIndex - 1}");
                    throw new InvalidOperationException();
                }
            }
            return _term;
        }
    }

    private object ParseTerm()
    {
        object term = _ParseTerm();
        if (term is string)
            return term;
        else if (term is bool)
            return term;
        else
        {
            double _term = (double)term;
            while (currentToken.Kind == TokenKind.MultOperator || currentToken.Kind == TokenKind.DivideOperator || currentToken.Kind == TokenKind.Modulus)
            {
                Token operatorToken = currentToken;
                Next(1);

                object nextToken = _ParseTerm();

                if (operatorToken.Kind == TokenKind.MultOperator && nextToken is double)
                    _term *= (double)nextToken;

                else if (operatorToken.Kind == TokenKind.DivideOperator && nextToken is double)
                    _term /= (double)nextToken;
                else if (operatorToken.Kind == TokenKind.Modulus && nextToken is double)
                    _term %= (double)nextToken;
                else
                {
                    Diagnostics.Errors.Add($"!semantic error: \"{operatorToken}\" cannot be used between string and number index: {currentTokenIndex - 1}");
                    throw new Exception();
                }
            }
            return _term;
        }
    }

    private object _ParseExpression()
    {
        object expressionResult = ParseTerm();
        // if the expression is a string we  will 
        if (expressionResult is string)
            return expressionResult;
        else if (expressionResult is bool)
            return expressionResult;
        else
        {
            double _expressionResult = (double)expressionResult;
            while (currentToken.Kind == TokenKind.PlusOperator || currentToken.Kind == TokenKind.MinusOperator)
            {
                Token operatorToken = currentToken;
                Next(1);
                object nextToken = _ParseExpression();
                if (operatorToken.Kind == TokenKind.PlusOperator && nextToken is double)
                    _expressionResult += (double)nextToken;
                else if (operatorToken.Kind == TokenKind.MinusOperator && nextToken is double)
                    _expressionResult -= (double)nextToken;
                else
                {
                    Diagnostics.Errors.Add($"!semantic error: \"{operatorToken}\" cannot be used between number and string index: {currentTokenIndex - 1}");
                    throw new Exception();
                }
            }

            return _expressionResult;
        }
    }

    private object ParseExpression()
    {
        object expressionResult = _ParseExpression();
        string stringExpression = null!;
        // string stringExpression = expressionResult.ToString();
        if (expressionResult is string && (currentToken.Kind == TokenKind.PlusOperator || currentToken.Kind == TokenKind.MinusOperator || currentToken.Kind == TokenKind.MultOperator || currentToken.Kind == TokenKind.DivideOperator || currentToken.Kind == TokenKind.Power))
        {
            Diagnostics.Errors.Add($"!semantic: error \"{currentToken}\" cannot be used between string and number index: {currentTokenIndex - 1}");
            throw new Exception();
        }

        // else if (expressionResult is string && (currentToken.Kind))
        while (currentToken.Kind == TokenKind.Concat)
        {
            Next(1);
            stringExpression = expressionResult.ToString()!;
            stringExpression += ParseExpression().ToString();
            return stringExpression;

        }
        return expressionResult;
    }

    /// <summary>
    /// Bolean Expression Evaluator
    /// </summary>
    /// <exception cref="Exception"></exception>

    bool EvaluateInnerExpression()
    {
        bool evaluation;
        if (currentToken.Kind == TokenKind.LeftParenthesis)
        {
            Next(1);
            object expression = EvaluateBooleanExpression();
            if (currentToken.Kind != TokenKind.RightParenthesis)
            {
                Diagnostics.Errors.Add($"!syntax error: ) is missing after \"{tokens[currentTokenIndex - 1]}\" at index: {currentTokenIndex - 1}");
                throw new Exception();
            }
            Next(1);
            return (bool)expression;
        }
        else if (currentToken.Kind == TokenKind.Not)
        {
            Next(1);
            object expression = !EvaluateInnerExpression();
            return (bool)expression;
        }
        else
        {
            object leftExpression = ParseExpression();

            if (leftExpression is bool)
            {
                return (bool)leftExpression;
            }

            TokenKind booleanOperator = currentToken.Kind;
            Next(1);

            object rightExpression = ParseExpression();

            if (rightExpression is bool)
                Next(1);

            switch (booleanOperator)
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
                    Diagnostics.Errors.Add($"!syntax error: invalid conditional expression after {tokens[currentTokenIndex - 1]} at index: {currentTokenIndex - 1}");
                    throw new Exception();
            }
            return evaluation;
        }
    }

    bool EvaluateBooleanExpression()
    {
        bool leftExpression = EvaluateInnerExpression();
        while (currentToken.Kind == TokenKind.Or || currentToken.Kind == TokenKind.And)
        {
            TokenKind _operator = currentToken.Kind;
            Next(1);
            bool rightExpression = EvaluateInnerExpression();
            if (_operator == TokenKind.Or)
            {
                leftExpression |= rightExpression;
            }
            if (_operator == TokenKind.And)
            {
                leftExpression &= rightExpression;
            }
        }
        return leftExpression;
    }

    bool EvaluateIfExpression()
    {
        if (currentToken.Kind != TokenKind.LeftParenthesis)
        {
            Diagnostics.Errors.Add($"!syntax error: ( expected after \"{tokens[currentTokenIndex - 1]}\" at index: {currentTokenIndex - 1}");
            throw new Exception();
        }

        Next(1);

        bool evaluation = EvaluateBooleanExpression();
        if (currentToken.Kind == TokenKind.RightParenthesis)
        {
            Next(1);
            return evaluation;
        }
        else
        {
            Diagnostics.Errors.Add($"!syntax error: ) expected after \"{tokens[currentTokenIndex - 1]}\" at index: {currentTokenIndex - 1}");
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

    #endregion

    #region Parser Main Function
    public void Parse()
    {
        if (tokens.Count() == 0)
        {
            variables.Clear();
            Diagnostics.Errors.Add("There's nothing to parse");
            throw new Exception();
        }
        else if (tokens.Count() == 1)
        {
            variables.Clear();
            Diagnostics.Errors.Add($"!syntax error invalid expression \"{currentToken.Value}\" at index: {currentTokenIndex - 1}");
            throw new Exception();

        }
        else if (tokens.Count() > 1)
        {
            Console.WriteLine(ParseExpression());
            if (currentToken.Kind != TokenKind.Semicolon)
            {
                variables.Clear();
                Diagnostics.Errors.Add($"!syntax error: operator or expression is missing after \"{tokens[currentTokenIndex - 1]}\" at index: {currentTokenIndex - 1}");
                throw new Exception();
            }
        }
    }

    #endregion

    // Create Variable Utility Function
    private void CreateVar(List<Token> variables)
    {
        Next(1);
        if (currentToken.Kind != TokenKind.Identifier)
        {
            Diagnostics.Errors.Add($"!syntax error: variable not defined after: \"{tokens[currentTokenIndex - 1]}\" at index: {currentTokenIndex - 1}");
            throw new Exception();
        }
        int Tokenindex = currentTokenIndex;

        Next(1);
        if (currentToken.Kind != TokenKind.Equals)
        {
            Diagnostics.Errors.Add($"!syntax error: = is missing after \"{tokens[currentTokenIndex - 1].Name}\" at index: {currentTokenIndex - 1}");
            throw new Exception();
        }

        Next(1);
        if (currentToken.Kind != TokenKind.Number && currentToken.Kind != TokenKind.String && currentToken.Kind != TokenKind.Identifier && currentToken.Kind != TokenKind.LeftParenthesis && currentToken.Kind != TokenKind.falseKeyWord && currentToken.Kind != TokenKind.trueKeyWord && currentToken.Kind != TokenKind.MinusOperator)
        {
            Diagnostics.Errors.Add($"!syntax error: variables must have a value");
            throw new Exception();
        }
        Token variable = new Token(TokenKind.Variable, tokens[Tokenindex].Name, ParseExpression());

        variables.Add(variable);

        foreach (var token in tokens)
        {
            if (token.Name == variable.Name)
            {
                token.Value = variable.Value;
            }
        }

        if (currentToken.Kind == TokenKind.Comma)
            CreateVar(variables);

        if (currentToken.Kind != TokenKind.inKeyWord)
        {
            Diagnostics.Errors.Add($"!syntax error: \"in\" is missing after \"{tokens[currentTokenIndex - 1]}\" at index: {currentTokenIndex - 1}");
            throw new Exception();
        }
    }
}