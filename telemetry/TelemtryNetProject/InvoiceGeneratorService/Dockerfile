FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 8099 8098

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["InvoiceGeneratorService/InvoiceGeneratorService.csproj", "InvoiceGeneratorService/"]
COPY ["TelemetryDotNet.Contracts/TelemetryDotNet.Contracts.csproj", "TelemetryDotNet.Contracts/"]
RUN dotnet restore "InvoiceGeneratorService/InvoiceGeneratorService.csproj"
COPY . .
WORKDIR "/src/InvoiceGeneratorService"
RUN dotnet build "InvoiceGeneratorService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InvoiceGeneratorService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InvoiceGeneratorService.dll"]
