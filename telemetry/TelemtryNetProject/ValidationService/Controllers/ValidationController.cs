using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using TelemtryNetProject.Contracts.Order.Api.v1.Request;
using TelemtryNetProject.Contracts.UserManagement.Api.V1.Requests;
using TelemtryNetProject.Contracts.ValidationService.Api.v1.Models;
using TelemtryNetProject.Contracts.ValidationService.Api.v1.Responses;
using ValidationService.ExternalServices;
using ValidationService.Validation;

namespace ValidationService.Controllers;

[ApiController]
[Route("[controller]")]
public class ValidationController : ControllerBase
{
    private readonly ILogger<ValidationController> _logger;
    private readonly IValidator<CreateUserRequest> _createUserRequestValidator;
    private readonly IValidator<CreateOrderRequest> _createOrderRequestValidator;

    public ValidationController(ILogger<ValidationController> logger,
        IValidator<CreateUserRequest> createUserRequestValidator,
        IValidator<CreateOrderRequest> createOrderRequestValidator)
    {
        _logger = logger;
        _createUserRequestValidator = createUserRequestValidator;
        _createOrderRequestValidator = createOrderRequestValidator;
    }

    [HttpPost("user", Name = "ValidateUser")]
    public async Task<IActionResult> ValidateUser(CreateUserRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("ValidateUser called");
        var results = new List<ValidationResult>();

        try
        {
            results = await _createUserRequestValidator.ValidateAsync(request, cancellationToken);

            _logger.LogInformation("ValidateUser finished, returning {ValidationResultResponse}", JsonSerializer.Serialize(results));
            
            return new OkObjectResult(new ValidationResultResponse() {
                Results = results
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception occured");
            results.Add(ValidationResult.CreateExceptionResult("Exception occured", e.Message));
            return new OkObjectResult(new ValidationResultResponse {
                Results = results
            });
        }
    }

    [HttpPost("order", Name = "ValidateOrder")]
    public async Task<IActionResult> ValidateOrder(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("ValidateOrder called");
        var results = new List<ValidationResult>();

        try
        {
            results = await _createOrderRequestValidator.ValidateAsync(request, cancellationToken);
            _logger.LogInformation("ValidateOrder finished, returning {ValidationResultResponse}", JsonSerializer.Serialize(results));

            return new OkObjectResult(new ValidationResultResponse() {
                Results = results
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception occured");
            results.Add(ValidationResult.CreateExceptionResult("Exception occured", e.Message));
            return new OkObjectResult(new ValidationResultResponse {
                Results = results
            });
        }
    }
}