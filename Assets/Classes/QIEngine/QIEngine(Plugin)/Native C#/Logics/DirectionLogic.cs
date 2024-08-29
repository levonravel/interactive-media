using System;
using System.Numerics;

public class DirectionLogic : ILogic
{
    private const float DefaultConeAngle = 45f;
    public float Weight { get; set; }
    private float coneAngle;

    public DirectionLogic(float coneAngle = DefaultConeAngle)
    {
        this.coneAngle = coneAngle;
    }

    public double Calculate(Node node)
    {
        // Get the newest and 4th newest positions to determine the movement direction
        var first = QIGlobalData.DuplicationFreeGazePositionSamples.GetNewest();
        var last = QIGlobalData.DuplicationFreeGazePositionSamples.GetNewest(4);

        // The position of the object of interest
        var objectPosition = node.Configuration.Position;

        // Calculate the direction vector and normalize it (input movement direction)
        var direction = Vector2.Normalize(first - last);

        // Calculate the dynamic cone center direction vector from the input position to the object and normalize it
        var coneCenterDirection = Vector2.Normalize(objectPosition - first);

        // Calculate the dot product between the dynamic cone center and the movement direction
        double dotProduct = Vector2.Dot(coneCenterDirection, direction);

        // Clamp the dot product to the valid range for Acos
        dotProduct = Math.Clamp(dotProduct, -1.0, 1.0);

        // The angle is derived from the arccosine of the dot product
        var angleRadians = Math.Acos(dotProduct);

        // Convert the angle to a range between 0 and 1
        var coneAngleRange = Math.PI / 4; // Assume a 45-degree cone for this example

        // Normalize the confidence score: 1 means aligned (moving towards the object), 0 means at the edge
        var confidence = Math.Clamp(1 - (angleRadians / coneAngleRange), 0.0, 1.0);

        UnityEngine.Debug.Log($"Direction: {confidence}");
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
