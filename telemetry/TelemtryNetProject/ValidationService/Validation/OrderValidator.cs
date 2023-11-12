using TelemtryNetProject.Contracts.Order.Api.v1.Request;
using TelemtryNetProject.Contracts.ValidationService.Api.v1.Models;

namespace ValidationService.Validation;

public class OrderValidator : IValidator<CreateOrderRequest>
{
    private static readonly string[] UnsupportedOrderNames = { "Russia", "North Korea", "China", "Putin" };

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

        if (request.OrderItems.Sum(x => x.Quantity) > 400)
        {
            results.Add(ValidationResult.CreateValidationResult("Order quantity sum is too big"));
        }
        
        return Task.FromResult(results);
    }
}