using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class S_HandManager : MonoBehaviour
{
    public static S_HandManager Ins { get; private set; }
    void Awake()
    {
        Ins = this;
    }

    [Header("References")]
    public GameObject ExplosionParticles;

    [Header("Parameters")]
    public float fingerPairDistanceThreshold = 0.05f;
}
