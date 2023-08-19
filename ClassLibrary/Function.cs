public class Funct
{
    public string Name { get; set; }

    public List<(string, object)> Args { get; set; }

    public List<Token> Body { get; set; }
    
    public Funct(string name, List<(string, object)> args, List<Token> body)
    {
        Name = name;
        Args = args;
        Body = body;
    }

    public object Clone()
    {
        Funct clone = new Funct(Name, new List<(string, object)>(), new List<Token>());

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
        return $"Name: {Name} | Arguments:{String.Join(" ", Args)} | Instructions: {String.Join(" ", Body)}";
    }
}