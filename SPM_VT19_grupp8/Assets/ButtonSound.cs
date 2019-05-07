using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

    [RequireComponent(typeof(Button))]
public class ButtonSound : MonoBehaviour
{
    public AudioClip buttonClick;
    private Button button { get { return GetComponent<Button>(); } }
    private AudioSource ausButton { get { return GetComponent<AudioSource>(); } }

    void Start()
    {
        gameObject.AddComponent<AudioSource>();
        ausButton.clip = buttonClick;
        ausButton.playOnAwake = false;

        button.onClick.AddListener(() => PlaySound());
    }

    void PlaySound()
    {
        ausButton.PlayOneShot(buttonClick);
    }
}
