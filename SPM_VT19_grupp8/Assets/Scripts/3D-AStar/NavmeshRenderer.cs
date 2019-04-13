using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavmeshRenderer : MonoBehaviour
{
    [SerializeField] private BoxCollider renderArea;
    [SerializeField] private int precision = 2;
    [SerializeField] private LayerMask colliders;
    private LayerMask navColl = 1 << 14;
    private List<BoxCollider> objects;

    [ContextMenu("Render a 3D-Navigational mesh for stunbot.")]
    public void Generate3DNavmesh()
    {
        BoxCollider area = (BoxCollider)GameObject.CreatePrimitive(PrimitiveType.Cube).GetComponent<BoxCollider>();
        area.size = renderArea.size;
        area.center = renderArea.transform.position + renderArea.center;
        area.gameObject.AddComponent<NavBox>();
        area.gameObject.layer = 14;
        CheckCollision(area, precision);

        foreach (BoxCollider b in objects)
        {
            if (b != null && b.transform.GetComponent<NavBox>() != null)
                AddNeighbours(b);
        }
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
                    DestroyImmediate(traversableBox.GetComponent<MeshRenderer>());
                    DestroyImmediate(traversableBox.GetComponent<MeshFilter>());
                    traversableBox.gameObject.AddComponent<NavBox>();
                    traversableBox.gameObject.layer = 14;
                    objects.Add(traversableBox);
                    traversableBox = CheckCollision(traversableBox, recursion - 1);
                }
            }
            objects.Remove(area);
            DestroyImmediate(area.gameObject);
        }
        else
        {
            
        }
        return area;
    }

    private void AddNeighbours(BoxCollider traversableBox)
    {
        RaycastHit[] hitsRight = Physics.BoxCastAll(traversableBox.center, traversableBox.size / 2, Vector3.right, Quaternion.identity, MathHelper.floatEpsilon, navColl);
        //RaycastHit[] hitsLeft = Physics.BoxCastAll(traversableBox.center, traversableBox.size / 2, Vector3.left, Quaternion.identity, MathHelper.floatEpsilon, navColl);
        //RaycastHit[] hitsUp = Physics.BoxCastAll(traversableBox.center, traversableBox.size / 2, Vector3.up, Quaternion.identity, MathHelper.floatEpsilon, navColl);
        //RaycastHit[] hitsDown = Physics.BoxCastAll(traversableBox.center, traversableBox.size / 2, Vector3.down, Quaternion.identity, MathHelper.floatEpsilon, navColl);
        //RaycastHit[] hits = new RaycastHit[hitsRight.Length + hitsLeft.Length + hitsUp.Length + hitsDown.Length];
        //int pos = 0;
        //hitsRight.CopyTo(hits, pos);
        //pos += hitsRight.Length;
        //hitsLeft.CopyTo(hits, pos);
        //pos += hitsLeft.Length;
        //hitsUp.CopyTo(hits, pos);
        //pos += hitsUp.Length;
        //hitsDown.CopyTo(hits, pos);
        //pos += hitsDown.Length;
        foreach (RaycastHit hit in hitsRight)
        {
            if (!hit.transform.gameObject.Equals(traversableBox.gameObject))
                traversableBox.GetComponent<NavBox>().neighbours.Add(hit.transform.gameObject);
        }
    }
}