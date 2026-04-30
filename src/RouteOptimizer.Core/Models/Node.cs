namespace RouteOptimizer.Core.Models;

public sealed class Node
{
    #region Fields
    public string Id { get; }
    #endregion

    #region Constructors
    public Node(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Node id cannot be empty.", nameof(id));

        Id = id;
    }
    #endregion
}