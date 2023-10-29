using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace MetricsPlayground.Controllers;

[ApiController]
[Route("[controller]")]
public class MainController : ControllerBase
{
    private static readonly string[] Summaries = new[] {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<MainController> _logger;
    private readonly IMemoryCache _memoryCache;

    public MainController(ILogger<MainController> logger, IMemoryCache memoryCache)
    {
        _logger = logger;
        _memoryCache = memoryCache;
    }

    [HttpGet]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
    }
    
    [HttpGet("{key}")]
    public IActionResult GetValue(string key)
    {
         _logger.LogDebug("Received request to get value with {Key}", key);

        try
        {
            if (_memoryCache.TryGetValue(key, out string? value))
                return Ok(value);

            _logger.LogWarning("Value with {Key} not found", key);
            return NotFound();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting value with {Key}", key);
            return StatusCode(500, new { e.Message });
        }
    }
    
    [HttpPost]
    public IActionResult SaveValue(KeyValueRequest request)
    {
        _logger.LogDebug("Data: {@Request}", request);

        try
        {
            _memoryCache.Set(request.Key, request.Value);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while setting value with {Key}", request.Key);
            return StatusCode(500, new { e.Message });
        }
    }
    
    [HttpPut]
    public IActionResult UpdateValue(KeyValueRequest request)
    {
        _logger.LogDebug("Data: {@Request}", request);

        try
        {
            _memoryCache.Set(request.Key, request.Value);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while updating value with {Key}", request.Key);
            return StatusCode(500, new { e.Message });
        }
    }
    
    [HttpDelete("{key}")]
    public IActionResult DeleteValue(string key)
    {
        _logger.LogDebug("Received request to delete value with {Key}", key);

        try
        {
            _memoryCache.Remove(key);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while deleting value with {Key}", key);
            return StatusCode(500, new { e.Message });
        }
    }

    [HttpPost("load")]
    public IActionResult LoadAdd(LoadRequest loadRequest)
    {
        _logger.LogDebug("Data: {@Request}", loadRequest);

        try
        {

            for (var i = 0; i < loadRequest.Count; i++)
            {
                _memoryCache.Set($"key-{i}-{loadRequest.Prefix}", Guid.NewGuid().ToString());
            }
            
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while setting value with {@Request}", loadRequest);
            return StatusCode(500, new { e.Message });
        }
    }
}