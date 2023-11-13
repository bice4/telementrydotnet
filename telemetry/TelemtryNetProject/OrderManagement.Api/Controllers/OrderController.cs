using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using OrderManagement.Domain.Repositories;
using OrderManagementApi.Translators;
using TelemetryDotNet.Contracts.Order.Api.v1.Models;

namespace OrderManagementApi.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<OrderController> _logger;

    public OrderController(IOrderRepository orderRepository, ILogger<OrderController> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    /// <summary>
    /// Get all orders
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Collection of all orders</returns>
    [HttpGet(Name = "GetAll")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Get all orders");

        try
        {
            var orders = await _orderRepository.GetOrdersAsync(cancellationToken);

            return Ok(orders.Select(x => x.ToOrderDto()));
        }
        catch (Exception e)
        {
            return LogExceptionAndReturnError(e);
        }
    }

    /// <summary>
    /// Get order by id
    /// </summary>
    /// <param name="id">Id of specific order</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Order with provided id</returns>
    [HttpGet("{id}", Name = "GetById")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get order by id: {Id}", id);

        try
        {
            var order = await _orderRepository.GetOrderAsync(ObjectId.Parse(id), cancellationToken);

            return order == null
                ? NotFound()
                : Ok(order.ToOrderDto());
        }
        catch (Exception e)
        {
            return LogExceptionAndReturnError(e);
        }
    }

    private IActionResult LogExceptionAndReturnError(Exception e)
    {
        _logger.LogError(e, "Exception occured while processing request: exception: {Exception}", e.Message);
        return StatusCode(500, e.Message);
    }
}