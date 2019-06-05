using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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

    /// <summary>
    /// Generates an area of boxcolliders on a layer that is only used for 3D PathFinding.
    /// The end result will be an area of boxcolliders inside the renderArea where no collider will overlap with anything
    /// on the colliders layermask. How small the boxcolliders generated can be is set by the precision variable, which
    /// also means higher precision can generate colliders closer to objects on the colliders layer.
    /// </summary>
    [ContextMenu("Render a 3D-Navigational 'mesh' for stunbot.")]
    public void Generate3DNavmesh()
    {
        BoxCollider area = Create3DNavigationBox(renderArea, renderArea.center, "NavBox 0", renderArea.size);

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

        if (recursion > 0)
        {
            Vector3[] boxPlacing = CalculateBoxWorldPlacing(area);

            for (int i = 0; i < 8; i++)
            {
                BoxCollider traversableBox = Create3DNavigationBox(area, boxPlacing[i], "NavBox" + (precision + 1 - recursion), area.size / 2);
                traversableBox = CheckCollision(traversableBox, recursion - 1);
            }
        }

        if (recursion > 0 || Physics.CheckBox(area.center, area.size / 2 + new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, colliders))
        {
            objects.Remove(area);
            DestroyImmediate(area.gameObject);
        }
        return area;
    }

    /// <summary>
    /// Calculates world coordinates for 8 sub boxes that were the result from dividing one box.
    /// </summary>
    /// <param name="area">The box that needs to be divided.</param>
    /// <returns></returns>
    private Vector3[] CalculateBoxWorldPlacing(BoxCollider area)
    {
        Vector3 offset = area.size / 4;

        Vector3 botLeftFront = area.center - offset;
        Vector3 botLeftBack = area.center + new Vector3(-offset.x, -offset.y, offset.z);
        Vector3 botRightBack = area.center + new Vector3(offset.x, -offset.y, offset.z);
        Vector3 botRightFront = area.center + new Vector3(offset.x, -offset.y, -offset.z);
        Vector3 topRightBack = area.center + offset;
        Vector3 topRightFront = area.center + new Vector3(offset.x, offset.y, -offset.z);
        Vector3 topLeftFront = area.center + new Vector3(-offset.x, offset.y, -offset.z);
        Vector3 topLeftBack = area.center + new Vector3(-offset.x, offset.y, offset.z);

        Vector3[] boxPlacing = { botLeftFront, botLeftBack, botRightBack, botRightFront, topRightBack, topRightFront, topLeftFront, topLeftBack };

        return boxPlacing;
    }

    /// <summary>
    /// Create a new box for 3D navigation.
    /// </summary>
    /// <param name="area">The area where this box is created.</param>
    /// <param name="position">The world position for the new box.</param>
    /// <param name="name">The name of the new box.</param>
    /// <param name="size">The size of the box. The first box created from <see cref="renderArea"/> should be the same as renderarea. All subsequent boxes should be half the size of the area they are created from.</param>
    /// <returns>A reference to the <see cref="BoxCollider"/> that was created.</returns>
    private BoxCollider Create3DNavigationBox(BoxCollider area, Vector3 position, string name, Vector3 size)
    {
        BoxCollider traversableBox = GameObject.CreatePrimitive(PrimitiveType.Cube).GetComponent<BoxCollider>();
        traversableBox.gameObject.name = name;
        traversableBox.transform.SetParent(transform, true);
        traversableBox.size = size;
        traversableBox.center = area.transform.position + position;
        DestroyImmediate(traversableBox.GetComponent<MeshRenderer>());
        DestroyImmediate(traversableBox.GetComponent<MeshFilter>());
        traversableBox.gameObject.AddComponent<NavBox>();
        traversableBox.gameObject.layer = 14;
        objects.Add(traversableBox);

        return traversableBox;
    }

    /// <summary>
    /// Checks how many other 3DNavMesh boxes that is next to the box and adds its list of neighbhours. Needed for AStar to function as intended.
    /// </summary>
    /// <param name="traversableBox">A box without neighbours that needs neighbhouring boxes checked.</param>
    private void AddNeighbours(BoxCollider traversableBox)
    {
        Collider[] colliders = Physics.OverlapBox(traversableBox.center, (traversableBox.size / 1.9f), Quaternion.identity, navColl);
        foreach (Collider coll in colliders)
        {
            if (!coll.transform.gameObject.Equals(traversableBox.gameObject))
                traversableBox.GetComponent<NavBox>().GetNeighbours().Add(coll.transform.GetComponent<NavBox>());
        }
    }
}