using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_HandInteractor : MonoBehaviour
{
    public S_Hand hand;

    IEnumerator ActivateTriggerThenDeactivate()
    {
        handTrigger.enabled = true;
        yield return new WaitForSeconds(0.1f);
        handTrigger.enabled = false;
    }

    BoxCollider handTrigger;
    private void Awake()
    {
        handTrigger = GetComponent<BoxCollider>();
    }

    public void OnGrabStart()
    {
        if(enabled)
            StartCoroutine(ActivateTriggerThenDeactivate());
    }

    public void OnGrabEnd()
    {

    }

    public void OnCollide(Rigidbody other)
    {
        hand.OnGrabbedObject(other);
    }

}
