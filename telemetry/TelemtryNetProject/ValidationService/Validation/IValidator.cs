using TelemtryNetProject.Contracts.ValidationService.Api.v1.Models;

namespace ValidationService.Validation;

public interface IValidator<in T>
{
    Task<List<ValidationResult>> ValidateAsync(T request, CancellationToken cancellationToken);
}