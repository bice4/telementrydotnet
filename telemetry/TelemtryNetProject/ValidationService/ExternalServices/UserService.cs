using TelemetryDotNet.Contracts.UserManagement.Api.V1.Responses;

namespace ValidationService.ExternalServices;

public class UserService
{
    private readonly HttpClient _httpClient;

    public UserService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool?> IsEmailExistsAsync(string email, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync($"users/{email}", cancellationToken);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadFromJsonAsync<EmailExistsResponse>(cancellationToken: cancellationToken);

        return content?.Exists;
    }
}