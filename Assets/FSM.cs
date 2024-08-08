using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FSM
{
    private const int UNNASIGNED_TRANSITION = -1;

    int currentState = 0;
    int stateCount = 0;

    private Dictionary<int, State> behaviour;
    private int[,] transitions;

    public FSM(int states, int flags)
    {
        behaviour = new Dictionary<int, State>();
        for (int i = 0; i < states; i++) {
            for (int j = 0; j < flags; j++)
            {
                transitions[i, j] = UNNASIGNED_TRANSITION;
            }
        
        }    
    }

    public int AddBehaviour(State state)
    {
        stateCount++;
        state.OnTransition += Transition;
        behaviour.Add(behaviour.Count, state);
        return stateCount - 1;
    }
    public void SetTransition(int originState, int flag, int destinationState)
    {
        transitions[originState, flag] = destinationState;
    }

    public void Transition(int flag) {
        behaviour[currentState].OnExit();
        currentState = transitions[currentState, flag];
        behaviour[currentState].OnEnter();
    }

    public void Tick() { 
        if (behaviour.ContainsKey(currentState))
        {
            behaviour[currentState].Perform();
        }
    }





    //void Start()
    //{
    //    movementStates.Add(Directions.Up, new UpMovementState());
    //    movementStates.Add(Directions.Down, new DownMovementState());
    //    movementStates.Add(Directions.Left, new LeftMovementState());
    //    movementStates.Add(Directions.Right, new RightMovementState());

    //    foreach (State state in movementStates.Values){
    //        //init
    //        state.Init(this.transform);
    //    }
    //}


    //void Update()
    //{
    //    //Ejecutar current State

    //    // movementStates[currentState].Perform();
    //}
}
