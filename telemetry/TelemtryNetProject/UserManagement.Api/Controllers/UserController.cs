using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using TelemtryNetProject.Contracts.UserManagement.Api.V1.Requests;
using TelemtryNetProject.Contracts.UserManagement.Api.V1.Responses;
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

    [HttpGet(Name = "GetAll")]
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

    [HttpGet("{id}", Name = "GetById")]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetById called with id: {Id}", id);

        try
        {
            var user = await _userRepository.GetUserAsync(ObjectId.Parse(id), cancellationToken);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user.ToUserFullDto());
        }
        catch (Exception e)
        {
            return LogExceptionAndReturnError(e);
        }
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateUser(CreateUserRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("CreateUser called with request: {Request}", request);

        try
        {
            var passwordHash = _passwordHasherService.HashPassword(request.Password);
            var user = request.ToUser();

            user.UpdatePassword(passwordHash);

            await _userRepository.AddUserAsync(user, cancellationToken);

            _userMetrics.UpdateUserMetrics(1);

            return new OkObjectResult(user.Id.ToString());
        }
        catch (Exception e)
        {
            return LogExceptionAndReturnError(e);
        }
    }

    [HttpPut("updateAddress")]
    public async Task<IActionResult> UpdateUserAddress(UpdateUserAddressRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("UpdateUserAddress called with request: {Request}", request);

        try
        {
            var user = await _userRepository.GetUserAsync(ObjectId.Parse(request.UserId), cancellationToken);

            if (user == null)
            {
                return NotFound();
            }

            user.UpdateAddress(request.ToAddress());

            await _userRepository.UpdateUserAsync(user, cancellationToken);

            return Ok();
        }
        catch (Exception e)
        {
            return LogExceptionAndReturnError(e);
        }
    }

    [HttpPut("updatePassword")]
    public async Task<IActionResult> UpdateUserPassword(UpdateUserPasswordRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("UpdateUserPassword called with request: {Request}", request);

        try
        {
            var user = await _userRepository.GetUserAsync(ObjectId.Parse(request.UserId), cancellationToken);

            if (user == null)
            {
                return NotFound();
            }

            var passwordHash = _passwordHasherService.HashPassword(request.Password);
            user.UpdatePassword(passwordHash);

            await _userRepository.UpdateUserAsync(user, cancellationToken);

            return Ok();
        }
        catch (Exception e)
        {
            return LogExceptionAndReturnError(e);
        }
    }

    [HttpDelete("{id}")]
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

            _userMetrics.UpdateUserMetrics(-1);

            return Ok();
        }
        catch (Exception e)
        {
            return LogExceptionAndReturnError(e);
        }
    }

    [HttpGet("exists/{email}", Name = "Exists")]
    public async Task<IActionResult> IsUserEmailExists(string email, CancellationToken cancellationToken)
    {
        _logger.LogInformation("IsUserEmailExists called with id: {Email}", email);

        try
        {
            var isEmailUniqueAsync = await _userRepository.IsEmailExists(email, cancellationToken);

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