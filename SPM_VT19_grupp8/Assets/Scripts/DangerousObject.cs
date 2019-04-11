using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerousObject : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private float knockback;
    private PlayerStateMachine player;
    private BoxCollider coll;

    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<PlayerStateMachine>();
        coll = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        RaycastHit hit;
        if (Physics.BoxCast(transform.position, coll.bounds.extents, (player.transform.position - transform.position), out hit, transform.rotation, Mathf.Infinity, player.gameObject.layer))
        {
            player.TakeDamage(damage);
        }
    }
}
