using TelemtryNetProject.Contracts.UserManagement.Api.V1.Models;
using TelemtryNetProject.Contracts.UserManagement.Api.V1.Requests;
using UserManagement.Domain.UserModels;

namespace UserManagement.Translators;

public static class AddressTranslator
{
    public static AddressDto ToAddressDto(this Address address)
        => new(address.Country, address.City, address.Street, address.ZipCode, address.ApartmentNumber,
            address.BuildingNumber, address.Floor);

    public static Address ToAddress(this CreateUserRequest request)
        => new(request.Country, request.City, request.Street, request.ZipCode, request.ApartmentNumber,
            request.BuildingNumber, request.Floor);

    public static Address ToAddress(this UpdateUserAddressRequest request)
        => new(request.Country, request.City, request.Street, request.ZipCode, request.ApartmentNumber,
            request.BuildingNumber, request.Floor);

}