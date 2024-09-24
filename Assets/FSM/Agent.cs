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

    public Transform target;
    public Transform town;
    [SerializeField]public List<Transform> waypointQueue;
    public float speed;
    public float interactDistance;
  
    

    public void Init(List<Node> path)
    {
        fsm = new FSM<Behaviours, Flags>();

        SetNewPath(path);

        //fsm.AddBehaviour<MoveTowardsState>(Behaviours.MoveTowards, onTickParameters: () => { return new object[] { transform, target, speed, interactDistance }; });
        fsm.AddBehaviour<MoveTowardsWaypointState>(Behaviours.MoveTowards, onTickParameters: () => { return new object[] { transform, speed, waypointQueue, interactDistance }; });


        fsm.AddBehaviour<ReturnToTownState>(Behaviours.ReturnToTown, onTickParameters: () => { return new object[] { transform, town, speed, interactDistance }; });
        fsm.AddBehaviour<GatherResource>(Behaviours.GatherResource);
        fsm.AddBehaviour<DepositInvState>(Behaviours.DepositInv);

        fsm.SetTransition(Behaviours.Idle, Flags.OnInvEmpty, Behaviours.MoveTowards);
        fsm.SetTransition(Behaviours.MoveTowards, Flags.OnTargetReach, Behaviours.GatherResource);

        //fsm.SetTransition(Behaviours.Idle, Flags.OnInvEmpty,Behaviours.MoveTowards);
        //fsm.SetTransition(Behaviours.MoveTowards, Flags.OnTargetReach, Behaviours.GatherResource);
        fsm.SetTransition(Behaviours.GatherResource, Flags.OnInvFull, Behaviours.ReturnToTown);
        fsm.SetTransition(Behaviours.ReturnToTown, Flags.OnTargetReach, Behaviours.DepositInv, () => { Debug.Log("Deposited light stones!"); });
        //fsm.SetTransition(Behaviours.DepositInv, Flags.OnInvEmpty, Behaviours.MoveTowards);


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
    }

}


