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
    public List<VarToken> variables = new List<VarToken>();

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
            currentToken = new Token(TokenKind.EndOfFile, "");
    }

    #endregion

    #region Parser Recursive Functions

    private object ParseFactor()
    {
        // if the token we are looking at is a number, we parse it, return it and move to the next
        if (currentToken.Kind == TokenKind.Number)
        {
            double factor = double.Parse(currentToken.Value);
            Next();
            return factor;
        }

        // same with strings
        else if (currentToken.Kind == TokenKind.String)
        {
            string str = currentToken.Value;
            Next();
            return str;
        }

        // if there is an identifier, we check if it is a variable and return its value
        else if (currentToken.Kind == TokenKind.Identifier)
        {
            object factor = null;
            bool exists = false;
            foreach (var variable in variables)
            {
                if (currentToken.Value == variable.Value)
                {
                    factor = variable.VarValue;
                    exists = true;
                    break;
                }
            }

            Next();
            if (exists == false) throw new Exception($"Variable does not exists: {tokens[currentTokenIndex]}");
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
        // it will be a syntax error.
        else if (currentToken.Kind == TokenKind.LeftParenthesis)
        {
            Next();
            object factor = ParseExpression();
            if (currentToken.Kind != TokenKind.RightParenthesis)
                throw new InvalidOperationException("Syntax Error");
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

        else
            throw new InvalidOperationException("Syntax Error");
    }

    private object ParseExpression()
    {
        object expressionResult = ParseTerm();
        // if the expression is a string we  will 
        if (expressionResult is string)
        {
            while (currentToken.Kind == TokenKind.Concat)
            {
                Next();
                string nextToken = (string)ParseTerm();
                expressionResult += nextToken;
            }
            return expressionResult;
        }
        else
        {
            double _expressionResult = (double)expressionResult;
            while (currentToken.Kind == TokenKind.PlusOperator || currentToken.Kind == TokenKind.MinusOperator)
            {
                Token operatorToken = currentToken;
                Next();
                double nextToken = (double)ParseTerm();
                if (operatorToken.Kind == TokenKind.PlusOperator)
                    _expressionResult += nextToken;
                else if (operatorToken.Kind == TokenKind.MinusOperator)
                    _expressionResult -= nextToken;
            }
            return _expressionResult;
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

                double nextToken = (double)_ParseTerm();

                if (operatorToken.Kind == TokenKind.MultOperator)
                    _term *= nextToken;

                else if (operatorToken.Kind == TokenKind.DivideOperator)
                    _term /= nextToken;
            }
            return _term;
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
                if (operatorToken.Kind != TokenKind.LeftParenthesis)
                    _term = Math.Pow((double)_term, (double)ParseExpression());
                else
                    throw new InvalidOperationException("Syntax Error");
            }
            return _term;
        }
    }

    #endregion

    #region Parser Main Function
    public void Parse()
    {
        if (tokens.Count() == 0)
        {
            variables.Clear();
            throw new Exception("There's nothing to parse");
        }
        Console.WriteLine(ParseExpression());

        if (currentToken.Kind == TokenKind.EndOfFile)
        {
            variables.Clear();
            throw new InvalidOperationException($"Syntax Error: ; is missing after {currentToken.Value}");
        }
    }

    #endregion

    // Create Variable Utility Function
    private void CreateVar(List<VarToken> variables)
    {
        Next();
        if (currentToken.Kind != TokenKind.Identifier)
            throw new Exception($"Syntax Error: you have not defined an identifier after {currentToken.Value}");
        int vartokenindex = currentTokenIndex;

        Next();
        if (currentToken.Kind != TokenKind.EqualsOperator)
            throw new Exception($"Syntax Error: = is missing after {tokens[currentTokenIndex - 1]}");

        Next();
        VarToken variable = new VarToken(TokenKind.Variable, tokens[vartokenindex].Value, ParseExpression());
        // tokens[vartokenindex]=new VarToken(TokenKind.Variable,tokens[vartokenindex].Value,ParseExpression());
        variables.Add(variable);

        if (currentToken.Kind == TokenKind.Comma)
            CreateVar(variables);
        if (currentToken.Kind != TokenKind.inKeyWord)
            throw new Exception($"Syntax Error: in is missing after {tokens[currentTokenIndex - 1]}");
    }
}

