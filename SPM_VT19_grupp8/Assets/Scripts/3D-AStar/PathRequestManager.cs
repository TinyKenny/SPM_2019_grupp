using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathRequestManager : MonoBehaviour
{
    private Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    private PathRequest currentPathRequest;

    private static PathRequestManager pathRequestManagerInstance;
    private AStarPathfindning pathfinding;

    private bool isProcessingPath;

    private void Awake()
    {
        pathRequestManagerInstance = this;
        pathfinding = GetComponent<AStarPathfindning>();
    }

    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, float colliderRadius, Action<Vector3[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, colliderRadius, callback);
        pathRequestManagerInstance.pathRequestQueue.Enqueue(newRequest);
        pathRequestManagerInstance.TryProcessNext();
    }

    private void TryProcessNext()
    {
        if(isProcessingPath == false && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathfinding.StartFindPath(currentPathRequest.PathStart, currentPathRequest.PathEnd, currentPathRequest.ColliderRadius);
        }
    }

    public void FinishedProcessingPath(Vector3[] path, bool success)
    {
        currentPathRequest.Callback(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }

    struct PathRequest
    {
        public Vector3 PathStart;
        public Vector3 PathEnd;
        public float ColliderRadius;
        public Action<Vector3[], bool> Callback;

        public PathRequest(Vector3 start, Vector3 end, float colliderRadius, Action<Vector3[], bool> callback)
        {
            PathStart = start;
            PathEnd = end;
            ColliderRadius = colliderRadius;
            Callback = callback;
        }
    }
}
