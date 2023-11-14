using System.Diagnostics.Metrics;

namespace UserManagement.Metrics;

public class UserMetrics
{
    /// <summary>
    /// Counter for new users added
    /// </summary>
    private Counter<int> UserAddedCounter { get; }

    public string MetricName { get; }

    public UserMetrics(string meterName = "UserManagement")
    {
        var meter = new Meter(meterName);
        MetricName = meterName;

        UserAddedCounter = meter.CreateCounter<int>("UserAdded");
    }

    public void UpdateUserMetrics(int val, string city) =>
        UserAddedCounter.Add(val, new KeyValuePair<string, object?>("City", city));
}