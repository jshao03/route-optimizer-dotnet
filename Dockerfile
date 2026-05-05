# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY RouteOptimizer.sln ./
COPY src/RouteOptimizer.Api/RouteOptimizer.Api.csproj src/RouteOptimizer.Api/
COPY src/RouteOptimizer.Core/RouteOptimizer.Core.csproj src/RouteOptimizer.Core/
COPY tests/RouteOptimizer.Tests/RouteOptimizer.Tests.csproj tests/RouteOptimizer.Tests/

RUN dotnet restore

COPY . .
RUN dotnet publish src/RouteOptimizer.Api/RouteOptimizer.Api.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "RouteOptimizer.Api.dll"]