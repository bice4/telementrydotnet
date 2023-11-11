using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using OrderManagement.Domain.OrderModels;
using OrderManagement.Domain.Repositories;

namespace OrderManagement.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private const string COLLECTION_NAME = "Orders";
    private readonly IConfiguration _configuration;

    public OrderRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task AddOrderAsync(Order order, CancellationToken cancellationToken)
    {
        var collection = GetCollection();
        return collection.InsertOneAsync(order, cancellationToken: cancellationToken);
    }

    public Task<Order?> GetOrderAsync(ObjectId orderId, CancellationToken cancellationToken)
    {
        var collection = GetCollection();
        return collection.Find(x => x.Id == orderId).SingleOrDefaultAsync(cancellationToken)!;
    }

    public Task<List<Order>> GetOrdersAsync(CancellationToken cancellationToken)
    {
        var collection = GetCollection();
        return collection.Find(_ => true).ToListAsync(cancellationToken)!;
    }


    private IMongoCollection<Order> GetCollection()
    {
        var connectionString = _configuration["MongoDb:ConnectionString"];
        var databaseName = _configuration["MongoDb:DatabaseName"];

        if (connectionString is null)
            throw new ArgumentNullException(nameof(connectionString));

        databaseName ??= "OrderManagement";

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        return database.GetCollection<Order>(COLLECTION_NAME);
    }
}