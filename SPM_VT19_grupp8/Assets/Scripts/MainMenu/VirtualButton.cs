using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VirtualButton : MonoBehaviour
{
    public InputField input;

    public void WriteLetter()
    {
        string letter = GetComponentInChildren<Text>().text;
        if (letter == "Space")
        {
            letter = " ";
        }
        if (letter == "<-" && input.text.Length > 0)
        {
            letter = "";
            input.text = input.text.Substring(0, input.text.Length - 1);
        }
        input.text += letter;
    }
}
