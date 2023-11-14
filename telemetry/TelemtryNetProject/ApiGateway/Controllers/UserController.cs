using ApiGateway.ExternalServices;
using Microsoft.AspNetCore.Mvc;
using TelemetryDotNet.Contracts.UserManagement.Api.V1.Requests;
using TelemetryDotNet.Contracts.UserManagement.Api.V1.Responses;
using TelemetryDotNet.Contracts.ValidationService.Api.v1.Responses;

namespace ApiGateway.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly ValidationServiceHttpClient _validationServiceHttpClient;
    private readonly UserServiceHttpClient _userServiceHttpClient;

    public UserController(ILogger<UserController> logger, ValidationServiceHttpClient validationServiceHttpClient, UserServiceHttpClient userServiceHttpClient)
    {
        _logger = logger;
        _validationServiceHttpClient = validationServiceHttpClient;
        _userServiceHttpClient = userServiceHttpClient;
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    /// <param name="request"><see cref="CreateUserRequest"/>User data</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(CreateUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationResultResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create(CreateUserRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Create user, request: {User}", request);

        try
        {
            var validationResultResponse = await _validationServiceHttpClient.IsUserValid(request, cancellationToken);

            if (validationResultResponse != null)
            {
                _logger.LogWarning("User validation failed, response: {@ValidationResultResponse}",
                    validationResultResponse);
                return BadRequest(validationResultResponse);
            }

            var createUserResponse = await _userServiceHttpClient.CreateUserAsync(request, cancellationToken);

            _logger.LogInformation("User created, userId: {UserId}", createUserResponse?.UserId);
            
            return new OkObjectResult(createUserResponse);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while creating user: {Message}", e.Message);
            return StatusCode(500, e);
        }
    }
}