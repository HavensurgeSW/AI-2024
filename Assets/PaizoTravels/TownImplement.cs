using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TownImplement : MonoBehaviour
{
    [SerializeField] GameObject workerPrefab;
    [SerializeField] GameObject caravanPrefab;

    Structure str;

    void Start()
    {
        str = new TownCenter();
    }


    public void CreateAgent(Action onTravelerAction)
    {

        GameObject agentInstance = Instantiate(workerPrefab, transform.position, Quaternion.identity);
        Traveler travelerScript = agentInstance.GetComponent<Traveler>();

        // Pass the Action to the Traveler script
        travelerScript.Init(onTravelerAction);
    }

    public void CreateAgentDebug() { 

        GameObject agentInstance = Instantiate(workerPrefab, transform.position, Quaternion.identity);
        Traveler travelerScript = agentInstance.GetComponent<Traveler>();

        // Pass the Action to the Traveler script
        travelerScript.Init(() => { Debug.Log("An action has been triggered!"); });
    }




}
