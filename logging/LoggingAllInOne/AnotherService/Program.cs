using AnotherService.Workers;
using Serilog;
using Serilog.Sinks.Grafana.Loki;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHostedService<TimerWorker>();

builder.Logging.ClearProviders();
builder.Configuration.AddEnvironmentVariables();

builder.Host.UseSerilog((hbc, lc) =>
{
    lc.MinimumLevel.Information();
    lc.WriteTo.Console();
    lc.Enrich.WithMachineName();
    lc.Enrich.WithThreadId();

    var serviceName = hbc.Configuration.GetValue<string>("SERVICE_NAME", "Unknown");
    var env = hbc.Configuration.GetValue<string>("ASPNETCORE_ENVIRONMENT", "Development");

    // var logPath = Path.Combine(env == "Development" ? "C:\\Projects\\telementrydotnet\\logging\\Logs\\" : "Logs", $"{serviceName}.log");

    // lc.WriteTo.File(logPath, rollingInterval: RollingInterval.Day);

    lc.WriteTo.GrafanaLoki("http://host.docker.internal:3100", new LokiLabel[] {
        new() { Key = "service", Value = serviceName! },
        new() { Key = "environment", Value = env! },
    }, new List<string>() { "service" }, textFormatter: new LokiJsonTextFormatter());
});


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseSerilogRequestLogging();

app.MapControllers();
app.Run();