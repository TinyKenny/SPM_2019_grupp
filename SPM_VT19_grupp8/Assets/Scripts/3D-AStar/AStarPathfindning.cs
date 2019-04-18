using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfindning : MonoBehaviour
{
    private Vector3 start;
    private Vector3 end;

    public void FindPath(Vector3 start, Vector3 end)
    {
        this.start = start;
        this.end = end;


    }
}
