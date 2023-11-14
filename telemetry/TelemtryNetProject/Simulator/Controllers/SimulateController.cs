using Microsoft.AspNetCore.Mvc;
using Simulator.Models;
using Simulator.Services;

namespace Simulator.Controllers;

[ApiController]
[Route("[controller]")]
public class SimulateController : ControllerBase
{
    private readonly ILogger<SimulateController> _logger;
    private readonly UserSimulatorService _simulatorService;

    public SimulateController(ILogger<SimulateController> logger, UserSimulatorService simulatorService)
    {
        _logger = logger;
        _simulatorService = simulatorService;
    }

    [HttpPost("user")]
    public async Task<IActionResult> SimulateCreateUsers(SimulateCreateUsersRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _simulatorService.CreateUsers(request, cancellationToken);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError("Error while simulating create users: {Exception}", e);
            return StatusCode(500, "Error while simulating create users");
        }
    }
}