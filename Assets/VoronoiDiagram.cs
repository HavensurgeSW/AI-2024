using System.Collections.Generic;
using UnityEngine;

public class VoronoiDiagram : MonoBehaviour
{
    public List<GameObject> nodes;  // List of GameObjects representing nodes
    public float edgeLength = 100f;  // Length of Voronoi edges for visualization

    private void Start()
    {
        DrawVoronoiDiagram();
    }

    private void DrawVoronoiDiagram()
    {
        // Create a new GameObject with a LineRenderer to draw Voronoi edges
        GameObject voronoiObject = new GameObject("VoronoiDiagram");
        LineRenderer lineRenderer = voronoiObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.widthMultiplier = 0.1f;  // Adjust width as needed
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));  // Simple material for visualization
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;

        HashSet<Edge> edges = new HashSet<Edge>();
        List<Vector3> linePoints = new List<Vector3>();

        // Loop through each pair of nodes to compute Voronoi edges
        for (int i = 0; i < nodes.Count; i++)
        {
            GameObject nodeA = nodes[i];
            Vector3 posA = nodeA.transform.position;

            for (int j = i + 1; j < nodes.Count; j++)
            {
                GameObject nodeB = nodes[j];
                Vector3 posB = nodeB.transform.position;

                // Calculate midpoint between nodeA and nodeB
                Vector3 midpoint = (posA + posB) / 2;

                // Calculate the perpendicular direction
                Vector3 direction = (posB - posA).normalized;
                Vector3 normal = new Vector3(-direction.z, 0, direction.x);

                // Calculate the edge points
                Vector3 edgeStart = midpoint + normal * edgeLength;
                Vector3 edgeEnd = midpoint - normal * edgeLength;

                Edge newEdge = new Edge(edgeStart, edgeEnd);

                // Only add edge if it's not already in the set (to avoid duplicate edges)
                if (!edges.Contains(newEdge))
                {
                    edges.Add(newEdge);
                    linePoints.Add(edgeStart);
                    linePoints.Add(edgeEnd);
                }
            }
        }

        // Set the positions of the LineRenderer
        lineRenderer.positionCount = linePoints.Count;
        lineRenderer.SetPositions(linePoints.ToArray());
    }

    private struct Edge
    {
        public Vector3 Start;
        public Vector3 End;

        public Edge(Vector3 start, Vector3 end)
        {
            Start = start;
            End = end;
        }

        // Override Equals and GetHashCode for Edge comparison in HashSet
        public override bool Equals(object obj)
        {
            if (obj is Edge edge)
            {
                return (Start == edge.Start && End == edge.End) || (Start == edge.End && End == edge.Start);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Start.GetHashCode() ^ End.GetHashCode();
        }
    }
}