using UnityEngine;

public class QIEngineSettings : ScriptableObject
{
    public enum GazeType
    {
        Mouse,
        CameraGaze,
#if QIENGINE_HOLOLENS
        Hololens,
#endif
#if QIENGINE_VIVE
        Vive,
#endif
    }

    public GazeType InputType;
    public bool HololensProvider;
    public bool ViveProvider;
    public float MaxZetaDistance;
    public float DistanceScale;
    public float InputSensitivity;
    public string WindingOrder;
    public Transform CustomPointer;
    public bool ShowEyeTracker;
}