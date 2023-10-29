namespace MetricsPlayground;

public class LoadRequest
{
    public string Prefix { get; set; }

    public int Count { get; set; }

    public LoadRequest(string prefix, int count)
    {
        Prefix = prefix;
        Count = count;
    }
}