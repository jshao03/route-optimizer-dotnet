using RouteOptimizer.Core.Data;
using RouteOptimizer.Core.Interfaces;
using RouteOptimizer.Core.Models;
using RouteOptimizer.Core.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(SampleGraphFactory.Create());
builder.Services.AddScoped<IRouteFinder, DijkstraRouteFinder>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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