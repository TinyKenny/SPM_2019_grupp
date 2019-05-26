﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StateMachine : MonoBehaviour
{
    [SerializeField] private State[] states = null;

    private Dictionary<Type, State> stateDictionary = new Dictionary<Type, State>();
    public State currentState;


    protected virtual void Awake()
    {
        for (int i = 0; i < states.Length; i++)
        {
            State state = states[i];
            State instance = Instantiate(state);
            instance.Initialize(this);
            stateDictionary.Add(instance.GetType(), instance);
            instance.Index = i;
            if (currentState == null)
            {
                currentState = instance;
                currentState.Enter();
            }
        }
    }

    public void TransitionTo<T>() where T : State
    {
        if (typeof(T) != currentState.GetType() && stateDictionary.ContainsKey(typeof(T)))
        {
            TransitionTask();
            currentState.Exit();
            currentState = stateDictionary[typeof(T)];
            currentState.Enter();
        }
    }

    protected virtual void Update()
    {
        currentState.HandleUpdate();
    }

    public virtual void TransitionTask()
    {

    }

    public void SetSavedState(int index)
    {
        Type t = states[index].GetType();
        TransitionTask();
        currentState.Exit();
        currentState = stateDictionary[t];
        currentState.Enter();
    }
}
