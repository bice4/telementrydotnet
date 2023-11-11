using System.ComponentModel.DataAnnotations;
using TelemtryNetProject.Contracts.Order.Api.v1.Models;
using TelemtryNetProject.Contracts.UserManagement.Api.V1.Requests;

namespace TelemtryNetProject.Contracts.Order.Api.v1.Request;

public class CreateOrderRequest
{
    [Required(ErrorMessage = "Order items are required")]
    public List<OrderItem> OrderItems { get; set; }
}