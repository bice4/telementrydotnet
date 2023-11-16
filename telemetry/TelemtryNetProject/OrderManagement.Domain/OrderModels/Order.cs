using MongoDB.Bson;

namespace OrderManagement.Domain.OrderModels;

public class Order
{
    public ObjectId Id { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public string UserId { get; private set; }

    public List<OrderItem> Items { get; private set; }
    
    public string InvoiceId { get; private set; } = string.Empty;

    public double TotalPrice => Items.Sum(x => x.Price * x.Quantity);
    public int TotalQuantity => Items.Sum(x => x.Quantity);


    public Order(string userId, List<OrderItem> items)
    {
        Id = ObjectId.GenerateNewId();
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
        Items = items;
        UserId = userId;
    }

    public void SetInvoiceId(string invoiceId)
    {
        InvoiceId = invoiceId;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
    
    public override string ToString()
    {
        return $"Id: {Id}, CreatedAt: {CreatedAt}, UpdatedAt: {UpdatedAt}, UserId: {UserId}, Items: {Items.Count}";
    }
}