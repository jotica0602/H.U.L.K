public class Funct
{
    public string Name { get; set; }
    public List<(string,object)> Args { get; set; }
    public List<Token> Instructions { get; set; }

    public static List<Funct> functions = new List<Funct>();

    public Funct(string name, List<(string,object)>args, List<Token> instructions)
    {
        Name = name;
        Args = args;
        Instructions = instructions;
    }

    public object Execute()
    {
        List<Token> variables = new List<Token>();
        foreach (var token in Instructions)
        {
            if (token.Kind == TokenKind.Identifier && token.Value != null && !variables.Contains(token))
            {
                variables.Add(token);
            }
        }
        Parser parser = new Parser(Instructions, variables);
        return parser.ParseExpression();

    }

    public override string ToString()
    {
        return $"Name: {Name} | Arguments:{String.Join(" ", Args)} | Instructions: {String.Join(" ", Instructions)}";
    }
}