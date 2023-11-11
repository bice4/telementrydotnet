using TelemtryNetProject.Contracts.Order.Api.v1.Request;
using TelemtryNetProject.Contracts.UserManagement.Api.V1.Requests;
using TelemtryNetProject.Contracts.ValidationService.Api.v1.Responses;

namespace ApiGateway.ExternalServices;

public class ValidationService
{
    private readonly HttpClient _httpClient;

    public ValidationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ValidationResultResponse?> IsUserValid(CreateUserRequest createUserRequest,
        CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync("user", createUserRequest, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ValidationResultResponse>(cancellationToken: cancellationToken);
    }
    
    public async Task<ValidationResultResponse?> IsOrderValid(CreateOrderRequest createUserRequest,
        CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync("order", createUserRequest, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ValidationResultResponse>(cancellationToken: cancellationToken);
    }
}