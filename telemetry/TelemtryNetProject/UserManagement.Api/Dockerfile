FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 8060

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["UserManagement.Api/UserManagement.Api.csproj", "UserManagement.Api/"]
COPY ["UserManagement.Infrastructure/UserManagement.Infrastructure.csproj", "UserManagement.Infrastructure/"]
COPY ["UserManagement.Domain/UserManagement.Domain.csproj", "UserManagement.Domain/"]
COPY ["TelemetryDotNet.Contracts/TelemetryDotNet.Contracts.csproj", "TelemetryDotNet.Contracts/"]
RUN dotnet restore "UserManagement.Api/UserManagement.Api.csproj"
COPY . .
WORKDIR "/src/UserManagement.Api"
RUN dotnet build "UserManagement.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UserManagement.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserManagement.Api.dll"]
