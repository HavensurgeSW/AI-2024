using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TownImplement : MonoBehaviour
{
    [SerializeField] GameObject workerPrefab;
    [SerializeField] GameObject caravanPrefab;

    TownCenter str;

    void Start()
    {
        str = new TownCenter();
    }


    public void CreateAgent()
    {

        GameObject agentInstance = Instantiate(workerPrefab, transform.position, Quaternion.identity);
        Traveler travelerScript = agentInstance.GetComponent<Traveler>();

        // Pass the Action to the Traveler script
        //travelerScript.Init(this.GetComponent<Node>(), str.mineLocations);
    }

    public void CreateAgentDebug() { 

        GameObject agentInstance = Instantiate(workerPrefab, transform.position, Quaternion.identity);
        Traveler travelerScript = agentInstance.GetComponent<Traveler>();

       
        //travelerScript.Init();
    }




}
