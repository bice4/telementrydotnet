namespace TelemetryDotNet.Contracts.ValidationService.Api.v1.Models;

public class ValidationResult
{
    public string? ErrorMessage { get; set; }
    public ValidationType ValidationType { get; set; }
    public string? Exception { get; set; }

    public static ValidationResult CreateValidationResult(string errorMessage)
    {
        return new ValidationResult {
            ErrorMessage = errorMessage,
            ValidationType = ValidationType.Validation
        };
    }

    public static ValidationResult CreateExceptionResult(string exception, string errorMessage)
    {
        return new ValidationResult {
            Exception = exception,
            ErrorMessage = errorMessage,
            ValidationType = ValidationType.Exception
        };
    }

    public override string ToString() 
        => $"{nameof(ErrorMessage)}: {ErrorMessage}, {nameof(ValidationType)}: {ValidationType:G}, {nameof(Exception)}: {Exception}";
}