using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineImplement : MonoBehaviour
{
    public int goldResource = 30;
    public int foodStorage = 15;

    public static Action OnMineEmpty;

    private void OnEnable()
    {
        Agent.OnMine += GetMined;
    }

    private void OnDisable()
    {
        Agent.OnMine -= GetMined;
    }

    public void GetMined() {
        if(goldResource>0)
            goldResource--;
        else
            OnMineEmpty?.Invoke();
    }

    public void DistributeFood() { 
        
    }
}
