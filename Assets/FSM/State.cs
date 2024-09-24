using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public struct BehaviourActions
{
    private Dictionary<int, List<Action>> mainThreadBehaviours;
    private ConcurrentDictionary<int, ConcurrentBag<Action>> multithreadablesBehaviours;
    private Action transitionBehaviour;

    public void AddMainThreadBehaviours(int executionOrder, Action behaviour)
    {
        if (mainThreadBehaviours == null)
            mainThreadBehaviours = new Dictionary<int, List<Action>>();

        if (!mainThreadBehaviours.ContainsKey(executionOrder))
            mainThreadBehaviours.Add(executionOrder, new List<Action>());

        mainThreadBehaviours[executionOrder].Add(behaviour);
    }

    public void AddMultithreadableBehaviours(int executionOrder, Action behaviour)
    {
        if (multithreadablesBehaviours == null)
            multithreadablesBehaviours = new ConcurrentDictionary<int, ConcurrentBag<Action>>();

        if (!multithreadablesBehaviours.ContainsKey(executionOrder))
            multithreadablesBehaviours.TryAdd(executionOrder, new ConcurrentBag<Action>());

        multithreadablesBehaviours[executionOrder].Add(behaviour);
    }

    public void SetTransitionBehaviour(Action behaviour)
    {
        transitionBehaviour = behaviour;
    }

    public Dictionary<int, List<Action>> MainThreadBehaviours => mainThreadBehaviours;
    public ConcurrentDictionary<int, ConcurrentBag<Action>> MultithreadablesBehaviours => multithreadablesBehaviours;
    public Action TransitionBehaviour => transitionBehaviour;
}
public abstract class State
{
    public Action<Enum> OnFlag;
    public abstract BehaviourActions GetOnEnterBehaviours(params object[] parameters);
    public abstract BehaviourActions GetTickBehaviours(params object[] parameters);
    public abstract BehaviourActions GetOnExitBehaviours(params object[] parameters);
}


public sealed class GatherResource : State
{
    public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
    {
        BehaviourActions behaviours = new BehaviourActions();
        //behaviours.AddMultithreadableBehaviours(0, () => { Debug.Log("Gathering"); });
       
        return behaviours;
    }

    public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
    {
        return default;
    }

    public override BehaviourActions GetTickBehaviours(params object[] parameters)
    {
        BehaviourActions behaviours = new BehaviourActions();
        behaviours.AddMainThreadBehaviours(0, () => { Debug.Log("Gold +1"); });

        behaviours.SetTransitionBehaviour(() =>
        {
                Debug.Log("Inventory full!");
                OnFlag?.Invoke(Flags.OnInvFull);       
        });

        return behaviours;
    }
}

public sealed class MoveTowardsWaypointState : State
{ 
    private Queue<Transform> way;
    private Transform currentTarget;
    private Transform ownerTransform;
    float speed;
    float distanceToTGT;

    public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
    {
        BehaviourActions behaviours = new BehaviourActions();

        way = new Queue<Transform>();
        ownerTransform = parameters[0] as Transform;
        List<Transform> waypoints = new List<Transform>();
        waypoints = parameters[2] as List<Transform>;
        for (int i = 0; i < waypoints.Count; i++)
        {
            way.Enqueue(waypoints[i]);
        }
        speed = Convert.ToSingle(parameters[1]);
        distanceToTGT = Convert.ToSingle(parameters[3]);

        currentTarget = way.Peek();

        return behaviours;
    }

    public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
    {
        return default;
    }

    public override BehaviourActions GetTickBehaviours(params object[] parameters)
    {
        BehaviourActions behaviours = new BehaviourActions();
             
        //behaviours.AddMultithreadableBehaviours(0, () => { Debug.Log("Moving..."); });
        behaviours.AddMainThreadBehaviours(0, () =>
        {
            if (currentTarget != null)
            {
                ownerTransform.position = Vector3.MoveTowards(ownerTransform.position, currentTarget.position, speed * Time.deltaTime);
            }
        });
        behaviours.SetTransitionBehaviour(() =>
        {
            
            if (Vector3.Distance(ownerTransform.position, currentTarget.position) < distanceToTGT)
            {
                               
                if (way.Count > 0)
                {
                    way.Dequeue();
                    Transform tempTry;
                    
                    if (way.TryPeek(out tempTry))
                    {
                        currentTarget = way.Peek();
                    }

                }
                else
                {
                    Debug.Log("Target reached!");
                    OnFlag?.Invoke(Flags.OnTargetReach);
                }
                
            }
        });

        return behaviours;
    }
}

public sealed class ReturnToTownState : State {
    private Queue<Transform> way;
    private Transform currentTarget;
    private Transform ownerTransform;
    float speed;
    float distanceToTGT;

    public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
    {
        BehaviourActions behaviours = new BehaviourActions();

        way = new Queue<Transform>();
        ownerTransform = parameters[0] as Transform;
        List<Transform> waypoints = new List<Transform>();
        waypoints = parameters[2] as List<Transform>;
        for (int i = 0; i < waypoints.Count; i++)
        {
            way.Enqueue(waypoints[i]);
        }
        speed = Convert.ToSingle(parameters[1]);
        distanceToTGT = Convert.ToSingle(parameters[3]);

        currentTarget = way.Peek();

        return behaviours;
    }

    public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
    {
        return default;
    }

    public override BehaviourActions GetTickBehaviours(params object[] parameters)
    {
        BehaviourActions behaviours = new BehaviourActions();

        //behaviours.AddMultithreadableBehaviours(0, () => { Debug.Log("Moving..."); });
        behaviours.AddMainThreadBehaviours(0, () =>
        {
            if (currentTarget != null)
            {
                ownerTransform.position = Vector3.MoveTowards(ownerTransform.position, currentTarget.position, speed * Time.deltaTime);
            }
        });
        behaviours.SetTransitionBehaviour(() =>
        {

            if (Vector3.Distance(ownerTransform.position, currentTarget.position) < distanceToTGT)
            {

                if (way.Count > 0)
                {
                    way.Dequeue();
                    Transform tempTry;

                    if (way.TryPeek(out tempTry))
                    {
                        currentTarget = way.Peek();
                    }

                }
                else
                {
                    Debug.Log("Target reached!");
                    OnFlag?.Invoke(Flags.OnTargetReach);
                }

            }
        });

        return behaviours;
    }
}

public sealed class DepositInvState : State {
    public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
    {
        BehaviourActions behaviours = new BehaviourActions();
        behaviours.AddMultithreadableBehaviours(0, () => { Debug.Log("Returned to Onu-Koro"); });
        return behaviours;
    }

    public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
    {
        return default;
    }

    public override BehaviourActions GetTickBehaviours(params object[] parameters)
    {
        BehaviourActions behaviours = new BehaviourActions();
        

        behaviours.SetTransitionBehaviour(() =>
        {
            Debug.Log("Stored light stones!");
            OnFlag?.Invoke(Flags.OnInvEmpty);
        });

        return behaviours;
    }


}
