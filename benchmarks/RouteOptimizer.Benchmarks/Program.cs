using System.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using RouteOptimizer.Core.Configuration;
using RouteOptimizer.Core.Models;
using RouteOptimizer.Core.Services;

const int iterations = 10000;
var random = new Random(42);

var settings = Options.Create(new RouteSettings
{
    MaxCacheSize = 100
});

var (graph, nodes) = CreateBenchmarkGraph();

var repeatedRequests = Enumerable.Repeat((nodes[0], nodes[39]), iterations).ToList();
var mixedRequests = GenerateMixedRequests(nodes, iterations, random);
var randomRequests = GenerateRandomRequests(nodes, iterations, random);

RunScenario("Repeated identical", graph, repeatedRequests, settings);
RunScenario("Mixed hot + random", graph, mixedRequests, settings);
RunScenario("Fully random", graph, randomRequests, settings);

static void RunScenario(
    string name,
    Graph graph,
    List<(Node Source, Node Destination)> requests,
    IOptions<RouteSettings> settings)
{
    Console.WriteLine(name);

    var uncached = MeasureUncached(graph, requests, settings);
    var cached = MeasureCached(graph, requests, settings);

    Console.WriteLine($"Uncached total time: {uncached.TotalMilliseconds} ms");
    Console.WriteLine($"Cached total time:   {cached.TotalMilliseconds} ms");

    if (cached.TotalMilliseconds > 0)
    {
        var speedup = uncached.TotalMilliseconds / cached.TotalMilliseconds;
        Console.WriteLine($"Approximate speedup: {speedup:F2}x");
    }

    Console.WriteLine();
}

static TimeSpan MeasureUncached(
    Graph graph,
    List<(Node Source, Node Destination)> requests,
    IOptions<RouteSettings> settings)
{
    var stopwatch = Stopwatch.StartNew();

    foreach (var (source, destination) in requests)
    {
        var routeFinder = new DijkstraRouteFinder(
            NullLogger<DijkstraRouteFinder>.Instance,
            settings);

        _ = routeFinder.FindShortestRoute(graph, source, destination);
    }

    stopwatch.Stop();
    return stopwatch.Elapsed;
}

static TimeSpan MeasureCached(
    Graph graph,
    List<(Node Source, Node Destination)> requests,
    IOptions<RouteSettings> settings)
{
    var routeFinder = new DijkstraRouteFinder(
        NullLogger<DijkstraRouteFinder>.Instance,
        settings);

    var stopwatch = Stopwatch.StartNew();

    foreach (var (source, destination) in requests)
    {
        _ = routeFinder.FindShortestRoute(graph, source, destination);
    }

    stopwatch.Stop();
    return stopwatch.Elapsed;
}

static List<(Node Source, Node Destination)> GenerateRandomRequests(
    List<Node> nodes,
    int count,
    Random random)
{
    var requests = new List<(Node Source, Node Destination)>(count);

    for (var i = 0; i < count; i++)
    {
        var source = nodes[random.Next(nodes.Count)];
        var destination = nodes[random.Next(nodes.Count)];

        while (destination == source)
        {
            destination = nodes[random.Next(nodes.Count)];
        }

        requests.Add((source, destination));
    }

    return requests;
}

static List<(Node Source, Node Destination)> GenerateMixedRequests(
    List<Node> nodes,
    int count,
    Random random)
{
    var requests = new List<(Node Source, Node Destination)>(count);

    var hotRoutes = new List<(Node Source, Node Destination)>
    {
        (nodes[0], nodes[39]),
        (nodes[1], nodes[24]),
        (nodes[4], nodes[49]),
        (nodes[9], nodes[39]),
        (nodes[14], nodes[44]),
        (nodes[2], nodes[17]),
        (nodes[7], nodes[34]),
        (nodes[11], nodes[29]),
        (nodes[20], nodes[49]),
        (nodes[5], nodes[25])
    };

    for (var i = 0; i < count; i++)
    {
        var useHotRoute = random.NextDouble() < 0.7;

        if (useHotRoute)
        {
            requests.Add(hotRoutes[random.Next(hotRoutes.Count)]);
        }
        else
        {
            var source = nodes[random.Next(nodes.Count)];
            var destination = nodes[random.Next(nodes.Count)];

            while (destination == source)
            {
                destination = nodes[random.Next(nodes.Count)];
            }

            requests.Add((source, destination));
        }
    }

    return requests;
}

static (Graph Graph, List<Node> Nodes) CreateBenchmarkGraph()
{
    var graph = new Graph();

    var nodes = Enumerable.Range(1, 50)
        .Select(i => new Node($"N{i}"))
        .ToList();

    foreach (var node in nodes)
    {
        graph.AddNode(node);
    }

    for (var i = 0; i < nodes.Count - 1; i++)
    {
        graph.AddEdge(nodes[i], nodes[i + 1], 1);
    }

    graph.AddEdge(nodes[0], nodes[4], 3);
    graph.AddEdge(nodes[0], nodes[9], 8);
    graph.AddEdge(nodes[1], nodes[7], 4);
    graph.AddEdge(nodes[2], nodes[11], 6);
    graph.AddEdge(nodes[4], nodes[14], 7);
    graph.AddEdge(nodes[7], nodes[19], 10);
    graph.AddEdge(nodes[9], nodes[24], 12);
    graph.AddEdge(nodes[11], nodes[17], 2);
    graph.AddEdge(nodes[14], nodes[21], 4);
    graph.AddEdge(nodes[17], nodes[29], 5);
    graph.AddEdge(nodes[19], nodes[34], 8);
    graph.AddEdge(nodes[21], nodes[27], 2);
    graph.AddEdge(nodes[24], nodes[39], 6);
    graph.AddEdge(nodes[27], nodes[39], 3);
    graph.AddEdge(nodes[29], nodes[44], 4);
    graph.AddEdge(nodes[34], nodes[49], 7);
    graph.AddEdge(nodes[39], nodes[49], 2);

    return (graph, nodes);
}