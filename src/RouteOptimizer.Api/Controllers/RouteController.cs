using Microsoft.AspNetCore.Mvc;
using RouteOptimizer.Core.Interfaces;
using RouteOptimizer.Core.Models;

namespace RouteOptimizer.Api.Controllers;

[ApiController]
[Route("api/route")]
public sealed class RouteController : ControllerBase
{
    private readonly Graph graph;
    private readonly IRouteFinder routeFinder;
    private readonly ILogger<RouteController> logger;

    public RouteController(
        Graph graph,
        IRouteFinder routeFinder,
        ILogger<RouteController> logger)
    {
        this.graph = graph;
        this.routeFinder = routeFinder;
        this.logger = logger;
    }

    [HttpPost]
    public IActionResult FindRoute([FromBody] RouteRequest request)
    {
        if (request is null ||
            string.IsNullOrWhiteSpace(request.Source) ||
            string.IsNullOrWhiteSpace(request.Destination))
        {
            return BadRequest(new { error = "Source and destination are required." });
        }

        logger.LogInformation(
            "Received route request from {Source} to {Destination}",
            request.Source,
            request.Destination);

        var result = routeFinder.FindShortestRoute(
            graph,
            new Node(request.Source),
            new Node(request.Destination));

        if (!result.RouteFound)
        {
            return NotFound(new { error = "No route found." });
        }

        return Ok(result);
    }

    [HttpGet("{source}/{destination}")]
    public IActionResult GetRoute(string source, string destination)
    {
        if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(destination))
        {
            return BadRequest(new { error = "Source and destination are required." });
        }

        logger.LogInformation(
            "Received route request from {Source} to {Destination}",
            source,
            destination);

        var result = routeFinder.FindShortestRoute(
            graph,
            new Node(source),
            new Node(destination));

        if (!result.RouteFound)
        {
            return NotFound(new { error = "No route found." });
        }

        return Ok(result);
    }
}