using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerManager : MonoBehaviour
{
    public static Action<bool> OnReturnToBaseCalled;
    public bool returnToBase = false;
    List<Traveler> workers;

    [SerializeField] private VoronoiHandler voronoiHandler;
    [SerializeField] private GameObject workerPrefab;
    [SerializeField] private GameObject caravanPrefab;
    public List<Node> shortestPath;

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


    public void CreateWorker()
    {
        GameObject workerInstance = Instantiate(workerPrefab, this.transform.position, Quaternion.identity);
        Traveler travelerScript = workerInstance.GetComponent<Traveler>();
        travelerScript.InitWithPath(shortestPath);
        AddWorkerToList(travelerScript);
    }

    private void OnEnable()
    {
        Agent.OnStartWork += ()=> { };
    }
    private void OnDisable()
    {
        Agent.OnStartWork -= ()=> { };
    }
}
