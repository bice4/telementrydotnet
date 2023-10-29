FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 8050

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["MetricsPlayground/MetricsPlayground.csproj", "MetricsPlayground/"]
RUN dotnet restore "MetricsPlayground/MetricsPlayground.csproj"
COPY . .
WORKDIR "/src/MetricsPlayground"
RUN dotnet build "MetricsPlayground.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MetricsPlayground.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MetricsPlayground.dll"]
