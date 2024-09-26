using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerManager : MonoBehaviour
{
    public static Action<bool> OnReturnToBaseCalled;
    public static Action<WorkerManager> OnInit;
    public bool returnToBase = false;

    public List<Traveler> workers;
    public List<CrabTraveler> crabs;

    [SerializeField] public VoronoiHandler WorkerVoronoiHandler;
    [SerializeField] public VoronoiHandler CrabVoronoiHandler;
    [SerializeField] private GameObject workerPrefab;
    [SerializeField] private GameObject crabPrefab;
    public List<Node> shortestPath;
    public List<Node> availableRoad;
    private MineImplement closestMine;

    private void Start()
    {
        workers = new List<Traveler>();
    }

    public void SetReturnToBase() { 
        returnToBase = !returnToBase;
        OnReturnToBaseCalled(returnToBase);
    }

    public void AddWorkerToList(Traveler traveler) { 
        workers.Add(traveler);
    }
    public void AddCrabToList(CrabTraveler crab) { 
        crabs.Add(crab);
    }

    public void AssignMineToWorkers(MineImplement m) {
        foreach (var worker in workers)
        {
            worker.AssignTargetMine(m);
        }

    }
    public void CreateWorker()
    {
        GameObject workerInstance = Instantiate(workerPrefab, this.transform.position, Quaternion.identity);
        Traveler travelerScript = workerInstance.GetComponent<Traveler>();
        travelerScript.InitWithPath(shortestPath);
        AddWorkerToList(travelerScript);
        travelerScript.AssignTargetMine(closestMine);
    }

    public void CreateCrab() { 
        GameObject crabInstance = Instantiate(crabPrefab, this.transform.position, Quaternion.identity);
        CrabTraveler crabScript = crabInstance.GetComponent<CrabTraveler>();
        crabScript.InitWithPath(availableRoad);
        AddCrabToList(crabScript);
        crabScript.AssignTargetMine(closestMine);        
    }

    public void SetClosestMine(Node n) {
        closestMine = MapManager.GetNode(new Vector2Int(n.mapPos.x, n.mapPos.y)).mineInNode;
    }
    public void SetShortestPath(List<Node> sp) { 
        shortestPath.Clear();
        shortestPath.AddRange(sp);
        foreach (var worker in workers) {
            worker.SetShortestPath(sp);
        }
        Node n = MapManager.GetNode(new Vector2Int(sp[sp.Count-1].mapPos.x, sp[sp.Count-1].mapPos.y));
        SetClosestMine(n);
        AssignMineToWorkers(n.mineInNode);
    }

    public void SetAvailableRoad(List<Node> ar) {
        availableRoad.Clear();
        availableRoad.AddRange(ar);
        foreach (var crab in crabs)
        {
            crab.SetShortestPath(ar);
        }
        Node n = MapManager.GetNode(new Vector2Int(ar[ar.Count - 1].mapPos.x, ar[ar.Count - 1].mapPos.y));
        SetClosestMine(n);
        
    }

    private void OnEnable()
    {
        //Agent.OnStartWork += ()=> { };
        
    }
    private void OnDisable()
    {
        //Agent.OnStartWork -= ()=> { };
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) {
            CreateWorker();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            CreateCrab();
        }
    }

    public void SetVoronoiManager(VoronoiHandler worker, VoronoiHandler crab) { 
        WorkerVoronoiHandler = worker;
        CrabVoronoiHandler = crab;
    }
    
}
