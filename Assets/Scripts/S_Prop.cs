using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using System;

[RequireComponent(typeof(Rigidbody))]
public class S_Prop : MonoBehaviour
{
    public static HashSet<S_Prop> AllProps {get; private set;} = new();

    bool grabbable = true;
    bool forceInteractions = true;

    Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        AllProps.Add(this);
    }

    private void OnDisable()
    {
        AllProps.Remove(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(grabbable) {
            S_HandInteractor interactor = other.GetComponent<S_HandInteractor>();
            if (interactor != null) // Is it an SHandInteractor?
            {
                interactor.OnCollide(rb);
            }
        }
    }

    private void FixedUpdate()
    {
        if(forceInteractions)
        {
            rb.AddForce(S_HandField.SumAllForceFields(transform.position));
        }
        
        if(transform.position.y < -10.0f) 
            Destroy(this);
    }
}
