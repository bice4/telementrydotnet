using System.Diagnostics;
using System.Text;
using System.Text.Json;
using OrderManagementApi.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using TelemtryNetProject.Contracts.Order.RabbitMq.v1.Requests;

namespace OrderManagementApi.Workers;

public class MessagesProcessingService : BackgroundService
{
    public static readonly string TraceActivityName = typeof(MessagesProcessingService).FullName!;
    private static readonly ActivitySource TraceActivitySource = new(TraceActivityName);

    private readonly ILogger<MessagesProcessingService> _logger;

    private readonly IConnection _connection;
    private readonly IModel _channel;

    private readonly OrderService _orderService;

    private readonly string _queueName;

    public MessagesProcessingService(
        IConfiguration configuration,
        IHostApplicationLifetime hostApplicationLifetime, ILogger<MessagesProcessingService> logger,
        OrderService orderService)
    {
        _logger = logger;
        _orderService = orderService;

        var factory = new ConnectionFactory {
            Uri = new Uri(configuration["RabbitMq:ConnectionString"]!)
        };

        while (!hostApplicationLifetime.ApplicationStopping.IsCancellationRequested)
        {
            try
            {
                _connection = factory.CreateConnection();
                break;
            }
            catch (BrokerUnreachableException)
            {
                // Ignore
            }
        }

        _queueName = configuration.GetSection("RabbitMq:QueueName").Value ?? "orders";

        hostApplicationLifetime.ApplicationStopping.ThrowIfCancellationRequested();

        _channel = _connection!.CreateModel();

        _channel.QueueDeclare(
            queue: _queueName,
            durable: false,
            exclusive: false,
            autoDelete: false
        );
        
        _logger.LogInformation("Connected to RabbitMQ and queue {QueueName} declared", _queueName);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (_, e) => ProcessMessage(e);

        _channel.BasicConsume(
            queue: _queueName,
            autoAck: true,
            consumer: consumer);

        return Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private void ProcessMessage(BasicDeliverEventArgs e)
    {
        string? parentActivityId = null;
        if (e.BasicProperties?.Headers?.TryGetValue("traceparent", out var parentActivityIdRaw) == true &&
            parentActivityIdRaw is byte[] traceParentBytes)
            parentActivityId = Encoding.UTF8.GetString(traceParentBytes);

        using var activity = TraceActivitySource.StartActivity(nameof(ProcessMessage), kind: ActivityKind.Consumer,
            parentId: parentActivityId);

        var body = e.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);

        _logger.LogInformation("Received message: {Message}", message);


        try
        {
            var order = JsonSerializer.Deserialize<PlaceOrderModel>(message);

            if (order != null)
                _orderService.PlaceOrder(order, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();
            else
            {
                _logger.LogError("Received message is null");
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error while processing message, exception: {Exception}", exception.Message);
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        await base.StopAsync(stoppingToken);

        _channel.Close();
        _connection.Close();
    }

    public override void Dispose()
    {
        base.Dispose();

        _channel.Dispose();
        _connection.Dispose();
    }
}