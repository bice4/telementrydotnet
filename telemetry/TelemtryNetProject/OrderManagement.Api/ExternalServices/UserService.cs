using TelemtryNetProject.Contracts.UserManagement.Api.V1.Requests;
using TelemtryNetProject.Contracts.UserManagement.Api.V1.Responses;

namespace OrderManagementApi.ExternalServices;

public class UserService
{
    private readonly HttpClient _httpClient;

    public UserService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> CreateUser(CreateUserRequest createUserRequest, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync("create", createUserRequest, cancellationToken);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        return content;
    }

    public async Task DeleteUser(string userId, CancellationToken cancellationToken)
    {
        var response = await _httpClient.DeleteAsync(userId, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}