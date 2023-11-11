using Microsoft.AspNetCore.HttpLogging;
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
        var serviceName = builder.Configuration["ServiceName"]!;
        resBuilder.AddService(serviceName);
        options.SetResourceBuilder(resBuilder);

        options.AddOtlpExporter();
    });


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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
        .AddHttpClientInstrumentation()
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