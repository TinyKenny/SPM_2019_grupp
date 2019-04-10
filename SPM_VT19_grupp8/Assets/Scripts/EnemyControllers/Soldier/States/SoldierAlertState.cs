using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemies/Soldier/Alert State")]
public class SoldierAlertState : SoldierBaseState
{
    private float currentCoolDown = 0;
    private float maxCoolDown = 2;

    public override void HandleUpdate()
    {
        if (currentCoolDown < 0)
        {
            Shoot();
        }
        currentCoolDown -= Time.deltaTime;

        if (!PlayerVisioCheck(20))
        {
            owner.TransitionTo<SoldierIdleState>();
        }
    }

    private void Shoot()
    {
        GameObject projectile = Instantiate(owner.projectilePrefab, owner.transform.position, Quaternion.FromToRotation(owner.transform.forward, (owner.playerTransform.position - owner.transform.position).normalized));
        projectile.GetComponent<ProjectileBehaviour>().SetInitialValues(1 << owner.gameObject.layer);
        currentCoolDown = maxCoolDown;
    }
}
