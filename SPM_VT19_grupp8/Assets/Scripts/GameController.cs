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
        levelTime = PlayerPrefs.GetFloat("playerTime");
        PlayerPrefs.SetFloat("playerTime", 0.0f);
    }
    
    void Update()
    {
        levelTime += Time.unscaledDeltaTime;
    }

    public void LoadLevel(int sceneIndex)
    {
        if(SceneManager.GetActiveScene().buildIndex == 2)
        {
            ScoreSaveLoad.SaveScore(PlayerPrefs.GetString("playerName"), levelTime);
        }
        else
        {
            PlayerPrefs.SetFloat("playerTime", levelTime);
        }

        SceneManager.LoadScene(sceneIndex);
    }
}
