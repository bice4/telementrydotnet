namespace UserManagement.Domain.UserModels;

public class Address
{
    public string Country { get; private set; }

    public string City { get; private set; }

    public string Street { get; private set; }

    public string ZipCode { get; private set; }

    public string? ApartmentNumber { get; private set; }

    public string? BuildingNumber { get; private set; }

    public string? Floor { get; private set; }


    public Address(string country, string city, string street, string zipCode, string? apartmentNumber,
        string? buildingNumber, string? floor)
    {
        Country = country ?? throw new ArgumentNullException(nameof(country));
        City = city ?? throw new ArgumentNullException(nameof(city));
        Street = street ?? throw new ArgumentNullException(nameof(street));
        ZipCode = zipCode ?? throw new ArgumentNullException(nameof(zipCode));

        ApartmentNumber = apartmentNumber;
        BuildingNumber = buildingNumber;
        Floor = floor;
    }

    public override string ToString()
    {
        return
            $"{nameof(Country)}: {Country}, {nameof(City)}: {City}, {nameof(Street)}: {Street}, {nameof(ZipCode)}: {ZipCode}, {nameof(ApartmentNumber)}: {ApartmentNumber}, {nameof(BuildingNumber)}: {BuildingNumber}, {nameof(Floor)}: {Floor}";
    }
}