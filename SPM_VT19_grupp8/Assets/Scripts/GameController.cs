using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
    public static GameController gameControllerInstance;
    public Text timeText;
    public PlayerStateMachine player;
    public GameObject PausePanel;
    public GameObject SelectedPauseButton;

    private float levelTime = 0.0f;
    
    void Start()
    {
        gameControllerInstance = this;
        levelTime = PlayerPrefs.GetFloat("playerTime");
        PlayerPrefs.SetFloat("playerTime", 0.0f);
        player.AddAmmo(PlayerPrefs.GetInt("playerAmmo"));
        PlayerPrefs.SetInt("playerAmmo", 0);
        PausePanel.gameObject.SetActive(false);
    }
    
    void Update()
    {
        levelTime += Time.deltaTime;
        timeText.text = "Time: " + (int)levelTime;
        if (Input.GetButtonDown("Pause"))
        {
            PausePanel.SetActive(!PausePanel.activeSelf);

            if (PausePanel.activeSelf)
            {
                EventSystem.current.SetSelectedGameObject(SelectedPauseButton);
            }
        }
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

    public void Quit()
    {
        Application.Quit();
    }
}
