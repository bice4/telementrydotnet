using System.Reflection;
using Microsoft.AspNetCore.HttpLogging;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

builder.Logging.ClearProviders();

builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));

builder.Services.AddHttpLogging(o => o.LoggingFields = HttpLoggingFields.All);

// Configure OpenTelemetry
builder.Services.AddOpenTelemetry()
    .ConfigureResource(b =>
    {
        b.AddService("WebApi");
    })
    .WithMetrics(b => b
        .AddAspNetCoreInstrumentation(o =>
        {
            o.Filter = (_, ctx) => ctx.Request.Path != "/metrics";
        })
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation()
        .AddProcessInstrumentation()
        .AddPrometheusExporter());


var app = builder.Build();


// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseSerilogRequestLogging();
app.MapPrometheusScrapingEndpoint();
app.UseHttpLogging();
app.MapControllers();
app.Run();