using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metrics
{
    public RollingQueue<float> Confidence =   new RollingQueue<float>(180);
    public RollingQueue<float> Distance   =   new RollingQueue<float>(180);
    public RollingQueue<float> Angle      =   new RollingQueue<float>(180);
    public RollingQueue<float> Magnitude  =   new RollingQueue<float>(180);
    public RollingQueue<float> Velocity   =   new RollingQueue<float>(180);
}
