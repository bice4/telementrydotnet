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


// Configure OpenTelemetry
builder.Services.AddOpenTelemetry()
    .WithMetrics(b =>
    {
        b.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("WebApi"));
        b.AddAspNetCoreInstrumentation(o => { o.Filter = (_, ctx) => ctx.Request.Path != "/metrics"; });
        b.AddHttpClientInstrumentation();
        b.AddRuntimeInstrumentation();
        b.AddProcessInstrumentation();
        b.AddPrometheusExporter(x => x.ScrapeResponseCacheDurationMilliseconds = 0);
    });


var app = builder.Build();


// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseSerilogRequestLogging();
app.MapPrometheusScrapingEndpoint();
app.MapControllers();
app.Run();