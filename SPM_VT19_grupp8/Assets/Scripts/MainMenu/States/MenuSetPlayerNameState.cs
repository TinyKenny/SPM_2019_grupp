using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    }

    public void LoadScene()
    {
        PlayerPrefs.SetString("playerName", playerName);
        PlayerPrefs.SetFloat("playerTime", 0f);
        SceneManager.LoadScene(owner.levelToLoad);
    }

    public void SetName(string name)
    {
        this.playerName = name;
    }
}
