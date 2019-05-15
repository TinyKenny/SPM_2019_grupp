using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Time Controller/Paused State")]
public class TimeControllerPausedState : TimeControllerBaseState
{



    public override void Enter()
    {
        base.Enter();
    }

    public override void HandleUpdate()
    {
        base.HandleUpdate();

        if (Input.GetButton("Pause"))
        {
            Owner.TransitionTo<TimeControllerUnpausedState>();
        }
    }


}
