namespace ClassLibrary;
public class Scope
{
    public Dictionary<string, Expression> Vars { get; set; }
    // public Dictionary<string, Function> Functions { get; set; }

    public Scope? Parent { get; set; }

    public Scope()
    {
        Vars = new Dictionary<string,Expression>();
        // Functions = new Dictionary<string, Function>();
    }

    public Scope MakeChild()
    {
        Scope child = new Scope();
        child.Parent = this;
        return child;
    }
}