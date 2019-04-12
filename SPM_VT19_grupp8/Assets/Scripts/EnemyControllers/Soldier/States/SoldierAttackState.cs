﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemies/Soldier/Attack State")]
public class SoldierAttackState : SoldierBaseState
{

    public override void Enter()
    {
        owner.agent.SetDestination(owner.transform.position);
    }

    public override void HandleUpdate()
    {
        if (owner.currentCoolDown < 0)
        {
            Shoot();
        }
        owner.currentCoolDown -= Time.deltaTime;

        if (!PlayerVisionCheck(25))
        {
            if (PlayerVisionCheck(40))
                owner.TransitionTo<SoldierChaseState>();
            else
            {
                owner.playerLastLocation = owner.playerTransform.position;
                owner.TransitionTo<SoldierAlertState>();
            }

        }
    }

    private void Shoot()
    {
        Vector3 playerPosition = owner.playerTransform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
        GameObject projectile = Instantiate(owner.projectilePrefab, owner.transform.position, Quaternion.identity);
        projectile.transform.LookAt(playerPosition);
        owner.transform.LookAt(owner.playerTransform.position);
        projectile.GetComponent<ProjectileBehaviour>().SetInitialValues(1 << owner.gameObject.layer);
        owner.currentCoolDown = owner.maxCoolDown;
    }
}
