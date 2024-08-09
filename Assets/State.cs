using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    public Action<int> OnFlag;

    public abstract List<Action> GetTickBehaviours(params object[] parameters);
    public abstract List<Action> GetOnEnterBehaviours(params object[] parameters);
    public abstract List<Action> GetOnExitBehaviours(params object[] parameters);

    public abstract void Transition(int flag);
    //public Action<int> OnTransition;
    //public abstract void OnEnter(params object[] param);
    //public abstract void OnExit(params object[] param);
    //public abstract void Perform(params object[] parameters);
}


public sealed class ChaseState : State
{
    public override List<Action> GetOnEnterBehaviours(params object[] parameters)
    {
        return new List<Action>();
    }

    public override List<Action> GetOnExitBehaviours(params object[] parameters)
    {
        return new List<Action>();
    }

    public override List<Action> GetTickBehaviours(params object[] parameters)
    {
        Transform OwnerTransform = parameters[0] as Transform;
        Transform TargetTransform = parameters[1] as Transform;
        float speed = Convert.ToSingle(parameters[2]);
        float explodeDistance = Convert.ToSingle(parameters[3]);
        float lostDistance = Convert.ToSingle(parameters[4]);

        List<Action> behaviours = new List<Action>();
        behaviours.Add(() =>
        {
            OwnerTransform.position += ((TargetTransform.position - OwnerTransform.position).normalized * speed * Time.deltaTime);
        });

        behaviours.Add(() =>
        {
            Debug.Log("Ey boss!");
        });

        behaviours.Add(() =>
        {
            //if (Vector3.Distance(TargetTransform.position - OwnerTransform.position) < explodeDistance)
            //{
            //    OnFlag?.Invoke((int));
            //}
        });

        // ()=>{} Esta villereada es una expresion lambda. Es una funcion anonima que puedo pasar como parametro o action 

        return behaviours;


    }

    public override void Transition(int flag)
    {
        throw new NotImplementedException();
    }
}

public sealed class PatrolState : State
{
    public override List<Action> GetOnEnterBehaviours(params object[] parameters)
    {
        return new List<Action>();
    }

    public override List<Action> GetOnExitBehaviours(params object[] parameters)
    {
        return new List<Action>();
    }

    public override List<Action> GetTickBehaviours(params object[] parameters)
    {
        return new List<Action>();
    }

    public override void Transition(int flag)
    {
        throw new NotImplementedException();
    }
}

public sealed class ExplodeState : State
{
    public override List<Action> GetOnEnterBehaviours(params object[] parameters)
    {
        return new List<Action>();
    }

    public override List<Action> GetOnExitBehaviours(params object[] parameters)
    {
        return new List<Action>();
    }

    public override List<Action> GetTickBehaviours(params object[] parameters)
    {
        return new List<Action>();
    }

    public override void Transition(int flag)
    {
        throw new NotImplementedException();
    }
}

