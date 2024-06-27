using QuantumInterface.QIEngine;
using System;
using System.Numerics;

public class GazeDirectionLogic : ILogic
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
    private float elevenDegrees = 0.191986f;

    public CalcType CalcType => CalcType.InversedDirection;

    public void Calculate(Node node, float weight, bool isStateRunner)
    {
        Vector2 currentPosition = node.Configuration.Position;
        Vector2 lastPosition = node.Configuration.LastPosition;
        Vector2 inputPosition = QIGlobalData.DuplicationFreeGazePositionSamples.GetNewest();

        double offset = Math.Atan(node.Configuration.Radius / Vector2.Distance(inputPosition, currentPosition));

        if (node.Configuration.Radius == 0)
        {
            offset = CalculateSquare2DOffset(node.Configuration.Position, currentPosition, node.Configuration.Dimensions);
        }

        // Adjust lenientOffset calculation to correctly include the 11-degree margin on both sides.
        double lenientOffset = offset * elevenDegrees;

        double theta = Math.Atan2(inputPosition.Y - currentPosition.Y, inputPosition.X - currentPosition.X);
        double thetaToMouse = Math.Atan2(currentPosition.Y - lastPosition.Y, currentPosition.X - lastPosition.X);

        // Check if thetaToMouse is within the lenientOffset from theta.
        bool lookingAt = thetaToMouse >= theta - lenientOffset && thetaToMouse <= theta + lenientOffset;

        Vector2 AB = (currentPosition - lastPosition).Normalize();
        Vector2 AC = (inputPosition - currentPosition).Normalize();

        float direction = Vector2.Dot(AC, AB);

        direction = Math.Clamp(direction, 0, 1);
        //Debug.Log($"Input direction current {currentPosition} previous {lastPosition} direction {direction}");
        //Debug.Log($"current {currentPosition} previous {lastPosition} AB {AB} AC {AC} direction {direction}");

        if (IsGazeInsideSquare2D(node.Configuration.Position, currentPosition - inputPosition, node.Configuration.Dimensions) && node.Configuration.Radius == 0)
        {
            node.Confidence = 1;
        }
        else if (node.Configuration.Radius > 0 && IsInRadius(inputPosition, currentPosition, node.Configuration.Radius))
        {
            node.Confidence = 1;
        }
        else
        {
            node.Confidence += (direction >= weight ? weight : direction);// * (lookingAt ? 1 : 0);            
        }

        if (isStateRunner)
        {
            StateHandler.SwitchState(node);
        }
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
        float distanceToNearestEdge;

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
        float halfWidth = dimensions.X / 2.0f;
        float halfHeight = dimensions.Y / 2.0f;

        // Calculate the boundaries of the rectangle
        float leftBoundary = squareCenter.X - halfWidth;
        float rightBoundary = squareCenter.X + halfWidth;
        float topBoundary = squareCenter.Y + halfHeight;
        float bottomBoundary = squareCenter.Y - halfHeight;

        // Check if gazePos is inside the rectangle
        if (gazePos.X > leftBoundary && gazePos.X < rightBoundary && gazePos.Y > bottomBoundary && gazePos.Y < topBoundary)
        {
            return true; // inside the rectangle
        }
        return false; // outside the rectangle
    }

    static bool IsInRadius(Vector2 nodePosition, Vector2 inputPosition, float radius)
    {
        double distanceSquared = (inputPosition.X - nodePosition.Y) * (inputPosition.X - nodePosition.X) +
            (inputPosition.Y - nodePosition.Y) * (inputPosition.Y - nodePosition.Y);
        return distanceSquared <= radius * radius;
    }
}