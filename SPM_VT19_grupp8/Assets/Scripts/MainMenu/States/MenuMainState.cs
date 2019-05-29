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
        Menu = ((MainMenuStateMachine)owner).MainState;

        foreach (Button b in Menu.GetComponentsInChildren<Button>())
        {
            Buttons.Add(b.name, b);
        }

        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {
            Buttons["ContinueGame"].interactable = true;
            Buttons["ContinueGame"].onClick.AddListener(ContinueGame);
        }
        else
            Buttons["ContinueGame"].interactable = false;
        Buttons["StartGame"].onClick.AddListener(StartGame);
        Buttons["LevelSelect"].onClick.AddListener(LevelSelect);
        Buttons["Options"].onClick.AddListener(Options);
        Buttons["HowToPlay"].onClick.AddListener(HowToPlay);
        Buttons["Credits"].onClick.AddListener(Credits);
        Buttons["QuitGame"].onClick.AddListener(Quit);

        base.Initialize(owner);
    }
    
    public override void Enter()
    {
        base.Enter();
        if (PreviousSelected == null)
        {
            EventSystem.current.SetSelectedGameObject(Buttons["StartGame"].gameObject);
            Debug.Log("Setting to start game button selected " + EventSystem.current.currentSelectedGameObject);
        }
    }

    public void StartGame()
    {
        LevelToLoad = 1;
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
