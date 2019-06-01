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
        new Color32(33, 33, 34, 255),    // "Black"
        new Color32(255, 255, 255, 255), // "white"
        new Color32(104, 85, 61, 255),   // "Bajsbrun"
        new Color32(180, 135, 72, 255),  // "Guldström"
        new Color32(225, 150, 247, 255), // "Varför kan man vara rosa?"
        new Color32(52, 64, 89, 255),    // "Om inte originaltexturen fungerar"
        new Color32(63, 17, 17, 255),    // "Mörkröd"
        new Color32(71, 166, 134, 255),  // "Turquoise"
        new Color32(99, 159, 94, 255),   // "Green"
        new Color32(112, 173, 241, 255)  // "Blue"
    };

    private int selectedColor = 0;
    private List<Material> colorableMaterials = new List<Material>();
    private Animator cameraAnim = null;
    private float selectCooldown = 0.5f;
    private float selectCooldownMax = 0.5f;

    public override void Initialize(StateMachine owner)
    {
        Menu = ((MainMenuStateMachine)owner).ColorSelectState;

        foreach (Button b in Menu.GetComponentsInChildren<Button>())
        {
            Buttons.Add(b.name, b);
        }

        Buttons["CSLBack"].onClick.AddListener(Back);
        Buttons["Start"].onClick.AddListener(LoadScene);
        Buttons["NextColor"].onClick.AddListener(NextColor);
        Buttons["PreviousColor"].onClick.AddListener(PreviousColor);

        GameObject[] colorableObjects = GameObject.FindGameObjectsWithTag("Color Pickable");

        foreach(GameObject go in colorableObjects)
        {
            Renderer rend = go.GetComponent<Renderer>();
            if(rend != null)
            {
                colorableMaterials.Add(rend.material);
            }
            
        }

        UpdatePreview();

        cameraAnim = Camera.main.GetComponent<Animator>();

        base.Initialize(owner);
    }

    public override void Enter()
    {
        base.Enter();
        EventSystem.current.SetSelectedGameObject(Buttons["NextColor"].gameObject);
        cameraAnim.SetTrigger("ZoomInTrigger");
    }

    public override void HandleUpdate()
    {
        base.HandleUpdate();
        if (Input.GetButtonDown("Pause"))
        {
            LoadScene();
        }

        ColorSelection();
    }

    private void ColorSelection()
    {
        if (EventSystem.current.currentSelectedGameObject.name.Equals("NextColor"))
        {
            if (Input.GetAxisRaw("Horizontal") > 0.9f)
            {
                if (selectCooldown <= 0)
                {
                    NextColor();
                    selectCooldown = selectCooldownMax;
                }
                else
                    selectCooldown -= Time.deltaTime;
            }
            else if (Input.GetAxisRaw("Horizontal") < -0.9f)
            {
                EventSystem.current.SetSelectedGameObject(Buttons["PreviousColor"].gameObject);
                selectCooldown = selectCooldownMax;
            }
            else
                selectCooldown = 0;
        }
        else if (EventSystem.current.currentSelectedGameObject.name.Equals("PreviousColor"))
        {
            if (Input.GetAxisRaw("Horizontal") < -0.9f)
            {
                if (selectCooldown <= 0)
                {
                    PreviousColor();
                    selectCooldown = selectCooldownMax;
                }
                else
                    selectCooldown -= Time.deltaTime;
            }
            else if (Input.GetAxisRaw("Horizontal") > 0.9f)
            {
                EventSystem.current.SetSelectedGameObject(Buttons["NextColor"].gameObject);
                selectCooldown = selectCooldownMax;
            }
            else
                selectCooldown = 0;
        }
    }

    public void LoadScene()
    {
        SaveFile.ClearSave();
        PlayerPrefs.SetInt("PlayerColorRed", colors[selectedColor].r);
        PlayerPrefs.SetInt("PlayerColorGreen", colors[selectedColor].g);
        PlayerPrefs.SetInt("PlayerColorBlue", colors[selectedColor].b);
        LoadingSceneManager.Instance.Show(SceneManager.LoadSceneAsync(LevelToLoad));
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

    public override void Exit()
    {
        cameraAnim.SetTrigger("ZoomOutTrigger");
        base.Exit();
    }
}
