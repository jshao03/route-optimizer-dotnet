import { useState } from "react";
import GraphVisualizer from "./components/GraphVisualizer";
import { edges, nodes } from "./data/graph";
import { findRoute } from "./services/routeApi";
import type { RouteResponse } from "./types/route";

const styles: Record<string, React.CSSProperties> = {
  page: {
    minHeight: "100vh",
    background: "#f8fafc",
    padding: 24,
    fontFamily: "Inter, ui-sans-serif, system-ui, sans-serif",
    color: "#0f172a",
  },
  layout: {
    maxWidth: 1200,
    margin: "0 auto",
    display: "grid",
    gridTemplateColumns: "360px 1fr",
    gap: 24,
  },
  card: {
    background: "white",
    border: "1px solid #e2e8f0",
    borderRadius: 20,
    boxShadow: "0 1px 3px rgba(15, 23, 42, 0.06)",
    overflow: "hidden",
  },
  cardHeader: {
    padding: 24,
    borderBottom: "1px solid #e2e8f0",
  },
  cardBody: {
    padding: 24,
  },
  title: {
    margin: 0,
    fontSize: 28,
    fontWeight: 700,
  },
  subtitle: {
    marginTop: 8,
    color: "#475569",
    fontSize: 14,
    lineHeight: 1.5,
  },
  label: {
    display: "block",
    fontSize: 14,
    fontWeight: 600,
    marginBottom: 8,
  },
  input: {
    width: "100%",
    padding: "12px 14px",
    borderRadius: 12,
    border: "1px solid #cbd5e1",
    fontSize: 14,
    boxSizing: "border-box",
    outline: "none",
  },
  buttonRow: {
    display: "flex",
    gap: 12,
    flexWrap: "wrap",
    marginTop: 8,
  },
  primaryButton: {
    border: 0,
    background: "#0f172a",
    color: "white",
    padding: "12px 18px",
    borderRadius: 14,
    fontWeight: 600,
    cursor: "pointer",
  },
  secondaryButton: {
    border: "1px solid #cbd5e1",
    background: "white",
    color: "#0f172a",
    padding: "12px 18px",
    borderRadius: 14,
    fontWeight: 600,
    cursor: "pointer",
  },
  statBox: {
    border: "1px solid #e2e8f0",
    borderRadius: 16,
    padding: 16,
    display: "flex",
    justifyContent: "space-between",
    alignItems: "center",
    marginTop: 12,
  },
  badge: {
    background: "#e2e8f0",
    color: "#0f172a",
    borderRadius: 999,
    padding: "6px 10px",
    fontSize: 12,
    fontWeight: 600,
  },
  error: {
    marginTop: 16,
    border: "1px solid #fecaca",
    background: "#fef2f2",
    color: "#991b1b",
    borderRadius: 16,
    padding: 16,
  },
  result: {
    marginTop: 16,
    border: "1px solid #e2e8f0",
    background: "#fff",
    borderRadius: 16,
    padding: 16,
  },
  pathRow: {
    display: "flex",
    gap: 8,
    flexWrap: "wrap",
    alignItems: "center",
    marginTop: 10,
  },
  pathNode: {
    display: "inline-block",
    background: "#0f172a",
    color: "white",
    borderRadius: 12,
    padding: "6px 10px",
    fontWeight: 700,
    fontSize: 13,
  },
};

export default function App() {
  const [source, setSource] = useState("A");
  const [destination, setDestination] = useState("D");
  const [result, setResult] = useState<RouteResponse | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [elapsedMs, setElapsedMs] = useState<number | null>(null);

  async function handleFindRoute() {
    setLoading(true);
    setError(null);
    setElapsedMs(null);

    const start = performance.now();

    try {
      const data = await findRoute(source, destination);
      const end = performance.now();

      setElapsedMs(end - start);
      setResult(data);
    } catch (err) {
      setResult(null);
      setError(
        err instanceof Error
          ? err.message
          : "Could not reach the API. Make sure the backend is running."
      );
    } finally {
      setLoading(false);
    }
  }

  function resetView() {
    setSource("A");
    setDestination("D");
    setResult(null);
    setError(null);
    setElapsedMs(null);
  }

  return (
    <div style={styles.page}>
      <div style={styles.layout}>
        <div style={styles.card}>
          <div style={styles.cardHeader}>
            <h1 style={styles.title}>Route Optimizer Demo</h1>
            <div style={styles.subtitle}>
              React + TypeScript frontend for your .NET routing engine.
            </div>
          </div>

          <div style={styles.cardBody}>
            <div>
              <label style={styles.label}>Source Node</label>
              <input
                style={styles.input}
                value={source}
                onChange={(e) => setSource(e.target.value.toUpperCase())}
              />
            </div>

            <div style={{ marginTop: 16 }}>
              <label style={styles.label}>Destination Node</label>
              <input
                style={styles.input}
                value={destination}
                onChange={(e) => setDestination(e.target.value.toUpperCase())}
              />
            </div>

            <div style={styles.buttonRow}>
              <button
                style={styles.primaryButton}
                onClick={handleFindRoute}
                disabled={loading}
              >
                {loading ? "Running..." : "Find Route"}
              </button>

              <button style={styles.secondaryButton} onClick={resetView}>
                Reset
              </button>
            </div>

            <div style={styles.statBox}>
              <div style={{ fontWeight: 600, color: "#475569" }}>Graph</div>
              <div style={styles.badge}>
                {nodes.length} nodes / {edges.length} edges
              </div>
            </div>

            <div style={styles.statBox}>
              <div style={{ fontWeight: 600, color: "#475569" }}>
                Request Time
              </div>
              <div style={styles.badge}>
                {elapsedMs === null ? "-" : `${elapsedMs.toFixed(2)} ms`}
              </div>
            </div>

            {error && <div style={styles.error}>{error}</div>}

            {result && result.routeFound && (
              <div style={styles.result}>
                <div style={{ fontWeight: 700 }}>Shortest Path</div>
                <div style={styles.pathRow}>
                  {result.route.routeNodes.map((node, index) => (
                    <div
                      key={`${node.id}-${index}`}
                      style={{ display: "flex", alignItems: "center", gap: 8 }}
                    >
                      <span style={styles.pathNode}>{node.id}</span>
                      {index < result.route.routeNodes.length - 1 && (
                        <span style={{ color: "#94a3b8" }}></span>
                      )}
                    </div>
                  ))}
                </div>
                <div style={{ marginTop: 12, color: "#475569" }}>
                  Total distance:{" "}
                  <strong style={{ color: "#0f172a" }}>
                    {result.totalCost}
                  </strong>
                </div>
              </div>
            )}
          </div>
        </div>

        <GraphVisualizer routeNodes={result?.route?.routeNodes} />
      </div>
    </div>
  );
}