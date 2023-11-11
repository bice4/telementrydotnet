using System.ComponentModel.DataAnnotations;

namespace TelemtryNetProject.Contracts.UserManagement.Api.V1.Requests;

public class UpdateUserAddressRequest
{
    [Required(ErrorMessage = $"{nameof(UserId)} is required")]
    public string UserId { get; set; }

    [Required(ErrorMessage = $"{nameof(Country)} is required")]
    public string Country { get; set; }

    [Required(ErrorMessage = $"{nameof(City)} is required")]
    public string City { get; set; }

    [Required(ErrorMessage = $"{nameof(Street)} is required")]
    public string Street { get; set; }

    [Required(ErrorMessage = $"{nameof(ZipCode)} is required")]
    public string ZipCode { get; set; }

    public string? ApartmentNumber { get; set; }

    public string? BuildingNumber { get; set; }

    public string? Floor { get; set; }

    public UpdateUserAddressRequest(string userId, string country, string city, string street, string zipCode,
        string? apartmentNumber, string? buildingNumber, string? floor)
    {
        UserId = userId;
        Country = country;
        City = city;
        Street = street;
        ZipCode = zipCode;
        ApartmentNumber = apartmentNumber;
        BuildingNumber = buildingNumber;
        Floor = floor;
    }

    public override string ToString()
    {
        return
            $"{nameof(UserId)}: {UserId}, {nameof(Country)}: {Country}, {nameof(City)}: {City}, {nameof(Street)}: {Street}, {nameof(ZipCode)}: {ZipCode}";
    }
}