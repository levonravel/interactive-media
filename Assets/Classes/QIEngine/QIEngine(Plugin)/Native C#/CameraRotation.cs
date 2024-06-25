using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    private static CameraRotation instance;
    public static CameraRotation Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<CameraRotation>();
            }
            return instance;
        }
    }

    public enum Direction
    {
        Right,
        Left,
        Forward
    }

    public UnityQINode Node;
    private float currentRotation = 0f; // Tracks the current rotation angle
    private Quaternion originalRotation; // Stores the original rotation
    private Vector3 originalPosition; // Stores the original position of the camera

    private void Start()
    {
        originalRotation = transform.rotation; // Store the original rotation at the start
        originalPosition = transform.position; // Store the original position at the start
    }

    public void Rotate(Direction direction, float speed)
    {
        if (direction == Direction.Forward)
        {
            // Smoothly return to the original rotation and position
            transform.rotation = Quaternion.Slerp(transform.rotation, originalRotation, speed * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, originalPosition, speed * Time.deltaTime);
            currentRotation = Mathf.Lerp(currentRotation, 0f, speed * Time.deltaTime);
            return;
        }

        float rotationAmount = speed * Time.deltaTime;
        if (direction == Direction.Left)
        {
            rotationAmount = -rotationAmount;
        }

        float newRotation = currentRotation + rotationAmount;
        if (newRotation > 90f)
        {
            rotationAmount = 90f - currentRotation;
            newRotation = 90f;
        }
        else if (newRotation < -90f)
        {
            rotationAmount = -90f - currentRotation;
            newRotation = -90f;
        }

        transform.RotateAround(Camera.main.transform.position + (Vector3.right * 5 + Vector3.forward * 5), Vector3.up, rotationAmount);
        currentRotation = newRotation;
    }
}
