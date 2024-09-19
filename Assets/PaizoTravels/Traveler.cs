using UnityEngine;
using System;
using UnityEngine.Playables;
using System.IO;
using System.Collections.Generic;

public class Traveler : MonoBehaviour
{
    [SerializeField]int inventory;
    private Action something;
    private Agent agent;
    private Pathfinding scout;
    List<Node> path;
    public Node way1;
    public Node way2;

   
    public void Init(Node w1, Node w2)
    {
        agent = new Agent();
        scout = new Pathfinding();
        agent.Init();

        if(scout.AStar(way1, way2, out path))
        agent.SetNewPath(path);
        
    }

    
}

public class Matoran : Traveler 
{
    
}

public class UssalCrab : Traveler
{ 

}