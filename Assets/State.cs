using System;
using UnityEngine;

public abstract class State
{
    public Action<int> OnTransition;
    public abstract void OnEnter(params object[] param);
    public abstract void OnExit(params object[] param);
    public abstract void Perform(params object[] parameters);
}
public sealed class Gathering : State
{

    public override void Perform(params object[] parameters)
    {
        throw new System.NotImplementedException();
        OnTransition?.Invoke((int)Flags.InventoryFull);
    }
    public override void OnEnter(params object[] param)
    {
        throw new NotImplementedException();
    }
    public override void OnExit(params object[] param)
    {
        throw new NotImplementedException();
    }
}

public sealed class Moving : State
{
    public override void Perform(params object[] a)
    {
        throw new System.NotImplementedException();
    }

    public override void OnEnter(params object[] param)
    {
        throw new NotImplementedException();
    }
    public override void OnExit(params object[] param)
    {
        throw new NotImplementedException();
    }
}
public sealed class Depositing : State
{
    public override void Perform(params object[] a)
    {
        throw new System.NotImplementedException();
    }

    public override void OnEnter(params object[] param)
    {
        throw new NotImplementedException();
    }
    public override void OnExit(params object[] param)
    {
        throw new NotImplementedException();
    }
}

public sealed class Idle : State
{
    public override void Perform(params object[] a)
    {
        throw new System.NotImplementedException();
    }

    public override void OnEnter(params object[] param)
    {
        throw new NotImplementedException();
    }
    public override void OnExit(params object[] param)
    {
        throw new NotImplementedException();
    }
}


