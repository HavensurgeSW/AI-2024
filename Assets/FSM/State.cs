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

//public sealed class ChaseState : State
//{
//    public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
//    {
//        return default;
//    }

//    public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
//    {
//        return default;
//    }

//    public override BehaviourActions GetTickBehaviours(params object[] parameters)
//    {
//        Transform OwnerTransform = parameters[0] as Transform;
//        Transform TargetTransform = parameters[1] as Transform;
//        float speed = Convert.ToSingle(parameters[2]);
//        float explodeDistance = Convert.ToSingle(parameters[3]);
//        float lostDistance = Convert.ToSingle(parameters[4]);

//        BehaviourActions behaviour = new BehaviourActions();

//        behaviour.AddMainThreadBehaviours(0, () =>
//        {
//            OwnerTransform.position += (TargetTransform.position - OwnerTransform.position).normalized * speed * Time.deltaTime;
//        });
//        behaviour.AddMultithreadableBehaviours(0, () =>
//        {
//            Debug.Log("Whistle!");
//        });
//        behaviour.SetTransitionBehaviour(() =>
//        {
//            if (Vector3.Distance(TargetTransform.position, OwnerTransform.position) < explodeDistance)
//            {
//                OnFlag?.Invoke(Flags.OnTargetReach);
//            }
//            else if (Vector3.Distance(TargetTransform.position, OwnerTransform.position) > lostDistance)
//            {
//                OnFlag?.Invoke(Flags.OnTargetLost);
//            }
//        });

//        return behaviour;
//    }
//}

//public sealed class PatrolState : State
//{
//    private Transform actualTrget;

//    public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
//    {
//        return default;
//    }

//    public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
//    {
//        return default;
//    }

//    public override BehaviourActions GetTickBehaviours(params object[] parameters)
//    {
//        Transform ownerTransform = parameters[0] as Transform;
//        Transform wayPoint1 = parameters[1] as Transform;
//        Transform wayPoint2 = parameters[2] as Transform;
//        Transform chaseTarget = parameters[3] as Transform;
//        float speed = Convert.ToSingle(parameters[4]);
//        float chaseDistance = Convert.ToSingle(parameters[5]);

//        BehaviourActions behaviours = new BehaviourActions();

//        behaviours.AddMultithreadableBehaviours(0, () =>
//        {
//            if (actualTrget == null)
//            {
//                actualTrget = wayPoint1;
//            }

//            if (Vector3.Distance(ownerTransform.position, actualTrget.position) < 0.2f)
//            {
//                if (actualTrget == wayPoint1)
//                    actualTrget = wayPoint2;
//                else
//                    actualTrget = wayPoint1;
//            }
//        });

//        behaviours.AddMainThreadBehaviours(1, () =>
//        {
//            ownerTransform.position += (actualTrget.position - ownerTransform.position).normalized * speed * Time.deltaTime;
//        });

//        behaviours.SetTransitionBehaviour(() =>
//        {
//            if (Vector3.Distance(ownerTransform.position, chaseTarget.position) < chaseDistance)
//            {
//                OnFlag?.Invoke(Flags.OnTargetNear);
//            }
//        });
//        return behaviours;
//    }

//}

//public sealed class ExplodeState : State
//{
//    public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
//    {
//        BehaviourActions behaviours = new BehaviourActions();
//        behaviours.AddMultithreadableBehaviours(0, () => { Debug.Log("boom"); });
//        return behaviours;
//    }

//    public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
//    {
//        return default;
//    }

//    public override BehaviourActions GetTickBehaviours(params object[] parameters)
//    {
//        BehaviourActions behaviours = new BehaviourActions();
//        behaviours.AddMultithreadableBehaviours(0, () => { Debug.Log("F"); });
//        return behaviours;
//    }

//}

public sealed class GatherResource : State
{
    public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
    {
        BehaviourActions behaviours = new BehaviourActions();
        behaviours.AddMultithreadableBehaviours(0, () => { Debug.Log("Gathering"); });
        return behaviours;
    }

    public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
    {
        return default;
    }

    public override BehaviourActions GetTickBehaviours(params object[] parameters)
    {
        BehaviourActions behaviours = new BehaviourActions();
        behaviours.AddMultithreadableBehaviours(0, () => { Debug.Log("Gold +1"); });

        behaviours.SetTransitionBehaviour(() =>
        {
                Debug.Log("Inventory full!");
                OnFlag?.Invoke(Flags.OnInvFull);       
        });

        return behaviours;
    }
}

public sealed class MoveTowardsState : State
{
    public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
    {
        BehaviourActions behaviours = new BehaviourActions();
        behaviours.AddMultithreadableBehaviours(0, () => { Debug.Log("Moving towards target"); });


        return behaviours;
    }

    public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
    {
        return default;
    }

    public override BehaviourActions GetTickBehaviours(params object[] parameters)
    {

        Transform ownerTransform = parameters[0] as Transform;
        Transform targetTransform = parameters[1] as Transform;
        float speed = Convert.ToSingle(parameters[2]);
        float distanceToTGT = Convert.ToSingle(parameters[3]);

        BehaviourActions behaviours = new BehaviourActions();
        //behaviours.AddMultithreadableBehaviours(0, () => { Debug.Log("Moving..."); });
        behaviours.AddMainThreadBehaviours(0, () => {
            ownerTransform.position = Vector3.MoveTowards(ownerTransform.position, targetTransform.position, speed * Time.deltaTime);    
        });


        behaviours.SetTransitionBehaviour(() =>
        {
            if (Vector3.Distance(ownerTransform.position, targetTransform.position) < distanceToTGT)
            {
                OnFlag?.Invoke(Flags.OnTargetReach);
            }
        });

        return behaviours;
    }
}

public sealed class ReturnToTownState : State {
    public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
    {
        BehaviourActions behaviours = new BehaviourActions();
        behaviours.AddMultithreadableBehaviours(0, () => { Debug.Log("Moving back to OnuKoro"); });
        return behaviours;
    }

    public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
    {
        return default;
    }

    public override BehaviourActions GetTickBehaviours(params object[] parameters)
    {
        Transform ownerTransform = parameters[0] as Transform;
        Transform targetTransform = parameters[1] as Transform;
        float speed = Convert.ToSingle(parameters[2]);
        float distanceToTGT = Convert.ToSingle(parameters[3]);

        BehaviourActions behaviours = new BehaviourActions();
        behaviours.AddMultithreadableBehaviours(0, () => { Debug.Log("Moving..."); });
        behaviours.AddMainThreadBehaviours(0, () =>
        {
            ownerTransform.position = Vector3.MoveTowards(ownerTransform.position, targetTransform.position, speed * Time.deltaTime);
        });

        behaviours.SetTransitionBehaviour(() =>
        {
            if (Vector3.Distance(ownerTransform.position, targetTransform.position) < distanceToTGT)
            {
                OnFlag?.Invoke(Flags.OnTargetReach);
            }
        });

        return behaviours;
    }
}
