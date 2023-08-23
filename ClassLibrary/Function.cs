public class Funct
{
    public List<(string, object)> Args { get; set; }
    
    public List<Token> Body { get; set; }
    
    public Funct(List<(string, object)> args, List<Token> body)
    {
        Args = args;
        Body = body;
    }

    public object Clone()
    {
        Funct clone = new Funct(new List<(string, object)>(), new List<Token>());

        foreach (var arg in Args)
        {
            clone.Args.Add((arg.Item1, arg.Item2));
        }

        foreach (var token in Body)
        {
            clone.Body.Add(token.Clone());
        }

        return clone;
    }

    public object Execute()
    {
        Parser parser = new Parser(Body, new List<Funct>());
        return parser.ParseExpression();
    }

    public override string ToString()
    {
        return $"Arguments:{String.Join(" ", Args)} | Instructions: {String.Join(" ", Body)}";
    }
}