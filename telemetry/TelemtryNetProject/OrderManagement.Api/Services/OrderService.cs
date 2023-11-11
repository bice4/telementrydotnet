using System.Diagnostics;
using System.Transactions;
using OrderManagement.Domain.Repositories;
using OrderManagementApi.ExternalServices;
using OrderManagementApi.Metrics;
using OrderManagementApi.Translators;
using TelemtryNetProject.Contracts.Order.RabbitMq.v1.Requests;

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

    public async Task PlaceOrder(PlaceOrderModel placeOrderModel, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Placing order for user {PlaceOrderModel}", placeOrderModel);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var userId = await _userService.CreateUser(placeOrderModel.User, cancellationToken: cancellationToken);

            try
            {
                var order = placeOrderModel.Order.ToOrder(userId);

                await _orderRepository.AddOrderAsync(order, cancellationToken);
                _metrics.AddOrder();
                _metrics.AddOrderItems(order.Items.Sum(x => x.Quantity));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while adding order, deleting user, exception: {Exception}", e.Message);
                await _userService.DeleteUser(userId, cancellationToken);
            }


            stopwatch.Stop();
            _logger.LogInformation("Order placed for user {UserId} takes: {Elapsed}", userId, stopwatch.Elapsed);
        }
        finally
        {
            stopwatch.Stop();
        }
    }
}