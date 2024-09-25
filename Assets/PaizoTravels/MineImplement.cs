using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineImplement : MonoBehaviour
{
    public int goldResource = 30;
    public int foodStorage = 15;

    private Vector2Int mapPos = new Vector2Int();

    public static Action<MineImplement> OnMineEmpty;

    #region ACTION_SUSCRIPTIONS
    private void OnEnable()
    {
        Agent.OnMine += GetMined;
    }

    private void OnDisable()
    {
        Agent.OnMine -= GetMined;
    }
    #endregion

    public void GetMined() {
        if(goldResource>0)
            goldResource--;
        else
            OnMineEmpty?.Invoke(this);
    }

    public void SetCoordinates(Vector2Int coord) { 
        mapPos = coord;
    }

    public Vector2Int GetCoordinates() { 
        return mapPos;
    }

}
