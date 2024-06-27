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

        double angleRadians = Math.Acos(dotProduct / (magAB * magAC));

        double normalizedAngle = angleRadians / 3.14159265358979323846;

        double mag = 1 / (1 + Vector2.Distance(last, first));

        return new ConfidenceLine()
        {
            Angle = normalizedAngle,
            Mag = mag
        };
    }
};
