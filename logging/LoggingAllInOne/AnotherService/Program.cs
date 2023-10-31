using AnotherService.Workers;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHostedService<TimerWorker>();

builder.Logging.ClearProviders();
builder.Configuration.AddEnvironmentVariables();

builder.Host.UseSerilog((hbc, lc) =>
{
    var serviceName = hbc.Configuration.GetValue<string>("SERVICE_NAME", "Unknown")!;

    lc.Enrich.WithMachineName();
    lc.Enrich.WithThreadId();
    lc.Enrich.WithThreadName();
    lc.Enrich.WithEnvironmentUserName();
    lc.Enrich.WithMemoryUsage();
    lc.Enrich.WithProperty("ServiceName", serviceName);
    
    lc.Enrich.FromLogContext();
    lc.WriteTo.Console();

    //
    // var env = hbc.Configuration.GetValue<string>("ASPNETCORE_ENVIRONMENT", "Unknown");

    #region Write to file 

    //var logPath = Path.Combine(env == "Development" ? "C:\\Projects\\telementrydotnet\\logging\\Logs\\" : "Logs", $"{serviceName}.log");

    //lc.WriteTo.File(logPath, rollingInterval: RollingInterval.Day);

    #endregion

    #region Write to Seq
    
    var seqUrl = hbc.Configuration.GetValue<string>("SEQ_URL", "http://host.docker.internal:5341");
    
    lc.WriteTo.Seq(seqUrl!);
    
    #endregion
    
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseSerilogRequestLogging();

app.Run();