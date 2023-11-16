using System.Reflection;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Simulator.ExternalService;
using Simulator.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSingleton<UserSimulatorService>();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo {
        Version = "v1",
        Title = "Simulator API",
        Description = "An ASP.NET Core Web API for simulate load"
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddHttpClient<ApiGatewayHttpClient>(c =>
{
    var url = builder.Configuration["ApiGatewayUrl"];
    c.BaseAddress = new Uri(url!);
});
builder.Logging.ClearProviders();
builder.Configuration.AddEnvironmentVariables();

builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));

builder.Services.AddOpenTelemetry()
    .ConfigureResource(b => { b.AddService("Simulator"); })
    .WithTracing(b => b
        .AddAspNetCoreInstrumentation(o => { o.Filter = ctx => ctx.Request.Path != "/metrics"; })
        .AddHttpClientInstrumentation());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();