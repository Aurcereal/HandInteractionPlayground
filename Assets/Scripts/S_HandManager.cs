using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using System;

public class S_HandManager : MonoBehaviour
{
    public static S_HandManager Ins { get; private set; }
    void Awake()
    {
        Ins = this;
        foreach(var pair in audioClipPairs)
        {
            audioLibrary.Add(pair.name, pair.clip);
        }
    }

    [Header("References")]
    public LeapProvider LeapProvider;
    public GameObject ExplosionParticles;
    public GameObject CreationParticles;
    public GameObject AttractParticles;
    public GameObject[] SpawnableObjects;
    public Material[] ObjectMaterials;
    public Material TintMaterial;

    public AudioPair[] audioClipPairs;
    Dictionary<string, AudioClip> audioLibrary = new();

    [Header("Parameters")]
    public float fingerPairDistanceThreshold = 0.05f;
    public float palmClapDistanceThreshold = 0.08f;

    public void SpawnObject(Vector3 pos, Vector3 dir)
    {
        var o = Instantiate(SpawnableObjects[UnityEngine.Random.Range(0, SpawnableObjects.Length)], pos, Quaternion.LookRotation(dir, Vector3.up));
        foreach(var rb in o.GetComponentsInChildren<Rigidbody>())
        {
            rb.AddForce(dir*10.0f, ForceMode.Impulse);
        }

        Material mat = ObjectMaterials[UnityEngine.Random.Range(0, ObjectMaterials.Length)];
        foreach(var mr in o.GetComponentsInChildren<MeshRenderer>())
        {
            mr.material = mat;
        }
    }

    public void PlaySound(string soundName, float volumeMult = 1f, float pitchMult = 1f)
    {
        var sa = GetComponent<AudioSource>();
        //sa.pitch = pitchMult * UnityEngine.Random.Range(0.9f, 1.1f);
        sa.Play();
        //sa.PlayOneShot(audioLibrary[soundName], volumeMult * UnityEngine.Random.Range(0.9f, 1.1f));
    }
}

[Serializable]
public struct AudioPair
{
    public string name;
    public AudioClip clip;
}
