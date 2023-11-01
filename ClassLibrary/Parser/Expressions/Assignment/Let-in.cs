using System.Globalization;

namespace ClassLibrary;

public class LetIn : Expression
{
    public override Scope? Scope { get; set; }
    public Expression Instruction;
    public override ExpressionKind Kind { get; set; }
    public override object? Value { get; set; }

    public LetIn(Expression instruction, Scope scope) : base(scope)
    {
        Kind = ExpressionKind.Temp;
        Instruction = instruction;
    }

    public override void Evaluate(Scope scope)
    {
        Instruction.Evaluate(Scope!);
        Value = Instruction.GetValue();
        if (Value is bool)   { Kind = ExpressionKind.Bool;   }
        if (Value is string) { Kind = ExpressionKind.String; }
        if (Value is double) { Kind = ExpressionKind.Number; }
    }

    public override object? GetValue() => Value;
}