using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
    public SaveFile CurrentSave {
        get
        {
            if (save == null)
            {
                save = new SaveFile();
                SaveFile.LoadSave();
            }
            return save;
        }
        set
        {
            save = value;
        }
    }

    public static GameController GameControllerInstance { get; private set; }
    public Text timeText;
    public PlayerStateMachine player;
    public GameObject PausePanel;
    public GameObject SelectedPauseButton;

    private float levelTime = 0.0f;
    private SaveFile save = null;

    private void Awake()
    {
        GameControllerInstance = this;
    }

    void Start()
    {
        levelTime = PlayerPrefs.GetFloat("playerTime");
        PlayerPrefs.SetFloat("playerTime", 0.0f);
        //player.AddAmmo(new AmmoPickupEventInfo(gameObject, PlayerPrefs.GetInt("playerAmmo"))); // make this better, please
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
        else if(Time.timeScale > 0)
            PausePanel.SetActive(false);
    }

    public void LoadLevel(int sceneIndex)
    {
        SaveFile.ClearSave();

        if (SceneManager.GetActiveScene().buildIndex == 2 && sceneIndex == 3)
        {
            ScoreSaveLoad.SaveScore(PlayerPrefs.GetString("playerName"), levelTime);
        }
        else
        {
            SavePlayerVariables();
        }

        SceneManager.LoadScene(sceneIndex);
    }

    public void SavePlayerVariables()
    {
        PlayerPrefs.SetFloat("playerTime", levelTime);
        PlayerPrefs.SetInt("playerAmmo", player.Ammo);
    }

    public void Quit()
    {
        SaveFile.SaveGame();
        Application.Quit();
    }

    public void ReturnToMenu()
    {
        SaveFile.SaveGame();
        SceneManager.LoadScene(0);
    }
}
