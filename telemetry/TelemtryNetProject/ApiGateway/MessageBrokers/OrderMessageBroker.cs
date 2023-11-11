using System.Diagnostics;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using TelemtryNetProject.Contracts.Order.RabbitMq.v1.Requests;

namespace ApiGateway.MessageBrokers;

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
        using var activity = TraceActivitySource.StartActivity(nameof(PublishMessage), ActivityKind.Producer);

        var factory = new ConnectionFactory {
            Uri = new Uri(_configuration["RabbitMq:ConnectionString"]!)
        };
        
        using var connection = factory.CreateConnection();
        using var channel = connection!.CreateModel();

        channel.QueueDeclare(
            queue: _queueName,
            durable: false,
            exclusive: false,
            autoDelete: false
        );
        
        _logger.LogInformation("Publishing message to queue: {QueueName}", _queueName);
        
        var basicProperties = channel.CreateBasicProperties();

        if (activity?.Id != null)
        {
            basicProperties.Headers = new Dictionary<string, object>
            {
                { "traceparent", activity.Id }
            };
        }

        channel.BasicPublish(
            exchange: String.Empty,
            routingKey: _queueName,
            basicProperties: basicProperties, 
            body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message)));
        
        _logger.LogInformation("Message published to queue: {QueueName}", _queueName);
    }
}