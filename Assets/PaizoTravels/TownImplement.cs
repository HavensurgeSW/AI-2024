using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TownImplement : MonoBehaviour
{

    public TownCenter str;
    public List<Node> mineLocations;
    public Node ownLocation;
    [SerializeField]List<Node> shortestPath;
    Pathfinding scout;

    WorkerManager workerManager;
    [SerializeField] int goldStock;


    public static Action<TownImplement> OnInit;
    public static Action<List<Node>> PathsCompleted;
    #region ACTION_SUSCRIPTIONS
    private void OnEnable()
    {
        MapManager.RecalculatePaths += RemoveMineFromList;
        Agent.OnDeposit += SaveGoldFromWorker;
    }
    private void OnDisable()
    {
        MapManager.RecalculatePaths -= RemoveMineFromList;
        Agent.OnDeposit -= SaveGoldFromWorker;
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
        goldStock = 0;
    }

    private void SaveGoldFromWorker(int gold) {
        goldStock += gold;
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

        if (routes.Count == 0) {
            Debug.Log("Rutas = 0");
        }
        foreach (List<Node> r in routes)
        {
            
            if (r.Count < shortestPath.Count)
            {
                shortestPath.Clear();
                shortestPath.AddRange(r);
                if (shortestPath == null) {
                    Debug.Log("Mas rompido");
                }
                workerManager.SetShortestPath(shortestPath);
            }

        }
        Node n = shortestPath[shortestPath.Count - 1];
        workerManager.SetClosestMine(n);
    }

    void RemoveMineFromList(Node n) {
        if (mineLocations.Contains(n)) {
            mineLocations.Remove(n);
        }
        FindNearestMine();
        workerManager.AssignMineToWorkers(MapManager.GetNode(new Vector2Int(n.mapPos.x, n.mapPos.y)).mineInNode);
        PathsCompleted?.Invoke(shortestPath);
    }

}
