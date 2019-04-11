using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StateMachine : MonoBehaviour
{
    [SerializeField] private State[] states;

    private Dictionary<Type, State> stateDictionary = new Dictionary<Type, State>();
    public State currentState;


    protected virtual void Awake()
    {
        foreach (State state in states)
        {
            State instance = Instantiate(state);
            instance.Initialize(this);
            stateDictionary.Add(instance.GetType(), instance);
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
            currentState.Exit();
            currentState = stateDictionary[typeof(T)];
            currentState.Enter();
            //Debug.Log(currentState.GetType());
        }
    }

    protected virtual void Update()
    {
        currentState.HandleUpdate();
    }
}
