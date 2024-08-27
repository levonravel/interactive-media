using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;

public static class QIMath
{
    public static double GetDistanceFromBounds(Node node, Vector2 inputPosition)
    {
        float distance = Vector2.Distance(node.Configuration.Position, inputPosition);
        float maxDistance = node.Configuration.StartConfidenceDistance == 0 ? 1080 : node.Configuration.StartConfidenceDistance;

        if (node.Configuration.Radius > 0)
        {
            distance -= node.Configuration.Radius;
            if (node.Parent != null)
            {
                var nodeOffsetRadius = node.Configuration.Radius;
                var parentOffsetRadius = node.Parent.Configuration.Radius;

                maxDistance = Vector2.Distance(node.Parent.Configuration.Position, node.Configuration.Position);
                var maxOffsetDistance = maxDistance - nodeOffsetRadius - parentOffsetRadius;

                distance = Vector2.Distance(node.Configuration.Position, inputPosition) - nodeOffsetRadius;
                var expDistance = 1 - (distance / maxOffsetDistance); //200, 90
                return expDistance;
            }
            else
            {
                if (distance > maxDistance) return 0;
                return distance == 0 ? 1 : Math.Exp(-distance / maxDistance);
            }
        }
 
        //2d cuboid
        if (node.Parent != null)
        {
            maxDistance = Calculate2DCuboidBoundingDistances(node, node.Parent);
            distance = Calculate2DCuboidDistance(node, inputPosition);
            return distance == 0 ? 1 : Math.Exp(-distance / maxDistance);
        }
        
        distance = Calculate2DCuboidDistance(node, inputPosition);
        if (distance > maxDistance) return 0;
        return distance == 0 ? 1 : Math.Exp(-distance / maxDistance);
    }

    private static float Calculate2DCuboidDistance(Node node, Vector2 inputPosition)
    {
        float halfWidth = node.Configuration.Dimensions.X / 2.0f;
        float halfHeight = node.Configuration.Dimensions.Y / 2.0f;

        // Calculate the distance from the point to the center of the square
        float deltaX = Math.Abs(inputPosition.X - node.Configuration.Position.X);
        float deltaY = Math.Abs(inputPosition.Y - node.Configuration.Position.Y);

        // Determine whether to compare with halfWidth or halfHeight
        return (deltaX > halfWidth || deltaY > halfHeight) ? Math.Max(deltaX - halfWidth, deltaY - halfHeight) : 0.0f;
    }

    private static float Calculate2DCuboidBoundingDistances(Node nodeA, Node nodeB)
    {
        float halfWidth1 = nodeA.Configuration.Dimensions.X / 2.0f;
        float halfHeight1 = nodeA.Configuration.Dimensions.Y / 2.0f;

        float halfWidth2 = nodeB.Configuration.Dimensions.X / 2.0f;
        float halfHeight2 = nodeB.Configuration.Dimensions.Y / 2.0f;

        // Calculate the distance between the centers of the two bounding boxes
        float deltaX = Math.Abs(nodeA.Configuration.Position.X - nodeB.Configuration.Position.X);
        float deltaY = Math.Abs(nodeA.Configuration.Position.Y - nodeB.Configuration.Position.Y);

        // Calculate the distance between the bounding boxes along the X and Y axes
        float distanceX = Math.Max(0, deltaX - halfWidth1 - halfWidth2);
        float distanceY = Math.Max(0, deltaY - halfHeight1 - halfHeight2);

        // Determine whether to compare with halfWidth or halfHeight
        return Math.Max(distanceX, distanceY);
    }
}
