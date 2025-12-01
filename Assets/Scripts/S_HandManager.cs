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
    public GameObject[] SpawnableObjects;
    public Material[] ObjectMaterials;

    [Header("Parameters")]
    public float fingerPairDistanceThreshold = 0.05f;

    public void SpawnObject(Vector3 pos, Vector3 dir)
    {
        var o = Instantiate(SpawnableObjects[currentSpawnable], pos, Quaternion.LookRotation(dir, Vector3.up));
        foreach(var rb in o.GetComponentsInChildren<Rigidbody>())
        {
            rb.AddForce(dir*10.0f, ForceMode.Impulse);
        }
        currentSpawnable = (currentSpawnable+1) % SpawnableObjects.Length;

        Material mat = ObjectMaterials[UnityEngine.Random.Range(0, ObjectMaterials.Length)];
        foreach(var mr in o.GetComponentsInChildren<MeshRenderer>())
        {
            mr.material = mat;
        }
    }
    int currentSpawnable = 0;
}
