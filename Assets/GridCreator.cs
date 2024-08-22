using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCreator : MonoBehaviour
{
    public GameObject cubePrefab;
    public int rows = 5;           
    public int columns = 5;        
    public float spacing = 2.0f;   

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        // Iterate over the rows and columns to create the grid
        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < columns; y++)
            {
                // Calculate the position for each cube
                Vector3 position = new Vector3(x * spacing, 0, y * spacing);

                // Instantiate the cube at the calculated position
                GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity);

                // Set the cube as a child of this GameObject
                cube.transform.parent = transform;

                // Set the name of the cube based on its position
                cube.name = $"{x},{y}";
            }
        }
    }
}
