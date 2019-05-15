using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeControllerBaseState : State
{

    protected TimeController Owner { get; private set; }

    public override void Initialize(StateMachine owner)
    {
        Owner = (TimeController)owner;
    }
}
