using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StateMachine : MonoBehaviour
{
    [SerializeField] private State[] states = null;

    private Dictionary<Type, State> stateDictionary = new Dictionary<Type, State>();
    public State CurrentState { get; private set; }


    protected virtual void Awake()
    {
        for (int i = 0; i < states.Length; i++)
        {
            State state = states[i];
            State instance = Instantiate(state);
            instance.Initialize(this);
            stateDictionary.Add(instance.GetType(), instance);
            instance.Index = i;
            if (CurrentState == null)
            {
                CurrentState = instance;
                CurrentState.Enter();
            }
        }
    }

    public void TransitionTo<T>() where T : State
    {
        if (typeof(T) != CurrentState.GetType() && stateDictionary.ContainsKey(typeof(T)))
        {
            TransitionTask();
            CurrentState.Exit();
            CurrentState = stateDictionary[typeof(T)];
            CurrentState.Enter();
        }
    }

    protected virtual void Update()
    {
        CurrentState.HandleUpdate();
    }

    public virtual void TransitionTask()
    {

    }

    public void SetSavedState(int index)
    {
        Type t = states[index].GetType();
        TransitionTask();
        CurrentState.Exit();
        CurrentState = stateDictionary[t];
        CurrentState.Enter();
    }
}
