using System.Globalization;

namespace ClassLibrary;

public class LetIn : Expression
{
    public Expression Execution;
    public override ExpressionKind Kind { get; set; }
    public override object? Value { get; set; }

    public LetIn(ExpressionKind kind, Expression execution) : base(kind)
    {
        Execution = execution;
    }

    public override void CheckSemantic()
    {
        return;
    }

    public override object? Evaluate(Scope scope)
    {
        Value = Execution.Evaluate(scope);
        if (Value is bool)
            Kind = ExpressionKind.Bool;
        if (Value is string)
            Kind = ExpressionKind.String;
        if (Value is double)
            Kind = ExpressionKind.Number;

        scope.Vars.Remove(scope.Vars.Last());
        return Value;
    }

    public override object? GetValue() => Value;

    public override void VisitNode()
    {
        throw new NotImplementedException();
    }
}