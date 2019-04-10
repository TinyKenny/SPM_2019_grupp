using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemies/Soldier/Alert State")]
public class SoldierAlertState : SoldierBaseState
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

        if (!PlayerVisioCheck(20))
        {
            owner.TransitionTo<SoldierChaseState>();
        }
    }

    private void Shoot()
    {
        GameObject projectile = Instantiate(owner.projectilePrefab, owner.transform.position, Quaternion.FromToRotation(owner.transform.forward, (owner.playerTransform.position - owner.transform.position).normalized));
        projectile.GetComponent<ProjectileBehaviour>().SetInitialValues(1 << owner.gameObject.layer);
        owner.currentCoolDown = owner.maxCoolDown;
    }
}
