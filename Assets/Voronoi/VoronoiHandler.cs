using System.Collections.Generic;

using UnityEngine;

public class VoronoiHandler : MonoBehaviour
{
    #region EXPOSED_FIELDS
    public float alpha = 0;
    #endregion
    
    #region PRIVATE_FIELDS
    private List<Edge> edges = new List<Edge>();
    private List<Sector> sectors = new List<Sector>();
    #endregion

    #region UNITY_CALLS
    private void OnDrawGizmos()
    {
        if (sectors == null) return;

        foreach (var sector in sectors)
        {
            sector.DrawSector(alpha);
            
            sector.DrawSegments();
        }
    }
    #endregion

    #region PUBLIC_METHODS
    public void Config(Vector3 bottomLeft, Vector3 topRight)
    {
        // Configure initial values of the Voronoi diagram by creating 4 edges representing the boundaries (left, up, right, down)
        edges.Add(new Edge(new Vector2(bottomLeft.x, bottomLeft.z), DIR.LEFT));
        edges.Add(new Edge(new Vector2(bottomLeft.x, topRight.z), DIR.UP));
        edges.Add(new Edge(new Vector2(topRight.x, topRight.z), DIR.RIGHT));
        edges.Add(new Edge(new Vector2(topRight.x, bottomLeft.z), DIR.DOWN));
    }

    public void UpdateSectors(List<(Vector2,float)> mines)
    {
        sectors.Clear();
        if (mines.Count == 0) return;

        
        foreach (var mine in mines)
        {
            sectors.Add(new Sector(mine.Item1));
        }

        
        foreach (var mineSector in sectors)
        {
            mineSector.AddSegmentLimits(edges);
        }

        
        for (int i = 0; i < mines.Count; i++)
        {
            for (int j = 0; j < mines.Count; j++)
            {
                if (i == j) continue;

                sectors[i].AddSegment(mines[i].Item1, mines[j].Item1, mines[i].Item2, mines[j].Item2);
            }
        }

      
        foreach (var mineSector in sectors)
        {
            mineSector.SetIntersections();
        }
    }

    public Vector2Int GetNearestMine(Vector2 currentPos)
    {
        if (sectors == null) return Vector2Int.zero;
        
        foreach (var sector in sectors)
        {
            if (sector.IsPointInSector(currentPos))
            {
                return new Vector2Int((int)sector.minePos.x, (int)sector.minePos.y);
            }
        }

        return Vector2Int.zero;
    }
    #endregion
}