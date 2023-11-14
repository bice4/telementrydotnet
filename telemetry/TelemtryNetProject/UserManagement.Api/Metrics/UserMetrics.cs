using System.Diagnostics.Metrics;

namespace UserManagement.Metrics;

public class UserMetrics
{
    /// <summary>
    /// Counter for new users added
    /// </summary>
    private Counter<int> UserAddedCounter { get; }
    private Counter<int> UsersAddedPerCountryCounter { get; }
    private Counter<int> UsersAddedPerAgeCounter { get; }
    
    public string MetricName { get; }

    public UserMetrics(string meterName = "UserManagement")
    {
        var meter = new Meter(meterName);
        MetricName = meterName;

        UserAddedCounter = meter.CreateCounter<int>("UserAdded");
        UsersAddedPerAgeCounter = meter.CreateCounter<int>("UsersAddedPerAge");
        UsersAddedPerCountryCounter = meter.CreateCounter<int>("UsersAddedPerCountry");
    }

    public void IncUserCounters(string country, int age)
    {
        UserAddedCounter.Add(1);
        UsersAddedPerAgeCounter.Add(1, new KeyValuePair<string, object?>("age", age));
        UsersAddedPerCountryCounter.Add(1, new KeyValuePair<string, object?>("country", country));
    }
    
    public void DecUserTotalCounter()
    {
        UserAddedCounter.Add(-1);
    }
}