FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 8070

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ValidationService/ValidationService.csproj", "ValidationService/"]
COPY ["TelemtryNetProject.Contracts/TelemtryNetProject.Contracts.csproj", "TelemtryNetProject.Contracts/"]
RUN dotnet restore "ValidationService/ValidationService.csproj"
COPY . .
WORKDIR "/src/ValidationService"
RUN dotnet build "ValidationService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ValidationService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ValidationService.dll"]
