using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;



[CreateAssetMenu(menuName = "States/Menu/Main/Menu Options State")]
public class MenuOptionsState : MenuBaseState
{
    private class KeyWithName
    {
        public string Name { get; private set; }
        public KeyCode Key { get; private set; }

        public KeyWithName(string name, KeyCode key)
        {
            Name = name;
            Key = key;
        }
    }

    //private List<KeyWithName> availableKeys = new List<KeyWithName>()
    //{
    //    new KeyWithName("A button", KeyCode.JoystickButton0),
    //    new KeyWithName("B button", KeyCode.JoystickButton1),
    //    new KeyWithName("X button", KeyCode.JoystickButton2),
    //    new KeyWithName("Y button", KeyCode.JoystickButton3),
    //    new KeyWithName("Left bumper", KeyCode.JoystickButton4),
    //    new KeyWithName("Right bumper", KeyCode.JoystickButton5),
    //    new KeyWithName("Left stick click", KeyCode.JoystickButton8),
    //    new KeyWithName("Right stick click", KeyCode.JoystickButton9)
    //};

    // using a dedicated class instead of having this here might be a good idea
    public static readonly Dictionary<string, KeyCode> availableKeysDict = new Dictionary<string, KeyCode>()
    {
        { "A button", KeyCode.JoystickButton0 },
        { "B button", KeyCode.JoystickButton1 },
        { "X button", KeyCode.JoystickButton2 },
        { "Y button", KeyCode.JoystickButton3 },
        { "Left bumper", KeyCode.JoystickButton4 },
        { "Right bumper", KeyCode.JoystickButton5 },
        { "Left stick click", KeyCode.JoystickButton8 },
        { "Right stick click", KeyCode.JoystickButton9 }
    };


    private Dictionary<string, Dropdown> dropdowns = new Dictionary<string, Dropdown>();


    public override void Initialize(StateMachine owner)
    {
        menu = ((MainMenuStateMachine)owner).OptionsState;

        foreach (Button b in menu.GetComponentsInChildren<Button>())
        {
            buttons.Add(b.name, b);
        }

        buttons["OptionsBack"].onClick.AddListener(Back);

        foreach (Dropdown d in menu.GetComponentsInChildren<Dropdown>())
        {
            dropdowns.Add(d.name, d);
            d.ClearOptions();
            d.AddOptions(new List<string>(availableKeysDict.Keys));
        }

        dropdowns["JumpDropdown"].onValueChanged.AddListener(delegate { JumpMethod(dropdowns["JumpDropdown"]); });
        string originalJumpKey = PlayerPrefs.GetString("JumpKey", "Right bumper");
        dropdowns["JumpDropdown"].value  = dropdowns["JumpDropdown"].options.FindIndex((i) => { return i.text.Equals(originalJumpKey); });

        dropdowns["WallrunDropdown"].onValueChanged.AddListener(delegate { WallrunMethod(dropdowns["WallrunDropdown"]); });
        string originalWallrunKey = PlayerPrefs.GetString("WallrunKey", "Left bumper");
        dropdowns["WallrunDropdown"].value  = dropdowns["WallrunDropdown"].options.FindIndex((i) => { return i.text.Equals(originalWallrunKey); });


        base.Initialize(owner);

    }

    public override void Enter()
    {
        base.Enter();
        EventSystem.current.SetSelectedGameObject(dropdowns["JumpDropdown"].gameObject);
    }

    private void JumpMethod(Dropdown dropdown)
    {
        PlayerPrefs.SetString("JumpKey", dropdown.options[dropdown.value].text);
    }

    private void WallrunMethod(Dropdown dropdown)
    {
        PlayerPrefs.SetString("WallrunKey", dropdown.options[dropdown.value].text);
    }
}
