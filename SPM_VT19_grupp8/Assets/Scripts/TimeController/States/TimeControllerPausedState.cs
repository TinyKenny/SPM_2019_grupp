using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Time Controller/Paused State")]
public class TimeControllerPausedState : TimeControllerBaseState
{
    public override void Enter()
    {
        base.Enter();

        Time.timeScale = 0;
    }

    public override void HandleUpdate()
    {
        base.HandleUpdate();
    }
}
