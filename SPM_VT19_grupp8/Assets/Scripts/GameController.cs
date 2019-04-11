using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController gameControllerInstance;

    private float levelTime = 0.0f;
    
    void Start()
    {
        gameControllerInstance = this;
    }
    
    void Update()
    {
        levelTime += Time.deltaTime;
    }

    public void LoadLevel(int sceneIndex)
    {
        // spara spelarens tid, på något passande sätt

        SceneManager.LoadScene(sceneIndex);
    }
}
