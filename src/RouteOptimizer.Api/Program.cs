using RouteOptimizer.Core.Data;
using RouteOptimizer.Core.Interfaces;
using RouteOptimizer.Core.Models;
using RouteOptimizer.Core.Services;
using RouteOptimizer.Core.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Register the route settings
builder.Services.Configure<RouteSettings>(builder.Configuration.GetSection("RouteSettings"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(SampleGraphFactory.Create());
builder.Services.AddSingleton<IRouteFinder, DijkstraRouteFinder>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// app.UseHttpsRedirection();

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.MapPost("/route", (RouteRequest request, Graph graph, IRouteFinder routeFinder) =>
{
    if (string.IsNullOrWhiteSpace(request.Source) || string.IsNullOrWhiteSpace(request.Destination))
    {
        return Results.BadRequest(new { error = "Source and destination are required." });
    }

    var result = routeFinder.FindShortestRoute(graph, new Node(request.Source), new Node(request.Destination));

    return result.RouteFound
        ? Results.Ok(result)
        : Results.NotFound(new { error = "No route found." });
})
.WithName("GetRoute")
.WithOpenApi();

app.Run();