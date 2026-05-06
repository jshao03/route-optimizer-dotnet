import type { GraphEdge, GraphNode } from "../types/route";

export const nodes: GraphNode[] = [
  { id: "A", x: 80, y: 120 },
  { id: "B", x: 210, y: 70 },
  { id: "C", x: 220, y: 180 },
  { id: "D", x: 430, y: 90 },
  { id: "E", x: 370, y: 220 },
];

export const edges: GraphEdge[] = [
  { from: "A", to: "B", weight: 5 },
  { from: "A", to: "C", weight: 2 },
  { from: "B", to: "D", weight: 4 },
  { from: "C", to: "D", weight: 7 },
  { from: "C", to: "E", weight: 3 },
  { from: "E", to: "D", weight: 1 },
];