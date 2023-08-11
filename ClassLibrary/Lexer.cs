using System.Diagnostics;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Xml.Schema;


// A Lexer walks through the code and splits it into Tokens until it found any meaning
// So, Lexer:
// Recieves <code> ------- return Tokens
public class Lexer
{
    #region Test Section
    static void Main(string[] args)
    {
        while (true)
        {
            try
            {
                Console.Write(">");
                string sourceCode = Console.ReadLine()!;

                var Lexer = new Lexer(sourceCode);
                if (Lexer.sourceCode == string.Empty)
                {
                    Diagnostics.Errors.Add("Empty Entry");
                    throw new Exception();
                }
                List<Token> tokens = Lexer.Tokenize();
                Parser parser = new(tokens);
                parser.Parse();

                parser.variables.Clear();
            }
            catch (Exception)
            {
                Console.WriteLine(Diagnostics.Errors[0]);
                Diagnostics.Errors.Clear();
                continue;
            }
        }
    }

    #endregion

    #region  Lexer Object
    public readonly string sourceCode;
    private int currentPosition;

    public Lexer(string sourceCode)
    {
        this.sourceCode = sourceCode;
        currentPosition = 0;
    }

    #endregion 

    #region Lexer Main Function: Tokenize
    // We need to split the Tokens
    // So I created a function named Tokenize, wich returns a List of Tokens containing all the Tokens
    public List<Token> Tokenize()
    {

        // Initialize List
        List<Token> tokens = new();

        while (currentPosition < sourceCode.Length)
        {

            // The character we are looking at is the current position of the sourceCode
            char currentChar = sourceCode[currentPosition];

            // If there's any white space we move on
            if (char.IsWhiteSpace(currentChar))
            {
                currentPosition++;
                continue;
            }
            // Identifiers
            else if (IsLetter(currentChar))
                tokens.Add(IdKind());
            // Quoted Strings
            else if (currentChar == '"')
                tokens.Add(StringKind());
            // Digits
            else if (char.IsDigit(currentChar))
                tokens.Add(NumberKind());
            // Operators 
            else if (IsOperator(currentChar))
                tokens.Add(OperatorKind());
            // Punctuators
            else if (IsPunctuator(currentChar))
                tokens.Add(PuncutatorKind());
            // unknown tokens
            else
            {
                tokens.Add(new Token(TokenKind.Unknown, currentChar.ToString(), null!));
                Diagnostics.Errors.Add($"!lexical error: \"{tokens.Last().Value}\" is not a valid token");
                currentPosition++;
            }
        }
        if (tokens.Last().Name != ";")
        {
            Diagnostics.Errors.Add("!syntax error: expression must end with \";\"");
            throw new Exception();
        }
        return tokens;
    }

    #endregion

    #region TokenKind Adder Functions

    private Token NumberKind()
    {

        string number = "";
        while ((currentPosition < sourceCode.Length) && (char.IsDigit(sourceCode[currentPosition]) || sourceCode[currentPosition] == '.'))
        {
            number += sourceCode[currentPosition];
            currentPosition++;
        }
        number = number.Replace('.', ',');
        // Token (Token Kind, Number Value)
        return new Token(TokenKind.Number, null!, double.Parse(number));
    }

    private Token IdKind()
    {

        string idkind = "";

        while (currentPosition < sourceCode.Length && (IsLetterOrDigit(sourceCode[currentPosition]) || sourceCode[currentPosition] == '_'))
        {
            idkind += sourceCode[currentPosition];
            currentPosition++;
        }
        if (IsKeyWord(idkind))
        {
            return KeyWord(idkind);
        }
        else
            return new Token(TokenKind.Identifier, idkind, null!);
    }

    private Token StringKind()
    {
        currentPosition++;
        string str = "";
        while (currentPosition < sourceCode.Length && sourceCode[currentPosition] != '"')
        {
            str += sourceCode[currentPosition];
            currentPosition++;
        }
        Next();
        return new Token(TokenKind.String, null!, str);
    }

