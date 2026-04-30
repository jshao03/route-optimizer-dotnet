using RouteOptimizer.Core.Interfaces;
using RouteOptimizer.Core.Models;
using RouteOptimizer.Core.DataStructures;
using System.Collections.Concurrent;

namespace RouteOptimizer.Core.Services;

public sealed class DijkstraRouteFinder : IRouteFinder
{
    public RouteResult FindShortestRoute(Graph graph, Node sourceNode, Node destinationNode)
    {
        if (!graph.ContainsNode(sourceNode) || !graph.ContainsNode(destinationNode))
        {
            return new RouteResult
            {
                RouteFound = false
            };
        }

        if (string.Equals(sourceNode.Id, destinationNode.Id, StringComparison.OrdinalIgnoreCase))
        {
            return new RouteResult
            {
                RouteFound = true,
                Route = new Route([sourceNode], 0f),
                TotalCost = 0
            };
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
                return new RouteResult
                {
                    RouteFound = true,
                    Route = shortestPathMap[node],
                    TotalCost = priority
                };
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

        return new RouteResult
        {
            RouteFound = false
        };
    }
}