using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[CreateAssetMenu(menuName = "States/Menu/Main/Set Player Name State")]
public class MenuSetPlayerNameState : MenuBaseState
{
    private string playerName;

    public override void Initialize(StateMachine owner)
    {
        menu = GameObject.Find("SetPlayerName");

        GameObject.Find("SPNBack").GetComponent<Button>().onClick.AddListener(Back);
        GameObject.Find("Start").GetComponent<Button>().onClick.AddListener(LoadScene);

        base.Initialize(owner);

        playerName = "No name";
    }

    public override void HandleUpdate()
    {
        GameObject.Find("InputField").GetComponent<InputField>().onValueChanged.AddListener(SetName);

        base.HandleUpdate();

        if (EventSystem.current.currentSelectedGameObject == GameObject.Find("InputField") && Input.GetButtonDown("Submit"))
        {
            EventSystem.current.SetSelectedGameObject(GameObject.Find("Start"));
        }
    }

    public void LoadScene()
    {
        PlayerPrefs.SetString("playerName", playerName);
        PlayerPrefs.SetFloat("playerTime", 0f);
        PlayerPrefs.SetInt("playerAmmo", 0);
        SceneManager.LoadScene(owner.levelToLoad);
    }

    public void SetName(string name)
    {
        this.playerName = name;
    }

    public override void Enter()
    {
        base.Enter();
        EventSystem.current.SetSelectedGameObject(GameObject.Find("InputField"));
    }
}
