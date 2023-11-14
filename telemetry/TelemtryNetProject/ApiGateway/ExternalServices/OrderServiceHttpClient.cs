using System.Net;
using TelemetryDotNet.Contracts.Order.Api.v1.Models;

namespace ApiGateway.ExternalServices;

/// <summary>
/// External service to communicate with the Order API
/// </summary>
public class OrderServiceHttpClient
{
    private readonly HttpClient _httpClient;

    public OrderServiceHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Get order by id
    /// </summary>
    /// <param name="orderId"><see cref="string"/>Id of specific order</param>
    /// <param name="cancellationToken"></param>
    /// <returns><see cref="OrderDto"/></returns>
    public async Task<OrderDto?> GetOrderByIdAsync(string orderId, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(orderId, cancellationToken);

        // If order is not found, return null
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<OrderDto>(cancellationToken: cancellationToken);
    }
}