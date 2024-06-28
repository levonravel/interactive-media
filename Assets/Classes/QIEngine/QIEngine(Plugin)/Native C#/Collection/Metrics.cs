using System.Collections;
using System.Collections.Generic;

public class Metrics
{
    public RollingQueue<double> Confidence =   new RollingQueue<double>(180);
    public RollingQueue<double> Distance   =   new RollingQueue<double>(180);
    public RollingQueue<double> Angle      =   new RollingQueue<double>(180);
    public RollingQueue<double> Magnitude  =   new RollingQueue<double>(180);
    public RollingQueue<double> Velocity   =   new RollingQueue<double>(180);
}
