using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public static class QIMath
{
    public static float GetDistanceFromBounds(Node node, Vector3 inputPosition)
    {
        float distance = Vector3.Distance(node.Configuration.Position, inputPosition);
        float maxDistance = node.Configuration.StartConfidenceDistance == 0 ? 1080 : node.Configuration.StartConfidenceDistance;

        if (node.Configuration.Radius > 0)
        {
            distance -= node.Configuration.Radius;
            if (node.Parent != null)
            {
                maxDistance = Vector3.Distance(node.Configuration.Position, node.Parent.Configuration.Position);
                return distance == 0 ? 1 : Mathf.Exp(-distance / (maxDistance - node.Configuration.Radius));
            }
            else
            {
                if (distance > maxDistance) return 0;
                return distance == 0 ? 1 : Mathf.Exp(-distance / maxDistance);
            }
        }
 
        //2d cuboid
        if (node.Parent != null)
        {
            maxDistance = Calculate2DCuboidBoundingDistances(node, node.Parent);
            distance = Calculate2DCuboidDistance(node, inputPosition);
            return distance == 0 ? 1 : Mathf.Exp(-distance / maxDistance);
        }
        
        distance = Calculate2DCuboidDistance(node, inputPosition);
        if (distance > maxDistance) return 0;
        return distance == 0 ? 1 : Mathf.Exp(-distance / maxDistance);
    }

    private static float Calculate2DCuboidDistance(Node node, Vector3 inputPosition)
    {
        float halfWidth = node.Configuration.Dimensions.x / 2.0f;
        float halfHeight = node.Configuration.Dimensions.y / 2.0f;

        // Calculate the distance from the point to the center of the square
        float deltaX = Mathf.Abs(inputPosition.x - node.Configuration.Position.x);
        float deltaY = Mathf.Abs(inputPosition.y - node.Configuration.Position.y);

        // Determine whether to compare with halfWidth or halfHeight
        return (deltaX > halfWidth || deltaY > halfHeight) ? Mathf.Max(deltaX - halfWidth, deltaY - halfHeight) : 0.0f;
    }

    private static float Calculate2DCuboidBoundingDistances(Node nodeA, Node nodeB)
    {
        float halfWidth1 = nodeA.Configuration.Dimensions.x / 2.0f;
        float halfHeight1 = nodeA.Configuration.Dimensions.y / 2.0f;

        float halfWidth2 = nodeB.Configuration.Dimensions.x / 2.0f;
        float halfHeight2 = nodeB.Configuration.Dimensions.y / 2.0f;

        // Calculate the distance between the centers of the two bounding boxes
        float deltaX = Mathf.Abs(nodeA.Configuration.Position.x - nodeB.Configuration.Position.x);
        float deltaY = Mathf.Abs(nodeA.Configuration.Position.y - nodeB.Configuration.Position.y);

        // Calculate the distance between the bounding boxes along the X and Y axes
        float distanceX = Mathf.Max(0, deltaX - halfWidth1 - halfWidth2);
        float distanceY = Mathf.Max(0, deltaY - halfHeight1 - halfHeight2);

        // Determine whether to compare with halfWidth or halfHeight
        return Mathf.Max(distanceX, distanceY);
    }
}
