using TelemetryDotNet.Contracts.UserManagement.Api.V1.Requests;
using TelemetryDotNet.Contracts.ValidationService.Api.v1.Models;
using ValidationService.ExternalServices;

namespace ValidationService.Validation;

public class UserValidator : IValidator<CreateUserRequest>
{
    private readonly UserServiceHttpClient _userServiceHttpClient;

    private static readonly string[] UnsupportedCountries = { "Russia", "Russian Federation", "North Korea", "China" };

    private static readonly string[] UnsupportedEmailDomains =
        { "rambler.ru", "rambler.com", "mail.ru", "yandex.ru", "yandex.com" };

    public UserValidator(UserServiceHttpClient userServiceHttpClient)
    {
        _userServiceHttpClient = userServiceHttpClient;
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
            var isEmailExists = await _userServiceHttpClient.IsEmailExistsAsync(request.Email, cancellationToken);
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
            results.Add(
                ValidationResult.CreateExceptionResult("An error occurred while calling UserService", e.Message));
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

        if (UnsupportedEmailDomains.Any(domain => request.Email.EndsWith(domain)))
        {
            results.Add(ValidationResult.CreateValidationResult("Email domain is unsupported"));
        }

        return results;
    }
}