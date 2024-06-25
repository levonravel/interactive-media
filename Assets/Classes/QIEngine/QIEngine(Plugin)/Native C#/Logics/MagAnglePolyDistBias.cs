using QuantumInterface.QIEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagAnglePolyDistBias : ILogic
{
    public CalcType CalcType => CalcType.MagAnglePolyDistBias;

    public void Calculate(Node node, float weight, bool isStateRunner)
    {
        if (!node.ShouldCalculateConfidence) return;

        var directionSource1 = node.Metrics.Angle.GetNewest();
        var directionSource2 = node.Metrics.Angle.GetNewest(1);
        var directionSource3 = node.Metrics.Angle.GetNewest(2);
       
        var accelSource = node.Metrics.Magnitude.GetNewest() - node.Metrics.Magnitude.GetNewest(1);

        if (accelSource < 0)
            accelSource = System.Math.Abs(accelSource);

        node.Confidence = directionSource1 +
                (directionSource2 * accelSource) +
                (directionSource3 * (accelSource * accelSource));

        if (isStateRunner)
        {
            StateHandler.SwitchState(node);
        }
    }
}