    private Token OperatorKind()
    {
        char _operator = sourceCode[currentPosition];

        if (_operator == '+')
        {
            Next();
            return new Token(TokenKind.PlusOperator, _operator.ToString(), null!);
        }

        else if (_operator == '-')
        {
            Next();
            return new Token(TokenKind.MinusOperator, _operator.ToString(), null!);
        }
        else if (_operator == '*')
        {
            Next();
            return new Token(TokenKind.MultOperator, _operator.ToString(), null!);
        }
        else if (_operator == '/')
        {
            Next();
            return new Token(TokenKind.DivideOperator, _operator.ToString(), null!);
        }
        else if (_operator == '^')
        {
            Next();
            return new Token(TokenKind.Power, _operator.ToString(), null!);
        }
        else if (_operator == '%')
        {
            Next();
            return new Token(TokenKind.Modulus, _operator.ToString(), null!);
        }
        else if (_operator == '@')
        {
            Next();
            return new Token(TokenKind.Concat, _operator.ToString(), null!);
        }
        else if (_operator == '<' && LookAhead(1) == '=')
        {
            Next();
            Next();
            return new Token(TokenKind.LessOrEquals, "<=", null!);
        }
        else if (_operator == '<')
        {
            Next();
            return new Token(TokenKind.LessThan, _operator.ToString(), null!);
        }
        else if (_operator == '>' && LookAhead(1) == '=')
        {
            Next();
            Next();
            return new Token(TokenKind.GreatherOrEquals, ">=", null!);
        }
        else if (_operator == '>')
        {
            Next();
            return new Token(TokenKind.GreatherThan, _operator.ToString(), null!);
        }
        else if (_operator == '!' && LookAhead(1) == '=')
        {
            Next();
            Next();
            return new Token(TokenKind.NotEquals, "!=", null!);
        }
        else if (_operator == '!')
        {
            Next();
            return new Token(TokenKind.Not, _operator.ToString(), null!);
        }
        else if (_operator == '=' && LookAhead(1) == '=')
        {
            Next();
            Next();
            return new Token(TokenKind.EqualsTo, "==", null!);
        }
        else if (_operator == '=')
        {
            Next();
            return new Token(TokenKind.Equals, null!, null!);
        }
        else if (_operator == '&')
        {
            Next();
            return new Token(TokenKind.And, null!, null!);
        }
        else
        {
            Next();
            return new Token(TokenKind.Or, _operator.ToString(), null!);
        }
    }

    private Token PuncutatorKind()
    {
        char punctuator = sourceCode[currentPosition];
        switch (punctuator)
        {
            case '(':
                Next();
                return new Token(TokenKind.LeftParenthesis, punctuator.ToString(), null!);

            case ')':
                Next();
                return new Token(TokenKind.RightParenthesis, punctuator.ToString(), null!);

            case ',':
                Next();
                return new Token(TokenKind.Comma, punctuator.ToString(), null!);

            case ';':
                Next();
                return new Token(TokenKind.Semicolon, punctuator.ToString(), null!);

            case ':':
                Next();
                return new Token(TokenKind.Colon, punctuator.ToString(), null!);

            case '"':
                Next();
                return new Token(TokenKind.Quote, punctuator.ToString(), null!);

            default:
                Next();
                return new Token(TokenKind.FullStop, punctuator.ToString(), null!);
        }
    }

    private Token KeyWord(string idkind)
    {
        switch (idkind)
        {
            case "let":
                return new Token(TokenKind.letKeyWord, null!, null!);
            case "in":
                return new Token(TokenKind.inKeyWord, null!, null!);
            case "function":
                return new Token(TokenKind.functionKeyWord, null!, null!);
            case "true":
                return new Token(TokenKind.trueKeyWord, null!, true);
            case "false":
                return new Token(TokenKind.falseKeyWord, null!, false);
            case "if":
                return new Token(TokenKind.ifKeyWord, null!, null!);
            default:
                return new Token(TokenKind.elseKeyWord, null!, null!);
        }
    }

    #endregion

    #region Utility Functions 

    private void Next()
    {
        currentPosition++;
    }
    private char LookAhead(int positions)
    {
        return sourceCode[currentPosition + positions];
    }

    private static bool IsLetter(char c)
    {
        return char.IsLetter(c) || c == '_';
    }

    private static bool IsLetterOrDigit(char c)
    {
        return char.IsLetterOrDigit(c) || c == '_';
    }

    private static bool IsOperator(char currentChar)
    {
        List<char> Operators = new()
        {
            '+', '-', '*', '/', '^','%','@',
            '=','<','>','!','|','&'
        };

        foreach (var _operator in Operators)
            if (currentChar == _operator)
                return true;

        return false;
    }

    private static bool IsPunctuator(char currentChar)
    {
        List<char> Punctuators = new()
        {
            '(', ')', ';', ',', '{', '}',
            '[', ']','.','"',':'
        };

        foreach (var puncutator in Punctuators)
            if (currentChar == puncutator)
                return true;
        return false;
    }

    private static bool IsKeyWord(string idkind)
    {
        List<string> keywords = new()
        {
            "let", "function",  "else",
            "in" , "if",        "true",
            "false"
        };

        foreach (var keyword in keywords)
            if (idkind == keyword)
                return true;
        return false;
    }

    #endregion
}