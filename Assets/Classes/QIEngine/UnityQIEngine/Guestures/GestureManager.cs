#if QIENGINE_HOLOLENS
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
#endif
using UnityEngine;

public class GestureManager : MonoBehaviour
{
    private static GestureManager instance;
    public static GestureManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GestureManager>();
                if (instance == null)
                {
                    instance = new GameObject().AddComponent<GestureManager>();
                    instance.gameObject.name = "GestureManager";
                }
                gestureTable = Resources.Load<GestureTable>("GestureTable");
            }
            return instance;
        }
    }

    public class GestureData
    {
        public Vector3 StartPosition;
        public Vector3 CurrentPosition;
        public bool IsGestured;
    }

    private static GestureTable gestureTable;
    private GestureData gestureData = new GestureData();


    public bool CheckGesture(string gestureType, Transform objectToPerformActionOn, float sensitivityForAction, IGestureLogic actionToPerform)
    {
#if QIENGINE_HOLOLENS || QIENGINE_VIVE
        foreach (var pose in gestureTable.Poses)
        {
            List<GestureTable.DistanceIdentifier> requirements = null;
            if ((pose.PoseName == gestureType))
            {
                requirements = pose.Gestures;
            }
            if (requirements == null) continue;
#if QIENGINE_HOLOLENS
            foreach (var requirement in requirements)
            {
                if (HandJointUtils.TryGetJointPose(requirement.FirstJoint, requirement.Handedness, out MixedRealityPose firstJoint) && HandJointUtils.TryGetJointPose(requirement.SecondJoint, requirement.Handedness, out MixedRealityPose secondJoint))
                {
#endif
                    Vector3 firstJointPosition = firstJoint.Position;
                    Vector3 secondJointPosition = secondJoint.Position;

                    float distance = Vector3.Distance(firstJointPosition, secondJointPosition);
                    bool comparisonResult = requirement.Qualifier == GestureTable.PoseQualifier.Greater
                        ? distance > requirement.DistanceAcceptance
                        : distance < requirement.DistanceAcceptance;

                    if (comparisonResult)
                    {
                        if (!gestureData.IsGestured)
                            gestureData.StartPosition = firstJointPosition;

                        gestureData.CurrentPosition = firstJointPosition;
                        gestureData.IsGestured = true;
                    }
                    else
                    {
                        Debug.Log("Stopped Gesturing");
                        return false;

                    }
                }
                else
                {
                    gestureData.IsGestured = false;
                }
            }

            if (!gestureData.IsGestured) return false;

            Debug.Log("Is Gesturing");

            actionToPerform.Invoke(gestureData, objectToPerformActionOn, sensitivityForAction);
            return true;
        }
#endif
        return false;
    }
}