using System.Collections;
using System.Collections.Generic;
using Leap;
using UnityEngine;

public class S_DualHandPoseHandler : MonoBehaviour
{
    [SerializeField] [ColorUsage(false, true)] Color[] tintColors;

    void Start()
    {
        S_HandManager.Ins.LeapProvider.OnUpdateFrame += OnUpdateFrame;
        prevPoseState = new();
    }

    //
    int currTintIndex = 0;
    void OnClap()
    {
        IEnumerator ScreenShake()
        {
            yield return null;
        }

        int tintIndex = UnityEngine.Random.Range(0, tintColors.Length-1);
        if(tintIndex == currTintIndex) tintIndex = tintColors.Length-1;
        currTintIndex = tintIndex;
        S_HandManager.Ins.TintMaterial.SetColor("_Tint", 
            tintColors[currTintIndex]);
    }
    //

    void OnUpdateFrame(Frame frame)
    {
        var lHand = frame.GetHand(Chirality.Left);
        var rHand = frame.GetHand(Chirality.Right);

        PoseState currPoseState;
        UpdatePoseState(out currPoseState, lHand, rHand);

        if(currPoseState.palmTogether && !prevPoseState.palmTogether)
        {
            OnClap();
        }

        prevPoseState = currPoseState;
    }

    static void UpdatePoseState(out PoseState poseState, Hand left, Hand right)
    {
        poseState = new();
        if(left != null && right != null)
        {
            poseState.palmTogether = (left.PalmPosition - right.PalmPosition).magnitude < S_HandManager.Ins.palmClapDistanceThreshold;
        }
    }

    PoseState prevPoseState;
    public struct PoseState
    {
        public bool palmTogether;
    }
}
