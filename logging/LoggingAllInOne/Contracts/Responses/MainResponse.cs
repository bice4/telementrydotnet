namespace Contracts.Responses;

public class MainResponse
{
    public string Answer { get; set; }

    public MainResponse(string answer)
    {
        Answer = answer;
    }

    public override string ToString() =>
        $"Answer: {Answer}";
}