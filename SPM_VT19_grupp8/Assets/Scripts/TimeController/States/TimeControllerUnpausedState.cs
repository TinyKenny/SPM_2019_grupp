using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Time Controller/Unpaused State")]
public class TimeControllerUnpausedState : TimeControllerBaseState
{

    private float slowMotionCooldownTimer { get { return Owner.slowMotionCooldownTimer; } }
    private float playerTimeScale { get { return Owner.playerTimeScale; } set { Owner.playerTimeScale = value; } }
    private float slowedPlayerTimeScale { get { return Owner.slowedPlayerTimeScale; } }
    private float slowedWorldTimeScale { get { return Owner.slowedWorldTimeScale; } }
    private float TimeSlowMultiplier { get { return Owner.TimeSlowMultiplier; } set { Owner.TimeSlowMultiplier = value; } }
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //private void SlowMotion()
    //{
    //    float timeLerpValue = Input.GetAxisRaw("Shoot");
    //    if (timeLerpValue > 0.0f && slowMotionCooldownTimer <= MathHelper.floatEpsilon)
    //    {
    //        playerTimeScale = Mathf.Lerp(1.0f, slowedPlayerTimeScale, timeLerpValue);
    //        Time.timeScale = Mathf.Lerp(1.0f, slowedWorldTimeScale, timeLerpValue);
    //        TimeSlowMultiplier = Mathf.Lerp(1.0f, 1.2f, timeLerpValue);

    //        currentSlowMotionEnergy = Mathf.Clamp(currentSlowMotionEnergy - (Time.deltaTime / Time.timeScale), 0.0f, slowMotionEnergyMax);

    //        if (currentSlowMotionEnergy <= MathHelper.floatEpsilon)
    //        {
    //            slowMotionCooldownTimer = slowMotionCooldown;
    //        }
    //    }
    //    else
    //    {
    //        slowMotionCooldownTimer -= Time.deltaTime;
    //        currentSlowMotionEnergy = Mathf.Clamp(currentSlowMotionEnergy + slowMotionEnergyRegeneration * Time.deltaTime, 0.0f, slowMotionEnergyMax);
    //    }
    //}
}
