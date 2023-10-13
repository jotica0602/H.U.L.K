public enum TokenKind
{
    Identifier,

    // KeyWords
    letKeyWord,

    inKeyWord,

    functionKeyWord,

    ifKeyWord, 
    
    elseKeyWord, 
    
    PI, 
    
    E,

    e,

    // Data Types
    String,

    Number,

    trueKeyWord,

    falseKeyWord,

    // Arithmetic Operators
    PlusOperator,

    MinusOperator,

    MultOperator,

    DivideOperator,

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

    // Functions

    sin,

    cos,

    log,

    rand,
   
    print,
    
    // Utility 
    EndOfFile,

    Unknown,

}