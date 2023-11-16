using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using InvoiceGeneratorService;
using TelemetryDotNet.Contracts.UserManagement.Api.V1.Models;

namespace OrderManagementApi.ExternalServices;

public class InvoiceGeneratorService
{
    private readonly ILogger<InvoiceGeneratorService> _logger;
    private readonly IConfiguration _configuration;

    public InvoiceGeneratorService(ILogger<InvoiceGeneratorService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<string> GenerateInvoiceAsync(OrderManagement.Domain.OrderModels.Order order,
        string correlationId,
        UserShortDto userShortDto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating invoice for order {@Order}", order);

        try
        {
            var url = _configuration["InvoiceServiceUrl"] ?? "http://localhost:8099";

            using var channel = GrpcChannel.ForAddress(url);
            var client = new OrderPlacer.OrderPlacerClient(channel);

            var createInvoiceRequest = new InvoiceRequest() {
                ReferenceId = correlationId,
                Order = new Order() {
                    User = new User() {
                        FullName = userShortDto.FullName,
                        Email = userShortDto.Email,
                        UserId = userShortDto.UserId,
                        Phone = userShortDto.PhoneNumber ?? "none"
                    },
                    CreatedAt = order.CreatedAt.ToTimestamp(),
                    OrderId = order.Id.ToString()!,
                    Items = {
                        order.Items.Select(x => new OrderItem() {
                            OrderItemId = x.Id.ToString()!,
                            Name = x.Name,
                            Price = x.Price,
                            Quantity = x.Quantity
                        })
                    }
                }
            };

            var createInvoiceResponse = await client.GenerateInvoiceAsync(createInvoiceRequest, cancellationToken: cancellationToken);
            return createInvoiceResponse.InvoiceId;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error generating invoice for orderId {OrderId}, {Error}", order.Id, e.Message);
            return null!;
        }
    }
}