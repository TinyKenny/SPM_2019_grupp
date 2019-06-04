using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649
public class TimeController : StateMachine
{
    public float SlowedPlayerTimeScale { get { return slowedPlayerTimeScale; } }
    public float SlowedWorldTimeScale { get { return slowedWorldTimeScale; } }
    public float SlowMotionEnergyMax { get { return slowMotionEnergyMax; } }
    public float SlowMotionEnergyRegeneration { get { return slowMotionEnergyRegeneration; } }
    public float SlowMotionCooldown { get { return slowMotionCooldown; } }
    public Slider SlowMotionEnergySlider { get { return slowMotionEnergySlider; } }
    public float CurrentSlowMotionEnergy { get; set; }
    public AudioSource AudioS { get; private set; }
    public AudioClip SlowSound { get { return slowSound; } }

    [SerializeField] private float slowedPlayerTimeScale = 0.5f;
    [SerializeField] private float slowedWorldTimeScale = 0.2f;
    [SerializeField] private float slowMotionEnergyMax = 5.0f;
    [SerializeField] private float slowMotionEnergyRegeneration = 1.0f;
    [SerializeField] private float slowMotionCooldown = 1.0f;
    [SerializeField] private Slider slowMotionEnergySlider;
    [SerializeField] private AudioClip slowSound;

    public float TimeSlowMultiplier { get; set; } // do something about this

    protected override void Awake()
    {
        AudioS = GetComponent<AudioSource>();
        base.Awake();
        TransitionTo<TimeControllerUnpausedState>();
    }

    protected override void Update()
    {
        base.Update();
    }

    private void OnDestroy()
    {
        ResetValues();
    }

    public float GetPlayerDeltaTime()
    {
        TimeControllerBaseState timeControllerState = (TimeControllerBaseState)CurrentState;
        return timeControllerState.GetPlayerDeltaTime();
    }

    public void ResetValues()
    {
        TransitionTo<TimeControllerUnpausedState>();
        CurrentState.Initialize(this); // ful lösning, men det nollställer värdena
    }

    public void UnPause()
    {
        TransitionTo<TimeControllerUnpausedState>();
    }

    public void Pause()
    {
        TransitionTo<TimeControllerPausedState>();
    }
}
