using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
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
        if (other.CompareTag("Player") && cooldown < 0)
        {
            ausDanger.PlayOneShot(dangerSound);
            PlayerDamageEventInfo pDEI = new PlayerDamageEventInfo(other.gameObject, damage, knockback);
            EventCoordinator.CurrentEventCoordinator.ActivateEvent(pDEI);
            cooldown = cooldownAmount;
        }
        else if (other.CompareTag("Enemy Hitbox"))
        {
            ausDanger.PlayOneShot(dangerSound);
            EnemyDamageEventInfo eDEI = new EnemyDamageEventInfo(other.gameObject);
            EventCoordinator.CurrentEventCoordinator.ActivateEvent(eDEI);
        }
    }
}
