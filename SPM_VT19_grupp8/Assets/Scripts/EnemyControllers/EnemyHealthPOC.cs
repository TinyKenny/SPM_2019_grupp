using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthPOC : MonoBehaviour
{
    [SerializeField]private float maxHealth = 2.0f;
    private float currentHealth = 2.0f;

    private Transform enemyObject;

    public AudioSource ausEnemy;
    public AudioClip soldierHurtSound;
    public AudioClip soldierDeathSound;
    public AudioClip stunbotHurtSound;
    public AudioClip stunbotDeathSound;

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

    public void TakeDamage(float damage)
    {
        if (GetComponent<SoldierStateMachine>() != null)
        {
            ausEnemy.PlayOneShot(soldierHurtSound);

            GetComponent<SoldierStateMachine>().SetAlerted(GameObject.Find("Player").transform.position);
        }
        else if (GetComponent<StunbotStateMachine>() != null)
        {
            ausEnemy.PlayOneShot(stunbotHurtSound);
        }
        currentHealth -= damage;
        if(currentHealth <= 0.0f)
        {
            if (enemyObject.gameObject.GetComponent<SoldierStateMachine>() != null)
            {
                ausEnemy.PlayOneShot(soldierDeathSound);
            }
            else if (enemyObject.gameObject.GetComponent<StunbotStateMachine>() != null)
            {
                ausEnemy.PlayOneShot(stunbotDeathSound);
            }
            Debug.Log("enemy dead");
            Destroy(enemyObject.gameObject);
        }
    }
}
