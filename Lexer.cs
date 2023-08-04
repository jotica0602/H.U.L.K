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
    #region Tests
    static void Main (string[] args)
    {   
        Console.Write("Write your mathematic expression here>: ");
        string sourceCode = Console.ReadLine();
        var Lexer = new Lexer(sourceCode);
        List <Token> tokens = Lexer.Tokenize();
        Console.WriteLine(String.Join('\n', tokens));
        Parser parser = new(tokens);
        double result = parser.Parse();
        Console.WriteLine($"Result>: {result}");
    }

    #endregion

    private readonly string sourceCode;
    private int currentPosition;

    public Lexer(string sourceCode){
        this.sourceCode = sourceCode;
        currentPosition = 0;
    }

    // We need to split the Tokens
    // So I created a function named Tokenize, wich returns a List of Tokens containing all the Tokens
    public List<Token> Tokenize(){

        // Initialize List
        List<Token> tokens = new();

        while (currentPosition < sourceCode.Length){

            // The character we are looking at is the current position of the sourceCode
            char currentChar = sourceCode[currentPosition];

            // If there's any white space we move on
            if (char.IsWhiteSpace(currentChar)){
                currentPosition++;
                continue;
            }

            // Identifiers
            if (IsLetter(currentChar))
                tokens.Add(IdKind());
            // Digits
            else if(char.IsDigit(currentChar))
                tokens.Add(NumberKind());
            // Operators 
            else if (IsOperator(currentChar))
                tokens.Add(OperatorKind());
            // Punctuators
            else if (IsPunctuator(currentChar))
                tokens.Add(PuncutatorKind());
            // unknown tokens
            else{
                tokens.Add(new Token(TokenKind.Unknown,currentChar.ToString()));
                currentPosition++;
            }
        }
        return tokens;
    }
    # region TokenKind Adder Functions

    private Token NumberKind(){

        string number = "";
        while((currentPosition < sourceCode.Length) && (char.IsDigit(sourceCode[currentPosition]) || char.Equals(sourceCode[currentPosition],','))){
            number += sourceCode[currentPosition];
            currentPosition++;
        }
            // Token (Token Kind, Number Value)
        return new Token(TokenKind.Number, number);
    }

    private Token IdKind(){

        string idkind = "";

        while (currentPosition < sourceCode.Length && (IsLetterOrDigit(sourceCode[currentPosition]) || sourceCode[currentPosition] == '_')){
            idkind+=sourceCode[currentPosition];
            currentPosition++;
        }

        return new Token(TokenKind.Identifier, idkind);
    }

    private Token OperatorKind(){
        char _operator= sourceCode[currentPosition];

        if(_operator == '+'){
            Next();
            return new Token(TokenKind.PlusOperator, _operator.ToString());
        }
        else if (_operator == '-'){           
            Next();
            return new Token(TokenKind.MinusOperator, _operator.ToString());
        }
        else if (_operator == '*'){
            Next();
            return new Token(TokenKind.MultOperator,_operator.ToString());
        }
        else if (_operator == '/'){
            Next();
            return new Token(TokenKind.DivideOperator,_operator.ToString());
        }
        else if(_operator == '!'){
            Next();
            return new Token(TokenKind.NegationOperator,_operator.ToString());
        }
        else if (_operator == '^'){
            Next();
            return new Token(TokenKind.PowerOperator,_operator.ToString());
        }
        else{
            Next();
            return new Token(TokenKind.EqualsOperator,_operator.ToString());
        }
    }

    private Token PuncutatorKind(){
        char punctuator= sourceCode[currentPosition];

        if( punctuator == '(' ){
            Next();
            return new Token(TokenKind.LeftParenthesis, punctuator.ToString());
        }
        else if ( punctuator == ')' ){           
            Next();
            return new Token(TokenKind.RightParenthesis, punctuator.ToString());
        }
        else if (punctuator == '{'){
            Next();
            return new Token(TokenKind.LeftCurlyBracket,punctuator.ToString());
        }
        else if (punctuator == '}'){
            Next();
            return new Token(TokenKind.RightCurlyBracket,punctuator.ToString());
        }
        else if (punctuator == '['){
            Next();
            return new Token(TokenKind.LeftBracket,punctuator.ToString());
        }
        else if (punctuator == ']'){
            Next();
            return new Token(TokenKind.RightBracket,punctuator.ToString());
        }
        else if (punctuator == ','){
            Next();
            return new Token(TokenKind.Comma,punctuator.ToString());
        }
        else if (punctuator == ';'){
            Next();
            return new Token(TokenKind.Semicolon,punctuator.ToString());
        }
        else if (punctuator== ';'){
            Next();
            return new Token(TokenKind.Colon,punctuator.ToString());
        }
        else if (punctuator == '"'){
            Next();
            return new Token(TokenKind.Quote,punctuator.ToString());
        }
        else{
            Next();
            return new Token(TokenKind.FullStop,punctuator.ToString());
        }
    }

    #endregion

    #region Utility Functions 

    private void Next(){
        currentPosition++;
    }

    private static bool IsLetter(char c){
        return char.IsLetter(c) || c == '_';
    }

    private static bool IsLetterOrDigit(char c){
        return char.IsLetterOrDigit(c) || c == '_';
    }
    
    private static bool IsOperator(char currentChar){
        List<char> Operators  = new(){
            '+', '-', '*', '/', '^', '=', '!'
        };

        foreach(var _operator in Operators)
            if(currentChar==_operator)
                return true;
        
        return false;
    }

    private static bool IsPunctuator(char currentChar){
        List<char> Punctuators  = new(){
            '(', ')', ';', ',', '{', '}',
            '[', ']','.','"',':'
        };

        foreach(var puncutator in Punctuators)
            if(currentChar==puncutator)
                return true;
        return false;
    }


    #endregion
}

// Token Object
public class Token{
    public TokenKind Kind { get; private set; }
    public string Value { get; private set; }
    public Token(TokenKind kind, string value){
        Kind = kind;
        Value = value;
    }
    // Show TokenKind and Value
    public override string ToString(){
        return $"{Kind}: {Value}";
    }
}

// Token Kinds

public enum TokenKind{
    
    Identifier,
    Number,

    //Operators
    PlusOperator,
    MinusOperator,
    MultOperator,
    DivideOperator,
    EqualsOperator,
    NegationOperator,
    PowerOperator,

    // Punctuators
    LeftParenthesis,
    RightParenthesis,
    LeftBracket,
    RightBracket,
    LeftCurlyBracket,
    RightCurlyBracket,
    Comma,
    Colon,
    Semicolon,
    FullStop,
    Quote,

    // KeyWords
    letKeyWord,
    inKeyWord,

    EndOfFile,
    Unknown
}




