using System.Diagnostics;
using ApiGateway.ExternalServices;
using ApiGateway.MessageBrokers;
using Microsoft.AspNetCore.Mvc;
using TelemetryDotNet.Contracts.ApiGateway.Api.v1.Requests;
using TelemetryDotNet.Contracts.ApiGateway.Api.v1.Responses;
using TelemetryDotNet.Contracts.Order.Api.v1.Models;
using TelemetryDotNet.Contracts.Order.RabbitMq.v1.Requests;
using TelemetryDotNet.Contracts.UserManagement.Api.V1.Models;
using TelemetryDotNet.Contracts.ValidationService.Api.v1.Models;
using TelemetryDotNet.Contracts.ValidationService.Api.v1.Responses;

namespace ApiGateway.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly ILogger<OrderController> _logger;
    private readonly ValidationService _validationService;
    private readonly OrderMessageBroker _orderMessageBroker;

    private readonly OrderService _orderService;
    private readonly UserService _userService;

    public OrderController(ILogger<OrderController> logger, ValidationService validationService,
        OrderMessageBroker orderMessageBroker, OrderService orderService, UserService userService)
    {
        _logger = logger;
        _validationService = validationService;
        _orderMessageBroker = orderMessageBroker;
        _orderService = orderService;
        _userService = userService;
    }

    /// <summary>
    /// Place order for user
    /// </summary>
    /// <param name="placeOrderRequest">Data for order and user</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Ok</returns>
    [HttpPost("place")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(PlaceOrderFailedResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PlaceOrder(PlaceOrderRequest placeOrderRequest,
        CancellationToken cancellationToken)
    {
        var reference = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        _logger.LogInformation("Placing order for {Reference}", reference);

        try
        {
            // Validate user and order data before placing order in parallel
            var tasks = new Task<ValidationResultResponse?>[2];

            tasks[0] = _validationService.IsUserValid(placeOrderRequest.UserRequest, cancellationToken);
            tasks[1] = _validationService.IsOrderValid(placeOrderRequest.OrderRequest, cancellationToken);

            var validationResultResponses = await Task.WhenAll(tasks);

            var results = validationResultResponses.SelectMany(x => x?.Results ?? new List<ValidationResult>())
                .ToList();

            // If any validation failed, return failed response
            if (results.Any())
            {
                _logger.LogWarning("Placing order failed, {Reference}", reference);

                var failedResponse = new PlaceOrderFailedResponse(
                    "Validation failed", results.Any(x => x.ValidationType == ValidationType.Exception));

                failedResponse.ValidationErrors = results.Where(x => x.ValidationType == ValidationType.Validation)
                    .Select(x => x.ErrorMessage).ToList();

                return new OkObjectResult(failedResponse);
            }

            _logger.LogInformation("Validation passed, placing order for {Reference}", reference);

            // If validation passed, place order in message broker
            _orderMessageBroker.PublishMessage(new PlaceOrderModel(placeOrderRequest.UserRequest,
                placeOrderRequest.OrderRequest, reference));

            _logger.LogInformation("Order placed for {Reference}", reference);

            return Ok("Order placed");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while placing order: {Message}", e.Message);
            return StatusCode(500, e);
        }
    }

    /// <summary>
    /// Get full order with user data
    /// </summary>
    /// <param name="id">Id of specific order</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Order model</returns>
    [HttpGet("{id}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(FullOrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetOrderById(string id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get order by id: {Id}", id);

        OrderDto? order;

        try
        {
            order = await _orderService.GetOrderByIdAsync(id, cancellationToken);

            if (order == null)
            {
                _logger.LogInformation("Order with id {Id} not found", id);
                return NotFound();
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception occured while sending request to OrderService: exception: {Exception}", e.Message);
            return StatusCode(500, e.Message);
        }

        UserFullDto? user;
        try
        {
            user = await _userService.GetUserByIdAsync(order.UserId, cancellationToken);
            
            if (user == null)
            {
                _logger.LogInformation("User with id {Id} not found", order.UserId);
                return NotFound();
            }
            
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception occured while sending request to UserService: exception: {Exception}", e.Message);
            return StatusCode(500, e.Message);
        }
        
        return new OkObjectResult(new FullOrderResponse(user, order));
    }
}