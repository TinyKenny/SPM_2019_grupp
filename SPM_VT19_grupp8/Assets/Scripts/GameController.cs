using System.Collections;
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

    private SaveFile save;
    private TimeController timeController;

    [SerializeField]
    private GameObject projectile;
    [SerializeField]
    private Text timeText;
    [SerializeField]
    private Text playerName;
    [SerializeField]
    private PlayerStateMachine player;
    [SerializeField]
    private GameObject PausePanel;
    [SerializeField]
    private GameObject SelectedPauseButton;
    [SerializeField]
    private GameObject PlayerDiedPanel;
    [SerializeField]
    private GameObject SelectedPlayerDiedButton;

    

    private void Awake()
    {
        GameControllerInstance = this;
    }

    void Start()
    {
        timeController = player.GetComponent<TimeController>();
        playerName.text = PlayerPrefs.GetString("playerName");
        LevelTime = CurrentSave.LevelTime;
        PausePanel.SetActive(false);
        PlayerDiedPanel.SetActive(false);
        SpawnProjectiles();
        EventCoordinator.CurrentEventCoordinator.RegisterEventListener<ParticleEventInfo>(FireParticle);
        EventCoordinator.CurrentEventCoordinator.RegisterEventListener<DisplayPlayerDiedMenuEventInfo>(DisplayPlayerDiedPanel);
    }
    
    void Update()
    {
        LevelTime += Time.deltaTime;
        timeText.text = "Time: " + (int)LevelTime;
        if (Input.GetButtonDown("Pause") && PlayerDiedPanel.activeSelf == false)
        {
            PausePanel.SetActive(!PausePanel.activeSelf);

            if (PausePanel.activeSelf)
            {
                timeController.Pause();
                EventSystem.current.SetSelectedGameObject(SelectedPauseButton);
            }
            else
            {
                timeController.UnPause();
            }
        }
        else if (Time.timeScale > 0)
        {
            PausePanel.SetActive(false);
            PlayerDiedPanel.SetActive(false);
        }
    }

    public void LoadLevel(int sceneIndex)
    {
        SaveFile.ClearSave();

        if (SceneManager.GetActiveScene().buildIndex == 2 && sceneIndex == 3)
        {
            ScoreSaveLoad.SaveScore(PlayerPrefs.GetString("playerName"), LevelTime);
            sceneIndex = 0;
            PlayerPrefs.SetInt("FinishedGame", 1);
        }
        else
        {
            CurrentSave.FinishLevel(LevelTime, player.Ammo);
        }

        timeController.Pause();
        LoadingSceneManager.Instance.Show(SceneManager.LoadSceneAsync(sceneIndex));
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
        EventCoordinator.CurrentEventCoordinator.UnregisterEventListener<ParticleEventInfo>(FireParticle);
        EventCoordinator.CurrentEventCoordinator.UnregisterEventListener<DisplayPlayerDiedMenuEventInfo>(DisplayPlayerDiedPanel);
        if (CurrentSave.CheckpointReached && PlayerPrefs.GetInt("FinishedGame") == 0)
        {
            CurrentSave.LevelTime = LevelTime;
            SaveFile.SaveGame();
        }
    }

    private void DisplayPlayerDiedPanel(EventInfo eI)
    {
        PlayerDiedPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(SelectedPlayerDiedButton);
        timeController.Pause();
    }

    private void FireParticle(EventInfo eI)
    {
        ParticleEventInfo pEI = (ParticleEventInfo)eI;
        ParticleSystem gO = Instantiate(pEI.ParticleSys);
        gO.transform.eulerAngles += pEI.EulerRotation;
        Vector3 temp = gO.transform.eulerAngles;
        gO.transform.eulerAngles = Vector3.zero;
        gO.transform.position += eI.GO.transform.position;
        gO.transform.eulerAngles = temp;
        Destroy(gO, gO.main.startLifetime.constantMax);
    }
}
