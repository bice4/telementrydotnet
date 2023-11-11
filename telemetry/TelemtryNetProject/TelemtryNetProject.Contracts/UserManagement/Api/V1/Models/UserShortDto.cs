namespace TelemtryNetProject.Contracts.UserManagement.Api.V1.Models;

public class UserShortDto
{
    public string UserId { get; set; }
    
    public string FullName { get; set; }

    public string Email { get; set; }

    public int Age { get; set; }

    public string Gender { get; set; }

    public UserShortDto(string fullName, string email, int age, string gender, string userId)
    {
        FullName = fullName;
        Email = email;
        Age = age;
        Gender = gender;
        UserId = userId;
    }
}