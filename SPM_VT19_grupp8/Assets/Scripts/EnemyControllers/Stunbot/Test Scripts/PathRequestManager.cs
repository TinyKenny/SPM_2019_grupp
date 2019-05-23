using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathRequestManager : MonoBehaviour
{
    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;

    static PathRequestManager instance;
    AStarPathfindning pathfinding;

    bool isProcessingPath;

    private void Awake()
    {
        instance = this;
        pathfinding = GetComponent<AStarPathfindning>();
    }

    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, float colliderRadius, Action<Vector3[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, colliderRadius, callback);
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    void TryProcessNext()
    {
        if(isProcessingPath == false && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd, currentPathRequest.colliderRadius);
        }
    }

    public void FinishedProcessingPath(Vector3[] path, bool success)
    {
        currentPathRequest.callback(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }

    struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public float colliderRadius;
        public Action<Vector3[], bool> callback;

        public PathRequest(Vector3 start, Vector3 end, float colliderRadius, Action<Vector3[], bool> callback)
        {
            pathStart = start;
            pathEnd = end;
            this.colliderRadius = colliderRadius;
            this.callback = callback;
        }
    }
}
