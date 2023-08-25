// After we made our list of Tokens, we need to find the sense of the expression, if it exists.
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
            currentToken = new Token(TokenKind.EndOfFile, "", null!);
    }

    public void ClearVariables()
    {
        variables.Clear();
    }
    #endregion

    #region Parser Recursive Functions

    private object ParseFactor()
    {
        switch (currentToken.Kind)
        {
            // </Get number value
            case TokenKind.Number:
                object factor = currentToken.Value;
                Eat(1);
                return (double)factor;

            // </Get string value
            case TokenKind.String:
                factor = currentToken.Value;
                Eat(1);
                return (string)factor;

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

                if (variables.ContainsKey(currentToken.Name))
                {
                    factor = variables[currentToken.Name];
                    Eat(1);
                    return factor;
                }

                else if (Global.functions.ContainsKey(currentToken.Name))
                {
                    Funct newFunction = (Funct)Global.functions[currentToken.Name].Clone();
                    stack.Add(newFunction);
                    factor = EvaluateFunction(stack.Last());
                    Eat(1);
                    return factor;
                }

                else
                {

                    Diagnostics.Errors.Add($"!semantic error: function or variable \"{tokens[currentTokenIndex].Name}\" does not exists.");
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
                    currentTokenIndex = tokens.Count() - 2;
                    Eat(1);
                    return ifInstruction;

                }

                // </In case if expression returns false
                else if (balanced && evaluation==false)
                {
                    // Move to the corresponding "if" else match
                    StepIntoElse(ifIndex);
                    Eat(1);

                    // Execute else instruction
                    object elseInstruction = ParseExpression();
                    currentTokenIndex = tokens.Count() - 2;
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

            // </In keyword means we stepped on a new inner expression
            case TokenKind.LeftParenthesis:
                Eat(1);
                factor = ParseExpression();
                if (currentToken.Kind != TokenKind.RightParenthesis)
                {
                    Diagnostics.Errors.Add($"!syntax error: ) is missing after \"{tokens[currentTokenIndex - 1]}\" at index: {currentTokenIndex - 1}.");
                    throw new Exception();
                }
                Eat(1);
                return factor;

            case TokenKind.PlusOperator:
                Eat(1);
                factor = 0 + (double)ParseFactor();
                return factor;
                
            case TokenKind.MinusOperator:
                Eat(1);
                factor = 0 - (double)ParseFactor();
                return factor;

            // </Create variables
            case TokenKind.letKeyWord:
                CreateVar();
                factor = ParseExpression();
                return factor;

            // </In keyword means we stepped on a new expression 
            case TokenKind.inKeyWord:
                Eat(1);
                factor = ParseExpression();
                return factor;


            default:
                Diagnostics.Errors.Add($"!syntax error: factor or expression is missing after \"{tokens[currentTokenIndex - 1]}\" at index: {currentTokenIndex - 1}.");
                throw new Exception();

        }
    }

    private object _ParseTerm()
    {

        object term = ParseFactor();

        if (term is string)
            return term;

        else if (term is null)
            return term!;

        else if (term is bool)
            return term;

        else
        {
            double _term = (double)term;
            while (currentToken.Kind == TokenKind.Power)
            {
                Token operatorToken = currentToken;
                Eat(1);
                object nextToken = _ParseTerm();

                if (currentToken.Kind == TokenKind.LeftParenthesis && nextToken is double)
                {
                    _term = Math.Pow((double)_term, (double)nextToken);
                }
                else if (currentToken.Kind != TokenKind.LeftParenthesis && nextToken is double)
                {
                    _term = Math.Pow((double)_term, (double)nextToken);
                }
                else
                {
                    Diagnostics.Errors.Add($"!semantic error: \"{operatorToken}\" cannot be used between string and number index: {currentTokenIndex - 1}.");
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

        else if (term is null)
            return term!;

        else if (term is bool)
            return term;

        else
        {
            double _term = (double)term;
            while (currentToken.Kind == TokenKind.MultOperator || currentToken.Kind == TokenKind.DivideOperator || currentToken.Kind == TokenKind.Modulus)
            {
                Token operatorToken = currentToken;
                Eat(1);

                object nextToken = ParseTerm();

                if (operatorToken.Kind == TokenKind.MultOperator && nextToken is double)
                    _term *= (double)nextToken;

                else if (operatorToken.Kind == TokenKind.DivideOperator && nextToken is double)
                    _term /= (double)nextToken;

                else if (operatorToken.Kind == TokenKind.Modulus && nextToken is double)
                    _term %= (double)nextToken;

                else
                {
                    Diagnostics.Errors.Add($"!semantic error: \"{operatorToken}\" cannot be used between string and number index: {currentTokenIndex - 1}.");
                    throw new Exception();
                }
            }
            return _term;
        }
    }

    private object _ParseExpression()
    {
        object expressionResult = ParseTerm();

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

                object nextToken = _ParseExpression();

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

    public object ParseExpression()
    {
        if (currentToken.Kind == TokenKind.functionKeyWord)
        {
            CreateFunction(Global.functions);
            return null!;
        }

        object expressionResult = _ParseExpression();

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
    bool EvaluateInnerExpression()
    {
        bool evaluation;
        if (currentToken.Kind == TokenKind.LeftParenthesis)
        {
            Eat(1);
            object expression = EvaluateBooleanExpression();

            if (currentToken.Kind != TokenKind.RightParenthesis)
            {
                Diagnostics.Errors.Add($"!syntax error: \"Right Parenthesis\" is missing after \"{tokens[currentTokenIndex - 1]}\" at index: {currentTokenIndex - 1}.");
                throw new Exception();
            }

            Eat(1);
            return (bool)expression;
        }

        else if (currentToken.Kind == TokenKind.Not)
        {
            Eat(1);
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
            Eat(1);

            object rightExpression = ParseExpression();

            if (rightExpression is bool)
                Eat(1);

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
                    Diagnostics.Errors.Add($"!syntax error: invalid conditional expression after \"{tokens[currentTokenIndex - 1]}\" at index: {currentTokenIndex - 1}.");
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
            Eat(1);

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
            Diagnostics.Errors.Add($"!syntax error: \"Left Parenthesis\" expected after \"{tokens[currentTokenIndex - 1]}\" at index: {currentTokenIndex - 1}.");
            throw new Exception();
        }

        Eat(1);

        bool evaluation = EvaluateBooleanExpression();

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
        int Tokenindex = currentTokenIndex;

        Eat(1);
        if (currentToken.Kind != TokenKind.Equals)
        {
            Diagnostics.Errors.Add($"!syntax error: = is missing after \"{tokens[currentTokenIndex - 1].Name}\" at index: {currentTokenIndex - 1}.");
            throw new Exception();
        }

        Eat(1);
        if (currentToken.Kind != TokenKind.Number && currentToken.Kind != TokenKind.String && currentToken.Kind != TokenKind.Identifier && currentToken.Kind != TokenKind.LeftParenthesis && currentToken.Kind != TokenKind.falseKeyWord && currentToken.Kind != TokenKind.trueKeyWord && currentToken.Kind != TokenKind.MinusOperator)
        {
            Diagnostics.Errors.Add($"!syntax error: variables must have a value.");
            throw new Exception();
        }

        Token variable = new Token(TokenKind.Identifier, tokens[Tokenindex].Name, ParseExpression());

        if (!variables.ContainsKey(variable.Name))
            variables.Add(variable.Name, variable.Value);

        else
        {
            variables.Remove(variable.Name);
            variables.Add(variable.Name, variable.Value);
        }

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
        functionName = currentToken.Name;

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
                Diagnostics.Errors.Add($"!syntax error: \"{currentToken}\" is not a valid argument.");
                throw new Exception();
            }

            args.Add((currentToken.Name, currentToken.Value));
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

        body.Add(new Token(TokenKind.Semicolon, ";", null!));

        // </Build function
        Funct function = new Funct(args, body);

        if (!functions.ContainsKey(functionName))
        {
            functions.Add(functionName, function);
            Console.WriteLine($"!function: \"{functionName}\" created.");
        }

        else
        {
            functions.Remove(functionName);
            Console.WriteLine($"!old function: \"{functionName}\" removed.");
            functions.Add(functionName, function);
            Console.WriteLine($"!new function: \"{functionName}\" created.");
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

        // </Get function arguments values
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

        if(currentToken.Kind != TokenKind.RightParenthesis)
        {
            Diagnostics.Errors.Add($"!syntax error: \"Right Parenthesis\" is missing after \"{tokens[currentTokenIndex-1]}\" at index {currentTokenIndex}");
            throw new Exception();
        }
        
        return evaluation;
    }
    #endregion
}