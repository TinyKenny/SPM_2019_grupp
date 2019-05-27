using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ColorTester))]
public class ColorTesterEditor : Editor
{
    private ColorTester tester;
    private string colorString;
    private bool testmode;

    private void OnEnable()
    {
        tester = (ColorTester)target;
        colorString = "";
        testmode = true;
    }

    public override void OnInspectorGUI()
    {
        testmode = EditorGUILayout.Toggle("Custom Editor", testmode);

        if (testmode)
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Update color string"))
            {
                //Debug.Log("wow!");
                UpdateColorString();
                //EditorGUILayout.HelpBox("this is helpful", MessageType.Info);
            }

            EditorGUILayout.HelpBox(colorString, MessageType.Info);
        }
        else
        {
            base.OnInspectorGUI();
        }
    }

    private void UpdateColorString()
    {
        colorString = "new Color32(" + tester.TestColor.r + ", " + tester.TestColor.g + ", " + tester.TestColor.b + ");";
    }
}
