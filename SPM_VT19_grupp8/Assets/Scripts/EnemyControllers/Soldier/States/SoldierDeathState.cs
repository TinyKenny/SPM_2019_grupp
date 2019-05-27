﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemies/Soldier/Death State")]
public class SoldierDeathState : SoldierBaseState
{
    private float deathAnimLength = 1;

    public override void Enter()
    {
        base.Enter();
        Agent.isStopped = true;
        Anim.SetTrigger("SoldierDeath");
    }

    public override void HandleUpdate()
    {
        deathAnimLength -= Time.deltaTime;

        if (deathAnimLength < 0)
        {
            Destroy(owner.gameObject);
        }
    }
}
