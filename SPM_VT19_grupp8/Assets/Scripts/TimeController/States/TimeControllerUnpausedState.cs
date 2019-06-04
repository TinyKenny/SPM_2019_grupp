using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "States/Time Controller/Unpaused State")]
public class TimeControllerUnpausedState : TimeControllerBaseState
{
    private float PlayerTimeScale { get; set; }
    private float SlowMotionCooldownTimer { get; set; }
    private float CurrentSlowMotionEnergy { get { return Owner.CurrentSlowMotionEnergy; } set { Owner.CurrentSlowMotionEnergy = value; } }

    private float TimeSlowMultiplier { get { return Owner.TimeSlowMultiplier; } set { Owner.TimeSlowMultiplier = value; } }

    private float SlowedPlayerTimeScale { get { return Owner.SlowedPlayerTimeScale; } }
    private float SlowedWorldTimeScale { get { return Owner.SlowedWorldTimeScale; } }
    private float SlowMotionEnergyMax { get { return Owner.SlowMotionEnergyMax; } }
    private float SlowMotionCooldown { get { return Owner.SlowMotionCooldown; } }
    private float SlowMotionEnergyRegeneration { get { return Owner.SlowMotionEnergyRegeneration; } }
    private Slider SlowMotionEnergySlider { get { return Owner.SlowMotionEnergySlider; } }

    private bool slowSoundTriggered = false;

    public override void Initialize(StateMachine owner)
    {
        base.Initialize(owner);
        PlayerTimeScale = 1.0f;
        SlowMotionCooldownTimer = 0.0f;
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
    }

    private void SlowMotion()
    {
        float timeLerpValue = Input.GetAxisRaw("SlowMotion");
        if (timeLerpValue > 0.0f && SlowMotionCooldownTimer <= MathHelper.FloatEpsilon)
        {
            if (slowSoundTriggered == false)
            {
                Owner.AudioS.PlayOneShot(Owner.SlowSound);
                slowSoundTriggered = true;
            }
            PlayerTimeScale = Mathf.Lerp(1.0f, SlowedPlayerTimeScale, timeLerpValue);
            Time.timeScale = Mathf.Lerp(1.0f, SlowedWorldTimeScale, timeLerpValue);
            TimeSlowMultiplier = Mathf.Lerp(1.0f, 1.2f, timeLerpValue);

            CurrentSlowMotionEnergy = Mathf.Clamp(CurrentSlowMotionEnergy - (timeLerpValue * Time.deltaTime / Time.timeScale), 0.0f, SlowMotionEnergyMax);

            if (CurrentSlowMotionEnergy <= MathHelper.FloatEpsilon)
            {
                SlowMotionCooldownTimer = SlowMotionCooldown;
            }
        }
        else
        {
            slowSoundTriggered = false;
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
