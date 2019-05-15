using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "States/Time Controller/Unpaused State")]
public class TimeControllerUnpausedState : TimeControllerBaseState
{
    private float PlayerTimeScale { get; set; }
    private float SlowMotionCooldownTimer { get; set; }
    private float CurrentSlowMotionEnergy { get; set; }

    private float TimeSlowMultiplier { get { return Owner.TimeSlowMultiplier; } set { Owner.TimeSlowMultiplier = value; } } // do something with this

    private float SlowedPlayerTimeScale { get { return Owner.SlowedPlayerTimeScale; } }
    private float SlowedWorldTimeScale { get { return Owner.SlowedWorldTimeScale; } }
    private float SlowMotionEnergyMax { get { return Owner.SlowMotionEnergyMax; } }
    private float SlowMotionCooldown { get { return Owner.SlowMotionCooldown; } }
    private float SlowMotionEnergyRegeneration { get { return Owner.SlowMotionEnergyRegeneration; } }
    private Slider SlowMotionEnergySlider { get { return Owner.SlowMotionEnergySlider; } }

    public override void Initialize(StateMachine owner)
    {
        base.Initialize(owner);
        PlayerTimeScale = 1.0f;
        SlowMotionCooldownTimer = 0.0f;
        CurrentSlowMotionEnergy = SlowMotionEnergyMax;
        SlowMotionEnergySlider.maxValue = SlowMotionEnergyMax;
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
        float timeLerpValue = Input.GetAxisRaw("Aim"); // change up the input axes
        if (timeLerpValue > 0.0f && SlowMotionCooldownTimer <= MathHelper.floatEpsilon)
        {
            PlayerTimeScale = Mathf.Lerp(1.0f, SlowedPlayerTimeScale, timeLerpValue);
            Time.timeScale = Mathf.Lerp(1.0f, SlowedWorldTimeScale, timeLerpValue);
            TimeSlowMultiplier = Mathf.Lerp(1.0f, 1.2f, timeLerpValue);

            CurrentSlowMotionEnergy = Mathf.Clamp(CurrentSlowMotionEnergy - (timeLerpValue * Time.deltaTime / Time.timeScale), 0.0f, SlowMotionEnergyMax);

            if (CurrentSlowMotionEnergy <= MathHelper.floatEpsilon)
            {
                SlowMotionCooldownTimer = SlowMotionCooldown;
            }
        }
        else
        {
            PlayerTimeScale = 1.0f;
            Time.timeScale = 1.0f;
            TimeSlowMultiplier = 1.0f;

            SlowMotionCooldownTimer -= Time.deltaTime;
            CurrentSlowMotionEnergy = Mathf.Clamp(CurrentSlowMotionEnergy + SlowMotionEnergyRegeneration * Time.deltaTime, 0.0f, SlowMotionEnergyMax);
        }
        SlowMotionEnergySlider.value = CurrentSlowMotionEnergy;
    }

    public override float GetPlayerDeltaTime()
    {
        return PlayerTimeScale * Time.deltaTime / Time.timeScale;
    }
}
