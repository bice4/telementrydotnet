using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using TelemetryDotNet.Contracts.UserManagement.Api.V1.Models;
using TelemetryDotNet.Contracts.UserManagement.Api.V1.Requests;
using TelemetryDotNet.Contracts.UserManagement.Api.V1.Responses;
using TelemetryDotNet.Contracts.ValidationService.Api.v1.Responses;
using UserManagement.Domain.Repositories;
using UserManagement.Infrastructure.Services;
using UserManagement.Metrics;
using UserManagement.Translators;

namespace UserManagement.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly UserMetrics _userMetrics;

    public UserController(ILogger<UserController> logger, IUserRepository userRepository,
        IPasswordHasherService passwordHasherService, UserMetrics userMetrics)
    {
        _logger = logger;
        _userRepository = userRepository;
        _passwordHasherService = passwordHasherService;
        _userMetrics = userMetrics;
    }

    /// <summary>
    /// Get all users
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>List of all users</returns>
    [HttpGet(Name = "GetAll")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<UserShortDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        _logger.LogDebug("GetAll called");

        try
        {
            var users = await _userRepository.GetUsersAsync(cancellationToken);

            return Ok(users.Select(x => x.ToUserShortDto()));
        }
        catch (Exception e)
        {
            return LogExceptionAndReturnError(e);
        }
    }

    /// <summary>
    /// Get user by id
    /// </summary>
    /// <param name="id">Id of specific user</param>
    /// <param name="cancellationToken"></param>
    /// <returns>User for provided id</returns>
    [HttpGet("{id}", Name = "GetById")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(UserFullDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetById called with id: {Id}", id);

        try
        {
            var user = await _userRepository.GetUserAsync(ObjectId.Parse(id), cancellationToken);

            return user == null
                ? NotFound()
                : new ObjectResult(user.ToUserFullDto());
        }
        catch (Exception e)
        {
            return LogExceptionAndReturnError(e);
        }
    }

    /// <summary>
    /// Create new user
    /// </summary>
    /// <param name="request">Data for new user</param>
    /// <param name="cancellationToken"></param>
    /// <returns>User id of created user</returns>
    [HttpPost("create")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create(CreateUserRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Create user with request: {Request}", request);

        try
        {
            var passwordHash = _passwordHasherService.HashPassword(request.Password);
            var user = request.ToUser(passwordHash);

            await _userRepository.AddUserAsync(user, cancellationToken);

            // If user was created successfully, update metrics
            _userMetrics.IncUserCounters(user.Address.Country, user.Age);

            return new OkObjectResult(new CreateUserResponse(user.Id.ToString()!));
        }
        catch (Exception e)
        {
            return LogExceptionAndReturnError(e);
        }
    }
    
    /// <summary>
    /// Delete user by id
    /// </summary>
    /// <param name="id">Id of specific user</param>
    /// <param name="cancellationToken"></param>
    /// <returns>OK</returns>
    [HttpDelete("{id}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteUser(string id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("DeleteUser called with id: {Id}", id);

        try
        {
            var result = await _userRepository.DeleteUserAsync(ObjectId.Parse(id), cancellationToken);

            if (!result)
            {
                return NotFound();
            }

            // If user was deleted successfully, update metrics
            _userMetrics.DecUserTotalCounter();

            return Ok();
        }
        catch (Exception e)
        {
            return LogExceptionAndReturnError(e);
        }
    }

    /// <summary>
    /// Check if user email exists
    /// </summary>
    /// <param name="email">Specific email</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Response with existing</returns>
    [HttpGet("users/{email}", Name = "Exists")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(EmailExistsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> IsUserEmailExists(string email, CancellationToken cancellationToken)
    {
        _logger.LogInformation("IsUserEmailExists called with id: {Email}", email);

        try
        {
            var isEmailUniqueAsync = await _userRepository.UserWithEmailExistsAsync(email, cancellationToken);

            return new ObjectResult(new EmailExistsResponse() {
                Exists = isEmailUniqueAsync
            });
        }
        catch (Exception e)
        {
            return LogExceptionAndReturnError(e);
        }
    }

    private IActionResult LogExceptionAndReturnError(Exception e)
    {
        _logger.LogError(e, "Exception occured while processing request: exception: {Exception}", e.Message);
        return StatusCode(500, e.Message);
    }
}