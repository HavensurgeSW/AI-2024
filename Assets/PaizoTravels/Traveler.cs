using UnityEngine;
using System;

public class Traveler : MonoBehaviour
{
    [SerializeField]int inventory;
    private Action something;

    public void Init(Action act)
    {
        something = act;
    }
    public void MoveToNode()
    {
        
    }
}

public class Matoran : Traveler 
{

}

public class UssalCrab : Traveler
{ 

}