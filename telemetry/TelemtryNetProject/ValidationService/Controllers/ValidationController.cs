using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TelemetryDotNet.Contracts.Order.Api.v1.Request;
using TelemetryDotNet.Contracts.UserManagement.Api.V1.Requests;
using TelemetryDotNet.Contracts.ValidationService.Api.v1.Models;
using TelemetryDotNet.Contracts.ValidationService.Api.v1.Responses;
using ValidationService.Extensions;
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

            if (results.HasErrors())
            {
                return new JsonResult(new ValidationResultResponse(results)) 
                {
                    StatusCode = 400
                };
            }

            return new OkObjectResult(new ValidationResultResponse(results) {
                Results = results
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception occured, Exception: {Exception}", e.Message);
            results.Add(ValidationResult.CreateExceptionResult("Exception occured", e.Message));
            return new OkObjectResult(new ValidationResultResponse(results));
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
           
            if (results.HasErrors())
            {
                return new JsonResult(new ValidationResultResponse(results)) 
                {
                    StatusCode = 400
                };
            }

            return new OkObjectResult(new ValidationResultResponse(results) {
                Results = results
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception occured, Exception: {Exception}", e.Message);
            results.Add(ValidationResult.CreateExceptionResult("Exception occured", e.Message));
            return new OkObjectResult(new ValidationResultResponse(results));
        }
    }
}