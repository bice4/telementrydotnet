namespace TelemetryDotNet.Contracts.Order.Api.v1.Models;

public class OrderItemDto
{
    public string Id { get; set; }
    public string ReferenceId { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }

    public OrderItemDto(string id, string referenceId, string name, int quantity)
    {
        Id = id;
        ReferenceId = referenceId;
        Name = name;
        Quantity = quantity;
    }

    public override string ToString()
    {
        return $"Id: {Id}, ReferenceId: {ReferenceId}, Name: {Name}, Quantity: {Quantity}";
    }
}