using System.Collections.Generic;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    public Node startNode;
    public Node destinationNode;

    private List<Node> allNodes = new List<Node>();

    [SerializeField]Material pathStartMaterial;
    [SerializeField]Material pathEndMaterial;
    [SerializeField]Material pathMaterial;

    private Pathfinding pathfinding = new Pathfinding();
   

    // Method to set the nodes created by MapManager
    public void SetNodes(List<Node> nodes)
    {
        allNodes = nodes;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Reset the graph when the 'R' key is pressed
            pathfinding.ResetNodes(allNodes);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {

            
            if (startNode != null && destinationNode != null)
            {
                // Perform a Depth-First Search to find a path to the destinationNode
                startNode.GetComponent<Renderer>().material = pathStartMaterial;
                destinationNode.GetComponent<Renderer>().material = pathEndMaterial;
                bool pathFound = pathfinding.DepthFirstSearch(startNode, destinationNode);

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

   
}