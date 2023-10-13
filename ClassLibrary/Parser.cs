// After we made our list of Tokens, we need to find the sense of the expression, if it exists.
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ClassLibrary
{
    public class Parser
    {

        // </Tokens List to Parse
        private readonly List<Token> tokens;

        // </VarList to keep record of created Variables
        private List<Dictionary<string, object>> Scope;
        private int scopeIndex = -1;

        // </Current Index
        private int currentTokenIndex;
        // </Current Token
        private Token currentToken;
        // </if-else tuples
        private List<(int, int)> ifElseMatches = new List<(int, int)>();
        // </Constructor
        public Parser(List<Token> tokens, List<Dictionary<string, object>> scope)
        {
            this.tokens = tokens;
            this.Scope = scope;
            currentTokenIndex = 0;
            currentToken = tokens[currentTokenIndex];
            CheckBalance();
        }

        #region Parser Main Function
        public void Parse()
        {
            if (tokens.Count() <= 0)
            {
                Scope.Clear();
                Console.WriteLine("there is nothing to parse.");
                return;
            }

            if (tokens.Count() == 1)
            {
                Scope.Clear();
                Console.WriteLine($"!syntax error: invalid expression");
                return;
            }

            ParseExpression();

            if (currentToken.Kind != TokenKind.Semicolon && currentToken.Kind != TokenKind.EndOfFile)
            {
                Scope.Clear();
                Diagnostics.Errors.Add($"!syntax error: operator or expression is missing after \"{tokens[currentTokenIndex - 1]}\" at index: {currentTokenIndex - 1}.");
                throw new Exception();
            }

        }

        #endregion

        #region Recursive-Descent Parsing Functions and Logic

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
                Consume(1);
                string stringExpression = expressionResult.ToString()!;
                stringExpression += ParseExpression().ToString();
                return stringExpression;
            }
            return expressionResult;
        }

        private object ParseAndOr()
        {
            object leftExpression = ParseComparison();

            if (!(leftExpression is bool) | leftExpression is null)
            {
                return leftExpression!;
            }

            bool _leftExpression = (bool)leftExpression!;

            while (currentToken.Kind == TokenKind.Or || currentToken.Kind == TokenKind.And)
            {
                TokenKind _operator = currentToken.Kind;
                Consume(1);

                bool rightExpression = (bool)ParseComparison();

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

        private object ParseComparison()
        {
            object leftExpression = ParseSum();

            if (leftExpression is null)
            {
                return leftExpression!;
            }

            else
            {
                bool evaluation;

                TokenKind _operator = currentToken.Kind;


                if (!IsBooleanOperator(_operator))
                {
                    return leftExpression;
                }

                else
                {
                    Consume(1);
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

                            if (leftExpression.GetType() == rightExpression.GetType())
                            {
                                evaluation = leftExpression.ToString() == rightExpression.ToString();
                            }

                            else
                            {
                                Diagnostics.Errors.Add($"!semantic error: comparison operator \"{_operator}\" cannot be applied between different data types.");
                                throw new Exception();
                            }

                            break;

                        case TokenKind.NotEquals:

                            if (leftExpression.GetType() == rightExpression.GetType())
                            {
                                evaluation = leftExpression.ToString() != rightExpression.ToString();
                            }

                            else
                            {
                                Diagnostics.Errors.Add($"!semantic error: comparison operator \"{_operator}\" cannot be applied between different data types.");
                                throw new Exception();
                            }

                            break;

                        default:
                            Diagnostics.Errors.Add($"!syntax error: invalid conditional expression after \"{tokens[currentTokenIndex - 1]}\" at index: {currentTokenIndex - 1}.");
                            throw new Exception();
                    }

                    return evaluation;
                }
            }
        }

        private object ParseSum()
        {
            object expressionResult = ParseFactor();

            if (expressionResult is string | expressionResult is null | expressionResult is bool)
            {
                return expressionResult!;
            }

            else
            {
                double leftExpression = (double)expressionResult!;
                while (currentToken.Kind == TokenKind.PlusOperator || currentToken.Kind == TokenKind.MinusOperator)
                {
                    Token operatorToken = currentToken;
                    Consume(1);

                    object rightExpression = ParseSum();

                    if (operatorToken.Kind == TokenKind.PlusOperator && rightExpression is double)
                    {
                        leftExpression += (double)rightExpression;
                    }

                    else if (operatorToken.Kind == TokenKind.MinusOperator && rightExpression is double)
                    {
                        leftExpression -= (double)rightExpression;
                    }

                    else
                    {
                        Diagnostics.Errors.Add($"!semantic error: \"{operatorToken}\" cannot be used between numbers and strings index: {currentTokenIndex - 1}.");
                        throw new Exception();
                    }
                }

                return leftExpression;
            }
        }


        private object ParseFactor()
        {
            object factor = ParsePower();
            if (factor is string | factor is null | factor is bool)
            {
                return factor!;
            }

            else
            {
                double _factor = (double)factor!;
                while (currentToken.Kind == TokenKind.MultOperator || currentToken.Kind == TokenKind.DivideOperator || currentToken.Kind == TokenKind.Modulus)
                {
                    Token operatorToken = currentToken;
                    Consume(1);

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

        private object ParsePower()
        {

            object factor = ParseTerm();

            if (factor is string | factor is null | factor is bool)
            {
                return factor!;
            }

            else
            {
                double leftExpression = (double)factor!;
                while (currentToken.Kind == TokenKind.Power)
                {
                    Token operatorToken = currentToken;
                    Consume(1);
                    object rightExpression = ParsePower();

                    if (currentToken.Kind == TokenKind.LeftParenthesis && rightExpression is double)
                    {
                        leftExpression = Math.Pow((double)leftExpression, (double)rightExpression);
                    }

                    else if (currentToken.Kind != TokenKind.LeftParenthesis && rightExpression is double)
                    {
                        leftExpression = Math.Pow((double)leftExpression, (double)rightExpression);
                    }

                    else
                    {
                        Diagnostics.Errors.Add($"!semantic error: \"{operatorToken}\" cannot be used between strings and numbers.");
                        throw new InvalidOperationException();
                    }
                }
                return leftExpression;
            }
        }

        private object ParseTerm()
        {
            switch (currentToken.Kind)
            {
                // </Get number value
                case TokenKind.Number:
                    object term = currentToken.GetValue();
                    Consume(1);
                    return (double)term;

                // </Get string value
                case TokenKind.String:
                    term = currentToken.GetValue();
                    Consume(1);
                    return (string)term;

                // </Get false
                case TokenKind.falseKeyWord:
                    Consume(1);
                    return false;

                // </Get true
                case TokenKind.trueKeyWord:
                    Consume(1);
                    return true;

                // </Get variable value or evaluate function
                case TokenKind.Identifier:
                    string identifierName = currentToken.GetName();

                    if (tokens[currentTokenIndex + 1].Kind != TokenKind.LeftParenthesis)
                    {
                        term = null!;

                        for (int i = Scope.Count - 1; scopeIndex >= -1; i--)
                        {
                            if (Scope[i].ContainsKey(identifierName))
                            {
                                term = Scope[i][identifierName];
                                Consume(1);
                                break;
                            }
                        }

                        if (term is null)
                        {
                            Diagnostics.Errors.Add($"!semantic error: variable \"{identifierName}\" does not exists.");
                            throw new Exception();
                        }

                        return term;
                    }

                    else if (Global.functions.ContainsKey(identifierName))
                    {
                        Funct function = Global.functions[identifierName];
                        term = EvaluateFunction(identifierName, function);
                        Consume(1);
                        return term;
                    }

                    else
                    {
                        Diagnostics.Errors.Add($"!semantic error: function \"{identifierName}\" does not exists.");
                        throw new Exception();
                    }

                // </Evaluate conditional expressions
                case TokenKind.ifKeyWord:

                    int ifIndex = currentTokenIndex;
                    Consume(1);
                    bool evaluation = EvaluateIfExpression();

                    // </In case if expression returns true
                    if (evaluation)
                    {
                        // Execute if instruction
                        object ifInstruction = ParseExpression();
                        currentTokenIndex = tokens.Count() - 1;
                        Consume(1);
                        return ifInstruction;
                    }

                    // </In case if expression returns false
                    else
                    {
                        // Move to the corresponding "if" else match
                        StepIntoElse(ifIndex);
                        Consume(1);

                        // Execute else instruction
                        object elseInstruction = ParseExpression();
                        currentTokenIndex = tokens.Count() - 1;
                        Consume(1);
                        return elseInstruction;
                    }

                // </Expressions cannot start with else keyword
                case TokenKind.elseKeyWord:
                    Diagnostics.Errors.Add("!syntax error: if-else instructions are not balanced.");
                    throw new Exception();

                // </A left parenthesis means we stepped at a new inner expression
                case TokenKind.LeftParenthesis:
                    Consume(1);
                    term = ParseExpression();

                    if (currentToken.Kind != TokenKind.RightParenthesis)
                    {
                        Diagnostics.Errors.Add($"!syntax error: ) is missing after \"{tokens[currentTokenIndex - 1]}\" at index: {currentTokenIndex - 1}.");
                        throw new Exception();
                    }

                    Consume(1);
                    return term;

                case TokenKind.PlusOperator:
                    Consume(1);
                    term = 0 + (double)ParseTerm();
                    return term;

                case TokenKind.MinusOperator:
                    Consume(1);
                    term = 0 - (double)ParseTerm();
                    return term;

                case TokenKind.Not:
                    Consume(1);
                    term = !(bool)ParseTerm();
                    return term;

                // </Create variables
                case TokenKind.letKeyWord:
                    Dictionary<string, object> enviroment = new Dictionary<string, object>();
                    Scope.Add(enviroment);
                    scopeIndex++;
                    CreateVar();
                    term = ParseExpression();
                    // variables.Clear();
                    return term;

                // </In keyword means we stepped at a new expression 
                case TokenKind.inKeyWord:
                    Consume(1);
                    term = ParseExpression();
                    Scope.Remove(Scope[scopeIndex]);
                    scopeIndex--;
                    return term;

                case TokenKind.E:
                    Consume(1);
                    return Math.E;

                case TokenKind.PI:
                    Consume(1);
                    return Math.PI;

                case TokenKind.sin:
                    Consume(1);
                    if (currentToken.Kind != TokenKind.LeftParenthesis)
                    {
                        Diagnostics.Errors.Add($"!syntax error: \"Right Parenthesis\" is missing after identifier \"Sin\" at index : {currentTokenIndex}");
                        throw new Exception();
                    }

                    Consume(1);
                    object arg = ParseExpression();

                    if (currentToken.Kind != TokenKind.RightParenthesis)
                    {
                        Diagnostics.Errors.Add($"!syntax error: \"Right Parenthesis\" is missing after \"{currentToken}\" at index : {currentTokenIndex + 1}");
                        throw new Exception();
                    }

                    Consume(1);

                    double result = Math.Sin((double)arg * (Math.PI / 180));
                    return result;

                case TokenKind.cos:
                    Consume(1);

                    if (currentToken.Kind != TokenKind.LeftParenthesis)
                    {
                        Diagnostics.Errors.Add($"!syntax error: \"Left Parenthesis\" is missing after identifier \"cos\" at index : {currentTokenIndex}");
                        throw new Exception();
                    }

                    Consume(1);
                    arg = ParseExpression();

                    if (currentToken.Kind != TokenKind.RightParenthesis)
                    {
                        Diagnostics.Errors.Add($"!syntax error: \"Right Parenthesis\" is missing {currentToken} at index : {currentTokenIndex + 1}");
                        throw new Exception();
                    }

                    Consume(1);

                    result = Math.Cos((double)arg * (Math.PI / 180));
                    return result;

                case TokenKind.log:
                    Consume(1);

                    if (currentToken.Kind != TokenKind.LeftParenthesis)
                    {
                        Diagnostics.Errors.Add($"!syntax error: \"Left Parenthesis\" is missing after identifier \"log\" at index : {currentTokenIndex}");
                        throw new Exception();
                    }

                    Consume(1);
                    object base_ = ParseExpression();

                    if (currentToken.Kind != TokenKind.Comma)
                    {
                        Diagnostics.Errors.Add($"!syntax error: \"Comma\" is missing after \"{tokens[currentTokenIndex - 1]}\" at index : {currentTokenIndex - 1}");
                        throw new Exception();
                    }

                    Consume(1);
                    object value = ParseExpression();

                    if (currentToken.Kind != TokenKind.RightParenthesis)
                    {
                        Diagnostics.Errors.Add($"!syntax error: \"Right Parenthesis\" is missing after \"{currentToken}\" at index : {currentTokenIndex + 1}");
                        throw new Exception();
                    }

                    Consume(1);

                    return null!;

                case TokenKind.print:
                    Consume(1);

                    if (currentToken.Kind != TokenKind.LeftParenthesis)
                    {
                        Diagnostics.Errors.Add($"!syntax error: \"Left Parenthesis\" is missing after identifier \"print\" at index : {currentTokenIndex}");
                        throw new Exception();
                    }

                    Consume(1);
                    arg = ParseExpression();

                    if (currentToken.Kind != TokenKind.RightParenthesis)
                    {
                        Diagnostics.Errors.Add($"!syntax error: \"Right Parenthesis\" is missing after \"{currentToken}\" at index : {currentTokenIndex}");
                        throw new Exception();
                    }

                    Consume(1);

                    Console.WriteLine(arg);

                    return null!;


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

        #endregion

        # region Utility Functions

        bool EvaluateIfExpression()
        {
            if (currentToken.Kind != TokenKind.LeftParenthesis)
            {
                Diagnostics.Errors.Add($"!syntax error: \"Left Parenthesis\" expected after \"{tokens[currentTokenIndex - 1]}\" at index: {currentTokenIndex - 1}.");
                throw new Exception();
            }

            Consume(1);

            bool evaluation = (bool)ParseAndOr();

            if (currentToken.Kind == TokenKind.RightParenthesis)
            {
                Consume(1);
                return evaluation;
            }

            else
            {
                Diagnostics.Errors.Add($"!syntax error: \"Right Parenthesis\" expected after \"{tokens[currentTokenIndex - 1]}\" at index: {currentTokenIndex - 1}.");
                throw new Exception();
            }
        }

        void CheckBalance()
        {
            int ifIndex = -1;
            int ifBalance = 0;

            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].Kind == TokenKind.ifKeyWord)
                {
                    ifIndex = i;
                    break;
                }
            }

            if (ifIndex >= 0)
            {
                Stack<int> stack = new();

                for (int i = ifIndex; i < tokens.Count; i++)
                {
                    switch (tokens[i].Kind)
                    {
                        case TokenKind.ifKeyWord:
                            ifBalance++;
                            stack.Push(i);
                            break;

                        case TokenKind.elseKeyWord:
                            ifBalance--;

                            if (ifBalance < 0)
                            {
                                Diagnostics.Errors.Add("!syntax error: if-else instructions are not balanced.");
                                throw new Exception();
                            }

                            ifElseMatches.Add((stack.Pop(), i));
                            break;

                        default:
                            continue;
                    }
                }


                if (ifBalance != 0)
                {
                    Diagnostics.Errors.Add("!syntax error: if-else instructions are not balanced.");
                    throw new Exception();
                }
            }
        }

        void StepIntoElse(int ifIndex)
        {
            foreach (var match in ifElseMatches)
            {
                if (ifIndex == match.Item1)
                {
                    currentTokenIndex = match.Item2;
                    break;
                }
            }
        }


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

        #endregion

        #region Variables and Functions Creation

        private void CreateVar()
        {

            Consume(1);
            if (currentToken.Kind != TokenKind.Identifier)
            {
                Diagnostics.Errors.Add($"!syntax error: variable not defined after: \"{tokens[currentTokenIndex - 1]}\" at index: {currentTokenIndex - 1}.");
                throw new Exception();
            }

            string varName = currentToken.GetName();

            Consume(1);
            if (currentToken.Kind != TokenKind.Equals)
            {
                Diagnostics.Errors.Add($"!syntax error: = is missing after \"{tokens[currentTokenIndex - 1].GetName()}\" at index: {currentTokenIndex - 1}.");
                throw new Exception();
            }

            Consume(1);
            if (currentToken.Kind != TokenKind.Number && currentToken.Kind != TokenKind.String && currentToken.Kind != TokenKind.Identifier && currentToken.Kind != TokenKind.LeftParenthesis && currentToken.Kind != TokenKind.falseKeyWord && currentToken.Kind != TokenKind.trueKeyWord && currentToken.Kind != TokenKind.MinusOperator)
            {
                Diagnostics.Errors.Add($"!syntax error: variables must have a value.");
                throw new Exception();
            }

            object varValue = ParseExpression();

            if (!Scope[scopeIndex].ContainsKey(varName))
            {
                Scope[scopeIndex].Add(varName, varValue);
            }

            else
            {
                Scope[scopeIndex][varName] = varValue;
            }

            if (currentToken.Kind == TokenKind.Comma)
            {
                CreateVar();
            }

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

            Consume(1);
            if (currentToken.Kind != TokenKind.Identifier)
            {
                Diagnostics.Errors.Add($"!syntax error: function name is not declared after \"{tokens[currentTokenIndex - 1]}\".");
                throw new Exception();
            }

            // </Get function name
            functionName = currentToken.GetName();

            Consume(1);

            if (currentToken.Kind != TokenKind.LeftParenthesis)
            {
                Diagnostics.Errors.Add($"!syntax error: \"Right Parenthesis\" is missing after \"{tokens[currentTokenIndex - 1]}\".");
                throw new Exception();
            }

            Consume(1);

            // </Get function arguments
            while (currentToken.Kind != TokenKind.RightParenthesis)
            {
                if (currentToken.Kind != TokenKind.Identifier)
                {
                    Diagnostics.Errors.Add($"!syntax error: \"{currentToken}\" is not a valid argument at index {currentTokenIndex}");
                    throw new Exception();
                }

                args.Add((currentToken.GetName(), null!));
                Consume(1);

                if (currentToken.Kind == TokenKind.Comma)
                {
                    Consume(1);
                }
            }

            Consume(1);

            if (currentToken.Kind != TokenKind.Arrow)
            {
                Diagnostics.Errors.Add($"!syntax error: \"=>\" is missing after \"{tokens[currentTokenIndex - 1]}\".");
                throw new Exception();
            }

            Consume(1);

            // </Get function body
            while (currentToken.Kind != TokenKind.Semicolon)
            {
                body.Add(currentToken);
                Consume(1);
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

        public void Consume(int positions)
        {
            currentTokenIndex += positions;

            if (currentTokenIndex < tokens.Count())
            {
                currentToken = tokens[currentTokenIndex];
            }

            else
            {
                currentToken = new Keyword(TokenKind.EndOfFile);
            }
        }

        public void ClearVariables() => Scope.Clear();

        #region Function Evaluation 
        private object EvaluateFunction(string functionName, Funct function)
        {
            Consume(1);

            if (currentToken.Kind != TokenKind.LeftParenthesis)
            {
                Diagnostics.Errors.Add($"!syntax error: \"Left Parenthesis\" is missing after \"{tokens[currentTokenIndex - 1]}\".");
                throw new Exception();
            }

            Consume(1);

            // </Get function argument values
            int index = 0;
            while (currentToken.Kind != TokenKind.RightParenthesis)
            {
                if (index > function.Args.Count - 1)
                {
                    Diagnostics.Errors.Add($"!semantic error: function: \"{functionName}\" recieves {function.Args.Count} argument(s), but {index + 1} were given. ");
                    throw new Exception();
                }

                object seconditem = ParseExpression();
                function.Args[index] = (function.Args[index].Item1, seconditem);

                index++;

                if (currentToken.Kind == TokenKind.Comma)
                {
                    Consume(1);
                }
            }

            if (index < function.Args.Count)
            {
                Diagnostics.Errors.Add($"!semantic error: function: \"{functionName}\" recieves {function.Args.Count} argument(s), but {index} were given. ");
                throw new Exception();
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
}