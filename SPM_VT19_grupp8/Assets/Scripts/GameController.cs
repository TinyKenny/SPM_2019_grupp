using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameController : MonoBehaviour
{
    public SaveFile CurrentSave {
        get
        {
            if (save == null)
                save = new SaveFile();
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
        player.AddAmmo(new AmmoPickupEventInfo(gameObject, PlayerPrefs.GetInt("playerAmmo"))); // make this better, please
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
        SaveFile.ClearSave();

        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            ScoreSaveLoad.SaveScore(PlayerPrefs.GetString("playerName"), levelTime);
        }
        else
        {
            PlayerPrefs.SetFloat("playerTime", levelTime);
            PlayerPrefs.SetInt("playerAmmo", player.Ammo);
        }

        SceneManager.LoadScene(sceneIndex);
    }

    public void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
            CurrentSave = (SaveFile)bf.Deserialize(file);
            file.Close();

            foreach(EnemyInfo enemy in CurrentSave.EnemyInfoList)
            {
                Vector3 position = enemy.Position;
                float health = enemy.Health;
                Debug.Log("Position: " + position + ", Health: " + health);
            }
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}
