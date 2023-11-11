using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using UserManagement.Domain.Repositories;
using UserManagement.Domain.UserModels;

namespace UserManagement.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private const string COLLECTION_NAME = "Users";
    private readonly IConfiguration _configuration;

    public UserRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<bool> IsEmailExists(string email, CancellationToken cancellationToken)
    {
        var collection = GetCollection();
        return collection.Find(x => x.Email == email).AnyAsync(cancellationToken: cancellationToken);
    }

    public Task AddUserAsync(User user, CancellationToken cancellationToken)
    {
        var collection = GetCollection();
        return collection.InsertOneAsync(user, cancellationToken: cancellationToken);
    }

    public Task<bool> UpdateUserAsync(User user, CancellationToken cancellationToken)
    {
        var collection = GetCollection();
        return collection.ReplaceOneAsync(x => x.Id == user.Id, user, cancellationToken: cancellationToken)
            .ContinueWith(x => x.Result.IsAcknowledged && x.Result.ModifiedCount > 0, cancellationToken);
    }

    public Task<bool> DeleteUserAsync(ObjectId id, CancellationToken cancellationToken)
    {
        var collection = GetCollection();
        return collection.DeleteOneAsync(x => x.Id == id, cancellationToken: cancellationToken)
            .ContinueWith(x => x.Result.IsAcknowledged && x.Result.DeletedCount > 0, cancellationToken);
    }

    public Task<User?> GetUserAsync(ObjectId id, CancellationToken cancellationToken)
    {
        var collection = GetCollection();
        return collection.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken: cancellationToken)!;
    }

    public Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var collection = GetCollection();
        return collection.Find(x => x.Email == email).FirstOrDefaultAsync(cancellationToken: cancellationToken)!;
    }

    public Task<List<User>> GetUsersAsync(CancellationToken cancellationToken)
    {
        var collection = GetCollection();
        return collection.Find(x => true).ToListAsync(cancellationToken: cancellationToken);
    }

    private IMongoCollection<User> GetCollection()
    {
        var connectionString = _configuration["MongoDb:ConnectionString"];
        var databaseName = _configuration["MongoDb:DatabaseName"];

        if (connectionString is null)
            throw new ArgumentNullException(nameof(connectionString));

        databaseName ??= "UserManagement";

        var client = new MongoClient(connectionString);

        var database = client.GetDatabase(databaseName);

        return database.GetCollection<User>(COLLECTION_NAME);
    }
}