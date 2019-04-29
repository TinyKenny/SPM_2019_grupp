using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthPOC : MonoBehaviour
{
    [SerializeField]private float maxHealth = 2.0f;
    private float currentHealth = 2.0f;

    private Transform enemyObject;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        //Debug.Log(transform.parent);

        if (transform.parent != null && transform.parent.GetComponent<StunbotStateMachine>() != null)
            enemyObject = transform.parent;
        else
            enemyObject = transform;

        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        //Debug.Log(enemyObject);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(float damage)
    {
        if (GetComponent<SoldierStateMachine>() != null)
            GetComponent<SoldierStateMachine>().setAlerted(GameObject.Find("Player").transform.position);
        currentHealth -= damage;
        if(currentHealth <= 0.0f)
        {
            Debug.Log("enemy dead");
            Destroy(enemyObject.gameObject);
        }
    }
}
