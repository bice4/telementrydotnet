using TelemetryDotNet.Contracts.ValidationService.Api.v1.Models;

namespace ValidationService.Extensions;

public static class ValidationResultExtensions
{
    public static bool HasErrors(this IEnumerable<ValidationResult> results)
        => results.Any();
}