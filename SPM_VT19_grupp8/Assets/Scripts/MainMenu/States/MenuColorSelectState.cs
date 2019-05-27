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
        new Color32(255, 255, 255, 255), // color 2, [what color is this?]
        new Color32(255, 255, 255, 255), // color 3, [what color is this?]
        new Color32(255, 255, 255, 255), // color 4, [what color is this?]
        new Color32(255, 255, 255, 255), // color 5, [what color is this?]
        new Color32(255, 255, 255, 255), // color 6, [what color is this?]
        new Color32(255, 255, 255, 255)  // color 7, [what color is this?]
    };
    private int selectedColor = 0;
    private List<Material> colorableMaterials = new List<Material>();

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

        foreach(Renderer rend in menu.GetComponentsInChildren<Renderer>())
        {
            if(rend.CompareTag("Color Pickable"))
            {
                colorableMaterials.Add(rend.material);
            }
        }

        UpdatePreview();

        base.Initialize(owner);
    }

    public override void Enter()
    {
        base.Enter();
        EventSystem.current.SetSelectedGameObject(buttons["NextColor"].gameObject);
    }

    public void LoadScene()
    {
        SaveFile.ClearSave();
        PlayerPrefs.SetInt("PlayerColorRed", colors[selectedColor].r);
        PlayerPrefs.SetInt("PlayerColorGreen", colors[selectedColor].g);
        PlayerPrefs.SetInt("PlayerColorBlue", colors[selectedColor].b);
        SceneManager.LoadScene(owner.levelToLoad);
    }

    private void UpdatePreview()
    {
        foreach(Material mat in colorableMaterials)
        {
            mat.color = colors[selectedColor];
        }
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
