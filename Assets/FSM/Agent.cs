using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Behaviours
{
    MoveTowards, GatherResource, DepositInv, Idle, ReturnToTown, Famished, NeedsNewPath
}

public enum Flags
{
    OnTargetReach, OnTargetNear,OnTargetLost, OnInvFull, OnInvEmpty, OnHungry, OnEat, OnMineDepleted
}

public class Agent : MonoBehaviour
{
    public static Action OnStartWork;
    public static Action<MineImplement> ProvideMineData;
    public static Action OnFinishWork;
    public static Action<int> OnDeposit;
    public static Action OnMine;
    public static Action OnLeave;
    public static Action OnFamished;

    [SerializeField]int inventory;
    [SerializeField]int hunger; 
    [SerializeField]int lunchbox;
    [SerializeField]int hungerLimit;
    [SerializeField]int inventoryLimit;
    [SerializeField]int lunchboxLimit;

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


        hunger = 0;
        hungerLimit = 3;
        inventory = 0;
        inventoryLimit = 15;
        lunchboxLimit = 6;
        lunchbox = lunchboxLimit;

        fsm = new FSM<Behaviours, Flags>();
        fsm.AddBehaviour<MovementStates.MoveTowardsWaypointState>(Behaviours.MoveTowards, onEnterParameters: () => { return new object[] { transform, speed, waypointQueue, interactDistance }; });
        fsm.AddBehaviour<MovementStates.ReturnToTownState>(Behaviours.ReturnToTown, onEnterParameters: () => { return new object[] { transform, speed, reverseQueue, interactDistance }; });
        fsm.AddBehaviour<MovementStates.NeedsNewPathState>(Behaviours.NeedsNewPath, onEnterParameters: () => { return new object[] { transform, speed, reverseQueue, interactDistance }; });
        fsm.AddBehaviour<WorkerInteractStates.GatherResource>(Behaviours.GatherResource, onEnterParameters: () => { return new object[] { (Func<MineResult>)MineGold, (Func<bool>)IncreaseHunger}; });
        fsm.AddBehaviour<WorkerInteractStates.FamishedState>(Behaviours.Famished, onEnterParameters: () => { return new object[] { (Action)RetrieveFood, (Action)EatFood}; });
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
        OnStartWork += () => { ProvideMineData?.Invoke(TGTMine); };
        GameUISetup.AlarmRing += AlarmRung;
        GameUISetup.AlarmCancel += AlarmCanceled;
        TownImplement.PathsCompleted += SetNewPath;
        
    }

    private void OnDisable()
    {
        //OnStartWork -= () => { };
        GameUISetup.AlarmRing -= AlarmRung;
        GameUISetup.AlarmCancel -= AlarmCanceled;
        TownImplement.PathsCompleted -= SetNewPath;
    }


    public enum MineResult { 
        InvFull = 1,
        NoGold =2,
        Success =3
    }
    public MineResult MineGold(){

        if (inventory >= inventoryLimit)
            return MineResult.InvFull;

        if (TGTMine.goldResource <= 0)
            return MineResult.NoGold;
        
            TGTMine.GetMined();
            inventory++;
            return MineResult.Success;        
    }
    public bool IncreaseHunger() {
        if (hunger < hungerLimit)
        {
            hunger++;
            return false;
        }
        if (hunger == hungerLimit && lunchbox > 0)
        {
            Debug.Log("Eating food");
            EatFood();
            return true;
        }
        if (lunchbox == 0) {
            Debug.Log("Getting food from mine");
            RetrieveFood();
        }
        
        return true;
       
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
            lunchbox--;        
            hunger = 0;
        }
        if (lunchbox == 0)
            RetrieveFood();
        
    }

    public void AlarmRung() {
        if (fsm.currentState != Convert.ToInt32(Behaviours.ReturnToTown))
        {
            //emergencyStateSave = (Behaviours)fsm.currentState;
            fsm.ForceAbsoluteState(Behaviours.ReturnToTown);
        }
    }
    public void AlarmCanceled() {
        fsm.ForceState(Behaviours.MoveTowards);
    }

    public void DepositResources() {
        OnDeposit?.Invoke(inventory);
        inventory = 0;
    }
    

}


