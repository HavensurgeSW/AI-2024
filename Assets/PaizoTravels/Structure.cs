using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure
{
    public Node location;
}

public class Mine : Structure
{
    int capacity;
    int supplies;

}

public class TownCenter : Structure 
{
    int storage;
    //TownImplement inst; 
    List<Traveler> workers;
    public List<Node> mineLocations = new List<Node>();

    public void SetMineLocations(List<Node> mines){ 
        for(int i = 0; i < mines.Count; i++)
        {
            mineLocations.Add(mines[i]);
        }
    }
}
