using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_DestroyAfterTime : MonoBehaviour
{
    [SerializeField] float timeUntilDestroy;
    void Awake()
    {
        Destroy(gameObject, timeUntilDestroy);
    }
}
