# Route Optimizer (.NET 8)

A .NET 8 Web API that computes the shortest route between nodes in a weighted graph using Dijkstra algorithm.

## Overview

This project was built to demonstrate backend engineering skills through a focused routing problem. It shows API design, graph modeling, algorithm implementation, custom data structures, testing, logging, and basic performance-oriented improvements.

The API accepts a source and destination node and returns the shortest weighted path between them.

## Features

- .NET 8 Web API
- weighted graph routing
- Dijkstra shortest-path algorithm
- custom min-heap priority queue
- bounded in-memory route cache
- config-driven route cache size
- structured logging
- unit tests for correctness and edge cases
- Swagger support for local API testing

## Project Structure

```text
route-optimizer-dotnet/
|-- src/
| |-- RouteOptimizer.Api/
| |-- RouteOptimizer.Core/
|-- tests/
| |-- RouteOptimizer.Tests/
|-- RouteOptimizer.sln
```

### RouteOptimizer.Api

Contains the API layer:

- endpoint definitions
- dependency injection setup
- Swagger configuration
- app configuration

### RouteOptimizer.Core

Contains the core routing logic:

- graph models
- route request/response models
- route-finding services
- custom data structures
- caching logic

### RouteOptimizer.Tests

Contains unit tests for routing behavior and edge cases.

## Architecture

The project is split into separate layers so the routing logic stays testable and independent from HTTP concerns.

- The API project handles requests, responses, dependency injection, and runtime configuration.
- The Core project contains the graph model and shortest-path logic.
- The Tests project validates correctness independently of the API.

This separation makes the algorithm easier to test and keeps the API layer thin.

## How It Works

The graph is represented as an adjacency list, where each node maps to its outgoing weighted edges.

When a route request is received:

1. the API validates the request
2. the route finder runs Dijkstra algorithm
3. the algorithm tracks the best known distance to each node
4. a custom min-heap selects the next node with the smallest tentative distance
5. once the destination is reached, the path is reconstructed and returned

## Algorithm

This project uses Dijkstrs algorithm to compute the shortest weighted path between two nodes.

Why Dijkstra:

- edge weights are non-negative
- it is a strong baseline shortest-path algorithm
- it creates room for later comparison with A

### High-level approach

- set the source distance to 0
- push the source into the min-heap
- repeatedly process the node with the smallest known distance
- relax neighbor distances when a shorter path is found
- reconstruct the final path from the previous-node map

## Data Structures

### Graph Representation

The graph uses an adjacency list for efficient neighbor lookup.

### Custom Min-Heap Priority Queue

Instead of repeatedly scanning all candidate nodes, the route finder uses a custom min-heap priority queue to retrieve the next lowest-cost node more efficiently.

### Route Cache

Repeated route queries are stored in a bounded in-memory cache.

Current cache design:

- keyed by source/destination pair
- maximum cache size is configurable through application settings
- bounded to avoid unbounded memory growth
- oldest entries are evicted when the cache reaches capacity

## Performance Considerations

This project includes a few practical performance-focused choices:

- **Adjacency list**: efficient neighbor traversal
- **Min-heap priority queue**: faster next-node selection than naive scanning
- **Early exit**: stops processing once the destination is finalized
- **Bounded in-memory cache**: avoids recomputing repeated route requests
- **Logging**: helps observe route flow and cache hits/misses

These improvements are intentionally simple and explainable rather than overengineered.

## API Endpoints

### `GET /health`

Simple health endpoint.

### `POST /route`

Returns the shortest route between two nodes.

## Example Request

```json
{
  "source": "A",
  "destination": "D"
}
```

## Example Response

```json
{
  "route": {
    "routeNodes": [
      {
        "id": "A"
      },
      {
        "id": "C"
      }
    ],
    "cost": 2
  },
  "totalCost": 2,
  "routeFound": true
}
```

## Running Locally

### Prerequisites

- .NET 8 SDK

## Configuration

Route cache behavior can be adjusted through application settings.

Example:

````json
{
  "RouteSettings": {
    "MaxCacheSize": 100
  }
}

### Run the API

```bash
dotnet run --project src/RouteOptimizer.Api
````

Then open Swagger in the browser using the local URL shown in the console, for example:

```text
http://localhost:5034/swagger
```

## Running with Docker

Build the image:

```bash
docker build -t route-optimizer-dotnet .
```

Run the container:

```bash
docker run -p 8080:8080 route-optimizer-dotnet
```

## Running Tests

```bash
dotnet test
```

## Logging

Logging is enabled to help trace request flow and routing behavior.

Example log events include:

- route requests received
- cache hits and misses
- successful route resolution
- missing node or no-route cases

## Example Test Coverage

The test suite covers cases such as:

- shortest weighted path is returned correctly
- source equals destination
- unreachable destination
- missing source node
- lower total cost is preferred over fewer hops

## Design Tradeoffs

A few deliberate tradeoffs were made:

- The graph is currently in-memory for simplicity and speed of implementation.
- The route cache is process-local and not distributed.
- The current cache eviction policy is intentionally simple.
- Dijkstra was chosen as a strong baseline before exploring A\* or larger-scale optimizations.

## Benchmark Notes

A simple benchmark was run to compare cached and uncached route lookups across three request patterns.

Benchmark setup:

- graph size: 50 nodes with additional shortcut edges
- cache size: 100 entries
- iterations per scenario: 10,000

### Results

#### Repeated identical

- uncached total time: `273.0843 ms`
- cached total time: `1.262 ms`
- approximate speedup: `216.39x`

#### Mixed hot + random

- uncached total time: `175.0282 ms`
- cached total time: `40.937 ms`
- approximate speedup: `4.28x`

#### Fully random

- uncached total time: `117.176 ms`
- cached total time: `114.0827 ms`
- approximate speedup: `1.03x`

### Interpretation

These results show that the bounded in-memory cache provides the most benefit when route lookups repeat or cluster around a smaller set of popular paths. In mixed workloads, caching still reduced repeated-query overhead meaningfully. In fully random workloads, cache reuse was low, so performance remained close to the uncached baseline.

This benchmark was intended as a practical comparison rather than a formal microbenchmark.

## Future Improvements

Possible next steps:

- cache improvement based on graph properties to choose the hot routes
- A\* search for heuristic-guided routing
- persistent graph storage
- richer observability and metrics
- configurable graph loading from external data
