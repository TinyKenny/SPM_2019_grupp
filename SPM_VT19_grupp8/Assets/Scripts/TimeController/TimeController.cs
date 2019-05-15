using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeController : StateMachine
{

    public float SlowedPlayerTimeScale { get { return slowedPlayerTimeScale; } }
    public float SlowedWorldTimeScale { get { return slowedWorldTimeScale; } }
    public float SlowMotionEnergyMax { get { return slowMotionEnergyMax; } }
    public float SlowMotionEnergyRegeneration { get { return slowMotionEnergyRegeneration; } }
    public float SlowMotionCooldown { get { return slowMotionCooldown; } }
    public Slider SlowMotionEnergySlider { get { return slowMotionEnergySlider; } }

    [SerializeField] private float slowedPlayerTimeScale = 0.5f;
    [SerializeField] private float slowedWorldTimeScale = 0.2f;
    [SerializeField] private float slowMotionEnergyMax = 5.0f;
    [SerializeField] private float slowMotionEnergyRegeneration = 1.0f;
    [SerializeField] private float slowMotionCooldown = 1.0f;
    [SerializeField] private Slider slowMotionEnergySlider = null;


    #region time-stuff
    public float TimeSlowMultiplier { get; set; } // do something with this
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
