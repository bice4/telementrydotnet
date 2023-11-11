using System.ComponentModel.DataAnnotations;

namespace TelemtryNetProject.Contracts.UserManagement.Api.V1.Requests;

public class CreateUserRequest
{
    [Required(ErrorMessage = $"{nameof(FirstName)} is required")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = $"{nameof(LastName)} is required")]
    public string LastName { get; set; }

    [Required(ErrorMessage = $"{nameof(Email)} is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; }

    [Required(ErrorMessage = $"{nameof(Password)} is required")]
    public string Password { get; set; }

    public string? PhoneNumber { get; set; }

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

    [Required(ErrorMessage = $"{nameof(Age)} is required")]
    [Range(13, 99, ErrorMessage = "Age must be between 13 and 99")]
    public int Age { get; set; }

    [Range(0, 2, ErrorMessage = "Gender must be between 0 and 2")]
    public int Gender { get; set; }

    public CreateUserRequest(string firstName, string lastName, string email, string password, string? phoneNumber,
        string country, string city, string street, string zipCode, string? apartmentNumber, string? buildingNumber,
        string? floor, int age, int gender)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Password = password;
        PhoneNumber = phoneNumber;
        Country = country;
        City = city;
        Street = street;
        ZipCode = zipCode;
        ApartmentNumber = apartmentNumber;
        BuildingNumber = buildingNumber;
        Floor = floor;
        Age = age;
        Gender = gender;
    }

    public override string ToString()
    {
        return $"FirstName: {FirstName}, " +
               $"LastName: {LastName}, " +
               $"Email: {Email}, " +
               $"Password: ******, " +
               $"PhoneNumber: {PhoneNumber}, " +
               $"Country: {Country}, " +
               $"City: {City}, " +
               $"Street: {Street}, " +
               $"ZipCode: {ZipCode}, " +
               $"ApartmentNumber: {ApartmentNumber}, " +
               $"BuildingNumber: {BuildingNumber}, " +
               $"Floor: {Floor}, " +
               $"Age: {Age}, " +
               $"Gender: {Gender}";
    }
}