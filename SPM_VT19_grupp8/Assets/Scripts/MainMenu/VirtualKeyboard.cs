using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VirtualKeyboard : MonoBehaviour
{
    public InputField input;

    void Start()
    {
        foreach (Button b in GetComponentsInChildren<Button>())
        {
            b.onClick.AddListener(b.GetComponent<VirtualButton>().WriteLetter);
            b.GetComponent<VirtualButton>().input = input;
        }
    }
}
