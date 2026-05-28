namespace RouteOptimizer.Core.Models;

public sealed class CachedRouteResultDto
{
    public bool RouteFound { get; set; }
    public double TotalCost { get; set; }
    public CachedRouteDto? Route { get; set; }
}

public sealed class CachedRouteDto
{
    public List<CachedNodeDto> RouteNodes { get; set; } = [];
    public double Cost { get; set; }
}

public sealed class CachedNodeDto
{
    public string Id { get; set; } = string.Empty;
}