using System.Reflection;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using UserManagement.Domain.Repositories;
using UserManagement.Infrastructure.Repositories;
using UserManagement.Infrastructure.Services;
using UserManagement.Metrics;

var builder = WebApplication.CreateBuilder(args);

builder.Logging
    .ClearProviders()
    .AddConsole()
    .AddOpenTelemetry(options =>
    {
        options.IncludeFormattedMessage = true;
        options.IncludeScopes = true;

        var resBuilder = ResourceBuilder.CreateDefault();
        var serviceName = builder.Configuration["ServiceName"] ?? "UserManagement";
        resBuilder.AddService(serviceName);
        options.SetResourceBuilder(resBuilder);

        options.AddOtlpExporter();
    });


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo {
        Version = "v1",
        Title = "User Management API",
        Description = "An ASP.NET Core Web API for managing users"
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IPasswordHasherService, PasswordHasherHasherService>();
builder.Services.AddHttpLogging(o => o.LoggingFields = HttpLoggingFields.All);
builder.Services.AddHealthChecks();
builder.Services.AddSingleton<UserMetrics>();

var meters = new UserMetrics();

builder.Services.AddOpenTelemetry()
    .ConfigureResource(b => { b.AddService(builder.Configuration["ServiceName"]!); })
    .WithTracing(b => b
        .AddAspNetCoreInstrumentation(o => { o.Filter = ctx => ctx.Request.Path != "/metrics"; })
        .AddHttpClientInstrumentation(x =>
        {
            x.RecordException = true;
        })
        .AddOtlpExporter())
    .WithMetrics(b => b
        .AddAspNetCoreInstrumentation(o => { o.Filter = (_, ctx) => ctx.Request.Path != "/metrics"; })
        .AddHttpClientInstrumentation()
        .AddMeter(meters.MetricName)
        .AddRuntimeInstrumentation()
        .AddProcessInstrumentation()
        .AddPrometheusExporter());

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();


app.MapHealthChecks("/health");
app.UseOpenTelemetryPrometheusScrapingEndpoint();
app.UseHttpLogging();
app.UseDeveloperExceptionPage();
app.MapControllers();
app.Run();