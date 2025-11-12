using Leap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Hand : MonoBehaviour
{
    [Header("References")]
    public Transform interactorTransform;
    public LeapProvider leapProvider;
    public Chirality chirality;
    public S_HandInteractor interactor;

    [Header("Parameters")]
    [Range(0.0f, 1.0f)] public float activationThreshold = 0.8f;
    [Range(0.0f, 1.0f)] public float deactivationThreshold = 0.6f;
    public float fingerPairDistanceThreshold = 0.1f;

    private void Start()
    {
        leapProvider.OnUpdateFrame += OnUpdateFrame;
        interactor.hand = this;
    }

    bool isGrabbing = false;
    Matrix4x4 currHandTransform;
    Matrix4x4 objectToHand;
    Rigidbody currHeldObject;
    void OnUpdateFrame(Frame frame)
    {
        Hand hand = frame.GetHand(chirality);

        if(hand != null)
        {

            // Finger checking
            // Finger[] fingers = hand.fingers;
            // for(int i=0; i<fingers.Length; i++)
            // {
            //     for(int j=i+1; j<fingers.Length; j++)
            //     {
            //         if ((fingers[i].TipPosition - fingers[j].TipPosition).magnitude <= fingerPairDistanceThreshold)
            //         {
            //             Debug.Log($"Fingers {i} and {j} are touching");
            //         }
            //     }
            // }
            if (!isGrabbing && hand.GrabStrength > activationThreshold)
            {
                isGrabbing = true;
                interactor.OnGrabStart();
            }
            else if (isGrabbing && hand.GrabStrength < deactivationThreshold)
            {
                isGrabbing = false;
                interactor.OnGrabEnd();
                if (currHeldObject != null) OnReleaseObject();
            }

            if (isGrabbing)
            {
                interactorTransform.position = hand.PalmPosition;
                interactorTransform.rotation = hand.Rotation;
                currHandTransform = Matrix4x4.Translate(hand.PalmPosition) * Matrix4x4.Rotate(hand.Rotation);
            }
        } 
        else
        {
            isGrabbing = false;
            interactor.OnGrabEnd();
            if (currHeldObject != null) OnReleaseObject();
        }
    }

    public void OnGrabbedObject(Rigidbody otherRb)
    {
        objectToHand = currHandTransform.inverse * otherRb.transform.localToWorldMatrix;
        currHeldObject = otherRb;

        currHeldObject.useGravity = false;
        currHeldObject.isKinematic = true;
    }

    public void OnReleaseObject()
    {
        currHeldObject.useGravity = true;
        currHeldObject.isKinematic = false;

        currHeldObject = null;
    }

    private void FixedUpdate()
    {
        if (currHeldObject != null)
        {
            Matrix4x4 targetTransform = currHandTransform * objectToHand;
            Vector3 targetPos = targetTransform.GetPosition();
            Quaternion targetRotation = targetTransform.rotation;

            currHeldObject.MoveRotation(targetRotation);
            currHeldObject.MovePosition(targetPos);
            // Vector3 diff = targetPos - currHeldObject.position;
            // if (Vector3.Dot(diff, diff) > 0.05 * 0.05)
            // {
            //     currHeldObject.velocity = diff * 30.0f;
            // }
        }
    }
}
