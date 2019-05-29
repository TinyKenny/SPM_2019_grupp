using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierCreator))]
public class BezierEditor : Editor
{
    BezierCreator creator;

    private void OnEnable()
    {
        creator = (BezierCreator)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }

    private void OnSceneGUI()
    {

        Handles.FreeMoveHandle(creator.AnchorOne.position, Quaternion.identity, 0.1f, Vector3.zero, Handles.SphereHandleCap);






        //Handles.color = Color.black;
        //Handles.DrawLine(creator.AnchorOne.position, creator.ControlOne.position);



    }
}
