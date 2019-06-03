using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attackstate for the soldier or guard enemy. Shoots the player when within range and transits back to either alert or chase state depending on if it can still see the player within chase distance or not.
/// </summary>
[CreateAssetMenu(menuName = "States/Enemies/Soldier/Attack State")]
public class SoldierAttackState : SoldierBaseState
{
    private float fireRate;
    private float fireRateCurrentCooldown;
    private float fireRateCooldownVarianceMax;
    private float inaccuracy = 4.0f;
    private GameObject projectilePrefab;

    public override void Enter()
    {
        Agent.SetDestination(Position);
        fireRate = Owner.FireRate;
        fireRateCurrentCooldown = fireRate;
        fireRateCooldownVarianceMax = Owner.FireRateCooldownVarianceMax;
        projectilePrefab = Owner.ProjectilePrefab;
    }

    /// <summary>
    /// Runs during update to check if it is allowed to shoot and shoots if the weapon is not on cooldown then fires <see cref="Shoot"/>. Transits to <see cref="SoldierChaseState"/> when player is not close enough, transits to <see cref="SoldierAlertState"/> if player is not visisble.
    /// </summary>
    public override void HandleUpdate()
    {
        if (fireRateCurrentCooldown < 0)
        {
            Shoot();
        }
        fireRateCurrentCooldown -= Time.deltaTime;

        if (!PlayerVisionCheck(50))
        {
            if (PlayerVisionCheck(80))
                Owner.TransitionTo<SoldierChaseState>();
            else
            {
                PlayerLastLocation = PlayerTransform.position;
                Owner.TransitionTo<SoldierAlertState>();
            }

        }
    }

    /// <summary>
    /// Changes facing to look at the player, then shoots a projectile at the player that will damage on impact. After shooting it resets the cooldown between shots.
    /// </summary>
    private void Shoot()
    {
        Anim.SetTrigger("SoldierShoot");

        Owner.PlaySound(Owner.ShootSound);

        Vector3 playerPosition = PlayerTransform.position;
        Owner.transform.LookAt(PlayerTransform.position);

        GameObject projectile = Instantiate(projectilePrefab, Owner.transform.position, Quaternion.identity);
        projectile.transform.LookAt(projectile.transform.position + (Quaternion.Euler(Random.Range(0.0f, inaccuracy), Random.Range(-inaccuracy, inaccuracy), 0.0f) * Owner.transform.forward));
        projectile.GetComponent<ProjectileBehaviour>().SetInitialValues(1 << Owner.gameObject.layer);

        fireRateCurrentCooldown = fireRate + Random.Range(0.0f, fireRateCooldownVarianceMax);
    }
}
