#if QIENGINE_HOLOLENS
using QuantumInterface.QIEngine;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.XR.OpenXR.Features.Interactions;
using UnityEngine.XR;

public class HololensGazeModule : IGazeInput
{
    public override Vector3 InputPosition { get; set; }
    public override float InputSensitivity { get; set; }
    public override Transform CustomPointer { get; set; }

    private float smoothing = 0.2f;

    private static readonly List<InputDevice> InputDeviceList = new List<InputDevice>();
    private InputDevice eyeTrackingDevice = default(InputDevice);
    private List<Vector3> gazePositions = new List<Vector3>();
    private IMixedRealityEyeGazeProvider eyeGazeProvider;
    
    
    public override void Invoke()
    {
        if (!eyeTrackingDevice.isValid)
        {
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.EyeTracking, InputDeviceList);
            if (InputDeviceList.Count > 0)
                eyeTrackingDevice = InputDeviceList[0];
        
            if (!eyeTrackingDevice.isValid)
            {
                Debug.LogWarning($"Unable to acquire eye tracking device. Have permissions been granted?");
                return;
            }
        }
        
        // Gets gaze data from the device.
        bool hasData = eyeTrackingDevice.TryGetFeatureValue(CommonUsages.isTracked, out bool isTracked);
        hasData &= eyeTrackingDevice.TryGetFeatureValue(EyeTrackingUsages.gazePosition, out Vector3 position);
        hasData &= eyeTrackingDevice.TryGetFeatureValue(EyeTrackingUsages.gazeRotation, out Quaternion rotation);
        
        if (isTracked && hasData)
        {
            if(Physics.Raycast(position, Camera.main.transform.forward, out RaycastHit hitInfo, int.MaxValue))
            {
                var distanceFromCamera = hitInfo.transform.position.magnitude - position.magnitude;
                if (CustomPointer != null)
                {
                    CustomPointer.gameObject.SetActive(true);
                    CustomPointer.transform.position = position + rotation * new Vector3(0, 0, distanceFromCamera - .1f);
                }
            }
            else
            {
                if (CustomPointer != null)
                {
                    CustomPointer.gameObject.SetActive(false);
                    //CustomPointer.transform.position = position + rotation * new Vector3(0, 0, 0);   
                }
            }

            Vector3 worldToScreenPoint = Camera.main.WorldToScreenPoint(position + rotation * new Vector3(0, 0, 1.1f));
            gazePositions.Add(new Vector3(worldToScreenPoint.x, worldToScreenPoint.y, worldToScreenPoint.z));
            
            if (gazePositions.Count > 9)
                gazePositions.RemoveAt(0);
            
            InputPosition = ExponentialMovingAverage(gazePositions, new Vector3(worldToScreenPoint.x, worldToScreenPoint.y, worldToScreenPoint.z));
            
            var headPosition = Camera.main.WorldToScreenPoint(Camera.main.transform.position);
            QIEngineInterpreter.UpdateConfidence(InputPosition.x, InputPosition.y, headPosition.x, headPosition.y, headPosition.z);
        }
    }
    
    
    private Vector3 ExponentialMovingAverage(List<Vector3> buffer, Vector3 currentValue)
    {
        if (buffer.Count == 0)
            return currentValue;

        Vector3 smoothedValue = buffer[buffer.Count - 1];

        for (int i = buffer.Count - 2; i >= 0; i--)
            smoothedValue = Vector3.Lerp(smoothedValue, buffer[i], smoothing);
        
        smoothedValue = Vector3.Lerp(smoothedValue, currentValue, smoothing);

        return smoothedValue;
    }
}
#endif
