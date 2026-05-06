export type GraphNode = {
  id: string;
  x: number;
  y: number;
};

export type GraphEdge = {
  from: string;
  to: string;
  weight: number;
};

export type RoutePathNode = {
  id: string;
};

export type RouteDetails = {
  routeNodes: RoutePathNode[];
  cost: number;
};

export type RouteResponse = {
  route: RouteDetails;
  routeFound: boolean;
  totalCost: number;
};