using System.ComponentModel.DataAnnotations;

namespace TelemtryNetProject.Contracts.UserManagement.Api.V1.Requests;

public class IsUserEmailUniqRequest
{
    [Required(ErrorMessage = $"{nameof(Email)} is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; }

    public IsUserEmailUniqRequest(string email)
    {
        Email = email;
    }
}