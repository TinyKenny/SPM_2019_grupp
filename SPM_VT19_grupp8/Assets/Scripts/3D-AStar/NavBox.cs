using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavBox : MonoBehaviour
{
    public List<GameObject> neighbours = new List<GameObject>();

    private void Awake()
    {
        foreach (GameObject g in neighbours)
            Debug.Log(g.GetType());
    }
}
