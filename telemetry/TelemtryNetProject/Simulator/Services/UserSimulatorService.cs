using System.Diagnostics;
using Bogus;
using Simulator.ExternalService;
using Simulator.Models;
using TelemetryDotNet.Contracts.UserManagement.Api.V1.Requests;

namespace Simulator.Services;

public class UserSimulatorService
{
    private readonly ILogger<UserSimulatorService> _logger;
    private readonly ApiGatewayHttpClient _apiGatewayHttpClient;

    private static readonly string[] Countries = {
        "USA", "Ukraine", "Canada", "Poland", "Germany", "Netherlands", "Portugal",
        "United Kingdom",
        "France",
        "Japan",
        "Australia",
        "Brazil"
    };

    public UserSimulatorService(ILogger<UserSimulatorService> logger, ApiGatewayHttpClient apiGatewayHttpClient)
    {
        _logger = logger;
        _apiGatewayHttpClient = apiGatewayHttpClient;
    }

    public async Task CreateUsers(SimulateCreateUsersRequest request, CancellationToken cancellationToken)
    {
        var st = Stopwatch.StartNew();

        _logger.LogInformation("Starting to simulate create users");

        Randomizer.Seed = new Random(DateTime.UtcNow.Ticks.GetHashCode());

        var users = new Faker<CreateUserRequest>()
            .RuleFor(x => x.Age, f => f.Random.Number(18, 50))
            .RuleFor(x => x.City, f => f.Address.City())
            .RuleFor(x => x.Email, f => f.Person.Email)
            .RuleFor(x => x.FirstName, f => f.Person.FirstName)
            .RuleFor(x => x.LastName, f => f.Person.LastName)
            .RuleFor(x => x.PhoneNumber, f => f.Person.Phone)
            .RuleFor(x => x.Street, f => f.Address.StreetName())
            .RuleFor(x => x.ZipCode, f => f.Address.ZipCode())
            .RuleFor(x => x.Country, GetRandomCountry)
            .RuleFor(x => x.Password, f => f.Internet.Password())
            .RuleFor(x => x.Gender, f => f.Random.Number(0, 2))
            .RuleFor(x => x.ApartmentNumber, f => f.Random.Number(1, 100).ToString())
            .RuleFor(x => x.BuildingNumber, f => f.Random.Number(1, 100).ToString())
            .RuleFor(x => x.Floor, f => f.Random.Number(1, 24).ToString())
            .Generate(request.UserCount);

        // Add error rate
        if (request.ErrorRate > 0)
        {
            var errorCount = (int)(request.UserCount * request.ErrorRate);
            for (var i = 0; i < errorCount; i++)
            {
                SimulateError(users[i]);
            }
        }


        if (request.InParallel.HasValue && request.InParallel.Value)
        {
            async void Action(CreateUserRequest user)
            {
                try
                {
                    var res = await Task.Run(() => _apiGatewayHttpClient.CreateUser(user, cancellationToken),
                        cancellationToken);

                    if (request.DelayInSec.HasValue)
                        await Task.Delay(TimeSpan.FromSeconds(request.DelayInSec.Value), cancellationToken);

                    if (res.error != null)
                    {
                        _logger.LogWarning("User creation failed: {User}\n Error result: {@Error}", user, res.error);
                    }
                    else
                    {
                        _logger.LogInformation("Created UserId: {UserId}", res.success?.UserId);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError("Error while simulating create users: {Exception}", e);
                }
            }

            Parallel.ForEach(users, Action);
        }
        else
        {
            foreach (var user in users)
            {
                try
                {
                    var res = await _apiGatewayHttpClient.CreateUser(user, cancellationToken);

                    if (request.DelayInSec.HasValue)
                        await Task.Delay(TimeSpan.FromSeconds(request.DelayInSec.Value), cancellationToken);

                    if (res.error != null)
                    {
                        _logger.LogWarning("User creation failed: {User}\n Error result: {@Error}", user, res.error);
                    }
                    else
                    {
                        _logger.LogInformation("Created UserId: {UserId}", res.success?.UserId);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError("Error while simulating create users: {Exception}", e);
                }
            }
        }

        st.Stop();

        _logger.LogInformation("Finished simulating create users in {Elapsed} ms", st.ElapsedMilliseconds);
    }

    private static string GetRandomCountry() => Countries[Random.Shared.Next(0, Countries.Length)];

    private static void SimulateError(CreateUserRequest user)
    {
        var val = new Random().Next(0, 4);

        switch (val)
        {
            case 0:
                user.Email = "invalid-email";
                break;
            case 1:
                user.FirstName = null!;
                break;
            case 2:
                user.Country = "Russia";
                break;
            case 3:
                user.FirstName = "Vladimir";
                user.LastName = "Putin";
                break;
        }
    }
}