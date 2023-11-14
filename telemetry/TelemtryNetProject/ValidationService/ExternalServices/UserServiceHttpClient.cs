using TelemetryDotNet.Contracts.UserManagement.Api.V1.Responses;

namespace ValidationService.ExternalServices;

/// <summary>
/// External service to communicate with the UserManagement API
/// </summary>
public class UserServiceHttpClient
{
    private readonly HttpClient _httpClient;

    public UserServiceHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// IS user email exists
    /// </summary>
    /// <param name="email"><see cref="string"/>User email</param>
    /// <param name="cancellationToken"></param>
    /// <returns>If email exists</returns>
    public async Task<bool?> IsEmailExistsAsync(string email, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync($"users/{email}", cancellationToken);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadFromJsonAsync<EmailExistsResponse>(cancellationToken: cancellationToken);

        return content?.Exists;
    }
}