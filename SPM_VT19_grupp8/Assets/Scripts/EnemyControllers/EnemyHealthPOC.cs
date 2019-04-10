﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthPOC : MonoBehaviour
{
    private float maxHealth = 2.0f;
    private float currentHealth = 2.0f;


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(transform.parent);
        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if(currentHealth <= 0.0f)
        {
            Debug.Log("enemy dead");
            Destroy(transform.parent.gameObject);
        }
    }
}
