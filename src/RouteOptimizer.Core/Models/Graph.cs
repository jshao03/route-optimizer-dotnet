namespace RouteOptimizer.Core.Models;

public sealed class Graph
{
    private readonly Dictionary<string, List<Edge>> _adjacency = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyDictionary<string, List<Edge>> Adjacency => _adjacency;

    public void AddNode(string nodeId)
    {
        if (!_adjacency.ContainsKey(nodeId))
        {
            _adjacency[nodeId] = new List<Edge>();
        }
    }

    public void AddEdge(string from, string to, double weight)
    {
        AddNode(from);
        AddNode(to);

        _adjacency[from].Add(new Edge(from, to, weight));
    }

    public IReadOnlyList<Edge> GetNeighbors(string nodeId)
    {
        return _adjacency.TryGetValue(nodeId, out var edges)
            ? edges
            : Array.Empty<Edge>();
    }

    public bool ContainsNode(string nodeId) => _adjacency.ContainsKey(nodeId);
}