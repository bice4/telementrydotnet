using System.Diagnostics;
using System.Text;
using System.Text.Json;
using OrderManagementApi.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TelemetryDotNet.Contracts.Order.RabbitMq.v1.Requests;

namespace OrderManagementApi.Workers;

/// <summary>
/// Messages processing service, responsible for processing messages from RabbitMQ
/// Producer: ApiGateway
/// Process message: <see cref="PlaceOrderModel"/>
/// </summary>
public class MessagesProcessingService : IHostedService
{
    public static readonly string TraceActivityName = typeof(MessagesProcessingService).FullName!;
    private static readonly ActivitySource TraceActivitySource = new(TraceActivityName);

    private readonly ILogger<MessagesProcessingService> _logger;

    private readonly OrderService _orderService;
    private readonly IConfiguration _configuration;

    private readonly string _queueName;

    private EventingBasicConsumer? _consumer;

    public MessagesProcessingService(IConfiguration configuration, ILogger<MessagesProcessingService> logger,
        OrderService orderService)
    {
        _configuration = configuration;
        _logger = logger;
        _orderService = orderService;

        _queueName = _configuration.GetSection("RabbitMq:QueueName").Value ?? "orders";
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _consumer = await StartRabbitAndCreateConsumer(cancellationToken);

        if (_consumer == null)
        {
            _logger.LogError("Error while starting RabbitMQ, consumer is null");
            return;
        }

        _consumer.Received += ConsumerOnReceived;
    }

    private async Task<EventingBasicConsumer> StartRabbitAndCreateConsumer(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory {
            Uri = new Uri(_configuration["RabbitMq:ConnectionString"]!)
        };

        var connected = false;

        while (!connected)
        {
            try
            {
                var connection = factory.CreateConnection();

                _logger.LogInformation("Connected to RabbitMQ");

                var channel = connection!.CreateModel();

                channel.QueueDeclare(
                    queue: _queueName,
                    durable: false,
                    exclusive: false,
                    autoDelete: false
                );

                _logger.LogInformation("Connected to RabbitMQ and queue {QueueName} declared", _queueName);


                var consumer = new EventingBasicConsumer(channel);
                channel.BasicConsume(
                    queue: _queueName,
                    autoAck: true,
                    consumer: consumer);

                connected = true;

                return consumer;
            }
            catch (Exception e)
            {
                _logger.LogError("Error while connecting to RabbitMQ, will try again in 2 sec exception: {Exception}",
                    e.Message);
                await Task.Delay(2000, cancellationToken);
            }
        }

        return null!;
    }

    private void ProcessMessage(BasicDeliverEventArgs e)
    {
        // Extract parent activity id from message headers, for distributed tracing
        string? parentActivityId = null;
        if (e.BasicProperties?.Headers?.TryGetValue("traceparent", out var parentActivityIdRaw) == true &&
            parentActivityIdRaw is byte[] traceParentBytes)
            parentActivityId = Encoding.UTF8.GetString(traceParentBytes);

        using var activity = TraceActivitySource.StartActivity(nameof(ProcessMessage), kind: ActivityKind.Consumer,
            parentId: parentActivityId);

        try
        {
            var body = e.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            _logger.LogInformation("Received message: {Message}", message);

            var order = JsonSerializer.Deserialize<PlaceOrderModel>(message);

            if (order != null)
                _orderService.PlaceOrder(order, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();
            else
                _logger.LogError("Received message is null");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error while processing message, exception: {Exception}", exception.Message);
        }
    }

    public async Task StopAsync(CancellationToken stoppingToken)
    {
        if (_consumer != null)
            _consumer.Received -= ConsumerOnReceived;

        await Task.CompletedTask;
    }

    private void ConsumerOnReceived(object? sender, BasicDeliverEventArgs e)
    {
        ProcessMessage(e);
    }
}