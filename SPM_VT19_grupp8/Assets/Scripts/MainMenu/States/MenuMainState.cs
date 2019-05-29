using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "States/Menu/Main/Main State")]
public class MenuMainState : MenuBaseState
{

    /// <summary>
    /// Initializes the menu state and sets the functions for all buttons.
    /// </summary>
    /// <param name="owner">The statemachine that sets this state.</param>
    public override void Initialize(StateMachine owner)
    {
        menu = ((MainMenuStateMachine)owner).MainState;

        foreach (Button b in menu.GetComponentsInChildren<Button>())
        {
            buttons.Add(b.name, b);
        }

        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
            buttons["ContinueGame"].onClick.AddListener(ContinueGame);
        else
            buttons["ContinueGame"].enabled = false;
        buttons["StartGame"].onClick.AddListener(StartGame);
        buttons["LevelSelect"].onClick.AddListener(LevelSelect);
        buttons["Options"].onClick.AddListener(Options);
        buttons["HowToPlay"].onClick.AddListener(HowToPlay);
        buttons["Credits"].onClick.AddListener(Credits);
        buttons["QuitGame"].onClick.AddListener(Quit);

        base.Initialize(owner);
    }
    
    public override void Enter()
    {
        base.Enter();
        if (Owner.previousSelected == null)
        {
            EventSystem.current.SetSelectedGameObject(buttons["StartGame"].gameObject);
            Debug.Log("Setting to start game button selected " + EventSystem.current.currentSelectedGameObject);
        }
    }

    public void StartGame()
    {
        Owner.levelToLoad = 1;
        Owner.TransitionTo<MenuSetPlayerNameState>();
    }

    public void ContinueGame()
    {
        SceneManager.LoadScene(SaveFile.GetContinueLevelBuildindex());
    }

    public void LevelSelect()
    {
        Owner.TransitionTo<MenuLevelSelectState>();
    }

    public void Options()
    {
        Owner.TransitionTo<MenuOptionsState>();
    }
    
    public void HowToPlay()
    {
        Owner.TransitionTo<MenuHowToPlayState>();
    }

    public void Credits()
    {
        Owner.TransitionTo<MenuCreditsState>();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
