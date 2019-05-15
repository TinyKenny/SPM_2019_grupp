using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "States/Time Controller/Unpaused State")]
public class TimeControllerUnpausedState : TimeControllerBaseState
{

    private float SlowMotionCooldownTimer { get; set; }
    private float CurrentSlowMotionEnergy { get; set; }
    private float PlayerTimeScale { get; set; }

    private float TimeSlowMultiplier { get { return Owner.TimeSlowMultiplier; } set { Owner.TimeSlowMultiplier = value; } } // do something with this

    private float slowedPlayerTimeScale { get { return Owner.slowedPlayerTimeScale; } }
    private float slowedWorldTimeScale { get { return Owner.slowedWorldTimeScale; } }
    private float slowMotionEnergyMax { get { return Owner.slowMotionEnergyMax; } }
    private float slowMotionCooldown { get { return Owner.slowMotionCooldown; } }
    private float slowMotionEnergyRegeneration { get { return Owner.slowMotionEnergyRegeneration; } }
    private Slider timeSlowEnergySlider { get { return Owner.timeSlowEnergySlider; } }

    public override void Initialize(StateMachine owner)
    {
        base.Initialize(owner);
        PlayerTimeScale = 1.0f;
        SlowMotionCooldownTimer = 0.0f;
        CurrentSlowMotionEnergy = slowMotionEnergyMax;
        timeSlowEnergySlider.maxValue = slowMotionEnergyMax;
    }

    public override void Enter()
    {
        base.Enter();
        SlowMotion(); // to make sure no frames of slowmotion are lost
    }

    public override void HandleUpdate()
    {
        base.HandleUpdate();

        SlowMotion();

        if (Input.GetButtonDown("Pause"))
        {
            Owner.TransitionTo<TimeControllerPausedState>();
        }
    }

    private void SlowMotion()
    {
        float timeLerpValue = Input.GetAxisRaw("Shoot");
        if (timeLerpValue > 0.0f && SlowMotionCooldownTimer <= MathHelper.floatEpsilon)
        {
            PlayerTimeScale = Mathf.Lerp(1.0f, slowedPlayerTimeScale, timeLerpValue);
            Time.timeScale = Mathf.Lerp(1.0f, slowedWorldTimeScale, timeLerpValue);
            TimeSlowMultiplier = Mathf.Lerp(1.0f, 1.2f, timeLerpValue);

            CurrentSlowMotionEnergy = Mathf.Clamp(CurrentSlowMotionEnergy - (timeLerpValue * Time.deltaTime / Time.timeScale), 0.0f, slowMotionEnergyMax);

            if (CurrentSlowMotionEnergy <= MathHelper.floatEpsilon)
            {
                SlowMotionCooldownTimer = slowMotionCooldown;
            }
        }
        else
        {
            PlayerTimeScale = 1.0f;
            Time.timeScale = 1.0f;
            TimeSlowMultiplier = 1.0f;

            SlowMotionCooldownTimer -= Time.deltaTime;
            CurrentSlowMotionEnergy = Mathf.Clamp(CurrentSlowMotionEnergy + slowMotionEnergyRegeneration * Time.deltaTime, 0.0f, slowMotionEnergyMax);
        }
        timeSlowEnergySlider.value = CurrentSlowMotionEnergy;
    }

    public override float GetPlayerDeltaTime()
    {
        return PlayerTimeScale * Time.deltaTime / Time.timeScale;
    }
}
