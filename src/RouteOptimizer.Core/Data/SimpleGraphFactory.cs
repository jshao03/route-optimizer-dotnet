using RouteOptimizer.Core.Models;

namespace RouteOptimizer.Core.Data;

public static class SampleGraphFactory
{
    public static Graph Create()
    {
        var graph = new Graph();

        graph.AddEdge("A", "B", 5);
        graph.AddEdge("A", "C", 2);
        graph.AddEdge("B", "D", 4);
        graph.AddEdge("C", "D", 7);
        graph.AddEdge("C", "E", 3);
        graph.AddEdge("E", "D", 1);

        return graph;
    }
}