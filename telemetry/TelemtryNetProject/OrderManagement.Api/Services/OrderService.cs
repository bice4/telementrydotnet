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

    public OrderService(IOrderRepository orderRepository, ILogger<OrderService> logger, OrderMetrics metrics,
        UserService userService)
    {
        _orderRepository = orderRepository;
        _logger = logger;
        _metrics = metrics;
        _userService = userService;
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
                _logger.LogError(e, "Error while adding order, deleting user, exception: {Exception}", e.Message);
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