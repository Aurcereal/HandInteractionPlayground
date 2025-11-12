using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using System;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class GrabbableProp : MonoBehaviour
{
    //HashSet<Vector3> collidingNormals = new();

    Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        S_HandInteractor interactor = other.GetComponent<S_HandInteractor>();
        if (interactor != null)
        {
            interactor.OnCollide(rb);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // if (rb.isKinematic)
        // {
        //     Debug.Log("Collision enter");
        //     Rigidbody oRb = collision.body as Rigidbody;
        //     if (oRb != null && oRb.isKinematic)
        //     {
                
        //         rb.MovePosition(rb.position + collision.contacts[0].separation * collision.contacts[0].normal);
        //     }
        // }
    }
}
