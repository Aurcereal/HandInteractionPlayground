using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

[RequireComponent(typeof(Rigidbody))]
public class GrabbableProp : MonoBehaviour
{
    Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        S_HandInteractor interactor = other.GetComponent<S_HandInteractor>();
        if(interactor != null)
        {
            interactor.OnCollide(rb);
        }
    }
}
