﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

#pragma warning disable 0649
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
    [SerializeField] private Text playerName;
    public PlayerStateMachine player;
    public GameObject PausePanel;
    public GameObject SelectedPauseButton;
    private SaveFile save;

    [SerializeField]
    private GameObject projectile;

    private void Awake()
    {
        GameControllerInstance = this;
    }

    void Start()
    {
        playerName.text = PlayerPrefs.GetString("playerName");
        LevelTime = CurrentSave.LevelTime;
        PausePanel.gameObject.SetActive(false);
        SpawnProjectiles();
        EventCoordinator.CurrentEventCoordinator.RegisterEventListener<ParticleEventInfo>(FireParticle);
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

    private void SpawnProjectiles()
    {
        if(CurrentSave.Projectiles.Count > 0)
        {
            foreach (ProjectileInfo pI in CurrentSave.Projectiles)
            {
                GameObject gO = Instantiate(projectile);
                gO.transform.position = pI.Position.Position;
                gO.transform.eulerAngles = pI.Rotation.Position;
            }
        }
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
        if (CurrentSave.CheckpointReached)
        {
            CurrentSave.LevelTime = LevelTime;
            SaveFile.SaveGame();
        }
    }

    
    private void FireParticle(EventInfo eI)
    {
        ParticleEventInfo pEI = (ParticleEventInfo)eI;
        ParticleSystem gO = Instantiate(pEI.ParticleSys);
        gO.transform.eulerAngles += eI.GO.transform.eulerAngles;
        Vector3 temp = gO.transform.eulerAngles;
        gO.transform.eulerAngles = Vector3.zero;
        gO.transform.position = eI.GO.transform.position;
        gO.transform.eulerAngles = temp;
        Destroy(gO, gO.main.startLifetime.constantMax);
    }
}
