using System.ComponentModel.DataAnnotations;

namespace TelemtryNetProject.Contracts.UserManagement.Api.V1.Requests;

public class UpdateUserPasswordRequest
{
    [Required(ErrorMessage = $"{nameof(UserId)} is required")]
    public string UserId { get; set; }

    [Required(ErrorMessage = $"{nameof(Password)} is required")]
    public string Password { get; set; }

    public UpdateUserPasswordRequest(string userId, string password)
    {
        UserId = userId;
        Password = password;
    }

    public override string ToString()
    {
        return $"{nameof(UserId)}: {UserId}";
    }
}