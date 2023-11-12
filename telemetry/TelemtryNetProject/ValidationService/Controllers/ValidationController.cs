using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TelemtryNetProject.Contracts.Order.Api.v1.Request;
using TelemtryNetProject.Contracts.UserManagement.Api.V1.Requests;
using TelemtryNetProject.Contracts.ValidationService.Api.v1.Models;
using TelemtryNetProject.Contracts.ValidationService.Api.v1.Responses;
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

    /// <summary>
    /// Validate create user request
    /// </summary>
    /// <param name="request">Create user request</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Validation result</returns>
    [HttpPost("user", Name = "ValidateUser")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ValidationResultResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ValidateUser(CreateUserRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("ValidateUser");
        var results = new List<ValidationResult>();

        try
        {
            results = await _createUserRequestValidator.ValidateAsync(request, cancellationToken);

            _logger.LogInformation("ValidateUser finished, returning {ValidationResultResponse}",
                JsonSerializer.Serialize(results));

            return new OkObjectResult(new ValidationResultResponse() {
                Results = results
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception occured, Exception: {Exception}", e.Message);
            results.Add(ValidationResult.CreateExceptionResult("Exception occured", e.Message));
            return new OkObjectResult(new ValidationResultResponse {
                Results = results
            });
        }
    }

    /// <summary>
    /// Validate create order request
    /// </summary>
    /// <param name="request">Create order request</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Validation result</returns>
    [HttpPost("order", Name = "ValidateOrder")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ValidationResultResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ValidateOrder(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("ValidateOrder called");
        var results = new List<ValidationResult>();

        try
        {
            results = await _createOrderRequestValidator.ValidateAsync(request, cancellationToken);
            _logger.LogInformation("ValidateOrder finished, returning {ValidationResultResponse}",
                JsonSerializer.Serialize(results));

            return new OkObjectResult(new ValidationResultResponse() {
                Results = results
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception occured, Exception: {Exception}", e.Message);
            results.Add(ValidationResult.CreateExceptionResult("Exception occured", e.Message));
            return new OkObjectResult(new ValidationResultResponse {
                Results = results
            });
        }
    }
}