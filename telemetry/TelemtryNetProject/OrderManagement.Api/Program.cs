using System.Reflection;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OrderManagement.Domain.Repositories;
using OrderManagement.Infrastructure.Repositories;
using OrderManagementApi.ExternalServices;
using OrderManagementApi.Metrics;
using OrderManagementApi.Services;
using OrderManagementApi.Workers;

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
        Title = "Order management API",
        Description = "An ASP.NET Core Web API for manage orders"
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddHttpLogging(o => o.LoggingFields = HttpLoggingFields.All);
builder.Services.AddHealthChecks();
builder.Services.AddTransient<IOrderRepository, OrderRepository>();
builder.Services.AddTransient<OrderService>();

builder.Services.AddHostedService<MessagesProcessingService>();

builder.Services.AddSingleton<OrderMetrics>();

var meters = new OrderMetrics();
builder.Services.AddHttpClient<UserService>(c =>
{
    var url = builder.Configuration["UserServiceUrl"];
    c.BaseAddress = new Uri(url!);
});

builder.Services.AddOpenTelemetry()
    .ConfigureResource(b => { b.AddService(builder.Configuration["ServiceName"]!); })
    .WithTracing(b => b
        .AddAspNetCoreInstrumentation(o => { o.Filter = ctx => ctx.Request.Path != "/metrics"; })
        .AddHttpClientInstrumentation()
        .AddSource(MessagesProcessingService.TraceActivityName)
        .AddOtlpExporter())
    .WithMetrics(b => b
        .AddAspNetCoreInstrumentation(o => { o.Filter = (_, ctx) => ctx.Request.Path != "/metrics"; })
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation()
        .AddProcessInstrumentation()
        .AddMeter(meters.MetricName)
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