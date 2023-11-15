using System.Diagnostics;
using Contracts.Requests;
using Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers;

[ApiController]
[Route("[controller]")]
public class MainController : ControllerBase
{
    private readonly ILogger<MainController> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly Random _random = Random.Shared;

    public MainController(ILogger<MainController> logger, IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }
    
    /// <summary>
    /// Simple endpoint that returns 200 after a delay
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Some log");
        await Task.Delay(300, cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Send request to AnotherService
    /// </summary>
    /// <param name="request"><see cref="MainRequest"/> Model</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(MainResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Post(MainRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received request {MainRequestId}, {ReferenceId}", request.Id, request.ReferenceId);

        // Create a new HttpClient with the name "AnotherService"
        using var client = _httpClientFactory.CreateClient("AnotherService");

        var serviceCount = GetServiceCount(out var isDocker);
        
        // Add headers to the request so that AnotherService can log the request source
        client.DefaultRequestHeaders.Add("X-Forwarded-For", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown");
        client.DefaultRequestHeaders.Add("X-Forwarded-Host", HttpContext.Request.Host.ToString());
        client.DefaultRequestHeaders.Add("X-Forwarded-Proto", HttpContext.Request.Scheme);
        client.DefaultRequestHeaders.Add("X-Forwarded-Port", HttpContext.Request.Host.Port.ToString());
        client.DefaultRequestHeaders.Add("TraceId", Activity.Current?.TraceId.ToString() ?? "Unknown");

        // If we are running in development, we only have one service
        if (serviceCount == 1)
        {
            client.BaseAddress = isDocker ? new Uri("http://host.docker.internal:8041") : new Uri("http://localhost:8041");
        }
        else
        {
            var serviceNumber = _random.Next(1, serviceCount);

            // If we are running in docker, we need to use the host.docker.internal address
            client.BaseAddress = isDocker
                ? new Uri($"http://host.docker.internal:804{serviceNumber}")
                : new Uri($"http://localhost:804{serviceNumber}");
        }
        
        _logger.LogInformation("Calling service {Service}", client.BaseAddress);

        // Send the request to AnotherService
        var response = await client.PostAsJsonAsync("Main", request, cancellationToken: cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            // If the request failed, log the error and return 500
            _logger.LogError("Error while calling AnotherService");
            return StatusCode(500);
        }

        // If the request succeeded, read the response and return it
        var mainResponse = await response.Content.ReadFromJsonAsync<MainResponse>(cancellationToken: cancellationToken);
        _logger.LogInformation("Returning 200 with response {@MainResponse}", mainResponse);

        return new OkObjectResult(mainResponse);
    }

    /// <summary>
    /// Get service count from environment variables
    /// </summary>
    /// <param name="isDocker"></param>
    /// <returns></returns>
    private int GetServiceCount(out bool isDocker)
    {
        var env = _configuration.GetValue<string>("ASPNETCORE_ENVIRONMENT", "Development");

        isDocker = env == "Docker";

        // If we are running in development, we only have one service
        if (env == "Development")
            return 1;

        var serviceCount = _configuration.GetValue<int>("SERVICE_COUNT", 1);
        return serviceCount;
    }
}