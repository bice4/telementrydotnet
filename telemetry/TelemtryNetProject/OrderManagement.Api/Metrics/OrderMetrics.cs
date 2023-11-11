using System.Diagnostics.Metrics;

namespace OrderManagementApi.Metrics;

public class OrderMetrics
{
    private Counter<int> OrderAddedCounter { get; }
    private Counter<int> OrderItemsAddedCounter { get; }

    public string MetricName { get; }

    public OrderMetrics(string meterName = "OrderManagement")
    {
        var meter = new Meter(meterName);
        MetricName = meterName;

        OrderAddedCounter = meter.CreateCounter<int>("OrderAdded");
        OrderItemsAddedCounter = meter.CreateCounter<int>("OrderItemsAdded");
    }

    public void AddOrder() => OrderAddedCounter.Add(1);
    public void AddOrderItems(int count) => OrderItemsAddedCounter.Add(count);
}