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

    [SerializeField] private VoronoiHandler voronoiHandler;
    [SerializeField] private GameObject workerPrefab;
    [SerializeField] private GameObject caravanPrefab;
    public List<Node> shortestPath;
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
    }
}
