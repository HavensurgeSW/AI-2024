using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TownImplement : MonoBehaviour
{
    [SerializeField] GameObject workerPrefab;
    [SerializeField] GameObject caravanPrefab;

    public TownCenter str;
    public List<Node> mineLocations;
    public Node ownLocation;
    [SerializeField]List<Node> shortestPath;
    Pathfinding scout;

    WorkerManager workerManager;



    public static Action<TownImplement> OnInit;
    #region ACTION_SUSCRIPTIONS
    private void OnEnable()
    {
        MapManager.RecalculatePaths += RemoveMineFromList;
     
    }
    private void OnDisable()
    {
        MapManager.RecalculatePaths -= RemoveMineFromList;
    }
    #endregion
    public void Init()
    {
        str = new TownCenter();
        scout = new Pathfinding();
        mineLocations = new List<Node>();
        shortestPath = new List<Node>();
        OnInit?.Invoke(this);
        workerManager = GetComponent<WorkerManager>();
        workerManager.shortestPath = this.shortestPath;
    }


    public void SetMineLocations(List<Node> mines)
    {
        for (int i = 0; i < mines.Count; i++)
        {
            mineLocations.Add(mines[i]);
        }
        FindNearestMine();
    }

    void FindNearestMine() {
        
        List<List<Node>> routes = new List<List<Node>>();

        for (int i = 0; i < mineLocations.Count; i++) {
            List<Node> path = new List<Node>();
            scout.AStar(ownLocation, mineLocations[i], out path);
            routes.Add(path);
            shortestPath = path;
        }

        foreach (List<Node> r in routes)
        {
            
            if (r.Count < shortestPath.Count)
            {
                shortestPath.Clear();
                shortestPath.AddRange(r);
            }

        }
    }

    void RemoveMineFromList(Node n) {
        if (mineLocations.Contains(n)) {
            mineLocations.Remove(n);
        }
        FindNearestMine();
    }

}
