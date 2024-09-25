using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Behaviours
{
    MoveTowards, GatherResource, DepositInv, Idle, ReturnToTown, Famished
}

public enum Flags
{
    OnTargetReach, OnTargetNear,OnTargetLost, OnInvFull, OnInvEmpty, OnHungry, OnEat
}

public class Agent : MonoBehaviour
{
    public static Action OnStartWork;
    public static Action OnFinishWork;
    public static Action OnDeposit;
    public static Action OnMine;
    public static Action OnLeave;
    public static Action OnFamished;

    int inventory;
    int hunger;
    int lunchbox;
    int hungerLimit;
    int inventoryLimit;
    int lunchboxLimit;

    MineImplement TGTMine;
 

    #region FSM_Parameters
    public FSM<Behaviours, Flags> fsm;
    [SerializeField] public List<Transform> waypointQueue;
    [SerializeField] public List<Transform> reverseQueue;
    public float speed;
    public float interactDistance;
    #endregion


    void Update()
    {
        fsm.Tick();
    }

    public void Init(List<Node> path)
    {
        fsm = new FSM<Behaviours, Flags>();

        SetNewPath(path);
        hunger = 0;
        hungerLimit = 3;
        inventory = 0;
        inventoryLimit = 15;
        lunchboxLimit = 6;
        lunchbox = lunchboxLimit;
        
        fsm.AddBehaviour<MovementStates.MoveTowardsWaypointState>(Behaviours.MoveTowards, onEnterParameters: () => { return new object[] { transform, speed, waypointQueue, interactDistance }; });
        fsm.AddBehaviour<MovementStates.ReturnToTownState>(Behaviours.ReturnToTown, onEnterParameters: () => { return new object[] { transform, speed, reverseQueue, interactDistance }; });
        fsm.AddBehaviour<WorkerInteractStates.GatherResource>(Behaviours.GatherResource, onEnterParameters: () => { return new object[] { (Func<bool>)MineGold, (Func<bool>)IncreaseHunger}; });
        fsm.AddBehaviour<WorkerInteractStates.FamishedState>(Behaviours.Famished, onEnterParameters: () => { return new object[] { (Action)RetrieveFood, (Action)EatFood}; });
        fsm.AddBehaviour<WorkerInteractStates.DepositInvState>(Behaviours.DepositInv);

        fsm.SetTransition(Behaviours.Idle, Flags.OnInvEmpty, Behaviours.MoveTowards);
        fsm.SetTransition(Behaviours.MoveTowards, Flags.OnTargetReach, Behaviours.GatherResource, () => { OnStartWork?.Invoke(); });
        fsm.SetTransition(Behaviours.GatherResource, Flags.OnInvFull, Behaviours.ReturnToTown, () => { OnFinishWork?.Invoke(); });
        fsm.SetTransition(Behaviours.ReturnToTown, Flags.OnTargetReach, Behaviours.DepositInv, () => { OnDeposit?.Invoke(); });
        fsm.SetTransition(Behaviours.DepositInv, Flags.OnInvEmpty, Behaviours.MoveTowards);

        fsm.SetTransition(Behaviours.GatherResource, Flags.OnHungry, Behaviours.Famished);
        fsm.SetTransition(Behaviours.Famished, Flags.OnEat, Behaviours.GatherResource);
        


        fsm.ForceState(Behaviours.MoveTowards);
        
        
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
        
    }


    public bool MineGold(){
        if (inventory >= inventoryLimit &&TGTMine.goldResource<=0)
            return true;
        else{ 
            TGTMine.GetMined();
            inventory++;
            OnMine?.Invoke();
            return false;
        } 
    }
    public bool IncreaseHunger() {
        if (hunger < hungerLimit)
        {
            hunger++;
            return false;
        }
        else return true;
       
    }

    public void RetrieveFood() {
        for (int i = 0; i < (lunchboxLimit - lunchbox) ; i++)
        {
            if (TGTMine.foodStorage == 0)
                break;

            lunchbox++;
            TGTMine.foodStorage--;
        }
    }

    public void EatFood() {
        if (lunchbox > 0) {
            hunger = 0;
            lunchbox--;        
        }
    }

}


