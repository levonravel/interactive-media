using QuantumInterface.QIEngine;

public class DistanceLogic : ILogic
{
    public float Weight { get; set; }

    public double Calculate(Node node)
    {
        if (!node.ShouldCalculateConfidence) return 0.0;

        return node.Metrics.Distance.GetNewest();
    }
}
