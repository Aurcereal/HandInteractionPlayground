using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_HandField : MonoBehaviour
{
    [SerializeField] float strength;

    public Vector3 Position {get; set;}
    public Vector3 Axis {get; set;}
    public float Multiplier { get; set;}

    public Vector3 SampleForceField(Vector3 samplePoint)
    {
        float r = (Position-samplePoint).magnitude;
        float attractFalloff = 0.25f + 0.75f * Mathf.InverseLerp(0.6f, 0.4f, r);
        float vortexFalloff = 0.25f + 0.75f * Mathf.InverseLerp(0.6f, 0.4f, r);

        Vector3 attract = strength * Multiplier * attractFalloff * (Position-samplePoint).normalized;
        Vector3 vortex = 0.28f*strength * Multiplier * vortexFalloff * Vector3.Cross(Axis, Position-samplePoint).normalized;

        return attract + vortex;
    }

    public static Vector3 SumAllForceFields(Vector3 samplePoint)
    {
        Vector3 sum = Vector3.zero;
        foreach(var hand in S_Hand.Hands)
        {
            S_HandField handField;
            hand.TryGetComponent(out handField);
            if(handField == null) {
                Debug.Log("Hand has no force field component!");
                continue;
            }
            sum += handField.SampleForceField(samplePoint);
        }
        return sum;
    }
}
