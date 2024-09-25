using UnityEngine;
using System;
using UnityEngine.Playables;
using System.IO;
using System.Collections.Generic;

public class Traveler : MonoBehaviour
{
    [SerializeField]private Agent agent;
    private Pathfinding scout;
    List<Node> path;
    List<Node> reversePath;
    public void InitWithPath(List<Node> map) {
        scout = new Pathfinding();
        agent = GetComponent<Agent>();
        agent.Init(map);        
    }

    public void AssignTargetMine(MineImplement targetMine) { 
        
    }
}
