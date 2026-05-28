using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RouteOptimizer.Core.Configuration;
using RouteOptimizer.Core.Interfaces;
using RouteOptimizer.Core.Models;

namespace RouteOptimizer.Core.Services;

public sealed class RedisCachedRouteFinder : IRouteFinder
{
    private readonly Graph graph;
    private readonly DijkstraRouteFinder inner;
    private readonly IDistributedCache cache;
    private readonly ILogger<RedisCachedRouteFinder> logger;
    private readonly RouteSettings settings;

    public RedisCachedRouteFinder(
        Graph graph,
        DijkstraRouteFinder inner,
        IDistributedCache cache,
        IOptions<RouteSettings> settings,
        ILogger<RedisCachedRouteFinder> logger)
    {
        this.graph = graph;
        this.inner = inner;
        this.cache = cache;
        this.logger = logger;
        this.settings = settings.Value;
    }

    public RouteResult FindShortestRoute(Graph graph, Node source, Node destination)
    {
        var cacheKey = BuildCacheKey(source, destination);

        var cachedJson = cache.GetString(cacheKey);

        if (!string.IsNullOrWhiteSpace(cachedJson))
        {
            logger.LogInformation(
                "Redis cache hit for {Source} -> {Destination}",
                source.Id,
                destination.Id);

            var cachedDto = JsonSerializer.Deserialize<CachedRouteResultDto>(cachedJson);
            if (cachedDto is not null)
            {
                return FromDto(cachedDto);
            }
        }

        logger.LogInformation(
            "Redis cache miss for {Source} -> {Destination}",
            source.Id,
            destination.Id);

        var result = inner.FindShortestRoute(graph, source, destination);

        if (result.RouteFound)
        {
            var dto = ToDto(result);
            var json = JsonSerializer.Serialize(dto);

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow =
                    TimeSpan.FromMinutes(settings.CacheTtlMinutes)
            };

            cache.SetString(cacheKey, json, options);
        }

        return result;
    }

    private static CachedRouteResultDto ToDto(RouteResult result)
    {
        return new CachedRouteResultDto
        {
            RouteFound = result.RouteFound,
            TotalCost = result.TotalCost,
            Route = result.Route is null
                ? null
                : new CachedRouteDto
                {
                    Cost = result.Route.Cost,
                    RouteNodes = result.Route.RouteNodes
                        .Select(node => new CachedNodeDto { Id = node.Id })
                        .ToList()
                }
        };
    }

    private static RouteResult FromDto(CachedRouteResultDto dto)
    {
        return new RouteResult
        {
            RouteFound = dto.RouteFound,
            TotalCost = dto.TotalCost,
            Route = dto.Route is null
                ? null
                : new Route(
                    dto.Route.RouteNodes.Select(node => new Node(node.Id)).ToList(),
                    dto.Route.Cost)
        };
    }

    private static string BuildCacheKey(Node source, Node destination)
        => $"route:{source.Id}:{destination.Id}";
}