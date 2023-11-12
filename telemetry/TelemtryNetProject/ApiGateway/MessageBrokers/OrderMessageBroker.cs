using System.Diagnostics;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using TelemtryNetProject.Contracts.Order.RabbitMq.v1.Requests;

namespace ApiGateway.MessageBrokers;

/// <summary>
/// Message broker for order
/// Can be used to publish messages to queue
/// Consumer is in OrderManagement.Api
/// Place message <see cref="PlaceOrderModel"/>
/// </summary>
public class OrderMessageBroker
{
    public static readonly string TraceActivityName = typeof(OrderMessageBroker).FullName!;
    private static readonly ActivitySource TraceActivitySource = new(TraceActivityName);
    
    private readonly string _queueName;
    
    private readonly IConfiguration _configuration;
    
    private readonly ILogger<OrderMessageBroker> _logger;

    public OrderMessageBroker(IConfiguration configuration, ILogger<OrderMessageBroker> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _queueName = _configuration.GetSection("RabbitMq:QueueName").Value ?? "orders";

    }
    
    public void PublishMessage(PlaceOrderModel message)
    {
        // Start activity for tracing
        using var activity = TraceActivitySource.StartActivity(nameof(PublishMessage), ActivityKind.Producer);

        var factory = new ConnectionFactory {
            Uri = new Uri(_configuration["RabbitMq:ConnectionString"]!)
        };
        
        // Create connection and channel to rabbitmq
        using var connection = factory.CreateConnection();
        using var channel = connection!.CreateModel();

        // Declare queue if not exists
        channel.QueueDeclare(
            queue: _queueName,
            durable: false,
            exclusive: false,
            autoDelete: false
        );
        
        _logger.LogInformation("Publishing message to queue: {QueueName}", _queueName);
        
        var basicProperties = channel.CreateBasicProperties();

        // Add traceparent header to message for tracing
        if (activity?.Id != null)
        {
            basicProperties.Headers = new Dictionary<string, object>
            {
                { "traceparent", activity.Id }
            };
        }

        // Publish message to queue, direct exchange
        channel.BasicPublish(
            exchange: String.Empty,
            routingKey: _queueName,
            basicProperties: basicProperties, 
            body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message)));
        
        _logger.LogInformation("Message published to queue: {QueueName}", _queueName);
    }
}