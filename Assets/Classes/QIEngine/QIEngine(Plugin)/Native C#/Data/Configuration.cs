using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Configuration
{
    public Vector3 Dimensions;
    private Vector3 position;
    public Vector3 LastPosition;
    public Quaternion Rotation;
    public float Radius;//in pixles
    public float SelectionThreshold;
    public float DeselectionThreshold;
    public float HoldOpenThreshold;
    public float StartConfidenceDistance;
    private int sampleCount;

    public Vector3 Position
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
        LastPosition = Vector3.zero;
        position = Vector3.zero;
    }
}
