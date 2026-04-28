namespace RouteOptimizer.Core.Models;

public sealed class RouteRequest
{
    public string Source { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
}