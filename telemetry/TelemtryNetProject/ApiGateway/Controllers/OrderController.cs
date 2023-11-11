using System.Diagnostics;
using ApiGateway.ExternalServices;
using ApiGateway.MessageBrokers;
using Microsoft.AspNetCore.Mvc;
using TelemtryNetProject.Contracts.ApiGateway.Api.v1.Requests;
using TelemtryNetProject.Contracts.ApiGateway.Api.v1.Responses;
using TelemtryNetProject.Contracts.Order.RabbitMq.v1.Requests;
using TelemtryNetProject.Contracts.ValidationService.Api.v1.Models;
using TelemtryNetProject.Contracts.ValidationService.Api.v1.Responses;

namespace ApiGateway.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly ILogger<OrderController> _logger;
    private readonly ValidationService _validationService;
    private readonly OrderMessageBroker _orderMessageBroker;

    public OrderController(ILogger<OrderController> logger, ValidationService validationService,
        OrderMessageBroker orderMessageBroker)
    {
        _logger = logger;
        _validationService = validationService;
        _orderMessageBroker = orderMessageBroker;
    }

    [HttpPost("place")]
    public async Task<IActionResult> PlaceOrder(PlaceOrderRequest placeOrderRequest,
        CancellationToken cancellationToken)
    {
        var reference = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        _logger.LogInformation("Placing order for {Reference}", reference);

        try
        {
            var tasks = new Task<ValidationResultResponse?>[2];

            tasks[0] = _validationService.IsUserValid(placeOrderRequest.UserRequest, cancellationToken);
            tasks[1] = _validationService.IsOrderValid(placeOrderRequest.OrderRequest, cancellationToken);

            var validationResultResponses = await Task.WhenAll(tasks);

            var results = validationResultResponses.SelectMany(x => x?.Results ?? new List<ValidationResult>())
                .ToList();

            if (results.Any())
            {
                _logger.LogInformation("Placing order failed, {Reference}", reference);

                var failedResponse = new PlaceOrderFailedResponse(
                    "Validation failed", results.Any(x => x.ValidationType == ValidationType.Exception));

                failedResponse.ValidationErrors = results.Where(x => x.ValidationType == ValidationType.Validation)
                    .Select(x => x.ErrorMessage).ToList();

                return new OkObjectResult(failedResponse);
            }

            _logger.LogInformation("Validation passed, placing order for {Reference}", reference);

            _orderMessageBroker.PublishMessage(new PlaceOrderModel(placeOrderRequest.UserRequest,
                placeOrderRequest.OrderRequest, reference));

            _logger.LogInformation("Order placed for {Reference}", reference);

            return Ok("Order placed");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception occured: {Message}", e.Message);
            return StatusCode(500, e);
        }
    }
}