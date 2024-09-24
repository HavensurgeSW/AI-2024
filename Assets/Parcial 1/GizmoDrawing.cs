using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoDrawing : MonoBehaviour
{
    // List of Transforms to compare
    public List<Transform> points = new List<Transform>();

    // Define the bounding box (2D square) in the XZ plane
    public Vector3 topLeft;
    public Vector3 bottomRight;

    // Length of the mediatrix line to draw (if no intersections)
    public float maxLineLength = 20f;

    // List to store all line segments (mediatrices) for intersection checks
    private List<(Vector3 start, Vector3 end)> lines = new List<(Vector3, Vector3)>();

    private void OnDrawGizmos()
    {
        if (points == null || points.Count < 2) return;

        // Clear the list of lines before drawing
        lines.Clear();

        // Draw the bounding box
        Gizmos.color = Color.green;

        // Create the other two corners for the box
        Vector3 bottomLeft = new Vector3(topLeft.x, topLeft.y, bottomRight.z); // Z from bottomRight
        Vector3 topRight = new Vector3(bottomRight.x, topLeft.y, topLeft.z);  // X from bottomRight

        // Draw the bounding box as a green square (on the XZ plane)
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);

        // Iterate over all pairs of points in the list to create mediatrices
        for (int i = 0; i < points.Count; i++)
        {
            for (int j = i + 1; j < points.Count; j++)
            {
                CreateAndDrawMediatrix(points[i], points[j]);
            }
        }
    }

    // Draw mediatrix between two Transform points
    private void CreateAndDrawMediatrix(Transform pointA, Transform pointB)
    {
        if (pointA == null || pointB == null) return;

        // Calculate the midpoint between pointA and pointB
        Vector3 midpoint = (pointA.position + pointB.position) / 2;

        // Get the direction from pointA to pointB and calculate the perpendicular direction
        Vector3 direction = pointB.position - pointA.position;
        Vector3 perpendicular = new Vector3(-direction.z, 0, direction.x).normalized; // Perpendicular to the XZ direction

        // Determine the start and end points of the mediatrix line
        Vector3 lineStart = midpoint + perpendicular * maxLineLength /2;
        Vector3 lineEnd = midpoint - perpendicular * maxLineLength /2;

        // Clamp the line start and end within the bounding box
        Vector3 clampedStart = ClampToBoundingBoxXZ(lineStart, topLeft, bottomRight);
        Vector3 clampedEnd = ClampToBoundingBoxXZ(lineEnd, topLeft, bottomRight);

        // Check for intersections with existing lines and stop at the first intersection
        (Vector3 start, Vector3 end) clippedLine = ClipLineWithExistingLines(clampedStart, clampedEnd);

        // Store the clipped line for future intersection checks
        lines.Add(clippedLine);

        // Draw the mediatrix line in red
        Gizmos.color = Color.red;
        Gizmos.DrawLine(clippedLine.start, clippedLine.end);
    }

    // Helper function to clamp a point inside the bounding box in the XZ plane
    private Vector3 ClampToBoundingBoxXZ(Vector3 point, Vector3 topLeft, Vector3 bottomRight)
    {
        float x = Mathf.Clamp(point.x, topLeft.x, bottomRight.x);
        float z = Mathf.Clamp(point.z, bottomRight.z, topLeft.z); // Clamping Z axis
        return new Vector3(x, point.y, z); // Maintain the Y coordinate
    }

    // Helper function to check for intersection with existing lines
    private (Vector3 start, Vector3 end) ClipLineWithExistingLines(Vector3 start, Vector3 end)
    {
        foreach (var line in lines)
        {
            if (TryGetLineIntersection(start, end, line.start, line.end, out Vector3 intersection))
            {
                // If intersection found, return the clipped line ending at the intersection
                return (start, intersection);
            }
        }

        // If no intersections, return the original line
        return (start, end);
    }

    // Line intersection algorithm (2D XZ plane)
    private bool TryGetLineIntersection(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, out Vector3 intersection)
    {
        intersection = Vector3.zero;

        float denominator = (p4.z - p3.z) * (p2.x - p1.x) - (p4.x - p3.x) * (p2.z - p1.z);

        // Lines are parallel if denominator is zero
        if (Mathf.Abs(denominator) < 0.0001f)
        {
            return false;
        }

        float ua = ((p4.x - p3.x) * (p1.z - p3.z) - (p4.z - p3.z) * (p1.x - p3.x)) / denominator;
        float ub = ((p2.x - p1.x) * (p1.z - p3.z) - (p2.z - p1.z) * (p1.x - p3.x)) / denominator;

        // Check if intersection point is within the line segments
        if (ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1)
        {
            intersection = p1 + ua * (p2 - p1);
            return true;
        }

        return false;
    }
}

