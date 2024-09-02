using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    [Header("Menu options")]
    public TMP_InputField heightInput;
    public TMP_InputField widthInput;
    public TMP_InputField minesInput;

    [SerializeField] Material mineMaterial;


    [Header("Grid Settings")]
    [SerializeField]private GameObject tilePrefab;
    [SerializeField]private int gridWidth = 5;     
    [SerializeField]private int gridHeight = 5;     
    [SerializeField]private float tileSpacing = 1.0f; 
    [SerializeField]private GraphManager graphManager;

    Structure goldMine;

    private Node[,] grid;  

    private void CreateGrid(int width, int height)
    {
        gridWidth = width;
        gridHeight = height;
        

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

       
        if (graphManager != null)
        {
            graphManager.SetNodes(allNodes);
        }
    }

    private void AssignStructureToNode(Structure str) 
    {
        int value3 = int.Parse(minesInput.text);

        for (int x = 0; x < value3; x++)
        {
            int rand1= Random.Range(0, gridWidth);
            int rand2= Random.Range(0, gridHeight);
            if (grid[rand1, rand2].CheckForStructure())
                grid[rand1, rand2].SetStructure(str);
            Debug.Log("Set mine at Tile: " + rand1 + ", " + rand2);
            grid[rand1, rand2].GetComponent<Renderer>().material = mineMaterial;
        }
    }

    public void InitGameElements()
    {
        gridHeight = int.Parse(heightInput.text);
        gridWidth = int.Parse(widthInput.text);

        CreateGrid(gridWidth, gridHeight);
        goldMine = new Mine();
        AssignStructureToNode(goldMine);
                
    }


}