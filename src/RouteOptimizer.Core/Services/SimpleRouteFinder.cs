using RouteOptimizer.Core.Interfaces;
using RouteOptimizer.Core.Models;

namespace RouteOptimizer.Core.Services;

public sealed class SimpleRouteFinder : IRouteFinder
{
    public RouteResult FindShortestRoute(Graph graph, string source, string destination)
    {
        if (!graph.ContainsNode(source) || !graph.ContainsNode(destination))
        {
            return new RouteResult
            {
                RouteFound = false
            };
        }

        if (string.Equals(source, destination, StringComparison.OrdinalIgnoreCase))
        {
            return new RouteResult
            {
                RouteFound = true,
                Path = [source],
                TotalDistance = 0
            };
        }

        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var queue = new Queue<(string Node, List<string> Path, double Distance)>();

        queue.Enqueue((source, [source], 0));
        visited.Add(source);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (string.Equals(current.Node, destination, StringComparison.OrdinalIgnoreCase))
            {
                return new RouteResult
                {
                    RouteFound = true,
                    Path = current.Path,
                    TotalDistance = current.Distance
                };
            }

            foreach (var edge in graph.GetNeighbors(current.Node))
            {
                if (visited.Contains(edge.To))
                    continue;

                var nextPath = new List<string>(current.Path) { edge.To };

                queue.Enqueue((edge.To, nextPath, current.Distance + edge.Weight));
                visited.Add(edge.To);
            }
        }

        return new RouteResult
        {
            RouteFound = false
        };
    }
}