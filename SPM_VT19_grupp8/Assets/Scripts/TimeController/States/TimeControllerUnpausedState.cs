using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Time Controller/Unpaused State")]
public class TimeControllerUnpausedState : TimeControllerBaseState
{

    private float slowMotionCooldownTimer { get; set; }
    private float currentSlowMotionEnergy { get; set; }

    private float playerTimeScale { get { return Owner.playerTimeScale; } set { Owner.playerTimeScale = value; } }
    private float TimeSlowMultiplier { get { return Owner.TimeSlowMultiplier; } set { Owner.TimeSlowMultiplier = value; } }

    private float slowedPlayerTimeScale { get { return Owner.slowedPlayerTimeScale; } }
    private float slowedWorldTimeScale { get { return Owner.slowedWorldTimeScale; } }
    private float slowMotionEnergyMax { get { return Owner.slowMotionEnergyMax; } }
    private float slowMotionCooldown { get { return Owner.slowMotionCooldown; } }
    private float slowMotionEnergyRegeneration { get { return Owner.slowMotionEnergyRegeneration; } }

    public override void Initialize(StateMachine owner)
    {
        base.Initialize(owner);
        slowMotionCooldownTimer = 0.0f;
        currentSlowMotionEnergy = slowMotionEnergyMax;
    }

    public override void Enter()
    {
        base.Enter();
        SlowMotion(); // to make sure no frames of slowmotion are lost
    }

    public override void HandleUpdate()
    {
        base.HandleUpdate();

        //SlowMotion();

        if (Input.GetButtonDown("Pause"))
        {
            Owner.TransitionTo<TimeControllerPausedState>();
        }
    }

    private void SlowMotion()
    {
        float timeLerpValue = Input.GetAxisRaw("Shoot");
        if (timeLerpValue > 0.0f && slowMotionCooldownTimer <= MathHelper.floatEpsilon)
        {
            playerTimeScale = Mathf.Lerp(1.0f, slowedPlayerTimeScale, timeLerpValue);
            Time.timeScale = Mathf.Lerp(1.0f, slowedWorldTimeScale, timeLerpValue);
            TimeSlowMultiplier = Mathf.Lerp(1.0f, 1.2f, timeLerpValue);

            currentSlowMotionEnergy = Mathf.Clamp(currentSlowMotionEnergy - (timeLerpValue * Time.deltaTime / Time.timeScale), 0.0f, slowMotionEnergyMax);

            if (currentSlowMotionEnergy <= MathHelper.floatEpsilon)
            {
                slowMotionCooldownTimer = slowMotionCooldown;
            }
        }
        else
        {
            playerTimeScale = 1.0f;
            Time.timeScale = 1.0f;
            TimeSlowMultiplier = 1.0f;

            slowMotionCooldownTimer -= Time.deltaTime;
            currentSlowMotionEnergy = Mathf.Clamp(currentSlowMotionEnergy + slowMotionEnergyRegeneration * Time.deltaTime, 0.0f, slowMotionEnergyMax);
        }
    }
}
