namespace RouteOptimizer.Core.Configuration;

public sealed class RouteSettings
{
    public int MaxCacheSize { get; set; } = 100;
    public int CacheTtlMinutes { get; set; } = 10;
}