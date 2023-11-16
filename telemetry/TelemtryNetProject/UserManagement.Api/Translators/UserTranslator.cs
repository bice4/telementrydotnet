using TelemetryDotNet.Contracts.UserManagement.Api.V1.Models;
using TelemetryDotNet.Contracts.UserManagement.Api.V1.Requests;
using UserManagement.Domain.UserModels;

namespace UserManagement.Translators;

public static class UserTranslator
{
    public static UserShortDto ToUserShortDto(this User user)
        => new(user.FullName, user.Email, user.Age, user.Gender.ToString("G"), user.Id.ToString()!, user.PhoneNumber);

    public static UserFullDto ToUserFullDto(this User user)
        => new(user.FirstName, user.LastName, user.Email, user.PhoneNumber,
            user.Age, user.Gender.ToString("G"), user.Address.ToAddressDto(), user.CreatedAt, user.UpdatedAt,
            user.Id.ToString()!);
    
    public static User ToUser(this CreateUserRequest request, string passwordHash)
    {
        var gender = (Gender)request.Gender;
        var address = request.ToAddress();

        return new User(request.FirstName, request.LastName, request.Email, passwordHash, request.Age, gender,
            address, request.PhoneNumber);
    }
}