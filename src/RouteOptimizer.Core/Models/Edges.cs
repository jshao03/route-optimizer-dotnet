namespace RouteOptimizer.Core.Models;

public sealed class Edge
{
    #region Fields
    public Node From { get; }
    public Node To { get; }
    public double Weight { get; }
    #endregion

    #region Constructors
    public Edge(Node from, Node to, double weight)
    {
        if (string.IsNullOrWhiteSpace(from.Id))
            throw new ArgumentException("From cannot be empty.", nameof(from.Id));

        if (string.IsNullOrWhiteSpace(to.Id))
            throw new ArgumentException("To cannot be empty.", nameof(to.Id));

        if (weight < 0)
            throw new ArgumentOutOfRangeException(nameof(weight), "Weight must be non-negative.");

        From = from;
        To = to;
        Weight = weight;
    }
    #endregion
}