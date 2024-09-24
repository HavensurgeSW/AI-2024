using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class FSM<EnumState, EnumFlag>
    where EnumState : Enum
    where EnumFlag : Enum
{
    private const int UNNASSIGNED_TRANSITION = -1;
    public int currentState = 0;
    private Dictionary<int, State> behaviours;
    private Dictionary<int, Func<object[]>> behaviourTickParameters;
    private Dictionary<int, Func<object[]>> behaviourOnEnterParameters;
    private Dictionary<int, Func<object[]>> behaviourOnExitParameters;

    private (int destinatinState, Action onTransition)[,] transitions;


    ParallelOptions parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = 32 };
    private BehaviourActions GetCurrentStateOnEnterBehaviours => behaviours[currentState].
                GetOnEnterBehaviours(behaviourOnEnterParameters[currentState]?.Invoke());
    private BehaviourActions GetCurrentStateOnExitBehaviours => behaviours[currentState].
                GetOnExitBehaviours(behaviourOnExitParameters[currentState]?.Invoke());
    private BehaviourActions GetCurrentStateTickBehaviours => behaviours[currentState].
                GetTickBehaviours(behaviourTickParameters[currentState]?.Invoke());
    public FSM()
    {
        int states = Enum.GetValues(typeof(EnumState)).Length;
        int flags = Enum.GetValues(typeof(EnumFlag)).Length;
        behaviours = new Dictionary<int, State>();
        transitions = new (int, Action)[states, flags];

        for (int i = 0; i < states; i++)
        {
            for (int j = 0; j < flags; j++)
            {
                transitions[i, j] = (UNNASSIGNED_TRANSITION, null);
            }
        }

        behaviourTickParameters = new Dictionary<int, Func<object[]>>();
        behaviourOnEnterParameters = new Dictionary<int, Func<object[]>>();
        behaviourOnExitParameters = new Dictionary<int, Func<object[]>>();
    }

    public void AddBehaviour<T>(EnumState state, Func<object[]> onTickParameters = null,
        Func<object[]> onEnterParameters = null, Func<object[]> onExitParameters = null) where T : State, new()
    {
        int stateIndex = Convert.ToInt32(state);
        if (!behaviours.ContainsKey(stateIndex))
        {
            State newBehaviour = new T();
            newBehaviour.OnFlag += Transition;
            behaviours.Add(stateIndex, newBehaviour);
            behaviourTickParameters.Add(stateIndex, onTickParameters);
            behaviourOnEnterParameters.Add(stateIndex, onEnterParameters);
            behaviourOnExitParameters.Add(stateIndex, onExitParameters);
        }
    }

    public void ForceState(EnumState state)
    {
        currentState = Convert.ToInt32(state);
        ExecuteBehaviour(GetCurrentStateOnEnterBehaviours);
    }

    public void SetTransition(EnumState originState, EnumFlag flag, EnumState destinationState, Action onTransition = null)
    {
        transitions[Convert.ToInt32(originState), Convert.ToInt32(flag)] = (Convert.ToInt32(destinationState), onTransition);
    }

    private void Transition(Enum flag)
    {
        if (transitions[currentState, Convert.ToInt32(flag)].destinatinState != UNNASSIGNED_TRANSITION)
        {
            ExecuteBehaviour(GetCurrentStateOnExitBehaviours);

            transitions[currentState, Convert.ToInt32(flag)].onTransition?.Invoke();

            currentState = transitions[currentState, Convert.ToInt32(flag)].destinatinState;

            ExecuteBehaviour(GetCurrentStateOnEnterBehaviours);

        }
    }

    public void Tick()
    {
        if (behaviours.ContainsKey(currentState))
        {
            ExecuteBehaviour(GetCurrentStateTickBehaviours);
        }
    }

    private void ExecuteBehaviour(BehaviourActions behaviourActions)
    {
        if (behaviourActions.Equals(default(BehaviourActions)))
            return;

        int executionOrder = 0;

        while ( (behaviourActions.MainThreadBehaviours != null          && behaviourActions.MainThreadBehaviours.Count > 0)         ||
                (behaviourActions.MultithreadablesBehaviours != null    && behaviourActions.MultithreadablesBehaviours.Count > 0))
        {
            Task multithreadableBehaviour = new Task(() =>
            {
                if (behaviourActions.MultithreadablesBehaviours != null)
                {
                    if (behaviourActions.MultithreadablesBehaviours.ContainsKey(executionOrder))
                    {
                        Parallel.ForEach(behaviourActions.MultithreadablesBehaviours[executionOrder], parallelOptions, (behaviour) =>
                        {
                            behaviour?.Invoke();
                        });
                        behaviourActions.MultithreadablesBehaviours.TryRemove(executionOrder, out _);
                    }
                }
            });

            multithreadableBehaviour.Start();

            if (behaviourActions.MainThreadBehaviours != null)
            {
                if (behaviourActions.MainThreadBehaviours.ContainsKey(executionOrder))
                {
                    foreach (Action behaviour in behaviourActions.MainThreadBehaviours[executionOrder])
                    {
                        behaviour?.Invoke();
                    }
                    behaviourActions.MainThreadBehaviours.Remove(executionOrder);
                }
            }

            multithreadableBehaviour.Wait();

            executionOrder++;
        }

        behaviourActions.TransitionBehaviour?.Invoke();
    }
}
