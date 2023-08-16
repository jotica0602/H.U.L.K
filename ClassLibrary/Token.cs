
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
        if (Value != null & Name == null) return $"{Value}";
        else if (Name != null & Value == null) return $"{Name}";
        else if (Kind == TokenKind.String) return $"{Value}";
        else if (Name == null & Value==null) return $"{Kind}";
        else return $"{Name} = {Value}";
    }
}

#endregion
