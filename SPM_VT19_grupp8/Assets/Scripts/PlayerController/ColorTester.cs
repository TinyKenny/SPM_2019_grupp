using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTester : MonoBehaviour
{
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
