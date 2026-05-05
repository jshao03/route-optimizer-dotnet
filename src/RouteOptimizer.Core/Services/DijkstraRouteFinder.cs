using RouteOptimizer.Core.Interfaces;
using RouteOptimizer.Core.Models;
using RouteOptimizer.Core.DataStructures;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
namespace RouteOptimizer.Core.Services;

public sealed class DijkstraRouteFinder : IRouteFinder
{
    #region Fields
    private readonly ILogger<DijkstraRouteFinder> logger;
    private const int MaxCacheSize = 100;

    private readonly ConcurrentDictionary<string, RouteResult> cache = new(StringComparer.OrdinalIgnoreCase);
    private readonly Queue<string> cacheOrder = new();
    private readonly object cacheLock = new();
    #endregion

    #region egion Constructor
    public DijkstraRouteFinder(ILogger<DijkstraRouteFinder> logger)
    {
        this.logger = logger;
    }
    #endregion

    #region Methods
    public RouteResult FindShortestRoute(Graph graph, Node sourceNode, Node destinationNode)
    {
        logger.LogInformation("Finding shortest route from {Source} to {Destination}", sourceNode.Id, destinationNode.Id);
        var cacheKey = $"{sourceNode.Id}:{destinationNode.Id}";

        if (cache.TryGetValue(cacheKey, out var cachedResult))
        {
            logger.LogInformation("Cache hit for route {Source} -> {Destination}", sourceNode.Id, destinationNode.Id);
            return cachedResult;
        }

        if (!graph.ContainsNode(sourceNode) || !graph.ContainsNode(destinationNode))
        {
            logger.LogWarning("Source or destination node not found. Source: {Source}, Destination: {Destination}", sourceNode.Id, destinationNode.Id);
            var result = new RouteResult
            {
                RouteFound = false
            };
            cache[cacheKey] = result;

            return result;
        }

        if (string.Equals(sourceNode.Id, destinationNode.Id, StringComparison.OrdinalIgnoreCase))
        {
            logger.LogInformation("Source and destination are the same node: {Node}", sourceNode.Id);

            var result = new RouteResult
            {
                RouteFound = true,
                Route = new Route([sourceNode], 0f),
                TotalCost = 0
            };
            cache[cacheKey] = result;

            return result;
        }

        var visited = new HashSet<Node>();
        var queue = new MinHeapPriorityQueue();
        var shortestPathMap = new Dictionary<Node, Route>();

        queue.Enqueue(sourceNode, 0);
        shortestPathMap[sourceNode] = new Route([sourceNode], 0);
        visited.Add(sourceNode);

        while (queue.Count > 0)
        {
            var (node, priority) = queue.Dequeue();
            var currentRoute = shortestPathMap[node];

            if (priority > currentRoute.Cost)
            {
                continue;
            }

            if (node == destinationNode)
            {
                var result = new RouteResult
                {
                    RouteFound = true,
                    Route = shortestPathMap[node],
                    TotalCost = priority
                };
                AddToCache(cacheKey, result);
                logger.LogInformation("Route found from {Source} to {Destination} with total distance {Distance}", sourceNode.Id, destinationNode.Id, result.TotalCost);

                return result;
            }

            var neighbors = graph.GetNeighbors(node);
            foreach (var edge in neighbors)
            {
                if (visited.Contains(edge.To))
                {
                    continue;
                }

                var updatedCost = currentRoute.Cost + edge.Weight;
                var updatedPath = new Route([.. currentRoute.RouteNodes, edge.To], currentRoute.Cost + edge.Weight);

                if (shortestPathMap.TryGetValue(edge.To, out var route))
                {
                    if (route.Cost > updatedCost)
                    {
                        shortestPathMap[edge.To] = updatedPath;
                        queue.Enqueue(edge.To, updatedCost);
                    }
                }
                else
                {
                    shortestPathMap.Add(edge.To, updatedPath);
                    queue.Enqueue(edge.To, updatedCost);
                }
            }

            visited.Add(node);
        }

        var failedResult = new RouteResult
        {
            RouteFound = false
        };
        AddToCache(cacheKey, failedResult);
        logger.LogInformation("Route not exist from {Source} to {Destination}", sourceNode.Id, destinationNode.Id);

        return failedResult;
    }

    /// <summary>
    /// add RouteResult to the cache
    /// remove the oldest entry if cache size exceeds the limit
    /// </summary>
    /// <param name="cacheKey"></param>
    /// <param name="result"></param>
    private void AddToCache(string cacheKey, RouteResult result)
    {
        lock (cacheLock)
        {
            if (cache.ContainsKey(cacheKey))
            {
                return;
            }

            if (cache.Count >= MaxCacheSize)
            {
                var oldestKey = cacheOrder.Dequeue();
                cache.TryRemove(oldestKey, out _);
            }

            cache[cacheKey] = result;
            cacheOrder.Enqueue(cacheKey);
        }
    }
    #endregion
}