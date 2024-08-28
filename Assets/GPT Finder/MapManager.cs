using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public GameObject tilePrefab;  // The prefab for the tiles
    public int gridWidth = 10;     // Number of tiles in width
    public int gridHeight = 10;    // Number of tiles in height
    public float tileSpacing = 1.0f;  // Spacing between tiles

    public GraphManager graphManager; // Reference to the GraphManager

    private Node[,] grid;  // 2D array to hold the nodes

    // Method to create the grid
    public void CreateGrid()
    {
        if (tilePrefab == null)
        {
            Debug.LogError("Tile Prefab is not assigned!");
            return;
        }

        grid = new Node[gridWidth, gridHeight];
        List<Node> allNodes = new List<Node>();

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 position = new Vector3(x * tileSpacing, 0, y * tileSpacing);
                GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity);
                tile.name = $"Tile_{x}_{y}";
                tile.transform.parent = this.transform;  // Set the grid manager as the parent

                Node node = tile.GetComponent<Node>();
                if (node != null)
                {
                    grid[x, y] = node;
                    allNodes.Add(node);

                    // Add neighbors to the node
                    if (x > 0)
                    {
                        Node leftNeighbor = grid[x - 1, y];
                        node.AddNeighbor(leftNeighbor);
                    }
                    if (y > 0)
                    {
                        Node bottomNeighbor = grid[x, y - 1];
                        node.AddNeighbor(bottomNeighbor);
                    }
                }
            }
        }

        // Pass the list of nodes to the GraphManager
        if (graphManager != null)
        {
            graphManager.SetNodes(allNodes);
        }
    }

    // Automatically create the grid when the game starts
    void Start()
    {
        CreateGrid();
    }
}