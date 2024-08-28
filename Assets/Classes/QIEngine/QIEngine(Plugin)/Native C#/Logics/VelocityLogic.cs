using QuantumInterface.QIEngine;

public class VelocityLogic : ILogic
{
    public float Weight { get; set; }

    public double Calculate(Node node)
    {
        if (!node.ShouldCalculateConfidence) return 0.0;

        double velocity = node.Metrics.Velocity.GetNewest();
        return velocity;
    }
}
