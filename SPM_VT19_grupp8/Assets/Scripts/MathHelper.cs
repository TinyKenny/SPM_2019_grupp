using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathHelper
{
    public const float floatEpsilon = 0.001f;

    public static Vector3 NormalForce(Vector3 velocity, Vector3 normal)
    {
        float dotValue = Vector3.Dot(velocity, normal);
        if (dotValue > 0.0f)
        {
            return Vector3.zero;
        }

        Vector3 projection = dotValue * normal;

        return -projection;
    }
}
