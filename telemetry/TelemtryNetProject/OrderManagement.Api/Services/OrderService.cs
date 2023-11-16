using System.Diagnostics;
using OrderManagement.Domain.Repositories;
using OrderManagementApi.ExternalServices;
using OrderManagementApi.Metrics;
using OrderManagementApi.Translators;
using TelemetryDotNet.Contracts.Order.RabbitMq.v1.Requests;

namespace OrderManagementApi.Services;

public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<OrderService> _logger;
    private readonly OrderMetrics _metrics;
    private readonly UserService _userService;
    private readonly ExternalServices.InvoiceGeneratorService _invoiceGeneratorService;

    public OrderService(IOrderRepository orderRepository, ILogger<OrderService> logger, OrderMetrics metrics,
        UserService userService, ExternalServices.InvoiceGeneratorService invoiceGeneratorService)
    {
        _orderRepository = orderRepository;
        _logger = logger;
        _metrics = metrics;
        _userService = userService;
        _invoiceGeneratorService = invoiceGeneratorService;
    }

    /// <summary>
    /// Place order for user
    /// </summary>
    /// <param name="placeOrderModel"><see cref="PlaceOrderModel"/> model</param>
    /// <param name="cancellationToken"></param>
    public async Task PlaceOrder(PlaceOrderModel placeOrderModel, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Placing order {PlaceOrderModel}", placeOrderModel);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Call user service to create user and get user id
            var createUserResponse =
                await _userService.CreateUser(placeOrderModel.User, cancellationToken: cancellationToken);

            if (createUserResponse == null)
            {
                _logger.LogError("Error while creating user");
                return;
            }

            try
            {
                // Create order
                var order = placeOrderModel.Order.ToOrder(createUserResponse.UserId);

                // Add order
                await _orderRepository.AddOrderAsync(order, cancellationToken);

                // Generate invoice
                var invoiceId =
                    await _invoiceGeneratorService.GenerateInvoiceAsync(order, placeOrderModel.CorrelationId,
                        createUserResponse, cancellationToken);

                if (invoiceId == null)
                    throw new Exception("Error while generating invoice");

                // Update order with invoice id
                _logger.LogInformation("Updating order {OrderId} with invoice id {InvoiceId}", order.Id, invoiceId);

                order.SetInvoiceId(invoiceId);

                await _orderRepository.UpdateOrderAsync(order, cancellationToken);

                // Update metrics for order
                // Add order count +1
                _metrics.AddOrder();

                // Add order price = sum of all order items price
                _metrics.RecordOrderTotalPrice(order.TotalPrice);

                // Add order quantity = sum of all order items quantity
                _metrics.RecordOrderTotalQuantity(order.TotalQuantity);

                // Add order items count = sum of all order items quantity
                _metrics.AddOrderItems(order.Items.Sum(x => x.Quantity));
            }
            catch (Exception e)
            {
                // If error occured while adding order, delete user
                _logger.LogError(e, "Error while adding order, exception: {Exception}", e.Message);
                await _userService.DeleteUser(createUserResponse.UserId, cancellationToken);
            }

            stopwatch.Stop();
            _logger.LogInformation("Order placed for user {UserId} takes: {Elapsed}", createUserResponse.UserId,
                stopwatch.Elapsed);
        }
        finally
        {
            stopwatch.Stop();
        }
    }
}