public enum TokenKind
{
    Identifier,

    // KeyWords
    LetKeyWord,

    InKeyWord,

    FunctionKeyWord,

    IfKeyWord, 
    
    ElseKeyWord, 
    
    // Data Types
    String,

    Number,

    TrueKeyWord,

    FalseKeyWord,

    // Arithmetic Operators
    Addition,

    Substraction,

    Multiplication,

    Division,

    Power,

    Modulus,

    // Boolean Operators
    Equals,

    Concat,

    And,

    Or,

    EqualsTo,

    LessOrEquals,

    LessThan,

    GreatherOrEquals,

    GreatherThan,

    NotEquals,

    Not,

    // Punctuators
    LeftParenthesis,

    RightParenthesis,

    Comma,

    Colon,

    Semicolon,

    FullStop,

    Quote,

    Arrow,

    // Utility 
    EndOfFile,

    Unknown,

}