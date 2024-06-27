using System.Collections;
using System.Collections.Generic;
using System.Numerics;

public class Configuration
{
    public Vector3 Dimensions;
    private Vector2 position;
    public Vector2 LastPosition;
    public Quaternion Rotation;
    public float Radius;//in pixles
    public float SelectionThreshold;
    public float DeselectionThreshold;
    public float HoldOpenThreshold;
    public float StartConfidenceDistance;
    private int sampleCount;

    public Vector2 Position
    {
        get
        {
            return position;
        }

        set
        {            
            if (value == position) return;

            if (sampleCount++ == 6)
            {
                sampleCount = 0;
                LastPosition = position;
            }
            position = value;
        }
    }

    public void ResetPositions()
    {
        LastPosition = Vector2.Zero;
        position = Vector2.Zero;
    }
}
