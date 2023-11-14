using TelemetryDotNet.Contracts.ValidationService.Api.v1.Models;

namespace TelemetryDotNet.Contracts.ValidationService.Api.v1.Responses;

public class ValidationResultResponse
{
    public List<ValidationResult> Results { get; set; }
    
    public ValidationResultResponse(List<ValidationResult> results)
    {
        Results = results;
    }
}
