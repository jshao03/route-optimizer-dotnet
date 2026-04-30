namespace RouteOptimizer.Core.Models;

public sealed class Graph
{
    #region Fields
    private readonly Dictionary<Node, List<Edge>> adjacency = new();

    public IReadOnlyDictionary<Node, List<Edge>> Adjacency => adjacency;
    #endregion

    #region Methods
    public void AddNode(Node node)
    {
        if (!adjacency.ContainsKey(node))
        {
            adjacency[node] = [];
        }
    }

    public void AddEdge(Node from, Node to, double weight)
    {
        AddNode(from);
        AddNode(to);

        adjacency[from].Add(new Edge(from, to, weight));
    }

    public IReadOnlyList<Edge> GetNeighbors(Node node)
    {
        return adjacency.TryGetValue(node, out var edges)
            ? edges
            : Array.Empty<Edge>();
    }

    public bool ContainsNode(Node node) => adjacency.ContainsKey(node);
    #endregion
}