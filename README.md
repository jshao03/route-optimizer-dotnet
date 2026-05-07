# Route Optimizer (.NET 8 + React)

A .NET 8 Web API that computes the shortest route between nodes in a weighted graph using Dijkstra algorithm, with a lightweight React + TypeScript frontend to visualize the result.

## Overview

This project was built to demonstrate backend engineering skills through a focused routing problem. It includes API design, graph modeling, algorithm implementation, custom data structures, testing, logging, bounded caching, containerization, benchmarking, and a small frontend demo layer.

The backend accepts a source and destination node and returns the shortest weighted path between them. The frontend visualizes the graph and highlights the returned route.

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
- React + TypeScript frontend graph visualizer
- Docker Compose support for running frontend and backend together
- benchmark project for comparing cache effectiveness under different workloads

## Project Structure

```text
route-optimizer-dotnet/
|-- src/
| |-- RouteOptimizer.Api/
| |-- RouteOptimizer.Core/
|-- tests/
| |-- RouteOptimizer.Tests/
|-- benchmarks/
  |-- RouteOptimizer.BenchMarks/
|-- frontend
|  |-- src/
|  |   |-- components/
|  |   |-- data/
|  |   |-- services/
|  |   |-- types/
|  |--Dockerfile
|-- Dockerfile
|-- docker-compose.yml
|-- RouteOptimizer.sln
```

### RouteOptimizer.Api

Contains the API layer:

- endpoint definitions
- dependency injection setup
- Swagger configuration
- app configuration
- CORS configuration for the frontend

### RouteOptimizer.Core

Contains the core routing logic:

- graph models
- route request/response models
- route-finding services
- custom data structures
- caching logic
- route settings

### RouteOptimizer.Tests

Contains unit tests for routing behavior and edge cases.

### RouteOptimizer.Benchmarks

Contains a simple benchmark project for comparing cached and uncached route lookups under different workloads.

### frontend

Contains the React + TypeScript visualization layer:

- source/destination inputs
- API calls to the backend
- SVG graph rendering
- highlighted route display
- request timing display

## Architecture

The project is split into separate layers so the routing logic stays testable and independent from HTTP concerns.

- The API project handles requests, responses, dependency injection, runtime configuration, and CORS.
- The Core project contains the graph model, shortest-path logic, cache behavior, and data structures.
- The Tests project validates correctness independently of the API.
- The frontend project acts as a thin demo layer that visualizes the route returned by the API.

This separation makes the algorithm easier to test and keeps the API layer thin.

## How It Works

The graph is represented as an adjacency list, where each node maps to its outgoing weighted edges.

When a route request is received:

1. the API validates the request
2. the route finder runs Dijkstra algorithm
3. the algorithm tracks the best known distance to each node
4. a custom min-heap selects the next node with the smallest tentative distance
5. when a shorter path is found, distances and previous-node references are updated
6. once the destination is reached, the path is reconstructed and returned
7. the frontend can render the returned route visually

## Algorithm

This project uses Dijkstrs algorithm to compute the shortest weighted path between two nodes.

Why Dijkstra:

- edge weights are non-negative
- it is a strong baseline shortest-path algorithm
- it creates room for later comparison with A\*

### High-level approach

