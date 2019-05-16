using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Navigation box class for 3D pathfinding navmesh. <see cref="NavmeshRenderer"/> automatically assigns this to all
/// boxes generated.
/// </summary>
public class NavBox : MonoBehaviour, IEquatable<NavBox>
{
    [HideInInspector] [SerializeField]
    private List<NavBox> neighbours = new List<NavBox>();
    public BoxCollider Coll { get; private set; }

    private void Awake()
    {
        Coll = GetComponent<BoxCollider>();
    }

    public bool Equals(NavBox obj)
    {

        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        
        return gameObject.Equals(obj.gameObject);
    }

    public override int GetHashCode()
    {
        return gameObject.GetHashCode();
    }

    public List<NavBox> GetNeighbours()
    {
        return neighbours;
    }
}
