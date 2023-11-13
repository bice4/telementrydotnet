using System.Net;
using TelemetryDotNet.Contracts.Order.Api.v1.Request;
using TelemetryDotNet.Contracts.UserManagement.Api.V1.Requests;
using TelemetryDotNet.Contracts.ValidationService.Api.v1.Responses;

namespace ApiGateway.ExternalServices;

/// <summary>
/// External service for validation
/// </summary>
public class ValidationService
{
    private readonly HttpClient _httpClient;

    public ValidationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Call validation service to validate user data
    /// </summary>
    /// <param name="createUserRequest"><see cref="CreateUserRequest"/> model</param>
    /// <param name="cancellationToken"></param>
    /// <returns><see cref="ValidationResultResponse"/></returns>
    public async Task<ValidationResultResponse?> IsUserValid(CreateUserRequest createUserRequest,
        CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync("user", createUserRequest, cancellationToken);

        if (response.StatusCode == HttpStatusCode.BadRequest)
            return await response.Content.ReadFromJsonAsync<ValidationResultResponse>(
                cancellationToken: cancellationToken);

        response.EnsureSuccessStatusCode();

        return null!;
    }

    /// <summary>
    /// Call validation service to validate order data
    /// </summary>
    /// <param name="createOrderRequest"><see cref="CreateOrderRequest"/> model</param>
    /// <param name="cancellationToken"></param>
    /// <returns><see cref="ValidationResultResponse"/></returns>
    public async Task<ValidationResultResponse?> IsOrderValid(CreateOrderRequest createOrderRequest,
        CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync("order", createOrderRequest, cancellationToken);
        if (response.StatusCode == HttpStatusCode.BadRequest)
            return await response.Content.ReadFromJsonAsync<ValidationResultResponse>(
                cancellationToken: cancellationToken);

        response.EnsureSuccessStatusCode();

        return null!;
    }
}