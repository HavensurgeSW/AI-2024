using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoronoiManager : MonoBehaviour
{
    public List<Transform> nodeTransforms; // Assign in the Inspector
    private List<Polygon> voronoiPolygons;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {

            GenerateVoronoiPolygons(nodeTransforms);
        }
    }

    void GenerateVoronoiPolygons(List<Transform> transforms)
    {
        voronoiPolygons = new List<Polygon>();
        int n = transforms.Count;

        for (int i = 0; i < n; i++)
        {
            Vector2 position = new Vector2(transforms[i].position.x, transforms[i].position.z);
            List<Point> polygonVertices = new List<Point>();

            // Create a "dummy" Voronoi polygon based on distance to neighbors
            for (int j = 0; j < n; j++)
            {
                if (i != j)
                {
                    Vector2 neighborPosition = new Vector2(transforms[j].position.x, transforms[j].position.z);
                    Vector2 midpoint = (position + neighborPosition) / 2;
                    // Create vertices around the midpoint
                    polygonVertices.Add(new Point(midpoint.x - 1, midpoint.y - 1));
                    polygonVertices.Add(new Point(midpoint.x + 1, midpoint.y - 1));
                    polygonVertices.Add(new Point(midpoint.x + 1, midpoint.y + 1));
                    polygonVertices.Add(new Point(midpoint.x - 1, midpoint.y + 1));
                }
            }

            // Create a unique polygon for this node
            voronoiPolygons.Add(new Polygon(polygonVertices));
        }
    }

    public bool IsPointInAnyPolygon(Vector2 point)
    {
        foreach (var polygon in voronoiPolygons)
        {
            if (polygon.IsPointInside(point))
                return true; // The point is inside one of the polygons
        }
        return false; // The point is outside all polygons
    }

    void OnDrawGizmos()
    {
        // Draw the polygons for visualization
        if (voronoiPolygons != null)
        {
            Gizmos.color = Color.green;
            foreach (var polygon in voronoiPolygons)
            {
                Vector3 firstVertex = new Vector3(polygon.Vertices[0].Position.x, 0, polygon.Vertices[0].Position.y);
                for (int i = 1; i < polygon.Vertices.Count; i++)
                {
                    Vector3 vertex = new Vector3(polygon.Vertices[i].Position.x, 0, polygon.Vertices[i].Position.y);
                    Gizmos.DrawLine(firstVertex, vertex);
                    firstVertex = vertex;
                }
                // Closing the polygon
                Gizmos.DrawLine(firstVertex, new Vector3(polygon.Vertices[0].Position.x, 0, polygon.Vertices[0].Position.y));
            }
        }
    }
}

public class Point
{
    public Vector2 Position { get; private set; }

    public Point(float x, float y)
    {
        Position = new Vector2(x, y);
    }
}

public class Polygon
{
    public List<Point> Vertices { get; private set; }

    public Polygon(List<Point> vertices)
    {
        Vertices = vertices;
    }

    public bool IsPointInside(Vector2 point)
    {
        int intersectCount = 0;
        int n = Vertices.Count;

        for (int i = 0; i < n; i++)
        {
            Point v1 = Vertices[i];
            Point v2 = Vertices[(i + 1) % n];

            // Check if the point is on an edge
            if (IsPointOnSegment(point, v1.Position, v2.Position))
                return true;

            // Check for intersections
            if ((v1.Position.y > point.y) != (v2.Position.y > point.y) &&
                (point.x < (v2.Position.x - v1.Position.x) * (point.y - v1.Position.y) / (v2.Position.y - v1.Position.y) + v1.Position.x))
            {
                intersectCount++;
            }
        }

        return (intersectCount % 2) == 1;
    }

    private bool IsPointOnSegment(Vector2 p, Vector2 v1, Vector2 v2)
    {
        float crossProduct = (p.y - v1.y) * (v2.x - v1.x) - (p.x - v1.x) * (v2.y - v1.y);
        if (Mathf.Abs(crossProduct) > 1e-10) return false; // Not collinear

        float dotProduct = (p.x - v1.x) * (v2.x - v1.x) + (p.y - v1.y) * (v2.y - v1.y);
        if (dotProduct < 0) return false; // p is before v1

        float squaredLength = (v2.x - v1.x) * (v2.x - v1.x) + (v2.y - v1.y) * (v2.y - v1.y);
        return dotProduct <= squaredLength; // p is after v2
    }
}
