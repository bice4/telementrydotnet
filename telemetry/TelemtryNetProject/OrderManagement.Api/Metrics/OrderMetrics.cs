using System.Diagnostics.Metrics;

namespace OrderManagementApi.Metrics;

public class OrderMetrics
{
    private Counter<int> OrderAddedCounter { get; }
    private Counter<int> OrderItemsAddedCounter { get; }
    private Histogram<double> OrdersPriceHistogram { get; }
    private Histogram<double> OrdersQuantityHistogram { get; }
    public string MetricName { get; }

    public OrderMetrics(string meterName = "OrderManagement")
    {
        var meter = new Meter(meterName);
        MetricName = meterName;

        OrdersPriceHistogram =
            meter.CreateHistogram<double>("orders-price", "USD", "Price distribution of order items");
        OrdersQuantityHistogram =
            meter.CreateHistogram<double>("orders-quantity", "pcs", "Quantity distribution of order items");

        OrderAddedCounter = meter.CreateCounter<int>("OrderAdded");
        OrderItemsAddedCounter = meter.CreateCounter<int>("OrderItemsAdded");
    }

    public void AddOrder() => OrderAddedCounter.Add(1);
    public void AddOrderItems(int count) => OrderItemsAddedCounter.Add(count);
    public void RecordOrderTotalPrice(double price) => OrdersPriceHistogram.Record(price);
    public void RecordOrderTotalQuantity(double quantity) => OrdersQuantityHistogram.Record(quantity);
}