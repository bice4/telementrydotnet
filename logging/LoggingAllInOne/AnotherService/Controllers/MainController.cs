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

    [HttpPost]
    public async Task<IActionResult> Post(MainRequest mainRequest)
    {
        _logger.LogInformation("Received request {@MainRequest}", mainRequest);
        
        if (_random.Next(0, 2) == 0)
        {
            await Task.Delay(1000);
            _logger.LogInformation("Returning 400");
            return StatusCode(400);
        }

        await Task.Delay(500);

        var serviceName = _configuration.GetValue<string>("SERVICE_NAME", "Unknown");


        _logger.LogInformation("Returning 200 with serviceName: {ServiceName}", serviceName);
        return Ok(new MainResponse($"Hello from serviceName: {serviceName}"));
    }
}