using MongoDB.Bson;

namespace UserManagement.Domain.UserModels;

public class User
{
    public ObjectId Id { get; private set; }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public string Email { get; private set; }

    public string Password { get; private set; }

    public int Age { get; private set; }

    public Address Address { get; private set; }

    public string? PhoneNumber { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public Gender Gender { get; private set; }

    public string FullName => $"{FirstName} {LastName}";

    public User(string firstName, string lastName, string email, string password, int age, Gender gender,
        Address address, string? phoneNumber)
    {
        Id = ObjectId.GenerateNewId();

        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Password = password ?? throw new ArgumentNullException(nameof(password));
        Gender = gender;
        Address = address ?? throw new ArgumentNullException(nameof(address));
        PhoneNumber = phoneNumber;
        Age = age > 0 ? age : throw new ArgumentException("Age must be greater than 0");
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string firstName, string lastName, string email, string password, int age, Gender gender,
        string phoneNumber, Address address)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Password = password;
        Age = age;
        Gender = gender;
        PhoneNumber = phoneNumber;
        Address = address;

        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePassword(string password)
    {
        Password = password;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateAge(int age)
    {
        Age = age;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateEmail(string email)
    {
        Email = email;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateFirstName(string firstName)
    {
        FirstName = firstName;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateLastName(string lastName)
    {
        LastName = lastName;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePhoneNumber(string? phoneNumber)
    {
        PhoneNumber = phoneNumber;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateAddress(Address address)
    {
        Address = address ?? throw new ArgumentNullException(nameof(address));
        UpdatedAt = DateTime.UtcNow;
    }

    public override string ToString()
    {
        return
            $"Id: {Id}, FullName: {FullName}, Email: {Email}, Age: {Age}, Gender: {Gender:G} CreatedAt: {CreatedAt}, UpdatedAt: {UpdatedAt}";
    }
}