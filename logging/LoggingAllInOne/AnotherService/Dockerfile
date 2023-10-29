FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 8041

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["AnotherService/AnotherService.csproj", "AnotherService/"]
COPY ["Contracts/Contracts.csproj", "Contracts/"]
RUN dotnet restore "AnotherService/AnotherService.csproj"
COPY . .
WORKDIR "/src/AnotherService"
RUN dotnet build "AnotherService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AnotherService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AnotherService.dll"]
