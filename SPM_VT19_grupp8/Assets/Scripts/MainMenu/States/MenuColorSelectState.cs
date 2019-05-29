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
        new Color32(255, 255, 255, 255), // color 0, "white"
        new Color32(104, 85, 61, 255), // color 1, "Bajsbrun"
        new Color32(202, 191, 81, 255), // color 2, "Guldström"
        new Color32(225, 150, 247, 255), // color 3, "Varför kan man vara rosa?"
        new Color32(52, 64, 89, 255), // color 4, "Om inte originaltexturen fungerar"
        new Color32(63, 17, 17, 255), // color 5, "Mörkröd"
        new Color32(71, 166, 134, 255), // color 6, "Turquoise"
        new Color32(33, 33, 34, 255), // color 7, "Black"
        new Color32(99, 159, 94, 255), // color 8, "Green"
        new Color32(112, 173, 241, 255) // color 9, "Blue"
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

    public override void HandleUpdate()
    {
        base.HandleUpdate();
        if (Input.GetButtonDown("Pause"))
        {
            LoadScene();
        }
    }

    public void LoadScene()
    {
        SaveFile.ClearSave();
        PlayerPrefs.SetInt("PlayerColorRed", colors[selectedColor].r);
        PlayerPrefs.SetInt("PlayerColorGreen", colors[selectedColor].g);
        PlayerPrefs.SetInt("PlayerColorBlue", colors[selectedColor].b);
        SceneManager.LoadScene(Owner.levelToLoad);
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
