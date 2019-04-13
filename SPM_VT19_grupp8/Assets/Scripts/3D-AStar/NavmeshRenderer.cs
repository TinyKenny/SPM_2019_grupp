using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavmeshRenderer : MonoBehaviour
{
    [SerializeField] private BoxCollider renderArea;
    [SerializeField] private int precision = 2;
    [SerializeField] private LayerMask colliders;

    [ContextMenu("Render a 3D-Navigational mesh for stunbot.")]
    public void Generate3DNavmesh()
    {
        BoxCollider area = (BoxCollider)GameObject.CreatePrimitive(PrimitiveType.Cube).GetComponent<BoxCollider>();
        area.size = renderArea.size;
        area.center = renderArea.transform.position + renderArea.center;
        CheckCollision(area, precision);
    }

    private BoxCollider CheckCollision(BoxCollider area, int recursion)
    {
        if (Physics.CheckBox(area.center, area.size / 2, Quaternion.identity, colliders))
        {
            if (recursion > 0)
            {
                Vector3 offset = area.size / 4;
                Vector3[] boxPlacing = { area.center - offset, area.center + new Vector3(-offset.x, -offset.y, offset.z), area.center + new Vector3(offset.x, -offset.y, offset.z), area.center + new Vector3(offset.x, -offset.y, -offset.z), area.center + offset, area.center + new Vector3(offset.x, offset.y, -offset.z), area.center + new Vector3(-offset.x, offset.y, -offset.z), area.center + new Vector3(-offset.x, offset.y, offset.z) };
                for (int i = 0; i < 8; i++)
                {
                    BoxCollider traversableBox = (BoxCollider)GameObject.CreatePrimitive(PrimitiveType.Cube).GetComponent<BoxCollider>();
                    traversableBox.size = area.size / 2;
                    traversableBox.center = area.transform.position + boxPlacing[i];
                    traversableBox = CheckCollision(traversableBox, recursion - 1);
                }
            }
            DestroyImmediate(area.gameObject);
        }
        else
        {
            
        }
        return area;
    }
}