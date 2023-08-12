
#region Token Objects
// Token Object
public class Token
{
    public TokenKind Kind { get; set; }
    public string Name { get; set; }
    public object Value { get; set; }
    public Token(TokenKind kind, string name, object value)
    {
        Kind = kind;
        Name = name;
        Value = value;

    }
    // Show TokenKind Value Properties
    public override string ToString()
    {
        if (Name == null) return $"{Kind}: {Value}";
        else if (Value == null) return $"{Kind}: {Name}";
        else return $"{Kind}: {Name} = {Value}";
    }
}

#endregion
