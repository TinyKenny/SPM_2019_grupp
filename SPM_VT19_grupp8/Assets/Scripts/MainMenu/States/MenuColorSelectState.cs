using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;


[CreateAssetMenu(menuName = "States/Menu/Main/Color Select State")]
public class MenuColorSelectState : MenuBaseState
{
    private Color32[] colors =
    {
        new Color32(255, 255, 255, 255), // color 0, white
        new Color32(0, 255, 255, 255), // color 1, [what color is this?]
        new Color32(255, 0, 255, 255), // color 2, [what color is this?]
        new Color32(255, 255, 0, 255), // color 3, [what color is this?]
        new Color32(0, 0, 255, 255), // color 4, [what color is this?]
        new Color32(0, 255, 0, 255), // color 5, [what color is this?]
        new Color32(255, 0, 0, 255), // color 6, [what color is this?]
        new Color32(0, 0, 0, 255)  // color 7, [what color is this?]
    };

    private int selectedColor = 0;

    private GameObject playerPreviewRef;

    // add a reference to the player preview

    public override void Initialize(StateMachine owner)
    {
        menu = ((MainMenuStateMachine)owner).ColorSelectState;

        foreach (Button b in menu.GetComponentsInChildren<Button>())
        {
            buttons.Add(b.name, b);
        }

        buttons["CSLBack"].onClick.AddListener(Back);
        buttons["Start"].onClick.AddListener(LoadScene);
        buttons["NextColor"].onClick.AddListener(NextColor);
        buttons["PreviousColor"].onClick.AddListener(PreviousColor);

        #region placeholder

        playerPreviewRef = buttons["Player Preview placeholder"].gameObject;
        buttons["Player Preview placeholder"].enabled = false;

        #endregion

        UpdatePreview();

        base.Initialize(owner);
    }

    public override void Enter()
    {
        base.Enter();
        EventSystem.current.SetSelectedGameObject(buttons["CSLBack"].gameObject);
    }

    public void LoadScene()
    {
        SaveFile.ClearSave();
        SceneManager.LoadScene(owner.levelToLoad);
    }

    private void UpdatePreview()
    {
        playerPreviewRef.GetComponent<Image>().color = colors[selectedColor];
    }

    private void NextColor()
    {
        selectedColor = (selectedColor + 1) % colors.Length;
        UpdatePreview();
    }

    private void PreviousColor()
    {
        selectedColor = (colors.Length + selectedColor - 1) % colors.Length;
        UpdatePreview();
    }
}
