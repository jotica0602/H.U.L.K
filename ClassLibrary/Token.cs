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

public class PureToken : Token
{
    public PureToken(TokenKind kind) : base(kind)
    {
        Kind = kind;
    }

    public override string GetName() => throw new NotImplementedException();

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

    public override string GetName() => throw new NotImplementedException();

    public override void SetName(string name) => throw new NotImplementedException();

    public override object GetValue() => Value;

    public override void SetValue(object value) => Value = value;

    public override string ToString() => $"{base.Kind}: {Value}";
}

public class Variable : CommonToken
{
    public object Value { get; set; }

    public Variable(TokenKind kind, string representation, object value) : base(kind, representation)
    {
        Value = value;
    }

    public override object GetValue() => Value;

    public override void SetValue(object value) => Value = value;

    public override string ToString() => $"{base.Kind} {base.Representation}={Value}";
}
