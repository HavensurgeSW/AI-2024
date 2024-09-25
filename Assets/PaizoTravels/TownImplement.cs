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
    public void Init()
    {
        str = new TownCenter();
        scout = new Pathfinding();
        mineLocations = new List<Node>();
        shortestPath = new List<Node>();
        OnInit?.Invoke(this);

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
    public void CreateWorker()
    {
        GameObject workerInstance = Instantiate(workerPrefab, this.transform.position, Quaternion.identity);
        Traveler travelerScript = workerInstance.GetComponent<Traveler>();
        travelerScript.InitWithPath(shortestPath);    
        workerManager.AddWorkerToList(travelerScript);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) {
            Debug.Log(shortestPath.Count);
            CreateWorker();
        }
    }

}
