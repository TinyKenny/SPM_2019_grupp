using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public int ammoAmount = 5;

    private GameObject player;

    private void Awake()
    {
        player = GameObject.Find("Player");
    }

    private void Update()
    {
        if (Vector3.Distance(player.transform.position, transform.position) < 1.0f)
        {
            player.GetComponent<PlayerStateMachine>().AddAmmo(ammoAmount);
            gameObject.SetActive(false);
        }
    }
}
