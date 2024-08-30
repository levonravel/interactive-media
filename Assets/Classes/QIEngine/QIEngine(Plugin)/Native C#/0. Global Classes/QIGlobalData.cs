using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine.UIElements;

public static class QIGlobalData
{
    public static int LineDistance = 4;
    public static Vector2 ScreenSize;
    public static Vector2 HeadGazePosition;
    public static float DistanceScale;
    public static float AspectRatio;
    public static float FieldOfView;
    public static string WindingOrder;
    public static float MaxZetaDistance;
    public static RollingQueue<Vector2> DuplicationFreeGazePositionSamples = new RollingQueue<Vector2>(180);
    public static RollingQueue<Vector2> GazePositionSamples = new RollingQueue<Vector2>(180);
    public static double CombinedConfidence;
}
