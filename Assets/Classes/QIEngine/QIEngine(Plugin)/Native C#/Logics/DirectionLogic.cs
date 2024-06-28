using QuantumInterface.QIEngine;
using System;
using System.Collections.Generic;
using System.Numerics;
public class DirectionLogic : ILogic
{

    /**
 * @brief Sets the confidence of the node it was called on.
 *
 * CalculateConfidence adds a value to the node's confidence based on the latest value in the angle difference metrics for
 * the node. If this value is zero, then the weight parameter is added instead.
 * @param node The node that confidence is being updated for.
 * @param weight The node's confidence weight.
 * @param isStateRunner Whether or not this confidence logic is the final decider in the confidence
 * calculation
 */
    private float coneAngle = 45;
    private float coneLength = 5f;
    public CalcType CalcType => CalcType.Direction;

    public void Calculate(Node node, float weight, bool isStateRunner)
    {
        Vector2 first = QIGlobalData.DuplicationFreeGazePositionSamples.GetNewest();
        Vector2 last = QIGlobalData.DuplicationFreeGazePositionSamples.GetNewest(4);
        Vector2 center = node.Configuration.Position;

        Vector2 direction = (first - last).Normalize();
        float angleToCircle = Vector2Extensions.Angle(direction, (center - first).Normalize());

        if (angleToCircle > coneAngle / 2)
        {
            node.Confidence = 0f; // Circle is outside the cone angle
        }

        float distanceToCircle = Vector2.Distance(first, center);
        double intersectionArea = CalculateCircleSegmentArea(distanceToCircle, node.Configuration.Radius, coneAngle);
        double maxCircleArea = Math.PI * node.Configuration.Radius * node.Configuration.Radius;

        node.Confidence = (float)Math.Min(1, intersectionArea / maxCircleArea);

        /*
        if (IsGazeInsideSquare2D(node.Configuration.Position, first, node.Configuration.Dimensions) && node.Configuration.Radius == 0)
        {
            node.Confidence = 1;
        }
        else if (node.Configuration.Radius > 0 && IsInRadius(node.Configuration.Position, first, node.Configuration.Radius))
        {
            node.Confidence = 1;
        }
        else
        {
            //node.Confidence += (direction >= weight ? weight : direction) * (lookingAt ? 1 : 0);
        }
        */

        if (isStateRunner)
        {
            StateHandler.SwitchState(node);
        }
        
    }

    public static double CalculateCircleSegmentArea(float distance, float radius, float angle)
    {
        double theta = 2 * Math.Acos(distance / radius);
        double segmentArea = 0.5f * radius * radius * (theta - Math.Sin(theta));
        double coneSectorArea = 0.5f * angle * (distance * distance);

        return Math.Min(segmentArea, coneSectorArea);
    }

    static double CalculateSquare2DOffset(Vector2 squareCenter, Vector2 gazePos, Vector3 dimensions)
    {
        // Calculate the relative position of the gaze from the center of the rectangle
        Vector2 relativePos = gazePos - squareCenter;

        Vector2 topEdgeDir = Vector2.Normalize(new Vector2(0, dimensions.Y / 2.0f + squareCenter.Y) - gazePos);
        Vector2 bottomEdgeDir = Vector2.Normalize(new Vector2(0, -dimensions.Y / 2.0f + squareCenter.Y) - gazePos);
        Vector2 leftEdgeDir = Vector2.Normalize(new Vector2(-dimensions.X / 2.0f + squareCenter.X, 0) - gazePos);
        Vector2 rightEdgeDir = Vector2.Normalize(new Vector2(dimensions.X / 2.0f + squareCenter.X, 0) - gazePos);

        double topAngle = Math.Acos(Vector2.Dot(topEdgeDir, Vector2.Normalize(relativePos)));
        double bottomAngle = Math.Acos(Vector2.Dot(bottomEdgeDir, Vector2.Normalize(relativePos)));
        double leftAngle = Math.Acos(Vector2.Dot(leftEdgeDir, Vector2.Normalize(relativePos)));
        double rightAngle = Math.Acos(Vector2.Dot(rightEdgeDir, Vector2.Normalize(relativePos)));

        double minAngle = Math.Min(Math.Min(topAngle, bottomAngle), Math.Min(leftAngle, rightAngle));
        double distanceToNearestEdge;

        if (minAngle == topAngle || minAngle == bottomAngle)
        {
            distanceToNearestEdge = dimensions.Y / 2.0f;
        }
        else
        {
            distanceToNearestEdge = dimensions.X / 2.0f;
        }

        double distanceToCenter = relativePos.Magnitude();
        double offset = Math.Atan(distanceToNearestEdge / distanceToCenter);

        return offset;
    }

    //we need to also check if were inside the bounds if so we need to return true even though we might not be looking at it we need to ensure
    //that this is 1.
    static bool IsGazeInsideSquare2D(Vector2 squareCenter, Vector2 gazePos, Vector3 dimensions)
    {
        // Calculate the half dimensions of the rectangle
        double halfWidth = dimensions.X / 2.0f;
        double halfHeight = dimensions.Y / 2.0f;

        // Calculate the boundaries of the rectangle
        double leftBoundary = squareCenter.X - halfWidth;
        double rightBoundary = squareCenter.X + halfWidth;
        double topBoundary = squareCenter.Y + halfHeight;
        double bottomBoundary = squareCenter.Y - halfHeight;

        // Check if gazePos is inside the rectangle
        if (gazePos.X > leftBoundary && gazePos.X < rightBoundary && gazePos.Y > bottomBoundary && gazePos.Y < topBoundary)
        {
            return true; // inside the rectangle
        }
        return false; // outside the rectangle
    }

    static bool IsInRadius(Vector2 nodePosition, Vector2 inputPosition, float radius)
    {
        double distanceSquared = (inputPosition.X - nodePosition.X) * (inputPosition.X - nodePosition.X) +
            (inputPosition.Y - nodePosition.Y) * (inputPosition.Y - nodePosition.Y);
        return distanceSquared <= radius * radius;
    }
}