using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class PhysicsComponent : MonoBehaviour
{
    public float Acceleration { get { return acceleration; } }
    public float Deceleration { get { return deceleration; } }
    public float MaxSpeed { get { return maxSpeed; } }
    public float AirResistanceCoefficient { get { return airResistanceCoefficient; } }
    public float Gravity { get { return gravity; } }
    public float SkinWidth { get { return skinWidth; } }
    public Vector3 Velocity { get; set; } = new Vector3(0.0f, 0.0f, 0.0f);

    [SerializeField, Min(0.0f)] private float acceleration;
    [SerializeField, Min(0.0f)] private float deceleration;
    [SerializeField, Min(0.0f)] private float maxSpeed;
    [SerializeField, Range(0.0f, 1.0f)] private float airResistanceCoefficient = 0.95f;
    [SerializeField, Min(0.0f)] private float gravity;
    [SerializeField, Min(0.0f)] private float skinWidth = 0.01f;
}
