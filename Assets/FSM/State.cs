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

namespace MovementStates
{
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

    public sealed class ReturnToTownState : State
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
}
namespace WorkerInteractStates
{
    public sealed class GatherResource : State
    {
        Func<bool> minegold;
        bool result = false;
        Func<bool> increasehunger;
        bool isHungry = false;
        
        public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
        {
            BehaviourActions behaviours = new BehaviourActions();
            
            Agent.OnStartWork?.Invoke();
            minegold = parameters[0] as Func<bool>;
            increasehunger = parameters[1] as Func<bool>;
         

            return behaviours;
        }

        public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
        {
            return default;
        }

        public override BehaviourActions GetTickBehaviours(params object[] parameters)
        {
            BehaviourActions behaviours = new BehaviourActions();
            behaviours.AddMainThreadBehaviours(0, () => {

                if ((isHungry = increasehunger.Invoke()) == false)
                    result = minegold.Invoke();
                
            });

            behaviours.SetTransitionBehaviour(() =>
            {
                if (isHungry)
                {
                    OnFlag?.Invoke(Flags.OnHungry);
                    
                }

                if (result)
                {
                    OnFlag?.Invoke(Flags.OnInvFull);
                    Agent.OnFinishWork?.Invoke();
                }

            });

            return behaviours;
        }
    }

    public sealed class DepositInvState : State
    {
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
                Agent.OnDeposit?.Invoke();
                OnFlag?.Invoke(Flags.OnInvEmpty);
            });

            return behaviours;
        }


    }

    public sealed class FamishedState : State {
        Action RetrieveFood;
        Action EatFood;
        public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
        {
            BehaviourActions behaviours = new BehaviourActions();
            RetrieveFood = parameters[0] as Action;
            EatFood = parameters[1] as Action;
            
            return behaviours;
        }

        public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
        {
            return default;
        }

        public override BehaviourActions GetTickBehaviours(params object[] parameters)
        {
            BehaviourActions behaviours = new BehaviourActions();

            behaviours.AddMainThreadBehaviours(0, () => {                
                RetrieveFood?.Invoke();
            });

            behaviours.SetTransitionBehaviour(() =>
            {
                OnFlag?.Invoke(Flags.OnEat);
            });

            return behaviours;
        }

    }
}
