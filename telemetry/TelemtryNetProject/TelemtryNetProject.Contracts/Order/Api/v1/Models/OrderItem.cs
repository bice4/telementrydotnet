using System.ComponentModel.DataAnnotations;

namespace TelemtryNetProject.Contracts.Order.Api.v1.Models;

public class OrderItem
{
    [Required(ErrorMessage = "Id is required")]
    public string Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }

    [Range(1, Int32.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int Quantity { get; set; }

    public OrderItem(string id, string name, int quantity)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
    }
}