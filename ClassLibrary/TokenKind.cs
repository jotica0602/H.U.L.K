
// Token Kinds
public enum TokenKind
{
    // <summary>
    // There are 2 kind of identifiers:
    // Variables and Functions
    // </summary>

    // Identifiers
    Identifier,
    // Variable
    Variable,
    // KeyWords
    letKeyWord, inKeyWord, functionKeyWord, ifKeyWord, elseKeyWord,

    // <summary>
    // There are 3 data types in H.U.L.K:
    //  Strings, Numbers and Bool
    // </summary>

    String, Number, trueKeyWord, falseKeyWord,

    // <summary>
    // Operators:
    // There are 3 kind of operators in H.U.L.K:
    // Arithmetic Operators a String Operator and Bool Operators 
    // </summary>

    // Arithmetic Operators
    PlusOperator, MinusOperator, MultOperator, DivideOperator, Power, Modulus, Equals,
    // String Operator
    Concat,
    // Bool Operators
    And, Or, EqualsTo, LessOrEquals, LessThan, GreatherOrEquals, GreatherThan, NotEquals, Not,

    // Punctuators
    LeftParenthesis, RightParenthesis, Comma, Colon, Semicolon, FullStop, Quote,

    EndOfFile,
    Unknown
}