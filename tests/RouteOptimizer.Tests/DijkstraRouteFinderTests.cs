using Microsoft.Extensions.Logging.Abstractions;
using RouteOptimizer.Core.Models;
using RouteOptimizer.Core.Services;
using Microsoft.Extensions.Options;
using RouteOptimizer.Core.Configuration;

namespace RouteOptimizer.Tests;

public sealed class DijkstraRouteFinderTests
{
    private static Graph CreateSampleGraph()
    {
        var graph = new Graph();

        var nodeA = new Node("A");
        var nodeB = new Node("B");
        var nodeC = new Node("C");
        var nodeD = new Node("D");
        var nodeE = new Node("E");

        graph.AddEdge(nodeA, nodeB, 5);
        graph.AddEdge(nodeA, nodeC, 2);
        graph.AddEdge(nodeB, nodeD, 4);
        graph.AddEdge(nodeC, nodeD, 7);
        graph.AddEdge(nodeC, nodeE, 3);
        graph.AddEdge(nodeE, nodeD, 1);

        return graph;
    }

    private static IOptions<RouteSettings> CreateRouteSettings(int maxCacheSize = 100)
    {
        var routeSettings = new RouteSettings
        {
            MaxCacheSize = maxCacheSize
        };

        return Options.Create(routeSettings);
    }

    [Fact]
    public void FindShortestRoute_ReturnsShortestWeightedPath()
    {
        var graph = CreateSampleGraph();
        var routeFinder = new DijkstraRouteFinder(NullLogger<DijkstraRouteFinder>.Instance, CreateRouteSettings());

        var result = routeFinder.FindShortestRoute(graph, new Node("A"), new Node("D"));

        Assert.True(result.RouteFound);
        Assert.Equal(6, result.TotalCost);
        Assert.Equal(["A", "C", "E", "D"], result.Route.GetNodeIds());
    }

    [Fact]
    public void FindShortestRoute_ReturnsSingleNodePath_WhenSourceEqualsDestination()
    {
        var graph = CreateSampleGraph();
        var routeFinder = new DijkstraRouteFinder(NullLogger<DijkstraRouteFinder>.Instance, CreateRouteSettings());

        var result = routeFinder.FindShortestRoute(graph, new Node("A"), new Node("A"));

        Assert.True(result.RouteFound);
        Assert.Equal(0, result.TotalCost);
        Assert.Equal(["A"], result.Route.GetNodeIds());
    }

    [Fact]
    public void FindShortestRoute_ReturnsNoRoute_WhenDestinationIsUnreachable()
    {
        var graph = CreateSampleGraph();
        graph.AddNode(new Node("Z"));

        var routeFinder = new DijkstraRouteFinder(NullLogger<DijkstraRouteFinder>.Instance, CreateRouteSettings());

        var result = routeFinder.FindShortestRoute(graph, new Node("A"), new Node("Z"));

        Assert.False(result.RouteFound);
        Assert.Empty(result.Route.GetNodeIds());
    }

    [Fact]
    public void FindShortestRoute_ReturnsNoRoute_WhenSourceNodeDoesNotExist()
    {
        var graph = CreateSampleGraph();
        var routeFinder = new DijkstraRouteFinder(NullLogger<DijkstraRouteFinder>.Instance, CreateRouteSettings());

        var result = routeFinder.FindShortestRoute(graph, new Node("X"), new Node("D"));

        Assert.False(result.RouteFound);
        Assert.Empty(result.Route.GetNodeIds());
    }

    [Fact]
    public void FindShortestRoute_PrefersLowerTotalWeight_OverFewerHops()
    {
        var graph = new Graph();
        graph.AddEdge(new Node("A"), new Node("B"), 10);
        graph.AddEdge(new Node("A"), new Node("C"), 1);
        graph.AddEdge(new Node("C"), new Node("E"), 1);
        graph.AddEdge(new Node("E"), new Node("D"), 1);
        graph.AddEdge(new Node("B"), new Node("D"), 1);

        var routeFinder = new DijkstraRouteFinder(NullLogger<DijkstraRouteFinder>.Instance, CreateRouteSettings());

        var result = routeFinder.FindShortestRoute(graph, new Node("A"), new Node("D"));

        Assert.True(result.RouteFound);
        Assert.Equal(3, result.TotalCost);
        Assert.Equal(["A", "C", "E", "D"], result.Route.GetNodeIds());
    }
}