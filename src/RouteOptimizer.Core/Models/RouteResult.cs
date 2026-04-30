namespace RouteOptimizer.Core.Models;

public sealed class RouteResult
{
    public Route Route { get; init; } = Route.Empty;
    public double TotalCost { get; init; }
    public bool RouteFound { get; init; }
}