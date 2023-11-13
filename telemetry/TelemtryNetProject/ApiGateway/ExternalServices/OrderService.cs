using System.Net;
using TelemetryDotNet.Contracts.Order.Api.v1.Models;

namespace ApiGateway.ExternalServices;

public class OrderService
{
    private readonly HttpClient _httpClient;

    public OrderService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<OrderDto?> GetOrderByIdAsync(string orderId, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(orderId, cancellationToken);
        
        if(response.StatusCode == HttpStatusCode.NotFound)
            return null;
        
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<OrderDto>(cancellationToken: cancellationToken);
    }
}