using TelemtryNetProject.Contracts.UserManagement.Api.V1.Requests;
using TelemtryNetProject.Contracts.ValidationService.Api.v1.Models;
using ValidationService.ExternalServices;

namespace ValidationService.Validation;

public class UserValidator : IValidator<CreateUserRequest>
{
    private readonly UserService _userService;

    private static readonly string[] UnsupportedCountries = { "Russia", "North Korea", "China" };
    private static readonly string[] UnsupportedEmailDomains = { "rambler", "mail", "yandex" };

    public UserValidator(UserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Validate create user request
    /// </summary>
    /// <param name="request"><see cref="CreateUserRequest"/> model</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Validation results collection of <see cref="ValidationResult"/></returns>
    public async Task<List<ValidationResult>> ValidateAsync(CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var results = new List<ValidationResult>();

        try
        {
            // Call external service (User service) to check if email exists
            var isEmailExists = await _userService.IsEmailExistsAsync(request.Email, cancellationToken);
            if (isEmailExists == null)
            {
                results.Add(ValidationResult.CreateValidationResult("Email is not valid"));
            }
            else if (isEmailExists.Value)
            {
                results.Add(ValidationResult.CreateValidationResult("Email already exists"));
            }
        }
        catch (Exception e)
        {
            // If external service is not available, return exception result
            // This is just an example, in real world you should handle this case
            results.Add(ValidationResult.CreateExceptionResult("An error occurred while calling UserService", e.Message));
        }
        
        if (UnsupportedCountries.Contains(request.Country))
        {
            results.Add(ValidationResult.CreateValidationResult("Country is unsupported"));
        }

        if (request.FirstName.Equals("Vladimir", StringComparison.InvariantCultureIgnoreCase) &&
            request.LastName.Equals("Putin", StringComparison.InvariantCultureIgnoreCase))
        {
            results.Add(
                ValidationResult.CreateValidationResult("You are not allowed to register. Please kill yourself"));
        }
        
        if (UnsupportedEmailDomains.Any(domain => request.Email.Contains(domain)))
        {
            results.Add(ValidationResult.CreateValidationResult("Email domain is unsupported"));
        }
        
        return results;
    }
}