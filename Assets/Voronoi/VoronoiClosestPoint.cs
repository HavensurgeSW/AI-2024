using UnityEngine;
using System.Collections.Generic;

public class VoronoiClosestPoint : MonoBehaviour
{
    public List<Transform> analyzedPoints = new List<Transform>(); // The list of points that create the Voronoi regions
    public Transform testPoint; // The point you want to test

    // Method to determine which analyzed point is closest to the testPoint
    public Transform GetClosestPoint()
    {
        Transform closestPoint = null;
        float shortestDistance = float.MaxValue;

        // Iterate over each analyzed point
        foreach (var analyzedPoint in analyzedPoints)
        {
            bool isClosest = true;

            // Compare this analyzed point against all other points
            foreach (var otherPoint in analyzedPoints)
            {
                if (otherPoint == analyzedPoint) continue;

                // Check if the testPoint is on the correct side of the mediatrix
                if (!IsPointCloserTo(testPoint.position, analyzedPoint.position, otherPoint.position))
                {
                    isClosest = false;
                    break;
                }
            }

            // If the testPoint is closer to this analyzedPoint than any other
            if (isClosest)
            {
                float distance = Vector3.Distance(testPoint.position, analyzedPoint.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closestPoint = analyzedPoint;
                }
            }
        }

        return closestPoint;
    }

    // Method to check if point is closer to 'pointA' than to 'pointB'
    private bool IsPointCloserTo(Vector3 point, Vector3 pointA, Vector3 pointB)
    {
        // Get the direction from pointA to pointB
        Vector3 direction = pointB - pointA;

        // Find the perpendicular vector (mediatrix)
        Vector3 perpendicular = new Vector3(-direction.z, 0, direction.x).normalized;

        // Calculate the midpoint of pointA and pointB (where the mediatrix passes through)
        Vector3 midpoint = (pointA + pointB) / 2;

        // Check which side of the mediatrix the test point is on
        Vector3 pointToMidpoint = point - midpoint;
        float dotProduct = Vector3.Dot(pointToMidpoint, perpendicular);

        // If the dot product is positive, the point is on the same side as pointA, otherwise, it's closer to pointB
        return dotProduct > 0;
    }

    private void OnDrawGizmos()
    {
        if (testPoint == null) return;

        Transform closest = GetClosestPoint();
        if (closest != null)
        {
            // Draw a line from the test point to the closest analyzed point
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(testPoint.position, closest.position);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            GetClosestPoint();
        }

    }
}