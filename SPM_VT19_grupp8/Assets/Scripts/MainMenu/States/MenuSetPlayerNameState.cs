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
    private GameObject nameField;

    public override void Initialize(StateMachine owner)
    {
        Menu = ((MainMenuStateMachine)owner).SetNameState;

        foreach (Button b in Menu.GetComponentsInChildren<Button>())
        {
            Buttons.Add(b.name, b);
        }

        Buttons["SPNBack"].onClick.AddListener(Back);
        Buttons["Start"].onClick.AddListener(SelectColor);

        nameField = ((MainMenuStateMachine)owner).TextField;

        base.Initialize(owner);

        playerName = "No name";
    }

    public override void HandleUpdate()
    {
        nameField.GetComponent<InputField>().onValueChanged.AddListener(SetName);

        base.HandleUpdate();

        if (EventSystem.current.currentSelectedGameObject == nameField && (Input.GetButtonDown("Submit") || Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.9))
        {
            EventSystem.current.SetSelectedGameObject(Buttons["QButton"].gameObject);
        }

        if (Input.GetButtonDown("Pause"))
        {
            SelectColor();
        }
    }

    // remove this
    public void LoadScene()
    {
        SaveFile.ClearSave();
        PlayerPrefs.SetString("playerName", playerName);
        SceneManager.LoadScene(LevelToLoad);
    }

    public void SelectColor()
    {
        PlayerPrefs.SetString("playerName", playerName);
        Owner.TransitionTo<MenuColorSelectState>();
    }

    public void SetName(string name)
    {
        this.playerName = name;
    }

    public override void Enter()
    {
        base.Enter();
        EventSystem.current.SetSelectedGameObject(nameField);
    }
}
