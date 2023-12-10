public abstract class Token
{
    public TokenKind Kind { get; set; }

    public Token(TokenKind kind)
    {
        Kind = kind;
    }

    public TokenKind GetKind() => Kind;
    
    public void SetKind(TokenKind kind) => Kind = kind;

    public abstract string GetName();

    public abstract void SetName(string name);

    public abstract object GetValue();

    public abstract void SetValue(object value);


    public override string ToString() => $"{Kind}";
}

public class Keyword : Token
{
    public Keyword(TokenKind kind) : base(kind)
    {
        Kind = kind;
    }

    public override string GetName() => Kind.ToString();

    public override object GetValue() => throw new NotImplementedException();

    public override void SetName(string name) => throw new NotImplementedException();

    public override void SetValue(object value) => throw new NotImplementedException();

}

public class CommonToken : Token
{
    public string Representation { get; set; }

    public CommonToken(TokenKind kind, string representation) : base(kind)
    {
        Representation = representation;
    }
    public override string GetName() => Representation;

    public override void SetName(string name) => Representation = name;

    public override object GetValue() => throw new NotImplementedException();

    public override void SetValue(object value) => throw new NotImplementedException();

    public override string ToString() => $"{base.Kind}: {Representation}";
}

public class Data : Token
{
    public object Value { get; set; }

    public Data(TokenKind kind, object value) : base(kind)
    {
        Value = value;
    }

    public override string GetName() => Value.ToString()!;

    public override void SetName(string name) => throw new NotImplementedException();

    public override object GetValue() => Value;

    public override void SetValue(object value) => Value = value;

    public override string ToString() => $"{base.Kind}: {Value}";
}