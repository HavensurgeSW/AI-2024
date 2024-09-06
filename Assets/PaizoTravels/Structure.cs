using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure
{
    
}

public class Mine : Structure
{
    int capacity;

}

public class TownCenter : Structure 
{
    int storage;
    List<Traveler> workers;
}
