using OrderManagement.Domain.OrderModels;
using TelemetryDotNet.Contracts.Order.Api.v1.Models;
using ContractOrderItem = TelemetryDotNet.Contracts.Order.Api.v1.Models.OrderItem;
using ContractCreateOrderRequest = TelemetryDotNet.Contracts.Order.Api.v1.Request.CreateOrderRequest;
using OrderItem = OrderManagement.Domain.OrderModels.OrderItem;

namespace OrderManagementApi.Translators;

public static class OrderTranslator
{
    private static OrderItem ToOrderItem(this ContractOrderItem orderItemModel)
        => new(orderItemModel.Id, orderItemModel.Name, orderItemModel.Quantity, orderItemModel.Price);

    public static Order ToOrder(this ContractCreateOrderRequest orderModel,
        string userId)
        => new(userId, orderModel.OrderItems.Select(ToOrderItem).ToList());

    private static OrderItemDto ToOrderItemDto(this OrderItem orderItem)
        => new(orderItem.Id.ToString()!, orderItem.ReferenceId, orderItem.Name, orderItem.Quantity);

    public static OrderDto ToOrderDto(this Order order)
        => new(order.Id.ToString()!, order.CreatedAt, order.UpdatedAt, order.UserId,
            order.Items.Select(ToOrderItemDto).ToList());
}