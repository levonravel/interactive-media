using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MoveNow : IGestureLogic
{
    public override void Invoke(GestureManager.GestureData guestureData, Transform obj, float sensitivity)
    {
        // Calculate the displacement in x, y, and z axes
        float deltaX = guestureData.CurrentPosition.x - guestureData.StartPosition.x;
        float deltaY = guestureData.CurrentPosition.y - guestureData.StartPosition.y;
        float deltaZ = guestureData.CurrentPosition.z - guestureData.StartPosition.z;

        // Calculate the movement direction based on the displacement
        Vector3 movementDirection = new Vector3(deltaX, deltaY, deltaZ).normalized;

        // Calculate the movement amount
        float moveAmount = Vector3.Distance(guestureData.CurrentPosition, guestureData.StartPosition) * sensitivity * Time.deltaTime;

        // Apply the movement to the object
        Vector3 movement = movementDirection * moveAmount;
        obj.position += movement;
    }
}
