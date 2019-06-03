using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    const float minPathUpdateTime = 0.2f;
    const float pathUpdateMoveThreshold = 0.5f;

    public Transform target;
    public float speed = 20.0f;
    public float turnSpeed = 3.0f;
    public float turnDst = 5.0f;
    public float stoppingDst = 10.0f;

    Path path;

    private void Start()
    {
        StartCoroutine(UpdatePath());
    }

    public void OnPathFound(Vector3[] waypoints, bool pathSuccess)
    {
        if (pathSuccess)
        {
            path = new Path(waypoints, transform.position, turnDst, stoppingDst);

            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator UpdatePath()
    {
        if (Time.timeSinceLevelLoad < 0.3f)
        {
            yield return new WaitForSeconds(0.3f);
        }

        PathRequestManager.RequestPath(transform.position, target.position, 0.5f, OnPathFound);

        float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
        Vector3 targetPosOld = target.position;

        while (true)
        {
            yield return new WaitForSeconds(minPathUpdateTime);
            if ((target.position - targetPosOld).sqrMagnitude > sqrMoveThreshold)
            {
                PathRequestManager.RequestPath(transform.position, target.position, 0.5f, OnPathFound);
                targetPosOld = target.position;
            }
        }
    }

    IEnumerator FollowPath()
    {
        bool followingPath = true;
        int pathIndex = 0;

        transform.LookAt(path.LookPoints[0]);

        float speedPercent = 1.0f;

        while (followingPath)
        {
            while (path.TurnBoundaries[pathIndex].HasCrossedLine(transform.position))
            {
                if(pathIndex == path.FinishLineIndex)
                {
                    followingPath = false;
                    break;
                }
                else
                {
                    pathIndex++;
                }
            }

            if (followingPath)
            {
                if (pathIndex >= path.SlowDownIndex && stoppingDst > 0)
                {
                    speedPercent = Mathf.Clamp01(path.TurnBoundaries[path.FinishLineIndex].DistanceFromPoint(transform.position) / stoppingDst);
                    if(speedPercent < 0.01f)
                    {
                        followingPath = false;
                    }
                }

                Quaternion targetRotation = Quaternion.LookRotation(path.LookPoints[pathIndex] - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
                transform.Translate(Vector3.forward * Time.deltaTime * speed * speedPercent, Space.Self);
            }
            yield return null;
        }
    }
}
