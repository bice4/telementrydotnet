namespace TelemtryNetProject.Contracts.Order.Api.v1.Models;

public class OrderDto
{
    public string Id { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public string UserId { get; private set; }

    public List<OrderItemDto> OrderItems { get; private set; }

    public OrderDto(string id, DateTimeOffset createdAt, DateTimeOffset updatedAt, string userId,
        List<OrderItemDto> orderItems)
    {
        Id = id;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        UserId = userId;
        OrderItems = orderItems;
    }

    public override string ToString()
    {
        return
            $"Id: {Id}, CreatedAt: {CreatedAt}, UpdatedAt: {UpdatedAt}, UserId: {UserId}, OrderItems: {OrderItems.Count}";
    }
}