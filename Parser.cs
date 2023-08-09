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
    //  Errors

    // Previous Token
    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
        currentTokenIndex = 0;
        currentToken = tokens[currentTokenIndex];
    }

    // Now we can move through our Tokens List
    public void Next()
    {
        currentTokenIndex++;
        if (currentTokenIndex < tokens.Count())
            currentToken = tokens[currentTokenIndex];
        else
            currentToken = new Token(TokenKind.EndOfFile, "", null);
    }

    #endregion

    #region Parser Recursive Functions

    private object ParseFactor()
    {
        // if the token we are looking at is a number, we parse it, return it and move to the next
        if (currentToken.Kind == TokenKind.Number)
        {
            double factor = (double)currentToken.Value;
            Next();
            return factor;
        }

        // same with strings
        else if (currentToken.Kind == TokenKind.String)
        {
            string str = (string)currentToken.Value;
            Next();
            return str;
        }

        // if there is an identifier, we check if it is an existent variable and return its value
        else if (currentToken.Kind == TokenKind.Identifier)
        {
            object factor = null;
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

            Next();
            if (exists == false)
            {
                Diagnostics.Errors.Add($"!semantic error: variable \"{tokens[currentTokenIndex - 1].Name}\" does not exists ");
                throw new Exception();
            }


            else return factor;
        }

        // in keyword means that we evaluating an expression
        else if (currentToken.Kind == TokenKind.inKeyWord)
        {
            Next();
            object factor = ParseExpression();
            return factor;
        }

        // let keyword means that we are declaring a variable
        else if (currentToken.Kind == TokenKind.letKeyWord)
        {
            CreateVar(variables);

            // after creating vars, we stepped into in keyword
            // and need to parse the next expression

            object factor = ParseExpression();

            return factor;
        }
        // if we find a left parenthesis we will parse the next expression until we find a right parenthesis, if we don't find any
        // it will be a syntax.
        else if (currentToken.Kind == TokenKind.LeftParenthesis)
        {
            Next();
            object factor = ParseExpression();
            if (currentToken.Kind != TokenKind.RightParenthesis)
            {
                Diagnostics.Errors.Add($"!syntax error: ) is missing after \"{tokens[currentTokenIndex - 1]}\"");
                throw new Exception();
            }
            Next();
            return factor;
        }

        // for negative arithmetic expressions
        else if (currentToken.Kind == TokenKind.MinusOperator)
        {
            Next();
            double factor = 0 - (double)ParseFactor();
            return factor;
        }
        // for missing expressions to operate with
        else
        {
            Diagnostics.Errors.Add($"!syntax error: factor or expression is missing after \"{tokens[currentTokenIndex - 1].Name}\"");
            throw new Exception();
        }
    }

    private object _ParseTerm()
    {
        object term = ParseFactor();
        if (term is string)
            return term;
        else
        {
            double _term = (double)term;
            while (currentToken.Kind == TokenKind.Power)
            {
                Token operatorToken = currentToken;
                Next();
                object nextToken = ParseExpression();

                if (operatorToken.Kind != TokenKind.LeftParenthesis && nextToken is double)
                    _term = Math.Pow((double)_term, (double)nextToken);
                else
                {
                    Diagnostics.Errors.Add($"!semantic error: \"{operatorToken}\" cannot be used between string and number");
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

        else
        {
            double _term = (double)term;
            while (currentToken.Kind == TokenKind.MultOperator || currentToken.Kind == TokenKind.DivideOperator)
            {
                Token operatorToken = currentToken;
                Next();

                object nextToken = ParseExpression();

                if (operatorToken.Kind == TokenKind.MultOperator && nextToken is double)
                    _term *= (double)nextToken;

                else if (operatorToken.Kind == TokenKind.DivideOperator && nextToken is double)
                    _term /= (double)nextToken;
                else
                {
                    Diagnostics.Errors.Add($"!semantic error: {operatorToken} cannot be used between string and number");
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

        else
        {
            double _expressionResult = (double)expressionResult;
            while (currentToken.Kind == TokenKind.PlusOperator || currentToken.Kind == TokenKind.MinusOperator)
            {
                Token operatorToken = currentToken;
                Next();
                object nextToken = _ParseExpression();
                if (operatorToken.Kind == TokenKind.PlusOperator && nextToken is double)
                    _expressionResult += (double)nextToken;
                else if (operatorToken.Kind == TokenKind.MinusOperator && nextToken is double)
                    _expressionResult -= (double)nextToken;
                else
                {
                    Diagnostics.Errors.Add($"!semantic error: \"{operatorToken}\" cannot be used between number and string");
                    throw new Exception();
                }
            }

            return _expressionResult;
        }
    }

    private object ParseExpression()
    {
        object expressionResult = _ParseExpression();
        string stringExpression = null;
        // string stringExpression = expressionResult.ToString();
        if (expressionResult is string && (currentToken.Kind == TokenKind.PlusOperator || currentToken.Kind == TokenKind.MinusOperator || currentToken.Kind == TokenKind.MultOperator || currentToken.Kind == TokenKind.DivideOperator || currentToken.Kind == TokenKind.Power))
        {
            Diagnostics.Errors.Add($"!semantic: error {currentToken} cannot be used between string and number");
            throw new Exception();
        }

        // else if (expressionResult is string && (currentToken.Kind))
        while (currentToken.Kind == TokenKind.Concat)
        {
            Next();
            stringExpression = expressionResult.ToString();
            stringExpression += ParseExpression().ToString();
            return stringExpression;

        }
        return expressionResult;
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
            Diagnostics.Errors.Add($"!syntax error invalid expression \"{currentToken.Value}\"");
            throw new Exception();

        }
        else if (tokens.Count() > 1)
        {
            Console.WriteLine(ParseExpression());
            if (currentToken.Kind != TokenKind.Semicolon)
            {
                variables.Clear();
                Diagnostics.Errors.Add($"!syntax error: operator is missing after \"{tokens[currentTokenIndex - 1]}\"");
                throw new Exception();
            }
        }
    }

    #endregion

    // Create Variable Utility Function
    private void CreateVar(List<Token> variables)
    {
        Next();
        if (currentToken.Kind != TokenKind.Identifier)
        {
            Diagnostics.Errors.Add($"!syntax error: variable not defined after: \"{tokens[currentTokenIndex - 1]}\" ");
            throw new Exception();
        }
        int Tokenindex = currentTokenIndex;

        Next();
        if (currentToken.Kind != TokenKind.Equals)
        {
            Diagnostics.Errors.Add($"!syntax error: = is missing after \"{tokens[currentTokenIndex - 1].Name}\"");
            throw new Exception();
        }

        Next();
        if (currentToken.Kind != TokenKind.Number && currentToken.Kind != TokenKind.String && currentToken.Kind != TokenKind.Identifier && currentToken.Kind != TokenKind.LeftParenthesis)
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
            Diagnostics.Errors.Add($"!syntax error: \"in\" is missing after Token:\"{tokens[currentTokenIndex - 1]}\"");
            throw new Exception();
        }
    }
}