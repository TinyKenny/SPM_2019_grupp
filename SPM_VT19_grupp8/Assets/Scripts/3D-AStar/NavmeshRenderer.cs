using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to generate a 3D navmesh used for <see cref="AStarPathfindning"/> to navigate where stunbot can move.
/// Needs to be used and generated outside of runtime to make stunbots and 3D pathfinding work.
/// </summary>
public class NavmeshRenderer : MonoBehaviour
{
    [SerializeField] private BoxCollider renderArea = null;
    [SerializeField] private int precision = 2;
    [SerializeField] private LayerMask colliders = 0;
    private LayerMask navColl = 1 << 14;
    private List<BoxCollider> objects = new List<BoxCollider>();
    private List<NavBox> boxes = new List<NavBox>();

    /// <summary>
    /// Generates an area of boxcolliders on a layer that is only used for 3D PathFinding.
    /// The end result will be an area of boxcolliders inside the renderArea where no collider will overlap with anything
    /// on the colliders layermask. How small the boxcolliders generated can be is set by the precision variable, which
    /// also means higher precision can generate colliders closer to objects on the colliders layer.
    /// </summary>
    [ContextMenu("Render a 3D-Navigational mesh for stunbot.")]
    public void Generate3DNavmesh()
    {
        BoxCollider area = (BoxCollider)GameObject.CreatePrimitive(PrimitiveType.Cube).GetComponent<BoxCollider>();
        area.gameObject.name = "NavBox 0";
        area.transform.SetParent(transform, true);
        area.size = renderArea.size;
        area.center = renderArea.transform.position + renderArea.center;
        area.gameObject.AddComponent<NavBox>();
        area.gameObject.layer = 14;
        boxes.Add(area.GetComponent<NavBox>());
        objects.Add(area);
        CheckCollision(area, precision);

        foreach (BoxCollider b in objects)
        {
            if (b != null && b.transform.GetComponent<NavBox>() != null)
                AddNeighbours(b);
        }
    }

    /// <summary>
    /// Checks whetever the area parameter overlaps with any objects on the colliders layer set in the inspector, if it does
    /// overlap it will generate 8 new boxcolliders, if not it will just return area. For every new collider created the 
    /// same procedure will be done with creating more boxes unless it has run more than precision amount of times.
    /// </summary>
    /// <param name="area">The area to check if it overlaps with colliders.</param>
    /// <param name="recursion">How many more iterations in the recursion it is allowed to make.</param>
    /// <returns></returns>
    private BoxCollider CheckCollision(BoxCollider area, int recursion)
    {
        if (Physics.CheckBox(area.center, area.size / 2 + new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, colliders))
        {
            if (recursion > 0)
            {
                Vector3 offset = area.size / 4;
                Vector3[] boxPlacing = { area.center - offset, area.center + new Vector3(-offset.x, -offset.y, offset.z), area.center + new Vector3(offset.x, -offset.y, offset.z), area.center + new Vector3(offset.x, -offset.y, -offset.z), area.center + offset, area.center + new Vector3(offset.x, offset.y, -offset.z), area.center + new Vector3(-offset.x, offset.y, -offset.z), area.center + new Vector3(-offset.x, offset.y, offset.z) };
                for (int i = 0; i < 8; i++)
                {
                    BoxCollider traversableBox = (BoxCollider)GameObject.CreatePrimitive(PrimitiveType.Cube).GetComponent<BoxCollider>();
                    traversableBox.gameObject.name = "NavBox" + (precision + 1 - recursion);
                    traversableBox.transform.SetParent(transform, true);
                    traversableBox.size = area.size / 2;
                    traversableBox.center = area.transform.position + boxPlacing[i];
                    DestroyImmediate(traversableBox.GetComponent<MeshRenderer>());
                    DestroyImmediate(traversableBox.GetComponent<MeshFilter>());
                    traversableBox.gameObject.AddComponent<NavBox>();
                    traversableBox.gameObject.layer = 14;
                    objects.Add(traversableBox);
                    boxes.Add(traversableBox.GetComponent<NavBox>());
                    traversableBox = CheckCollision(traversableBox, recursion - 1);
                }
            }
            objects.Remove(area);
            boxes.Remove(area.GetComponent<NavBox>());
            DestroyImmediate(area.gameObject);
        }
        else
        {
            
        }
        return area;
    }
    
    private void AddNeighbours(BoxCollider traversableBox)
    {
        Collider[] colliders = Physics.OverlapBox(traversableBox.center, (traversableBox.size / 1.9f), Quaternion.identity, navColl);
        foreach (Collider coll in colliders)
        {
            if (!coll.transform.gameObject.Equals(traversableBox.gameObject))
                traversableBox.GetComponent<NavBox>().Neighbours.Add(coll.transform.GetComponent<NavBox>());
        }
    }
}