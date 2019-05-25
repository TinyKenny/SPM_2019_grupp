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
    public float LevelTime { get; private set; } = 0.0f;

    public static GameController GameControllerInstance { get; private set; }
    public Text timeText;
    public PlayerStateMachine player;
    public GameObject PausePanel;
    public GameObject SelectedPauseButton;
    private SaveFile save = null;

    private void Awake()
    {
        GameControllerInstance = this;
    }

    void Start()
    {
        LevelTime = CurrentSave.LevelTime;
        PausePanel.gameObject.SetActive(false);
    }
    
    void Update()
    {
        LevelTime += Time.deltaTime;
        timeText.text = "Time: " + (int)LevelTime;
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
            ScoreSaveLoad.SaveScore(PlayerPrefs.GetString("playerName"), LevelTime);
        }
        else
        {
            CurrentSave.FinishLevel(LevelTime, player.Ammo);
        }

        SceneManager.LoadScene(sceneIndex);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }

    private void OnDestroy()
    {
        CurrentSave.LevelTime = LevelTime;
        SaveFile.SaveGame();
    }
}
