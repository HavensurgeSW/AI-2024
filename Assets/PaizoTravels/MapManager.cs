using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;


public class MapManager : MonoBehaviour
{
    #region UNITY_EDITOR
    [Header("Menu options")]
    public TMP_InputField heightInput;
    public TMP_InputField widthInput;
    public TMP_InputField minesInput;
    public Slider heightSlider;
    public Slider widthSlider;

    [Header("Grid Settings")]
    [SerializeField]private GameObject tilePrefab;
    [SerializeField]private GameObject townPrefab;
    [SerializeField]private GameObject minePrefab;
    [SerializeField]private int gridWidth = 5;     
    [SerializeField]private int gridHeight = 5;     
    [SerializeField]private float tileSpacing = 1.0f; 
    [SerializeField]private GraphManager graphManager;
    #endregion

    private Node[,] grid;
    private List<Node> mineList = new List<Node>();
    private List<Node> minesInUse = new List<Node>();

    public static Action<Node> RecalculatePaths;

    private void OnEnable()
    {
        MineImplement.OnMineEmpty += RemoveMine;
    }
    private void OnDisable()
    {
        MineImplement.OnMineEmpty -= RemoveMine;
    }
    private void CreateGrid(int width, int height)
    {
        gridWidth = width;
        gridHeight = height;

        GridUtils.GridSize.Set(gridWidth, gridHeight);
        

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
                tile.transform.parent = this.transform;

                Node node = tile.GetComponent<Node>();
                
                if (node != null)
                {
                    grid[x, y] = node;
                    allNodes.Add(node);
                    
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

    private void AssignRandomStructures() 
    {
        int totalMines = int.Parse(minesInput.text);
        int rand1;
        int rand2;

        GameObject MineObject;
        MineImplement MI = new MineImplement();
        for (int x = 0; x < totalMines; x++)
        {
            Mine protoMine = new Mine();
            rand1= Random.Range(0, gridWidth);
            rand2= Random.Range(0, gridHeight);
            if (grid[rand1, rand2].CheckForStructure() == false)
            {
                grid[rand1, rand2].SetStructure(protoMine);
                grid[rand1, rand2].mapPos.x = rand1;
                grid[rand1, rand2].mapPos.y = rand2;
                Debug.Log("Set mine at Tile: " + rand1 + ", " + rand2);
                mineList.Add(grid[rand1, rand2]);

                MineObject = Instantiate(minePrefab, grid[rand1, rand2].transform);
                MI = MineObject.GetComponent<MineImplement>();
                MI.SetCoordinates(new Vector2Int(rand1, rand2));

            }
        }

        GameObject TC;
        TownImplement TI;
        bool townBuildFinish = false;
        do
        {
           
            rand1 = Random.Range(0, gridWidth);
            rand2 = Random.Range(0, gridHeight);
            if (grid[rand1, rand2].CheckForStructure() == false)
            {
                TC = Instantiate(townPrefab, grid[rand1,rand2].transform);
                TI = TC.GetComponent<TownImplement>();
                TI.Init();
             

                grid[rand1, rand2].SetTown(TI.str);
                TI.ownLocation = grid[rand1, rand2];
                TI.SetMineLocations(mineList);

                townBuildFinish = true;
                Debug.Log("Town built at " + rand1 + ", " + rand2);
            }

        } while (!townBuildFinish);
    }

    public void InitGameElements()
    {
        
        gridHeight = Mathf.RoundToInt(heightSlider.value);
        gridWidth= Mathf.RoundToInt(widthSlider.value);

        CreateGrid(gridWidth, gridHeight);
        AssignRandomStructures();
                
    }

    private void RemoveMine(MineImplement m) {
        Node n = grid[m.GetCoordinates().x, m.GetCoordinates().y];
        if (mineList.Contains(n))
        {
            mineList.Remove(n);
            RecalculatePaths?.Invoke(n);
        }
    }

}