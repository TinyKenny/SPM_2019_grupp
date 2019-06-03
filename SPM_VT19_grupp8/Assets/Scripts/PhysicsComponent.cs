using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class PhysicsComponent : MonoBehaviour
{
    public float Acceleration { get { return acceleration; } }
    public float Deceleration { get { return deceleration; } }
    public float MaxSpeed { get { return maxSpeed; } }
    public float FrictionCoefficient { get { return frictionCoefficient; } }
    public float AirResistanceCoefficient { get { return airResistanceCoefficient; } }
    public float Gravity { get { return gravity; } }
    public float SkinWidth { get { return skinWidth; } }
    public Vector3 Velocity { get; set; } = new Vector3(0.0f, 0.0f, 0.0f);

    [SerializeField] private float acceleration;
    [SerializeField] private float deceleration;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float frictionCoefficient = 0.4f;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float airResistanceCoefficient = 0.95f;
    [SerializeField] private float gravity;
    [SerializeField] private float skinWidth = 0.01f;
}
