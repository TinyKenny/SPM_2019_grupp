using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 3D AStarPathfinding for flying enemies. This is the class used on every instance of a flying enemies to find 
/// paths between start and end. Requires a <see cref="NavmeshRenderer"/> that has generated a 3D NavMesh in 
/// order to function.
/// </summary>
public class AStarPathfindning : MonoBehaviour
{
    private PathRequestManager requestManager;
    private LayerMask environmentLayer;
    private LayerMask navigationLayer;

    private void Awake()
    {
        requestManager = GetComponent<PathRequestManager>();
        environmentLayer = LayerMask.GetMask(new string[] { "Environment" });
        navigationLayer = LayerMask.GetMask(new string[] { "3DNavMesh" });
    }

    /// <summary>
    /// Starts a coroutine of <see cref="FindPath(startPosition, targetPosition, colliderRadius)"/>
    /// </summary>
    /// <param name="startPosition"></param>
    /// <param name="targetPosition"></param>
    /// <param name="colliderRadius"></param>
    public void StartFindPath(Vector3 startPosition, Vector3 targetPosition, float colliderRadius)
    {
        StartCoroutine(FindPath(startPosition, targetPosition, colliderRadius));
    }

    /// <summary>
    /// Calculates the shortest path from start to end on a pregenerated 3D navmesh.
    /// Once it is done pathfinding, it calls <see cref="PathRequestManager.FinishedProcessingPath()"/> with the found path, and wether or not the pathfinding was successful or not.
    /// </summary>
    /// <param name="start">The start position of the path.</param>
    /// <param name="end">The end position of the path.</param>
    /// <param name="colliderRadius">The radius of the <see cref="SphereCollider"/> on the stunbot.</param>
    /// <returns>Nothing, as it is a coroutine.</returns>
    IEnumerator FindPath(Vector3 start, Vector3 end, float colliderRadius)
    {
        BoxCompareNode endBoxCompareNode;
        BoxCompareNode startBoxCompareNode;

        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        if (!Physics.SphereCast(start, colliderRadius, (end - start).normalized, out RaycastHit rayHit, (end-start).magnitude, environmentLayer))
        {
            waypoints = new Vector3[] { end };
            pathSuccess = true;
            requestManager.FinishedProcessingPath(waypoints, pathSuccess);
            yield break;
        }

        try
        {
            Collider[] colls;

            colls = Physics.OverlapBox(end, new Vector3(0.45f, 0.45f, 0.45f), Quaternion.identity, navigationLayer); //end
            NavBox endBox = colls[0].GetComponent<NavBox>();

            colls = Physics.OverlapBox(start, new Vector3(0.45f, 0.45f, 0.45f), Quaternion.identity, navigationLayer); //start
            NavBox startBox;
            if (colls.Length > 0)
            {
                startBox = colls[0].GetComponent<NavBox>();
            }
            else
            {
                if(Physics.BoxCast(start, new Vector3(0.45f, 0.45f, 0.45f), (end - start).normalized, out RaycastHit hitInfo, Quaternion.identity, (end - start).magnitude, navigationLayer))
                {
                    startBox = hitInfo.transform.GetComponent<NavBox>();
                }
                else
                {
                    startBox = colls[0].GetComponent<NavBox>(); // This will cause an IndexOutOfRangeException, which we in this case want.
                }
            }

            endBoxCompareNode = new BoxCompareNode(endBox, end);
            startBoxCompareNode = new BoxCompareNode(startBox, end);
        }
        catch (System.IndexOutOfRangeException)
        {
            requestManager.FinishedProcessingPath(waypoints, false);
            yield break;
        }

        Dictionary<NavBox, BoxCompareNode> list = new Dictionary<NavBox, BoxCompareNode>();
        PriorityQueue pq = new PriorityQueue();

        startBoxCompareNode.Known = false;
        startBoxCompareNode.DistanceTraveled = 0;
        list[startBoxCompareNode.GetBox()] = startBoxCompareNode;
        startBoxCompareNode.Position = start;

        pq.Insert(startBoxCompareNode);
        Vector3 currentPosition = start;

        while (endBoxCompareNode.Known == false && pq.Size() != 0)
        {
            BoxCompareNode box = pq.DeleteMin();
            currentPosition = box.Position;

            if(box.GetBox() == endBoxCompareNode.GetBox())
            {
                pathSuccess = true;
                break;
            }

            if (!box.Known)
            {
                box.Known = true;

                foreach (NavBox aBox in box.Neighbours)
                {
                    BoxCompareNode compBox = new BoxCompareNode(aBox, end);
                    if (list.ContainsKey(aBox))
                    {
                        compBox = list[aBox];
                    }
                    else
                    {
                        list[aBox] = compBox;
                    }

                    if (!compBox.Known)
                    {
                        Vector3 nextPosition = compBox.GetBox().Coll.ClosestPointOnBounds(currentPosition);
                        float distance = box.DistanceTraveled + (currentPosition - nextPosition).sqrMagnitude;
                        if (distance < compBox.DistanceTraveled)
                        {
                            compBox.DistanceTraveled = distance;
                            compBox.Previous = box;
                            compBox.Position = nextPosition;
                            pq.Insert(compBox);
                        }
                    }
                }
            }
        }

        if (pathSuccess)
        {
            List<Vector3> pathList = new List<Vector3>();
            pathList.Add(end);

            for (BoxCompareNode b = list[endBoxCompareNode.GetBox()]; b != null; b = list[b.GetBox()].Previous)
            {
                if (b.GetBox() != startBoxCompareNode.GetBox())
                {
                    pathList.Add(b.Position);
                }
            }
            pathList.Reverse();
            waypoints = pathList.ToArray();
        }

        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
        yield return null;
    }
}