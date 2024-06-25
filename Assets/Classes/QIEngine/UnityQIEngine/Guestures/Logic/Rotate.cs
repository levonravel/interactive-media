using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Rotate : IGestureLogic
{
    public override void Invoke(GestureManager.GestureData guestureData, Transform obj, float sensitivity)
    {
        // left . right
        float deltaX = guestureData.CurrentPosition.x - guestureData.StartPosition.x;
        float rotationDirectionX = Mathf.Sign(deltaX);
        float rotationAmountX = rotationDirectionX * sensitivity * Time.deltaTime;

        // up . down
        float deltaY = guestureData.CurrentPosition.y - guestureData.StartPosition.y;
        float rotationDirectionY = Mathf.Sign(deltaY);
        float rotationAmountY = rotationDirectionY * sensitivity * Time.deltaTime;

        // Combine both rotation amounts
        Vector3 currentRotation = obj.rotation.eulerAngles;
        currentRotation.y += rotationAmountX;
        currentRotation.x += rotationAmountY;

        // Apply the combined rotation
        obj.rotation = Quaternion.Euler(currentRotation);
    }
}
