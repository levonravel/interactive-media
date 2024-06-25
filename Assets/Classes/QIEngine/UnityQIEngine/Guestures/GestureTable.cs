#if QIENGINE_HOLOLENS
using Microsoft.MixedReality.Toolkit.Utilities;
#endif


using System.Collections.Generic;
using UnityEngine;

public class GestureTable : ScriptableObject
{
    public enum PoseQualifier
    {
        Greater,
        Less,
    }

    public enum GestureType
    {
        Pinch,
        Peace,
    }

    [System.Serializable]
    public class DistanceIdentifier
    {
        public PoseQualifier Qualifier;
#if QIENGINE_HOLOLENS
        public Handedness Handedness;
        public TrackedHandJoint FirstJoint;
        public TrackedHandJoint SecondJoint;
#endif
        public float DistanceAcceptance;
    }

    [System.Serializable]
    public class Pose
    {
        public string PoseName;
        public List<DistanceIdentifier> Gestures = new List<DistanceIdentifier>();
    }
    public GestureType Gesture;
    public List<Pose> Poses = new List<Pose>();
    //public SerializableDictionary<GestureType, List<DistanceIdentifier>> GestureLookup = new();
}