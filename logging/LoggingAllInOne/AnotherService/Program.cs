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
    lc.WriteTo.Console();

    var serviceName = hbc.Configuration.GetValue<string>("SERVICE_NAME", "Unknown");

    var env = hbc.Configuration.GetValue<string>("ASPNETCORE_ENVIRONMENT", "Unknown");

    var logPath = Path.Combine(env == "Development" ? "C:\\Projects\\telementrydotnet\\logging\\Logs\\" : "Logs", $"{serviceName}.log");

    lc.WriteTo.File(logPath, rollingInterval: RollingInterval.Day);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();