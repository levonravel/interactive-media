using System.Numerics;

public class DistanceMetric : IMetric
{
    public void Calculate(Node node, Vector2 inputPosition)
    {
        double distance = QIMath.GetDistanceFromBounds(node, inputPosition);
        node.Metrics.Distance.Enqueue(distance);
    }
}