- set the source distance to 0 and enque source node into the min-heap
- repeatedly process the node with the smallest known distance
- expand based on adjacent list of the dequeued node
- update shortest path map when a shorter distance to the node encoutered
- target reached when target node is dequeued

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
- **Configurable cache size**: allows runtime tuning without code changes
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
    "routeNodes": [{ "id": "A" }, { "id": "C" }, { "id": "E" }, { "id": "D" }],
    "cost": 6
  },
  "totalCost": 6,
  "routeFound": true
}
```

## Running Locally

### Prerequisites

- .NET 8 SDK
- Node.js
- npm

## Configuration

Route cache behavior can be adjusted through application settings.

Example:

```json
{
  "RouteSettings": {
    "MaxCacheSize": 100
  }
}
```

## Running Locally

### Prerequisites

- .NET 8 SDK
- Node.js
- npm
- Docker Desktop (optional, for containerized runs)

### Run the API

```bash
dotnet run --project src/RouteOptimizer.Api
```

Then open Swagger in the browser using the local URL shown in the console, for example:

```text
http://localhost:5034/swagger
```

### Run the Frontend

From the `frontend` directory:

```bash
npm install
npm run dev
```

Then open:

```text
http://localhost:5173
```

The frontend expects the backend API to be running locally and uses CORS to call it.

## Running Tests

```bash
dotnet test
```

## Running Benchmarks

```bash
dotnet run --project benchmarks/RouteOptimizer.Benchmarks
```

## Running with Docker

### Backend only

Build the image:

```bash
docker build -t route-optimizer-dotnet .
```

Run the container:

```bash
docker run -p 8080:8080 route-optimizer-dotnet
```

Then open:

- `http://localhost:8080/health`
- `http://localhost:8080/swagger`

### Frontend + backend together with Docker Compose

From the repo root:

```bash
docker compose up --build
```

Then open:

- frontend: `http://localhost:3000`
- backend Swagger: `http://localhost:8080/swagger`
- backend health endpoint: `http://localhost:8080/health`

Stop the stack with:

```bash
docker compose down
```

## How the Docker Setup Works

The project uses two containers:

- **api**: the ASP.NET Core backend
- **frontend**: the built React app served by nginx

### Backend container

The backend image:

- restores and publishes the API project
- runs the published app in an ASP.NET runtime image
- listens on port `8080` inside the container

The Docker Compose mapping:

```text
8080:8080
```

means:

- host port `8080` on your machine
- forwards to container port `8080`

### Frontend container

The frontend image:

- installs Node dependencies
- builds the Vite app
- copies the built files into nginx
- serves the built frontend on port `80` inside the container

The Docker Compose mapping:

```text
3000:80
```

means:

- host port `3000` on your machine
- forwards to container port `80`

### Important note about browser execution

Even though the frontend is served from the frontend container, the React application logic runs in the **browser**, not inside the container after the files are loaded.

That means:

- nginx serves the built frontend files
- your browser downloads the JavaScript bundle
- React runs in the browser
- the browser then calls the backend API

Because of that, the frontend uses the host-facing backend URL such as:

```text
http://localhost:8080
```

rather than the Docker service name `api:8080`.

### Why CORS is needed

The frontend and backend use different origins:

- frontend: `http://localhost:3000` in Docker or `http://localhost:5173` in Vite dev mode
- backend: `http://localhost:8080`

So the API must allow those origins through CORS.

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

## Frontend Demo

The React frontend is intentionally lightweight and acts as a visualization layer rather than the core of the project.

It currently shows:

- a small sample graph
- highlighted route nodes and edges
- total cost returned by the API
- request timing for the frontend call

This makes the routing engine easier to demonstrate visually in interviews and on GitHub.

## Design Tradeoffs

A few deliberate tradeoffs were made:

- The graph is currently in-memory for simplicity and speed of implementation.
- The route cache is process-local and not distributed.
- The current cache eviction policy is intentionally simple, but the cache size is configurable to make tuning easier without code changes.
- Dijkstra was chosen as a strong baseline before exploring A\* or larger-scale optimizations.
- The frontend is intentionally small and focused on visualization rather than product-level UI complexity.

## Future Improvements

Possible next steps:

- smarter cache eviction and route popularity tracking for better hot-route retention
- richer graph visualization with animation of traversal steps
- benchmark comparison between naive priority selection and heap-based selection
- A\* search for heuristic-guided routing
- persistent graph storage
- richer observability and metrics
- configurable graph loading from external data
- distributed caching for multi-instance deployments
- stronger frontend component styling and route history
