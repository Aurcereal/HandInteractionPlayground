using Leap;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_HandPoseHandler : MonoBehaviour
{
    [SerializeField] PoseState.Action thumbIndexAction;
    [SerializeField] PoseState.Action thumbMiddleAction;
    [SerializeField] PoseState.Action thumbRingAction;

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

    #region Attractor
    void OnAttractorStart()
    {
        sHand.HandField.Multiplier = 1f;
    }
    void WhileAttractorContinuing(Vector3 pos)
    {
        sHand.HandField.Position = pos;
    }
    void OnAttractorEnd()
    {
        sHand.HandField.Multiplier = 0f;
    }
    #endregion

    #region Creation
    void OnCreation(Vector3 origin, Vector3 direction)
    {
        Debug.Log("CREATE!");

    }
    #endregion

    #region Explosion
    void OnExplosion(Vector3 origin, Vector3 direction)
    {
        Debug.Log($"EXPLODE! {origin} {direction}");
        Instantiate(S_HandManager.Ins.ExplosionParticles, origin, Quaternion.AngleAxis(UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.onUnitSphere));
    }
    #endregion

    void OnUpdateFrame(Frame frame)
    {
        const int att = (int) PoseState.Action.ATTRACT;
        const int cre = (int) PoseState.Action.CREATE;
        const int exp = (int) PoseState.Action.EXPLODE;

        Hand hand = frame.GetHand(sHand.chirality);

        if(hand != null)
        {
            Finger[] fingers = hand.fingers;

            UpdatePoseState(hand);

            if(poseState.actionsEnabled[att])
            {
                if(!prevPoseState.actionsEnabled[att])
                    OnAttractorStart();
                else
                    WhileAttractorContinuing(poseState.actionPositions[att]);
            } 
            else if(prevPoseState.actionsEnabled[att])
            {
                OnAttractorEnd();
            }

            if(poseState.actionsEnabled[cre] && !prevPoseState.actionsEnabled[cre])
            {
                OnCreation(poseState.actionPositions[cre], sHand.interactorTransform.forward);
            }

            if(poseState.actionsEnabled[exp] && !prevPoseState.actionsEnabled[exp])
            {
                OnExplosion(poseState.actionPositions[exp], sHand.interactorTransform.forward);
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
                    if ((fingers[i].TipPosition - fingers[j].TipPosition).magnitude <= S_HandManager.Ins.fingerPairDistanceThreshold)
                    {
                        PoseState.Action? actionType = 
                            (i == 0 && j == 1) ? thumbIndexAction :
                            (i == 0 && j == 2) ? thumbMiddleAction :
                            (i == 0 && j == 3) ? thumbRingAction : null;
                        if(actionType != null)
                        {
                            int actionIndex = (int) actionType;
                            poseState.actionsEnabled[actionIndex] = true;
                            poseState.actionPositions[actionIndex] = 0.5f * (fingers[i].TipPosition + fingers[j].TipPosition);
                        }
                    }
                }
            }
        }
    }

    const int ACTION_COUNT = 3;

    PoseState prevPoseState;
    PoseState poseState;
    struct PoseState
    {
        public enum Action
        {
            ATTRACT,
            CREATE,
            EXPLODE
        }
        public bool[] actionsEnabled;
        public Vector3[] actionPositions;
        public PoseState(int actionCount) {
            actionsEnabled = new bool[ACTION_COUNT];
            actionPositions = new Vector3[ACTION_COUNT];
        }
    }
}
