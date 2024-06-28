using QuantumInterface.QIEngine;

public class VelocityLogic : ILogic
{
    public CalcType CalcType => CalcType.Velocity;

    public void Calculate(Node node, float weight, bool isStateRunner)
    {
        if (!node.ShouldCalculateConfidence) return;

        double velocity = node.Metrics.Velocity.GetNewest();
        node.Confidence *= velocity * weight;

        if (isStateRunner)
        {
            StateHandler.SwitchState(node);
        }
    }
}
