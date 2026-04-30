namespace RouteOptimizer.Core.Models;

public sealed class Graph
{
    #region Fields
    private readonly Dictionary<string, List<Edge>> adjacency = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyDictionary<string, List<Edge>> Adjacency => adjacency;
    #endregion

    #region Methods
    public void AddNode(string nodeId)
    {
        if (!adjacency.ContainsKey(nodeId))
        {
            adjacency[nodeId] = new List<Edge>();
        }
    }

    public void AddEdge(string from, string to, double weight)
    {
        AddNode(from);
        AddNode(to);

        adjacency[from].Add(new Edge(from, to, weight));
    }

    public IReadOnlyList<Edge> GetNeighbors(string nodeId)
    {
        return adjacency.TryGetValue(nodeId, out var edges)
            ? edges
            : Array.Empty<Edge>();
    }

    public bool ContainsNode(string nodeId) => adjacency.ContainsKey(nodeId);
    #endregion
}