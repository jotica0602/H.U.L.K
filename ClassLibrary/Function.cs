namespace ClassLibrary
{
    public class Funct
    {
        public List<(string, object)> Args { get; set; }

        public List<Token> Body { get; set; }

        public Funct(List<(string, object)> args, List<Token> body)
        {
            Args = args;
            Body = body;
        }

        public object Execute()
        {
            Dictionary<string, object> variables = new Dictionary<string, object>();

            // Get function variables 
            foreach (var arg in Args)
            {
                variables.Add(arg.Item1, arg.Item2);
            }

            Parser parser = new Parser(Body, new Dictionary<string, object>(variables));
            return parser.ParseExpression();
        }

        public override string ToString()
        {
            return $"Arguments:{string.Join(" ", Args)} | Instructions: {string.Join(" ", Body)}";
        }
    }
}