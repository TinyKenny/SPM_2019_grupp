using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NavmeshRenderer))]
public class NavmeshRendererEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.wordWrap = true;
        if(GUILayout.Button("Generate navigation data\n(likely to cause unity to freeze and/or crash)", buttonStyle))
        {
            if(EditorUtility.DisplayDialog("", "Are you sure you want to generate navigation data?", "Yes, I know what I'm doing and want to generate navigation data", "Cancel"))
            {
                
                Debug.Log("You fool! You've doomed us all!");
            }
        }
    }
}
