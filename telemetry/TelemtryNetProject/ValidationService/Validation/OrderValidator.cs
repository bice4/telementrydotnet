using TelemetryDotNet.Contracts.Order.Api.v1.Request;
using TelemetryDotNet.Contracts.ValidationService.Api.v1.Models;

namespace ValidationService.Validation;

public class OrderValidator : IValidator<CreateOrderRequest>
{
    private static readonly string[] UnsupportedOrderNames = { "Russia", "North Korea", "China", "Putin" };

    private const int MAX_ORDER_QUANTITY_SUM = 400;
    private const double MAX_ORDER_PRICE = 10000;

    /// <summary>
    /// Validate create order request
    /// </summary>
    /// <param name="request"><see cref="CreateOrderRequest"/> model</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Validation results collection of <see cref="ValidationResult"/></returns>
    public Task<List<ValidationResult>> ValidateAsync(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var results = new List<ValidationResult>();

        if (!request.OrderItems.Any())
            results.Add(ValidationResult.CreateValidationResult("Order items are required"));

        if (UnsupportedOrderNames.Any(name =>
                request.OrderItems.Any(item => item.Name.Contains(name, StringComparison.CurrentCultureIgnoreCase))))
        {
            results.Add(ValidationResult.CreateValidationResult("Order name is unsupported"));
        }

        if (request.OrderItems.Sum(x => x.Quantity) > MAX_ORDER_QUANTITY_SUM)
        {
            results.Add(ValidationResult.CreateValidationResult("Order quantity sum is too big"));
        }

        if (request.OrderItems.Sum(x => x.Price) > MAX_ORDER_PRICE)
        {
            results.Add(ValidationResult.CreateValidationResult("Order price sum is too big"));
        }

        return Task.FromResult(results);
    }
}