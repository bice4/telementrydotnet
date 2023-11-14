namespace AnotherService.Workers;

/// <summary>
/// Simple counter worker that logs every configured interval
/// </summary>
public class TimerWorker : BackgroundService
{
    private const int DEFAULT_TIMER_INTERVAL_MS = 1000;

    private readonly ILogger<TimerWorker> _logger;
    private readonly IConfiguration _configuration;

    private int _counter = 0;

    public TimerWorker(ILogger<TimerWorker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var timerInterval = _configuration.GetValue<int>("TimerIntervalMs", DEFAULT_TIMER_INTERVAL_MS);
            _counter++;
            _logger.LogInformation("Next tick in {TimerInterval} ms. Counter: {Counter}", timerInterval, _counter);
            await Task.Delay(timerInterval, stoppingToken);
        }
    }
}