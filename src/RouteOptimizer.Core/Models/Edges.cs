namespace RouteOptimizer.Core.Models;

public sealed class Edge
{
    #region Fields
    public string From { get; }
    public string To { get; }
    public double Weight { get; }
    #endregion

    #region Constructors
    public Edge(string from, string to, double weight)
    {
        if (string.IsNullOrWhiteSpace(from))
            throw new ArgumentException("From cannot be empty.", nameof(from));

        if (string.IsNullOrWhiteSpace(to))
            throw new ArgumentException("To cannot be empty.", nameof(to));

        if (weight < 0)
            throw new ArgumentOutOfRangeException(nameof(weight), "Weight must be non-negative.");

        From = from;
        To = to;
        Weight = weight;
    }
    #endregion
}