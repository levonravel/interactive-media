using System;
using System.Collections.Generic;

public class ConfidenceCalculator
{
    // Method to calculate the combined confidence score with smoothing
    public double CalculateConfidence(Node node)
    {
        var calculatedConfidences = new List<double>();

        foreach (var logic in node.LogicCalculations)
        {
            var confidence = Math.Clamp(logic.Weight * logic.Calculate(node), 0, 1); // Apply bias, saves a loop

            if (confidence < 0 || confidence > 1)
            {
                throw new ArgumentException("All confidence values must be between 0 and 1.");
            }
            else
            {
                calculatedConfidences.Add(confidence);
            }
        }

        // Compute the weighted confidence score
        double weightedSum = 0;

        foreach (var weightedConfidence in calculatedConfidences)
        {
            weightedSum += weightedConfidence;
        }

        // Normalize the combined confidence to ensure it is between 0 and 1
        double totalLogicParameters = calculatedConfidences.Count;
        double combinedConfidence = weightedSum / totalLogicParameters;
        combinedConfidence = Math.Clamp(combinedConfidence, 0.0, 1.0);

        // Update the node's combined confidence metric for use in smoothing logic
        QIGlobalData.CombinedConfidence = combinedConfidence;

        return combinedConfidence;
    }
}
