﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEndScript : MonoBehaviour
{
    public int sceneToLoad;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameController.gameControllerInstance.LoadLevel(sceneToLoad);
        }
    }
}
