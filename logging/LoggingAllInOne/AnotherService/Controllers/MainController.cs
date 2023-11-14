using Contracts.Requests;
using Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace AnotherService.Controllers;

[ApiController]
[Route("[controller]")]
public class MainController : ControllerBase
{
    private readonly ILogger<MainController> _logger;
    private readonly IConfiguration _configuration;
    private readonly Random _random = Random.Shared;

    public MainController(ILogger<MainController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Simple endpoint that returns 200 after a delay or 400 with a 50% chance
    /// </summary>
    /// <param name="mainRequest"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Post(MainRequest mainRequest, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received request {@MainRequest}", mainRequest);

        // Simulate some random errors
        if (_random.Next(0, 2) == 0)
        {
            // Simulate a 400 error
            await Task.Delay(1000, cancellationToken);
            _logger.LogInformation("Returning 400");
            return StatusCode(400);
        }

        // Wait for a while to simulate some work
        await Task.Delay(500, cancellationToken);

        // Get the service name from the environment variable SERVICE_NAME
        var serviceName = _configuration.GetValue<string>("SERVICE_NAME", "Unknown");

        // Log the service name
        _logger.LogInformation("Returning 200 with serviceName: {ServiceName}", serviceName);

        // Return the service name in the response
        return new OkObjectResult(new MainResponse($"Hello from serviceName: {serviceName}"));
    }
}