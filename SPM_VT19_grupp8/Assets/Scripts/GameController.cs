using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController gameControllerInstance;
    public Text timeText;
    public PlayerStateMachine player;

    private float levelTime = 0.0f;
    
    void Start()
    {
        gameControllerInstance = this;
        levelTime = PlayerPrefs.GetFloat("playerTime");
        PlayerPrefs.SetFloat("playerTime", 0.0f);
        player.AddAmmo(PlayerPrefs.GetInt("playerAmmo"));
        PlayerPrefs.SetInt("playerAmmo", 0);
    }
    
    void Update()
    {
        levelTime += Time.unscaledDeltaTime;
        timeText.text = "Time: " + (int)levelTime;
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
            PlayerPrefs.SetInt("playerAmmo", player.ammo);
        }

        SceneManager.LoadScene(sceneIndex);
    }
}
