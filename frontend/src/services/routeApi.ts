import type { RouteResponse } from "../types/route";

const API_BASE = import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5034";

export async function findRoute(
  source: string,
  destination: string,
): Promise<RouteResponse> {
  const response = await fetch(`${API_BASE}/api/route`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({ source, destination }),
  });

  const data = await response.json();
  debugger;
  if (!response.ok) {
    throw new Error(data.error ?? "Route lookup failed.");
  }

  return data as RouteResponse;
}
