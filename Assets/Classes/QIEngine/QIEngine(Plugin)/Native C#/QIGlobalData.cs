using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class QIGlobalData
{
    public static int LineDistance = 4;
    public static Rect ScreenSize;
    public static Vector3 HeadGazePosition;
    public static float DistanceScale;
    public static float AspectRatio;
    public static float FieldOfView;
    public static string WindingOrder;
    public static float MaxZetaDistance;
    public static RollingQueue<Vector3> DuplicationFreeGazePositionSamples = new RollingQueue<Vector3>(180);
    public static RollingQueue<Vector3> GazePositionSamples = new RollingQueue<Vector3>(180);
}
