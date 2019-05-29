using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

    [RequireComponent(typeof(Button))]
public class ButtonSound : MonoBehaviour
{
    [SerializeField] private AudioClip buttonClick;
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();

        button.onClick.AddListener(() => PlaySound());
    }

    void PlaySound()
    {
        MainMenuStateMachine.AS.PlayOneShot(buttonClick);
    }
}
