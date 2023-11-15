namespace Contracts.Requests;

public class MainRequest
{
    public string Name { get; set; }
    public Guid Id { get; set; }
    public string ReferenceId { get; set; }
    public int Age { get; set; }

    public MainRequest(string name, Guid id, int age, string referenceId)
    {
        Name = name;
        Id = id;
        Age = age;
        ReferenceId = referenceId;
    }
}