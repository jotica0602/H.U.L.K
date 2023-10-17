namespace ClassLibrary;
public class Scope
{
    public List<Dictionary<string, Expression>> Vars { get; set; }

    public Scope? Parent { get; set; }

    public Scope()
    {
        Vars = new List<Dictionary<string, Expression>>();
    }

    public Scope MakeChild()
    {
        Scope child = new Scope();
        child.Parent = this;
        return child;
    }
}
