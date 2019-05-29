using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class MainMenuStateMachine : StateMachine
{
    public int levelToLoad { get; set; }
    public GameObject PreviousSelected { get { return previousSelected; } private set { previousSelected = value; } }
    public GameObject MainState { get { return mainState; } private set { mainState = value; } }
    public GameObject CreditsState { get { return creditsState; } private set { creditsState = value; } }
    public GameObject HowToPlayState { get { return howToPlayState; } private set { howToPlayState = value; } }
    public GameObject LevelSelectState { get { return levelSelectState; } private set { levelSelectState = value; } }
    public GameObject SetNameState { get { return setNameState; } private set { setNameState = value; } }
    public GameObject ColorSelectState { get { return colorSelectState; } private set { colorSelectState = value; } }
    public GameObject OptionsState { get { return optionsState; } private set { optionsState = value; } }
    public GameObject TextField { get { return textField; } private set { textField = value; } }

    [SerializeField] private GameObject previousSelected;
    [SerializeField] private GameObject mainState;
    [SerializeField] private GameObject creditsState;
    [SerializeField] private GameObject howToPlayState;
    [SerializeField] private GameObject levelSelectState;
    [SerializeField] private GameObject setNameState;
    [SerializeField] private GameObject colorSelectState;
    [SerializeField] private GameObject optionsState;
    [SerializeField] private GameObject textField;

    public override void TransitionTask()
    {
        previousSelected = EventSystem.current.currentSelectedGameObject;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
