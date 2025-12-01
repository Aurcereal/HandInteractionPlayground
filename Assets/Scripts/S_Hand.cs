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

    public static S_Hand LeftHand {get; private set;}
    public static S_Hand RightHand {get; private set;}
    public static S_Hand[] Hands => new S_Hand[] {LeftHand, RightHand};

    public S_HandField HandField {get; private set;}
    private void Awake()
    {
        if(chirality == Chirality.Left)
            LeftHand = this;
        else
            RightHand = this;

        HandField = GetComponent<S_HandField>();
    }

    private void Start()
    {
        leapProvider.OnUpdateFrame += OnUpdateFrame;
        interactor.hand = this;
    }

    private void OnDisable()
    {
        if (currHeldObject != null) Destroy(currHeldObject);
    }

    public bool IsGrabbing {get; private set;} = false;
    Matrix4x4 currHandTransform;
    Matrix4x4 objectToHand;
    Rigidbody currHeldObject;
    void OnUpdateFrame(Frame frame)
    {
        Hand hand = frame.GetHand(chirality);
        Debug.Log(IsGrabbing);

        if(hand != null)
        {

            if (!IsGrabbing && hand.GrabStrength > activationThreshold)
            {
                IsGrabbing = true;
                interactor.OnGrabStart();
            }
            else if (IsGrabbing && hand.GrabStrength < deactivationThreshold)
            {
                IsGrabbing = false;
                interactor.OnGrabEnd();
                if (currHeldObject != null) OnReleaseObject();
            }

            // Update Interactor Transform
            interactorTransform.position = hand.PalmPosition;
            interactorTransform.rotation = hand.Rotation;
            interactorTransform.SetLossyScale(Vector3.one);
            currHandTransform = Matrix4x4.Translate(hand.PalmPosition) * Matrix4x4.Rotate(hand.Rotation);
        } 
        else
        {
            IsGrabbing = false;
            interactor.OnGrabEnd();
            if (currHeldObject != null) OnReleaseObject();
        }
    }

    public void OnGrabbedObject(Rigidbody otherRb)
    {
        objectToHand = interactorTransform.worldToLocalMatrix * otherRb.transform.localToWorldMatrix;
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
