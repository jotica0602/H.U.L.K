namespace ClassLibrary;

public abstract class AtomExpression : Expression
{
    public override Scope? Scope { get; set; }
    public AtomExpression(Scope scope) : base(scope) { }

    public override string ToString() => $"{Value}";
}

