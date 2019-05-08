using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class MainMenuStateMachine : StateMachine
{
    public int levelToLoad;
    public GameObject previousSelected;
    public GameObject MainState;
    public GameObject CreditsState;
    public GameObject HowToPlayState;
    public GameObject LevelSelectState;
    public GameObject SetNameState;
    public GameObject TextField;

    public AudioSource ausMenu;
    public AudioClip buttonTransition;

    public override void TransitionTask()
    {
        ausMenu.PlayOneShot(buttonTransition);
        previousSelected = EventSystem.current.currentSelectedGameObject;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
