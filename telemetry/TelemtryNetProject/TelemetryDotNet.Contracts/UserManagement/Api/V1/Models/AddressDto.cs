namespace TelemetryDotNet.Contracts.UserManagement.Api.V1.Models;

public class AddressDto
{
    public string Country { get; set; }

    public string City { get; set; }

    public string Street { get; set; }

    public string ZipCode { get; set; }

    public string? ApartmentNumber { get; set; }

    public string? BuildingNumber { get; set; }

    public string? Floor { get; set; }

    public AddressDto(string country, string city, string street, string zipCode, string? apartmentNumber,
        string? buildingNumber, string? floor)
    {
        Country = country;
        City = city;
        Street = street;
        ZipCode = zipCode;
        ApartmentNumber = apartmentNumber;
        BuildingNumber = buildingNumber;
        Floor = floor;
    }
}