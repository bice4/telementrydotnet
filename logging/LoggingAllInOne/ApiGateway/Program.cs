using System.Reflection;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo {
        Version = "v1",
        Title = "Api gateway API",
        Description = "An ASP.NET Core Web API for as example"
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddHttpClient();

builder.Logging.ClearProviders();
builder.Configuration.AddEnvironmentVariables();

// Read serilog configuration from appsettings.json
builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
    loggerConfiguration.Enrich.WithProperty("ServiceName", "ApiGateway");
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseSerilogRequestLogging();

app.MapControllers();
app.Run();