using RouteOptimizer.Core.Models;

namespace RouteOptimizer.Core.Data;

public static class SampleGraphFactory
{
    public static Graph Create()
    {
        var graph = new Graph();

        graph.AddEdge(new Node("A"), new Node("B"), 5);
        graph.AddEdge(new Node("A"), new Node("C"), 2);
        graph.AddEdge(new Node("B"), new Node("D"), 4);
        graph.AddEdge(new Node("C"), new Node("D"), 7);
        graph.AddEdge(new Node("C"), new Node("E"), 3);
        graph.AddEdge(new Node("E"), new Node("D"), 1);

        return graph;
    }
}