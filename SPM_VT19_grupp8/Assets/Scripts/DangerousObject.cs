using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerousObject : MonoBehaviour
{
    [SerializeField] private float damage = 0;
    [SerializeField] private float knockback = 0;
    [SerializeField] private float cooldownAmount = 1;
    [SerializeField] private AudioClip dangerSound;
    private float cooldown = 0;
    private AudioSource ausDanger;

    private void Awake()
    {
        ausDanger = GetComponent<AudioSource>();
    }

    private void Update()
    {
        cooldown -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        ausDanger.PlayOneShot(dangerSound);
        if (other.CompareTag("Player") && cooldown < 0)
        {
            PlayerDamageEventInfo pDEI = new PlayerDamageEventInfo(other.gameObject, damage, knockback);
            EventCoordinator.CurrentEventCoordinator.ActivateEvent(pDEI);
            cooldown = cooldownAmount;
        }
        else if (other.CompareTag("Enemy Hitbox"))
        {
            EnemyDamageEventInfo eDEI = new EnemyDamageEventInfo(other.gameObject);
            EventCoordinator.CurrentEventCoordinator.ActivateEvent(eDEI);
        }
    }
}
