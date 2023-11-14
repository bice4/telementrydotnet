using System.ComponentModel.DataAnnotations;
using TelemetryDotNet.Contracts.Order.Api.v1.Request;
using TelemetryDotNet.Contracts.UserManagement.Api.V1.Requests;

namespace TelemetryDotNet.Contracts.ApiGateway.Api.v1.Requests;

public class PlaceOrderRequest
{
    [Required(ErrorMessage = "Order information is required")]
    public CreateOrderRequest OrderRequest { get; set; }

    [Required(ErrorMessage = "User information is required")]
    public CreateUserRequest UserRequest { get; set; }

    public PlaceOrderRequest(CreateOrderRequest orderRequest, CreateUserRequest userRequest)
    {
        OrderRequest = orderRequest;
        UserRequest = userRequest;
    }
}