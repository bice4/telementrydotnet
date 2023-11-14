using System.Reflection;
using ApiGateway.ExternalServices;
using ApiGateway.MessageBrokers;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

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
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo {
        Version = "v1",
        Title = "Api gateway API",
        Description = "An ASP.NET Core Web API for entry point to the system"
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddHttpLogging(o => o.LoggingFields = HttpLoggingFields.All);
builder.Services.AddHealthChecks();
builder.Services.AddSingleton<OrderMessageBroker>();

builder.Services.AddHttpClient<ValidationServiceHttpClient>(c =>
{
    var url = builder.Configuration["ValidationServiceUrl"];
    c.BaseAddress = new Uri(url!);
});

builder.Services.AddHttpClient<UserServiceHttpClient>(c =>
{
    var url = builder.Configuration["UserServiceUrl"];
    c.BaseAddress = new Uri(url!);
});

builder.Services.AddHttpClient<OrderServiceHttpClient>(c =>
{
    var url = builder.Configuration["OrderServiceUrl"];
    c.BaseAddress = new Uri(url!);
});

builder.Services.AddOpenTelemetry()
    .ConfigureResource(b => { b.AddService(builder.Configuration["ServiceName"]!); })
    .WithTracing(b => b
        .AddAspNetCoreInstrumentation(o => { o.Filter = ctx => ctx.Request.Path != "/metrics"; })
        .AddHttpClientInstrumentation()
        .AddSource(OrderMessageBroker.TraceActivityName)
        .AddOtlpExporter())
    .WithMetrics(b => b
        .AddAspNetCoreInstrumentation(o => { o.Filter = (_, ctx) => ctx.Request.Path != "/metrics"; })
        .AddHttpClientInstrumentation()
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