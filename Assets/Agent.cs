using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Flags {
    InventoryFull, ReachResource, ReachTown, AlarmRang, ResourceDepleted, Commanded
}
public enum Commands
{
    Moving, Gathering, Depositing, Idle
};
public class Agent : MonoBehaviour
{
    FSM fsm;



    private void Start()
    {
        fsm = new FSM(Enum.GetValues(typeof(Commands)).Length, Enum.GetValues(typeof(Flags)).Length);
        int movingID = fsm.AddBehaviour(new Moving());
        int gatheringID = fsm.AddBehaviour(new Gathering());
        int depositingID = fsm.AddBehaviour(new Depositing());
        int idleID = fsm.AddBehaviour(new Idle());

        fsm.SetTransition(movingID, (int)Flags.ReachResource, gatheringID);
        fsm.SetTransition(gatheringID, (int)Flags.InventoryFull, movingID);
        fsm.SetTransition(gatheringID, (int)Flags.ResourceDepleted, movingID);
        fsm.SetTransition(movingID, (int)Flags.ReachTown,depositingID);
        
    }


}
