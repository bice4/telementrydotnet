using System.ComponentModel.DataAnnotations;
using TelemtryNetProject.Contracts.Order.Api.v1.Models;

namespace TelemtryNetProject.Contracts.Order.Api.v1.Request;

public class CreateOrderRequest
{
    [Required(ErrorMessage = "Order items are required")]
    public List<OrderItem> OrderItems { get; set; }

    public CreateOrderRequest(List<OrderItem> orderItems)
    {
        OrderItems = orderItems;
    }
}