using TelemtryNetProject.Contracts.ValidationService.Api.v1.Models;

namespace TelemtryNetProject.Contracts.ValidationService.Api.v1.Responses;

public class ValidationResultResponse
{
    public List<ValidationResult> Results { get; set; }
}
