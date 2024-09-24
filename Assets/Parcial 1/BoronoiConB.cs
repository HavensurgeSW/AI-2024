using System.Collections.Generic;
using UnityEngine;

public class BoronoiConB : MonoBehaviour
{
    public List<Transform> points = new List<Transform>();
    private Vector3 boardMin, boardMax;

    void Start()
    {
        // Define the board boundaries based on the cube's position and scale.
        GameObject board = GameObject.Find("Board"); // Assuming the cube is named "Board"
        Vector3 boardPosition = board.transform.position;
        Vector3 boardScale = board.transform.localScale;

        boardMin = boardPosition - boardScale / 2;
        boardMax = boardPosition + boardScale / 2;
    }

    void OnDrawGizmos()
    {
        if (points == null || points.Count < 2)
            return;

        for (int i = 0; i < points.Count; i++)
        {
            for (int j = i + 1; j < points.Count; j++)
            {
                DrawMediatriz(points[i].position, points[j].position);
            }
        }
    }

    void DrawMediatriz(Vector3 pointA, Vector3 pointB)
    {
        // Calculate the midpoint between two points
        Vector3 midpoint = (pointA + pointB) / 2;

        // Find the direction perpendicular to the line between the two points
        Vector3 direction = (pointB - pointA).normalized;
        Vector3 perpendicular = new Vector3(-direction.z, direction.y, direction.x); // Perpendicular direction in 3D space

        // Define a large range for the mediatriz line, and calculate intersections with the board
        Vector3[] boundaryIntersections = GetBoundaryIntersections(midpoint, perpendicular);

        // Draw the line within the boundary intersections
        Gizmos.color = Color.green;
        Gizmos.DrawLine(boundaryIntersections[0], boundaryIntersections[1]);
    }

    Vector3[] GetBoundaryIntersections(Vector3 midpoint, Vector3 direction)
    {
        Vector3[] intersections = new Vector3[2];
        float tMin = float.MaxValue, tMax = -float.MaxValue;

        // Define the 4 sides of the board
        Vector3[] boardEdges = new Vector3[4]
        {
            new Vector3(boardMin.x, midpoint.y, boardMin.z), // Bottom-left
            new Vector3(boardMax.x, midpoint.y, boardMin.z), // Bottom-right
            new Vector3(boardMax.x, midpoint.y, boardMax.z), // Top-right
            new Vector3(boardMin.x, midpoint.y, boardMax.z), // Top-left
        };

        for (int i = 0; i < 4; i++)
        {
            Vector3 edgeStart = boardEdges[i];
            Vector3 edgeEnd = boardEdges[(i + 1) % 4];

            // Check if the line intersects this edge
            Vector3 intersection;
            if (LineIntersection(midpoint, direction, edgeStart, edgeEnd, out intersection))
            {
                float t = Vector3.Distance(midpoint, intersection);
                if (t < tMin)
                {
                    tMin = t;
                    intersections[0] = intersection;
                }
                if (t > tMax)
                {
                    tMax = t;
                    intersections[1] = intersection;
                }
            }
        }

        return intersections;
    }

    bool LineIntersection(Vector3 p1, Vector3 dir1, Vector3 p2, Vector3 p3, out Vector3 intersection)
    {
        Vector3 dir2 = p3 - p2;
        Vector3 cross = Vector3.Cross(dir1, dir2);
        float denominator = cross.sqrMagnitude;

        // Parallel check
        if (denominator < Mathf.Epsilon)
        {
            intersection = Vector3.zero;
            return false;
        }

        Vector3 diff = p2 - p1;
        float t = Vector3.Cross(diff, dir2).magnitude / cross.magnitude;
        intersection = p1 + dir1 * t;
        return true;
    }
}