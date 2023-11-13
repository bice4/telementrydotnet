namespace TelemetryDotNet.Contracts.UserManagement.Api.V1.Responses;

public class CreateUserResponse
{
    public string UserId { get; set; }

    public CreateUserResponse(string userId)
    {
        UserId = userId;
    }
}