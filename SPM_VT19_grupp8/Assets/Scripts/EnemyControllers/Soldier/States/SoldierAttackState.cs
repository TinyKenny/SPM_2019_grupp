using System.Collections;
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

        if (!PlayerVisionCheck(20))
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
        GameObject projectile = Instantiate(owner.projectilePrefab, owner.transform.position, Quaternion.identity/*Quaternion.FromToRotation(owner.transform.forward, (owner.playerTransform.position - owner.transform.position).normalized) * owner.projectilePrefab.transform.rotation*/);
        projectile.transform.LookAt(owner.playerTransform);
        projectile.GetComponent<ProjectileBehaviour>().SetInitialValues(1 << owner.gameObject.layer);
        owner.currentCoolDown = owner.maxCoolDown;
    }
}
