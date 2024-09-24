using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Behaviours
{
    MoveTowards, GatherResource, DepositInv, Idle, ReturnToTown
}

public enum Flags
{
    OnTargetReach, OnTargetNear,OnTargetLost, OnInvFull, OnInvEmpty
}
public class Agent : MonoBehaviour
{
    private FSM<Behaviours, Flags> fsm;
    [SerializeField] public List<Transform> waypointQueue;
    [SerializeField] public List<Transform> reverseQueue;
    public float speed;
    public float interactDistance;
  
    

    public void Init(List<Node> path)
    {
        fsm = new FSM<Behaviours, Flags>();
       

        SetNewPath(path);

       
        fsm.AddBehaviour<MoveTowardsWaypointState>(Behaviours.MoveTowards, onEnterParameters: () => { return new object[] { transform, speed, waypointQueue, interactDistance }; });
        fsm.AddBehaviour<ReturnToTownState>(Behaviours.ReturnToTown, onEnterParameters: () => { return new object[] { transform, speed, reverseQueue, interactDistance }; });
        fsm.AddBehaviour<GatherResource>(Behaviours.GatherResource, onTickParameters: () => { return new object[] { }; });
        fsm.AddBehaviour<DepositInvState>(Behaviours.DepositInv);

        fsm.SetTransition(Behaviours.Idle, Flags.OnInvEmpty, Behaviours.MoveTowards);
        fsm.SetTransition(Behaviours.MoveTowards, Flags.OnTargetReach, Behaviours.GatherResource);
        fsm.SetTransition(Behaviours.GatherResource, Flags.OnInvFull, Behaviours.ReturnToTown);
        fsm.SetTransition(Behaviours.ReturnToTown, Flags.OnTargetReach, Behaviours.DepositInv, () => { Debug.Log("Deposited light stones!"); });
        fsm.SetTransition(Behaviours.DepositInv, Flags.OnInvEmpty, Behaviours.MoveTowards);


        fsm.ForceState(Behaviours.MoveTowards);
        
    }


    void Update()
    {
        fsm.Tick();
    }

    public void SetNewPath(List<Node> path)
    { 
        for (int i = 0; i < path.Count; i++)
        {
            waypointQueue.Add(path[i].transform);
        }
        
        for (int i = path.Count-1; i >= 0; i--) { 
            reverseQueue.Add(path[i].transform);
        }
        //Debug.Log(reverseQueue.Count);
    }

}


