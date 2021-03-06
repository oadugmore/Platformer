﻿using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, SelectionBase, AddComponentMenu(Constants.centralizedControlMenuName + "/Elliptical Movement")]
public class CEllipticalMovementObject : MonoBehaviour, ICCycleObject
{
    [Min(0.001f)]
    public float horizontalAxis;
    [Min(0.001f)]
    public float verticalAxis;
    public bool rotateClockwise;
    
    [SerializeField]
    private float offsetAngleDegrees;
    [SerializeField]
    private float offsetAngle;

    private Rigidbody movementObject;

    void Start()
    {
        movementObject = GetComponentInChildren<Rigidbody>();
    }

    public void UpdateCyclePosition(float cyclePos)
    {
        var destination = CalculatePosition(cyclePos);
        movementObject.MovePosition(destination);
    }

    public Vector3 CalculatePosition(float cyclePos)
    {
        var fraction = cyclePos;
        var newAngle = fraction * 2 * Mathf.PI + offsetAngle;
        if (rotateClockwise)
        {
            newAngle = -newAngle;
        }

        var h = horizontalAxis * Mathf.Cos(newAngle);
        var v = verticalAxis * Mathf.Sin(newAngle);
        return transform.TransformPoint(h, v, 0);
    }

    private void OnValidate()
    {
        offsetAngle = offsetAngleDegrees * Mathf.Deg2Rad;
    }
    
    // Estimates the elliptic integral using numeric integration
    [System.Obsolete]
    private float TrapezoidEstimation_Ellipse(float theta1, float theta2, float k)
    {
        var numTrapezoids = 20;
        float deltaX = (theta2 - theta1) / numTrapezoids;
        float xi = theta1;
        float sum = EllipticIntegral(xi, k) / 2;
        xi += deltaX;
        for (int i = 1; i < numTrapezoids; ++i)
        {
            sum += EllipticIntegral(xi, k);
            xi += deltaX;
        }
        sum += EllipticIntegral(xi, k) / 2;
        return sum * Mathf.Abs(deltaX);
    }

    // the incomplete elliptic integral of the second kind
    [System.Obsolete]
    private float EllipticIntegral(float theta, float k)
    {
        return Mathf.Sqrt(1 - Mathf.Pow(k, 2) * Mathf.Pow(Mathf.Sin(theta), 2));
    }
}
