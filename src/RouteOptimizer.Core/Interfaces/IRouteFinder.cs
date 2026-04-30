using RouteOptimizer.Core.Models;

namespace RouteOptimizer.Core.Interfaces;

public interface IRouteFinder
{
    RouteResult FindShortestRoute(Graph graph, Node sourceNode, Node destinationNode);
}