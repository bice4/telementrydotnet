using System.Net;
using TelemetryDotNet.Contracts.UserManagement.Api.V1.Models;
using TelemetryDotNet.Contracts.UserManagement.Api.V1.Requests;
using TelemetryDotNet.Contracts.UserManagement.Api.V1.Responses;
using TelemetryDotNet.Contracts.ValidationService.Api.v1.Responses;

namespace ApiGateway.ExternalServices;

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
    /// Get user by id
    /// </summary>
    /// <param name="userId">Id of specific user</param>
    /// <param name="cancellationToken"></param>
    /// <returns><see cref="UserFullDto"/></returns>
    public async Task<UserFullDto?> GetUserByIdAsync(string userId, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(userId, cancellationToken);

        // If user is not found, return null
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<UserFullDto>(cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    /// <param name="request"><see cref="CreateUserRequest"/>User data</param>
    /// <param name="cancellationToken"></param>
    /// <returns><see cref="CreateUserResponse"/></returns>
    public async Task<CreateUserResponse?> CreateUserAsync(CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync("create", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<CreateUserResponse>(cancellationToken: cancellationToken);
    }

  
}