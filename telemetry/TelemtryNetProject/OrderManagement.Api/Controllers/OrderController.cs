using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using OrderManagement.Domain.Repositories;
using OrderManagementApi.Translators;

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

    [HttpGet(Name = "GetAll")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        _logger.LogDebug("GetAll called");

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

    [HttpGet("{id}", Name = "GetById")]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetById called with id: {Id}", id);

        try
        {
            var order = await _orderRepository.GetOrderAsync(ObjectId.Parse(id), cancellationToken);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order.ToOrderDto());
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