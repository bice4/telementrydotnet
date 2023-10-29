namespace Contracts.Requests;

public class MainRequest
{
    public string Name { get; set; }
    public Guid Id { get; set; }

    public MainRequest(string name, Guid id)
    {
        Name = name;
        Id = id;
    }

    public override string ToString() =>
        $"Name: {Name}, Id: {Id}";
}