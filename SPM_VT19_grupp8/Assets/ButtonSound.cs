using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

#pragma warning disable 0649
[RequireComponent(typeof(Button))]
public class ButtonSound : MonoBehaviour
{
    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private AudioSource auS;
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();

        button.onClick.AddListener(() => ClickSound());
    }

    void ClickSound()
    {
        PlaySound(buttonClick);
    }

    public void PlaySound(AudioClip ac)
    {
        AudioSource aS = Instantiate(auS);
        aS.transform.position = transform.position;
        aS.spatialBlend = 0;
        aS.PlayOneShot(ac);
        Destroy(aS.gameObject, ac.length);
    }
}
