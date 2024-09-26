using UnityEngine;
using System;
using UnityEngine.Playables;
using System.IO;
using System.Collections.Generic;

public class Traveler : MonoBehaviour
{
    [SerializeField]private Agent agent;
 
    List<Node> path;
    List<Node> reversePath;
    public void InitWithPath(List<Node> map) {
        agent = GetComponent<Agent>();
        agent.Init(map);        
    }

    public void AssignTargetMine(MineImplement targetMine) { 

        agent.SetTargetMine(targetMine);
    }
    public void SetShortestPath(List<Node> route) { 
        agent.SetNewPath(route);
    }
    public void SetMidPath(List<Node> path) {
        agent.SetMidPath(path);
    }
}
