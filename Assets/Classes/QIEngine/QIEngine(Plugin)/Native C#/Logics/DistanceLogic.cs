using QuantumInterface.QIEngine;

public class DistanceLogic : ILogic
{
    public CalcType CalcType => CalcType.Distance;

    public void Calculate(Node node, float weight, bool isStateRunner)
    {
        if (!node.ShouldCalculateConfidence) return;

        //TODO Increment more proportionally -Levon "Ask at next meeting for ideas"
        double distance = node.Metrics.Distance.GetNewest() * weight;

        node.Confidence += distance >= weight ? weight : distance;

        if (isStateRunner)
        {
            StateHandler.SwitchState(node);
        }
    }
}
