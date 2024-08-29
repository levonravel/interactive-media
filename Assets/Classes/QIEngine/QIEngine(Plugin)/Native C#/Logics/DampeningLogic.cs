using QuantumInterface.QIEngine;
using System;

public class DampeningLogic : ILogic
{
    public float Weight { get; set; } = 1.0f; // Default weight for the dampening logic
    private double smoothedConfidence = 0.5; // Initial smoothed confidence value
    public double SmoothingFactor { get; set; } = 0.1; // Default smoothing factor

    // Method to calculate the dampened confidence
    public double Calculate(Node node)
    {
        if (!node.ShouldCalculateConfidence) return 0.0;

        // Retrieve the last calculated confidence value from QIGlobalData
        double combinedConfidence = QIGlobalData.CombinedConfidence; // This should be updated by other logic calculations

        // Apply continuous smoothing for both increase and decrease
        smoothedConfidence = (SmoothingFactor * combinedConfidence) + ((1 - SmoothingFactor) * smoothedConfidence);

        // Clamp the smoothed confidence to ensure it remains between 0 and 1
        smoothedConfidence = Math.Clamp(smoothedConfidence, 0.0, 1.0);

        // Avoid very small values by treating anything less than a small threshold as zero
        if (smoothedConfidence < 0.001)
        {
            smoothedConfidence = 0.0;
        }

        // Update the global confidence for the next iteration
        QIGlobalData.CombinedConfidence = smoothedConfidence;

        return smoothedConfidence;
    }
}
