using System.Net;
using TelemetryDotNet.Contracts.UserManagement.Api.V1.Models;

namespace ApiGateway.ExternalServices;

public class UserService
{
    private readonly HttpClient _httpClient;

    public UserService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<UserFullDto?> GetUserByIdAsync(string userId, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(userId, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<UserFullDto>(cancellationToken: cancellationToken);
    }
}