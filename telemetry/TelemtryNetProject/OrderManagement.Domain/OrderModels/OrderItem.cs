using MongoDB.Bson;

namespace OrderManagement.Domain.OrderModels;

public class OrderItem
{
    public ObjectId Id { get; set; }
    public string ReferenceId { get; private set; }
    public string Name { get; private set; }
    public int Quantity { get; private set; }
    public double Price { get; private set; }

    public OrderItem(string referenceId, string name, int quantity, double price)
    {
        Id = ObjectId.GenerateNewId();
        ReferenceId = referenceId;
        Name = name;
        Quantity = quantity;
        Price = price;
    }

    public override string ToString()
    {
        return $"Id: {Id}, ReferenceId: {ReferenceId}, Name: {Name}, Quantity: {Quantity}, Price: {Price}";
    }
}