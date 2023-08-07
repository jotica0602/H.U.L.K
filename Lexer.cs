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
        Console.Write("Write your arithmetic expression here>: ");
        string sourceCode = Console.ReadLine();
        // try let a = 2^(3+2)^2, b = 3*4 in a*b;
        var Lexer = new Lexer(sourceCode);

        List<Token> tokens = Lexer.Tokenize();
        // Console.WriteLine(String.Join('\n', tokens));
        Parser parser = new(tokens);
        parser.Parse();

        parser.variables.Clear();
    }

    #endregion

    #region  Lexer Object
    private readonly string sourceCode;
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
                tokens.Add(new Token(TokenKind.Unknown, currentChar.ToString()));
                currentPosition++;
            }
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
        return new Token(TokenKind.Number, number);
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
            return new Token(TokenKind.Identifier, idkind);
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
        return new Token(TokenKind.String, str);
    }

    private Token OperatorKind()
    {
        char _operator = sourceCode[currentPosition];

        if (_operator == '+')
        {
            Next();
            return new Token(TokenKind.PlusOperator, _operator.ToString());
        }
        else if (_operator == '-')
        {
            Next();
            return new Token(TokenKind.MinusOperator, _operator.ToString());
        }
        else if (_operator == '*')
        {
            Next();
            return new Token(TokenKind.MultOperator, _operator.ToString());
        }
        else if (_operator == '/')
        {
            Next();
            return new Token(TokenKind.DivideOperator, _operator.ToString());
        }
        else if (_operator == '^')
        {
            Next();
            return new Token(TokenKind.Power, _operator.ToString());
        }
        else if (_operator == '@')
        {
            Next();
            return new Token(TokenKind.Concat, _operator.ToString());
        }
        else
        {
            Next();
            return new Token(TokenKind.EqualsOperator, _operator.ToString());
        }
    }

    private Token PuncutatorKind()
    {
        char punctuator = sourceCode[currentPosition];

        if (punctuator == '(')
        {
            Next();
            return new Token(TokenKind.LeftParenthesis, punctuator.ToString());
        }
        else if (punctuator == ')')
        {
            Next();
            return new Token(TokenKind.RightParenthesis, punctuator.ToString());
        }
        else if (punctuator == ',')
        {
            Next();
            return new Token(TokenKind.Comma, punctuator.ToString());
        }
        else if (punctuator == ';')
        {
            Next();
            return new Token(TokenKind.Semicolon, punctuator.ToString());
        }
        else if (punctuator == ':')
        {
            Next();
            return new Token(TokenKind.Colon, punctuator.ToString());
        }
        else if (punctuator == '"')
        {
            Next();
            return new Token(TokenKind.Quote, punctuator.ToString());
        }
        else
        {
            Next();
            return new Token(TokenKind.FullStop, punctuator.ToString());
        }
    }

    private Token KeyWord(string idkind)
    {
        if (idkind == "let")
            return new Token(TokenKind.letKeyWord, idkind);
        else if (idkind == "function")
            return new Token(TokenKind.function, idkind);
        else
            return new Token(TokenKind.inKeyWord, idkind);
    }

    #endregion

    #region Utility Functions 

    private void Next()
    {
        currentPosition++;
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
        List<char> Operators = new(){
            '+', '-', '*', '/', '^', '=', '!', '@',
            '='
        };

        foreach (var _operator in Operators)
            if (currentChar == _operator)
                return true;

        return false;
    }

    private static bool IsPunctuator(char currentChar)
    {
        List<char> Punctuators = new(){
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
        List<string> keywords = new(){
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

# region Token Objects
// Token Object
public class Token
{
    public TokenKind Kind { get; set; }
    public string Value { get; set; }
    public Token(TokenKind kind, string value)
    {
        Kind = kind;
        Value = value;
    }
    // Show TokenKind and Value
    public override string ToString()
    {
        return $"{Kind}: {Value}";
    }
}

public class VarToken : Token
{
    public object VarValue { get; set; }
    public VarToken(TokenKind kind, string value, object varValue) : base(kind, value)
    {
        VarValue = varValue;
    }
    public override string ToString()
    {
        return $"{Kind}Kind Name:{Value} VarValue: {VarValue}";
    }
}

// Token Kinds

public enum TokenKind
{
    // Identifiers
    Identifier,
    // Variable
    Variable,
    // KeyWords
    letKeyWord, inKeyWord, function,
    // Strings
    String,
    // Number
    Number,
    //Operators
    PlusOperator, MinusOperator, MultOperator, DivideOperator,
    Equals, Negation, Power, Concat, EqualsOperator,
    // Punctuators
    LeftParenthesis, RightParenthesis,
    LeftBracket, RightBracket,
    LeftCurlyBracket, RightCurlyBracket,
    ////////////////////////////////////
    Comma, Colon, Semicolon, FullStop,
    Quote,

    EndOfFile,
    Unknown
}
#endregion



