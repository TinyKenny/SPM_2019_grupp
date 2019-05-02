using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerousObject : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private float knockback;
    [SerializeField] private float cooldownAmount = 1;
    private PlayerStateMachine player;
    private BoxCollider coll;
    private float cooldown = 0;

    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<PlayerStateMachine>();
        coll = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        cooldown -= Time.deltaTime;
        RaycastHit hit;
        if (Physics.BoxCast(transform.position, coll.bounds.extents, (player.transform.position - transform.position).normalized, out hit, Quaternion.identity, player.skinWidth * 10, 1 << 10, QueryTriggerInteraction.Ignore) && cooldown < 0)
        {
            cooldown = cooldownAmount;
            player.Velocity += (player.transform.position - transform.position).normalized * knockback * player.getPlayerDeltaTime();
            player.TakeDamage(damage);
        }
    }
}
