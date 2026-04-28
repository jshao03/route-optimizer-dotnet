namespace RouteOptimizer.Core.Models;

public sealed class RouteResult
{
    public List<string> Path { get; init; } = [];
    public double TotalDistance { get; init; }
    public bool RouteFound { get; init; }
}