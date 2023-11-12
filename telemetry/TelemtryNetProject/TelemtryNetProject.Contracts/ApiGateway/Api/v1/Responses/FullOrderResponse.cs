using TelemtryNetProject.Contracts.Order.Api.v1.Models;
using TelemtryNetProject.Contracts.UserManagement.Api.V1.Models;

namespace TelemtryNetProject.Contracts.ApiGateway.Api.v1.Responses;

public class FullOrderResponse
{
    public UserFullDto User { get; set; }

    public OrderDto Order { get; set; }

    public FullOrderResponse(UserFullDto user, OrderDto order)
    {
        User = user;
        Order = order;
    }
}