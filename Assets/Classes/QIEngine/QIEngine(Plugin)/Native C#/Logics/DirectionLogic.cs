using System;
using System.Numerics;

public class DirectionLogic : ILogic
{
    private const float DefaultConeAngle = 22f;
    public float Weight { get; set; }
    private float coneAngleDegrees = 11;

    public double Calculate(Node node)
    {
        var first = QIGlobalData.DuplicationFreeGazePositionSamples.GetNewest();
        var last = QIGlobalData.DuplicationFreeGazePositionSamples.GetNewest(4);
        var objectPosition = node.Configuration.Position;
        var direction = Vector2.Normalize(first - last);
        var coneCenterDirection = Vector2.Normalize(objectPosition - first);
        double dotProduct = Vector2.Dot(coneCenterDirection, direction);

        dotProduct = Math.Clamp(dotProduct, -1.0, 1.0);

        var angleRadians = Math.Acos(dotProduct);
        var coneAngleRange = coneAngleDegrees * (Math.PI / 180);
        var confidence = Math.Clamp(1 - (angleRadians / coneAngleRange), 0.0, 1.0);

        return confidence;
    }



    private static double CalculateCircleSegmentArea(float distance, float radius, float angle)
    {
        if (distance >= radius) return 0; // If the distance is beyond the circle, the area is zero.

        double theta = 2 * Math.Acos(distance / radius);
        double segmentArea = 0.5 * radius * radius * (theta - Math.Sin(theta));
        double coneSectorArea = 0.5 * angle * Math.Pow(distance, 2);

        return Math.Min(segmentArea, coneSectorArea);
    }

    private static double CalculateSquare2DOffset(Vector2 squareCenter, Vector2 gazePos, Vector3 dimensions)
    {
        Vector2 relativePos = gazePos - squareCenter;

        double minAngle = GetMinimumAngleToEdges(squareCenter, gazePos, dimensions);
        double distanceToNearestEdge = (minAngle == GetAngleToEdge(squareCenter, gazePos, new Vector2(0, dimensions.Y / 2.0f)) ||
                                        minAngle == GetAngleToEdge(squareCenter, gazePos, new Vector2(0, -dimensions.Y / 2.0f))) ?
                                        dimensions.Y / 2.0f : dimensions.X / 2.0f;

        double distanceToCenter = relativePos.Length();
        return Math.Atan(distanceToNearestEdge / distanceToCenter);
    }

    private static double GetMinimumAngleToEdges(Vector2 squareCenter, Vector2 gazePos, Vector3 dimensions)
    {
        double topAngle = GetAngleToEdge(squareCenter, gazePos, new Vector2(0, dimensions.Y / 2.0f));
        double bottomAngle = GetAngleToEdge(squareCenter, gazePos, new Vector2(0, -dimensions.Y / 2.0f));
        double leftAngle = GetAngleToEdge(squareCenter, gazePos, new Vector2(-dimensions.X / 2.0f, 0));
        double rightAngle = GetAngleToEdge(squareCenter, gazePos, new Vector2(dimensions.X / 2.0f, 0));

        return Math.Min(Math.Min(topAngle, bottomAngle), Math.Min(leftAngle, rightAngle));
    }

    private static double GetAngleToEdge(Vector2 squareCenter, Vector2 gazePos, Vector2 edgePoint)
    {
        Vector2 edgeDir = Vector2.Normalize(edgePoint - gazePos);
        Vector2 relativePos = gazePos - squareCenter;
        return Math.Acos(Vector2.Dot(edgeDir, Vector2.Normalize(relativePos)));
    }

    private static bool IsGazeInsideSquare2D(Vector2 squareCenter, Vector2 gazePos, Vector3 dimensions)
    {
        double halfWidth = dimensions.X / 2.0f;
        double halfHeight = dimensions.Y / 2.0f;

        return (gazePos.X > squareCenter.X - halfWidth && gazePos.X < squareCenter.X + halfWidth &&
                gazePos.Y > squareCenter.Y - halfHeight && gazePos.Y < squareCenter.Y + halfHeight);
    }

    private static bool IsInRadius(Vector2 nodePosition, Vector2 inputPosition, float radius)
    {
        double distanceSquared = Vector2.DistanceSquared(nodePosition, inputPosition);
        return distanceSquared <= radius * radius;
    }
}
