﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLevel : MonoBehaviour
{
    public int sceneToLoad;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Wat");
        if (other.CompareTag("Player"))
        {
            GameController.GameControllerInstance.LoadLevel(sceneToLoad);
            LoadingSceneManager.Instance.Show(SceneManager.LoadSceneAsync(sceneToLoad));
        }
    }
}
