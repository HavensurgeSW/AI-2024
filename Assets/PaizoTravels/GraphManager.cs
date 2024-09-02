using System.Collections.Generic;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    public Node startNode;
    public Node destinationNode;

    public List<Node> allNodes = new List<Node>();
    private Pathfinding pathfinding = new Pathfinding();

    
    public void SetNodes(List<Node> nodes)
    {
        allNodes = nodes;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
        
            pathfinding.ResetNodes(allNodes);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {

            
            if (startNode != null && destinationNode != null)
            {
                
                RandomizeDistances();
                bool pathFound = pathfinding.Dijkstra(startNode, destinationNode);

                if (pathFound)
                {
                    Debug.Log("Path found to the destination!");
                }
                else
                {
                    Debug.Log("Path not found to the destination.");
                }
            }
        }
    }

    public void RandomizeDistances()
    {
        System.Random random = new System.Random();

        foreach (Node node in allNodes)
        {
            foreach (Node neighbor in node.neighbors)
            {
                float randomDistance = random.Next(1, 11);
                node.distance = randomDistance;
                neighbor.distance = randomDistance;
            }
        }
        Debug.Log("Distances between nodes have been randomized.");
    }


}