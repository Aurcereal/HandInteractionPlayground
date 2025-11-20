using Leap;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_HandPoseHandler : MonoBehaviour
{
    [SerializeField] PoseState.Action thumbIndexAction;

    public float fingerPairDistanceThreshold = 0.05f;

    const int ACTION_COUNT = 2;

    S_Hand sHand;
    void Awake()
    {
        sHand = GetComponent<S_Hand>();
        poseState = new PoseState(ACTION_COUNT);
    }

    void Start()
    {
        sHand.leapProvider.OnUpdateFrame += OnUpdateFrame;
    }

    public void OnAttractorStart()
    {
        sHand.HandField.Multiplier = 1f;
    }
    public void WhileAttractorContinuing(Vector3 pos)
    {
        sHand.HandField.Position = pos;
    }
    public void OnAttractorEnd()
    {
        sHand.HandField.Multiplier = 0f;
    }

    void OnUpdateFrame(Frame frame)
    {
        Hand hand = frame.GetHand(sHand.chirality);

        if(hand != null)
        {
            Finger[] fingers = hand.fingers;

            UpdatePoseState(hand);

            if(poseState.actionsEnabled[(int) PoseState.Action.ATTRACT])
            {
                if(!prevPoseState.actionsEnabled[(int) PoseState.Action.ATTRACT])
                    OnAttractorStart();
                else
                    WhileAttractorContinuing(0.5f * (fingers[0].TipPosition + fingers[1].TipPosition));
            } 
            else if(prevPoseState.actionsEnabled[(int) PoseState.Action.ATTRACT])
            {
                OnAttractorEnd();
            }

            if(poseState.actionsEnabled[(int) PoseState.Action.CREATE] && !prevPoseState.actionsEnabled[(int) PoseState.Action.CREATE])
            {
                Debug.Log("CREATE!");
            }
            
        }
    }

    void UpdatePoseState(Hand hand)
    {
        prevPoseState = poseState;
        poseState = new PoseState(ACTION_COUNT);

        if(!sHand.IsGrabbing) {
            Finger[] fingers = hand.fingers;
            for(int i=0; i<fingers.Length; i++)
            {
                for(int j=i+1; j<fingers.Length; j++)
                {
                    if ((fingers[i].TipPosition - fingers[j].TipPosition).magnitude <= fingerPairDistanceThreshold)
                    {
                        // Debug.Log($"Fingers {i} and {j} are touching");
                        if(i == 0 && j == 1)
                        {
                            poseState.actionsEnabled[(int) thumbIndexAction] = true;
                        }
                    }
                }
            }
        }
    }

    PoseState prevPoseState;
    PoseState poseState;
    struct PoseState
    {
        public enum Action
        {
            ATTRACT,
            CREATE
        }
        public bool[] actionsEnabled;
        public PoseState(int actionCount) {actionsEnabled = new bool[ACTION_COUNT];}
    }
}
