using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Scale : IGestureLogic
{
    private bool isDiagonal;

    public Scale(bool scaleDiagonal)
    {
        isDiagonal = scaleDiagonal;
    }

    public override void Invoke(GestureManager.GestureData guestureData, Transform obj, float sensitivity)
    {
        // Calculate the guesture distance relative to the start distance
        float guestureDelta = Vector3.Distance(guestureData.CurrentPosition, guestureData.StartPosition);

        // Determine the scale factor based on guestureDelta and a sensitivity factor
        float scaleFactor = 1.0f + guestureDelta * sensitivity;

        //Scale object from all directions
        if (!isDiagonal)
        {
            // Apply the scale to the object
            obj.localScale *= scaleFactor;
        }
        else
        {
            // Calculate the pivot point (top right corner in local space)
            Vector3 pivotPoint = new Vector3(0.5f, 0.5f, 0.0f);

            // Calculate the position of the top right corner in world space
            Vector3 pivotWorldPosition = obj.TransformPoint(obj.localScale.x * pivotPoint.x, obj.localScale.y * pivotPoint.y, 0.0f);

            // Move the object to the pivot point
            obj.position = pivotWorldPosition;

            // Apply the scale to the object
            obj.localScale *= scaleFactor;

            // Move the object back to its original position
            obj.position -= obj.TransformVector(obj.localScale.x * pivotPoint.x, obj.localScale.y * pivotPoint.y, 0.0f);
        }
    }
}
