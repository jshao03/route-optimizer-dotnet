namespace RouteOptimizer.Core.Models;

public sealed class Route
{
    #region Fields
    public static Route Empty = new Route([], 0f);
    private readonly List<Node> routeNodes = [];
    private double cost = 0f;
    public IReadOnlyList<Node> RouteNodes => routeNodes;
    public double Cost => cost;
    #endregion

    #region Constructors
    public Route(List<Node> nodes, double cost)
    {
        this.routeNodes = nodes;
        this.cost = cost;
    }
    #endregion

    #region Methods
    public void AddEdge(Node node, double edgeCost)
    {
        this.routeNodes.Add(node);
        this.cost += edgeCost;
    }
    #endregion
}