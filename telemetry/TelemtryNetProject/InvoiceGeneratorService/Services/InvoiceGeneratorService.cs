using Grpc.Core;

namespace InvoiceGeneratorService.Services;

public class InvoiceGeneratorService : OrderPlacer.OrderPlacerBase
{
    private readonly ILogger<InvoiceGeneratorService> _logger;

    public InvoiceGeneratorService(ILogger<InvoiceGeneratorService> logger)
    {
        _logger = logger;
    }

    public override async Task<InvoiceResponse> GenerateInvoice(InvoiceRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Generating invoice for order {@Request}", request);

        await Task.Delay(100);

        _logger.LogInformation("Invoice generated for order {@Request}", request);

        return new InvoiceResponse() {
            InvoiceId = Guid.NewGuid().ToString()!
        };
    }
}