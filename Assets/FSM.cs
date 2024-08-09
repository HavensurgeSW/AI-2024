using System;
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
    private Dictionary<int, Func<object[]>> behaviourTickParameters;
    private Dictionary<int, Func<object[]>> behaviourOnEnterParameters;
    private Dictionary<int, Func<object[]>> behaviourOnExitParameters;
    private int[,] transitions;

    public FSM(int states, int flags)
    {
        behaviour = new Dictionary<int, State>();
        for (int i = 0; i < states; i++)
        {
            for (int j = 0; j < flags; j++)
            {
                transitions[i, j] = UNNASIGNED_TRANSITION;
            }
        }
        behaviourTickParameters = new Dictionary<int, Func<object[]>>();
        behaviourOnEnterParameters = new Dictionary<int, Func<object[]>>();
        behaviourOnExitParameters = new Dictionary<int, Func<object[]>>();
    }

    public void AddBehaviour<T>(int stateIndex, Func<object[]> onTickParameters = null,
                                Func<object[]> onEnterParameters = null, Func<object[]> onExitParameters = null) where T: State, new()
    {
        if (behaviour.ContainsKey(stateIndex))
        {
            State newBehaviour = new T();
            behaviour.Add(stateIndex, newBehaviour);
            behaviourTickParameters.Add(stateIndex, onTickParameters);
            behaviourOnExitParameters.Add(stateIndex, onExitParameters);
        }
    }
    public void SetTransition(int originState, int flag, int destinationState)
    {
        transitions[originState, flag] = destinationState;
    }

    public void Transition(int flag)
    {

        if (transitions[currentState, flag] != UNNASIGNED_TRANSITION)
        {
            foreach (Action behaviours in behaviour[currentState].GetOnExitBehaviours(behaviourOnExitParameters[currentState]?.Invoke()))
            {
                behaviours?.Invoke();
            }

            currentState = transitions[currentState, flag];

            foreach (Action behaviours in behaviour[currentState].GetOnEnterBehaviours(behaviourOnEnterParameters[currentState]?.Invoke()))
            {
                behaviours?.Invoke();
            }

        }
    }

    public void Tick() { 
        if (behaviour.ContainsKey(currentState))
        {
            
            foreach (Action behaviours in behaviour[currentState].GetTickBehaviours(behaviourTickParameters[currentState]?.Invoke()))
            {
                behaviours?.Invoke();
            }
        }
    }

    // Action<x> admite funciones que tomen X como parametro
    // Func<x> admite funciones que tengan x como return
    // object asi como es asi es _literalmente cualquier tipo_



}
