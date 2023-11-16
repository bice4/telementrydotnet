using MongoDB.Bson;
using OrderManagement.Domain.OrderModels;

namespace OrderManagement.Domain.Repositories;

public interface IOrderRepository
{
    Task AddOrderAsync(Order order, CancellationToken cancellationToken);
    
    Task<Order?> GetOrderAsync(ObjectId orderId, CancellationToken cancellationToken);
    
    Task<List<Order>> GetOrdersAsync(CancellationToken cancellationToken);
    
    Task UpdateOrderAsync(Order order, CancellationToken cancellationToken);
}