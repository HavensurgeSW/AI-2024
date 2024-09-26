using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabTraveler : MonoBehaviour
{
    [SerializeField] private CrabAgent agent;

    List<Node> path;
    List<Node> reversePath;
    public void InitWithPath(List<Node> map)
    {
        agent = GetComponent<CrabAgent>();
        agent.Init(map);
    }

    public void AssignTargetMine(MineImplement targetMine)
    {

        agent.SetTargetMine(targetMine);
    }
    public void SetShortestPath(List<Node> route)
    {
        agent.SetNewPath(route);
    }
}
