using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityMetric : IMetric
{
    public void Calculate(Node node, Vector3 inputPosition)
    {
        // Get the two most recent magnitude values
        float magnitude1 = node.Metrics.Magnitude.GetNewest();
        float magnitude2 = node.Metrics.Magnitude.GetNewest(2);

        // Calculate the change in magnitude over time (velocity)
        float velocity = Mathf.Abs(magnitude2 - magnitude1); // Assuming velocity is always positive

        // Define a maximum possible velocity value (adjust as needed)
        float maxVelocity = 1.0f;

        // If velocity is 0, set normalizedVelocity to 1 to avoid division by zero
        float normalizedVelocity = (velocity == 0) ? 1.0f : velocity / maxVelocity;

        node.Metrics.Velocity.Enqueue(normalizedVelocity);
    }
}
