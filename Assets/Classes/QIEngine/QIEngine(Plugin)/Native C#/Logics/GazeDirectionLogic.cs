using QuantumInterface.QIEngine;
using System.Collections.Generic;
using UnityEngine;

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

        float offset = Mathf.Atan(node.Configuration.Radius / Vector3.Distance(inputPosition, currentPosition));

        if (node.Configuration.Radius == 0)
        {
            offset = CalculateSquare2DOffset(node.Configuration.Position, currentPosition, node.Configuration.Dimensions);
        }

        // Adjust lenientOffset calculation to correctly include the 11-degree margin on both sides.
        float lenientOffset = offset * elevenDegrees;

        double theta = Mathf.Atan2(inputPosition.y - currentPosition.y, inputPosition.x - currentPosition.x);
        double thetaToMouse = Mathf.Atan2(currentPosition.y - lastPosition.y, currentPosition.x - lastPosition.x);

        // Check if thetaToMouse is within the lenientOffset from theta.
        bool lookingAt = thetaToMouse >= theta - lenientOffset && thetaToMouse <= theta + lenientOffset;

        Vector2 AB = (currentPosition - lastPosition).normalized;
        Vector2 AC = (inputPosition - currentPosition).normalized;

        float direction = Vector2.Dot(AC, AB);

        direction = Mathf.Clamp01(direction);
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


    static float CalculateSquare2DOffset(Vector2 squareCenter, Vector2 gazePos, Vector3 dimensions)
    {
        // Calculate the relative position of the gaze from the center of the rectangle
        Vector2 relativePos = gazePos - squareCenter;

        Vector2 topEdgeDir = Vector3.Normalize(new Vector2(0, dimensions.y / 2.0f + squareCenter.y) - gazePos);
        Vector2 bottomEdgeDir = Vector3.Normalize(new Vector2(0, -dimensions.y / 2.0f + squareCenter.y) - gazePos);
        Vector2 leftEdgeDir = Vector3.Normalize(new Vector2(-dimensions.x / 2.0f + squareCenter.x, 0) - gazePos);
        Vector2 rightEdgeDir = Vector3.Normalize(new Vector2(dimensions.x / 2.0f + squareCenter.x, 0) - gazePos);

        float topAngle = Mathf.Acos(Vector3.Dot(topEdgeDir, Vector3.Normalize(relativePos)));
        float bottomAngle = Mathf.Acos(Vector3.Dot(bottomEdgeDir, Vector3.Normalize(relativePos)));
        float leftAngle = Mathf.Acos(Vector3.Dot(leftEdgeDir, Vector3.Normalize(relativePos)));
        float rightAngle = Mathf.Acos(Vector3.Dot(rightEdgeDir, Vector3.Normalize(relativePos)));

        float minAngle = Mathf.Min(topAngle, bottomAngle, leftAngle, rightAngle);
        float distanceToNearestEdge;

        if (minAngle == topAngle || minAngle == bottomAngle)
        {
            distanceToNearestEdge = dimensions.y / 2.0f;
        }
        else
        {
            distanceToNearestEdge = dimensions.x / 2.0f;
        }

        float distanceToCenter = relativePos.magnitude;
        float offset = Mathf.Atan(distanceToNearestEdge / distanceToCenter);

        return offset;
    }

    //we need to also check if were inside the bounds if so we need to return true even though we might not be looking at it we need to ensure
    //that this is 1.
    static bool IsGazeInsideSquare2D(Vector2 squareCenter, Vector2 gazePos, Vector3 dimensions)
    {
        // Calculate the half dimensions of the rectangle
        float halfWidth = dimensions.x / 2.0f;
        float halfHeight = dimensions.y / 2.0f;

        // Calculate the boundaries of the rectangle
        float leftBoundary = squareCenter.x - halfWidth;
        float rightBoundary = squareCenter.x + halfWidth;
        float topBoundary = squareCenter.y + halfHeight;
        float bottomBoundary = squareCenter.y - halfHeight;

        // Check if gazePos is inside the rectangle
        if (gazePos.x > leftBoundary && gazePos.x < rightBoundary && gazePos.y > bottomBoundary && gazePos.y < topBoundary)
        {
            return true; // inside the rectangle
        }
        return false; // outside the rectangle
    }

    static bool IsInRadius(Vector3 nodePosition, Vector2 inputPosition, float radius)
    {
        double distanceSquared = (inputPosition.x - nodePosition.x) * (inputPosition.x - nodePosition.x) +
            (inputPosition.y - nodePosition.y) * (inputPosition.y - nodePosition.y);
        return distanceSquared <= radius * radius;
    }
}