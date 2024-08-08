using System;
using UnityEngine;

public abstract class State
{
    public abstract void Perform(params object[] parameters);
    public Action<int> onTransition;
}
public sealed class Gathering : State
{
    public override void Perform(params object[] parameters)
    {
        throw new System.NotImplementedException();
        onTransition?.Invoke((int)Flags.InventoryFull);
    }
}

public sealed class Moving : State
{
    public override void Perform(params object[] a)
    {
        throw new System.NotImplementedException();
    }
}
public sealed class Depositing : State
{
    public override void Perform(params object[] a)
    {
        throw new System.NotImplementedException();
    }
}

public sealed class Idle : State
{
    public override void Perform(params object[] a)
    {
        throw new System.NotImplementedException();
    }
}


