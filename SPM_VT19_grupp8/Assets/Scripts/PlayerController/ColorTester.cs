using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTester : MonoBehaviour
{
    public Color32 TestColor { get { return testColor; } }
    [SerializeField] private Color32 testColor = Color.white;
    private Renderer rend;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        rend.material.color = testColor;
    }
}
