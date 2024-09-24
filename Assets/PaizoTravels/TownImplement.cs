using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TownImplement : MonoBehaviour
{
    [SerializeField] GameObject workerPrefab;
    [SerializeField] GameObject caravanPrefab;

    public TownCenter str;
    List<Node> shortestPath;
    Pathfinding scout;
    void Start()
    {
        str = new TownCenter();
        scout = new Pathfinding();
        FindNearestMine();
    }


    public void CreateAgent()
    {
        GameObject agentInstance = Instantiate(workerPrefab, this.transform.position, Quaternion.identity);
        Traveler travelerScript = agentInstance.GetComponent<Traveler>();
        travelerScript.InitWithPath(shortestPath);        
    }

    public void CreateAgentDebug() { 

        GameObject agentInstance = Instantiate(workerPrefab, this.transform.position, Quaternion.identity);
        Traveler travelerScript = agentInstance.GetComponent<Traveler>();

       
        //travelerScript.Init();
    }

    void FindNearestMine() {
        
        shortestPath = new List<Node>();
        List<Node> path;
        
        foreach (Node mine in str.mineLocations)
        {
            if (scout.AStar(str.location, mine, out path)){
                if (path.Count < shortestPath.Count)
                {
                    shortestPath = path;
                    
                }
            }
        }
    }

}
