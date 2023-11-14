using TelemetryDotNet.Contracts.UserManagement.Api.V1.Requests;
using TelemetryDotNet.Contracts.UserManagement.Api.V1.Responses;

namespace OrderManagementApi.ExternalServices;

public class UserService
{
    private readonly HttpClient _httpClient;

    public UserService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CreateUserResponse?> CreateUser(CreateUserRequest createUserRequest, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync("create", createUserRequest, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<CreateUserResponse>(cancellationToken: cancellationToken);
    }
    
    public async Task DeleteUser(string userId, CancellationToken cancellationToken)
    {
        var response = await _httpClient.DeleteAsync(userId, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}