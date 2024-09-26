using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrabFSM
{
    public enum Behaviours
    {
        MoveTowards, GatherResource, DepositInv, Idle, ReturnToTown, NeedsNewPath
    }

    public enum Flags
    {
        OnTargetReach, OnInvFull, OnInvEmpty
    }
}
public class CrabAgent : MonoBehaviour
{
    public static Action OnStartWork;
    public static Action OnFinishWork;
    public static Action<int> OnDeposit;
    public static Action OnMine;
    public static Action OnLeave;
    public static Action OnFamished;

    [SerializeField]int inventory;
    [SerializeField]int inventoryLimit;
   

    Behaviours emergencyStateSave;

    public MineImplement TGTMine;
 

    #region FSM_Parameters
    public FSM<Behaviours, Flags> fsm;
    [SerializeField] public List<Transform> waypointQueue;
    [SerializeField] public List<Transform> reverseQueue;
    public float speed;
    public float interactDistance;
    #endregion


    void Update()
    {
        if (fsm != null)
        {
            fsm.Tick();
        }
    }
    public void Init(List<Node> path)
    {
        SetNewPath(path);

        inventory = 0;
        inventoryLimit = 10;
    

        fsm = new FSM<Behaviours, Flags>();
        fsm.AddBehaviour<MovementStates.MoveTowardsWaypointState>(Behaviours.MoveTowards, onEnterParameters: () => { return new object[] { transform, speed, waypointQueue, interactDistance }; });
        fsm.AddBehaviour<MovementStates.ReturnToTownState>(Behaviours.ReturnToTown, onEnterParameters: () => { return new object[] { transform, speed, reverseQueue, interactDistance }; });
        fsm.AddBehaviour<MovementStates.NeedsNewPathState>(Behaviours.NeedsNewPath, onEnterParameters: () => { return new object[] { transform, speed, reverseQueue, interactDistance }; });
        fsm.AddBehaviour<WorkerInteractStates.GatherResource>(Behaviours.GatherResource);
        fsm.AddBehaviour<WorkerInteractStates.FamishedState>(Behaviours.Famished, onEnterParameters: () => { return new object[] { }; });
        fsm.AddBehaviour<WorkerInteractStates.DepositInvState>(Behaviours.DepositInv, onEnterParameters: () => { return new object[] {(Action)DepositResources }; });
        fsm.AddBehaviour<IdleState>(Behaviours.Idle);
       

        fsm.SetTransition(Behaviours.Idle, Flags.OnInvEmpty, Behaviours.MoveTowards);
        fsm.SetTransition(Behaviours.MoveTowards, Flags.OnTargetReach, Behaviours.GatherResource, () => { OnStartWork?.Invoke(); });
        fsm.SetTransition(Behaviours.GatherResource, Flags.OnInvFull, Behaviours.ReturnToTown, () => { OnFinishWork?.Invoke(); });
        fsm.SetTransition(Behaviours.GatherResource, Flags.OnMineDepleted, Behaviours.NeedsNewPath, () => { OnFinishWork?.Invoke(); });
        fsm.SetTransition(Behaviours.NeedsNewPath, Flags.OnTargetReach, Behaviours.MoveTowards, () => { OnFinishWork?.Invoke(); });

        

        fsm.SetTransition(Behaviours.ReturnToTown, Flags.OnTargetReach, Behaviours.DepositInv);
        fsm.SetTransition(Behaviours.DepositInv, Flags.OnInvEmpty, Behaviours.MoveTowards);

        fsm.SetTransition(Behaviours.GatherResource, Flags.OnHungry, Behaviours.Famished);
        fsm.SetTransition(Behaviours.Famished, Flags.OnEat, Behaviours.GatherResource);
        


        fsm.ForceState(Behaviours.MoveTowards);      
    }

    public void SetTargetMine(MineImplement MI) {
        TGTMine = MI;
    }

    public void SetNewPath(List<Node> path)
    {
        waypointQueue.Clear();
        reverseQueue.Clear();
        for (int i = 0; i < path.Count; i++)
        {
            waypointQueue.Add(path[i].transform);
        }
        
        for (int i = path.Count-1; i >= 0; i--) { 
            reverseQueue.Add(path[i].transform);
        }
        
    }

    private void OnEnable()
    {
        GameUISetup.AlarmRing += AlarmRung;
        GameUISetup.AlarmCancel += AlarmCanceled;
        TownImplement.PathsCompleted += SetNewPath;
        
    }

    private void OnDisable()
    {
        GameUISetup.AlarmRing -= AlarmRung;
        GameUISetup.AlarmCancel -= AlarmCanceled;
        TownImplement.PathsCompleted -= SetNewPath;
    }

    public void AlarmRung() {
        if (fsm.currentState != Convert.ToInt32(Behaviours.ReturnToTown))
        {
            //emergencyStateSave = (Behaviours)fsm.currentState;
        }
        fsm.ForceAbsoluteState(Behaviours.ReturnToTown);
    }
    public void AlarmCanceled() {
        fsm.ForceState(Behaviours.MoveTowards);
    }

    public void DepositResources() {
        OnDeposit?.Invoke(inventory);
        inventory = 0;
    }
    

}


