using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeController : StateMachine
{


    //make it so that the player actually gets its deltatime from this script
    // this script *should* be on the player


    #region time-stuff
    public float TimeSlowMultiplier { get; set; } // do something with this
    [SerializeField] public float slowedPlayerTimeScale = 0.5f;
    [SerializeField] public float slowedWorldTimeScale = 0.2f;
    [SerializeField] public float slowMotionEnergyMax = 5.0f;
    [SerializeField] public float slowMotionEnergyRegeneration = 1.0f;
    [SerializeField] public Slider timeSlowEnergySlider = null;
    [SerializeField] public float slowMotionCooldown = 1.0f;
    #endregion



    protected override void Awake()
    {
        base.Awake();
        TransitionTo<TimeControllerUnpausedState>();
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public float GetPlayerDeltaTime()
    {
        TimeControllerBaseState timeControllerState = (TimeControllerBaseState)currentState;

        return timeControllerState.GetPlayerDeltaTime();
    }
}
