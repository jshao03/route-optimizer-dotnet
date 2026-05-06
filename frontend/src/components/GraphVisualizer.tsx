import { edges, nodes } from "../data/graph";
import type { RoutePathNode } from "../types/route";

type GraphVisualizerProps = {
  routeNodes?: RoutePathNode[];
};

function getNode(id: string) {
  return nodes.find((node) => node.id === id);
}

function edgeKey(from: string, to: string) {
  return `${from}-${to}`;
}

function buildHighlightedEdges(path?: RoutePathNode[]) {
  const keys = new Set<string>();

  if (!path) {
    return keys;
  }

  for (let i = 0; i < path.length - 1; i++) {
    keys.add(edgeKey(path[i].id, path[i + 1].id));
  }

  return keys;
}

export default function GraphVisualizer({
  routeNodes,
}: GraphVisualizerProps) {
  const highlightedEdges = buildHighlightedEdges(routeNodes);

  return (
    <div
      style={{
        background: "white",
        border: "1px solid #e2e8f0",
        borderRadius: 20,
        boxShadow: "0 1px 3px rgba(15, 23, 42, 0.06)",
        overflow: "hidden",
      }}
    >
      <div style={{ padding: 24, borderBottom: "1px solid #e2e8f0" }}>
        <h2 style={{ margin: 0, fontSize: 22 }}>Graph Visualizer</h2>
        <div
          style={{
            marginTop: 8,
            color: "#475569",
            fontSize: 14,
            lineHeight: 1.5,
          }}
        >
          The highlighted path shows the route returned by your API.
        </div>
      </div>

      <div style={{ padding: 24 }}>
        <div
          style={{
            overflow: "hidden",
            borderRadius: 20,
            border: "1px solid #e2e8f0",
            background: "white",
          }}
        >
          <svg
            viewBox="0 0 520 300"
            style={{ width: "100%", height: 520, background: "#f8fafc" }}
          >
            <defs>
              <marker
                id="arrow"
                viewBox="0 0 10 10"
                refX="9"
                refY="5"
                markerWidth="6"
                markerHeight="6"
                orient="auto-start-reverse"
              >
                <path d="M 0 0 L 10 5 L 0 10 z" fill="#64748b" />
              </marker>
              <marker
                id="arrow-highlight"
                viewBox="0 0 10 10"
                refX="9"
                refY="5"
                markerWidth="6"
                markerHeight="6"
                orient="auto-start-reverse"
              >
                <path d="M 0 0 L 10 5 L 0 10 z" fill="#0f172a" />
              </marker>
            </defs>

            {edges.map((edge) => {
              const from = getNode(edge.from);
              const to = getNode(edge.to);

              if (!from || !to) {
                return null;
              }

              const isHighlighted = highlightedEdges.has(
                edgeKey(edge.from, edge.to)
              );
              const midX = (from.x + to.x) / 2;
              const midY = (from.y + to.y) / 2;

              return (
                <g key={edgeKey(edge.from, edge.to)}>
                  <line
                    x1={from.x}
                    y1={from.y}
                    x2={to.x}
                    y2={to.y}
                    strokeWidth={isHighlighted ? 5 : 2.5}
                    stroke={isHighlighted ? "#0f172a" : "#94a3b8"}
                    markerEnd={
                      isHighlighted
                        ? "url(#arrow-highlight)"
                        : "url(#arrow)"
                    }
                  />
                  <text
                    x={midX}
                    y={midY - 8}
                    textAnchor="middle"
                    fill="#64748b"
                    fontSize="12"
                    fontWeight="600"
                  >
                    {edge.weight}
                  </text>
                </g>
              );
            })}

            {nodes.map((node) => {
              const isInPath =
                Array.isArray(routeNodes) &&
                routeNodes.some((pathNode) => pathNode.id === node.id);

              return (
                <g key={node.id}>
                  <circle
                    cx={node.x}
                    cy={node.y}
                    r={isInPath ? 22 : 18}
                    fill={isInPath ? "#0f172a" : "white"}
                    stroke="#0f172a"
                    strokeWidth="2"
                  />
                  <text
                    x={node.x}
                    y={node.y + 4}
                    textAnchor="middle"
                    fill={isInPath ? "white" : "#0f172a"}
                    fontSize="13"
                    fontWeight="700"
                  >
                    {node.id}
                  </text>
                </g>
              );
            })}
          </svg>
        </div>
      </div>
    </div>
  );
}