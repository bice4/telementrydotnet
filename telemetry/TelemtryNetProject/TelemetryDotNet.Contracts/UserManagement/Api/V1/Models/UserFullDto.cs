namespace TelemetryDotNet.Contracts.UserManagement.Api.V1.Models;

public class UserFullDto
{
    public string UserId { get; set; }
    
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string? PhoneNumber { get; set; }

    public int Age { get; set; }

    public string Gender { get; set; }

    public AddressDto Address { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public UserFullDto(string firstName, string lastName, string email, string? phoneNumber, int age, string gender,
        AddressDto address, DateTimeOffset createdAt, DateTimeOffset updatedAt, string userId)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        Age = age;
        Gender = gender;
        Address = address;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        UserId = userId;
    }
}