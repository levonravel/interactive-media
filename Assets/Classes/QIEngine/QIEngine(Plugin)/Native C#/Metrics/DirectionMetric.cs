using System;
using System.Numerics;

public class DirectionMetric : IMetric
{
    public void Calculate(Node node, Vector2 inputPosition)
    {
        ConfidenceLine line = MagAngleBetween(node, QIGlobalData.DuplicationFreeGazePositionSamples);
        node.Metrics.Angle.Enqueue(line.Angle);
        node.Metrics.Magnitude.Enqueue(line.Mag);
    }

    static ConfidenceLine MagAngleBetween(Node node, RollingQueue<Vector2> inputPositions)
    {
        Vector2 center = node.Configuration.Position;
        Vector2 last = inputPositions.GetNewest(QIGlobalData.LineDistance - 1);
        Vector2 first = inputPositions.GetNewest();

        Vector2 AB = last - first;
        Vector2 AC = center - last;

        double dotProduct = Vector2.Dot(AB, AC);

        double magAB = Math.Sqrt(AB.X * AB.X + AB.Y * AB.Y);
        double magAC = Math.Sqrt(AC.X * AC.X + AC.Y * AC.Y);

        double angleRadians = 0;
        if (magAB > 0 && magAC > 0)
        {
            double cosTheta = dotProduct / (magAB * magAC);
            cosTheta = Math.Clamp(cosTheta, -1.0, 1.0); // Clamp to valid range for Acos
            angleRadians = Math.Acos(cosTheta);
        }

        double normalizedAngle = angleRadians / Math.PI;

        double mag = Math.Clamp(1 / (1 + Vector2.Distance(last, first)), 0.0, 1.0);

        return new ConfidenceLine()
        {
            Angle = normalizedAngle,
            Mag = mag
        };
    }
}
