using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceMetric : IMetric
{
    public void Calculate(Node node, Vector3 inputPosition)
    {
        float distance = QIMath.GetDistanceFromBounds(node, inputPosition);
        node.Metrics.Distance.Enqueue(distance);
    }
}
