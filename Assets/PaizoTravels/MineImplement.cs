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
        
    }

    private void OnDisable()
    {
        
    }
    #endregion

    public void GetMined() {
        if (goldResource > 0)
            goldResource--;
        if (goldResource <= 0)
        {
            OnMineEmpty?.Invoke(this);
            Debug.Log("Mine emptied!");
        }
        
    }

    public void SetCoordinates(Vector2Int coord) { 
        mapPos = coord;
    }

    public Vector2Int GetCoordinates() { 
        return mapPos;
    }

}
