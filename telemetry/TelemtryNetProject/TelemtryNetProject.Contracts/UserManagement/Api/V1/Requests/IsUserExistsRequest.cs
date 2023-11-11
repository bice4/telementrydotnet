using System.ComponentModel.DataAnnotations;

namespace TelemtryNetProject.Contracts.UserManagement.Api.V1.Requests;

public class IsUserExistsRequest
{
    [Required(ErrorMessage = $"{nameof(UserId)} is required")]
    public string UserId { get; set; }
    
    public IsUserExistsRequest(string userId)
    {
        UserId = userId;
    }
}