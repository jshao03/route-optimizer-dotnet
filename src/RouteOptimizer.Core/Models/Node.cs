namespace RouteOptimizer.Core.Models;

public sealed class Node
{
    public string Id { get; }

    public Node(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Node id cannot be empty.", nameof(id));

        Id = id;
    }
}