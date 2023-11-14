using System.Net;
using TelemetryDotNet.Contracts.UserManagement.Api.V1.Requests;
using TelemetryDotNet.Contracts.UserManagement.Api.V1.Responses;
using TelemetryDotNet.Contracts.ValidationService.Api.v1.Responses;

namespace Simulator.ExternalService;

/// <summary>
/// External service to communicate with the ApiGateway API
/// </summary>
public class ApiGatewayHttpClient
{
    private readonly HttpClient _httpClient;

    public ApiGatewayHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    /// <param name="createUserRequest"><see cref="CreateUserRequest"/>Data</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<(CreateUserResponse? success, ValidationResultResponse? error)> CreateUser(
        CreateUserRequest createUserRequest,
        CancellationToken cancellationToken)
    {
        var response = await PostAsync<CreateUserRequest, (CreateUserResponse success, ValidationResultResponse error)>
        (String.Empty, createUserRequest, async message =>
        {
            if (message.StatusCode == HttpStatusCode.BadRequest)
            {
                var error =
                    await message.Content.ReadFromJsonAsync<ValidationResultResponse>(
                        cancellationToken: cancellationToken);
                return (null!, error)!;
            }

            message.EnsureSuccessStatusCode();

            var success =
                await message.Content.ReadFromJsonAsync<CreateUserResponse>(cancellationToken: cancellationToken);
            return (success!, null!);
        }, cancellationToken);

        return response;
    }

    private async Task<TResponse> PostAsync<TRequest, TResponse>(string path, TRequest data,
        Func<HttpResponseMessage, Task<TResponse>> responseHandler, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync(path, data, cancellationToken);

        return await responseHandler(response);
    }
}