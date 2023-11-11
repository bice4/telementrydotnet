using System.Text.Json;
using TelemtryNetProject.Contracts.Order.Api.v1.Request;
using TelemtryNetProject.Contracts.UserManagement.Api.V1.Requests;

namespace TelemtryNetProject.Contracts.Order.RabbitMq.v1.Requests;

public class PlaceOrderModel
{
    public CreateUserRequest User { get; set; }
    public CreateOrderRequest Order { get; set; }

    public string CorrelationId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public PlaceOrderModel(CreateUserRequest user, CreateOrderRequest order, string correlationId)
    {
        User = user;
        Order = order;
        CorrelationId = correlationId;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}