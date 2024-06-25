using UnityEngine;

public class DirectionMetric : IMetric
{
    public void Calculate(Node node, Vector3 inputPosition)
    {
        ConfidenceLine line = MagAngleBetween(node, QIGlobalData.DuplicationFreeGazePositionSamples);
        node.Metrics.Angle.Enqueue(line.Angle);
        node.Metrics.Magnitude.Enqueue(line.Mag);
    }

    static ConfidenceLine MagAngleBetween(Node node, RollingQueue<Vector3> inputPositions)
    {
        Vector3 center = node.Configuration.Position;
        Vector3 last = inputPositions.GetNewest(QIGlobalData.LineDistance - 1);
        Vector3 first = inputPositions.GetNewest();

        Vector3 AB = last - first;
        Vector3 AC = center - last;

        float dotProduct = Vector3.Dot(AB, AC);

        float magAB = Mathf.Sqrt(AB.x * AB.x + AB.y * AB.y + AB.z * AB.z);
        float magAC = Mathf.Sqrt(AC.x * AC.x + AC.y * AC.y + AC.z * AC.z);

        double angleRadians = Mathf.Acos(dotProduct / (magAB * magAC));

        float normalizedAngle = (float)(angleRadians / 3.14159265358979323846);

        float mag = 1 / (1 + Vector3.Distance(last, first));

        return new ConfidenceLine()
        {
            Angle = normalizedAngle,
            Mag = mag
        };
    }
};
