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

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        _logger.LogInformation("Some log");
        await Task.Delay(300);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Post(MainRequest request)
    {
        _logger.LogInformation("Received request {@MainRequest}", request);

        using var client = _httpClientFactory.CreateClient("AnotherService");

        var serviceCount = GetServiceCount(out var isDocker);

        if (serviceCount == 1)
        {
            client.BaseAddress = isDocker ? new Uri("http://host.docker.internal:8041") : new Uri("http://localhost:8041");
        }
        else
        {
            var serviceNumber = _random.Next(1, serviceCount);


            client.BaseAddress = isDocker
                ? new Uri($"http://host.docker.internal:804{serviceNumber}")
                : new Uri($"http://localhost:804{serviceNumber}");
        }
        
        _logger.LogInformation("Calling service {Service}", client.BaseAddress);

        var response = await client.PostAsJsonAsync("Main", request);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Error while calling AnotherService");
            return StatusCode(500);
        }

        var mainResponse = await response.Content.ReadFromJsonAsync<MainResponse>();
        _logger.LogInformation("Returning 200 with response {@MainResponse}", mainResponse);

        return Ok(mainResponse);
    }

    private int GetServiceCount(out bool isDocker)
    {
        var env = _configuration.GetValue<string>("ASPNETCORE_ENVIRONMENT", "Development");

        isDocker = env == "Docker";

        if (env == "Development")
            return 1;

        var serviceCount = _configuration.GetValue<int>("SERVICE_COUNT", 1);
        return serviceCount;
    }
}