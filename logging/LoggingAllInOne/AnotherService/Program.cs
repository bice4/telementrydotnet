using AnotherService.Workers;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHostedService<TimerWorker>();

builder.Logging.ClearProviders();
builder.Configuration.AddEnvironmentVariables();

builder.Host.UseSerilog((hbc, lc) =>
{
    var serviceName = hbc.Configuration.GetValue<string>("SERVICE_NAME", "Unknown")!;

    // Add enrichers
    lc.Enrich.WithMachineName();
    lc.Enrich.WithThreadId();
    lc.Enrich.WithThreadName();
    lc.Enrich.WithEnvironmentUserName();
    lc.Enrich.WithMemoryUsage();
    lc.Enrich.WithProperty("ServiceName", serviceName);
    
    lc.Enrich.FromLogContext();
    lc.WriteTo.Console();
    
    #region Write to file 
    
    //var env = hbc.Configuration.GetValue<string>("ASPNETCORE_ENVIRONMENT", "Unknown");

    //var logPath = Path.Combine(env == "Development" ? "C:\\Projects\\telementrydotnet\\logging\\Logs\\" : "Logs", $"{serviceName}.log");

    //lc.WriteTo.File(logPath, rollingInterval: RollingInterval.Day);

    #endregion

    #region Write to Seq
    
    // Get SEQ_URL from environment variables, default to http://host.docker.internal:5341
    var seqUrl = hbc.Configuration.GetValue<string>("SEQ_URL", "http://host.docker.internal:5341");
    
    // Write to SEQ_URL
    lc.WriteTo.Seq(seqUrl!);
    
    #endregion
    
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseSerilogRequestLogging();

app.Run();