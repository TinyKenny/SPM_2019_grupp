using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTurretScript : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float attackCooldown = 3.0f;
    public float attackTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(attackTimer <= 0.0f)
        {
            GameObject projectile = Instantiate(projectilePrefab, transform.position + transform.forward, transform.rotation);
            projectile.GetComponent<ProjectileBehaviour>().SetInitialValues(1 << gameObject.layer);
            attackTimer = attackCooldown;
        }
        else
        {
            attackTimer -= Time.deltaTime;
        }
    }
}
