using Microsoft.AspNetCore.Server.Kestrel.Core;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

var serviceName = builder.Configuration["ServiceName"] ?? "InvoiceGeneratorService";

builder.Logging
    .ClearProviders()
    .AddConsole()
    .AddOpenTelemetry(options =>
    {
        options.IncludeFormattedMessage = true;
        options.IncludeScopes = true;

        var resBuilder = ResourceBuilder.CreateDefault();
        resBuilder.AddService(serviceName);
        options.SetResourceBuilder(resBuilder);

        options.AddOtlpExporter();
    });

builder.Services.AddHealthChecks();
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

builder.Services.AddGrpc();
builder.Services.AddOpenTelemetry()
    .ConfigureResource(b => { b.AddService(serviceName); })
    .WithTracing(b => b
        .AddAspNetCoreInstrumentation(o => { o.Filter = ctx => ctx.Request.Path != "/metrics"; })
        .AddHttpClientInstrumentation()
        .AddGrpcClientInstrumentation()
        .AddOtlpExporter())
    .WithMetrics(b => b
        .AddAspNetCoreInstrumentation(o => { o.Filter = (_, ctx) => ctx.Request.Path != "/metrics"; })
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation()
        .AddProcessInstrumentation()
        .AddPrometheusExporter());

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8099, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
    
    options.ListenAnyIP(8098, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1;
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<InvoiceGeneratorService.Services.InvoiceGeneratorService>();

app.MapHealthChecks("/health");
app.UseOpenTelemetryPrometheusScrapingEndpoint();
app.UseDeveloperExceptionPage();
app.Run();