using QuantumInterface.QIEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityLogic : ILogic
{
    public CalcType CalcType => CalcType.Velocity;

    public void Calculate(Node node, float weight, bool isStateRunner)
    {
        if (!node.ShouldCalculateConfidence) return;

        float velocity = node.Metrics.Velocity.GetNewest();
        node.Confidence *= velocity * weight;

        if (isStateRunner)
        {
            StateHandler.SwitchState(node);
        }
    }
}
