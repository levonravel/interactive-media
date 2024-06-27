using System;
using System.Numerics;

public class VelocityMetric : IMetric
{
    public void Calculate(Node node, Vector2 inputPosition)
    {
        // Get the two most recent magnitude values
        double magnitude1 = node.Metrics.Magnitude.GetNewest();
        double magnitude2 = node.Metrics.Magnitude.GetNewest(2);

        // Calculate the change in magnitude over time (velocity)
        double velocity = Math.Abs(magnitude2 - magnitude1); // Assuming velocity is always positive

        // Define a maximum possible velocity value (adjust as needed)
        double maxVelocity = 1.0f;

        // If velocity is 0, set normalizedVelocity to 1 to avoid division by zero
        double normalizedVelocity = (velocity == 0) ? 1.0f : velocity / maxVelocity;

        node.Metrics.Velocity.Enqueue(normalizedVelocity);
    }
}
